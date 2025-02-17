using Microsoft.Office.Interop.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebGrease.Css.Ast;
using static Microsoft.Exchange.WebServices.Data.SearchFilter;

namespace WebApplication1.Controllers
{
    public class ComissionController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProcessosVendas")]
        public List<BB_PROPOSALS_GET_V1> GetProcessosVendas([FromBody] UserName1 Owner)
        {
            //AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            List<BB_PROPOSALS_GET_V1> lst = new List<BB_PROPOSALS_GET_V1>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_Proposal_Resumo", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userEmail", Owner.username);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        try
                        {
                            BB_PROPOSALS_GET_V1 m = new BB_PROPOSALS_GET_V1();
                            m.ID = (int)rdr["ID"];
                            m.AccountNumber = rdr["NCliente"].ToString();
                            m.ClientName = rdr["Cliente"].ToString();
                            m.Name = rdr["Name"].ToString();
                            //m.Description = rdr["Description"].ToString();
                            m.TotalValue = rdr["valor"].ToString() != "" ? (double.Parse(rdr["valor"].ToString())) : 0;
                            m.QuoteCRM = rdr["QuoteCRM"].ToString();
                            m.ModifiedTime = rdr["UltimaModificacao"] != null ? DateTime.Parse(rdr["UltimaModificacao"].ToString()) : new DateTime();
                            m.CreatedTime = rdr["CreatedTime"] != null ? DateTime.Parse(rdr["CreatedTime"].ToString()) : new DateTime();
                            //m.Status = new BB_Proposal_Status
                            //{
                            //    Description = rdr["Estado"].ToString(),
                            //    Phase = (int)rdr["Phase"],
                            //    ID = (int)rdr["EstadoID"],
                            //};
                            m.FinancingStatus = rdr["Financeiro"].ToString();
                            m.ServiceStatus = rdr["Servico"].ToString();
                            m.LeaseDeskStatus = rdr["PosVenda"].ToString();
                            m.CreatedByEmail = rdr["CreatedByEmail"].ToString();
                            m.CreatedByName = rdr["CreatedByName"].ToString();
                            m.AccountManagerName = rdr["AccountManagerName"].ToString();
                            m.AccountManagerEmail = rdr["AccountManagerEmail"].ToString();
                            m.ModifiedByEmail = rdr["ModifiedByEmail"].ToString();
                            m.ModifiedByName = rdr["ModifiedByName"].ToString();
                            lst.Add(m);

                            if ((int)rdr["ID"] > 2000)
                            {
                                var test = m;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }


            return lst;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProcessosVendasShared")]
        public List<BB_PROPOSALS_GET_V1> GetProcessosVendasShared([FromBody] UserName1 Owner)
        {
            //AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            List<BB_PROPOSALS_GET_V1> lst = new List<BB_PROPOSALS_GET_V1>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_proposals_by_Shared", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userEmail", Owner.username);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        try
                        {
                            BB_PROPOSALS_GET_V1 m = new BB_PROPOSALS_GET_V1();
                            m.ID = (int)rdr["ID"];
                            m.AccountNumber = rdr["NCliente"].ToString();
                            m.ClientName = rdr["Cliente"].ToString();
                            m.Name = rdr["Name"].ToString();
                            //m.Description = rdr["Description"].ToString();
                            m.TotalValue = rdr["valor"].ToString() != "" ? (double.Parse(rdr["valor"].ToString())) : 0;
                            m.QuoteCRM = rdr["QuoteCRM"].ToString();
                            m.ModifiedTime = rdr["UltimaModificacao"] != null ? DateTime.Parse(rdr["UltimaModificacao"].ToString()) : new DateTime();
                            m.CreatedTime = rdr["CreatedTime"] != null ? DateTime.Parse(rdr["CreatedTime"].ToString()) : new DateTime();
                            //m.Status = new BB_Proposal_Status
                            //{
                            //    Description = rdr["Estado"].ToString(),
                            //    Phase = (int)rdr["Phase"],
                            //    ID = (int)rdr["EstadoID"],
                            //};
                            m.FinancingStatus = rdr["Financeiro"].ToString();
                            m.ServiceStatus = rdr["Servico"].ToString();
                            m.LeaseDeskStatus = rdr["PosVenda"].ToString();
                            m.CreatedByEmail = rdr["CreatedByEmail"].ToString();
                            m.CreatedByName = rdr["CreatedByName"].ToString();
                            m.AccountManagerName = rdr["AccountManagerName"].ToString();
                            m.AccountManagerEmail = rdr["AccountManagerEmail"].ToString();
                            m.ModifiedByEmail = rdr["ModifiedByEmail"].ToString();
                            m.ModifiedByName = rdr["ModifiedByName"].ToString();
                            lst.Add(m);

                            if ((int)rdr["ID"] > 2000)
                            {
                                var test = m;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }


            return lst;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProposta")]
        public IHttpActionResult GetProposta(int? proposalID)
        {
            try
            {


                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo i = new LoadProposalInfo();
                i.ProposalId = proposalID.Value;
                ActionResponse a = p1.LoadProposal(i);

                double? LeiDaCopiaPriada = 0;
                double? sobrevalorizacao = 0;
                double? retomas = 0;
                BB_Proposal pr1 = new BB_Proposal();
                BB_Proposal_PrazoDiferenciado prazoDiferenciado1 = new BB_Proposal_PrazoDiferenciado();
                using (var db = new BB_DB_DEVEntities2())
                {
                    pr1 = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();

                    List<BB_Proposal_Quote> lstQuotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                    foreach (var quote in lstQuotes)
                    {
                        double? TCP = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).Select(x => x.TCP).FirstOrDefault();

                        var contador = db.BB_Proposal_Counters.Where(x => x.OSID == quote.ID).Count();

                        if (TCP is null)
                            TCP = 0;

                        if (quote.IsUsed.GetValueOrDefault() == false)
                            LeiDaCopiaPriada = (TCP * quote.Qty) + LeiDaCopiaPriada;

                    }

                    List<BB_Proposal_Overvaluation> o = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalID).ToList();
                    foreach (var quote in o)
                    {
                        sobrevalorizacao += quote.Total;
                    }

                    List<BB_Proposal_Upturn> ret = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposalID).ToList();
                    foreach (var quote1 in ret)
                    {
                        retomas += quote1.Total;
                    }

                    string nLocadora = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).Select(x => x.NLocadora).FirstOrDefault();
                    prazoDiferenciado1 = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).FirstOrDefault();


                    if (a.ProposalObj.Draft.financing.diffTerm != null)
                        a.ProposalObj.Draft.financing.diffTerm.Nlocadora = nLocadora;
                }


                //sobrevalorizacao = (sobrevalorizacao + retomas) - retomas;

                a.ProposalObj.Draft.details.ValueTotal = a.ProposalObj.Draft.details.ValueTotal + LeiDaCopiaPriada.Value + sobrevalorizacao.Value;


                double? sobrevalorizacao1 = 0;
                sobrevalorizacao1 = sobrevalorizacao - retomas;
                //double? sobrevalorizacao = 0;
                //sobrevalorizacao = a.ProposalObj.Draft.overvaluations.Select(x => x.Total).FirstOrDefault();
                double? OPSHWvalorTotal = 0;
                double? OPSHWUnti = 0;
                if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0 && sobrevalorizacao1 > 0)
                {
                    if (a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").Count() > 0)
                    {


                        OPSHWvalorTotal = a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(c => c.TotalNetsale)).First();

                        OPSHWUnti = a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(c => c.UnitDiscountPrice)).First();
                    }
                }

                //if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0)
                //{

                //    a.ProposalObj.Draft.details.ValueTotal = 0;
                //    foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                //    {
                //        quote.TotalNetsale = sobrevalorizacao1 != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao1) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                //        quote.UnitDiscountPrice = Math.Round((quote.TotalNetsale / quote.Qty), 2);
                //        //quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;

                //        a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                //    }
                //    a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
                //}
                //else
                //{
                //    //a.ProposalObj.Draft.details.ValueTotal = 0;
                //    //foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                //    //{
                //    //    a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                //    //}
                //    //a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
                //}

                using (var db1 = new BB_DB_DEV_LeaseDesk())
                {
                    LD_Contrato c = db1.LD_Contrato.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    if (c != null)
                    {
                        a.ProposalObj.NUS = db1.LD_Contrato_Facturacao.Where(x => x.LDID == c.ID).Select(x => x.NUS).FirstOrDefault();
                        a.ProposalObj.FolderDoc = c.Pasta;
                        a.ProposalObj.LeasedeskComentariosGC = c.ComentariosGC;
                        a.ProposalObj.LeasedeskComentarios = c.Comments;
                        a.ProposalObj.LeasedeskComentariosDevolucao = c.ComentariosDevolucao;
                        a.ProposalObj.LeasedeskStatus = db1.LD_Observacoes_Motivos.Where(x => x.ID == c.MotivoID).Select(x => x.Motive).FirstOrDefault();
                    }
                    else
                    {
                        a.ProposalObj.NUS = "NA";
                        a.ProposalObj.FolderDoc = "NA";
                        a.ProposalObj.LeasedeskComentariosGC = "NA";
                        a.ProposalObj.LeasedeskComentarios = "NA";
                        a.ProposalObj.LeasedeskComentariosDevolucao = "NA";
                        a.ProposalObj.LeasedeskStatus = "NA";
                    }

                }

                ValoresTotaisRenda vt = new ValoresTotaisRenda();
                vt.RendaFinanciada = 0;
                vt.VVA = 0;
                if (a.ProposalObj.Draft.financing.FinancingTypeCode != 0 && a.ProposalObj.Draft.financing.diffTerm != null)
                {
                    vt.RendaFinanciada = (a.ProposalObj.Draft.financing.diffTerm.Rent != null ? a.ProposalObj.Draft.financing.diffTerm.Rent : 0);


                }

                ApprovedPrintingService activePS = null;
                if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    try
                    {
                        activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                        if (activePS != null && activePS.GlobalClickVVA != null)
                        {

                            vt.VVA = activePS.GlobalClickVVA.PVP;
                            switch (activePS.GlobalClickVVA.RentBillingFrequency)
                            {
                                case 3:
                                    activePS.BWVolume = activePS.BWVolume * 3;
                                    activePS.CVolume = activePS.CVolume * 3;
                                    vt.VVA = activePS.GlobalClickVVA.PVP * 3;
                                    break;
                                case 6:
                                    activePS.BWVolume = activePS.BWVolume * 6;
                                    activePS.CVolume = activePS.CVolume * 6;
                                    vt.VVA = activePS.GlobalClickVVA.PVP * 3;
                                    break;
                                default: break;
                            }

                            //switch (activePS.GlobalClickVVA.RentBillingFrequency)
                            //{
                            //    case 3:
                            //        activePS.GlobalClickVVA.PVP = activePS.GlobalClickVVA != null ? activePS.GlobalClickVVA.PVP * 3 : 0;
                            //        break;
                            //    case 6:
                            //        activePS.GlobalClickVVA.PVP = activePS.GlobalClickVVA != null ? activePS.GlobalClickVVA.PVP * 6 : 0;
                            //        break;
                            //    default: break;
                            //}
                            //vt.VVA = activePS.GlobalClickVVA != null ? activePS.GlobalClickVVA.PVP : 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

                if (retomas.Value > 0)
                {
                    a.ProposalObj.Draft.details.ValueTotal -= retomas.Value;
                }

                a.ProposalObj.Draft.details.ValueTotal = pr1 != null && pr1.SubTotal != null ? pr1.SubTotal.Value : a.ProposalObj.Draft.details.ValueTotal;

                if (a.ProposalObj.Draft.baskets.rs_basket.Count() > 0 || a.ProposalObj.Draft.opsPacks.opsManage.Count() > 0)
                    vt.ServicosRecorentesMes = a.ProposalObj.Draft.baskets.rs_basket.Sum(x => x.MonthlyFee) + a.ProposalObj.Draft.opsPacks.opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity);
                else
                    vt.ServicosRecorentesMes = 0;

                if (a.ProposalObj.Draft.baskets.rs_basket.Count() > 0 || a.ProposalObj.Draft.opsPacks.opsManage.Count() > 0)
                    vt.ServicosRecorentesTotal = a.ProposalObj.Draft.baskets.rs_basket.Sum(x => x.TotalNetsale) + a.ProposalObj.Draft.opsPacks.opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity * x.TotalMonths);
                else
                    vt.ServicosRecorentesTotal = 0;


                vt.ConfiguracaoOneShotValor = a.ProposalObj.Draft.details.ValueTotal - vt.ServicosRecorentesTotal;

                double fee = (activePS != null && activePS.Fee != null ? activePS.Fee : 0);

                if (activePS != null && activePS.GlobalClickVVA != null)
                {
                    switch (activePS.GlobalClickVVA.RentBillingFrequency)
                    {
                        case 3:
                            //vt.RendaFinanciada = vt.RendaFinanciada * 3;
                            vt.ServicosRecorentesMes = vt.ServicosRecorentesMes * 3;
                            fee = fee * 3;
                            break;
                        case 6:
                            //vt.RendaFinanciada = vt.RendaFinanciada * 6;
                            vt.ServicosRecorentesMes = vt.ServicosRecorentesMes * 6;
                            fee = fee * 6;
                            break;
                        default: break;
                    }
                }




                vt.LeiCopiaPrivada = LeiDaCopiaPriada;
                vt.RendaTotal = vt.VVA + vt.RendaFinanciada + vt.ServicosRecorentesMes + fee;
                if (prazoDiferenciado1 != null && prazoDiferenciado1.FinancingID == 6)
                {
                    vt.RendaTotal = vt.RendaFinanciada;
                }
                vt.sobrevalorizacaoTotal = sobrevalorizacao != null && sobrevalorizacao.HasValue && sobrevalorizacao.Value != 0 ? sobrevalorizacao.Value : 0;
                vt.retomasTotal = retomas != null && retomas.HasValue && retomas.Value != 0 ? retomas.Value : 0;
                a.ProposalObj.valoretotais = vt;



                ProposalBLL p11 = new ProposalBLL();
                LoadProposalInfo ii = new LoadProposalInfo();
                ii.ProposalId = proposalID.Value;
                ActionResponse aa = p11.LoadProposal(ii);
                //CALCULO COMISSOES
                Comission _Comission = new Comission();


                //HW PRINTING
                HW_Printing hw = new HW_Printing();
                hw = CalculoHWPrinting(aa.ProposalObj);
                _Comission.Hw_Printing = hw;


                //ITS
                ITS_Comissao its  = CalculoITS(aa.ProposalObj);
                _Comission.ITS = its;


                //VVA
                TFM_Comissao tfm = CalculoTFM(aa.ProposalObj);
                _Comission.TFM = tfm;


                //ACELADOR NOVO CLIETNE
               NewClient_Comissao com  = CalculoNewClient(_Comission, proposalID);
                _Comission.Newclient = com;

                //ACELADOR Debito Directo
                DebitoDirecto_Commisao d = CalculoDebitoDirecto(_Comission, proposalID);
                _Comission.DebitoDirecto = d;

                _Comission.TotalComissao = _Comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() 
                    + _Comission.ITS.ITSComissao.GetValueOrDefault() + 
                    _Comission.TFM.TFMComissao.GetValueOrDefault() +
                    _Comission.Newclient.NewClientComissao.GetValueOrDefault()
                    + _Comission.DebitoDirecto.DebitoDirectoComissao.GetValueOrDefault();


                a.ProposalObj.Comission = _Comission;

                return Ok(a.ProposalObj);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok();
        }

        private DebitoDirecto_Commisao CalculoDebitoDirecto(Comission comission, int? proposalID)
        {
            //double? DebitoDirecto = 0;

            DebitoDirecto_Commisao DebitoDirecto = new DebitoDirecto_Commisao();
            DebitoDirecto.Isdebit = false;
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_Financing _BB_Proposal_Financing = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                    //Codigo debito directo é 2
                    if (_BB_Proposal_Financing != null && _BB_Proposal_Financing.PaymentMethodId.GetValueOrDefault() == 2)
                    {
                        DebitoDirecto.Isdebit = true;
                        double? Applydiscount = 2.5;
                        double? Applydiscountpercetagem = Math.Round((Applydiscount / 100).GetValueOrDefault(), 4);
                        DebitoDirecto.ValueValor = comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault();
                        DebitoDirecto.Percentagem = Applydiscount;
                        double? vlaor = (comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault()) * Applydiscountpercetagem;
                        DebitoDirecto.DebitoDirectoComissao = Math.Round(vlaor.GetValueOrDefault(), 2) ;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return DebitoDirecto;
        }

        private NewClient_Comissao CalculoNewClient(Comission comission, int? proposalID)
        {
            //double? NewClientComissao = 0;

            NewClient_Comissao _NewClient_Comissao = new NewClient_Comissao();
            _NewClient_Comissao.IsNewClient = false;
            
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_Client _BB_Proposal_Client = db.BB_Proposal_Client.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                    if(_BB_Proposal_Client != null && _BB_Proposal_Client.IsNewClient == true)
                    {
                        double? Applydiscount = 25;
                        double? Applydiscountpercetagem = Applydiscount / 100;
                        _NewClient_Comissao.Percentagem = Applydiscount;
                         double? valor = (comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault()) * Applydiscountpercetagem;
                        _NewClient_Comissao.NewClientComissao = Math.Round(valor.GetValueOrDefault(), 2);
                        _NewClient_Comissao.IsNewClient = true;
                        _NewClient_Comissao.ValueValor = comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return _NewClient_Comissao;
        }

        private TFM_Comissao CalculoTFM(ProposalRootObject ProposalObj)
        {
            double? vva = 0;
            double? VvaComissao = 0;

            TFM_Comissao tfm = new TFM_Comissao();

            try
            {
                ApprovedPrintingService activePS = null;
                if (ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    activePS = ProposalObj.Draft.printingServices2.ApprovedPrintingServices[ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                    if (activePS != null && activePS.GlobalClickVVA != null)
                    {

                        vva = activePS.GlobalClickVVA.PVP * activePS.ContractDuration;
                        
                        
                    }
                }

                double? ApplyDiscount = 1;
                double? ApplyDiscountPercentagem = ApplyDiscount  / 100;

                VvaComissao = Math.Round((vva * ApplyDiscountPercentagem).GetValueOrDefault(),2);

                if (VvaComissao >= 500)
                    VvaComissao = 500;

                tfm.TFMComissao = VvaComissao;
                tfm.Percentagem = ApplyDiscount;
                tfm.ValueValor = vva;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return tfm;
        }

        private ITS_Comissao CalculoITS(ProposalRootObject proposalObj)
        {
            double? _SumGP2_HW = 0;
            double? _SumGP2_BPO = 0;
            double? _SumGP2_SW = 0;

            double? _SumGP2_Total = 0;

            ITS_Comissao its = new ITS_Comissao();

            try
            {
                //Configurador ONE SHOT
                List<OsBasket> _BB_Proposal_Quote_HW = proposalObj.Draft.baskets.os_basket.Where(
                    x => x.Family == "PRSHW"
                    || x.Family == "MCSHW"
                    || x.Family == "IMSHW"
                    || x.Family == "VSSHW"
                    || x.Family == "BPOHW"
                    || x.Family == "OPSPSV"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_HW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_HW += item.GPTotal;
                }

              

                //Configurador Servicos Recorrnts
                List<RsBasket> _BB_Proposal_Quote_RS_HW = proposalObj.Draft.baskets.rs_basket.Where(
                    x => x.Family == "PRSHW"
                    || x.Family == "MCSHW"
                    || x.Family == "IMSHW"
                    || x.Family == "VSSHW"
                    || x.Family == "BPOHW"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_RS_HW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_HW += item.GPTotal;
                }

                its.HW = _SumGP2_HW;

                //Configurador ONE SHOT
                List<OsBasket> _BB_Proposal_Quote_SW = proposalObj.Draft.baskets.os_basket.Where(
                    x => x.Family == "PRSSW"
                    || x.Family == "MCSSW"
                    || x.Family == "IMSSW"
                    || x.Family == "VSSSW"
                    || x.Family == "BPOSW"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_SW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_SW += item.GPTotal;
                }



                //Configurador Servicos Recorrnts
                List<RsBasket> _BB_Proposal_Quote_RS_SW = proposalObj.Draft.baskets.rs_basket.Where(
                    x => x.Family == "PRSSW"
                    || x.Family == "MCSSW"
                    || x.Family == "IMSSW"
                    || x.Family == "VSSSW"
                    || x.Family == "BPOSW"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_RS_SW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_SW += item.GPTotal;
                }

                its.SW = _SumGP2_SW;

                //Configurador BPO
                List<OsBasket> _BB_Proposal_Quote_BPO = proposalObj.Draft.baskets.os_basket.Where(
                    x => x.CodeRef == "IMS-MSV-BPO" || x.CodeRef == "MCS-MSV-BPO" || x.CodeRef == "BPS-MSV-BPO"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_BPO)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_BPO += item.GPTotal;
                }

                //Configurador BPO
                List<RsBasket> _BB_Proposal_Quote_BPO_RS = proposalObj.Draft.baskets.rs_basket.Where(
                    x => x.CodeRef == "IMS-MSV-BPO" || x.CodeRef == "MCS-MSV-BPO" || x.CodeRef == "BPS-MSV-BPO"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_BPO_RS)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_BPO += item.GPTotal;
                }

                _SumGP2_Total = _SumGP2_BPO + _SumGP2_SW + _SumGP2_HW;

                double? ApplyDiscount = 6;

                double? ApplyDiscountPercetagem = ApplyDiscount / 100;

                its.ITSComissao = Math.Round((_SumGP2_Total * ApplyDiscountPercetagem).GetValueOrDefault(), 2);
                its.GP2 = _SumGP2_Total;
                its.Percetangem = ApplyDiscount;
                //Tecto maximo 6000
                if (its.ITSComissao >= 6000)
                    its.ITSComissao = 6000;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return its;
        }

        public HW_Printing CalculoHWPrinting(ProposalRootObject proposalObj)
        {
            //double? HW_Printing = 0;
            double? PVPTotal = 0;
            double? ValorFinal_C_Desconto = 0;

            HW_Printing hw = new HW_Printing();

            ProposalBLL p1 = new ProposalBLL();
            LoadProposalInfo i = new LoadProposalInfo();
            i.ProposalId = proposalObj.Draft.details.ID;
            ActionResponse a = p1.LoadProposal(i);
            proposalObj = a.ProposalObj;


            try
            {
                double? _PercentagemDesconto = 0;
                List<OsBasket> _BB_Proposal_Quote = proposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW" || x.Family == "PPHW").ToList();

                PVPTotal = _BB_Proposal_Quote.Sum(x => x.TotalPVP);
                ValorFinal_C_Desconto = _BB_Proposal_Quote.Sum(x => x.TotalNetsale);

                 using (var db = new BB_DB_DEVEntities2())
                {
                    //ValorFinal_C_Desconto = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalObj.Draft.details.ID && (x.Family == "OPSHW" || x.Family == "PPHW")).Select(x => x.TotalNetsale).Sum().GetValueOrDefault();
                    //PVPTotal = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalObj.Draft.details.ID && (x.Family == "OPSHW" || x.Family == "PPHW")).Select(x => x.TotalPVP).Sum().GetValueOrDefault();
                    //if (proposalObj.Draft.overvaluations.Count() > 0)
                    //{
                    //    ValorFinal_C_Desconto = ValorFinal_C_Desconto + proposalObj.Draft.overvaluations.Select(x => x.Total).FirstOrDefault();
                    //}

                

                if (ValorFinal_C_Desconto >= 0 && PVPTotal >= 0)
                {
                    _PercentagemDesconto = Math.Round(ValorFinal_C_Desconto.GetValueOrDefault() / PVPTotal.GetValueOrDefault(), 4);
                    _PercentagemDesconto = Math.Round((100 - (_PercentagemDesconto * 100)).GetValueOrDefault(), 2);
                }
              
                    List<BB_Commission> _lstBB_Commission = db.BB_Commission.ToList();
                    double? _ApplyDiscount = 0;

                    if (_PercentagemDesconto < 0)
                        _PercentagemDesconto = 0;

                    foreach (var item in _lstBB_Commission)
                    {
                        if (_PercentagemDesconto < item.Discount_End && _PercentagemDesconto >= item.Discount_Start)
                        {
                            _ApplyDiscount = item.HW_Printing;
                            hw.Percentagem = _ApplyDiscount;
                            _ApplyDiscount = _ApplyDiscount / 100;
                            break;
                        }
                    }
                    
                    //Calculate Comission
                    hw.HW_PrintingComissao = Math.Round((_ApplyDiscount * ValorFinal_C_Desconto).GetValueOrDefault(), 2);
                   
                    hw.ValueHardware = ValorFinal_C_Desconto;
                   

                    //Tecto maximo 5000
                    if (hw.HW_PrintingComissao >= 5000)
                        hw.HW_PrintingComissao = 5000;
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return hw;

        }

        [AcceptVerbs("GET", "POST")]
        public void CreateCommission(int proposalID)
        {
            try
            {
                BB_Commission_General bb_commission_general = new BB_Commission_General();

                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo i = new LoadProposalInfo();
                i.ProposalId = proposalID;
                ActionResponse loadProposal = p1.LoadProposal(i);

                var basket = loadProposal.ProposalObj.Draft.baskets.os_basket;

                // --------------- PONTO 1 -------------->

                List<BB_Proposal_Quote> oneShot = new List<BB_Proposal_Quote>();
                List<BB_Proposal_Quote_RS> servicosRecorrentes = new List<BB_Proposal_Quote_RS>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    oneShot = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                    servicosRecorrentes = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).ToList();

                    var profitDictionary = new Dictionary<string, GrossProfit>
                    {
                        { "HW", new GrossProfit(){ GPTotal = 0,} },
                        { "IMS_VSS", new GrossProfit(){ GPTotal = 0,} },
                        { "PRS", new GrossProfit(){ GPTotal = 0,} },
                        { "MCS_BPS", new GrossProfit(){ GPTotal = 0,} },                     
                        { "MOBOTIX", new GrossProfit(){ GPTotal = 0,} },
                    };


                    // --------------- PONTO 2 -------------->

                    List<string> mobotixCodRefs = db.BB_Data_Integration.Where(x => x.Description_Portuguese.Contains("MOBOTIX")).Select(x => x.CodeRef).ToList();

                    var clientGMA = loadProposal.ProposalObj.Draft.client.GMA;
                    var isGMA = loadProposal.ProposalObj.Draft.client.isGMA;

                    // função interna a ser chamada para fazer o somatório do GPTotal para cada família
                    void AddProfit(string family, double? amount, string codeRef, int? quantity)
                    {
                        if (family.Contains("OPSHW") || family.Contains("PPHW") || family.EndsWith("CS"))
                        {
                            // se o cliente for GMA, vou somar tudo o que é HW e multiplicar por 0.1
                            // assim, nunca vai cair no else
                            if (clientGMA != null && clientGMA != "" && isGMA == true)
                            {
                                var GMA_Amout = amount * 0.1;
                                profitDictionary["HW"].GPTotal += (GMA_Amout ?? 0) * quantity;
                            }
                            // se o cliente NAO for GMA, soma-se o GPTotal normalmente, sem aplicar uma regra especial
                            else
                            {
                                profitDictionary["HW"].GPTotal += (amount ?? 0) * quantity;
                }
                        }

                        if (family.Contains("IMS") || family.Contains("WPH"))
                        {
                            profitDictionary["IMS_VSS"].GPTotal += (amount ?? 0) * quantity;
            }

                        if (family.Contains("PRS") || family.Contains("SV"))
                        {
                            profitDictionary["PRS"].GPTotal += (amount ?? 0) * quantity;
            }

                        if (family.Contains("MCS") || family.Contains("BPS"))
                        {
                            profitDictionary["MCS_BPS"].GPTotal += (amount ?? 0) * quantity;
            }
                    }

                    // Fatores a ter em conta para o cálculo do GPTotal
                    var financingTypeCode = loadProposal.ProposalObj.Draft.financing.FinancingTypeCode;
                    var actionCampaignId = loadProposal.ProposalObj.Draft.details.CampaignID;

                    // Cálculo do GPTotal para cada familia de cada maquina

                    foreach (var oneShot_Item in oneShot)
                    {
                        if (financingTypeCode == 3)
                        {
                            var result = (oneShot_Item.TotalNetsale - oneShot_Item.TotalCost) * 0.75;

                            AddProfit(oneShot_Item.Family, result, oneShot_Item.CodeRef, oneShot_Item.Qty);
                        }
                        else if (oneShot_Item.IsUsed == true)
                        {
                            AddProfit(oneShot_Item.Family, oneShot_Item.TotalCost, oneShot_Item.CodeRef, oneShot_Item.Qty);
                        }
                        else if (actionCampaignId == 3)
                        {
                            var result = oneShot_Item.TotalNetsale * 0.75;

                            AddProfit(oneShot_Item.Family, result, oneShot_Item.CodeRef, oneShot_Item.Qty);
                        }
                        else
                        {
                            // conta default

                            if(oneShot_Item.Description.Contains("MOBOTIX"))
                            {
                                profitDictionary["MOBOTIX"].GPTotal += (oneShot_Item.GPTotal ?? 0) * oneShot_Item.Qty;

                            }
                            else if(oneShot_Item.Description.Contains("BPS"))
                            {
                                profitDictionary["MCS_BPS"].GPTotal += (oneShot_Item.GPTotal ?? 0) * oneShot_Item.Qty;

                            }
                            else { 
                                AddProfit(oneShot_Item.Family, oneShot_Item.GPTotal, oneShot_Item.CodeRef, oneShot_Item.Qty);
                            }
                            
                        }
                    }

                    // servicos recorrentes
                    foreach (var servRecor_Item in servicosRecorrentes)
                    {
                        AddProfit(servRecor_Item.Family, servRecor_Item.GPTotal, servRecor_Item.CodeRef, servRecor_Item.Qty);
                    }


                    var profit_OfficeHW = profitDictionary["HW"];
                    var profit_IMS_VSS = profitDictionary["IMS_VSS"];
                    var profit_PRS = profitDictionary["PRS"];
                    var profit_MCS_BPS = profitDictionary["MCS_BPS"];
                    var profit_MOBOTIX = profitDictionary["MOBOTIX"];

                    // --------------- PONTO 3 -------------->

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();

                    bool isNewClient = proposal.ClientAccountNumber.StartsWith("P");
                    bool isNewBusinessLine = loadProposal.ProposalObj.Draft.baskets.newBusinessLine ?? false;

                    // Definicao da percentagem de comissao a aplicar a cada familia
                    if (!isNewClient)
                    {
                        profit_OfficeHW.ComissionPercentage = 9;
                        profit_IMS_VSS.ComissionPercentage = 9;
                        profit_PRS.ComissionPercentage = 9;
                        profit_MCS_BPS.ComissionPercentage = 9;
                        profit_MOBOTIX.ComissionPercentage = 9;
                    }
                    else if (isNewBusinessLine)
                    {
                        profit_OfficeHW.ComissionPercentage = 12.5;
                        profit_IMS_VSS.ComissionPercentage = 12.5;
                        profit_PRS.ComissionPercentage = 12.5;
                        profit_MCS_BPS.ComissionPercentage = 12.5;
                        profit_MOBOTIX.ComissionPercentage = 12.5;
                    }
                    else
                    {
                        profit_OfficeHW.ComissionPercentage = 15.5;
                        profit_IMS_VSS.ComissionPercentage = 15.5;
                        profit_PRS.ComissionPercentage = 15.5;
                        profit_MCS_BPS.ComissionPercentage = 15.5;
                        profit_MOBOTIX.ComissionPercentage = 15.5;
                    }


                    // Valor do GPTotal acrescido da comissao definida acima
                    // Exemplo: CalculatedCommission = GPTotal * 0.09

                    profit_PRS.CalculatedCommission = profit_PRS.GPTotal * (profit_PRS.ComissionPercentage / 100);
                    profit_OfficeHW.CalculatedCommission = profit_OfficeHW.GPTotal * (profit_OfficeHW.ComissionPercentage / 100);
                    profit_MOBOTIX.CalculatedCommission = profit_MOBOTIX.GPTotal * (profit_MOBOTIX.ComissionPercentage / 100);
                    profit_IMS_VSS.CalculatedCommission = profit_IMS_VSS.GPTotal * (profit_IMS_VSS.ComissionPercentage / 100);
                    profit_MCS_BPS.CalculatedCommission = profit_MCS_BPS.GPTotal * (profit_MCS_BPS.ComissionPercentage / 100);

                    // Soma da CalculatedCommission todas as familias
                    bb_commission_general.Comision = profit_OfficeHW.CalculatedCommission +
                                                     profit_PRS.CalculatedCommission +
                                                     profit_MOBOTIX.CalculatedCommission +
                                                     profit_IMS_VSS.CalculatedCommission +
                                                     profit_MCS_BPS.CalculatedCommission;

                    bb_commission_general.Comision = Math.Round((double)bb_commission_general.Comision, 2);


                    // --------------- PONTO 4 -------------->

                    // Calculo da comissao a aplicar a familias do dicionario protocolDictionary

                    List<Machine> machines = new List<Machine>();

                    double? pvpClick;
                    double? vendaClick;

                    var protocolDictionary = new Dictionary<string, CommissionDictionary>()
                    {
                        { "A3 Printing_Colour", new CommissionDictionary(){ Commission = 20, Adjustment = 5} },
                        { "A3 Printing_BW", new CommissionDictionary(){ Commission = 8, Adjustment = 5} },
                        { "A4 Printing_Colour", new CommissionDictionary(){ Commission = 9, Adjustment = 5} },
                        { "A4 Printing_BW", new CommissionDictionary(){ Commission = 4, Adjustment = 5} },
                    };

                    bool? isSecondHand = false;

                    foreach (var quote in oneShot)
                    {
                        // verificar se o negócio tem second hand ou não
                        if (quote.IsUsed == true && isSecondHand == false) isSecondHand = true;

                        var equipamentos = db.BB_Equipamentos.Where(e => e.CodeRef == quote.CodeRef).ToList();

                        foreach (var equipamento in equipamentos)
                        {
                            BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == quote.Proposal_ID).FirstOrDefault();
                            BB_PrintingServices ps = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == ps2.ID).FirstOrDefault();
                            ApprovedPrintingService activePS = null;

                            if (loadProposal.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                            {                         
                                activePS = loadProposal.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[loadProposal.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                                
                                // VVA ----------------------------
                                if (activePS != null && activePS.GlobalClickVVA != null)
                                {
                                    BB_VVA vva = db.BB_VVA.Where(x => x.PrintingServiceID == ps.ID).FirstOrDefault();

                                    double? volTotal = ps.BWVolume + ps.CVolume;

                                    if (equipamento.PHC4 == "BW")
                                    {
                                        pvpClick = equipamento.ClickPriceBW;
                                        vendaClick = ((ps.BWVolume * vva.PVP) / volTotal) / ps.BWVolume;
                                    }
                                    else
                                    {
                                        pvpClick = equipamento.ClickPriceC;
                                        vendaClick = ((ps.CVolume * vva.PVP) / volTotal) / ps.CVolume;
                                    }
                                }
                                // Sem Volume ----------------------------
                                else if (activePS != null && activePS.GlobalClickNoVolume != null)
                                {
                                    BB_PrintingServices_NoVolume ps_noVol = db.BB_PrintingServices_NoVolume.Where(x => x.PrintingServiceID == ps.ID).FirstOrDefault();

                                    if (equipamento.PHC4 == "BW")
                                    {
                                        pvpClick = equipamento.ClickPriceBW;
                                        vendaClick = ps_noVol.GlobalClickBW;
                                    }
                                    else
                                    {
                                        pvpClick = equipamento.ClickPriceC;
                                        vendaClick = ps_noVol.GlobalClickC;
                                    }
                                }
                                // Por Modelo ----------------------------
                                else
                                {
                                    BB_PrintingService_Machines ps_m = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == ps.ID).FirstOrDefault();

                                    if (equipamento.PHC4 == "BW")
                                    {
                                        pvpClick = equipamento.ClickPriceBW;
                                        vendaClick = ps_m.ApprovedBW;
                                    }
                                    else
                                    {
                                        pvpClick = equipamento.ClickPriceC;
                                        vendaClick = ps_m.ApprovedC;
                                    }
                                }

                            Machine machine = new Machine
                            {
                                CodeRef = quote.CodeRef,
                                Description = quote.Description,
                                Qty = quote.Qty,
                                DescPerClick = 100 - ((100* vendaClick) / pvpClick),
                                PHC1 = equipamento.PHC1,
                                PHC4 = equipamento.PHC4
                            };

                                // Formulas a aplicar a cada registo do "protocolDictionary" a cada maquina
                                string key = $"{machine.PHC1}_{machine.PHC4}";

                                if (protocolDictionary.ContainsKey(key))
                                {
                                    // ha penalizacao
                                    if (machine.DescPerClick >= 0)
                                    {
                                        machine.AppliedCommission = (protocolDictionary[key].Commission * machine.Qty) -
                                            (protocolDictionary[key].Adjustment * machine.DescPerClick);
                                    }
                                    // ha bonificacao
                                    else
                                    {
                                        machine.AppliedCommission = (protocolDictionary[key].Commission * machine.Qty) - (2 * machine.DescPerClick);
                                    }

                                    machines.Add(machine);
                                };

                            }
                        }
                    };


                    // --------------------------------------------------------------------------------------------------------------------------------------------------------
                    //                              Construcao do modelo 'bb_commission_general' para o insert
                    // --------------------------------------------------------------------------------------------------------------------------------------------------------

                    using (var dbUsers = new masterEntities())
                        {
                            AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.Email == proposal.AccountManager).FirstOrDefault();
                            
                            bb_commission_general.Comercial = user.DisplayName;
                            
                            
                            
                            bb_commission_general.N_Trab = user.ErpNumber;
                            bb_commission_general.Manager_Nombre = user.Manager;

                            bb_commission_general.Manager = dbUsers.AspNetUsers
                                                                    .Where(x => x.Email == user.ManagerEmail)
                                                                    .Select(x => x.ErpNumber)
                                                                    .FirstOrDefault();
                           

                        bb_commission_general.Delegacion = user.Location;
                        bb_commission_general.Area = user.AreaComercial;
                        bb_commission_general.Usuario_Sharepoint = user.USUARIO_Sharepoint_Email;
                        bb_commission_general.Usuario_Sharepoint_Nombre = user.USUARIO_Sharepoint_Nome;

                    }

                    DateTime? modifiedDate = db.LD_Contrato.Where(x => x.ProposalID == proposalID).Select(x => x.ModifiedTime).FirstOrDefault();

                    if (modifiedDate.HasValue)
                    {
                        // exemplo:  01-02-2023 => 2302
                        bb_commission_general.Production = ((modifiedDate.Value.Year % 100) * 100) + modifiedDate.Value.Month;
                    }                  

                    bb_commission_general.Pedido = proposal.CreatedTime.Value.Year + proposalID.ToString();
                    bb_commission_general.Pedido_SAP = proposal.Pedido_SAP;
                    bb_commission_general.Cliente = loadProposal.ProposalObj.Draft.client.accountnumber;
                    bb_commission_general.Nombre_Cliente = loadProposal.ProposalObj.Draft.client.Name;
                    bb_commission_general.CN_Total = Math.Round((double)proposal.ValueTotal, 2);
                    bb_commission_general.Comision_Copias = 0;

                    foreach (var machine in machines)
                    {
                        bb_commission_general.Comision_Copias = bb_commission_general.Comision_Copias + machine.AppliedCommission;
                    }

                    //bb_commission_general.Comision_Copias = protocolDictionary.Values
                    //    .Where(cd => cd.Machines != null)
                    //    .SelectMany(cd => cd.Machines)
                    //    .Sum(m => m.AppliedCommission ?? 0);


                    bb_commission_general.Total_Comision = bb_commission_general.Comision + bb_commission_general.Comision_Copias;
                    bb_commission_general.Total_Comision = Math.Round((double)bb_commission_general.Total_Comision, 2);

                    // TotalNetSalte dos mobotix
                    bb_commission_general.CN_Mobotix = basket.Where(x => x.Description.Contains("Mobotix")).Sum(x => x.TotalNetsale);

                    bb_commission_general.CN_Hard = basket.Where(x => x.Family.Contains("OPSHW") || x.Family.Contains("PPHW") || x.Family.EndsWith("CS")).Sum(x => x.TotalNetsale);

                    bb_commission_general.CN_IMS_VSS = basket.Where(x => x.Family.Contains("IMS") || x.Family.Contains("WPH")).Sum(x => x.TotalNetsale) + bb_commission_general.CN_Mobotix;

                    bb_commission_general.CN_PRS = basket.Where(x => x.Family.Contains("PRS") || x.Family.EndsWith("SV")).Sum(x => x.TotalNetsale);

                    bb_commission_general.CN_MCS_BPS = basket.Where(x => x.Family.Contains("MCS") || x.Family.Contains("BPS")).Sum(x => x.TotalNetsale);


                    bb_commission_general.CN_Mobotix = Math.Round((double)bb_commission_general.CN_Mobotix, 2);
                    bb_commission_general.CN_Hard = Math.Round((double)bb_commission_general.CN_Hard, 2);
                    bb_commission_general.CN_IMS_VSS = Math.Round((double)bb_commission_general.CN_IMS_VSS, 2);
                    bb_commission_general.CN_PRS = Math.Round((double)bb_commission_general.CN_PRS, 2);
                    bb_commission_general.CN_MCS_BPS = Math.Round((double)bb_commission_general.CN_MCS_BPS, 2);


                    // Soma de todos os GP daquele proposalID (incluindo RS)
                    bb_commission_general.GP_Hard = profitDictionary.Where(d => d.Key == "HW").Sum(x => x.Value.GPTotal);

                    bb_commission_general.GP_IMS_VSS = profit_IMS_VSS.GPTotal + profit_MOBOTIX.GPTotal;

                    bb_commission_general.GP_PRS = profit_PRS.GPTotal;

                    bb_commission_general.GP_MCS_BPS = profit_MCS_BPS.GPTotal;

                    bb_commission_general.GP_Total = bb_commission_general.GP_Hard +
                                                     bb_commission_general.GP_IMS_VSS +
                                                     bb_commission_general.GP_PRS +
                                                     bb_commission_general.GP_MCS_BPS;

                    bb_commission_general.GP_Hard = Math.Round((double)bb_commission_general.GP_Hard, 2);
                    bb_commission_general.GP_IMS_VSS = Math.Round((double)bb_commission_general.GP_IMS_VSS, 2);
                    bb_commission_general.GP_PRS = Math.Round((double)bb_commission_general.GP_PRS, 2);
                    bb_commission_general.GP_MCS_BPS = Math.Round((double)bb_commission_general.GP_MCS_BPS, 2);
                    bb_commission_general.GP_Total = Math.Round((double)bb_commission_general.GP_Total, 2);

                    bb_commission_general.Incidencias = null;
                    bb_commission_general.Es_Segunda_Mano = isSecondHand;
                    bb_commission_general.Es_GMA = loadProposal.ProposalObj.Draft.client.isGMA;
                    bb_commission_general.CBB = bb_commission_general.Es_GMA;
                    bb_commission_general.Es_Prospecto = loadProposal.ProposalObj.Draft.baskets.prospect;            

                    int? campaignID = loadProposal.ProposalObj.Draft.details.CampaignID;
                    if (campaignID == 0)
                    {
                        bb_commission_general.Tipo_Operacion = "Negocio Tradicional";
                    }
                    else
                    {
                        bb_commission_general.Tipo_Operacion = db.BB_Campanha.Where(x => x.ID == campaignID).Select(x => x.Campanha).FirstOrDefault();
                    }

                    bb_commission_general.Tipo_Financiacion = db.BB_FinancingType.Where(x => x.Code == loadProposal.ProposalObj.Draft.financing.FinancingTypeCode).Select(x => x.Type).FirstOrDefault();

                    bb_commission_general.Fecha_Operacion = DateTime.Now;
                    bb_commission_general.CreatedBy = null;
                    bb_commission_general.ModifiedDate = null;
                    bb_commission_general.ModifiedBy = null;

                    // Definir o tipo de cliente
                    if (isNewBusinessLine == true)
                    {
                        bb_commission_general.Tipo_Cliente = "NLN";
                    }
                    else if(isNewClient == true)
                    {
                        bb_commission_general.Tipo_Cliente = "PROSPECTO";
                    }
                    else
                    {
                        bb_commission_general.Tipo_Cliente = "CLIENTE";
                    }

                    // Definir o campo GMA
                    if(bb_commission_general.Es_GMA == true)
                    {
                        bb_commission_general.GMA_10 = "GMA";
                    }
                    else
                    {
                        bb_commission_general.GMA_10 = "X"; 
                    }

                    bb_commission_general.Observacion = null;

                    // Definicao da Condicion para o calculo dos premios ---------------------

                    int? campaignID_Cond = db.BB_Proposal.Where(x => x.ID == proposalID).Select(x => x.CampaignID).FirstOrDefault();
                    bool extensionAlq = false;

                    var equipamentosX = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                    var isPPMachine = equipamentosX.Where(x => x.Family.StartsWith("PP")).Any();
                    var isAditamento = db.BB_Proposal.Where(x => x.ID == proposalID).Select(x => x.ContractNumberPai).FirstOrDefault();

                    if (campaignID_Cond == 3 || campaignID_Cond == 5)
                    {
                        extensionAlq = true;
                    }

                    if (isSecondHand == true)
                    {
                        bb_commission_general.Condicion = "1";
                    }                 
                    else if (extensionAlq == true)
                    {
                        bb_commission_general.Condicion = "2";
                    }
                    else if (isAditamento != null || isAditamento != "")
                    {
                        bb_commission_general.Condicion = "3";
                    }
                    else if (isPPMachine == true)
                    {
                        bb_commission_general.Condicion = "4";
                    }               
                    else
                    {
                        bb_commission_general.Condicion = "0";
                    }

                    

                    // Calculo de Premios ----------------------------------------------------
                    bb_commission_general.GP_HW_Premio = CalculatePremio((double)bb_commission_general.GP_Hard, bb_commission_general.Condicion);
                    bb_commission_general.GP_HW_Premio = Math.Round((double)bb_commission_general.GP_HW_Premio, 2);


                    bb_commission_general.GP_IMS_VSS_Premio = CalculatePremio((double)bb_commission_general.GP_IMS_VSS, bb_commission_general.Condicion);
                    bb_commission_general.GP_IMS_VSS_Premio = Math.Round((double)bb_commission_general.GP_IMS_VSS_Premio, 2);

                    bb_commission_general.GP_PRS_Premio = CalculatePremio((double)bb_commission_general.GP_PRS, bb_commission_general.Condicion);
                    bb_commission_general.GP_PRS_Premio = Math.Round((double)bb_commission_general.GP_PRS_Premio, 2);

                    bb_commission_general.GP_MCS_BPS_Premio = CalculatePremio((double)bb_commission_general.GP_MCS_BPS, bb_commission_general.Condicion);
                    bb_commission_general.GP_MCS_BPS_Premio = Math.Round((double)bb_commission_general.GP_MCS_BPS_Premio, 2);

                    bb_commission_general.GP_Total_Premios = bb_commission_general.GP_HW_Premio +
                                                                bb_commission_general.GP_IMS_VSS_Premio +
                                                                bb_commission_general.GP_PRS_Premio +
                                                                bb_commission_general.GP_MCS_BPS_Premio;

                    bb_commission_general.GP_Total_Premios = Math.Round((double)bb_commission_general.GP_Total_Premios, 2);

                    // Calculo do Percentage_GP ----------------------------------------------
                    if (bb_commission_general.CN_Total > 0)
                    {
                        var percentage_GP = ((bb_commission_general.GP_Total / bb_commission_general.CN_Total) * 100);                
                        if (percentage_GP != null)
                        {
                            percentage_GP = Math.Round((double)percentage_GP, 2);
                            bb_commission_general.Percentage_GP = percentage_GP.ToString() + '%';
                        }
                        else
                        {
                            bb_commission_general.Percentage_GP = "-";
                        }
                    }
                    else
                    {
                        bb_commission_general.Percentage_GP = "0%";
                    }

                    // Calculo do Percentage_Comision ----------------------------------------
                    if (bb_commission_general.GP_Total > 0)
                    {
                        var percentage_Comision = ((bb_commission_general.Comision / bb_commission_general.GP_Total) * 100);                      

                        if (percentage_Comision != null)
                        {
                            percentage_Comision = Math.Round((double)percentage_Comision, 2);
                            bb_commission_general.Percentage_Comision = percentage_Comision.ToString() + '%';
                        }
                        else
                        {
                            bb_commission_general.Percentage_Comision = "-";
                        }
                    }
                    else
                    {
                        bb_commission_general.Percentage_Comision = "0%";
                    }

                    bb_commission_general.Calculo = "Bsine Bilfer";
                    bb_commission_general.Estado_Factura = "PENDIENTE";

                    // Empty Info ON PURPOSE -------------------------------------------------
                    bb_commission_general.Factura_SAP = null;
                    bb_commission_general.Fecha_Factura = null;
                    bb_commission_general.Fecha_Pago_Comision = null;
                    bb_commission_general.Fecha_Registro = null;
                    bb_commission_general.CN_MRR = null;
                    bb_commission_general.GP_MRR = null;
                    bb_commission_general.Manager_Nombre_2 = null;
                    bb_commission_general.Manager_2 = null;
                    bb_commission_general.Incidencias = null;


                    // ----------------------------------------------------------------------------------------------------
                    //                              Construcao da coluna 'Logs'
                    // ----------------------------------------------------------------------------------------------------
                    string logPhase_1 = string.Format("({0} MG41 - Comision sobre el margen = (Margen Hw({1}) + Margen ITS ({2}) + " +
                    "Margen IMS ({3})) + Comisión de margen (cliente)({4}%) - ({5}%) = {6})",
                    proposalID,                       // {0}
                    bb_commission_general.Margen_HW,  // {1}
                    bb_commission_general.Margen_ITS, // {2}
                    bb_commission_general.Margen_IMS, // {3}
                    profit_OfficeHW.ComissionPercentage,  // {4}
                    0,                                // {5}
                    bb_commission_general.Comision // {6}
                    );

                    string newLine = "\n \n";

                    string opType = campaignID == 6 ? "<" : ">";

                    string logPhase_2 = string.Format("({0} - GPFull Log Com GP CA: NET SALES ({1}))" +
                        "{2} 0 | CA HARD = {3} AND GP HARD = {4} AND %GP HARD = {5})",
                    proposalID,                             // {0}
                    bb_commission_general.CN_Total,    // {1}
                    opType,                                 // {2}
                    bb_commission_general.CN_HW,            // {3}
                    bb_commission_general.Margen_HW,        // {4}
                    (bb_commission_general.Margen_HW * 100) / bb_commission_general.CN_HW // {5}
                    );

                    string logFinal = logPhase_1 + newLine + logPhase_2 + newLine + "\n";

                    bb_commission_general.Logs = logFinal;
                    bb_commission_general.BB_Numero = proposalID.ToString();

                    // ----- OLD INFO -------------------------------------------------------------------------------------

                    //bb_commission_general.Codigo_Agencia = loadProposal.ProposalObj.Draft.details.CRM_QUOTE_ID;
                    //bb_commission_general.Sales_Group = "540"; // hardcoded


                    //bb_commission_general.HR_Comentario = HRComments;
                    //bb_commission_general.Invoice_List = null;
                    //bb_commission_general.Margen = null;

                    //bb_commission_general.CN_HW = basket.Where(x => x.Family.Contains("HW") || x.Family.EndsWith("CS")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_HW = profit_Hard.GPTotal;
                    //bb_commission_general.Margen_HW_Nuevo = bb_commission_general.Margen_HW;

                    //bb_commission_general.Margen_Office_HW = profit_OfficeHW.GPTotal;

                    //bb_commission_general.CN_PP_HW = basket.Where(x => x.Family.Contains("PPHW")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_PP_HW = profit_PPHW.GPTotal;

                    //bb_commission_general.CN_IP_HW = basket.Where(x => x.Family.Contains("IPHW")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_IP_HW = profit_IPHW.GPTotal;

                    //bb_commission_general.CN_ITS = basket.Where(x => x.Family.Contains("ITS") || x.Family.Contains("MCS") || x.Family.Contains("BPS") || x.Family.Contains("IMS")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_ITS = profit_ITS_MCS_BPS_IMS.GPTotal;

                    //bb_commission_general.CN_MCS = basket.Where(x => x.Family.Contains("MCS")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_MCS = profit_MCS.GPTotal;

                    //bb_commission_general.CN_BPS = basket.Where(x => x.Family.Contains("BPS")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_BPS = profit_BPS.GPTotal;

                    //bb_commission_general.CN_IMS = 0;

                    //bb_commission_general.Margen_IMS = profit_IMS.GPTotal;

                    //bb_commission_general.CN_WPH = basket.Where(x => x.Family.Contains("WPH")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_WPH = profit_WPH.GPTotal;

                    //bb_commission_general.CN_Mobotix = basket.Where(x => x.Description.Contains("Mobotix")).Sum(x => x.TotalNetsale);
                    //bb_commission_general.Margen_Mobotix = profit_MOBOTIX.GPTotal;

                    //bb_commission_general.Numero_Cliente_SAP = bb_commission_general.Numero_Cliente;

                    //bb_commission_general.Metodo_Pago_Productos = db.BB_FinancingPaymentMethod.Where(x => x.ID == loadProposal.ProposalObj.Draft.financing.PaymentMethodId).Select(x => x.Type).FirstOrDefault();

                    // Perguntar ao Luis?
                    //bb_commission_general.Metodo_Pago_Mantenimiento = "ADEUDO DIRECTO";

                    //bb_commission_general.Periodo_Solicitado = bb_commission_general.Ano_Mes_CN;

                    //bb_commission_general.Facturación = null;

                    //bb_commission_general.CN_IMS += basket.Where(x => x.Family.Contains("IMS") && !mobotixCodRefs.Contains(b.CodeRef)).Select(x => x.TotalNetsale).FirstOrDefault();

                    //bb_commission_general.Pagado = null;
                    //bb_commission_general.Controlado = null;
                    //bb_commission_general.Comisionado = null;
                    //bb_commission_general.Excluido = null;
                    //bb_commission_general.Es_Doc_Share = null;
                    //bb_commission_general.Es_Invoice_List = bb_commission_general.Invoice_List;
                    //bb_commission_general.Support_BEU = loadProposal.ProposalObj.Draft.baskets.BEUSupport;

                    //var HRCommentsList = db.BB_WFA_Comments_Business.Where(x => x.ProposalID == proposalID && x.CommentType == "RRHH").ToList();
                    //if (HRCommentsList.Count() >= 1)
                    //{
                    //    foreach( var comment in HRCommentsList)
                    //    {
                    //        HRComments += comment.Comment;
                    //    } 
                    //}


                    // ----------------------------------------------------------------------------------------------------

                    List<BB_Commission_General> lastCommission = db.BB_Commission_General.Where(x => x.BB_Numero == proposalID.ToString()).ToList();
                    if (lastCommission.Any())
                    {
                        db.BB_Commission_General.RemoveRange(lastCommission);
                    }

                    db.BB_Commission_General.Add(bb_commission_general);
                    db.SaveChanges();

                }

                // ---------------------------------------------------------------------
                // TESTS ---------------------------------------------------------------
                //                                                                  ----
                var obj_To_Check_Info_When_Breakpoint = bb_commission_general;      //--
                //                                                                  ----
                // ---------------------------------------------------------------------
                // ---------------------------------------------------------------------


            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("ExportCommissions")]
        public HttpResponseMessage ExportCommissions()
        {
            List<BB_Commission_General> commission_lst = new List<BB_Commission_General>();

            string tempPath = Path.Combine("c:\\DocumentPrinting\\", "TempCommissionsFile.xlsx");
            Application excelApp = null;
            Workbook workbook = null;
            Worksheet worksheet = null;

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    commission_lst = db.BB_Commission_General.ToList();
                }

                if (!commission_lst.Any())
                {
                    throw new Exception("No data found.");
                }

                //obter o Type do primeiro registo da lista
                Type tipo = commission_lst.FirstOrDefault().GetType();
                //obter todas as propriedades do Type (por exemplo nomes das colunas da bd)
                PropertyInfo[] propriedades = tipo.GetProperties();

                //criar uma app Excel
                excelApp = new Application();
                workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                worksheet = (Worksheet)workbook.Worksheets[1];

                int line = 1;
                int column = 1;

                var campoParaExcel = new Dictionary<string, string>
                {
                    { "Pedido_SAP", "PEDIDO SAP" },
                    { "Pedido", "PEDIDO" },
                    { "Delegacion", "DELEGACION" },
                    { "Area", "AREA" },
                    { "Fecha_Operacion","FECHA OPERACIÓN" },
                    { "Tipo_Operacion","TIPO OPERACIÓN" },
                    { "Tipo_Cliente","TIPO CLIENTE" },
                    { "GMA_10","GMA 10%" },
                    { "Cliente", "CLIENTE" },
                    { "Nombre_Cliente", "NOMBRE CLIENTE" },
                    { "Observacion","OBSERVACION" },
                    { "N_Trab", "Nº TRAB" },
                    { "Comercial", "COMERCIAL" },
                    { "Factura_SAP","FACTURA SAP" },
                    { "Fecha_Factura","FECHA FACTURA" },
                    { "Comision", "COMISION" },
                    { "Comision_Copias", "COMISION COPIAS" },
                    { "Total_Comision", "TOTAL COMISION" },
                    { "Estado_Factura","ESTADO FACTURA" },
                    { "Fecha_Pago_Comision","FECHA PAGO COMISION" },
                    { "Fecha_Registro","FECHA REGISTRO" },
                    { "Production", "PRODUCCION" },
                    { "CN_Hard","CN HARD" },
                    { "CN_IMS_VSS","CN IMS+VSS" },
                    { "CN_PRS","CN PRS" },
                    { "CN_MCS_BPS","CN MCS+BPS" },
                    { "CN_MRR","CN MRR" },
                    { "CN_Total", "CN TOTAL" },
                    { "GP_Hard", "GP HARD" },
                    { "GP_IMS_VSS","GP IMS+VSS" },
                    { "GP_PRS","GP PRS" },
                    { "GP_MCS_BPS","GP MCS+BPS" },
                    { "GP_MRR","GP MRR" },
                    { "GP_Total", "GP TOTAL" },
                    { "Condicion","CONDICION" },
                    { "GP_HW_Premio","GP HW PREMIO" },
                    { "GP_IMS_VSS_Premio","GP IMS+VSS PREMIO" },
                    { "GP_PRS_Premio","GP PRS PREMIO" },
                    { "GP_MCS_BPS_Premio","GP MCS+BPS PREMIO" },
                    { "GP_Total_Premios","GP TOTAL PREMIOS" },
                    { "Usuario_Sharepoint","USUARIO SHAREPOINT" },
                    { "Usuario_Sharepoint_Nombre","USUARIO SHAREPOINT NOMBRE" },
                    { "Manager", "MANAGER" },
                    { "Manager_Nombre", "MANAGER NOMBRE" },
                    { "Manager_2", "MANAGER2" },
                    { "Manager_Nombre_2", "MANAGER2 NOMBRE" },
                    { "Calculo","CALCULO" },
                    { "Percentage_GP","% GP" },
                    { "Percentage_Comision","% COMISION" },
                    { "Incidencias","INCIDENCIAS" },
                    { "Logs","LOGS" },
                    { "Es_Segunda_Mano","ES SEGUNDA MANO" },
                    { "Es_GMA","ES GMA" },
                    { "CBB","CBB" },
                    { "Es_Prospecto","ES PROSPECTO" }


                    //-----------------------------


                    //{ "Es_Doc_Share","" },
                    //{ "Es_Invoice_List","" },
                    //{ "ID", "Identificador" },
                    //{ "Codigo_Agencia", "Código da Agência" },
                    //{ "Sales_Group", "Grupo de Vendas" },
                    //{ "Periodo_Solicitado", "Período Solicitado" },
                    //{ "HR_Comentario", "Comentário RH" },
                    //{ "Invoice_List", "Lista de Faturas" },
                    //{ "BB_Numero", "Número BB" },
                    //{ "Facturación", "Faturamento" },
                    //{ "Margen", "Margem" },
                    //{ "CN_HW","" },
                    //{ "Margen_HW","" },
                    //{ "Margen_HW_Nuevo","" },
                    //{ "Margen_Office_HW","" },
                    //{ "CN_PP_HW","" },
                    //{ "Margen_PP_HW","" },
                    //{ "CN_IP_HW","" },
                    //{ "Margen_IP_HW","" },
                    //{ "CN_ITS","" },
                    //{ "Margen_ITS","" },
                    //{ "CN_MCS","" },
                    //{ "Margen_MCS","" },
                    //{ "CN_BPS","" },
                    //{ "Margen_BPS","" },
                    //{ "CN_IMS","" },
                    //{ "Margen_IMS","" },
                    //{ "CN_WPH","" },
                    //{ "Margen_WPH","" },
                    //{ "CN_Mobotix","" },
                    //{ "Margen_Mobotix","" },
                    //{ "Pagado","" },
                    //{ "Controlado","" },
                    //{ "Comisionado","" },
                    //{ "Incidencia","" },
                    //{ "Excluido","" },
                    //{ "Support_BEU","" },
                    //{ "Numero_Cliente_SAP","" },
                    //{ "Tipo_Financiacion","" },
                    //{ "Metodo_Pago_Productos","" },
                    //{ "Metodo_Pago_Mantenimiento","" },
                    //{ "CreatedBy","" },
                    //{ "ModifiedDate","" },
                    //{ "ModifiedBy","" }
                };


                // Adicionar cabeçalhos no Excel a partir do dicionário
                foreach (var campo in campoParaExcel)
                {
                    // campo.Key é o nome original do campo
                    // campo.Value é o nome que será exibido no Excel
                    worksheet.Cells[line, column] = campo.Value;
                    column++;
                }


                // Adicionar dados
                line++;

                foreach (var commission in commission_lst)
                {
                    column = 1;
                    foreach (var campo in campoParaExcel) // Itera sobre o dicionário
                    {
                        // Obtém a propriedade correspondente à chave do dicionário
                        var prop = propriedades.FirstOrDefault(p => p.Name == campo.Key);

                        if (prop != null)
                        {
                            // Obtém o valor da propriedade para o objeto atual
                            object value = prop.GetValue(commission);
                            worksheet.Cells[line, column] = value;
                        }
                        column++;
                    }
                    line++;
                }

                //foreach (var commission in commission_lst)
                //{
                //    column = 1;
                //    foreach (PropertyInfo prop in propriedades)
                //    {
                //        object value = prop.GetValue(commission);
                //        worksheet.Cells[line, column] = value;
                //        column++;
                //    }
                //    line++;
                //}

                // Definir altura padrão para todas as linhas
                worksheet.Rows.RowHeight = worksheet.StandardHeight;

                // Salvar um ficheiro temporário para sacar o array de bytes
                workbook.SaveAs(tempPath, XlFileFormat.xlOpenXMLWorkbook);

                // Fechar o workbook e a app Excel
                workbook.Close(false, Type.Missing, Type.Missing);
                excelApp.Quit();

                // Ler bytes do ficheiro e apagar o ficheiro temporário
                byte[] fileBytes = File.ReadAllBytes(tempPath);
                File.Delete(tempPath);

                // Criar uma resposta HTTP com os bytes do ficheiro
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(fileBytes);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Export_Deals_BES.xlsx"
                };

                return response;
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Error: {ex.Message}");

                // Retornar resposta HTTP com erro
                HttpResponseMessage errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                errorResponse.Content = new StringContent("Erro ao exportar comissões para Excel.");
                return errorResponse;
            }
            finally
            {
                // Release dos objetos COM
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);
            }
        }






        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAllCommissions")]
        public IHttpActionResult GetAllCommissions()
        {          
            List<BB_Commission_General> commision_general_lst = new List<BB_Commission_General>();
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    commision_general_lst = db.BB_Commission_General.ToList();
                }
                return Ok(commision_general_lst);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return Content(HttpStatusCode.BadRequest, "Problem getting commissions.");
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCommission")]
        public IHttpActionResult GetCommission(int? commissionID)
        {
            BB_Commission_General commision = new BB_Commission_General();
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    if(commissionID != null)
                    {
                        commision = db.BB_Commission_General.Where(x => x.ID == commissionID).FirstOrDefault();
                    }
                }
                return Ok(commision);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return Content(HttpStatusCode.BadRequest, "Problem getting commissions.");
            }
        }

        // ######################################################################################

        // ---------------------------------------------------------------------------------------------------------------------
        // HELPERS -------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------------

        private double CalculatePremio(double value, string condicion)
        {
            switch (condicion)
            {
                case "4":
                case "3":
                case "0":
                    return value;

                case "1":
                    return value * 0.5;

                case "2":
                    return value * 0.25;

                default:
                    return 0;
            }
        }





        // ---------------------------------------------------------------------------------------------------------------------
        // CLASSES -------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------------

        public class GrossProfit
        {
            public double? GPTotal { get; set; }
            public double ComissionPercentage { get; set; }
            public double? CalculatedCommission { get; set; }

        }

        public class CommissionDictionary
        {
            public int? Commission { get; set; }
            //(penalizacao/bonificacao)
            public int? Adjustment { get; set; }
            public double? CalculatedCommission { get; set; }
            public List<Machine> Machines { get; set; }
        }

        public class Machine
        {
            public string CodeRef { get; set; }
            public string Description { get; set; }
            public int? Qty { get; set; }
            public double? DescPerClick { get; set; }
            public string PHC1 { get; set; }
            public string PHC4 { get; set;}
            public double? AppliedCommission { get; set;}
        }
    }
}
