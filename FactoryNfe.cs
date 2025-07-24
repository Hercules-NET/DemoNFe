using DFe.Classes.Flags;
using NFe.Classes.Informacoes;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Classes.Informacoes.Pagamento;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;

namespace HerculesZeusDfeDemo
{
    public class FactoryNfe
    {
        public static NFe.Classes.NFe Gerar(int serie, int nro)
        {
            var nf = new NFe.Classes.NFe
            {
                infNFe = new infNFe
                {
                    versao = "4.00"
                }
            };

            //ide
            nf.infNFe.ide = new ide
            {
                cUF = DFe.Classes.Entidades.Estado.SP,
                cNF = new Random().Next(10000000, 99999999).ToString(),
                natOp = "Venda de mercadoria adquirida ou recebida de terceiros",
                mod = ModeloDocumento.NFe,
                serie = serie,
                nNF = nro,
                dhEmi = new DateTimeOffset(DateTime.UtcNow).ToOffset(new TimeSpan(-3, 0, 0)),
                dhSaiEnt = new DateTimeOffset(DateTime.UtcNow).ToOffset(new TimeSpan(-3, 0, 0)),
                tpNF = TipoNFe.tnSaida,
                idDest = DestinoOperacao.doInterna,
                cMunFG = 3529005,//código ibge marilia/sp
                tpImp = TipoImpressao.tiRetrato,
                tpEmis = TipoEmissao.teNormal,
                tpAmb = TipoAmbiente.Homologacao,
                finNFe = FinalidadeNFe.fnNormal,
                indFinal = ConsumidorFinal.cfConsumidorFinal,
                indPres = PresencaComprador.pcPresencial,
                procEmi = ProcessoEmissao.peAplicativoContribuinte,
                verProc = "Hercules Dfe",
            };

            //emit
            nf.infNFe.emit = new emit
            {
                CNPJ = "62559695000101",
                xNome = "BREDA'S INFORMATICA LTDA",
                xFant = "BREDAS SISTEMAS",
                enderEmit = new enderEmit
                {
                    xLgr = "AVENIDA PEDRO DE TOLEDO",
                    nro = "120",
                    xCpl = "",
                    xBairro = "PALMITAL",
                    cMun = 3529005,
                    xMun = "MARILIA",
                    UF = DFe.Classes.Entidades.Estado.SP,
                    CEP = "17509020",
                    cPais = 1058,
                    xPais = "BRASIL"
                },
                IE = "438405242115",
                CRT = CRT.SimplesNacional
            };

            //dest
            nf.infNFe.dest = new dest(VersaoServico.Versao400)
            {
                CNPJ = "10331198000140",
                enderDest = new enderDest
                {
                    xLgr = "RUA LIMEIRA",
                    nro = "62",
                    xCpl = "",
                    xBairro = "PALMITAL",
                    cMun = 3529005,
                    xMun = "MARILIA",
                    UF = "SP",
                    CEP = "17509020",
                    cPais = 1058,
                    xPais = "BRASIL"
                },
                indIEDest = indIEDest.NaoContribuinte,
                //IE = "956224310481",
                xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL"
            };

            //transp
            nf.infNFe.transp = new transp
            {
                modFrete = ModalidadeFrete.mfSemFrete,
                vol = new List<vol>
                {
                    new vol
                    {
                        qVol = 1,
                        esp = "VOLUME(S)"
                    }
                }
            };

            //det
            nf.infNFe.det.Add(new NFe.Classes.Informacoes.Detalhe.det()
            {
                nItem = 1,
                prod = new NFe.Classes.Informacoes.Detalhe.prod()
                {
                    cProd = "10",
                    cEAN = "7896060010492",
                    cEANTrib = "7896060010492",
                    xProd = "ÁGUA MINERAL",
                    NCM = "22021000",
                    CEST = "0300700",
                    CFOP = 5102,
                    uCom = "UN",
                    uTrib = "UN",
                    qCom = 1,//quantidade
                    qTrib = 1,
                    vProd = 10,//valor produto R$ 10,00
                    vUnCom = 10,
                    vUnTrib = 10,
                    indTot = NFe.Classes.Informacoes.Detalhe.IndicadorTotal.ValorDoItemCompoeTotalNF
                },
                imposto = new NFe.Classes.Informacoes.Detalhe.Tributacao.imposto()
                {
                    vTotTrib = 9.07m, //Imposto calculado por IBPT
                    ICMS = new NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.ICMS
                    {
                        TipoICMS = new ICMSSN102() //NCM 102
                        {
                            CSOSN = NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos.Csosnicms.Csosn102,
                            orig = NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos.OrigemMercadoria.OmNacional
                        }
                    },
                    PIS = new NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.PIS()
                    {
                        TipoPIS = new PISNT()
                        {
                            CST = NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos.CSTPIS.pis08
                        }
                    },
                    COFINS = new COFINS()
                    {
                        TipoCOFINS = new COFINSNT()
                        {
                            CST = NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos.CSTCOFINS.cofins08
                        }
                    }
                }
            });

            //pag
            nf.infNFe.pag = new List<pag>
            {
                new pag
                {
                    detPag = new List<detPag>()
                    {
                        new detPag()
                        {
                            indPag = IndicadorPagamentoDetalhePagamento.ipDetPgVista,
                            tPag = FormaPagamento.fpDinheiro,
                            vPag = 10,//valor total da forma pagamento em dinheiro
                        }
                    }
                }
            };

            //total
            nf.infNFe.total = new total
            {
                ICMSTot = new ICMSTot
                {
                    vBC = 0,
                    vICMS = 0,
                    vICMSDeson = 0,
                    vFCP = 0,
                    vBCST = 0,
                    vST = 0,
                    vFCPST = 0,
                    vFCPSTRet = 0,
                    vIPI = 0,
                    vIPIDevol = 0,
                    vPIS = 0,
                    vCOFINS = 0,
                    vProd = 10,
                    vFrete = 0,
                    vSeg = 0,
                    vOutro = 0,
                    vDesc = 0,
                    vII = 0,
                    vTotTrib = 9.07m,//IBPT
                    vNF = 10//valor total da nota
                }
            };

            //observacao
            nf.infNFe.infAdic = new NFe.Classes.Informacoes.Observacoes.infAdic()
            {
                infCpl = "TESTE DE OBSERVAÇÃO"
            };

            //reponsavel tecnico
            nf.infNFe.infRespTec = new Shared.NFe.Classes.Informacoes.InfRespTec.infRespTec()
            {
                CNPJ = "62559695000101",
                xContato = "Danilo Breda",
                email = "bredas@bredas.com.br",
                fone = "1434133244",
                idCSRT = null,
                hashCSRT = null
            };

            return nf;
        }

    }
}
