using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class FacturacaoController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetListFacturacao")]
        public IHttpActionResult GetListFacturacao()
        {

            List<LD_ContratoModel> listModel = new List<LD_ContratoModel>();
            List<LD_Contrato> list = new List<LD_Contrato>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    //list = db.LD_Contrato.Where(x => x.IsClosed == true).ToList();
                    list = db.LD_Contrato.Where(x => x.IsFacturacao == true).ToList();



                    foreach (var i in list)
                    {
                        try
                        {
                            var config1 = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<LD_Contrato, LD_ContratoModel>();
                            });

                            IMapper iMapper1 = config1.CreateMapper();

                            LD_ContratoModel m = iMapper1.Map<LD_Contrato, LD_ContratoModel>(i);

                            BB_Proposal b = db.BB_Proposal.Where(x => x.ID == m.ProposalID).FirstOrDefault();

                            m.StatusName = db.LD_Status.Where(x => x.ID == m.StatusID).Select(x => x.Status).FirstOrDefault();
                            m.SistemaAssinatura = db.LD_Assinatura_System.Where(x => x.ID == m.SystemAssinaturaID).Select(x => x.System).FirstOrDefault();
                            m.MotivoDescricao = db.LD_Observacoes_Motivos.Where(x => x.ID == i.MotivoID).Select(x => x.Motive).FirstOrDefault();
                            m.DevolucaoMotivoDescricao = db.LD_Devolucao_Motivo.Where(x => x.ID == i.DevolucaoMotivoID).Select(x => x.Motivo).FirstOrDefault();
                            m.ComentariosDevolucao = i.ComentariosDevolucao;


                            LD_Contrato_Facturacao cf = db.LD_Contrato_Facturacao.Where(x => x.LDID == i.ID).FirstOrDefault();
                            if (cf != null)
                            {
                                m.FacuracaoModifiedby = cf.ModifiedBy;
                                m.FacuracaoModifiedtime = cf.ModifiedTime;
                                m.NUS = cf.NUS;
                                m.InvoiceAssistant = cf.InvoiceAssistant;
                            }


                            if (b == null)
                                b = new BB_Proposal();

                            BB_Clientes c = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();


                            if (c == null)
                                c = new BB_Clientes();

                            m.NCliente = c.accountnumber;
                            m.NomeCliente = c.Name;

                            using (var db1 = new masterEntities())
                            {
                                m.ModifiedBy = db1.AspNetUsers.Where(x => x.Email == i.ModifiedBy).Select(x => x.DisplayName).FirstOrDefault();
                                m.CreatedBy = db1.AspNetUsers.Where(x => x.Email == i.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                            }

                            using (var db2 = new BB_DB_DEVEntities2())
                            {
                                BB_Proposal_Financing f = db2.BB_Proposal_Financing.Where(x => x.ProposalID == b.ID).FirstOrDefault();
                                if (f != null && f.FinancingTypeCode != null)
                                {
                                    m.Financiamento = db2.BB_FinancingType.Where(x => x.Code == f.FinancingTypeCode).Select(x => x.Type).FirstOrDefault();
                                }
                                if (b.CampaignID == 0)
                                {
                                    m.TipoNegocio = "Negócio Tradicional";
                                }
                                else
                                {
                                    m.TipoNegocio = db2.BB_Campanha.Where(x => x.ID == b.CampaignID).Select(x => x.Campanha).FirstOrDefault();
                                }

                                if (f != null && f.FinancingTypeCode != null && f.FinancingTypeCode == 0)
                                {
                                    m.Financiamento = "NA";
                                }

                                if (b.CampaignID == 0 && f != null && f.FinancingTypeCode != null && f.FinancingTypeCode == 0)
                                {
                                    m.TipoNegocio = "Venda Directa";
                                }
                            }

                            listModel.Add(m);
                        }
                        catch (Exception ex)
                        {
                            return NotFound();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return NotFound();
            }

            listModel = listModel.OrderByDescending(x => x.ModifiedTime).ToList();
            return Ok(listModel);
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("GetListFacturacaoNew")]
        public IHttpActionResult GetListFacturacaoNew()
        {
            //List<LD_ContratoModel> listModel = new List<LD_ContratoModel>();
            List<LD_ContratoModel> listModel = new List<LD_ContratoModel>();
            List<LD_Contrato> list = new List<LD_Contrato>();
            try
            {
               
                try
                {
                    string bdConnect = @AppSettingsGet.BasedadosConnect;
                    using (SqlConnection conn = new SqlConnection(bdConnect))
                    {

                        conn.Open();

                        // 1.  create a command object identifying the stored procedure
                        SqlCommand cmd = new SqlCommand("get_Facturacao_ALL", conn);
                        cmd.CommandTimeout = 180;
                        // 2. set the command object so it knows to execute a stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                        SqlDataReader rdr = cmd.ExecuteReader();


                        // iterate through results, printing each to console
                        while (rdr.Read())
                        {


                            LD_ContratoModel m = new LD_ContratoModel();

                            //BB_Proposal b = db.BB_Proposal.Where(x => x.ID == m.ProposalID).FirstOrDefault();
                            m.ID = (int)rdr["ID"];
                            m.InvoiceAssistant = rdr["InvoiceAssistant"].ToString();
                            m.NUS = rdr["NUS"].ToString();
                            m.QuoteNumber = rdr["QuoteNumber"].ToString();
                            m.NCliente = rdr["NCliente"].ToString();
                            m.NomeCliente = rdr["NomeCliente"].ToString();
                            m.TipoNegocio = rdr["TipoNegocio"].ToString();
                            m.Financiamento = rdr["Financiamento"].ToString();
                            m.SistemaAssinatura = rdr["SistemaAssinatura"].ToString();
                            m.CreatedBy = rdr["CreatedBy"].ToString();
                            m.ModifiedBy = rdr["ModifiedBy"].ToString();
                            m.ModifiedTime = rdr["ModifiedTime"] != null && rdr["ModifiedTime"].ToString() != "" ? DateTime.Parse(rdr["ModifiedTime"].ToString()) : new DateTime();
                            m.FacuracaoModifiedby = rdr["FacuracaoModifiedby"].ToString();
                            m.FacuracaoModifiedtime = rdr["FacuracaoModifiedtime"] != null && rdr["FacuracaoModifiedtime"].ToString() != "" ? DateTime.Parse(rdr["FacuracaoModifiedtime"].ToString()) : new DateTime();
                            m.ProposalID = (int)rdr["ProposalID"];


                            listModel.Add(m);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            listModel = listModel.OrderByDescending(x => x.ModifiedTime).ToList();
            return Ok(listModel);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GravarNUS")]
        public IHttpActionResult GravarNUS(GravarNUSModel a)
        {
            LD_Contrato_Facturacao l = new LD_Contrato_Facturacao();
            LD_Contrato c = new LD_Contrato();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    l = db.LD_Contrato_Facturacao.Where(x => x.LDID == a.id).FirstOrDefault();
                    if (l == null)
                    {
                        l = new LD_Contrato_Facturacao();
                        l.LDID = a.id;
                        l.ModifiedBy = a.ModifiedBy;
                        l.ModifiedTime = DateTime.Now;
                        l.NUS = a.NUS;

                        db.LD_Contrato_Facturacao.Add(l);
                    }
                    else
                    {
                        l.ModifiedBy = a.ModifiedBy;
                        l.ModifiedTime = DateTime.Now;
                        l.NUS = a.NUS;
                        db.Entry(l).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    c = db.LD_Contrato.Where(x => x.ID == a.id).FirstOrDefault();

                    if (c != null)
                    {
                        c.IsClosed = true;
                        c.StatusID = 7;
                        db.Entry(c).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProposta")]
        public IHttpActionResult GetProposta(int? proposalID)
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

            if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0)
            {

                a.ProposalObj.Draft.details.ValueTotal = 0;
                foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                {
                    quote.TotalNetsale = sobrevalorizacao1 != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao1) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                    quote.UnitDiscountPrice = Math.Round((quote.TotalNetsale / quote.Qty), 2);
                    //quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;

                    a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                }
                a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
            }
            else
            {
                //a.ProposalObj.Draft.details.ValueTotal = 0;
                //foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                //{
                //    a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                //}
                //a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
            }

            using (var db1 = new BB_DB_DEV_LeaseDesk())
            {
                LD_Contrato c = db1.LD_Contrato.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                a.ProposalObj.NUS = db1.LD_Contrato_Facturacao.Where(x => x.LDID == c.ID).Select(x => x.NUS).FirstOrDefault();
                a.ProposalObj.FolderDoc = c.Pasta;
                a.ProposalObj.LeasedeskComentariosGC = c.ComentariosGC;
                a.ProposalObj.LeasedeskComentarios = c.Comments;
                a.ProposalObj.LeasedeskComentariosDevolucao = c.ComentariosDevolucao;
                a.ProposalObj.LeasedeskStatus = db1.LD_Observacoes_Motivos.Where(x => x.ID == c.MotivoID).Select(x => x.Motive).FirstOrDefault();
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






            return Ok(a.ProposalObj);
        }
    }
}
