// See https://aka.ms/new-console-template for more information
using DFe.Utils;
using HerculesZeusDfeDemo;
using NFe.Classes;
using NFe.Classes.Servicos.Tipos;
using NFe.Servicos;
using NFe.Servicos.Retorno;
using NFe.Utils;
using NFe.Utils.NFe;
using NFe.Utils.Validacao;
using System.Net;

//diretorio de schemas
var diretorioSchema = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)!, "Schemas");

//protocolo comunicacao
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

//criando configuração
var configuracao = new ConfiguracaoServico()
{
    ValidarCertificadoDoServidor = false,
    DiretorioSalvarXml = "", //nao salvar xmls
    SalvarXmlServicos = false,
    ValidarSchemas = false, //hoje e feito manualmente
    DiretorioSchemas = diretorioSchema,
    ProtocoloDeSeguranca = ServicePointManager.SecurityProtocol,
    RemoverAcentos = true,
    DefineVersaoServicosAutomaticamente = true,
    VersaoLayout = DFe.Classes.Flags.VersaoServico.Versao400,
    ModeloDocumento = DFe.Classes.Flags.ModeloDocumento.NFe,
    tpEmis = NFe.Classes.Informacoes.Identificacao.Tipos.TipoEmissao.teNormal,
    tpAmb = DFe.Classes.Flags.TipoAmbiente.Homologacao,
    cUF = DFe.Classes.Entidades.Estado.SP,
    TimeOut = 20000,
    Certificado = new DFe.Utils.ConfiguracaoCertificado()
    {
        TipoCertificado = DFe.Utils.TipoCertificado.A1ByteArray,
        ArrayBytesArquivo = File.ReadAllBytes("C:/PROJETOS/cert_BREDAS_venc_012025_senha_12345678.pfx"),
        Senha = "12345678",
        ManterDadosEmCache = false,
        SignatureMethodSignedXml = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
        DigestMethodReference = "http://www.w3.org/2000/09/xmldsig#sha1"
    }
};

//gerando nfe
var nfe = FactoryNfe.Gerar(serie: 32, nro: 101);
var codigoNumero = nfe.infNFe.ide.cNF;
Console.WriteLine("Codigo NFE: " + codigoNumero);
var idLote = Convert.ToInt32(nfe.infNFe.ide.nNF);//idlote é o mesmo que o numero da nf

//assinando nfe
nfe.Assina(configuracao);
var protocolo = nfe.infNFe.Id.ToUpper().Replace("NFE", "");

//validando xml
Validador.Valida(ServicoNFe.NFeAutorizacao, configuracao.VersaoNFeAutorizacao, FuncoesXml.ClasseParaXmlString(nfe), false, configuracao.DiretorioSchemas);

Helpers.AbrirXml(nfe.ObterXmlString());//abrindo para visualizacao

//xml gerado localmente
Console.WriteLine("XML gerado e assinado localmente, pressione qualquer tecla para enviar para sefaz");
Console.ReadKey();

//enviando lote para api do sefaz
string recibo = "";
string xmlEnvio = "";
try
{
    using (var servicoNFe = new ServicosNFe(configuracao))
    {
        RetornoNFeAutorizacao saida = servicoNFe.NFeAutorizacao(idLote, IndicadorSincronizacao.Assincrono, new List<NFe.Classes.NFe> { nfe }, compactarMensagem: false);

        var dto = new
        {
            //dados retorno
            EnvioStr = saida.EnvioStr,
            Status = saida.Retorno.cStat,
            Motivo = saida.Retorno.xMotivo,
            NumeroRecibo = saida.Retorno.infRec?.nRec ?? string.Empty,
            NumeroProtocolo = protocolo,
            DataHoraEstimativaProcessado = DateTimeOffset.UtcNow.AddMilliseconds(saida.Retorno?.infRec?.tMed ?? 0),

            //dados complementares
            ModeloNfe = (int)nfe.infNFe.ide.mod,
            CodigoNumericoNfe = nfe.infNFe.ide.cNF,
            IdentificadorDestinoNfe = (int?)nfe.infNFe.ide.idDest ?? throw new Exception("nfe.indfNFe.ide.idDest nulo, favor verificar programador"),
            FormatoImpressaoDanfeNfe = (int)nfe.infNFe.ide.tpImp,
            TipoEmissaoNfe = (int)nfe.infNFe.ide.tpEmis,
            DigitoVerificadorChaveAcessoNfe = nfe.infNFe.ide.cDV,
            TipoAmbienteNfe = (int)nfe.infNFe.ide.tpAmb,
            FinalidadeNfe = (int)nfe.infNFe.ide.finNFe,
            ProcessoEmissaoNfe = (int)nfe.infNFe.ide.procEmi
        };
        Console.WriteLine("Recibo: " + dto.NumeroRecibo);
        recibo = dto.NumeroRecibo;
        xmlEnvio = dto.EnvioStr;
        Helpers.AbrirXml(xmlEnvio);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ocorreu um erro geral ao Emitir NFE {ex.Message}");
    throw;
}

//consulta de recibo de lote enviado
Console.WriteLine("Lote enviado com sucesso, pressione qualquer tecla para consultar recibo");
Console.ReadKey();

//verificando lote na api do sefaz
using (var servicoNFe = new ServicosNFe(configuracao))
{
    var retornoRecibo = servicoNFe.NFeRetAutorizacao(recibo);

    if (NfeSituacao.LoteRecebido(retornoRecibo.Retorno.cStat))//porem nao precessado ainda
    {
        Console.WriteLine($"Lote ainda não foi precessado pela SEFAZ | {retornoRecibo.Retorno.cStat} | {retornoRecibo.Retorno.xMotivo}");
        return -1;
    }
    else if (NfeSituacao.LoteProcessado(retornoRecibo.Retorno.cStat))
    {
        //processo de processado
        var protNFe = retornoRecibo.Retorno.protNFe.FirstOrDefault();
        var infProt = protNFe.infProt;

        if (NfeSituacao.Autorizada(infProt.cStat))
        {
            //autorizado uso nfe
            var nfeproc = new nfeProc
            {
                NFe = new NFe.Classes.NFe().CarregarDeXmlString(xmlEnvio),
                protNFe = protNFe,
                versao = protNFe.versao
            };

            var xmlretorno = nfeproc.ObterXmlString();//XML

            Console.WriteLine($"Nota Fiscal Emitida com Sucesso!: ({retornoRecibo.Retorno.cStat}) {retornoRecibo.Retorno.xMotivo}");

            //abrindo xml
            Helpers.AbrirXml(xmlretorno);
            return 1;
        }
        else
        {
            Console.WriteLine($"Retorno erro recibo: ({retornoRecibo.Retorno.cStat}) {retornoRecibo.Retorno.xMotivo}");
            return -1;
        }
    }
    else if (retornoRecibo.Retorno.cStat == 656) //consumo indevido
    {
        Console.WriteLine($"Sefaz com problemas de acesso multiplos indevidos, contate o suporte!");
        return -1;
    }
    else if (retornoRecibo.Retorno.cStat == 225) //falha de schema
    {
        Console.WriteLine($"Nota Rejeitada ({retornoRecibo.Retorno.cStat}) {retornoRecibo.Retorno.xMotivo}");
        return -1;
    }
    else
    {
        Console.WriteLine($"A pesquisa de lote voltou com um erro não mapeado (1): {retornoRecibo.Retorno.cStat} | {retornoRecibo.Retorno.xMotivo}");
        return -1;
    }
}
