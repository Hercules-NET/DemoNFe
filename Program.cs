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
        ArrayBytesArquivo = File.ReadAllBytes("C:/PROJETOS/cert_BREDAS_venc_012026_senha_12345678.pfx"),
        Senha = "12345678",
        ManterDadosEmCache = false,
        SignatureMethodSignedXml = "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
        DigestMethodReference = "http://www.w3.org/2000/09/xmldsig#sha1"
    }
};

//gerando nfe
var nfe = FactoryNfe.Gerar(serie: 32, nro: 112);
var codigoNumero = nfe.infNFe.ide.cNF;
Console.WriteLine("Codigo NFE: " + codigoNumero);
var idLote = Convert.ToInt32(nfe.infNFe.ide.nNF);//idlote é o mesmo que o numero da nf

Helpers.AbrirXml(nfe.ObterXmlString());//abrindo para visualizacao (sem assinatura)

//assinando nfe
nfe.Assina(configuracao);
var chaveAcesso = nfe.infNFe.Id.ToUpper().Replace("NFE", "");

//validando xml
Validador.Valida(ServicoNFe.NFeAutorizacao, configuracao.VersaoNFeAutorizacao, FuncoesXml.ClasseParaXmlString(nfe), false, configuracao.DiretorioSchemas);

Helpers.AbrirXml(nfe.ObterXmlString());//abrindo para visualizacao (com assinatura)

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
        RetornoNFeAutorizacao retornoSefaz = servicoNFe.NFeAutorizacao(idLote, IndicadorSincronizacao.Sincrono, new List<NFe.Classes.NFe> { nfe }, compactarMensagem: false);

        var dto = new
        {
            // Dados de Retorno
            EnvioStr = retornoSefaz.EnvioStr, // String XML enviada à Sefaz, salve como log para auditoria
            Status = retornoSefaz.Retorno.cStat, // Código do status retornado pela Sefaz, indica o resultado do processamento (ex: 100 = Autorizado)
            Motivo = retornoSefaz.Retorno.xMotivo, // Mensagem descritiva do status, detalha o motivo do processamento (ex: "Autorizado o uso da NF-e")

            NumeroProtocolo = retornoSefaz.Retorno.protNFe.infProt.nProt, /*
            O número de protocolo (nProt) é gerado pela Sefaz após a autorização da NF-e.
            Na emissão síncrona, ele é fundamental porque indica que a nota foi efetivamente autorizada.
            Ele é usado para:
                - Comprovar a validade jurídica da NF-e (junto ao Fisco e clientes).
                - Realizar operações posteriores: cancelamento, carta de correção, consulta de status, etc.
                - Armazenamento e auditoria: vincula o evento ao documento fiscal autorizado.
            */

            NumeroChaveAcesso = chaveAcesso, /*
            A chave de acesso é o identificador único da NF-e.
            Ela é usada para:
                - Consultar a nota fiscal na Sefaz.
                - Gerar DANFE (representação gráfica).
                - Cancelar, inutilizar ou corrigir a nota fiscal.
                - Referenciar a NF-e em outros documentos fiscais.
            */

            // Dados complementares referentes à NF-e emitida
            ModeloNfe = (int)nfe.infNFe.ide.mod, // Modelo do documento fiscal (ex: 55 = NF-e, 65 = NFC-e)
            CodigoNumericoNfe = nfe.infNFe.ide.cNF, // Código numérico gerado para compor a chave de acesso
            IdentificadorDestinoNfe = (int?)nfe.infNFe.ide.idDest ?? throw new Exception("nfe.infNFe.ide.idDest nulo, favor verificar programador"), // Identifica a localização do destinatário (1 = operação interna, 2 = interestadual, 3 = exterior)
            FormatoImpressaoDanfeNfe = (int)nfe.infNFe.ide.tpImp, // Formato de impressão do DANFE (ex: 1 = Retrato, 2 = Paisagem)
            TipoEmissaoNfe = (int)nfe.infNFe.ide.tpEmis, // Tipo de emissão da NF-e (1 = normal, 2 = contingência FS, 3 = SCAN, etc.)
            DigitoVerificadorChaveAcessoNfe = nfe.infNFe.ide.cDV, // Dígito verificador da chave de acesso, usado para validar se a chave está correta
            TipoAmbienteNfe = (int)nfe.infNFe.ide.tpAmb, // Identifica o ambiente de emissão (1 = produção, 2 = homologação)
            FinalidadeNfe = (int)nfe.infNFe.ide.finNFe, // Finalidade da emissão (1 = normal, 2 = complementar, 3 = ajuste, 4 = devolução)
            ProcessoEmissaoNfe = (int)nfe.infNFe.ide.procEmi // Processo de emissão utilizado (0 = aplicativo do contribuinte, 1 = avulsa Sefaz, 2 = Sefaz, 3 = terceiros)
        };
        xmlEnvio = dto.EnvioStr;

        //Verificando o status de retorno
        var protNFe = retornoSefaz.Retorno.protNFe;
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

            var xmlFinal = nfeproc.ObterXmlString();
            Console.WriteLine($"Nota Fiscal Emitida com Sucesso!");
            //abrindo xml
            Helpers.AbrirXml(xmlFinal);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ocorreu um erro geral ao Emitir NFE {ex.Message}");
    throw;
}

return 0;