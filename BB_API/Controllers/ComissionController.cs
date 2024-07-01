using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebGrease.Css.Ast;

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
                        { "IMS", new GrossProfit(){ GPTotal = 0,} },                   
                        { "PRS", new GrossProfit(){ GPTotal = 0,} },                    
                        { "OfficeHW", new GrossProfit(){ GPTotal = 0,} },
                        { "PPHW", new GrossProfit(){ GPTotal = 0,} },
                        { "IPHW", new GrossProfit(){ GPTotal = 0,} },
                        { "ITS_MCS_BPS_IMS", new GrossProfit(){ GPTotal = 0,} },
                        { "MCS", new GrossProfit(){ GPTotal = 0,} },
                        { "BPS", new GrossProfit(){ GPTotal = 0,} },
                        { "IMS_EXCLUDING", new GrossProfit(){ GPTotal = 0,} },
                        { "WPH", new GrossProfit(){ GPTotal = 0,} },
                        { "MOBOTIX", new GrossProfit(){ GPTotal = 0,} },
                    };


                    // --------------- PONTO 2 -------------->

                    List<string> mobotixCodRefs = db.BB_Data_Integration.Where(x => x.Description_Portuguese.Contains("MOBOTIX")).Select(x => x.CodeRef).ToList();

                    // função interna a ser chamada para fazer o somatório do GPTotal para cada família
                    void AddProfit(string family, double? amount, string codeRef)
                    {
                        if (family.EndsWith("HW") || family.EndsWith("CS"))
                        {
                            profitDictionary["HW"].GPTotal += amount ?? 0;
                        }

                        if (family.StartsWith("IMS"))
                        {
                            profitDictionary["IMS"].GPTotal += amount ?? 0;
                        }

                        if (family.StartsWith("PRS"))
                        {
                            profitDictionary["PRS"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("OPSHW") || family.Contains("PPHW") || family.EndsWith("CS"))
                        {
                            profitDictionary["OfficeHW"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("PPHW"))
                        {
                            profitDictionary["PPHW"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("IPHW"))
                        {
                            profitDictionary["IPHW"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("ITS") || family.Contains("MCS") || family.Contains("BPS") || family.Contains("IMS"))
                        {
                            profitDictionary["ITS_MCS_BPS_IMS"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("MCS"))
                        {
                            profitDictionary["MCS"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("BPS"))
                        {
                            profitDictionary["BPS"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("WPH"))
                        {
                            profitDictionary["WPH"].GPTotal += amount ?? 0;
                        }

                        // Familias IMS com o exclude da lista de codRefs Mobotix
                        if (family.Contains("IMS") && !mobotixCodRefs.Contains(codeRef))
                        {
                            profitDictionary["IMS_EXCLUDING"].GPTotal += amount ?? 0;
                        }

                        if (family.Contains("Mobotix"))
                        {
                            profitDictionary["MOBOTIX"].GPTotal += amount ?? 0;
                        }
                    }
                    
                    // Cálculo do GPTotal para cada familia de cada maquina

                    foreach (var oneShot_Item in oneShot)
                    {
                        AddProfit(oneShot_Item.Family, oneShot_Item.GPTotal, oneShot_Item.CodeRef);
                    }

                    foreach (var servRecor_Item in servicosRecorrentes)
                    {
                        AddProfit(servRecor_Item.Family, servRecor_Item.GPTotal, servRecor_Item.CodeRef);
                    }

                    var profit_Hard = profitDictionary["HW"];
                    var profit_IMS = profitDictionary["IMS"];
                    var profit_PRS = profitDictionary["PRS"];
                    var profit_OfficeHW = profitDictionary["OfficeHW"];
                    var profit_PPHW = profitDictionary["PPHW"];
                    var profit_IPHW = profitDictionary["IPHW"];
                    var profit_ITS_MCS_BPS_IMS = profitDictionary["ITS_MCS_BPS_IMS"];
                    var profit_MCS = profitDictionary["MCS"];
                    var profit_BPS = profitDictionary["BPS"];
                    var profit_WPH = profitDictionary["WPH"];

                    var profit_IMS_EXCLUDING = profitDictionary["IMS_EXCLUDING"];
                    var profit_MOBOTIX = profitDictionary["MOBOTIX"];


                    // --------------- PONTO 3 -------------->

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();

                    bool isNewClient = proposal.ClientAccountNumber.StartsWith("P");
                    bool isNewBusinessLine = loadProposal.ProposalObj.Draft.baskets.newBusinessLine ?? false;

                    // Definicao da percentagem de comissao a aplicar a cada familia
                    if (!isNewClient)
                    {
                        profit_Hard.ComissionPercentage = 9;
                        profit_IMS.ComissionPercentage = 9;
                        profit_PRS.ComissionPercentage = 9;
                        profit_OfficeHW.ComissionPercentage = 9;
                        profit_PPHW.ComissionPercentage = 9;
                        profit_IPHW.ComissionPercentage = 9;
                        profit_ITS_MCS_BPS_IMS.ComissionPercentage = 9;
                        profit_MCS.ComissionPercentage = 9;
                        profit_BPS.ComissionPercentage = 9;
                        profit_IMS_EXCLUDING.ComissionPercentage = 9;
                        profit_WPH.ComissionPercentage = 9;
                        profit_MOBOTIX.ComissionPercentage = 9;
                    }
                    else if (isNewBusinessLine)
                    {
                        profit_Hard.ComissionPercentage = 12.5;
                        profit_IMS.ComissionPercentage = 12.5;
                        profit_PRS.ComissionPercentage = 12.5;
                        profit_OfficeHW.ComissionPercentage = 12.5;
                        profit_PPHW.ComissionPercentage = 12.5;
                        profit_IPHW.ComissionPercentage = 12.5;
                        profit_ITS_MCS_BPS_IMS.ComissionPercentage = 12.5;
                        profit_MCS.ComissionPercentage = 12.5;
                        profit_BPS.ComissionPercentage = 12.5;
                        profit_IMS_EXCLUDING.ComissionPercentage = 12.5;
                        profit_WPH.ComissionPercentage = 12.5;
                        profit_MOBOTIX.ComissionPercentage = 12.5;
                    }
                    else
                    {
                        profit_Hard.ComissionPercentage = 15.5;
                        profit_IMS.ComissionPercentage = 15.5;
                        profit_PRS.ComissionPercentage = 15.5;
                        profit_OfficeHW.ComissionPercentage = 15.5;
                        profit_PPHW.ComissionPercentage = 15.5;
                        profit_IPHW.ComissionPercentage = 15.5;
                        profit_ITS_MCS_BPS_IMS.ComissionPercentage = 15.5;
                        profit_MCS.ComissionPercentage = 15.5;
                        profit_BPS.ComissionPercentage = 15.5;
                        profit_IMS_EXCLUDING.ComissionPercentage = 15.5;
                        profit_WPH.ComissionPercentage = 15.5;
                        profit_MOBOTIX.ComissionPercentage = 15.5;
                    }


                    // Valor do GPTotal acrescido da comissao definida acima
                    // Exemplo: CalculatedCommission = GPTotal * 0.09

                    profit_Hard.CalculatedCommission = profit_Hard.GPTotal * (profit_Hard.ComissionPercentage / 100);
                    profit_IMS.CalculatedCommission = profit_IMS.GPTotal * (profit_IMS.ComissionPercentage / 100);
                    profit_PRS.CalculatedCommission = profit_PRS.GPTotal * (profit_PRS.ComissionPercentage / 100);
                    profit_OfficeHW.CalculatedCommission = profit_OfficeHW.GPTotal * (profit_OfficeHW.ComissionPercentage / 100);
                    profit_PPHW.CalculatedCommission = profit_PPHW.GPTotal * (profit_PPHW.ComissionPercentage / 100);
                    profit_IPHW.CalculatedCommission = profit_IPHW.GPTotal * (profit_IPHW.ComissionPercentage / 100);
                    profit_ITS_MCS_BPS_IMS.CalculatedCommission = profit_ITS_MCS_BPS_IMS.GPTotal * (profit_ITS_MCS_BPS_IMS.ComissionPercentage / 100);
                    profit_MCS.CalculatedCommission = profit_MCS.GPTotal * (profit_MCS.ComissionPercentage / 100);
                    profit_BPS.CalculatedCommission = profit_BPS.GPTotal * (profit_BPS.ComissionPercentage / 100);
                    profit_IMS_EXCLUDING.CalculatedCommission = profit_IMS_EXCLUDING.GPTotal * (profit_IMS_EXCLUDING.ComissionPercentage / 100);
                    profit_WPH.CalculatedCommission = profit_WPH.GPTotal * (profit_WPH.ComissionPercentage / 100);
                    profit_MOBOTIX.CalculatedCommission = profit_MOBOTIX.GPTotal * (profit_MOBOTIX.ComissionPercentage / 100);


                    // --------------- PONTO 4 -------------->

                    // Calculo da comissao a aplicar a familias do dicionario protocolDictionary

                    List<Machine> machines = new List<Machine>();

                    double? pvpClick;
                    double? vendaClick;

                    var protocolDictionary = new Dictionary<string, CommissionDictionary>()
                    {
                        { "Printing A3_Colour", new CommissionDictionary(){ Commission = 20, Adjustment = 5} },
                        { "Printing A3_BW", new CommissionDictionary(){ Commission = 8, Adjustment = 5} },
                        { "Printing A4_Colour", new CommissionDictionary(){ Commission = 9, Adjustment = 5} },
                        { "Printing A4_BW", new CommissionDictionary(){ Commission = 4, Adjustment = 5} },
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
                                PHC4 = equipamento.PHC2
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

                                    protocolDictionary[key].Machines.Add(machine);
                                };

                            }
                        }
                    };


                    // --------------------------------------------------
                    // Construcao do modelo para o insert
                    // --------------------------------------------------

                    using (var dbUsers = new masterEntities())
                        {
                            AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.Email == proposal.AccountManager).FirstOrDefault();
                            
                            bb_commission_general.Vendedor = user.DisplayName;
                            
                            
                            
                            bb_commission_general.Numero_Vendedor = user.ErpNumber;
                            bb_commission_general.Manager = user.Manager;

                            bb_commission_general.Numero_Manager = dbUsers.AspNetUsers
                                                                    .Where(x => x.Email == user.ManagerEmail)
                                                                    .Select(x => x.ErpNumber)
                                                                    .FirstOrDefault();

                            bb_commission_general.Agencia = user.Location;
                            bb_commission_general.Codigo_Agencia = "NAO TEMOS";
                            bb_commission_general.Sales_Group = "540"; // hardcoded
                        }

                    DateTime? modifiedDate = db.LD_Contrato.Where(x => x.ProposalID == proposalID).Select(x => x.ModifiedTime).FirstOrDefault();

                    if (modifiedDate.HasValue)
                    {
                        // exemplo:  01-02-2023 => 2302
                        bb_commission_general.Ano_Mes_CN = ((modifiedDate.Value.Year % 100) * 100) + modifiedDate.Value.Month;
                    }
                     
                    bb_commission_general.Periodo_Solicitado = bb_commission_general.Ano_Mes_CN;
                    bb_commission_general.HR_Comentario = null;
                    bb_commission_general.Invoice_List = null;
                    bb_commission_general.BB_Numero = proposalID.ToString();
                    bb_commission_general.BB_Numero_Entero = proposal.CreatedTime.Value.Year + proposalID.ToString();
                    bb_commission_general.SAP_Numero = null;
                    bb_commission_general.Numero_Cliente = loadProposal.ProposalObj.Draft.client.accountnumber;
                    bb_commission_general.Cliente = loadProposal.ProposalObj.Draft.client.Name;
                    bb_commission_general.Facturación = null;
                    bb_commission_general.Cifra_Negocio = proposal.SubTotal;

                    // Total_Margin => Soma de todos os GP daquele proposalID (incluindo RS)
                    bb_commission_general.Margen_Total = profitDictionary.Where(d => d.Key != "OfficeHW").Sum(x => x.Value.GPTotal);

                    // Same as above
                    bb_commission_general.Margen_Total_Nueva = bb_commission_general.Margen_Total;

                    // soma de todas as familias do estilo: profit_Hard.CalculatedCommission + profit_OfficeHW.CalculatedCommission + .....
                    bb_commission_general.Comision_Sobre_Margen = profit_OfficeHW.CalculatedCommission +
                                                                 profit_IMS.CalculatedCommission +
                                                                 profit_PRS.CalculatedCommission +
                                                                 profit_PPHW.CalculatedCommission +
                                                                 profit_IPHW.CalculatedCommission +
                                                                 profit_ITS_MCS_BPS_IMS.CalculatedCommission +
                                                                 profit_MCS.CalculatedCommission +
                                                                 profit_BPS.CalculatedCommission +
                                                                 profit_IMS_EXCLUDING.CalculatedCommission +
                                                                 profit_WPH.CalculatedCommission +
                                                                 profit_MOBOTIX.CalculatedCommission;

                    // ponto 4 A3_COLOR... regras 

                    bb_commission_general.Comision_Mantenimiento = protocolDictionary.Values
                        .Where(cd => cd.Machines != null)
                        .SelectMany(cd => cd.Machines)
                        .Sum(m => m.AppliedCommission ?? 0);


                    bb_commission_general.Comisiones = bb_commission_general.Comision_Sobre_Margen + bb_commission_general.Comision_Mantenimiento;
                    bb_commission_general.Margen = null;

                    var basket = loadProposal.ProposalObj.Draft.baskets.os_basket;

                    bb_commission_general.CN_HW = basket.Where(x => x.Family.Contains("HW")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_HW = profit_Hard.GPTotal;
                    bb_commission_general.Margen_HW_Nuevo = bb_commission_general.Margen_HW;

                    bb_commission_general.CN_Office_HW = basket.Where(x => x.Family.Contains("OPSHW") || x.Family.Contains("PPHW")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_Office_HW = profit_OfficeHW.GPTotal;

                    bb_commission_general.CN_PP_HW = basket.Where(x => x.Family.Contains("PPHW")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_PP_HW = profit_PPHW.GPTotal;

                    bb_commission_general.CN_IP_HW = basket.Where(x => x.Family.Contains("IPHW")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_IP_HW = profit_IPHW.GPTotal;

                    bb_commission_general.CN_ITS = basket.Where(x => x.Family.Contains("ITS") || x.Family.Contains("MCS") || x.Family.Contains("BPS") || x.Family.Contains("IMS")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_ITS = profit_ITS_MCS_BPS_IMS.GPTotal;

                    bb_commission_general.CN_PRS = basket.Where(x => x.Family.Contains("PRS")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_PRS = profit_PRS.GPTotal;

                    bb_commission_general.CN_MCS = basket.Where(x => x.Family.Contains("MCS")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_MCS = profit_MCS.GPTotal;

                    bb_commission_general.CN_BPS = basket.Where(x => x.Family.Contains("BPS")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_BPS = profit_BPS.GPTotal;

                    bb_commission_general.CN_IMS = 0;
                    
                    // Familias IMS com o exclude dos mobotix
                    foreach(var b in basket)
                    {
                        if(b.Family.Contains("IMS") && !mobotixCodRefs.Contains(b.CodeRef))
                        {
                            bb_commission_general.CN_IMS += b.TotalNetsale;
                        }
                        //bb_commission_general.CN_IMS += basket.Where(x => x.Family.Contains("IMS") && !mobotixCodRefs.Contains(b.CodeRef)).Select(x => x.TotalNetsale).FirstOrDefault();
                    }

                    bb_commission_general.Margen_IMS = profit_IMS.GPTotal;

                    bb_commission_general.CN_WPH = basket.Where(x => x.Family.Contains("WPH")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_WPH = profit_WPH.GPTotal;

                    bb_commission_general.CN_Mobotix = basket.Where(x => x.Family.Contains("Mobotix")).Sum(x => x.TotalNetsale);
                    bb_commission_general.Margen_Mobotix = profit_MOBOTIX.GPTotal;

                    bb_commission_general.Pagado = null;
                    bb_commission_general.Controlado = null;
                    bb_commission_general.Comisionado = null;
                    bb_commission_general.Incidencia = null;
                    bb_commission_general.Excluido = null;
                    bb_commission_general.Es_Segunda_Mano = isSecondHand;
                    bb_commission_general.Es_Doc_Share = null;
                    bb_commission_general.Es_GMA = loadProposal.ProposalObj.Draft.baskets.GMA;
                    bb_commission_general.Es_Invoice_List = bb_commission_general.Invoice_List;
                    bb_commission_general.Support_BEU = loadProposal.ProposalObj.Draft.baskets.BEUSupport;
                    //bb_commission_general.CBB = loadProposal.ProposalObj.Draft.baskets.CBB;
                    bb_commission_general.Numero_Cliente_SAP = bb_commission_general.Numero_Cliente;
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

                    bb_commission_general.Metodo_Pago_Productos = db.BB_FinancingPaymentMethod.Where(x => x.ID == loadProposal.ProposalObj.Draft.financing.PaymentMethodId).Select(x => x.Type).FirstOrDefault();

                    // Perguntar ao Luis?
                    bb_commission_general.Metodo_Pago_Mantenimiento = "ADEUDO DIRECTO";

                    bb_commission_general.CreatedDate = DateTime.Now;
                    bb_commission_general.CreatedBy = null;
                    bb_commission_general.ModifiedDate = null;
                    bb_commission_general.ModifiedBy = null;


                    //LOGS Column -------------------------------------------------------------------------------------
                    string logPhase_1 = string.Format("({0} MG41 - Comision sobre el margen = (Margen Hw({1}) + Margen ITS ({2}) + " +
                    "Margen IMS ({3})) + Comisión de margen (cliente)({4}%) - ({5}%) = {6})",
                    proposalID, // 0
                    bb_commission_general.Margen_HW,  // 1
                    bb_commission_general.Margen_ITS, // 2
                    bb_commission_general.Margen_IMS, // 3
                    profit_Hard.ComissionPercentage,  // 4
                    0, // 5
                    bb_commission_general.Comision_Sobre_Margen // 6
                    );

                    string newLine = "\n \n";

                    string opType = campaignID == 6 ? "<" : ">";

                    string logPhase_2 = string.Format("({0} - GPFull Log Com GP CA: NET SALES ({1}))" +
                        "{2} 0 | CA HARD = {3} AND GP HARD = {4} AND %GP HARD = {5})",
                    proposalID,                             // 0
                    bb_commission_general.Cifra_Negocio,    // 1
                    opType,                                 // 2
                    bb_commission_general.CN_HW,            // 3
                    bb_commission_general.Margen_HW,        // 4
                    (bb_commission_general.Margen_HW * 100) / bb_commission_general.CN_HW // 5
                    );

                    string logFinal = logPhase_1 + newLine + logPhase_2 + newLine + "\n";

                    //string logPhase_3 = string.Format("({0})"
                    //);


                    bb_commission_general.Logs = logFinal;

                }

                // ---------------------------------------------
                // TESTS MYLENE --------------------------------
                //                                            --
                var obj_Mylene = bb_commission_general;      //-
                //                                            --
                // ---------------------------------------------
                // ---------------------------------------------

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
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
