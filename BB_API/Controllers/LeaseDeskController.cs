using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Office.Interop.Word;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.SetupXML;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class LeaseDeskController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("PrazoDiferenciadoPedido")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> PrazoDiferenciadoPedido(ProposalRootObject p)
        {
            string financingTypeCode = "";

            AspNetUsers username = new AspNetUsers();
            int? proposalID = p.Draft.details.ID;
            ActionResponse err = new ActionResponse();
            if (proposalID == null || proposalID == 0)
            {
                ProposalBLL pro = new ProposalBLL();
                err = pro.ProposalDraftSaveAs(p);
                proposalID = err.ProposalIDReturn;
            }
            else
            {
                ProposalBLL pro = new ProposalBLL();
                err = pro.ProposalDraftSave(p);
            }

            ActionResponse ac = new ActionResponse();
            try
            {
                string clientName = p.Draft.client.Name;
                string accountNumber = p.Draft.client.accountnumber;
                string nif = "";
                double recurringServices = 0;

                BB_Proposal bb = new BB_Proposal();
                BB_Proposal_Vva vva = new BB_Proposal_Vva();
                BB_Proposal_Financing f = new BB_Proposal_Financing();
                BB_Proposal_PrazoDiferenciado pd;

                double taxaMensalServicosRecorrentes = 0;

                using (var db = new BB_DB_DEVEntities2())
                {
                    pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    f = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    recurringServices = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).Sum(x => x.TotalNetsale).GetValueOrDefault() + db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).Sum(x => x.UnitDiscountPrice * x.TotalMonths * x.Quantity).GetValueOrDefault();
                    taxaMensalServicosRecorrentes = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).Sum(x => x.MonthlyFee).GetValueOrDefault() + db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).Sum(x => x.UnitDiscountPrice).GetValueOrDefault();
                }

                if (p.Draft.details.CRM_QUOTE_ID == null || p.Draft.details.CRM_QUOTE_ID == "")
                {
                    ac.Message = "Preenchimento do campo QuoteNumber é obrigatório!";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
                }
                if (pd != null && pd.IsComplete.GetValueOrDefault() == false)
                {
                    ac.Message = "Tem um pedido pendente no LeaseDesk para esta proposta, não é possivel fazer a operação de momento!";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
                }

                if (p.Draft.client.NIF == null || p.Draft.client.NIF == "")
                {
                    ac.Message = "Este cliente não possiu NIF, verifique em CRM ou contacte a equipa de BB!";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
                }
                using (var db1 = new masterEntities())
                {
                    username = db1.AspNetUsers.Where(x => x.Email == p.Draft.details.CreatedBy).FirstOrDefault();
                }

                using (var db = new BB_DB_DEVEntities2())
                {
                    bb = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    financingTypeCode = db.BB_FinancingType.Where(x => x.Code == p.Draft.financing.FinancingTypeCode).Select(x => x.Type).FirstOrDefault();

                    if (pd == null)
                    {
                        pd = new BB_Proposal_PrazoDiferenciado();
                        db.Entry(bb).State = bb.ID == 0 ? EntityState.Added : EntityState.Modified;
                        nif = db.BB_Clientes.Where(x => x.accountnumber == bb.ClientAccountNumber).Select(x => x.NIF).FirstOrDefault();

                        pd.FinancingID = p.Draft.financing.FinancingTypeCode;
                        pd.CreatedBy = p.Draft.details.CreatedBy;
                        pd.CreatedTime = DateTime.Now.AddHours(-1);
                        pd.ValorFinanciamento = bb.SubTotal - recurringServices;
                        pd.PrazoDiferenciado = p.Months;
                        pd.Frequency = p.Frequency;
                        pd.ProposalID = proposalID;
                        pd.IsComplete = false;
                        pd.GestorContaObservacoes = p.GestorContaObservacoes;
                        pd.Type = p.Type;
                        pd.NLocadora = "";
                        pd.DSO = username.Location;
                        pd.DataExpiracao = null;
                        double renda_ = 0;

                        switch (p.Frequency)
                        {
                            case 1:
                                foreach (var i in p.Draft.financing.FinancingFactors.Monthly)
                                {
                                    if (i.Contracto == p.Months)
                                    {
                                        renda_ = i.Value;

                                    }
                                }
                                break;
                            case 3:
                                foreach (var i in p.Draft.financing.FinancingFactors.Trimestral)
                                {
                                    if (i.Contracto == p.Months)
                                    {
                                        renda_ = i.Value;

                                    }
                                }
                                break;
                            default:
                                break;
                        }




                        if (p.Draft.financing.FinancingTypeCode == 6)
                        {
                            if (p.Draft.printingServices2.ActivePrintingService != null && p.Draft.printingServices2.ActivePrintingService.Value > 0)
                            {
                                ApprovedPrintingService aps = p.Draft.printingServices2.ApprovedPrintingServices[p.Draft.printingServices2.ActivePrintingService.Value - 1];
                                if (aps.GlobalClickVVA != null)
                                {
                                    renda_ = renda_ + aps.GlobalClickVVA.PVP;
                                }
                            }

                            renda_ = renda_ + taxaMensalServicosRecorrentes;
                        }

                        //if (p.Draft.financing != null && p.Draft.financing.IncludeServices == true && renda_ > 0 && f.FinancingTypeCode != 6)
                        //{
                        //    if (p.Draft.printingServices2.ActivePrintingService != null && p.Draft.printingServices2.ActivePrintingService.Value > 0)
                        //    {
                        //        ApprovedPrintingService aps = p.Draft.printingServices2.ApprovedPrintingServices[p.Draft.printingServices2.ActivePrintingService.Value - 1];
                        //        if (aps.GlobalClickVVA != null)
                        //        {
                        //            renda_ = renda_ - aps.GlobalClickVVA.PVP;
                        //        }
                        //    }
                        //}



                        pd.ValorRenda = Math.Round(renda_, 2);

                        db.BB_Proposal_PrazoDiferenciado.Add(pd);


                    }
                    else
                    {
                        //if (p.Type == "DIFF")
                        //    bb.StatusID = 9;

                        pd.FinancingID = p.Draft.financing.FinancingTypeCode;
                        pd.CreatedBy = p.Draft.details.CreatedBy;
                        pd.CreatedTime = DateTime.Now.AddHours(-1);
                        pd.ValorFinanciamento = bb.SubTotal - recurringServices;
                        pd.PrazoDiferenciado = p.Months;
                        pd.Frequency = p.Frequency;
                        pd.ProposalID = proposalID;
                        pd.IsComplete = false;
                        pd.GestorContaObservacoes = p.GestorContaObservacoes;
                        pd.Type = p.Type;
                        pd.IsAproved = null;
                        pd.ModifiedBy = null;
                        pd.ModifiedTime = null;
                        pd.NLocadora = "";
                        pd.DSO = username.Location;
                        double renda_ = 0;
                        pd.DataExpiracao = null;

                        switch (p.Frequency)
                        {
                            case 1:
                                foreach (var i in p.Draft.financing.FinancingFactors.Monthly)
                                {
                                    if (i.Contracto == p.Months)
                                    {
                                        renda_ = i.Value;
                                    }
                                }
                                break;
                            case 3:
                                foreach (var i in p.Draft.financing.FinancingFactors.Trimestral)
                                {
                                    if (i.Contracto == p.Months)
                                    {
                                        renda_ = i.Value;
                                    }
                                }
                                break;
                            default:
                                break;
                        }


                        //Renda flexpage
                        //if (p.Draft.financing.FinancingTypeCode == 6)
                        //{
                        //    if (p.Draft.printingServices2.ActivePrintingService != null && p.Draft.printingServices2.ActivePrintingService.Value > 0)
                        //    {
                        //        ApprovedPrintingService aps = p.Draft.printingServices2.ApprovedPrintingServices[p.Draft.printingServices2.ActivePrintingService.Value - 1];
                        //        if (aps.GlobalClickVVA != null)
                        //        {
                        //            renda_ = renda_ + aps.GlobalClickVVA.PVP;
                        //        }
                        //    }

                        //    renda_ = renda_ + taxaMensalServicosRecorrentes;
                        //}


                        //if (p.Draft.financing != null && p.Draft.financing.IncludeServices == true && renda_ > 0 && f.FinancingTypeCode != 6)
                        //{
                        //    if (p.Draft.printingServices2.ActivePrintingService != null && p.Draft.printingServices2.ActivePrintingService.Value > 0)
                        //    {
                        //        ApprovedPrintingService aps = p.Draft.printingServices2.ApprovedPrintingServices[p.Draft.printingServices2.ActivePrintingService.Value - 1];
                        //        if (aps.GlobalClickVVA != null)
                        //        {
                        //            renda_ = renda_ - aps.GlobalClickVVA.PVP;
                        //        }
                        //    }
                        //}

                        pd.ValorRenda = Math.Round(renda_, 2);


                        db.Entry(pd).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    db.Entry(bb).State = EntityState.Modified;
                    db.SaveChanges();


                    //GRAVAR A QUOTE
                    List<BB_Proposal_Aprovacao_Quote> quotes = db.BB_Proposal_Aprovacao_Quote.Where(x => x.Proposal_ID == proposalID).ToList();

                    db.BB_Proposal_Aprovacao_Quote.RemoveRange(quotes);
                    db.SaveChanges();

                    List<BB_Proposal_Quote> lstQuote = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                    foreach (BB_Proposal_Quote quote in lstQuote)
                    {
                        BB_Proposal_Aprovacao_Quote approvalQuote = new BB_Proposal_Aprovacao_Quote
                        {
                            ClickPriceBW = quote.ClickPriceBW,
                            ClickPriceC = quote.ClickPriceC,
                            CodeRef = quote.CodeRef,
                            Description = quote.Description,
                            DiscountPercentage = quote.DiscountPercentage,
                            Family = quote.Family,
                            GPPercentage = quote.GPPercentage,
                            GPTotal = quote.GPTotal,
                            IsFinanced = quote.IsFinanced,
                            IsMarginBEU = quote.IsMarginBEU,
                            IsUsed = quote.IsUsed,
                            Locked = quote.Locked,
                            Margin = quote.Margin,
                            Name = quote.Name,
                            Proposal_ID = proposalID,
                            PVP = quote.PVP,
                            Qty = quote.Qty,
                            TCP = quote.TCP,
                            TotalCost = quote.TotalCost,
                            TotalNetsale = quote.TotalNetsale,
                            TotalPVP = quote.TotalPVP,
                            UnitDiscountPrice = quote.UnitDiscountPrice,
                            UnitPriceCost = quote.UnitPriceCost
                        };
                        db.BB_Proposal_Aprovacao_Quote.Add(approvalQuote);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ac.Message = "Contacte a equipa de BB!" + ex.Message.ToString();
                            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);

                        }
                    }
                    List<BB_Proposal_OPSImplement> opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();
                    foreach (BB_Proposal_OPSImplement opsi in opsImplement)
                    {
                        BB_Proposal_Aprovacao_Quote approvalQuote = new BB_Proposal_Aprovacao_Quote
                        {
                            CodeRef = opsi.CodeRef,
                            Description = opsi.Name + " - " + opsi.Description,
                            Family = opsi.Family,
                            IsUsed = false,
                            Proposal_ID = proposalID,
                            Qty = opsi.Quantity,
                            TotalNetsale = opsi.UnitDiscountPrice * opsi.Quantity,
                            UnitDiscountPrice = opsi.UnitDiscountPrice,
                        };
                        db.BB_Proposal_Aprovacao_Quote.Add(approvalQuote);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ac.Message = "Contacte a equipa de BB!" + ex.Message.ToString();
                            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
                        }
                    }
                }


                string dest = AppSettingsGet.PedidoPrazoDiferenciadoEmail;
                //EMAIL SEND
                EmailService emailSend = new EmailService();
                EmailMesage message = new EmailMesage();

                string sub = "";
                if (p.Type == "DIFF")
                {
                    sub = "Business Builder - DIFF - " + accountNumber + " - " + nif + " - " + financingTypeCode + " - " + username.DisplayName;
                    err.Message = "Pedido de prazo financiamento efectuado com sucesso!";
                }
                else
                {
                    sub = "Business Builder - TAB - " + accountNumber + " - " + nif + " - " + financingTypeCode + " - " + username.DisplayName;
                    err.Message = "Pedido de aprovação financeira efectuado com sucesso!";
                }

                string frequencyEmailString = "";
                if (p.Frequency == 1)
                {
                    frequencyEmailString = "mes(es)";
                }
                else
                {
                    frequencyEmailString = "trimestre(s)";
                }

                message.Destination = dest;
                message.Subject = sub;
                //message.Subject = "Business Team - ";
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                strBuilder.Append("<br/>");
                strBuilder.Append("Foi solicitado um pedido de informação para o Prazo de " + p.Months + " " + frequencyEmailString + ", para o seguinte cliente:");
                strBuilder.Append("<br/>");
                strBuilder.Append("Cliente: " + clientName);
                strBuilder.Append("<br/>");
                strBuilder.Append("NIF: " + nif);
                strBuilder.Append("<br/>");
                strBuilder.Append("Para aceder à aplicação Business Builder e responder, por favor utilize o seguinte link: " + "https://bb.konicaminolta.pt/LeaseDeskRent/Index" + " e selecionar a opção Aprovações Pendentes.");
                strBuilder.Append("<br/>");
                strBuilder.Append("<br/>");
                strBuilder.Append("Business Builder - Por favor, não responder a este email.");
                message.Body = strBuilder.ToString();

                await emailSend.SendEmailaync(message);


                return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
            }
            catch (Exception ex)
            {
                EmailService emailSend = new EmailService();
                EmailMesage message = new EmailMesage();
                message.Destination = "joao.reis@konicaminolta.pt";
                message.Subject = "ERRo";
                //message.Subject = "Business Team - ";
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                strBuilder.Append("Business Builder - Por favor, não responder a este email.");
                strBuilder.Append("NIF: " + ex.Message.ToString());
                strBuilder.Append("NIF: " + ex.InnerException.ToString());
                message.Body = strBuilder.ToString();

                await emailSend.SendEmailaync(message);

                return Request.CreateResponse<ActionResponse>(HttpStatusCode.InternalServerError, ac);
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("PrazoDiferenciadoSave")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> PrazoDiferenciadoSave(PrazoDiferenciado p)
        {
            BB_Proposal_PrazoDiferenciado prazoDif = new BB_Proposal_PrazoDiferenciado();
            ActionResponse ac = new ActionResponse();
            try
            {
                using (var db1 = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_PrazoDiferenciado pd = db1.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == p.ProposalID).FirstOrDefault();
                    if (pd.ValorFinanciamento > 0)
                    {

                        string type_contract = db1.BB_FinancingType.Where(x => x.Code == pd.FinancingID).Select(x => x.Type).FirstOrDefault();

                        p.ValorFactor = db1.BB_Bank_Bnp.Where(x => x.Type_Duration == "M" && x.Type_Contract == type_contract && x.Contracto == p.nrMeses && x.Volume_Start <= pd.ValorFinanciamento && pd.ValorFinanciamento < x.Volume_End).Select(x => x.Value).FirstOrDefault();

                        if (p.ValorFactor == null && p.ValorRenda > 0 && pd.ValorFinanciamento > 0)
                        {
                            p.ValorFactor = p.ValorRenda / pd.ValorFinanciamento;
                        }
                    }
                }



                using (var db1 = new BB_DB_DEVEntities2())
                {
                    if (p.isApproved == null)
                    {
                        prazoDif = db1.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == p.ProposalID).FirstOrDefault();
                        prazoDif.ValorFactor = p.ValorFactor;
                        prazoDif.ValorRenda = p.ValorRenda;
                        prazoDif.Commets = p.comments;
                        prazoDif.ModifiedTime = DateTime.Now;
                        prazoDif.ModifiedBy = p.ModifiedBy;
                        prazoDif.Alocadora = p.Alocadora;
                        prazoDif.PrazoDiferenciado = p.nrMeses;
                        //prazoDif.Frequency = p.Frequency;
                        prazoDif.NLocadora = p.NLocadora;

                        //prazoDif.Frequency = null;
                        //prazoDif.IsAproved = null;
                        db1.Entry(prazoDif).State = EntityState.Modified;
                        db1.SaveChanges();
                        return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
                    }
                }

                DateTime dataExpeiracao = DateTime.Now;
                if (p.Alocadora == "BNP")
                {
                    dataExpeiracao = dataExpeiracao.AddDays(28);
                }
                if (p.Alocadora == "GRENK")
                {
                    dataExpeiracao = dataExpeiracao.AddDays(28);
                }
                if (p.Alocadora == "KM")
                {
                    dataExpeiracao = dataExpeiracao.AddDays(28);
                }

                using (var db = new BB_DB_DEVEntities2())
                {
                    prazoDif = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == p.ProposalID).FirstOrDefault();
                    prazoDif.ValorFactor = p.ValorFactor;
                    prazoDif.ValorRenda = p.ValorRenda;
                    prazoDif.IsComplete = true;
                    prazoDif.IsAproved = p.isApproved;
                    prazoDif.Commets = p.comments;
                    prazoDif.ModifiedTime = DateTime.Now;
                    prazoDif.ModifiedBy = p.ModifiedBy;
                    prazoDif.Alocadora = p.Alocadora;
                    prazoDif.PrazoDiferenciado = p.nrMeses;
                    //prazoDif.Frequency = p.Frequency;
                    prazoDif.NLocadora = p.NLocadora;
                    prazoDif.DataExpiracao = dataExpeiracao;
                    db.Entry(prazoDif).State = EntityState.Modified;
                    db.SaveChanges();
                }

                BB_Proposal bb = new BB_Proposal();
                string clientName = "";
                using (var db = new BB_DB_DEVEntities2())
                {
                    bb = db.BB_Proposal.Where(x => x.ID == p.ProposalID).FirstOrDefault();
                    //bb.StatusID = 10;
                    db.Entry(bb).State = EntityState.Modified;
                    db.SaveChanges();

                    clientName = db.BB_Clientes.Where(x => x.accountnumber == bb.ClientAccountNumber).Select(x => x.Name).FirstOrDefault();
                }

                string isApproved = p.isApproved.GetValueOrDefault() ? "Aprovado" : "Recusado";
                string approvalType = prazoDif.Type == "DIFF" ? "Prazo Diferenciado" : "Aprovação Financeira";


                string frequencyEmailString = "";
                if (p.Frequency == 3)
                {
                    frequencyEmailString = "Trimestre(s)";
                }
                else
                {
                    frequencyEmailString = "Mes(es)";
                }
                //EMAIL SEND
                EmailService emailSend = new EmailService();
                EmailMesage message = new EmailMesage();

                message.Destination = bb.CreatedBy;
                message.Subject = "BB - " + approvalType + ": " + isApproved + " - " + clientName;
                StringBuilder strBuilder = new StringBuilder();

                strBuilder.Append("Caro(a) Gestor(a),<br/>");
                strBuilder.Append("O seguinte pedido de <b>" + approvalType + "</b> foi <b>" + isApproved + "</b><br/>");
                strBuilder.Append("<br/>");
                if (bb.Name != null) strBuilder.Append("<b>Oportunidade</b>: " + bb.Name + "<br/>");
                if (bb.CRM_QUOTE_ID != null) strBuilder.Append("<b>Quote CRM</b>: " + bb.CRM_QUOTE_ID + "<br/>");
                strBuilder.Append("<b>ID Interno</b>: " + bb.ID + "<br/>");
                if (clientName != null) strBuilder.Append("<b>Cliente</b>: " + clientName + "<br/>");
                strBuilder.Append("<br/>");

                if (prazoDif.IsAproved != null) strBuilder.Append("<b>Pedido de " + approvalType + "</b>: " + isApproved + "<br/>");
                if (isApproved == "Aprovado")
                {
                    if (prazoDif.Alocadora != null) strBuilder.Append("<b>Entidade</b>: " + prazoDif.Alocadora + "<br/>");
                    if (prazoDif.PrazoDiferenciado != null && prazoDif.Frequency != null) strBuilder.Append("<b>Duração</b>: " + prazoDif.PrazoDiferenciado + " " + frequencyEmailString + "<br/>");
                    strBuilder.Append("<br/>");
                    if (prazoDif.ValorFinanciamento != null) strBuilder.Append("<b>Valor a Financiar</b>: " + prazoDif.ValorFinanciamento + " €<br/>");
                    if (prazoDif.ValorRenda != null) strBuilder.Append("<b>Renda Aprovada</b>: " + prazoDif.ValorRenda + " €<br/>");
                    if (prazoDif.ValorFactor != null) strBuilder.Append("<b>Factor Aprovada</b>: " + prazoDif.ValorFactor + " €<br/>");
                    strBuilder.Append("<br/>");
                }
                strBuilder.Append("<b>Observações</b>: " + p.comments + "<br/>");
                strBuilder.Append("<br/>");
                if (prazoDif.CreatedTime != null) strBuilder.Append("<b>Data de Pedido</b>: " + prazoDif.CreatedTime.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                if (prazoDif.ModifiedTime != null) strBuilder.Append("<b>Data de Resposta</b>: " + prazoDif.ModifiedTime.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                strBuilder.Append("<br/>");
                strBuilder.Append("Para aceder à aplicação, utilize o seguinte link: " + "https://bb.konicaminolta.pt" + "<br/>");
                strBuilder.Append("Por favor, não responder a este email." + "<br/>");
                strBuilder.Append("<br/>");
                strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                message.Body = strBuilder.ToString();

                await emailSend.SendEmailaync(message);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DocumentoSave")]
        public IHttpActionResult DocumentoSave(DocumentProposal p)
        {
            try
            {
                LD_DocumentProposal documentoSave = new LD_DocumentProposal();
                documentoSave.ClassificationID = p.Classification.ID;
                documentoSave.CreatedBy = p.CreatedBy;
                documentoSave.CreatedTime = p.CreatedTime;
                documentoSave.QuoteNumber = p.QuoteNumber;
                documentoSave.SystemID = p.SystemID;
                documentoSave.FileFullPath = p.FileFullPath;
                documentoSave.DocumentIsValid = p.DocumentIsValid;
                documentoSave.DocumentIsProcess = p.DocumentIsProcess;
                documentoSave.FileName = p.FileName;
                documentoSave.ContratoID = p.ContratoID;
                documentoSave.Comments = p.Comments;
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    db.LD_DocumentProposal.Add(documentoSave);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GravarObservacoes")]
        public IHttpActionResult GravarObservacoes(GravarObser p)
        {
            try
            {

                LD_Contrato c = new LD_Contrato();

                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    c = db.LD_Contrato.Where(x => x.ID == p.id).FirstOrDefault();
                    c.ModifiedBy = p.modifiedby;
                    c.ModifiedTime = DateTime.Now.AddHours(-1);
                    c.Comments = p.Observacoes;
                    c.MotivoID = p.motivoID;

                    db.Entry(c).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("Gravarpasta")]
        public IHttpActionResult Gravarpasta(GravarPastaModel p)
        {
            try
            {
                LD_Contrato c = new LD_Contrato();

                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    c = db.LD_Contrato.Where(x => x.ID == p.id).FirstOrDefault();

                    c.ModifiedTime = DateTime.Now.AddHours(-1);
                    c.Pasta = p.Pasta;

                    db.Entry(c).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("FecharProcesso")]
        public async System.Threading.Tasks.Task<IHttpActionResult> FecharProcessoAsync(FecharProcessoModel a)
        {
            LD_Contrato l = new LD_Contrato();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    l = db.LD_Contrato.Where(x => x.ID == a.id).FirstOrDefault();
                    l.IsFacturacao = true;
                    l.ModifiedTime = DateTime.Now;
                    l.ModifiedBy = a.ModifiedBy;
                    l.StatusID = 6;
                    db.Entry(l).State = EntityState.Modified;
                    db.SaveChanges();

                    LD_Contrato_Facturacao f = new LD_Contrato_Facturacao();

                    f = db.LD_Contrato_Facturacao.Where(x => x.LDID == a.id).FirstOrDefault();

                    if (f == null)
                    {
                        f = new LD_Contrato_Facturacao();
                        f.LDID = a.id;
                        f.ModifiedBy = a.ModifiedBy;
                        f.ModifiedTime = DateTime.Now;
                        db.LD_Contrato_Facturacao.Add(f);
                        db.SaveChanges();
                    }


                }

                EmailService emailSend = new EmailService();
                EmailMesage message = new EmailMesage();

                message.Destination = "Facturacao@konicaminolta.pt";
                message.Subject = "BB Serviço - Transferencia Processo de Venda - Quote: " + l.QuoteNumber;
                //message.Subject = "Business Team - ";
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("<p>Caro(a) Gestor(a),</p>");
                strBuilder.Append("<p>Foi transferido o processo de venda para o departamento de Facturação.</p>");
                strBuilder.Append("<b>Por favor, insira NUS no processo</b>");
                strBuilder.Append("<br/>");
                strBuilder.Append("<p>Ao seu dispor,</p>");
                strBuilder.Append("<p>Konica Minolta</p>");
                message.Body = strBuilder.ToString();

                await emailSend.SendEmailaync(message);

                ComissionController comission = new ComissionController();
                if(l.ProposalID != null)
                {
                    comission.CreateCommission((int)l.ProposalID);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DevolverProcesso")]
        public async System.Threading.Tasks.Task<IHttpActionResult> DevolverProcessoAsync(DevolverProcessoModel a)
        {
            string nomeCliente = "";
            LD_Contrato l = new LD_Contrato();
            try
            {
                int? idProposal = 0;
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    l = db.LD_Contrato.Where(x => x.ID == a.id).FirstOrDefault();
                    l.StatusID = 4;
                    l.Retorno = true;
                    l.ModifiedBy = a.ModifiedBy;
                    l.ModifiedTime = DateTime.Now.AddHours(-1);
                    l.ComentariosDevolucao = a.Comentarios;
                    l.DevolucaoMotivoID = a.DevolucacoMotivoID;
                    db.Entry(l).State = EntityState.Modified;
                    db.SaveChanges();
                    idProposal = l.ProposalID;


                }

                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == idProposal).FirstOrDefault();

                    BB_Clientes c = db.BB_Clientes.Where(x => x.accountnumber == proposal.ClientAccountNumber).FirstOrDefault();
                    nomeCliente = c != null ? c.Name : "";

                    EmailService emailSend = new EmailService();
                    EmailMesage message = new EmailMesage();

                    message.Destination = proposal.CreatedBy;
                    message.Subject = "LeaseDesk - Quote: " + proposal.CRM_QUOTE_ID + " Cliente: " + nomeCliente;
                    //message.Subject = "Business Team - ";
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.Append("<p>Caro(a) Gestor(a),</p>");
                    strBuilder.Append("<p>o seu processo foi devolvido pela gestor(a) " + l.ModifiedBy + " </>");
                    strBuilder.Append("<p>Veja os seguintes comentários e prossiga as alteraçoes:</p>");
                    strBuilder.Append("<p>" + a.Comentarios + "</p>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<p>Após as alterações concluídas, submeta o processo!</p>");
                    strBuilder.Append("<p>Ao seu dispor,</p>");
                    strBuilder.Append("<p>LeaseDesk</p>");
                    message.Body = strBuilder.ToString();

                    await emailSend.SendEmailaync(message);


                    //Devolve para rascunho
                    proposal.StatusID = 1;
                    db.Entry(proposal).State = EntityState.Modified;
                    db.SaveChanges();
                }



            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPrazosDiferenciadosNotComplete")]
        public IHttpActionResult GetPrazosDiferenciadosNotComplete(bool? isComplete)
        {
            IList<PrazoDiferenciado> lstValorFinanciado = new List<PrazoDiferenciado>();

            List<BB_Proposal_PrazoDiferenciado> lstBB_Proposal_PrazoDiferenciado = new List<BB_Proposal_PrazoDiferenciado>();

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    //lstBB_Proposal_PrazoDiferenciado = db.BB_Proposal_PrazoDiferenciado.Where(x => x.IsComplete == isComplete.Value).ToList();
                    lstBB_Proposal_PrazoDiferenciado = db.BB_Proposal_PrazoDiferenciado.ToList();


                    foreach (var item in lstBB_Proposal_PrazoDiferenciado)
                    {
                        BB_Proposal_PrazoDiferenciado p = new BB_Proposal_PrazoDiferenciado();
                        p = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == item.ProposalID).FirstOrDefault();
                        BB_Clientes c = new BB_Clientes();
                        BB_Proposal pro = new BB_Proposal();
                        pro = db.BB_Proposal.Where(x => x.ID == item.ProposalID).FirstOrDefault();
                        if (pro != null)
                        {
                            try
                            {
                                string ProdutoFinanceiro = "";


                                BB_Proposal_Overvaluation o = new BB_Proposal_Overvaluation();


                                c = db.BB_Clientes.Where(x => x.accountnumber == pro.ClientAccountNumber).FirstOrDefault();
                                ProdutoFinanceiro = db.BB_FinancingType.Where(x => x.Code == p.FinancingID).Select(x => x.Type).FirstOrDefault();
                                o = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == item.ProposalID).FirstOrDefault();
                                string displayName = "";
                                string displayNameMod = "";

                                using (var u = new masterEntities())
                                {
                                    displayName = u.AspNetUsers.Where(x => x.Email == item.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                                    displayNameMod = u.AspNetUsers.Where(x => x.Email == item.ModifiedBy).Select(x => x.DisplayName).FirstOrDefault();
                                }

                                if (c == null)
                                {
                                    c = new BB_Clientes();
                                }
                                PrazoDiferenciado pd = new PrazoDiferenciado();
                                pd.Cliente = c.Name;
                                pd.NIF = c.NIF;
                                pd.ProposalID = item.ProposalID.Value;
                                pd.ProdutoFinanceiro = ProdutoFinanceiro;
                                pd.nrMeses = p.PrazoDiferenciado;
                                pd.Frequency = p.Frequency;
                                pd.CreatedBy = displayName;//item.CreatedBy;
                                pd.CreatedTime = item.CreatedTime.Value;
                                //pd.ValorProposta = valortotalQUote + LeiDaCopiaPriada + valorProposal_Overvaluation;
                                pd.ValorProposta = p.ValorFinanciamento;
                                pd.ValorRenda = p.ValorRenda != null ? p.ValorRenda.Value : 0;
                                pd.ID = p.ID;
                                pd.Type1 = item.Type;
                                pd.quotenumber = pro.CRM_QUOTE_ID;
                                pd.AccountNumber = c.accountnumber;
                                pd.GestorContaObservacoes = item.GestorContaObservacoes;
                                pd.ModifiedBy = displayNameMod;
                                pd.ModifiedTime = item.ModifiedTime;
                                pd.IsComplete = p.IsComplete;
                                pd.isApproved = p.IsAproved;
                                pd.NLocadora = p.NLocadora;
                                pd.DSO = p.DSO;
                                lstValorFinanciado.Add(pd);
                            }
                            catch (Exception ex)
                            {
                                return NotFound();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return Ok(lstValorFinanciado);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPrazosDiferenciadosNotCompleteNewVersion")]
        public IHttpActionResult GetPrazosDiferenciadosNotCompleteNewVersion(bool? isComplete)
        {
            IList<PrazoDiferenciado> lstValorFinanciado = new List<PrazoDiferenciado>();

            List<BB_Proposal_PrazoDiferenciado> lstBB_Proposal_PrazoDiferenciado = new List<BB_Proposal_PrazoDiferenciado>();
            string bdConnect = @AppSettingsGet.BasedadosConnect;
            try
            {
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_AprovacoesFinaceiras_ALL", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        try
                        {
                            PrazoDiferenciado pd = new PrazoDiferenciado();
                            pd.Cliente = rdr["Cliente"].ToString();
                            pd.NIF = rdr["NIF"].ToString();
                            pd.ProposalID = (int)rdr["ProposalID"];
                            pd.ProdutoFinanceiro = rdr["ProdutoFinanceiro"].ToString();
                            pd.nrMeses = rdr["nrMeses"] != null ? int.Parse(rdr["nrMeses"].ToString()) : 0;
                            pd.CreatedBy = rdr["CreatedBy"].ToString();
                            pd.CreatedTime = rdr["CreatedTime"] != null ? DateTime.Parse(rdr["CreatedTime"].ToString()) : new DateTime();
                            //pd.ValorProposta = valortotalQUote + LeiDaCopiaPriada + valorProposal_Overvaluation;
                            pd.ValorProposta = rdr["ValorProposta"] != null ? Double.Parse(rdr["ValorProposta"].ToString()) : 0;
                            pd.ValorRenda = rdr["ValorRenda"] != null && rdr["ValorRenda"].ToString() != "" ? Double.Parse(rdr["ValorRenda"].ToString()) : 0;
                            pd.ID = (int)rdr["ID"];
                            pd.Type1 = rdr["Type1"].ToString();
                            pd.quotenumber = rdr["quotenumber"].ToString();
                            pd.AccountNumber = rdr["AccountNumber"].ToString();
                            pd.GestorContaObservacoes = rdr["GestorContaObservacoes"].ToString();
                            pd.ModifiedBy = rdr["ModifiedBy"].ToString();
                            pd.ModifiedTime = rdr["ModifiedTime"] != null && rdr["ModifiedTime"].ToString() != "" ? DateTime.Parse(rdr["ModifiedTime"].ToString()) : new DateTime();
                            pd.IsComplete = rdr["IsComplete"] != null ? bool.Parse(rdr["IsComplete"].ToString()) : false;
                            pd.isApproved = rdr["isApproved"] != null && rdr["isApproved"].ToString() != "" ? bool.Parse(rdr["isApproved"].ToString()) : false;
                            pd.NLocadora = rdr["NLocadora"].ToString();
                            pd.DSO = rdr["DSO"].ToString();
                            pd.DataExpiracao = rdr["DataExpiracao"] != null && rdr["DataExpiracao"].ToString() != "" ? DateTime.Parse(rdr["DataExpiracao"].ToString()) : new DateTime();
                            lstValorFinanciado.Add(pd);
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

            return Ok(lstValorFinanciado);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPrazosDiferenciadosByID")]
        public IHttpActionResult GetPrazosDiferenciadosByID(int? id)
        {
            List<BB_Proposal_PrazoDiferenciado_History> Lsthistory = new List<BB_Proposal_PrazoDiferenciado_History>();

            BB_Proposal_PrazoDiferenciado p = new BB_Proposal_PrazoDiferenciado();
            BB_Clientes c = new BB_Clientes();
            BB_Proposal pro = new BB_Proposal();
            string ProdutoFinanceiro = "";
            PrazoDiferenciado pd = new PrazoDiferenciado();
            List<BB_Proposal_Overvaluation> sobrevaorizacoes = new List<BB_Proposal_Overvaluation>();
            BB_Proposal_Overvaluation o = new BB_Proposal_Overvaluation();
            BB_Proposal_Vva vva = new BB_Proposal_Vva();
            BB_Proposal_Financing f = new BB_Proposal_Financing();

            double? sobrevalorizacao = 0;
            double? valortotalQUote = 0;
            double? LeiDaCopiaPriada = 0;

            ProposalBLL p1 = new ProposalBLL();
            LoadProposalInfo i = new LoadProposalInfo();
            ActionResponse a = new ActionResponse();
            int nrMaquinas = 0;

            double txMensalservicosRecorrentes = 0;
            double txMensalOpsManage = 0;

            try
            {


                using (var db = new BB_DB_DEVEntities2())
                {
                    p = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ID == id).First();
                    pro = db.BB_Proposal.Where(x => x.ID == p.ProposalID).First();
                    c = db.BB_Clientes.Where(x => x.accountnumber == pro.ClientAccountNumber).First();
                    vva = db.BB_Proposal_Vva.Where(x => x.ProposalID == p.ProposalID).FirstOrDefault();
                    f = db.BB_Proposal_Financing.Where(x => x.ProposalID == p.ProposalID).FirstOrDefault();
                    ProdutoFinanceiro = db.BB_FinancingType.Where(x => x.Code == p.FinancingID).Select(x => x.Type).FirstOrDefault();

                    Lsthistory = db.BB_Proposal_PrazoDiferenciado_History.Where(x => x.ProposalID == pro.ID).ToList();

                    txMensalservicosRecorrentes = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == pro.ID).Sum(x => x.UnitDiscountPrice).GetValueOrDefault();
                    txMensalOpsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == pro.ID).Sum(x => x.UnitDiscountPrice * x.Quantity).GetValueOrDefault();

                    i.ProposalId = p.ProposalID.Value;
                    a = p1.LoadProposal(i);

                    foreach (var ia in Lsthistory)
                    {
                        if (String.IsNullOrEmpty(ia.ModifiedBy))
                        {
                            ia.IsAproved = null;
                            ia.Commets = "";

                        }
                    }

                    o = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == pro.ID).FirstOrDefault();
                    sobrevaorizacoes = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == pro.ID).ToList();
                    foreach (var ie in sobrevaorizacoes)
                    {
                        sobrevalorizacao += ie.Total;
                    }
                    //List<BB_Proposal_Aprovacao_Quote> lstQuotes = db.BB_Proposal_Aprovacao_Quote.Where(x => x.Proposal_ID == pro.ID && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                    List<BB_Proposal_Quote> lstQuotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == pro.ID && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                    List<BB_Proposal_Aprovacao_Quote> lstQuotes1 = db.BB_Proposal_Aprovacao_Quote.Where(x => x.Proposal_ID == pro.ID).ToList();

                    foreach (var quote in lstQuotes)
                    {
                        double? TCP = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).Select(x => x.TCP).FirstOrDefault();

                        BB_Equipamentos equi = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).FirstOrDefault();
                        if (equi != null)
                            nrMaquinas++;
                        //var contador = db.BB_Proposal_Counters.Where(x => x.OSID == quote.ID).Count();

                        if (TCP is null)
                            TCP = 0;

                        if (quote.IsUsed == false)
                        {
                            LeiDaCopiaPriada = (TCP * quote.Qty) + LeiDaCopiaPriada;

                        }

                    }

                    //txMensalOpsManage = txMensalOpsManage * p.Frequency.GetValueOrDefault();



                    pd.Flexpage = db.BB_Proposal_Financing.Where(x => x.ProposalID == p.ProposalID).Select(x => x.IncludeServices).FirstOrDefault();




                    foreach (var quote in lstQuotes1)
                    {
                        valortotalQUote += quote.TotalNetsale;
                    }

                    //double? sobrevalorizacao = 0;
                    double? OPSHWvalorTotal = 0;
                    double? OPSHWUnti = 0;
                    if (sobrevaorizacoes != null && sobrevaorizacoes.Count > 0 && lstQuotes1 != null && lstQuotes1.Where(x => x.Family == "OPSHW").Count() > 0)
                    {
                        OPSHWvalorTotal = lstQuotes1.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(d => d.TotalNetsale)).First();
                        OPSHWUnti = lstQuotes1.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(d => d.UnitDiscountPrice)).First();
                    }

                    if (sobrevalorizacao != null && sobrevalorizacao != 0 && lstQuotes1 != null && lstQuotes1.Where(x => x.Family == "OPSHW").Count() > 0)
                    {
                        foreach (var quote in lstQuotes1)
                        {
                            quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                            quote.UnitDiscountPrice = Math.Round((quote.TotalNetsale.Value / quote.Qty.Value), 2);
                            //quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                        }

                    }
                    pd.configuracaoNegocio = lstQuotes1;
                }

                if (c == null)
                    c = new BB_Clientes();


                pd.vva = new ValoresTotaisRenda();
                ApprovedPrintingService activePS = null;
                if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                    if (activePS != null)
                    {
                        pd.vva.VVA = activePS.GlobalClickVVA != null ? Math.Round(activePS.GlobalClickVVA.PVP, 2) * p.Frequency.GetValueOrDefault() : 0;
                        pd.vva.VolumeBW = activePS.BWVolume * p.Frequency.GetValueOrDefault();
                        pd.vva.VolumeC = activePS.CVolume * p.Frequency.GetValueOrDefault();

                    }

                }

                //double? valorto = valortotalQUote + LeiDaCopiaPriada + sobrevalorizacao;

                //if (f != null && f.IncludeServices.GetValueOrDefault() == true && p.ValorRenda > 0)
                //{
                //    p.ValorRenda = p.ValorRenda - vva.MonthlyFee.Value;
                //}

                pd.Cliente = c.Name;
                pd.NIF = c.NIF;
                pd.ProposalID = p.ProposalID.Value;
                pd.ValorProposta = Math.Round(p.ValorFinanciamento.Value, 2);
                pd.nrMeses = p.PrazoDiferenciado.GetValueOrDefault();
                pd.ProdutoFinanceiro = ProdutoFinanceiro;
                pd.Type1 = p.Type;
                pd.CreatedBy = p.CreatedBy;
                pd.CreatedTime = p.CreatedTime.Value;
                pd.ModifiedBy = p.ModifiedBy;
                pd.ModifiedTime = p.ModifiedTime.GetValueOrDefault();
                pd.AccountNumber = c.accountnumber;
                pd.ValorRenda = Math.Round(p.ValorRenda.GetValueOrDefault(), 2);
                pd.GestorContaObservacoes = p.GestorContaObservacoes;
                pd.comments = p.Commets;
                pd.Alocadora = p.Alocadora;
                pd.IsComplete = p.IsComplete;
                pd.isApproved = p.IsAproved;
                pd.history = new List<BB_Proposal_PrazoDiferenciado_History>();
                pd.isSobrevarizacao = sobrevaorizacoes != null && sobrevaorizacoes.Count > 0 && sobrevalorizacao > 0 ? true : false;
                pd.totalSobrevalorizacao = sobrevalorizacao;
                pd.NLocadora = p.NLocadora;
                pd.sobrevalorizacao = sobrevaorizacoes;
                pd.DSO = p.DSO;
                pd.quotenumber = pro.CRM_QUOTE_ID;
                pd.leicopiaprivada = LeiDaCopiaPriada;
                pd.nrMaquinas = nrMaquinas;
                pd.Frequency = p.Frequency;
                pd.TaxaMensalOPSManage = txMensalOpsManage;
                pd.TaxaMensalServicosRecorrentes = txMensalservicosRecorrentes;
                pd.ValorBnpAprovacao = p.BNP_ApprovedValue;
                pd.DataExpiracao = p.DataExpiracao;
                //BB_Proposal_PrazoDiferenciado_History nnn = new BB_Proposal_PrazoDiferenciado_History();

                //nnn.ProposalID = p.ProposalID.Value;
                //nnn.ValorFinanciamento = pro.ValueTotal + (o != null && o.Total != null ? o.Total : 0) + LeiDaCopiaPriada;
                //nnn.CreatedBy = p.CreatedBy;
                //nnn.CreatedTime = p.CreatedTime.Value;
                //nnn.ModifiedBy = p.ModifiedBy;
                //nnn.ModifiedTime = p.ModifiedTime.GetValueOrDefault();
                //nnn.ValorRenda = p.ValorRenda.GetValueOrDefault();
                //nnn.GestorContaObservacoes = p.GestorContaObservacoes;
                //nnn.Commets = p.Commets;
                //Lsthistory.Add(nnn);
                pd.history = Lsthistory;
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(pd);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPrazosDiferenciadosByID1")]
        public IHttpActionResult GetPrazosDiferenciadosByID1(int? id)
        {

            BB_Proposal_PrazoDiferenciado p = new BB_Proposal_PrazoDiferenciado();
            BB_Clientes c = new BB_Clientes();
            BB_Proposal pro = new BB_Proposal();
            string ProdutoFinanceiro = "";
            PrazoDiferenciado pd = new PrazoDiferenciado();
            BB_Proposal_Overvaluation o = new BB_Proposal_Overvaluation();

            double? LeiDaCopiaPriada = 0;
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    p = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == id).FirstOrDefault();
                }

                if (p != null)
                    pd.Alocadora = p != null ? p.Alocadora : "";

            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(pd);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetContractoByID")]
        public IHttpActionResult GetContractoByID(int? contractoID)
        {

            LD_Contrato contrcto = new LD_Contrato();
            LD_TipoContrato t = new LD_TipoContrato();
            LD_DocSign_Control ld_DocSign_Control = new LD_DocSign_Control();
            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                contrcto = db.LD_Contrato.Where(x => x.ID == contractoID).FirstOrDefault();
                t = db.LD_TipoContrato.Where(x => x.ID == contrcto.TipoContratoID).FirstOrDefault();

                ld_DocSign_Control = db.LD_DocSign_Control.Where(x => x.ProposalID == contrcto.ProposalID).FirstOrDefault();

            }



            var config1 = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LD_Contrato, LD_Contrato_Model>();
            });

            IMapper iMapper1 = config1.CreateMapper();

            LD_Contrato_Model c = iMapper1.Map<LD_Contrato, LD_Contrato_Model>(contrcto);


            //Controlo UI botoes Docsign
            if (ld_DocSign_Control == null)
            {
                c.IsButtonImitirDocSign = false;
                c.IsButtonSuspender = false;
                c.IsButtonDelete = false;
            }
            if (ld_DocSign_Control != null && ld_DocSign_Control.DocSignStatusSent == 0)
            {
                c.IsButtonImitirDocSign = true;
                c.IsButtonSuspender = false;
                c.IsButtonDelete = false;
            }
            if (ld_DocSign_Control != null && ld_DocSign_Control.DocSignStatusSent == 1)
            {
                c.IsButtonImitirDocSign = false;
                c.IsButtonSuspender = true;
                c.IsButtonDelete = true;
            }

            c.TipoContrato = t != null ? t.TipoContrato : "";

            c.FolderDOC = contrcto.Pasta;



            return Ok(c);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_Proposal_Contract_Signin")]
        public IHttpActionResult Get_BB_Proposal_Contract_Signin(int? contractoID)
        {

            LD_Contrato contrcto = new LD_Contrato();
            List<BB_Proposal_Contacts_Signing> a = new List<BB_Proposal_Contacts_Signing>();
            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                contrcto = db.LD_Contrato.Where(x => x.ID == contractoID).FirstOrDefault();

            }

            using (var db = new BB_DB_DEVEntities2())
            {
                a = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == contrcto.ProposalID).ToList();

            }

            //var config1 = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<LD_Contrato, LD_Contrato_Model>();
            //});

            //IMapper iMapper1 = config1.CreateMapper();

            //LD_Contrato_Model c = iMapper1.Map<LD_Contrato, LD_Contrato_Model>(contrcto);

            //c.TipoContrato = t != null ? t.TipoContrato : "";




            return Ok(a);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_LD_Observacoes_Motivo")]
        public IHttpActionResult Get_LD_Observacoes_Motivo()
        {


            List<LD_Observacoes_Motivos> lsta = new List<LD_Observacoes_Motivos>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    lsta = db.LD_Observacoes_Motivos.ToList();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(lsta);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_LD_Devolucao_Motivo")]
        public IHttpActionResult Get_LD_Devolucao_Motivo()
        {


            List<LD_Devolucao_Motivo> lsta = new List<LD_Devolucao_Motivo>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    lsta = db.LD_Devolucao_Motivo.ToList();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(lsta);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("LeaseDeskUploadFiles")]
        public HttpResponseMessage LeaseDeskUploadFiles()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            try
            {

                string root = @AppSettingsGet.LeaseDesk_UploadFile_DIR;







                if (httpRequest.Files.Count > 0)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("httpRequest.Files.Count", EventLogEntryType.Information, 101, 1);
                    }
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {
                        string path = root + file + "\\Documentacao\\";
                        if (!Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);

                        var postedFile = httpRequest.Files[file];
                        var filePath = path + postedFile.FileName;
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);

                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.Message.ToString(), EventLogEntryType.Information, 101, 1);
                    eventLog.WriteEntry(ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                }
            }
            return result;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ContratoUploadFile")]
        public HttpResponseMessage ContratoUploadFile(int? id, string u, int? pageNum, int orderNum, bool requestAss)
        {

            int? contractoID = id;
            string modifedby = u;
            string docPath = "";
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            LD_Contrato routeContrato = new LD_Contrato();

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                routeContrato = db.LD_Contrato.Where(x => x.ID == contractoID).FirstOrDefault();
            }

            string root = @AppSettingsGet.LeaseDesk_UploadFile_Contrato_DocuSign + routeContrato.QuoteNumber + "\\";

            LD_Contrato p = new LD_Contrato();

            try
            {
                //validar se o numero de ordem existe ou nao
                //exemplo: se existir mandar alerta 

                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    bool existsOrder = false;

                    List<LD_DocSign_Control_Files> dscf = db.LD_DocSign_Control_Files.Where(x => x.ProposalID == routeContrato.ProposalID).ToList();

                    foreach (var item in dscf)
                    {
                        if (item.OrderFile == orderNum)
                        {
                            existsOrder = true;
                            break;
                        }
                    }

                    if (existsOrder)
                    {
                        string errorMessage = "OrderNum já existe !";
                        HttpResponseMessage req = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        req.Content = new StringContent(errorMessage, Encoding.UTF8, "application/json");

                        return req;

                    };
                }


                if (!Directory.Exists(root))
                    System.IO.Directory.CreateDirectory(root);

                if (httpRequest.Files.Count > 0)
                {
                    var docfiles = new List<string>();
                    foreach (string file in httpRequest.Files)
                    {

                        var postedFile = httpRequest.Files[file];
                        var filePath = root + postedFile.FileName;
                        docPath = filePath;
                        postedFile.SaveAs(filePath);
                        docfiles.Add(filePath);
                    }
                    result = Request.CreateResponse(HttpStatusCode.Created, docfiles);

                    using (var db = new BB_DB_DEV_LeaseDesk())
                    {
                        p = db.LD_Contrato.Where(x => x.ID == contractoID).FirstOrDefault();
                        p.ModifiedBy = modifedby;
                        p.PathContracto = docPath;
                        p.ModifiedTime = DateTime.Now;
                        p.FilenameContracto = Path.GetFileName(docPath);
                        db.Entry(p);
                        db.SaveChanges();

                        //create or update do docuSign Control

                        LD_DocSign_Control docuSignControl = db.LD_DocSign_Control.Where(x => x.ProposalID == p.ProposalID).FirstOrDefault();

                        if (docuSignControl != null)
                        {
                            docuSignControl.DocSignStatusSent = 0;
                            docuSignControl.ModifiedBy = p.ModifiedBy;
                            docuSignControl.ModifiedTime = DateTime.Now;
                            //docuSignControl.NrPageSign = pageNum;
                            //docuSignControl.DocPath = docPath;
                            docuSignControl.LeasedeskID = contractoID;
                            docuSignControl.ProposalID = p.ProposalID;


                            db.Entry(docuSignControl).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            docuSignControl = new LD_DocSign_Control();
                            docuSignControl.DocSignStatusSent = 0;
                            docuSignControl.CreatedBy = p.ModifiedBy;
                            docuSignControl.NrPageSign = pageNum;
                            docuSignControl.DocPath = docPath;
                            docuSignControl.LeasedeskID = contractoID;
                            docuSignControl.ProposalID = p.ProposalID;
                            docuSignControl.CreatedTime = DateTime.Now;

                            db.LD_DocSign_Control.Add(docuSignControl);
                            db.SaveChanges();
                        }

                        LD_DocSign_Control_Files newFile = new LD_DocSign_Control_Files();
                        newFile.ProposalID = p.ProposalID;
                        newFile.FilePath = docPath;
                        newFile.NrPageSign = pageNum;
                        newFile.OrderFile = orderNum;
                        newFile.CreatedDatetime = DateTime.Now;
                        newFile.IsToSign = requestAss;
                        //if (pageNum == 0 || pageNum == null)
                        //{
                        //    newFile.IsToSign = false;
                        //}
                        //else
                        //{
                        //    newFile.IsToSign = true;
                        //}

                        newFile.IsToSign = requestAss;
                        db.LD_DocSign_Control_Files.Add(newFile);
                        db.SaveChanges();
                    }
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocuSigns")]
        public List<LD_DocSign_Control_Files> GetDocuSigns(int? contractoID)
        {
            LD_Contrato p = new LD_Contrato();

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                p = db.LD_Contrato.Where(x => x.ID == contractoID).FirstOrDefault();
            }

            List<LD_DocSign_Control_Files> lstDocs = new List<LD_DocSign_Control_Files>();
            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                lstDocs = db.LD_DocSign_Control_Files.Where(x => x.ProposalID == p.ProposalID).OrderBy(x => x.OrderFile).ToList();
            }

            return lstDocs;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocuSignStatus")]
        public string GetDocuSignStatus(int proposalId)
        {
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    LD_DocSign_Control ldc = db.LD_DocSign_Control.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    if (ldc != null)
                    {
                        int? docStatus = ldc.DocSignStatusSent;

                        if (docStatus == 0)
                        {
                            return "Ready";
                        }
                        else if (docStatus == 1)
                        {
                            return "In Progress";
                        }
                        else if (docStatus == 2)
                        {
                            return "Completed";
                        }

                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("DeleteDocuSignFile")]
        public IHttpActionResult DeleteDocuSignFile(int docToDelID)
        {
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    LD_DocSign_Control_Files ldc = db.LD_DocSign_Control_Files.Where(x => x.ID == docToDelID).FirstOrDefault();

                    int proposalID = ldc.ProposalID.GetValueOrDefault(); ;

                    db.LD_DocSign_Control_Files.Remove(ldc);
                    db.SaveChanges();


                    //Remover o docsigncontrolo caso o nao exista mais ficheiros
                    List<LD_DocSign_Control_Files> lstLD_DocSign_Control_Files = new List<LD_DocSign_Control_Files>();

                    lstLD_DocSign_Control_Files = db.LD_DocSign_Control_Files.Where(x => x.ProposalID == proposalID).ToList();
                    if (lstLD_DocSign_Control_Files != null && lstLD_DocSign_Control_Files.Count == 0)
                    {
                        LD_DocSign_Control l = new LD_DocSign_Control();

                        l = db.LD_DocSign_Control.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                        db.LD_DocSign_Control.Remove(l);
                        db.SaveChanges();
                    }

                    return Ok();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocumentsProposal")]
        public IHttpActionResult GetDocumentsProposal(int? ContractoID)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            string root = @"D:\LeaseDesk\Documentation\";
            List<DocumentProposal> list = new List<DocumentProposal>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    //COUNTERS

                    List<LD_DocumentProposal> lstDocuments = db.LD_DocumentProposal.Where(x => x.ContratoID == ContractoID).ToList();
                    foreach (var item in lstDocuments)
                    {
                        var configCounter = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<LD_DocumentProposal, DocumentProposal>();
                        });

                        IMapper iMapperCounter = configCounter.CreateMapper();

                        var dc = iMapperCounter.Map<LD_DocumentProposal, DocumentProposal>(item);
                        dc.Classification = db.LD_DocumentClassification.Where(x => x.ID == item.ClassificationID).FirstOrDefault();
                        dc.System = db.LD_System.Where(x => x.ID == item.SystemID).FirstOrDefault();
                        list.Add(dc);
                    }


                }
            }
            catch
            {
                NotFound();
            }

            return Ok(list);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocumentClassfication")]
        public IHttpActionResult GetDocumentClassfication(int? proposalID)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;


            int financingTypeCode = 4;


            List<LD_DocumentClassification> list = new List<LD_DocumentClassification>();
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    //list = (from b in db.BB_Financing_Documentos
                    //        join e in db.LD_DocumentClassification on b.ClassificationID equals e.ID
                    //        where b.FinancingID == financingTypeCode
                    //        select e).ToList();

                    list = db.LD_DocumentClassification.ToList();


                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocumentsType")]
        public IHttpActionResult GetDocumentsType()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;


            int financingTypeCode = 4;


            List<LD_DocumentClassification> list = new List<LD_DocumentClassification>();
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    //list = (from b in db.BB_Financing_Documentos
                    //        join e in db.LD_DocumentClassification on b.ClassificationID equals e.ID
                    //        where b.FinancingID == financingTypeCode
                    //        select e).ToList();

                    list = db.LD_DocumentClassification.ToList();


                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(list);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetLeaseDeskDocumentos")]
        public IHttpActionResult GetLeaseDeskDocumentos(int? proposalID)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;


            int financingTypeCode = 4;


            List<string> list = new List<string>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(list);
        }




        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProcessosVendas")]
        public IHttpActionResult GetProcessosVendas(string year)
        {
            List<LD_ContratoModel> listModel = new List<LD_ContratoModel>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_LeaseDesk_ALL_New", conn);
                    cmd.Parameters.Add("@year1", SqlDbType.NVarChar, 20).Value = year;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {


                        LD_ContratoModel m = new LD_ContratoModel();
                        m.ID = (int)rdr["ID"];
                        m.ProposalID = (int)rdr["ProposalID"];
                        m.StatusName = rdr["Status"].ToString();
                        m.SistemaAssinatura = rdr["Assinatura"].ToString();
                        m.Comments = rdr["Comentarios"].ToString();
                        m.MotivoDescricao = rdr["EstadoProcesso"].ToString();
                        m.DevolucaoMotivoDescricao = rdr["Devolucao"].ToString();
                        m.ComentariosDevolucao = //i.ComentariosDevolucao;
                        m.QuoteNumber = rdr["Quote"].ToString();
                        //LD_Contrato_Facturacao cf = db.LD_Contrato_Facturacao.Where(x => x.LDID == i.ID).FirstOrDefault();
                        //if (cf != null)
                        //{
                        //    m.NUS = cf.NUS;
                        //}
                        m.isClosed = rdr["isClosed"] != null ? Boolean.Parse(rdr["isClosed"].ToString()) : false;

                        m.NCliente = rdr["NCliente"].ToString();
                        m.NomeCliente = rdr["Cliente"].ToString();

                        m.ModifiedBy = rdr["ModificadoPor"].ToString();
                        m.CreatedBy = rdr["CriadoPor"].ToString();

                        m.ModifiedTime = rdr["ModificadoEM"] != null ? DateTime.Parse(rdr["ModificadoEM"].ToString()) : new DateTime();
                        m.CreatedTime = rdr["CriadoEM"] != null ? DateTime.Parse(rdr["CriadoEM"].ToString()) : new DateTime();

                        m.Financiamento = rdr["Locadora"].ToString();

                        m.TipoNegocio = rdr["TipoNegocio"].ToString();

                        listModel.Add(m);



                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProcessosVendas_OLD")]
        public IHttpActionResult GetProcessosVendas_OLD()
        {

            List<LD_ContratoModel> listModel = new List<LD_ContratoModel>();
            List<LD_Contrato> list = new List<LD_Contrato>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    list = db.LD_Contrato.ToList();


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
                                m.NUS = cf.NUS;
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

            return Ok(listModel.OrderByDescending(x => x.ModifiedTime).ToList());
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetHistoricoDocSign")]
        public IHttpActionResult GetHistoricoDocSign([FromBody] string quote)
        {
            List<LD_DocSign_Control_Model_History> listModel = new List<LD_DocSign_Control_Model_History>();

            List<LD_DocSign_Control_History> lstLD_DocSign_Control_History = new List<LD_DocSign_Control_History>();

            try
            {
                using (var db1 = new BB_DB_DEV_LeaseDesk())
                {
                    LD_Contrato cntracto = db1.LD_Contrato.Where(x => x.QuoteNumber == quote).FirstOrDefault();

                    lstLD_DocSign_Control_History = db1.LD_DocSign_Control_History.Where(x => x.ProposalID == cntracto.ProposalID).ToList();


                    foreach (var i in lstLD_DocSign_Control_History)
                    {
                        var config1 = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<LD_DocSign_Control_History, LD_DocSign_Control_Model_History>();
                        });

                        IMapper iMapper1 = config1.CreateMapper();

                        LD_DocSign_Control_Model_History m = iMapper1.Map<LD_DocSign_Control_History, LD_DocSign_Control_Model_History>(i);


                        if (m.DocSignStatusSent == 0)
                            m.DescricaoStatus = "Ready";
                        if (m.DocSignStatusSent == 1)
                            m.DescricaoStatus = "In Progress";
                        if (m.DocSignStatusSent == 2)
                            m.DescricaoStatus = "Sent";

                        listModel.Add(m);
                    }
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return Ok(listModel.OrderByDescending(x => x.ModifiedTime).ToList());
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetHistorico")]
        public IHttpActionResult GetHistorico([FromBody] string quote)
        {
            List<LD_ContratoHistoricoModel> listModel = new List<LD_ContratoHistoricoModel>();
            List<LD_Contrato_History> list = new List<LD_Contrato_History>();

            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    list = db.LD_Contrato_History.Where(x => x.QuoteNumber == quote).ToList();


                    foreach (var i in list)
                    {
                        var config1 = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<LD_Contrato_History, LD_ContratoHistoricoModel>();
                        });

                        IMapper iMapper1 = config1.CreateMapper();

                        LD_ContratoHistoricoModel m = iMapper1.Map<LD_Contrato_History, LD_ContratoHistoricoModel>(i);

                        m.MotivoDescricao = db.LD_Observacoes_Motivos.Where(x => x.ID == i.MotivoID).Select(x => x.Motive).FirstOrDefault();
                        m.DevolucaoDescricao = db.LD_Devolucao_Motivo.Where(x => x.ID == i.MotivoID).Select(x => x.Motivo).FirstOrDefault();

                        BB_Proposal b = db.BB_Proposal.Where(x => x.ID == m.ProposalID).FirstOrDefault();

                        m.StatusName = db.LD_Status.Where(x => x.ID == m.StatusID).Select(x => x.Status).FirstOrDefault();
                        m.SistemaAssinatura = db.LD_Assinatura_System.Where(x => x.ID == m.SystemAssinaturaID).Select(x => x.System).FirstOrDefault();

                        BB_Clientes c = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();

                        if (c == null)
                            c = new BB_Clientes();

                        m.Cliente = c.accountnumber + " - " + c.Name;

                        using (var db1 = new masterEntities())
                        {
                            m.ModifiedBy = db1.AspNetUsers.Where(x => x.Email == i.ModifiedBy).Select(x => x.DisplayName).FirstOrDefault();
                            m.CreatedBy = db1.AspNetUsers.Where(x => x.Email == i.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                        }

                        listModel.Add(m);
                    }
                }



            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return Ok(listModel.OrderByDescending(x => x.ModifiedTime).ToList());
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPA5Historico")]
        public IHttpActionResult GetPA5Historico([FromBody] int contractoID)
        {

            List<LD_Email_Log> list = new List<LD_Email_Log>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    list = db.LD_Email_Log.Where(x => x.ContractID == contractoID).ToList();
                }


            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return Ok(list.OrderByDescending(x => x.ProcessDate).ToList());
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetContractoDetail")]
        public IHttpActionResult GetContractoDetail(int? contractoID)
        {
            LD_Contrato c = new LD_Contrato();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    c = db.LD_Contrato.Where(x => x.ID == contractoID).FirstOrDefault();
                }
            }
            catch
            {
                return NotFound();
            }

            return Ok(c);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("DocumentoDelete")]
        public IHttpActionResult DocumentoDelete(int? ContractoID)
        {
            List<LD_Contrato> list = new List<LD_Contrato>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    var doc = db.LD_DocumentProposal.Where(x => x.ID == ContractoID).FirstOrDefault();
                    string filename = doc.FileFullPath;
                    db.LD_DocumentProposal.Remove(doc);

                    int num = db.SaveChanges();

                    if (num == 1)
                    {
                        if (File.Exists(@filename))
                        {
                            File.Delete(@filename);
                        }
                    }
                }
            }
            catch
            {
                return NotFound();
            }

            return Ok();
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetLD_Sistema")]
        public IHttpActionResult GetLD_Sistema()
        {
            List<LD_Assinatura_System> ld = new List<LD_Assinatura_System>();
            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                try
                {
                    ld = db.LD_Assinatura_System.ToList();
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }

            return Ok(ld);
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

                if (prazoDiferenciado1 != null)
                {
                    a.ProposalObj.Draft.financing.DataExpiracao = prazoDiferenciado1.DataExpiracao;
                }

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
            if (activePS != null && activePS.GlobalClickNoVolume != null)
            {
                switch (activePS.GlobalClickNoVolume.PageBillingFrequency)
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

            LeaseDeskBLL lDBll = new LeaseDeskBLL();

            List<ConditionPVP> condPvp = lDBll.FinancingDetails(proposalID);

            a.ProposalObj.ConditionsPvp = condPvp;




            return Ok(a.ProposalObj);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GravarDocumentLine")]
        public IHttpActionResult GravarDocumentLine(DocumentProposal a)
        {
            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                try
                {
                    LD_DocumentProposal dp = db.LD_DocumentProposal.Where(x => x.ID == a.ID).FirstOrDefault();
                    dp.ClassificationID = a.Classification.ID;
                    dp.ModifiedTime = DateTime.Now;
                    dp.ModifiedBy = a.ModifiedBy;
                    dp.DocumentIsValid = a.DocumentIsValid;
                    dp.DocumentIsProcess = a.DocumentIsProcess;
                    dp.Comments = a.Comments;
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            return Ok(true);
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("PontosDeEnvioResumo")]
        public IHttpActionResult PontosDeEnvioResumo(int? proposalID)
        {
            List<BB_Proposal_DeliveryLocation> lstBB_Proposal_DeliveryLocation = null;
            List<BB_Proposal_DeliveryLocationResumoModel> lstBB_Proposal_DeliveryLocationResumoModel = null;
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    try
                    {
                        lstBB_Proposal_DeliveryLocation = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalID).ToList();
                        lstBB_Proposal_DeliveryLocationResumoModel = new List<BB_Proposal_DeliveryLocationResumoModel>();
                        foreach (var i in lstBB_Proposal_DeliveryLocation)
                        {
                            Dictionary<int?, List<BB_Proposal_ItemDoBasket>> lstBB_Proposal_ItemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == i.IDX && x.Group != null).GroupBy(x => x.Group).ToDictionary(x => x.Key, x => x.ToList());

                            foreach (KeyValuePair<int?, List<BB_Proposal_ItemDoBasket>> p in lstBB_Proposal_ItemDoBasket)
                            {
                                foreach (var it in p.Value)
                                {
                                    BB_Proposal_DeliveryLocationResumoModel resumo = new BB_Proposal_DeliveryLocationResumoModel();
                                    resumo.Group = p.Key;
                                    resumo.Adress1 = i.Adress1;
                                    resumo.Adress2 = i.Adress2;
                                    resumo.PostalCode = i.PostalCode;
                                    resumo.City = i.City;
                                    resumo.Contacto = i.Contacto;
                                    resumo.Phone = i.Phone;
                                    resumo.Email = i.Email;

                                    resumo.CodeRef = it.CodeRef;
                                    resumo.Qty = it.Qty;
                                    resumo.Description = it.Description;
                                    lstBB_Proposal_DeliveryLocationResumoModel.Add(resumo);
                                }
                            }
                        }





                    }
                    catch (Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(lstBB_Proposal_DeliveryLocationResumoModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("PontosDeEnvioDetalhe")]
        public IHttpActionResult PontosDeEnvioDetalhe(int? proposalID)
        {
            List<BB_Proposal_DeliveryLocation> lstBB_Proposal_DeliveryLocation = null;
            List<BB_Proposal_DeliveryLocationResumoModel> lstBB_Proposal_DeliveryLocationResumoModel = null;
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    try
                    {
                        lstBB_Proposal_DeliveryLocation = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalID).ToList();
                        lstBB_Proposal_DeliveryLocationResumoModel = new List<BB_Proposal_DeliveryLocationResumoModel>();
                        foreach (var i in lstBB_Proposal_DeliveryLocation)

                        {
                            int qty = 0;
                            var lstBB_Proposal_ItemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == i.IDX && x.Group != null).ToLookup(x => x.Group);

                            int nrProposta = 1;
                            foreach (IGrouping<int?, BB_Proposal_ItemDoBasket> groupItem in lstBB_Proposal_ItemDoBasket)
                            {

                                // groupItem is <IEnumerable<List<CustomObject>>

                                //for(int i1 =0; i1 < groupItem.Count(); i1++)
                                //{
                                //    groupItem.Key.Value
                                //}

                                foreach (var idd in groupItem)
                                {

                                    qty = idd.Qty.Value;
                                    int contadorinterno = nrProposta;
                                    for (int idx = 0; idx < qty; idx++)
                                    {
                                        BB_Proposal_DeliveryLocationResumoModel resumo = new BB_Proposal_DeliveryLocationResumoModel();
                                        resumo.PropostaID = contadorinterno++;
                                        resumo.Group = groupItem.Key;
                                        resumo.Adress1 = i.Adress1;
                                        resumo.Adress2 = i.Adress2;
                                        resumo.PostalCode = i.PostalCode;
                                        resumo.City = i.City;
                                        resumo.Contacto = i.Contacto;
                                        resumo.Phone = i.Phone;
                                        resumo.Email = i.Email;
                                        resumo.ProposalID = proposalID;
                                        resumo.CodeRef = idd.CodeRef;
                                        resumo.Qty = 1;
                                        resumo.Description = idd.Description;
                                        lstBB_Proposal_DeliveryLocationResumoModel.Add(resumo);
                                    }

                                }
                                nrProposta += qty;

                            }
                            //foreach (KeyValuePair<int?, List<BB_Proposal_ItemDoBasket>> p in lstBB_Proposal_ItemDoBasket)
                            //{
                            //    int count = 0;
                            //    foreach(var o in p.Value)
                            //    {

                            //        for(int u = 0; u< o.Qty; u++)
                            //        {
                            //            BB_Proposal_DeliveryLocationResumoModel resumo = new BB_Proposal_DeliveryLocationResumoModel();
                            //            resumo.PropostaID = count++;
                            //            resumo.Group = p.Key;
                            //            resumo.Adress1 = i.Adress1;
                            //            resumo.Adress2 = i.Adress2;
                            //            resumo.PostalCode = i.PostalCode;
                            //            resumo.City = i.City;
                            //            resumo.Contacto = i.Contacto;
                            //            resumo.Phone = i.Phone;
                            //            resumo.Email = i.Email;

                            //            resumo.CodeRef = o.CodeRef;
                            //            resumo.Qty = 1;
                            //            resumo.Description = o.Description;
                            //            lstBB_Proposal_DeliveryLocationResumoModel.Add(resumo);
                            //        }


                            //    }


                            //Dictionary<BB_Proposal_ItemDoBasket, int?> lstBB_Proposal_ItemDoBasketCount = p.Value.GroupBy(x => x.CodeRef).ToDictionary(x => x.FirstOrDefault(), x => x.Select(y => y.Qty).FirstOrDefault());

                            //int count = 1;

                            //foreach (var it in p.Key)
                            //{
                            //    BB_Proposal_DeliveryLocationResumoModel resumo = new BB_Proposal_DeliveryLocationResumoModel();
                            //    resumo.PropostaID = count;
                            //    resumo.Group = p.Key;
                            //    resumo.Adress1 = i.Adress1;
                            //    resumo.Adress2 = i.Adress2;
                            //    resumo.PostalCode = i.PostalCode;
                            //    resumo.City = i.City;
                            //    resumo.Contacto = i.Contacto;
                            //    resumo.Phone = i.Phone;
                            //    resumo.Email = i.Email;

                            //    resumo.CodeRef = it.Key.CodeRef;
                            //    resumo.Qty = 1;
                            //    resumo.Description = it.Key.Description;
                            //    lstBB_Proposal_DeliveryLocationResumoModel.Add(resumo);
                            //    count++;
                            //}

                            //foreach (var it in p.Value)
                            //{
                            //    var ismaquina = db.BB_Equipamentos.Where(x => x.CodeRef == it.CodeRef).FirstOrDefault();

                            //    BB_Proposal_DeliveryLocationResumoModel resumo = new BB_Proposal_DeliveryLocationResumoModel();
                            //    resumo.Group = p.Key;
                            //    resumo.Adress1 = i.Adress1;
                            //    resumo.Adress2 = i.Adress2;
                            //    resumo.PostalCode = i.PostalCode;
                            //    resumo.City = i.City;
                            //    resumo.Contacto = i.Contacto;
                            //    resumo.Phone = i.Phone;
                            //    resumo.Email = i.Email;

                            //    resumo.CodeRef = it.CodeRef;
                            //    resumo.Qty = it.Qty;
                            //    resumo.Description = it.Description;
                            //    lstBB_Proposal_DeliveryLocationResumoModel.Add(resumo);
                            //}
                            //    }
                            //}





                        }

                        List<BB_Proposal_LeaseDesk_Detalhe> lst = db.BB_Proposal_LeaseDesk_Detalhe.Where(x => x.ProposalID == proposalID).ToList();
                        db.BB_Proposal_LeaseDesk_Detalhe.RemoveRange(lst);
                        db.SaveChanges();
                        foreach (var item in lstBB_Proposal_DeliveryLocationResumoModel)
                        {
                            var config1 = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<BB_Proposal_DeliveryLocationResumoModel, BB_Proposal_LeaseDesk_Detalhe>();
                            });

                            IMapper iMapper1 = config1.CreateMapper();

                            BB_Proposal_LeaseDesk_Detalhe quote = iMapper1.Map<BB_Proposal_DeliveryLocationResumoModel, BB_Proposal_LeaseDesk_Detalhe>(item);

                            db.BB_Proposal_LeaseDesk_Detalhe.Add(quote);
                            db.SaveChanges();
                            item.ID = quote.ID.ToString(); ;

                        }


                    }

                    catch (Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(lstBB_Proposal_DeliveryLocationResumoModel.OrderBy(x => x.PropostaID).ToList());
        }
        [AcceptVerbs("GET", "POST")]
        [ActionName("ValidateDocumentation")]
        public IHttpActionResult ValidateDocumentation(ValidateDocumentarionModel a)
        {
            RespostaLeaseDeskValidateDocumentacao resposta = new RespostaLeaseDeskValidateDocumentacao();
            //int ProposalID = 123;
            var financingTypeCode = 4;
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<LD_DocumentClassification> list = new List<LD_DocumentClassification>(); ;
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    list = (from b in db.BB_Financing_Documentos
                            join e in db.LD_DocumentClassification on b.ClassificationID equals e.ID
                            where b.FinancingID == financingTypeCode
                            select e).ToList();

                    dic = list.ToDictionary(x => x.Classification, x => 0);

                }



                foreach (var entry in dic.Keys.ToList())
                {
                    bool f = a.docContrato.Exists(x => x.Classification.Classification == entry && x.DocumentIsValid == true);

                    if (f)
                        dic[entry] = dic[entry] + 1;

                }

                resposta.documentosEmFalta = new List<string>();
                //Validar o dicionario se encotnrou documento
                foreach (KeyValuePair<string, int> entry in dic)
                {
                    if (entry.Value < 1)
                    {
                        resposta.documentosEmFalta.Add(entry.Key);
                    }
                }

                if (resposta.documentosEmFalta.Count > 0)
                {
                    resposta.ErrMensagem = "Faltam os Seguintes documentos serem validados:";
                    resposta.errCode = 1;
                    return Ok(resposta);
                }


                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    try
                    {
                        LD_Contrato dp = db.LD_Contrato.Where(x => x.ID == a.contractoID).FirstOrDefault();
                        dp.ContratoValidado = true;
                        dp.ContratoGerado = false;
                        dp.ModifiedTime = DateTime.Now;
                        dp.ModifiedBy = "";

                        db.Entry(dp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return NotFound();
                    }
                }



            }
            catch (Exception ex)
            {
                return NotFound();
            }



            return Ok(resposta);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("EmitContractViaDocusign")]
        public void EmitContractViaDocusign(int? ProposalID)
        {
            LD_DocSign_Control docControl = new LD_DocSign_Control();

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                docControl = db.LD_DocSign_Control.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                docControl.DocSignStatusSent = 1;
                docControl.ModifiedTime = DateTime.Now;

                db.Entry(docControl).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("SuspenderContractViaDocusign")]
        public void SuspenderContractViaDocusign(int? ProposalID)
        {
            LD_DocSign_Control docControl = new LD_DocSign_Control();

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                docControl = db.LD_DocSign_Control.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                docControl.DocSignStatusSent = 0;
                docControl.ModifiedTime = DateTime.Now;

                db.Entry(docControl).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("StopContractViaDocusign")]
        public void StopContractViaDocusign(int? ProposalID)
        {
            LD_DocSign_Control docControl = new LD_DocSign_Control();

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                docControl = db.LD_DocSign_Control.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                docControl.DocSignStatusSent = 4;
                docControl.ModifiedTime = DateTime.Now;

                db.Entry(docControl).State = EntityState.Modified;
                db.SaveChanges();
            }
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("ApagarContractViaDocusign")]
        public void ApagarContractViaDocusign(int? ProposalID)
        {
            LD_DocSign_Control docControl = new LD_DocSign_Control();
            List<LD_DocSign_Control_Files> lstLD_DocSign_Control_Files = new List<LD_DocSign_Control_Files>();

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                lstLD_DocSign_Control_Files = db.LD_DocSign_Control_Files.Where(x => x.ProposalID == ProposalID).ToList();

                foreach(var i in lstLD_DocSign_Control_Files)
                {
                    db.LD_DocSign_Control_Files.Remove(i);
                    db.SaveChanges();
                }

                docControl = db.LD_DocSign_Control.Where(x => x.ProposalID == ProposalID).FirstOrDefault();

                db.LD_DocSign_Control.Remove(docControl);
                db.SaveChanges();
            }
        }




        [AcceptVerbs("GET", "POST")]
        [ActionName("EmitirContrato")]
        public async System.Threading.Tasks.Task<IHttpActionResult> EmitirContratoAsync(EmitirContractoModel ec)
        {
            try
            {
                List<BB_Proposal_Quote> quotes = new List<BB_Proposal_Quote>();
                BB_Proposal_PrintingServices psconfigs = new BB_Proposal_PrintingServices();
                BB_FinancingType FinancingType = new BB_FinancingType();
                BB_Proposal b = new BB_Proposal();
                LD_Contrato c = new LD_Contrato();
                BB_Proposal_Financing financiamento = new BB_Proposal_Financing();
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    c = db.LD_Contrato.Where(x => x.ID == ec.id).FirstOrDefault();

                    //c.ContratoValidado = true;
                    //c.en

                }
                using (var db = new BB_DB_DEVEntities2())
                {

                    b = db.BB_Proposal.Where(x => x.ID == c.ProposalID).FirstOrDefault();
                    quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == b.ID).ToList();
                    psconfigs = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == b.ID).FirstOrDefault();
                    financiamento = db.BB_Proposal_Financing.Where(x => x.ProposalID == b.ID).FirstOrDefault();
                    if (financiamento != null)
                    {
                        FinancingType = db.BB_FinancingType.Where(x => x.Code == financiamento.FinancingTypeCode).FirstOrDefault();
                    }
                }


                EmailService emailSend = new EmailService();
                EmailMesage message = new EmailMesage();

                message.Destination = ec.EmailPara;
                message.Subject = "Konica Minolta - Contrato - Quote: " + b.CRM_QUOTE_ID;


                if (ec.gestorContaEmail != null && ec.gestorContaEmail.Length > 0)
                    message.CC = ec.gestorContaEmail;

                if (ec.EmailCC != null && ec.EmailCC.Length > 0)
                {
                    message.CC += (ec.gestorContaEmail != null && ec.gestorContaEmail.Length > 0) ? "," + ec.EmailCC : ec.EmailCC;

                }


                StringBuilder sb = new StringBuilder();
                sb.AppendLine("1. Condições Gerais.");
                sb.AppendLine("2. Contrato de Compra e Venda");
                //Termos



                if (FinancingType != null && FinancingType.Code == 3)
                {
                    sb.AppendLine("3. Contrato de Locação Mandatada");
                }

                if (psconfigs != null && psconfigs.modeId != 0)
                {
                    sb.AppendLine("4. Contrato de Prestação de Serviços de Cópia, Impressão, Digitalização e Manutenção");
                }

                sb.AppendLine("5. Contrato de Prestação de Serviços Informáticos e Manutenção");


                sb.AppendLine("6. Contrato de Prestação de Serviços de Consultoria em Soluções de Otimização de Impressão");

                foreach (var i in quotes)
                {
                    if (i.CodeRef == "9960OD32006" || i.CodeRef == "SRTU PPD" || i.CodeRef == "SRTU AV")
                    {
                        sb.AppendLine("7. Contrato de Serviço de Recolha de Toners Usados/ Gestão de Embalagem Vazias de Toner");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.Family.Contains("IMS"))
                    {
                        sb.AppendLine("8. Contrato de Prestação de Serviços de Manutenção e Suporte de Infraestruturas de Tecnologias de Informação");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.Family.Contains("MCS"))
                    {
                        sb.AppendLine("9. Contrato de Prestação de Serviços Específico de Implementação, Manutenção e Suporte a Soluções");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.Family.Contains("SW"))
                    {
                        sb.AppendLine("10. Contrato de Licenças de Software");
                        break;
                    }
                }
                foreach (var i in quotes)
                {
                    if (i.Family.Contains("CSV"))
                    {
                        sb.AppendLine("11. Contrato de Prestação de Serviços Cloud");
                        break;
                    }
                }
                foreach (var i in quotes)
                {
                    if (i.Description.Contains("Evolution"))
                    {
                        sb.AppendLine("12. Bizhub Evolution");
                        break;
                    }
                }


                //message.CC = ec.gestorContaEmail+ ","+ ec.EmailCC;
                //message.Subject = "Business Team - ";
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                strBuilder.Append("<br/>");
                strBuilder.Append("Bem vindo ao mundo Konica Minolta");
                strBuilder.Append("<br/>");
                strBuilder.Append("Segue o contrato para assinatura");
                strBuilder.Append("<br/>");
                strBuilder.Append("Por nos ter escolhido.");
                strBuilder.Append("<br/>");
                strBuilder.Append("<br/>");
                strBuilder.Append("Business Builder - Por favor, não responder a este email.");
                message.Body = strBuilder.ToString();

                await emailSend.SendEmailayncWithAttachement(message, c.PathContracto);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Ok();
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GerarContracto")]
        public IHttpActionResult GerarContracto(int? proposalID)
        {

            GerarContrato gc = new GerarContrato();
            gc.proposalID = proposalID;
            gc.ContractType = "";
            gc.FinancingType = "";
            gc.FinancingPaymentMethod = "";
            gc.monthly = new List<Monthly>();
            gc.trimestral = new List<Trimestral>();

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal b = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    BB_Proposal_Financing f = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    BB_Clientes c = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();
                    BB_Proposal_PrazoDiferenciado p = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                    if (c == null)
                        c = new BB_Clientes();

                    gc.ContractType = db.BB_FinancingContractType.Where(x => x.ID == f.ContractTypeId).Select(x => x.Company).FirstOrDefault();
                    gc.FinancingType = db.BB_FinancingType.Where(x => x.Code == f.FinancingTypeCode).Select(x => x.Type).FirstOrDefault();
                    gc.FinancingPaymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == f.PaymentMethodId).Select(x => x.Type).FirstOrDefault();
                    //gc.monthly = db.BB_Proposal_FinancingMonthly.Where(x => x.FinancingID == f.ID && x.ProposalID == proposalID).Select(x => new Monthly { Contracto = x.Contracto.Value, Value = x.Value.Value }).ToList();
                    //gc.trimestral = db.BB_Proposal_FinancingTrimestral.Where(x => x.FinancingID == f.ID && x.ProposalID == proposalID).Select(x => new Trimestral { Contracto = x.Contracto.Value, Value = x.Value.Value }).ToList();
                    gc.ClienteAccountNumber = c != null ? c.accountnumber : "";
                    gc.ClienteName = c != null ? c.Name : "";
                    gc.Locadora = p != null ? p.Alocadora : "";
                    gc.NomeProposta = b.Name;
                    gc.Renda = p != null ? p.ValorRenda : 0;
                    gc.NrMeses = p != null ? p.PrazoDiferenciado : 0;
                    gc.QuoteNumber = b.CRM_QUOTE_ID;

                    gc.lst_BB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();

                    gc.lst_BB_Proposal_Contacts_Documentation = 
                        db.BB_Proposal_Contacts_Documentation.Where(x => x.ProposalID == proposalID).ToList();

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Ok(gc);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GerarContractoFormal")]
        public IHttpActionResult GerarContractoFormal(int? proposalID)
        {

            GerarContratoFormal gc = new GerarContratoFormal();
            gc.proposalID = proposalID;
            gc.ContractType = "";
            gc.FinancingType = "";
            gc.FinancingPaymentMethod = "";
            gc.monthly = new List<Monthly>();
            gc.trimestral = new List<Trimestral>();
            BB_Proposal b = new BB_Proposal();
            BB_Clientes c = new BB_Clientes();
            BB_FinancingType FinancingType = new BB_FinancingType();
            BB_FinancingPaymentMethod aymentMethod = new BB_FinancingPaymentMethod();
            BB_FinancingContractType contractType = new BB_FinancingContractType();
            BB_Proposal_PrazoDiferenciado prazodiferencido = new BB_Proposal_PrazoDiferenciado();
            BB_Proposal_Financing financiamento = new BB_Proposal_Financing();
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    financiamento = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    BB_Proposal_Financing f = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    FinancingType = db.BB_FinancingType.Where(x => x.Code == financiamento.FinancingTypeCode).FirstOrDefault();
                    aymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == financiamento.PaymentMethodId).FirstOrDefault();
                    contractType = db.BB_FinancingContractType.Where(x => x.ID == financiamento.ContractTypeId).FirstOrDefault();
                    prazodiferencido = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID && x.Type == "TAB" && x.IsAproved.Value == true).FirstOrDefault();

                    b = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    c = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();

                    if (c == null)
                        c = new BB_Clientes();

                    gc.ContractType = contractType != null ? contractType.Company : "";
                    gc.FinancingType = FinancingType != null ? FinancingType.Type : "";
                    gc.FinancingPaymentMethod = aymentMethod != null ? aymentMethod.Type : "";
                    gc.NIF = c.NIF;
                    gc.Morada = c.address1_line1;
                    gc.ENDEREÇODEMAIL = c.emailaddress1;
                    gc.Cliente = c.Name;



                }
            }


            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return Ok(gc);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DocumentoDownload")]
        public HttpResponseMessage DocumentoDownload(int? DocumentoID)
        {

            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            string path = "";
            try
            {
                LD_DocumentProposal documento = new LD_DocumentProposal();

                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    documento = db.LD_DocumentProposal.Where(x => x.ID == DocumentoID).First();

                }

                //Check whether File exists.
                if (!File.Exists(documento.FileFullPath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: .");
                    throw new HttpResponseException(response);
                }

                byte[] bytes = File.ReadAllBytes(documento.FileFullPath);

                //Set the Response Content.
                string filename = Path.GetFileName(documento.FileFullPath);
                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = filename;

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));




                HttpResponseMessage response1 = new HttpResponseMessage(HttpStatusCode.OK);
                response1.Content = new StreamContent(new FileStream(documento.FileFullPath, FileMode.Open, FileAccess.Read));
                response1.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                response1.Content.Headers.ContentDisposition.FileName = filename;
                response1.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));

                return response1;

            }
            catch (Exception ex)
            {

                return response;
            }

            return response;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("SavePA5Documents")]
        public IHttpActionResult SavePA5Documents(ListPA5Documents lst)
        {
            try
            {
                //Desactiva Email
                if (lst.Mode == 4)
                {
                    using (var db = new BB_DB_DEV_LeaseDesk())
                    {
                        var itemsToDelete = db.LD_PA5_EmailConfigSent.Where(x => x.ContractID == lst.ContractID);

                        if (itemsToDelete.Count() > 0)
                        {
                            db.LD_PA5_EmailConfigSent.RemoveRange(itemsToDelete);
                            db.SaveChanges();

                            LD_Email_Log l1 = new LD_Email_Log();
                            l1.ProcessDate = DateTime.Now.AddHours(1);
                            l1.EmailSent = lst.ClientesEmails;
                            l1.EmailReceived = "";
                            l1.Status = "Email Desactivo";
                            l1.ContractID = lst.ContractID;
                            l1.QuoteNumber = "";
                            db.LD_Email_Log.Add(l1);
                            db.SaveChanges();
                        }

                    }

                    return Ok();
                }

                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    db.LD_PA5_DocumentProposal.RemoveRange(db.LD_PA5_DocumentProposal.Where(x => x.ContractID == lst.ContractID));
                    db.SaveChanges();

                    List<LD_PA5_DocumentProposal> lstLD_PA5_DocumentProposal = new List<LD_PA5_DocumentProposal>();

                    LD_Email_Log l = new LD_Email_Log();
                    l.ProcessDate = DateTime.Now.AddHours(1);
                    l.EmailSent = lst.ClientesEmails;
                    l.EmailReceived = "";
                    l.Status = "Preparado P/ Enviar";
                    l.ContractID = lst.ContractID;
                    l.QuoteNumber = "";

                    db.LD_Email_Log.Add(l);

                    //foreach (var item in lst.LstPA5Documents)
                    //{
                    //    //Outros
                    //    if (item.ID == 6 && item.Name != "Outros(s)" && item.IsToSend)
                    //    {
                    //        string[] arrLst = item.Name.Split(';');
                    //        foreach (var i in arrLst)
                    //        {
                    //            LD_PA5_DocumentType docType = new LD_PA5_DocumentType();
                    //            docType.IsEnabled = true;
                    //            docType.Name = i;
                    //            docType.IsRequired = true;

                    //            db.LD_PA5_DocumentType.Add(docType);
                    //            db.SaveChanges();

                    //        }

                    //    }
                    //}

                    foreach (var item in lst.LstPA5Documents)
                    {

                        //LD_PA5_DocumentProposal _LD_PA5_DocumentProposal = new LD_PA5_DocumentProposal();

                        //Outros
                        if (item.ID == 6 && item.Name != "Outros(s)" && item.IsToSend)
                        {
                            string[] arrLst = item.Name.Split(';');
                            foreach (var i in arrLst)
                            {
                                LD_PA5_DocumentProposal _LD_PA5_DocumentProposal = new LD_PA5_DocumentProposal();
                                _LD_PA5_DocumentProposal.ContractID = lst.ContractID;

                                LD_PA5_DocumentType docType = new LD_PA5_DocumentType();
                                docType.IsEnabled = true;
                                docType.Name = i;
                                docType.IsRequired = true;

                                db.LD_PA5_DocumentType.Add(docType);
                                db.SaveChanges();

                                //_LD_PA5_DocumentProposal.PA5DocumentID = db.BB_Documents_Type.Where(x => x.Name == i).Select(x => x.ID).FirstOrDefault();
                                _LD_PA5_DocumentProposal.PA5DocumentID = docType.ID;
                                _LD_PA5_DocumentProposal.IsToSend = item.IsToSend;
                                _LD_PA5_DocumentProposal.IsValid = item.IsValid;
                                lstLD_PA5_DocumentProposal.Add(_LD_PA5_DocumentProposal);
                            }
                        }
                        else
                        {
                            LD_PA5_DocumentProposal _LD_PA5_DocumentProposal = new LD_PA5_DocumentProposal();
                            _LD_PA5_DocumentProposal.ContractID = lst.ContractID;
                            _LD_PA5_DocumentProposal.PA5DocumentID = item.ID;
                            _LD_PA5_DocumentProposal.IsToSend = item.IsToSend;
                            _LD_PA5_DocumentProposal.IsValid = item.IsValid;
                            lstLD_PA5_DocumentProposal.Add(_LD_PA5_DocumentProposal);
                        }

                        //lstLD_PA5_DocumentProposal.Add(_LD_PA5_DocumentProposal);
                    }

                    db.LD_PA5_DocumentProposal.AddRange(lstLD_PA5_DocumentProposal);
                    db.SaveChanges();

                    if (lst.Mode == 2) //envia email de boas vindas
                    {
                        LD_PA5_EmailConfigSent _LD_PA5_EmailConfigSent = new LD_PA5_EmailConfigSent();
                        _LD_PA5_EmailConfigSent.ContractID = lst.ContractID;
                        _LD_PA5_EmailConfigSent.CreatedBy = lst.EmailCreatedBy;
                        _LD_PA5_EmailConfigSent.EmailCC = lst.EmailCC;
                        _LD_PA5_EmailConfigSent.CreateTime = DateTime.Now.AddDays(1);
                        _LD_PA5_EmailConfigSent.EmailTo = lst.ClientesEmails;
                        _LD_PA5_EmailConfigSent.NextDateSent = DateTime.Now;
                        _LD_PA5_EmailConfigSent.Notes = lst.PA5DocumentosEmailNotas;
                        _LD_PA5_EmailConfigSent.Nr_Reminder = 1;
                        _LD_PA5_EmailConfigSent.IsFinish = false;
                        _LD_PA5_EmailConfigSent.IsStarted = false;
                        _LD_PA5_EmailConfigSent.Mode = 2;
                        _LD_PA5_EmailConfigSent.TypeEmail = 1;
                        db.LD_PA5_EmailConfigSent.Add(_LD_PA5_EmailConfigSent);
                        db.SaveChanges();

                        //Criar Reminder
                        LD_PA5_EmailConfigSent _LD_PA5_EmailConfigSent1 = new LD_PA5_EmailConfigSent();
                        _LD_PA5_EmailConfigSent1.ContractID = lst.ContractID;
                        _LD_PA5_EmailConfigSent1.CreatedBy = lst.EmailCreatedBy;
                        _LD_PA5_EmailConfigSent1.CreateTime = DateTime.Now.AddDays(3);
                        _LD_PA5_EmailConfigSent1.EmailTo = lst.ClientesEmails;
                        _LD_PA5_EmailConfigSent.EmailCC = lst.EmailCC;
                        _LD_PA5_EmailConfigSent1.NextDateSent = DateTime.Now;
                        _LD_PA5_EmailConfigSent1.Notes = lst.PA5DocumentosEmailNotas;
                        _LD_PA5_EmailConfigSent1.Nr_Reminder = 3;
                        _LD_PA5_EmailConfigSent1.IsFinish = false;
                        _LD_PA5_EmailConfigSent1.IsStarted = false;
                        _LD_PA5_EmailConfigSent1.Mode = 2;
                        _LD_PA5_EmailConfigSent1.TypeEmail = 2;
                        db.LD_PA5_EmailConfigSent.Add(_LD_PA5_EmailConfigSent1);
                        db.SaveChanges();
                    }

                    if (lst.Mode == 3) //envia email solicitar mais documentos
                    {
                        LD_PA5_EmailConfigSent _LD_PA5_EmailConfigSent = new LD_PA5_EmailConfigSent();
                        _LD_PA5_EmailConfigSent.ContractID = lst.ContractID;
                        _LD_PA5_EmailConfigSent.CreatedBy = lst.EmailCreatedBy;
                        _LD_PA5_EmailConfigSent.CreateTime = DateTime.Now.AddDays(1);
                        _LD_PA5_EmailConfigSent.EmailTo = lst.ClientesEmails;
                        _LD_PA5_EmailConfigSent.EmailCC = lst.EmailCC;
                        _LD_PA5_EmailConfigSent.NextDateSent = DateTime.Now;
                        _LD_PA5_EmailConfigSent.Notes = lst.PA5DocumentosEmailNotas;
                        _LD_PA5_EmailConfigSent.Nr_Reminder = 3;
                        _LD_PA5_EmailConfigSent.IsFinish = false;
                        _LD_PA5_EmailConfigSent.IsStarted = false;
                        _LD_PA5_EmailConfigSent.Mode = 3;
                        _LD_PA5_EmailConfigSent.TypeEmail = 2;
                        db.LD_PA5_EmailConfigSent.Add(_LD_PA5_EmailConfigSent);
                        db.SaveChanges();
                    }



                }
                return Ok();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPA5Documents")]
        public IHttpActionResult GetPA5Documents(int? contractID)
        {


            try
            {
                List<LD_PA5_DocumentType> lstLD_PA5_DocumentType = new List<LD_PA5_DocumentType>();
                List<LD_PA5_DocumentProposal> lstLD_PA5_DocumentProposal = new List<LD_PA5_DocumentProposal>();
                List<PA5Documents> lstPA5Documents = new List<PA5Documents>();
                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    lstLD_PA5_DocumentType = db.LD_PA5_DocumentType.ToList();
                    lstLD_PA5_DocumentProposal = db.LD_PA5_DocumentProposal.Where(x => x.ContractID == contractID).ToList();

                    //Auqi colocar a lista Outros em ultimo lugar.
                    foreach (var doc in lstLD_PA5_DocumentType)
                    {
                        PA5Documents _PA5Documents = new PA5Documents();
                        LD_PA5_DocumentProposal _LD_PA5_DocumentProposal = lstLD_PA5_DocumentProposal.Where(x => x.PA5DocumentID == doc.ID).FirstOrDefault();
                        if (_LD_PA5_DocumentProposal != null)
                        {
                            _PA5Documents.ID = doc.ID;
                            _PA5Documents.Name = doc.Name;
                            _PA5Documents.IsValid = _LD_PA5_DocumentProposal.IsValid.GetValueOrDefault();
                            _PA5Documents.IsToSend = _LD_PA5_DocumentProposal.IsToSend.GetValueOrDefault();

                        }
                        else
                        {
                            _PA5Documents.ID = doc.ID;
                            _PA5Documents.Name = doc.Name;
                            _PA5Documents.IsValid = false;
                            _PA5Documents.IsToSend = false;
                        }
                        lstPA5Documents.Add(_PA5Documents);


                    }
                }
                return Ok(lstPA5Documents);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveSalesProcessContacts")]
        public IHttpActionResult SaveSalesProcessContacts(SalesProcessViewModel spvm)
        {
            if (spvm.ProposalID != null)
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    LD_Contrato ldc = db.LD_Contrato.Where(x => x.ProposalID == spvm.ProposalID).FirstOrDefault();
                    ldc.SystemAssinaturaID = spvm.SignatureType;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return NotFound();
                    }
                }
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<SignatureContact> signatureContacts = spvm.SignatureContacts;
                    List<BB_Proposal_Contacts_Signing> dbContacts = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == spvm.ProposalID).ToList();
                    List<int> toDeleteIds = dbContacts.Select(x => x.ID).Except(signatureContacts.Select(x => x.ID.GetValueOrDefault())).ToList();
                    if (toDeleteIds.Count > 0)
                    {
                        db.BB_Proposal_Contacts_Signing.RemoveRange(dbContacts.Where(x => toDeleteIds.Contains(x.ID)).ToList());
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    foreach (SignatureContact sc in spvm.SignatureContacts)
                    {
                        if (sc.ID == null)
                        {
                            BB_Proposal_Contacts_Signing contact = new BB_Proposal_Contacts_Signing()
                            {
                                ProposalID = spvm.ProposalID,
                                Email = sc.Email,
                                Name = sc.Name,
                                Telefone = sc.Telephone
                            };
                            db.BB_Proposal_Contacts_Signing.Add(contact);
                        }
                        else
                        {
                            BB_Proposal_Contacts_Signing match = dbContacts.Where(x => x.ID == (int)sc.ID).FirstOrDefault();
                            if (match != null)
                            {
                                match.Email = sc.Email;
                                match.Name = sc.Name;
                                match.Telefone = sc.Telephone;
                            }
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return NotFound();
                        }
                    }

                }
                //using (var db = new BB_DB_DEV_LeaseDesk())
                //{
                //    LD_DocSign_Control aux = new LD_DocSign_Control();
                //    aux.
                //}
            }
            return Ok();
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetPA5DocumentsEmail")]
        public IHttpActionResult GetPA5DocumentsEmail(int? contractID)
        {

            string email = "";
            try
            {

                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    LD_Contrato l = db.LD_Contrato.Where(x => x.ID == contractID).FirstOrDefault();

                    BB_Proposal_Contacts_Documentation s = db.BB_Proposal_Contacts_Documentation.Where(x => x.ProposalID == l.ProposalID).FirstOrDefault();

                    if (s != null)
                    {
                        email = s.Email;
                    }



                }
                return Ok(email);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(email);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("TesteEmailSMPT")]
        public IHttpActionResult TesteEmailSMPT()
        {
            EmailService a = new EmailService();
            a.SendEmailayncTeste();
            return Ok(true);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetSitesTabContractInfo")]
        public IHttpActionResult GetSitesTabContractInfo(int? contractID)
        {
            MultiData data = new MultiData();
            //BB_Proposal proposalObj = new BB_Proposal();
            int? proposalID = null;
            try
            {
                if (contractID != null)
                {
                    using (var db = new BB_DB_DEV_LeaseDesk())
                    {

                        int? contractProposal = db.LD_Contrato.Where(x => x.ID == contractID).Select(x => x.ProposalID).FirstOrDefault();

                        if(contractProposal != null)
                        {
                            BB_Proposal proposalObj = db.BB_Proposal.Where(x => x.ID == contractProposal).FirstOrDefault();
                            data.isMultipleContracts = proposalObj.IsMultipleContract;
                            data.ClientAccountNumber = proposalObj.ClientAccountNumber;
                            data.Plant = proposalObj.Plant;
                            data.CodArrend = proposalObj.CodArrend;
                            data.AccordNumber = proposalObj.AccordNumber;
                            data.ContractNumberPai = proposalObj.ContractNumberPai;
                            data.DataFechoContracto = proposalObj.DataFechoContracto;

                            // ###############################################################
                            // PARA EFEITOS DE TESTE, APAGAR DEPOIS <------ ##################
                            data.Plant = "TESTE PLANT";                             // #######
                            data.CodArrend = "TESTE CODARREND";                     // #######
                            data.AccordNumber = "TESTE ACCORDNUMBER";               // #######
                            data.ContractNumberPai = "TESTE CONTRACTNUMBERPAI";     // #######
                            // ###############################################################
                            // ###############################################################

                            proposalID = proposalObj.ID;
                        }
                    }
                    using (var dbX = new BB_DB_DEVEntities2())
                    {
                        if (proposalID != null)
                        {
                            List<BB_Proposal_DeliveryLocation> bb_pp_dl_lst = dbX.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalID).ToList();

                            data.DL_Table_Info_Lst = new List<DL_Table_Info>();

                            foreach(var deliverLocation in bb_pp_dl_lst)
                            {
                                DL_Table_Info dl_info = new DL_Table_Info();

                                dl_info.Tipo = deliverLocation.AccountType;
                                dl_info.DeliveryLocation = deliverLocation.Adress1 + " " + deliverLocation.PostalCode;

                                BB_LocaisEnvio bb_local_envio = dbX.BB_LocaisEnvio.Where(x => x.ID == deliverLocation.IDX).FirstOrDefault();
                                if(bb_local_envio != null)
                                {
                                    dl_info.CIF = bb_local_envio.NIF_CIF;
                                }

                                // VERIFICAR ##########################################
                                dl_info.CompanyName = "VERIFICAR";
                                dl_info.SAP_Nr = "VERIFICAR";
                                dl_info.SAP_Company = "VERIFICAR";   

                                data.DL_Table_Info_Lst.Add(dl_info);
                            }
                        }
                    }
                }

            return Ok(data);

            }catch(Exception ex)
            {
                string errorMessage = ex.Message;
                return Ok(data);
            }
        }
        public class MultiData
        {
            public bool? isMultipleContracts { get; set; }
            public string ClientAccountNumber { get; set; }
            public string Plant { get; set; }
            public string CodArrend { get; set; }
            public string AccordNumber { get; set; }
            public string ContractNumberPai { get; set; }
            public DateTime? DataFechoContracto { get; set; }

            public List<DL_Table_Info> DL_Table_Info_Lst { get; set; }

        }

        public class DL_Table_Info
        {
            public string Tipo { get; set; }
            public string CompanyName { get; set; }
            public string CIF { get; set; }
            public string DeliveryLocation { get; set; }
            public string SAP_Nr { get; set; }
            public string SAP_Company { get; set; }

        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveMultiContractInfo")]
        public IHttpActionResult SaveMultiContractInfo(int? contractID, string soldTo, bool isMultipleContract)
        {
            try
            {
                if (contractID != null)
                {
                    using (var db = new BB_DB_DEV_LeaseDesk())
                    {
                        int? contractProposal = db.LD_Contrato.Where(x => x.ID == contractID).Select(x => x.ProposalID).FirstOrDefault();

                        if (contractProposal != null)
                        {
                            BB_Proposal bb_proposal = db.BB_Proposal.Where(x => x.ID == contractProposal).FirstOrDefault();
                            bb_proposal.IsMultipleContract = isMultipleContract;
                            if(soldTo != null)
                            {
                                BB_Proposal_Client bb_Proposal_Client = db.BB_Proposal_Client.Where(x => x.ProposalID == bb_proposal.ID).FirstOrDefault();

                                bb_Proposal_Client.ClientID = soldTo;
                                bb_proposal.ClientAccountNumber= soldTo;

                            }

                            db.Entry(bb_proposal).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return null;
            }
        }



    }
    public class GravarObser
    {

        public int? id { get; set; }

        public string Observacoes { get; set; }
        public string modifiedby { get; set; }
        public int? motivoID { get; set; }

    }
    public class PrazoDiferenciado
    {
        public int ID { get; set; }
        public int ProposalID { get; set; }
        public string Cliente { get; set; }
        public double? ValorProposta { get; set; }
        public string NIF { get; set; }
        public string ProdutoFinanceiro { get; set; }
        public double? ValorRenda { get; set; }
        public double? ValorFactor { get; set; }
        public int? nrMeses { get; set; }
        public int? Frequency { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public string Alocadora { get; set; }

        public string comments { get; set; }

        public bool? isApproved { get; set; }

        public string Type1 { get; set; }
        public string AccountNumber { get; set; }

        public string quotenumber { get; set; }

        public string GestorContaObservacoes { get; set; }

        public List<BB_Proposal_PrazoDiferenciado_History> history { get; set; }

        public ValoresTotaisRenda vva { get; set; }
        public bool? Flexpage { get; set; }

        public Nullable<bool> IsComplete { get; set; }

        public bool? isSobrevarizacao { get; set; }

        public double? totalSobrevalorizacao { get; set; }

        public string NLocadora { get; set; }

        public List<BB_Proposal_Overvaluation> sobrevalorizacao { get; set; }
        public List<BB_Proposal_Aprovacao_Quote> configuracaoNegocio { get; set; }
        public string DSO { get; set; }

        public double? leicopiaprivada { get; set; }

        public int nrMaquinas { get; set; }
        public double? TaxaMensalServicosRecorrentes { get; set; }

        public double? TaxaMensalOPSManage { get; set; }

        public double? ValorBnpAprovacao { get; set; }

        public DateTime? DataExpiracao { get; set; }

    }


    public class LD_Contrato_Model
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public string QuoteNumber { get; set; }
        public string PathContracto { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<bool> ContratoValidado { get; set; }
        public Nullable<bool> ContratoGerado { get; set; }
        public Nullable<System.DateTime> Assinatura { get; set; }
        public Nullable<int> SystemAssinaturaID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string FilenameContracto { get; set; }
        public Nullable<int> TipoContratoID { get; set; }
        public string ComentariosGC { get; set; }

        public string TipoContrato { get; set; }

        public string ComentariosDevolucao { get; set; }

        public Nullable<int> MotivoID { get; set; }

        public Nullable<int> DevolucaoMotivoID { get; set; }

        public bool? isClosed { get; set; }

        public string NUS { get; set; }

        public string FacuracaoModifiedby { get; set; }
        public DateTime? FacuracaoModifiedtime { get; set; }

        public string FolderDOC { get; set; }

        public bool? IsButtonImitirDocSign { get; set; }
        public bool? IsButtonSuspender { get; set; }
        public bool? IsButtonDelete { get; set; }
    }

}
