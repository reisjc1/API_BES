using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;
using System.Data.Entity;
using WebApplication1.Models.ViewModels;
using System.Text;
using WebApplication1.Controllers;
using WebApplication1.Models.RequestModels;
using WebApplication1.Models.ViewModels.Portal_KM.Models.ViewModels.ServiceViewModels;
using WebApplication1.Models.ViewModels.PrintingServicesViewModels;
using System.Data.SqlClient;
using System.Data;
using WebApplication1.App_Start;

namespace WebApplication1.BLL
{
    public class ServiceBLL
    {
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public List<ServiceValidationRequestIndexEntry> GetServiceRequestIndexEntries()
        {

            List<BB_Proposal_PrintingServiceValidationRequest> validationRequests = db.BB_Proposal_PrintingServiceValidationRequest
                .Include(x => x.BB_PrintingServices.BB_VVA)
                .Include(x => x.BB_PrintingServices.BB_PrintingServices_NoVolume)
                .Where(x => x.IsComplete == false && x.ToDelete == false)
                .ToList();
            List<ServiceValidationRequestIndexEntry> indexEntries = new List<ServiceValidationRequestIndexEntry>();
            foreach (BB_Proposal_PrintingServiceValidationRequest validationRequest in validationRequests)
            {
                string clientAccountNumber = db.BB_Proposal.Where(x => x.ID == validationRequest.BB_PrintingServices.BB_Proposal_PrintingServices2.ProposalID).Select(x => x.ClientAccountNumber).FirstOrDefault();
                string clientName = db.BB_Clientes.Where(x => x.accountnumber == clientAccountNumber).Select(x => x.Name).FirstOrDefault();
                string serviceType = "";
                int serviceTypeId = 0;
                if (validationRequest.BB_PrintingServices.BB_VVA != null)
                {
                    serviceType = "Click Global - VVA";
                    serviceTypeId = 1;
                }
                else if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume != null)
                {
                    serviceType = "Click Global - Sem Volume Incluído";
                    serviceTypeId = 2;
                }
                else
                {
                    serviceType = "Click Por Modelo - Sem Volume Incluído";
                    serviceTypeId = 3;
                }
                ServiceValidationRequestIndexEntry indexEntry = new ServiceValidationRequestIndexEntry()
                {
                    ID = validationRequest.ID,
                    Type = serviceType,
                    TypeID = serviceTypeId,
                    ClientName = clientName,
                    RequestedBy = validationRequest.RequestedBy,
                    RequestedAt = (DateTime)validationRequest.RequestedAt,
                    Observations = validationRequest.SEObservations,

                };
                indexEntries.Add(indexEntry);
            }
            return indexEntries;
        }


        public void DeleteServiceById (int id)
        {
            try
            {
                BB_Proposal_PrintingServiceValidationRequest a = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.ID == id).FirstOrDefault();
                if (a != null)
                {
                    a.ToDelete = true;

                    db.Entry(a).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
            }

         
        }

        public ActionResponse RequestServiceController(ServiceInputs si, ProposalRootObject proposal)
        {
            ActionResponse err = new ActionResponse();
            err.ProposalObj = proposal;
            try
            {
                bool serviceRequestExists = ServiceRequestExists(proposal, si);
                if (serviceRequestExists)
                {
                    err.Message = "Já existe um pedido de cotação para os valores inseridos.";
                }
                else
                {

                    BB_PrintingServices newPS = CreateServiceRequest(proposal, si);
                    if (newPS != null)
                    {
                        List<BB_Proposal_PrintingServiceValidationRequest> psvr = newPS.BB_Proposal_PrintingServiceValidationRequest.ToList();
                        ApprovedPrintingService pendingQuoteRequest = new ApprovedPrintingService
                        {
                            BWVolume = newPS.BWVolume.Value,
                            CVolume = newPS.CVolume.Value,
                            RequestedAt = psvr[0].RequestedAt,
                            ContractDuration = newPS.ContractDuration.Value,
                            Fee = newPS.Fee.Value,
                            IsPrecalc = newPS.IsPrecalc.Value,
                            SEObservations = psvr[0].SEObservations,
                            SCObservations = psvr[0].SCObservations,
                            ID = newPS.ID,
                            Machines = new List<Machine>(),
                        };
                        int serviceType = 0;
                        if (newPS.BB_VVA != null)
                        {
                            serviceType = 1;
                            pendingQuoteRequest.GlobalClickVVA = new GlobalClickVVA
                            {
                                BWExcessPVP = newPS.BB_VVA.BWExcessPVP.Value,
                                CExcessPVP = newPS.BB_VVA.CExcessPVP.Value,
                                ExcessBillingFrequency = newPS.BB_VVA.ExcessBillingFrequency.Value,
                                RequestedBWExcess = newPS.BB_VVA.RequestedBWExcess.Value,
                                RequestedCExcess = newPS.BB_VVA.RequestedCExcess.Value,
                                RequestedRent = newPS.BB_VVA.RequestedRent.Value,
                                PVP = newPS.BB_VVA.PVP.Value,
                                RentBillingFrequency = newPS.BB_VVA.RentBillingFrequency.Value,
                                ReturnType = newPS.BB_VVA.ReturnType.Value
                            };
                        }
                        else if (newPS.BB_PrintingServices_NoVolume != null)
                        {
                            serviceType = 2;
                            pendingQuoteRequest.GlobalClickNoVolume = new GlobalClickNoVolume
                            {
                                GlobalClickBW = newPS.BB_PrintingServices_NoVolume.GlobalClickBW.Value,
                                GlobalClickC = newPS.BB_PrintingServices_NoVolume.GlobalClickC.Value,
                                PageBillingFrequency = newPS.BB_PrintingServices_NoVolume.PageBillingFrequency.Value,
                                RequestedGlobalClickBW = newPS.BB_PrintingServices_NoVolume.RequestedGlobalClickBW.Value,
                                RequestedGlobalClickC = newPS.BB_PrintingServices_NoVolume.RequestedGlobalClickC.Value,
                            };
                        }
                        else
                        {
                            serviceType = 3;
                            pendingQuoteRequest.Machines = si.PS_Basket;
                            pendingQuoteRequest.ClickPerModel = new ClickPerModel()
                            {
                                PageBillingFrequency = newPS.BB_PrintingServices_ClickPerModel.PageBillingFrequency.Value
                            };
                        }
                        int requestID = newPS.BB_Proposal_PrintingServiceValidationRequest.ToList()[0].ID;
                        string origin = proposal.Draft.details.ModifiedBy;
                        //SendServiceRequestEmail(origin, requestID, serviceType);
                        proposal.Draft.printingServices2.PendingServiceQuoteRequests.Add(pendingQuoteRequest);
                        err.Message = "Pedido de Serviço efectuado com sucesso!";
                    }
                }
                err.ProposalObj = proposal;
                return err;
            }
            catch (Exception e)
            {
                err.Message = "Falhou Pedido de serviço! Contactar a equipa do BB.";
                return err;
            }
        }

        private bool ServiceRequestExists(ProposalRootObject proposal, ServiceInputs si)
        {
            BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2
                .Where(x => x.ID == proposal.Draft.printingServices2.ID)
                .Include(x => x.BB_PrintingServices)
                .Include(x => x.BB_PrintingServices.Select(ps => ps.BB_Proposal_PrintingServiceValidationRequest))
                .Include(x => x.BB_PrintingServices.Select(ps => ps.BB_VVA))
                .Include(x => x.BB_PrintingServices.Select(ps => ps.BB_PrintingServices_NoVolume))
                .FirstOrDefault();

            List<BB_PrintingServices> printingServices = ps2.BB_PrintingServices.Where(x => !x.IsPrecalc.Value).ToList();
            bool requestExists = false;
            foreach (BB_PrintingServices ps in printingServices)
            {
                BB_Proposal_PrintingServiceValidationRequest validationRequest = ps.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.ToDelete == false).FirstOrDefault();
                if (validationRequest != null && !validationRequest.IsComplete.Value && ps.BWVolume == si.BWVolume && ps.CVolume == si.CVolume && ps.ContractDuration == si.ContractDuration)
                {
                    if ((si.RentBillingFrequency != null && ps.BB_VVA != null) || (si.PageBillingFrequency != null && ps.BB_PrintingServices_NoVolume != null))
                    {
                        requestExists = true;
                    }
                }
            }
            return requestExists;
        }
        private BB_PrintingServices CreateServiceRequest(ProposalRootObject proposal, ServiceInputs si)
        {
            BB_PrintingServices newPS = new BB_PrintingServices()
            {
                BWVolume = si.BWVolume,
                CVolume = si.CVolume,
                ContractDuration = si.ContractDuration,
                PrintingServices2ID = proposal.Draft.printingServices2.ID.Value,
                Fee = 0,
                IsPrecalc = false
            };
            db.BB_PrintingServices.Add(newPS);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            if (si.ExcessBillingFrequency != null && si.RentBillingFrequency != null)
            {
                BB_VVA vva = new BB_VVA()
                {
                    BWExcessPVP = si.RecommendedBWExcess,
                    CExcessPVP = si.RecommendedCExcess,
                    ExcessBillingFrequency = si.ExcessBillingFrequency,
                    PVP = si.RecommendedRent,
                    RentBillingFrequency = si.RentBillingFrequency,
                    PrintingServiceID = newPS.ID,
                    ReturnType = si.ReturnType,
                    RequestedBWExcess = si.RequestedBWExcess,
                    RequestedCExcess = si.RequestedCExcess,
                    RequestedRent = si.RequestedRent,
                };
                db.BB_VVA.Add(vva);
                try
                {
                    db.SaveChanges();
                    newPS.BB_VVA = vva;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            else if (si.PS_Basket != null)
            {
                BB_PrintingServices_ClickPerModel cpm = new BB_PrintingServices_ClickPerModel()
                {
                    PageBillingFrequency = si.PageBillingFrequency,
                    PrintingServiceID = newPS.ID
                };
                db.BB_PrintingServices_ClickPerModel.Add(cpm);
                try
                {
                    db.SaveChanges();
                    newPS.BB_PrintingServices_ClickPerModel = cpm;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                foreach (Machine m in si.PS_Basket)
                {
                    BB_PrintingService_Machines newMachine = new BB_PrintingService_Machines()
                    {
                        PrintingServiceID = newPS.ID,
                        BWCost = m.BWCost,
                        CCost = m.CCost,
                        BWPVP = m.ClickPriceBW,
                        CPVP = m.ClickPriceC,
                        CodeRef = m.CodeRef,
                        BWVolume = m.bwPages,
                        CVolume = m.cPages,
                        Description = m.Description,
                        IsInClient = m.IsInClient,
                        IsUsed = m.IsUsed,
                        Quantity = m.Qty,
                        RequestedBWClickPrice = m.RequestedBWClickPrice,
                        RequestedCClickPrice = m.RequestedCClickPrice,
                    };
                    db.BB_PrintingService_Machines.Add(newMachine);
                    try
                    {
                        db.SaveChanges();
                        newPS.BB_PrintingService_Machines.Add(newMachine);
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
            }
            else if (si.PageBillingFrequency != null)
            {
                BB_PrintingServices_NoVolume nv = new BB_PrintingServices_NoVolume()
                {
                    GlobalClickBW = si.RecommendedGlobalClickBW,
                    GlobalClickC = si.RecommendedGlobalClickC,
                    RequestedGlobalClickBW = si.RequestedGlobalClickBW,
                    RequestedGlobalClickC = si.RequestedGlobalClickC,
                    PageBillingFrequency = si.PageBillingFrequency,
                    PrintingServiceID = newPS.ID,
                };
                db.BB_PrintingServices_NoVolume.Add(nv);
                try
                {
                    db.SaveChanges();
                    newPS.BB_PrintingServices_NoVolume = nv;
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            BB_Proposal_PrintingServiceValidationRequest serviceRequest = new BB_Proposal_PrintingServiceValidationRequest()
            {
                IsApproved = false,
                ApprovedBy = null,
                RequestedAt = DateTime.Now,
                IsComplete = false,
                PrintingServiceID = newPS.ID,
                RequestedBy = proposal.Draft.details.ModifiedBy,
                SEObservations = si.Observations,
                ToDelete = false
            };
            db.BB_Proposal_PrintingServiceValidationRequest.Add(serviceRequest);
            try
            {
                db.SaveChanges();
                newPS.BB_Proposal_PrintingServiceValidationRequest.Add(serviceRequest);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return newPS;
        }
        private async void SendServiceRequestEmail(string origin, int requestID, int serviceType)
        {
            EmailService emailSend = new EmailService();
            EmailMesage message = new EmailMesage();

            message.Destination = "andre.gomes@konicaminolta.pt";
            message.Subject = "BB Serviço - Pedido de Cotação: " + origin;
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<p>Caro(a),</p>");
            strBuilder.Append("<p>Foi registado um novo pedido de cotação.</p>");
            strBuilder.Append("<p>Para darmos seguimento ao processo, clique no seguinte link:</p>");
            strBuilder.Append("<p>Para aceder à aplicação, por favor utilize o seguinte link: " + "https://bb.konicaminolta.pt/Service/Edit/" + requestID + "?TypeId=" + serviceType + "</p>");
            strBuilder.Append("<br/>");
            strBuilder.Append("<p>Ao seu dispor,</p>");
            strBuilder.Append("<p>BB Serviço</p>");
            message.Body = strBuilder.ToString();

            await emailSend.SendEmailaync(message);
        }
        public List<ServiceValidationRequestHistoryEntry> GetServiceRequestHistory()
        {
            List<ServiceValidationRequestHistoryEntry> history = new List<ServiceValidationRequestHistoryEntry>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_Service_ALL_V1", conn);
                    //cmd.Parameters.Add("@year1", SqlDbType.NVarChar, 20).Value = year;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {

                        ServiceValidationRequestHistoryEntry m = new ServiceValidationRequestHistoryEntry();
                        m.ID = (int)rdr["ID"];
                        m.ApprovedAt = DateTime.Parse(rdr["ApprovedAt"].ToString());
                        m.RequestedAt = DateTime.Parse(rdr["RequestedAt"].ToString());
                        m.ClientName = rdr["Name"].ToString();
                        m.RequestedBy = rdr["RequestedBy"].ToString();
                        m.ApprovedValue = rdr["Renda"] != null  && rdr["Renda"].ToString() != "" ? Double.Parse(rdr["Renda"].ToString()) : new Double();
                        m.ApprovedClickBW = rdr["ExcPreto"] != null && rdr["ExcPreto"].ToString() != "" ? Double.Parse(rdr["ExcPreto"].ToString()) : new Double();
                        m.ApprovedClickC = rdr["ExcCor"] != null && rdr["ExcCor"].ToString() != "" ? Double.Parse(rdr["ExcCor"].ToString()) : new Double();
                        m.Type = rdr["Modalidade"].ToString();
                        m.TypeID = rdr["TypeID"] != null ? (int)rdr["TypeID"] : 3;
                        m.Leasedesk = rdr["Leasedesk"].ToString();
                        m.QuoteNumber = rdr["QuoteNumber"].ToString(); 

                        history.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return history;
        }

        public List<ServiceValidationRequestHistoryEntry> GetServiceRequestHistory_OLD()
        {
            try
            {
                DateTime a = DateTime.Parse("2022-10-15 17:29:01.453");
                List<BB_Proposal_PrintingServiceValidationRequest> validationRequests = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.IsComplete == true && x.ToDelete == false && x.RequestedAt >= a).ToList();
                List<ServiceValidationRequestHistoryEntry> history = new List<ServiceValidationRequestHistoryEntry>();
                foreach (BB_Proposal_PrintingServiceValidationRequest validationRequest in validationRequests)
                {
                    string clientAccountNumber = db.BB_Proposal.Where(x => x.ID == validationRequest.BB_PrintingServices.BB_Proposal_PrintingServices2.ProposalID).Select(x => x.ClientAccountNumber).FirstOrDefault();
                    string clientName = db.BB_Clientes.Where(x => x.accountnumber == clientAccountNumber).Select(x => x.Name).FirstOrDefault();
                    ServiceValidationRequestHistoryEntry entry = new ServiceValidationRequestHistoryEntry
                    {
                        ID = validationRequest.ID,
                        ApprovedAt = (DateTime)validationRequest.ApprovedAt,
                        ClientName = clientName,
                        RequestedAt = (DateTime)validationRequest.RequestedAt,
                        RequestedBy = validationRequest.RequestedBy,
                        Fee = validationRequest.BB_PrintingServices.Fee.Value,
                    };
                    if (validationRequest.BB_PrintingServices.BB_VVA != null)
                    {
                        entry.Type = "Click Global - VVA";
                        entry.TypeID = 1;
                        entry.ApprovedValue = validationRequest.BB_PrintingServices.BB_VVA.PVP != null ? Math.Round(validationRequest.BB_PrintingServices.BB_VVA.PVP.Value, 2) : validationRequest.BB_PrintingServices.BB_VVA.PVP;
                        entry.ApprovedClickBW = validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP != null ? Math.Round(validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP.Value, 4) : validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP;
                        entry.ApprovedClickC = validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP != null ? Math.Round(validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP.Value, 4) : validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP;
                    }
                    else if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume != null)
                    {
                        entry.Type = "Click Global - Sem Volume Incluído";
                        entry.TypeID = 2;
                        entry.ApprovedValue = null;
                        entry.ApprovedClickBW = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW != null ? Math.Round(validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW.Value, 4) : validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW;
                        entry.ApprovedClickC = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC != null ? Math.Round(validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC.Value, 4) : validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC;
                    }
                    else
                    {
                        entry.Type = "Click Por Modelo - Sem Volume Incluído";
                        entry.TypeID = 3;
                        entry.ApprovedValue = null;
                        entry.ApprovedClickBW = null;
                        entry.ApprovedClickC = null;
                    }
                    history.Insert(0, entry);
                }
                return history;
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return null;
        }

        public ServiceRequestHistory GetServiceRequestHistoryById(int id)
        {
            try
            {
                BB_Proposal_PrintingServiceValidationRequest validationRequest = db.BB_Proposal_PrintingServiceValidationRequest
                .Include(x => x.BB_PrintingServices)
                .Where(vr => vr.ID == id && vr.ToDelete == false)
                .FirstOrDefault();
                if (validationRequest != null)
                {
                    var proposalID = validationRequest.BB_PrintingServices.BB_Proposal_PrintingServices2.ProposalID;
                    string clientAccountNumber = db.BB_Proposal.Where(x => x.ID == proposalID).Select(x => x.ClientAccountNumber).FirstOrDefault();
                    BB_Clientes client = db.BB_Clientes.Where(x => x.accountnumber == clientAccountNumber).FirstOrDefault();
                    string ClientAccountNumber = "";
                    string ClientName = "";
                    bool IsNewClient = false;
                    if (client != null)
                    {
                        ClientAccountNumber = client.accountnumber;
                        ClientName = client.Name;
                        if (client.accountnumber != null && client.accountnumber.ToLower().StartsWith("p"))
                        {
                            IsNewClient = true;
                        }
                    }
                    List<BB_PrintingService_Machines> historyEquipments = validationRequest.BB_PrintingServices.BB_PrintingService_Machines.ToList();
                    List<BB_Equipamentos> equipments = db.BB_Equipamentos.ToList();
                    if (validationRequest.BB_PrintingServices.BB_VVA != null)
                    {
                        VVAServiceRequestHistory srh = new VVAServiceRequestHistory()
                        {
                            ApprovedAt = validationRequest.ApprovedAt.GetValueOrDefault(),
                            ApprovedRent = validationRequest.BB_PrintingServices.BB_VVA.PVP.GetValueOrDefault(),
                            ApprovedExcessBW = validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP.GetValueOrDefault(),
                            ApprovedExcessC = validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP.GetValueOrDefault(),
                            AverageCostBW = 0,
                            AverageCostC = 0,
                            BWVolume = validationRequest.BB_PrintingServices.BWVolume.GetValueOrDefault(),
                            ClientAccountNumber = ClientAccountNumber,
                            ClientName = ClientName,
                            CVolume = validationRequest.BB_PrintingServices.CVolume.GetValueOrDefault(),
                            ContractDuration = validationRequest.BB_PrintingServices.ContractDuration.GetValueOrDefault(),
                            Equipments = new List<ServiceValidationRequestEquipment>(),
                            ExcessBillingFrequency = validationRequest.BB_PrintingServices.BB_VVA.ExcessBillingFrequency.GetValueOrDefault(),
                            ExcessBWPVP = 0,
                            ExcessCPVP = 0,
                            Fee = validationRequest.BB_PrintingServices.Fee.GetValueOrDefault(),
                            RentBillingFrequency = validationRequest.BB_PrintingServices.BB_VVA.RentBillingFrequency.GetValueOrDefault(),
                            ID = validationRequest.ID,
                            IsNewClient = IsNewClient,
                            RequestedAt = validationRequest.RequestedAt.GetValueOrDefault(),
                            RequestedBy = validationRequest.RequestedBy,
                            SEObservations = validationRequest.SEObservations,
                            SCObservations = validationRequest.SCObservations,
                            PVP = 0,
                            TotalCost = 0,
                            Type = "Click Global - VVA",
                        };
                        var query = from pe in historyEquipments
                                    join e in equipments on pe.CodeRef equals e.CodeRef
                                    select new ServiceValidationRequestEquipment
                                    {
                                        CodeRef = e.CodeRef,
                                        Description = e.Description != null ? e.Description : pe.Description,
                                        Quantity = pe.Quantity.GetValueOrDefault(),
                                        RecBWVolume = e.RecBWVolume,
                                        BWBaseCost = e.BWBaseCost,
                                        ApprovedBW = pe.ApprovedBW,
                                        ApprovedC = pe.ApprovedC,
                                        RecCVolume = e.RecCVolume,
                                        CBaseCost = e.CBaseCost,
                                        ClickPriceBW = pe.BWPVP,
                                        ClickPriceC = pe.CPVP,
                                        BWCoefficient = srh.BWVolume > 0 ? (pe.BWVolume / srh.BWVolume) * 100 : 0,
                                        CCoefficient = srh.CVolume > 0 ? (pe.CVolume / srh.CVolume) * 100 : 0,
                                        BWPages = pe.BWVolume,
                                        CPages = pe.CVolume,
                                        Type = e.PHC5 == null ? "" : (e.PHC5.Contains("(MF)") ? "MFP" : "Printer"),
                                    };
                        srh.Equipments = query.ToList();
                        srh.AverageCostBW = (double)(srh.Equipments.Sum(x => x.BWBaseCost * x.BWPages) / srh.Equipments.Sum(x => x.BWPages));
                        srh.AverageCostC = (double)(srh.Equipments.Where(x => x.CBaseCost != 0).Sum(x => x.CBaseCost * x.CPages) / srh.Equipments.Sum(x => x.CPages));
                        double bwCost = (double)(srh.Equipments.Sum(x => x.BWBaseCost * x.BWPages));
                        double cCost = (double)(srh.Equipments.Where(x => x.ClickPriceC != 0).Sum(x => x.CBaseCost * x.CPages));
                        srh.TotalCost = bwCost + cCost;
                        List<BB_Proposal_OPSImplement> opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();
                        foreach (BB_Proposal_OPSImplement implementPack in opsImplement)
                        {
                            OPSImplement item = new OPSImplement
                            {
                                CodeRef = implementPack.CodeRef,
                                Description = implementPack.Description,
                                ID = implementPack.ID,
                                Name = implementPack.Name,
                                PVP = implementPack.PVP,
                                Quantity = implementPack.Quantity,
                                UnitDiscountPrice = implementPack.UnitDiscountPrice,
                            };
                            srh.OPSImplement.Add(item);
                        }
                        List<BB_Proposal_OPSManage> opsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).ToList();
                        foreach (BB_Proposal_OPSManage managePack in opsManage)
                        {
                            OPSManage item = new OPSManage
                            {
                                CodeRef = managePack.CodeRef,
                                Description = managePack.Description,
                                ID = managePack.ID,
                                Name = managePack.Name,
                                PVP = managePack.PVP,
                                Quantity = managePack.Quantity,
                                UnitDiscountPrice = managePack.UnitDiscountPrice,
                                TotalMonths = managePack.TotalMonths
                            };
                            srh.OPSManage.Add(item);
                        }
                        return srh;
                    }
                    else if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume != null)
                    {
                        NoVolumeServiceRequestHistory srh = new NoVolumeServiceRequestHistory()
                        {
                            ApprovedAt = validationRequest.ApprovedAt.GetValueOrDefault(),
                            ClickPriceBW = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW.GetValueOrDefault(),
                            ClickPriceC = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC.GetValueOrDefault(),
                            AverageCostBW = 0,
                            AverageCostC = 0,
                            BWVolume = validationRequest.BB_PrintingServices.BWVolume.GetValueOrDefault(),
                            ClientAccountNumber = ClientAccountNumber,
                            ClientName = ClientName,
                            CVolume = validationRequest.BB_PrintingServices.CVolume.GetValueOrDefault(),
                            ContractDuration = validationRequest.BB_PrintingServices.ContractDuration.GetValueOrDefault(),
                            Equipments = new List<ServiceValidationRequestEquipment>(),
                            Fee = validationRequest.BB_PrintingServices.Fee.GetValueOrDefault(),
                            ID = validationRequest.ID,
                            IsNewClient = IsNewClient,
                            RequestedAt = validationRequest.RequestedAt.GetValueOrDefault(),
                            RequestedBy = validationRequest.RequestedBy,
                            SEObservations = validationRequest.SEObservations,
                            SCObservations = validationRequest.SCObservations,
                            PageBillingFrequency = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.PageBillingFrequency.GetValueOrDefault(),
                            TotalCost = 0,
                            Type = "Click Global - Sem Volume Incluído",
                        };
                        var query = from pe in historyEquipments
                                    join e in equipments on pe.CodeRef equals e.CodeRef
                                    select new ServiceValidationRequestEquipment
                                    {
                                        CodeRef = e.CodeRef,
                                        Description = e.Description != null ? e.Description : pe.Description,
                                        Quantity = pe.Quantity.GetValueOrDefault(),
                                        RecBWVolume = e.RecBWVolume,
                                        BWBaseCost = e.BWBaseCost,
                                        ApprovedBW = pe.ApprovedBW,
                                        ApprovedC = pe.ApprovedC,
                                        RecCVolume = e.RecCVolume,
                                        CBaseCost = e.CBaseCost,
                                        ClickPriceBW = pe.BWPVP,
                                        ClickPriceC = pe.CPVP,
                                        BWCoefficient = srh.BWVolume > 0 ? (pe.BWVolume / srh.BWVolume) * 100 : 0,
                                        CCoefficient = srh.CVolume > 0 ? (pe.CVolume / srh.CVolume) * 100 : 0,
                                        BWPages = pe.BWVolume,
                                        CPages = pe.CVolume,
                                        Type = e.PHC5 == null ? "" : (e.PHC5.Contains("(MF)") ? "MFP" : "Printer"),
                                    };
                        srh.Equipments = query.ToList();
                        srh.AverageCostBW = (double)(srh.Equipments.Sum(x => x.BWBaseCost * x.BWPages) / srh.Equipments.Sum(x => x.BWPages));
                        srh.AverageCostC = srh.CVolume > 0 ? (double)(srh.Equipments.Where(x => x.CBaseCost != 0).Sum(x => x.CBaseCost * x.CPages) / srh.Equipments.Sum(x => x.CPages)) : 0;
                        double bwCost = (double)(srh.Equipments.Sum(x => x.BWBaseCost * x.BWPages));
                        double cCost = (double)(srh.Equipments.Where(x => x.ClickPriceC != 0).Sum(x => x.CBaseCost * x.CPages));
                        srh.TotalCost = bwCost + cCost;
                        List<BB_Proposal_OPSImplement> opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();
                        foreach (BB_Proposal_OPSImplement implementPack in opsImplement)
                        {
                            OPSImplement item = new OPSImplement
                            {
                                CodeRef = implementPack.CodeRef,
                                Description = implementPack.Description,
                                ID = implementPack.ID,
                                Name = implementPack.Name,
                                PVP = implementPack.PVP,
                                Quantity = implementPack.Quantity,
                                UnitDiscountPrice = implementPack.UnitDiscountPrice,
                            };
                            srh.OPSImplement.Add(item);
                        }
                        List<BB_Proposal_OPSManage> opsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).ToList();
                        foreach (BB_Proposal_OPSManage managePack in opsManage)
                        {
                            OPSManage item = new OPSManage
                            {
                                CodeRef = managePack.CodeRef,
                                Description = managePack.Description,
                                ID = managePack.ID,
                                Name = managePack.Name,
                                PVP = managePack.PVP,
                                Quantity = managePack.Quantity,
                                UnitDiscountPrice = managePack.UnitDiscountPrice,
                                TotalMonths = managePack.TotalMonths
                            };
                            srh.OPSManage.Add(item);
                        }
                        return srh;
                    }
                    else
                    {
                        ClickPerModelServiceRequestHistory srh = new ClickPerModelServiceRequestHistory()
                        {
                            ApprovedAt = validationRequest.ApprovedAt.GetValueOrDefault(),
                            BWVolume = validationRequest.BB_PrintingServices.BWVolume.GetValueOrDefault(),
                            ClientAccountNumber = ClientAccountNumber,
                            ClientName = ClientName,
                            CVolume = validationRequest.BB_PrintingServices.CVolume.GetValueOrDefault(),
                            ContractDuration = validationRequest.BB_PrintingServices.ContractDuration.GetValueOrDefault(),
                            Equipments = new List<ServiceValidationRequestEquipment>(),
                            Fee = validationRequest.BB_PrintingServices.Fee.GetValueOrDefault(),
                            ID = validationRequest.ID,
                            IsNewClient = IsNewClient,
                            RequestedAt = validationRequest.RequestedAt.GetValueOrDefault(),
                            RequestedBy = validationRequest.RequestedBy,
                            SEObservations = validationRequest.SEObservations,
                            SCObservations = validationRequest.SCObservations,
                            PageBillingFrequency = validationRequest.BB_PrintingServices.BB_PrintingServices_ClickPerModel.PageBillingFrequency.GetValueOrDefault(),
                            Type = "Click Por Modelo - Sem Volume Incluído",
                        };
                        var query = from pe in historyEquipments
                                    join e in equipments on pe.CodeRef equals e.CodeRef
                                    select new ServiceValidationRequestEquipment
                                    {
                                        CodeRef = e.CodeRef,
                                        Description = e.Description != null ? e.Description : pe.Description,
                                        Quantity = pe.Quantity.GetValueOrDefault(),
                                        RecBWVolume = e.RecBWVolume,
                                        BWBaseCost = e.BWBaseCost,
                                        ApprovedBW = pe.ApprovedBW,
                                        ApprovedC = pe.ApprovedC,
                                        RecCVolume = e.RecCVolume,
                                        CBaseCost = e.CBaseCost,
                                        ClickPriceBW = pe.BWPVP,
                                        ClickPriceC = pe.CPVP,
                                        RequestedBWClickPrice = pe.RequestedBWClickPrice,
                                        RequestedCClickPrice = pe.RequestedCClickPrice,
                                        BWCoefficient = srh.BWVolume > 0 ? (pe.BWVolume / srh.BWVolume) * 100 : 0,
                                        CCoefficient = srh.CVolume > 0 ? (pe.CVolume / srh.CVolume) * 100 : 0,
                                        BWPages = pe.BWVolume,
                                        CPages = pe.CVolume,
                                        Type = e.PHC5 == null ? "" : (e.PHC5.Contains("(MF)") ? "MFP" : "Printer"),
                                    };
                        srh.Equipments = query.ToList();
                        List<BB_Proposal_OPSImplement> opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();
                        foreach (BB_Proposal_OPSImplement implementPack in opsImplement)
                        {
                            OPSImplement item = new OPSImplement
                            {
                                CodeRef = implementPack.CodeRef,
                                Description = implementPack.Description,
                                ID = implementPack.ID,
                                Name = implementPack.Name,
                                PVP = implementPack.PVP,
                                Quantity = implementPack.Quantity,
                                UnitDiscountPrice = implementPack.UnitDiscountPrice,
                            };
                            srh.OPSImplement.Add(item);
                        }
                        List<BB_Proposal_OPSManage> opsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).ToList();
                        foreach (BB_Proposal_OPSManage managePack in opsManage)
                        {
                            OPSManage item = new OPSManage
                            {
                                CodeRef = managePack.CodeRef,
                                Description = managePack.Description,
                                ID = managePack.ID,
                                Name = managePack.Name,
                                PVP = managePack.PVP,
                                Quantity = managePack.Quantity,
                                UnitDiscountPrice = managePack.UnitDiscountPrice,
                                TotalMonths = managePack.TotalMonths
                            };
                            srh.OPSManage.Add(item);
                        }
                        return srh;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            return null;
        }

        public ServiceValidationRequest GetServiceRequestById(int id)
        {
            BB_Proposal_PrintingServiceValidationRequest validationRequest = db.BB_Proposal_PrintingServiceValidationRequest
                .Include(x => x.BB_PrintingServices)
                .Where(vr => vr.ID == id)
                .FirstOrDefault();
            if (validationRequest != null)
            {
                string clientAccountNumber = db.BB_Proposal.Where(x => x.ID == validationRequest.BB_PrintingServices.BB_Proposal_PrintingServices2.ProposalID).Select(x => x.ClientAccountNumber).FirstOrDefault();
                BB_Clientes client = db.BB_Clientes.Where(x => x.accountnumber == clientAccountNumber).FirstOrDefault();
                SVRClient svrClient = new SVRClient();
                if (client != null)
                {
                    svrClient.ClientAccountNumber = client.accountnumber;
                    svrClient.ClientName = client.Name;
                    svrClient.IsNewClient = false;
                    if (client.accountnumber != null && client.accountnumber.ToLower().StartsWith("p"))
                    {
                        svrClient.IsNewClient = true;
                    }
                }
                int? proposalID = validationRequest.BB_PrintingServices.BB_Proposal_PrintingServices2.ProposalID;
                List<BB_Proposal_Quote> proposalEquipments = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID && x.TCP != null).ToList();
                List<BB_Equipamentos> equipments = db.BB_Equipamentos.ToList();
                ServiceValidationRequest svr = new ServiceValidationRequest();
                if (validationRequest.BB_PrintingServices.BB_VVA != null)
                {
                    svr = ProcessVVAServiceRequest(validationRequest, svrClient, proposalEquipments, equipments);
                }
                else if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume != null)
                {
                    svr = ProcessGlobalClickNoVolumeServiceRequest(validationRequest, svrClient, proposalEquipments, equipments);
                }
                else
                {
                    svr = ProcessClickPerModelServiceRequest(validationRequest, svrClient, proposalEquipments, equipments);
                }
                List<BB_Proposal_OPSImplement> opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();
                foreach (BB_Proposal_OPSImplement implementPack in opsImplement)
                {
                    OPSImplement item = new OPSImplement
                    {
                        CodeRef = implementPack.CodeRef,
                        Description = implementPack.Description,
                        ID = implementPack.ID,
                        Name = implementPack.Name,
                        PVP = implementPack.PVP,
                        Quantity = implementPack.Quantity,
                        UnitDiscountPrice = implementPack.UnitDiscountPrice,
                    };
                    svr.OPSImplement.Add(item);
                }
                List<BB_Proposal_OPSManage> opsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).ToList();
                foreach (BB_Proposal_OPSManage managePack in opsManage)
                {
                    OPSManage item = new OPSManage
                    {
                        CodeRef = managePack.CodeRef,
                        Description = managePack.Description,
                        ID = managePack.ID,
                        Name = managePack.Name,
                        PVP = managePack.PVP,
                        Quantity = managePack.Quantity,
                        UnitDiscountPrice = managePack.UnitDiscountPrice,
                        TotalMonths = managePack.TotalMonths
                    };
                    svr.OPSManage.Add(item);
                }
                return svr;
            }
            return null;
        }

        public async System.Threading.Tasks.Task ProcessVVAServiceValidationReplyAsync(VVAServiceValidationReply svr)
        {
            try
            {
                BB_Proposal_PrintingServiceValidationRequest validationRequest = db.BB_Proposal_PrintingServiceValidationRequest
                .Include(x => x.BB_PrintingServices)
                .Where(vr => vr.ID == svr.RequestID)
                .FirstOrDefault();
                if (validationRequest != null && validationRequest.IsComplete == false)
                {
                    string approvedBy;
                    using (var masterEntities = new masterEntities())
                    {
                        approvedBy = masterEntities.AspNetUsers.Where(x => x.DisplayName == svr.ModifiedBy).Select(x => x.Email).FirstOrDefault();
                    }

                    validationRequest.ApprovedBy = approvedBy;
                    validationRequest.IsApproved = true;
                    validationRequest.IsComplete = true;
                    validationRequest.SCObservations = svr.SCObservations;
                    validationRequest.BB_PrintingServices.BB_VVA.PVP = svr.ApprovedRent;
                    validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP = svr.ApprovedExcessClickBW;
                    validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP = svr.ApprovedExcessClickC;
                    validationRequest.BB_PrintingServices.Fee = svr.Fee;
                    validationRequest.ApprovedAt = DateTime.Now;
                    validationRequest.IsApproved = svr.isApproved;


                    foreach (OPSImplement implementPack in svr.OPSImplement)
                    {
                        var toEdit = db.BB_Proposal_OPSImplement.Where(x => x.ID == implementPack.ID).FirstOrDefault();
                        if (toEdit != null)
                        {
                            toEdit.UnitDiscountPrice = implementPack.UnitDiscountPrice;
                            toEdit.IsValidated = true;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }
                    foreach (OPSManage managePack in svr.OPSManage)
                    {
                        var toEdit = db.BB_Proposal_OPSManage.Where(x => x.ID == managePack.ID).FirstOrDefault();
                        if (toEdit != null)
                        {
                            toEdit.UnitDiscountPrice = managePack.UnitDiscountPrice;
                            toEdit.IsValidated = true;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }
                    BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2.Where(x => x.ID == validationRequest.BB_PrintingServices.PrintingServices2ID).FirstOrDefault();
                    if (ps2 != null)
                    {
                        if (ps2.ActivePrintingService == null)
                        {
                            ps2.ActivePrintingService = 1;
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    if (validationRequest.BB_PrintingServices.BB_PrintingService_Machines.Count > 0)
                    {
                        validationRequest.BB_PrintingServices.BB_PrintingService_Machines.Clear();
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    foreach (ServiceValidationRequestEquipment equipment in svr.Equipments)
                    {
                        BB_PrintingService_Machines machine = new BB_PrintingService_Machines()
                        {
                            BWVolume = equipment.BWPages,
                            CodeRef = equipment.CodeRef,
                            CVolume = equipment.CPages,
                            Description = equipment.Description,
                            PrintingServiceID = validationRequest.PrintingServiceID,
                            Quantity = equipment.Quantity,
                            ApprovedBW = equipment.ApprovedBW,
                            ApprovedC = equipment.ApprovedC,
                            BWCost = equipment.BWBaseCost,
                            CCost = equipment.CBaseCost,
                            BWPVP = equipment.ClickPriceBW,
                            CPVP = equipment.ClickPriceC,
                            IsInClient = equipment.IsInClient,
                            IsUsed = equipment.IsUsed,
                        };
                        db.BB_PrintingService_Machines.Add(machine);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    EmailService emailSend = new EmailService();
                    EmailMesage message = new EmailMesage();

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ps2.ProposalID).FirstOrDefault();
                    string clientName = "";
                    if (proposal != null && proposal.ClientAccountNumber != null)
                    {
                        clientName = db.BB_Clientes.Where(x => x.accountnumber == proposal.ClientAccountNumber).Select(x => x.Name).FirstOrDefault();
                    }

                    message.Destination = proposal.CreatedBy;
                    if (svr.isApproved)
                    {
                        message.Subject = "BB - APROVACAO de Serviço - " + clientName;
                    }
                    else
                    {
                        message.Subject = "BB - REPROVACAO de Serviço - " + clientName;
                    }
                    StringBuilder strBuilder = new StringBuilder();
                    string tableStyle = "border: 1px solid #ddd; border-collapse: collapse";
                    string thStyle = "background-color: #245982;text-align: center;color: white;border: 1px solid #ddd; border-collapse: collapse";

                    
                    strBuilder.Append("Caro(a) Gestor(a),<br/>");
                    if (svr.isApproved == true)
                    {
                        strBuilder.Append("O seguinte pedido de <b>Aprovação de Serviço</b> foi <b>APROVADO</b><br/>");
                    }
                    else
                    {
                        strBuilder.Append("O seguinte pedido de <b>Aprovação de Serviço</b> foi <b>REJEITADO</b><br/>");
                    }
                   
                    strBuilder.Append("<br/>");

                    if (proposal.Name != null) strBuilder.Append("<b>Oportunidade</b>: " + proposal.Name + "<br/>");
                    if (proposal.CRM_QUOTE_ID != null) strBuilder.Append("<b>Quote CRM</b>: " + proposal.CRM_QUOTE_ID + "<br/>");
                    strBuilder.Append("<b>ID Interno</b>: " + proposal.ID + "<br/>");
                    if (clientName != null) strBuilder.Append("<b>Cliente</b>: " + clientName + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Modalidade de Serviço</b>: VVA<br/>");
                    if (validationRequest.BB_PrintingServices.ContractDuration != null) strBuilder.Append("<b>Duração de Contrato</b>: " + validationRequest.BB_PrintingServices.ContractDuration + " Meses<br/>");
                    if (validationRequest.BB_PrintingServices.BWVolume != null) strBuilder.Append("<b>Volume Key</b>: " + validationRequest.BB_PrintingServices.BWVolume + "<br/>");
                    if (validationRequest.BB_PrintingServices.CVolume != null) strBuilder.Append("<b>Volume Cor</b>: " + validationRequest.BB_PrintingServices.CVolume + "<br/>");
                    strBuilder.Append("<br/>");
                    if (validationRequest.BB_PrintingServices.BB_VVA.PVP != null) strBuilder.Append("<b>Renda Aprovada</b>: " + validationRequest.BB_PrintingServices.BB_VVA.PVP + " €<br/>");
                    if (validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP != null) strBuilder.Append("<b>Excedente Key</b>: " + validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP + " €<br/>");
                    if (validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP != null) strBuilder.Append("<b>Excedente Cor</b>: " + validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP + " €<br/>");
                    if (validationRequest.BB_PrintingServices.Fee != null) strBuilder.Append("<b>Acréscimo de Renda Cor</b>: " + validationRequest.BB_PrintingServices.Fee + " €<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<table style = '" + tableStyle + "'><tr><th style='" + thStyle + "'>Equipamento</th><th style='" + thStyle + "'>Quantidade</th><th style='" + thStyle + "'>Páginas Key</th><th style='" + thStyle + "'>Páginas Cor</th></tr>");
                    foreach (ServiceValidationRequestEquipment equipment in svr.Equipments)
                    {
                        strBuilder.Append("<tr><td style = '" + tableStyle + "'>" + equipment.Description + "</td><td style = '" + tableStyle + "'>" + equipment.Quantity + "</td><td style = '" + tableStyle + "'>" + equipment.BWPages + "</td><td style = '" + tableStyle + "'>" + equipment.CPages + "</td></tr>");
                    }
                    strBuilder.Append("</table>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Observações</b>: " + validationRequest.SCObservations + "<br/>");
                    strBuilder.Append("<br/>");
                    if (validationRequest.RequestedAt != null) strBuilder.Append("<b>Data de Pedido</b>: " + validationRequest.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    if (validationRequest.ApprovedAt != null) strBuilder.Append("<b>Data de Resposta</b>: " + validationRequest.ApprovedAt.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("Para aceder à aplicação, utilize o seguinte link: " + "https://bb.konicaminolta.pt" + "<br/>");
                    strBuilder.Append("Por favor, não responder a este email." + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                    message.Body = strBuilder.ToString();

                    await emailSend.SendEmailaync(message);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
        public async System.Threading.Tasks.Task ProcessGlobalClickNoVolumeServiceValidationReplyAsync(GlobalClickNoVolumeServiceValidationReply svr)
        {
            try
            {
                BB_Proposal_PrintingServiceValidationRequest validationRequest = db.BB_Proposal_PrintingServiceValidationRequest
                .Include(x => x.BB_PrintingServices)
                .Where(vr => vr.ID == svr.RequestID)
                .FirstOrDefault();
                if (validationRequest != null && validationRequest.IsComplete == false)
                {
                    string approvedBy;
                    using (var masterEntities = new masterEntities())
                    {
                        approvedBy = masterEntities.AspNetUsers.Where(x => x.DisplayName == svr.ModifiedBy).Select(x => x.Email).FirstOrDefault();
                    }

                    validationRequest.ApprovedBy = approvedBy;
                    validationRequest.IsApproved = svr.isApproved;
                    validationRequest.IsComplete = true;
                    validationRequest.SCObservations = svr.SCObservations;
                    validationRequest.BB_PrintingServices.Fee = svr.Fee;
                    validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW = svr.ApprovedClickPriceBW;
                    validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC = svr.ApprovedClickPriceC;
                    validationRequest.ApprovedAt = DateTime.Now;

                    foreach (OPSImplement implementPack in svr.OPSImplement)
                    {
                        var toEdit = db.BB_Proposal_OPSImplement.Where(x => x.ID == implementPack.ID).FirstOrDefault();
                        if (toEdit != null)
                        {
                            toEdit.UnitDiscountPrice = implementPack.UnitDiscountPrice;
                            toEdit.IsValidated = true;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }
                    foreach (OPSManage managePack in svr.OPSManage)
                    {
                        var toEdit = db.BB_Proposal_OPSManage.Where(x => x.ID == managePack.ID).FirstOrDefault();
                        if (toEdit != null)
                        {
                            toEdit.UnitDiscountPrice = managePack.UnitDiscountPrice;
                            toEdit.IsValidated = true;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }

                    BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2.Where(x => x.ID == validationRequest.BB_PrintingServices.PrintingServices2ID).FirstOrDefault();
                    if (ps2 != null)
                    {
                        if (ps2.ActivePrintingService == null)
                        {
                            ps2.ActivePrintingService = 1;
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    if (validationRequest.BB_PrintingServices.BB_PrintingService_Machines.Count > 0)
                    {
                        validationRequest.BB_PrintingServices.BB_PrintingService_Machines.Clear();
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    foreach (ServiceValidationRequestEquipment equipment in svr.Equipments)
                    {
                        BB_PrintingService_Machines machine = new BB_PrintingService_Machines()
                        {
                            BWVolume = equipment.BWPages,
                            CodeRef = equipment.CodeRef,
                            CVolume = equipment.CPages,

                            Description = equipment.Description,
                            PrintingServiceID = validationRequest.PrintingServiceID,
                            Quantity = equipment.Quantity,
                            IsInClient = equipment.IsInClient,
                            IsUsed = equipment.IsUsed,
                            ApprovedBW = equipment.ApprovedBW,
                            ApprovedC = equipment.ApprovedC,
                            BWCost = equipment.BWBaseCost,
                            CCost = equipment.CBaseCost,
                            BWPVP = equipment.ClickPriceBW,
                            CPVP = equipment.ClickPriceC,
                        };
                        db.BB_PrintingService_Machines.Add(machine);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    EmailService emailSend = new EmailService();
                    EmailMesage message = new EmailMesage();

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ps2.ProposalID).FirstOrDefault();
                    string clientName = "";
                    if (proposal != null && proposal.ClientAccountNumber != null)
                    {
                        clientName = db.BB_Clientes.Where(x => x.accountnumber == proposal.ClientAccountNumber).Select(x => x.Name).FirstOrDefault();
                    }

                    message.Destination = proposal.CreatedBy;
                    if (svr.isApproved)
                    {
                        message.Subject = "BB - APROVACAO de Serviço - " + clientName;
                    }
                    else
                    {
                        message.Subject = "BB - REPROVACAO de Serviço - " + clientName;
                    }
                       
                    StringBuilder strBuilder = new StringBuilder();
                    string tableStyle = "border: 1px solid #ddd; border-collapse: collapse";
                    string thStyle = "background-color: #245982;text-align: center;color: white;border: 1px solid #ddd; border-collapse: collapse";

                    strBuilder.Append("Caro(a) Gestor(a),<br/>");
                    if(svr.isApproved)
                    {
                        strBuilder.Append("O seguinte pedido de <b>Aprovação de Serviço</b> foi <b>APROVADO</b><br/>");
                    }
                    else
                    {
                        strBuilder.Append("O seguinte pedido de <b>Aprovação de Serviço</b> foi <b>REPROVADO</b><br/>");
                    }
                    
                    strBuilder.Append("<br/>");

                    if (proposal.Name != null) strBuilder.Append("<b>Oportunidade</b>: " + proposal.Name + "<br/>");
                    if (proposal.CRM_QUOTE_ID != null) strBuilder.Append("<b>Quote CRM</b>: " + proposal.CRM_QUOTE_ID + "<br/>");
                    strBuilder.Append("<b>ID Interno</b>: " + proposal.ID + "<br/>");
                    if (clientName != null) strBuilder.Append("<b>Cliente</b>: " + clientName + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Modalidade de Serviço</b>: Click Global - Sem Volume Incluído<br/>");
                    if (validationRequest.BB_PrintingServices.ContractDuration != null) strBuilder.Append("<b>Duração de Contrato</b>: " + validationRequest.BB_PrintingServices.ContractDuration + " Meses<br/>");
                    if (validationRequest.BB_PrintingServices.BWVolume != null) strBuilder.Append("<b>Volume Key</b>: " + validationRequest.BB_PrintingServices.BWVolume + "<br/>");
                    if (validationRequest.BB_PrintingServices.CVolume != null) strBuilder.Append("<b>Volume Cor</b>: " + validationRequest.BB_PrintingServices.CVolume + "<br/>");
                    strBuilder.Append("<br/>");
                    if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW != null) strBuilder.Append("<b>Excedente Key</b>: " + validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW + " €<br/>");
                    if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC != null) strBuilder.Append("<b>Excedente Cor</b>: " + validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC + " €<br/>");
                    if (validationRequest.BB_PrintingServices.Fee != null) strBuilder.Append("<b>Acréscimo de Renda Cor</b>: " + validationRequest.BB_PrintingServices.Fee + " €<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<table style = '" + tableStyle + "'><tr><th style='" + thStyle + "'>Equipamento</th><th style='" + thStyle + "'>Quantidade</th><th style='" + thStyle + "'>Páginas Key</th><th style='" + thStyle + "'>Páginas Cor</th></tr>");
                    foreach (ServiceValidationRequestEquipment equipment in svr.Equipments)
                    {
                        strBuilder.Append("<tr><td style = '" + tableStyle + "'>" + equipment.Description + "</td><td style = '" + tableStyle + "'>" + equipment.Quantity + "</td><td style = '" + tableStyle + "'>" + equipment.BWPages + "</td><td style = '" + tableStyle + "'>" + equipment.CPages + "</td></tr>");
                    }
                    strBuilder.Append("</table>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Observações</b>: " + validationRequest.SCObservations + "<br/>");
                    strBuilder.Append("<br/>");
                    if (validationRequest.RequestedAt != null) strBuilder.Append("<b>Data de Pedido</b>: " + validationRequest.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    if (validationRequest.ApprovedAt != null) strBuilder.Append("<b>Data de Resposta</b>: " + validationRequest.ApprovedAt.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("Para aceder à aplicação, utilize o seguinte link: " + "https://bb.konicaminolta.pt" + "<br/>");
                    strBuilder.Append("Por favor, não responder a este email." + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                    message.Body = strBuilder.ToString();

                    await emailSend.SendEmailaync(message);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        public async System.Threading.Tasks.Task ProcessClickPerModelServiceValidationReplyAsync(ServiceValidationReply svr)
        {
            try
            {
                BB_Proposal_PrintingServiceValidationRequest validationRequest = db.BB_Proposal_PrintingServiceValidationRequest
                .Include(x => x.BB_PrintingServices)
                .Where(vr => vr.ID == svr.RequestID)
                .FirstOrDefault();
                if (true || validationRequest != null && validationRequest.IsComplete == false)
                {
                    string approvedBy;
                    using (var masterEntities = new masterEntities())
                    {
                        approvedBy = masterEntities.AspNetUsers.Where(x => x.DisplayName == svr.ModifiedBy).Select(x => x.Email).FirstOrDefault();
                    }

                    validationRequest.ApprovedBy = approvedBy;
                    validationRequest.IsApproved = true;
                    validationRequest.IsComplete = true;
                    validationRequest.SCObservations = svr.SCObservations;
                    validationRequest.BB_PrintingServices.Fee = svr.Fee;
                    validationRequest.ApprovedAt = DateTime.Now;

                    foreach (OPSImplement implementPack in svr.OPSImplement)
                    {
                        var toEdit = db.BB_Proposal_OPSImplement.Where(x => x.ID == implementPack.ID).FirstOrDefault();
                        if (toEdit != null)
                        {
                            toEdit.UnitDiscountPrice = implementPack.UnitDiscountPrice;
                            toEdit.IsValidated = true;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }
                    foreach (OPSManage managePack in svr.OPSManage)
                    {
                        var toEdit = db.BB_Proposal_OPSManage.Where(x => x.ID == managePack.ID).FirstOrDefault();
                        if (toEdit != null)
                        {
                            toEdit.UnitDiscountPrice = managePack.UnitDiscountPrice;
                            toEdit.IsValidated = true;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }

                    BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2.Where(x => x.ID == validationRequest.BB_PrintingServices.PrintingServices2ID).FirstOrDefault();
                    if (ps2 != null)
                    {
                        if (ps2.ActivePrintingService == null)
                        {
                            ps2.ActivePrintingService = 1;
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    foreach (ServiceValidationRequestEquipment equipment in svr.Equipments)
                    {
                        BB_PrintingService_Machines machine = db.BB_PrintingService_Machines.Where(x => x.ID == equipment.ID).FirstOrDefault();
                        machine.ApprovedBW = equipment.ApprovedBW;
                        machine.ApprovedC = equipment.ApprovedC;
                        machine.BWVolume = equipment.BWPages;
                        machine.CVolume = equipment.CPages;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    EmailService emailSend = new EmailService();
                    EmailMesage message = new EmailMesage();

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ps2.ProposalID).FirstOrDefault();
                    string clientName = "";
                    if (proposal != null && proposal.ClientAccountNumber != null)
                    {
                        clientName = db.BB_Clientes.Where(x => x.accountnumber == proposal.ClientAccountNumber).Select(x => x.Name).FirstOrDefault();
                    }

                    message.Destination = proposal.CreatedBy;
                    message.Subject = "BB - Aprovação de Serviço - " + clientName;
                    StringBuilder strBuilder = new StringBuilder();
                    string tableStyle = "border: 1px solid #ddd; border-collapse: collapse";
                    string thStyle = "background-color: #245982;text-align: center;color: white;border: 1px solid #ddd; border-collapse: collapse";

                    strBuilder.Append("Caro(a) Gestor(a),<br/>");
                    strBuilder.Append("O seguinte pedido de <b>Aprovação de Serviço</b> foi <b>Aprovado</b><br/>");
                    strBuilder.Append("<br/>");

                    if (proposal.Name != null) strBuilder.Append("<b>Oportunidade</b>: " + proposal.Name + "<br/>");
                    if (proposal.CRM_QUOTE_ID != null) strBuilder.Append("<b>Quote CRM</b>: " + proposal.CRM_QUOTE_ID + "<br/>");
                    strBuilder.Append("<b>ID Interno</b>: " + proposal.ID + "<br/>");
                    if (clientName != null) strBuilder.Append("<b>Cliente</b>: " + clientName + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Modalidade de Serviço</b>: Click Global - Sem Volume Incluído<br/>");
                    if (validationRequest.BB_PrintingServices.ContractDuration != null) strBuilder.Append("<b>Duração de Contrato</b>: " + validationRequest.BB_PrintingServices.ContractDuration + " Meses<br/>");
                    if (validationRequest.BB_PrintingServices.BWVolume != null) strBuilder.Append("<b>Volume Key</b>: " + validationRequest.BB_PrintingServices.BWVolume + "<br/>");
                    if (validationRequest.BB_PrintingServices.CVolume != null) strBuilder.Append("<b>Volume Cor</b>: " + validationRequest.BB_PrintingServices.CVolume + "<br/>");
                    strBuilder.Append("<br/>");
                    if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW != null) strBuilder.Append("<b>Excedente Key</b>: " + validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW + " €<br/>");
                    if (validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC != null) strBuilder.Append("<b>Excedente Cor</b>: " + validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC + " €<br/>");
                    if (validationRequest.BB_PrintingServices.Fee != null) strBuilder.Append("<b>Acréscimo de Renda Cor</b>: " + validationRequest.BB_PrintingServices.Fee + " €<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<table style = '" + tableStyle + "'><tr><th style='" + thStyle + "'>Equipamento</th><th style='" + thStyle + "'>Quantidade</th><th style='" + thStyle + "'>Páginas Key</th><th style='" + thStyle + "'>Páginas Cor</th><th style='" + thStyle + "'>ClickPrice Key</th><th style='" + thStyle + "'>ClickPrice Cor</th></tr>");
                    foreach (ServiceValidationRequestEquipment equipment in svr.Equipments)
                    {
                        strBuilder.Append("<tr><td style = '" + tableStyle + "'>" + equipment.Description + "</td><td style = '" + tableStyle + "'>" + equipment.Quantity + "</td><td style = '" + tableStyle + "'>" + equipment.BWPages + "</td><td style = '" + tableStyle + "'>" + equipment.CPages + "</td><td style = '" + tableStyle + "'>" + equipment.ApprovedBW + "</td><td style = '" + tableStyle + "'>" + equipment.ApprovedC + "</td></tr>");
                    }
                    strBuilder.Append("</table>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Observações</b>: " + validationRequest.SCObservations + "<br/>");
                    strBuilder.Append("<br/>");
                    if (validationRequest.RequestedAt != null) strBuilder.Append("<b>Data de Pedido</b>: " + validationRequest.RequestedAt.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    if (validationRequest.ApprovedAt != null) strBuilder.Append("<b>Data de Resposta</b>: " + validationRequest.ApprovedAt.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("Para aceder à aplicação, utilize o seguinte link: " + "https://bb.konicaminolta.pt" + "<br/>");
                    strBuilder.Append("Por favor, não responder a este email." + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                    message.Body = strBuilder.ToString();

                    await emailSend.SendEmailaync(message);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        private VVAServiceValidationRequest ProcessVVAServiceRequest(BB_Proposal_PrintingServiceValidationRequest validationRequest, SVRClient client, List<BB_Proposal_Quote> proposalEquipments, List<BB_Equipamentos> equipments)
        {
            VVAServiceValidationRequest svr = new VVAServiceValidationRequest()
            {
                Client = client,
                Volumes = new SVRVolumes()
                {
                    BWVolume = validationRequest.BB_PrintingServices.BWVolume.GetValueOrDefault(),
                    CVolume = validationRequest.BB_PrintingServices.CVolume.GetValueOrDefault(),
                },
                ContractDuration = validationRequest.BB_PrintingServices.ContractDuration.GetValueOrDefault(),
                Equipments = new List<ServiceValidationRequestEquipment>(),
                ExcessBillingFrequency = validationRequest.BB_PrintingServices.BB_VVA.ExcessBillingFrequency.GetValueOrDefault(),
                RequestedExcessBWPVP = validationRequest.BB_PrintingServices.BB_VVA.RequestedBWExcess.GetValueOrDefault(),
                RequestedExcessCPVP = validationRequest.BB_PrintingServices.BB_VVA.RequestedCExcess.GetValueOrDefault(),
                RequestedPVP = validationRequest.BB_PrintingServices.BB_VVA.RequestedRent.GetValueOrDefault(),
                RentBillingFrequency = validationRequest.BB_PrintingServices.BB_VVA.RentBillingFrequency.GetValueOrDefault(),
                ID = validationRequest.ID,
                RequestedBy = validationRequest.RequestedBy,
                SEObservations = validationRequest.SEObservations,
                RecommendedExcessBWPVP = validationRequest.BB_PrintingServices.BB_VVA.BWExcessPVP.GetValueOrDefault(),
                RecommendedExcessCPVP = validationRequest.BB_PrintingServices.BB_VVA.CExcessPVP.GetValueOrDefault(),
                RecommendedPVP = validationRequest.BB_PrintingServices.BB_VVA.PVP.GetValueOrDefault(),
                Type = "Click Global - VVA",
            };
            List<ServiceValidationRequestEquipment> svrEquipments = DistributePages(proposalEquipments, equipments, svr);
            svr.AverageCostBW = (double)(svrEquipments.Sum(x => x.BWBaseCost * x.BWPages) / svrEquipments.Sum(x => x.BWPages));
            svr.AverageCostC = (double)(svrEquipments.Where(x => x.CBaseCost != 0).Sum(x => x.CBaseCost * x.CPages) / svrEquipments.Sum(x => x.CPages));
            svr.Equipments = svrEquipments;
            return svr;
        }

        public GlobalClickNoVolumeServiceValidationRequest ProcessGlobalClickNoVolumeServiceRequest(BB_Proposal_PrintingServiceValidationRequest validationRequest, SVRClient client, List<BB_Proposal_Quote> proposalEquipments, List<BB_Equipamentos> equipments)
        {
            GlobalClickNoVolumeServiceValidationRequest svr = new GlobalClickNoVolumeServiceValidationRequest()
            {
                Client = client,
                Volumes = new SVRVolumes()
                {
                    BWVolume = validationRequest.BB_PrintingServices.BWVolume.GetValueOrDefault(),
                    CVolume = validationRequest.BB_PrintingServices.CVolume.GetValueOrDefault(),
                },
                ContractDuration = validationRequest.BB_PrintingServices.ContractDuration.GetValueOrDefault(),
                Equipments = new List<ServiceValidationRequestEquipment>(),
                PageBillingFrequency = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.PageBillingFrequency.GetValueOrDefault(),
                RecommendedClickPriceBW = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickBW.GetValueOrDefault(),
                RecommendedClickPriceC = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.GlobalClickC.GetValueOrDefault(),
                RequestedClickPriceBW = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.RequestedGlobalClickBW.GetValueOrDefault(),
                RequestedClickPriceC = validationRequest.BB_PrintingServices.BB_PrintingServices_NoVolume.RequestedGlobalClickC.GetValueOrDefault(),
                AverageCostBW = 0,
                AverageCostC = 0,
                ID = validationRequest.ID,
                RequestedBy = validationRequest.RequestedBy,
                SEObservations = validationRequest.SEObservations,
                Type = "Click Global - Sem Volume Incluído",
            };
            List<ServiceValidationRequestEquipment> svrEquipments = DistributePages(proposalEquipments, equipments, svr);
            svr.AverageCostBW = (double)(svrEquipments.Sum(x => x.BWBaseCost * x.RecBWVolume) / svrEquipments.Sum(x => x.RecBWVolume));
            svr.AverageCostC = (double)(svrEquipments.Where(x => x.CBaseCost != 0).Sum(x => x.CBaseCost * x.RecCVolume) / svrEquipments.Sum(x => x.RecCVolume));
            svr.Equipments = svrEquipments;
            return svr;
        }

        public ClickPerModelServiceValidationRequest ProcessClickPerModelServiceRequest(BB_Proposal_PrintingServiceValidationRequest validationRequest, SVRClient client, List<BB_Proposal_Quote> proposalEquipments, List<BB_Equipamentos> equipments)
        {
            ClickPerModelServiceValidationRequest svr = new ClickPerModelServiceValidationRequest()
            {
                Client = client,
                Volumes = new SVRVolumes()
                {
                    BWVolume = validationRequest.BB_PrintingServices.BWVolume.GetValueOrDefault(),
                    CVolume = validationRequest.BB_PrintingServices.CVolume.GetValueOrDefault(),
                },
                ContractDuration = validationRequest.BB_PrintingServices.ContractDuration.GetValueOrDefault(),
                Equipments = new List<ServiceValidationRequestEquipment>(),
                PageBillingFrequency = validationRequest.BB_PrintingServices.BB_PrintingServices_ClickPerModel.PageBillingFrequency.GetValueOrDefault(),
                AverageCostBW = 0,
                AverageCostC = 0,
                ID = validationRequest.ID,
                RequestedBy = validationRequest.RequestedBy,
                SEObservations = validationRequest.SEObservations,
                Type = "Click Por Modelo - Sem Volume Incluído",
            };

            List<ServiceValidationRequestEquipment> svrEquipments = new List<ServiceValidationRequestEquipment>();
            var query = from m in validationRequest.BB_PrintingServices.BB_PrintingService_Machines
                        join e in equipments on m.CodeRef equals e.CodeRef
                        select new ServiceValidationRequestEquipment
                        {
                            ID = m.ID,
                            ApprovedBW = e.ClickPriceBW != null ? e.ClickPriceBW : equipments.Where(x => x.PHC1 == e.PHC1 && x.PHC4 == e.PHC4 && x.PHC5 == e.PHC5).Select(x => x.ClickPriceBW).Max() * 1.15,
                            ApprovedC = e.ClickPriceC != null ? e.ClickPriceC : equipments.Where(x => x.PHC1 == e.PHC1 && x.PHC4 == e.PHC4 && x.PHC5 == e.PHC5).Select(x => x.ClickPriceC).Max() * 1.15,
                            CodeRef = e.CodeRef,
                            Description = e.Description != null ? e.Description : m.Description,
                            Quantity = (int)m.Quantity,
                            RecBWVolume = e.RecBWVolume,
                            BWBaseCost = e.BWBaseCost,
                            RecCVolume = e.RecCVolume,
                            CBaseCost = e.CBaseCost,
                            ClickPriceBW = e.ClickPriceBW != null ? e.ClickPriceBW : equipments.Where(x => x.PHC1 == e.PHC1 && x.PHC4 == e.PHC4 && x.PHC5 == e.PHC5).Select(x => x.ClickPriceBW).Max() * 1.15,
                            ClickPriceC = e.ClickPriceC != null ? e.ClickPriceC : equipments.Where(x => x.PHC1 == e.PHC1 && x.PHC4 == e.PHC4 && x.PHC5 == e.PHC5).Select(x => x.ClickPriceC).Max() * 1.15,
                            BWCoefficient = 0,
                            CCoefficient = 0,
                            BWPages = 0,
                            CPages = 0,
                            RequestedBWClickPrice = m.RequestedBWClickPrice,
                            RequestedCClickPrice = m.RequestedCClickPrice,
                            Type = e.PHC5 == null ? "" : (e.PHC5.Contains("(MF)") ? "MFP" : "Printer"),
                            IsUsed = m.IsUsed.GetValueOrDefault(),
                            IsInClient = m.IsInClient.GetValueOrDefault()
                        };
            svrEquipments = query.ToList();
            int totalRecBW = (int)svrEquipments.Sum(x => x.RecBWVolume * x.Quantity);
            int totalRecC = (int)svrEquipments.Sum(x => x.RecCVolume * x.Quantity);
            int totalUsedBW = 0;
            int totalUsedC = 0;
            foreach (ServiceValidationRequestEquipment svre in svrEquipments)
            {
                if (svr.Volumes.BWVolume != 0 && svre.RecBWVolume != null)
                {
                    svre.BWCoefficient = ((double)svre.RecBWVolume / totalRecBW) * svre.Quantity * 100;
                    int toAddBW = (int)Math.Floor((double)(svr.Volumes.BWVolume * (svre.BWCoefficient / 100)));
                    svre.BWPages += toAddBW;
                    totalUsedBW += toAddBW;
                }
                if (svr.Volumes.CVolume != 0 && svre.RecCVolume != null)
                {
                    svre.CCoefficient = ((double)svre.RecCVolume / totalRecC) * svre.Quantity * 100;
                    int toAddC = (int)Math.Floor((double)(svr.Volumes.CVolume * (svre.CCoefficient / 100)));
                    svre.CPages += toAddC;
                    totalUsedC += toAddC;
                }
            }

            int toUseBW = svr.Volumes.BWVolume - totalUsedBW;
            int toUseC = svr.Volumes.CVolume - totalUsedC;
            foreach (ServiceValidationRequestEquipment svre in svrEquipments)
            {
                if (totalUsedBW == totalRecBW && totalUsedC == totalRecC)
                {
                    break;
                }
                if (svr.Volumes.BWVolume != 0 && svre.RecBWVolume != null && totalUsedBW != totalRecBW)
                {
                    int toAddBW = (int)Math.Round((double)(toUseBW * (svre.BWCoefficient / 100)));
                    svre.BWPages += toAddBW;
                    totalUsedBW += toAddBW;
                }
                if (svr.Volumes.BWVolume != 0 && svre.RecBWVolume != null && totalUsedC != totalRecC)
                {
                    int toAddC = (int)Math.Round((double)(toUseC * (svre.CCoefficient / 100)));
                    svre.CPages += toAddC;
                    totalUsedC += toAddC;
                }
            }
            svr.AverageCostBW = (double)(svrEquipments.Sum(x => x.BWBaseCost * x.RecBWVolume) / svrEquipments.Sum(x => x.RecBWVolume));
            svr.AverageCostC = (double)(svrEquipments.Where(x => x.CBaseCost != 0).Sum(x => x.CBaseCost * x.RecCVolume) / svrEquipments.Sum(x => x.RecCVolume));
            svr.Equipments = svrEquipments;
            return svr;
        }

        private List<ServiceValidationRequestEquipment> DistributePages(List<BB_Proposal_Quote> proposalEquipments, List<BB_Equipamentos> equipments, ServiceValidationRequest svr)
        {
            List<ServiceValidationRequestEquipment> svrEquipments = new List<ServiceValidationRequestEquipment>();
            var query = from pe in proposalEquipments
                        join e in equipments on pe.CodeRef equals e.CodeRef
                        select new ServiceValidationRequestEquipment
                        {
                            CodeRef = e.CodeRef,
                            Description = e.Description != null ? e.Description : pe.Description,
                            Quantity = (int)pe.Qty,
                            RecBWVolume = e.RecBWVolume,
                            BWBaseCost = e.BWBaseCost,
                            RecCVolume = e.RecCVolume,
                            CBaseCost = e.CBaseCost,
                            ClickPriceBW = e.ClickPriceBW != null ? e.ClickPriceBW : equipments.Where(x => x.PHC1 == e.PHC1 && x.PHC4 == e.PHC4 && x.PHC5 == e.PHC5).Select(x => x.ClickPriceBW).Max() * 1.15,
                            ClickPriceC = e.ClickPriceC != null ? e.ClickPriceC : equipments.Where(x => x.PHC1 == e.PHC1 && x.PHC4 == e.PHC4 && x.PHC5 == e.PHC5).Select(x => x.ClickPriceC).Max() * 1.15,
                            BWCoefficient = 0,
                            CCoefficient = 0,
                            BWPages = 0,
                            CPages = 0,
                            Type = e.PHC5 == null ? "" : (e.PHC5.Contains("(MF)") ? "MFP" : "Printer"),
                            IsUsed = pe.IsUsed.GetValueOrDefault(),
                            IsInClient = pe.IsInClient.GetValueOrDefault()
                        };
            svrEquipments = query.ToList();
            int totalRecBW = (int)svrEquipments.Sum(x => x.RecBWVolume * x.Quantity);
            int totalRecC = (int)svrEquipments.Sum(x => x.RecCVolume * x.Quantity);
            int totalUsedBW = 0;
            int totalUsedC = 0;
            foreach (ServiceValidationRequestEquipment svre in svrEquipments)
            {
                if (svr.Volumes.BWVolume != 0 && svre.RecBWVolume != null)
                {
                    svre.BWCoefficient = ((double)svre.RecBWVolume / totalRecBW) * svre.Quantity * 100;
                    int toAddBW = (int)Math.Floor((double)(svr.Volumes.BWVolume * (svre.BWCoefficient / 100)));
                    svre.BWPages += toAddBW;
                    totalUsedBW += toAddBW;
                }
                if (svr.Volumes.CVolume != 0 && svre.RecCVolume != null)
                {
                    svre.CCoefficient = ((double)svre.RecCVolume / totalRecC) * svre.Quantity * 100;
                    int toAddC = (int)Math.Floor((double)(svr.Volumes.CVolume * (svre.CCoefficient / 100)));
                    svre.CPages += toAddC;
                    totalUsedC += toAddC;
                }
            }

            int toUseBW = svr.Volumes.BWVolume - totalUsedBW;
            int toUseC = svr.Volumes.CVolume - totalUsedC;
            foreach (ServiceValidationRequestEquipment svre in svrEquipments)
            {
                if (totalUsedBW == totalRecBW && totalUsedC == totalRecC)
                {
                    break;
                }
                if (svr.Volumes.BWVolume != 0 && svre.RecBWVolume != null && totalUsedBW != totalRecBW)
                {
                    int toAddBW = (int)Math.Round((double)(toUseBW * (svre.BWCoefficient / 100)));
                    svre.BWPages += toAddBW;
                    totalUsedBW += toAddBW;
                }
                if (svr.Volumes.BWVolume != 0 && svre.RecBWVolume != null && totalUsedC != totalRecC)
                {
                    int toAddC = (int)Math.Round((double)(toUseC * (svre.CCoefficient / 100)));
                    svre.CPages += toAddC;
                    totalUsedC += toAddC;
                }
            }
            return svrEquipments;
        }

        public List<ApprovedPrintingService> DeleteServiceQuoteRequest(ServiceQuoteDeleteRequest sqdr)
        {
            try
            {
                var request = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == sqdr.ID).FirstOrDefault();
                if (request != null)
                {
                    request.ToDelete = true;
                    try
                    {
                        db.SaveChanges();
                        ApprovedPrintingService toRemove = sqdr.PSList.Where(x => x.ID == sqdr.ID).FirstOrDefault();
                        if (toRemove != null)
                        {
                            sqdr.PSList.Remove(toRemove);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return sqdr.PSList;
        }

        public ReevaluateVVARequest ReevaluateVVARequest(ReevaluateVVARequest rr)
        {
            try
            {
                var request = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == rr.ID)
                    .Include(x => x.BB_PrintingServices)
                    .Include(x => x.BB_PrintingServices.BB_VVA)
                    .FirstOrDefault();
                if (request != null)
                {
                    request.IsApproved = false;
                    request.IsComplete = false;
                    request.ApprovedAt = null;
                    request.ApprovedBy = null;
                    request.RequestedAt = DateTime.Now;
                    request.SEObservations = rr.Observations;
                    if (request.BB_PrintingServices != null && request.BB_PrintingServices.BB_VVA != null)
                    {
                        request.BB_PrintingServices.BB_VVA.RequestedBWExcess = rr.RequestedBWExcess;
                        request.BB_PrintingServices.BB_VVA.RequestedCExcess = rr.RequestedCExcess;
                        request.BB_PrintingServices.BB_VVA.RequestedRent = rr.RequestedRent;
                    }
                    ApprovedPrintingService toEdit = rr.ApprovedPrintingServices.Where(x => x.ID == rr.ID).FirstOrDefault();
                    if (toEdit != null)
                    {
                        int toEditIndex = rr.ApprovedPrintingServices.IndexOf(toEdit) + 1;
                        if (toEditIndex < request.BB_PrintingServices.BB_Proposal_PrintingServices2.ActivePrintingService)
                        {
                            request.BB_PrintingServices.BB_Proposal_PrintingServices2.ActivePrintingService -= 1;
                            rr.ActivePrintingService -= 1;
                        }
                        toEdit.GlobalClickVVA.RequestedBWExcess = rr.RequestedBWExcess;
                        toEdit.GlobalClickVVA.RequestedCExcess = rr.RequestedCExcess;
                        toEdit.GlobalClickVVA.RequestedRent = rr.RequestedRent;
                        toEdit.RequestedAt = DateTime.Now;
                        toEdit.SEObservations = rr.Observations;
                        rr.PendingServiceQuoteRequests.Add(toEdit);
                        rr.ApprovedPrintingServices.Remove(toEdit);
                    }
                }
                try
                {
                    db.SaveChanges();
                    SendServiceRequestEmail(request.RequestedBy, rr.ID, 1);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return rr;
        }

        public ReevaluateGlobalClickNoVolumeRequest ReevaluateGlobalClickNoVolumeRequest(ReevaluateGlobalClickNoVolumeRequest rr)
        {
            try
            {
                var request = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == rr.ID)
                    .Include(x => x.BB_PrintingServices)
                    .Include(x => x.BB_PrintingServices.BB_VVA)
                    .FirstOrDefault();
                if (request != null)
                {
                    request.IsApproved = false;
                    request.IsComplete = false;
                    request.ApprovedAt = null;
                    request.ApprovedBy = null;
                    request.RequestedAt = DateTime.Now;
                    request.SEObservations = rr.Observations;
                    if (request.BB_PrintingServices != null && request.BB_PrintingServices.BB_PrintingServices_NoVolume != null)
                    {
                        request.BB_PrintingServices.BB_PrintingServices_NoVolume.RequestedGlobalClickBW = rr.RequestedGlobalClickBW;
                        request.BB_PrintingServices.BB_PrintingServices_NoVolume.RequestedGlobalClickC = rr.RequestedGlobalClickC;
                    }
                    ApprovedPrintingService toEdit = rr.ApprovedPrintingServices.Where(x => x.ID == rr.ID).FirstOrDefault();
                    if (toEdit != null)
                    {
                        int toEditIndex = rr.ApprovedPrintingServices.IndexOf(toEdit) + 1;
                        if (toEditIndex < request.BB_PrintingServices.BB_Proposal_PrintingServices2.ActivePrintingService)
                        {
                            request.BB_PrintingServices.BB_Proposal_PrintingServices2.ActivePrintingService -= 1;
                            rr.ActivePrintingService -= 1;
                        }
                        toEdit.GlobalClickNoVolume.RequestedGlobalClickBW = rr.RequestedGlobalClickBW;
                        toEdit.GlobalClickNoVolume.RequestedGlobalClickC = rr.RequestedGlobalClickC;
                        toEdit.RequestedAt = DateTime.Now;
                        toEdit.SEObservations = rr.Observations;
                        rr.PendingServiceQuoteRequests.Add(toEdit);
                        rr.ApprovedPrintingServices.Remove(toEdit);
                    }
                }
                try
                {
                    db.SaveChanges();
                    SendServiceRequestEmail(request.RequestedBy, rr.ID, 2);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return rr;
        }

        public ReevaluateClickPerModelRequest ReevaluateClickPerModelRequest(ReevaluateClickPerModelRequest rr)
        {
            try
            {
                var request = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == rr.ID)
                    .Include(x => x.BB_PrintingServices)
                    .FirstOrDefault();
                if (request != null)
                {
                    request.IsApproved = false;
                    request.IsComplete = false;
                    request.ApprovedAt = null;
                    request.ApprovedBy = null;
                    request.RequestedAt = DateTime.Now;
                    request.SEObservations = rr.Observations;
                    List<Machine> machines = new List<Machine>();
                    foreach (ServiceValidationRequestEquipment equipment in rr.Equipments)
                    {
                        BB_PrintingService_Machines machine = db.BB_PrintingService_Machines.Where(x => x.ID == equipment.ID).FirstOrDefault();
                        if (machine != null)
                        {
                            machine.RequestedBWClickPrice = equipment.RequestedBWClickPrice;
                            machine.RequestedCClickPrice = equipment.RequestedCClickPrice;
                            try
                            {
                                db.SaveChanges();
                                machines.Add(
                                    new Machine
                                    {
                                        BWVolume = machine.BWVolume,
                                        CodeRef = machine.CodeRef,
                                        CVolume = machine.CVolume,
                                        Description = machine.Description,
                                        Qty = machine.Quantity.Value,
                                        ID = machine.ID,
                                        RequestedBWClickPrice = machine.RequestedBWClickPrice,
                                        RequestedCClickPrice = machine.RequestedCClickPrice,
                                        ClickPriceBW = machine.ApprovedBW,
                                        ClickPriceC = machine.ApprovedC,
                                        BWPVP = machine.BWPVP,
                                        CPVP = machine.CPVP
                                    }
                                );
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }
                    ApprovedPrintingService toEdit = rr.ApprovedPrintingServices.Where(x => x.ID == rr.ID).FirstOrDefault();
                    if (toEdit != null)
                    {
                        int toEditIndex = rr.ApprovedPrintingServices.IndexOf(toEdit) + 1;
                        if (toEditIndex < request.BB_PrintingServices.BB_Proposal_PrintingServices2.ActivePrintingService)
                        {
                            request.BB_PrintingServices.BB_Proposal_PrintingServices2.ActivePrintingService -= 1;
                            rr.ActivePrintingService -= 1;
                        }
                        toEdit.Machines = machines;
                        toEdit.RequestedAt = DateTime.Now;
                        toEdit.SEObservations = rr.Observations;
                        rr.PendingServiceQuoteRequests.Add(toEdit);
                        rr.ApprovedPrintingServices.Remove(toEdit);
                    }
                }
                try
                {
                    db.SaveChanges();
                    //SendServiceRequestEmail(request.RequestedBy, rr.ID, 2);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return rr;
        }
    }
}