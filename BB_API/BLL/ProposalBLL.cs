using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using log4net;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WebApplication1.App_Start;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Models.SetupXML;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.ViewModels.PrintingServicesViewModels;

namespace WebApplication1.BLL
{
    public class ProposalBLL
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public ActionResponse ProposalDraftSave(ProposalRootObject p)
        {
            ActionResponse err = new ActionResponse();
            try
            {
                using (var context = new BB_DB_DEVEntities2())
                {
                    BB_Proposal proposal = db.BB_Proposal
                        .Where(x => x.ID == p.Draft.details.ID)
                        .Include(x => x.BB_Proposal_OPSImplement)
                        .Include(x => x.BB_Proposal_OPSManage)
                        .FirstOrDefault();
                    proposal.AccountManager = p.Draft.details.AccountManager;
                    proposal.CampaignID = p.Draft.details.CampaignID;
                    proposal.ClientAccountNumber = p.Draft.client.accountnumber;
                    proposal.CRM_QUOTE_ID = p.Draft.details.CRM_QUOTE_ID;
                    proposal.Description = p.Draft.details.Description;
                    proposal.ModifiedBy = p.Draft.details.ModifiedBy;
                    proposal.ModifiedTime = DateTime.Now;
                    proposal.Name = p.Draft.details.Name;
                    proposal.SubTotal = p.Summary.subTotal;
                    proposal.ValueTotal = p.Summary.businessTotal;

                    proposal.IsMultipleContract = p.Draft.details.IsMultipleContract ?? false;
                    proposal.IsNP = p.Draft.baskets.IsNP;
                    proposal.ContractNumberPai = p.Draft.details.ExistanteContractNumber;


                    //List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposal.ID).ToList();
                    //db.BB_Proposal_Quote.RemoveRange(quotes);

                    //List<BB_Proposal_Quote_RS> quotesRS = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposal.ID).ToList();
                    //db.BB_Proposal_Quote_RS.RemoveRange(quotesRS);

                    //List<BB_Proposal_Commission> commision = db.BB_Proposal_Commission.Where(x => x.ProposalID == proposal.ID).ToList();
                    //db.BB_Proposal_Commission.RemoveRange(commision);

                    List<BB_Proposal_PsConfig> psConfi = db.BB_Proposal_PsConfig.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_PsConfig.RemoveRange(psConfi);

                    //List<BB_Proposal_Overvaluation> over = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposal.ID).ToList();
                    //db.BB_Proposal_Overvaluation.RemoveRange(over);

                    List<BB_Proposal_Vva> vva = db.BB_Proposal_Vva.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Vva.RemoveRange(vva);

                    List<BB_Proposal_PrintingServices> pritningService = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_PrintingServices.RemoveRange(pritningService);

                    //List<BB_Proposal_Financing> fin = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposal.ID).ToList();
                    //db.BB_Proposal_Financing.RemoveRange(fin);

                    //List<BB_Proposal_FinancingMonthly> financingMOntlhy = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == proposal.ID).ToList();
                    //db.BB_Proposal_FinancingMonthly.RemoveRange(financingMOntlhy);

                    //List<BB_Proposal_FinancingTrimestral> financingtri = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == proposal.ID).ToList();
                    //db.BB_Proposal_FinancingTrimestral.RemoveRange(financingtri);

                    List<BB_Proposal_Client> lstCliente = db.BB_Proposal_Client.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Client.RemoveRange(lstCliente);

                    List<BB_Proposal_Consignments> lstConsignacoes = db.BB_Proposal_Consignments.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Consignments.RemoveRange(lstConsignacoes);


                    db.Entry(proposal).State = proposal.ID == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();

                    if (proposal != null)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };

                        log4net.ThreadContext.Properties["proposal_id"] = proposal.ID;
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(proposal, settings);
                        Exception message = new Exception("Proposta gravada com sucesso");
                        log.Info(json, message);
                    }

                }

                using (var context = new BB_DB_DEVEntities2())
                {
                    BB_Proposal proposal = context.BB_Proposal.Find(p.Draft.details.ID);
                    try
                    {
                        string financingCompany = context.BB_FinancingContractType
                                                         .Where(f => f.ID == p.Draft.financing.ContractTypeId)
                                                         .Select(f => f.Company + " - " + f.CompanyCode)
                                                         .FirstOrDefault();

                        proposal.CodArrend = financingCompany;
                        context.Entry(proposal).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    int ProposalID = proposal.ID;



                    List<BB_Maquinas_Usadas_Gestor> lstmaquinsaudasGestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.ProposalID == ProposalID).ToList();
                    foreach (var item in lstmaquinsaudasGestor)
                    {
                        item.ProposalID = null;
                        item.IsReserved = false;
                        db.Entry(item).State = item.ID == 0 ? EntityState.Added : EntityState.Modified;
                        db.SaveChanges();
                    }

                    // ATUALIZAR A BB_PROPOSAL_QUOTE ------------------------------------------------------------------------------
                    p.Draft.baskets.os_basket = Update_BB_Proposal_Quote(p, ProposalID);

                    // ATUALIZAR A BB_PROPOSAL_QUOTE_RS ---------------------------------------------------------------------------
                    Update_BB_Proposal_Quote_RS(p.Draft.baskets.rs_basket, ProposalID);

                    Update_OPS(proposal, p.Draft, ProposalID);

                    UpdateFinancing(p.Draft.financing, ProposalID);

                    UpdateOvervaluation(p.Draft.overvaluations, ProposalID);

                    // UPDATE do BB_PROPOSAL_Commission ----------------------------------------------------------------------------
                    UpdateCommissions(p.Summary.commission, ProposalID);

                    // UPDATE das RETOMAS ------------------------------------------------------------------------------------------

                    UpdateUpturns(p.Draft.upturns.upturns, ProposalID, p.Draft.client.accountnumber);

                    p.Draft.upturns.upturns = db.BB_Proposal_Upturn.Where(x => x.ProposalID == ProposalID).ToList();


                    //PRINTING SERVICES --------------------------------------------------------------------------------------------

                    // ISTO ESTÁ A SER UTILIZADO???? EU ACHO QUE NAO
                    if (p.Draft.printingServices != null)
                    {
                        var configpPrintingServices = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<PrintingServices, BB_Proposal_PrintingServices>();
                        });

                        IMapper iMapperPrintinfServices = configpPrintingServices.CreateMapper();

                        BB_Proposal_PrintingServices printingService = iMapperPrintinfServices.Map<PrintingServices, BB_Proposal_PrintingServices>(p.Draft.printingServices);

                        printingService.ProposalID = ProposalID;

                        db.BB_Proposal_PrintingServices.Add(printingService);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }

                        //VVA
                        if (p.Draft.printingServices.vva != null)
                        {
                            var configpVVA = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<Vva, BB_Proposal_Vva>();
                            });

                            IMapper iMapperVVA = configpVVA.CreateMapper();

                            BB_Proposal_Vva vva = iMapperVVA.Map<Vva, BB_Proposal_Vva>(p.Draft.printingServices.vva);

                            vva.ProposalID = ProposalID;


                            db.BB_Proposal_Vva.Add(vva);
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


                    try
                    {
                        PrintingServices2 printingServices2 = p.Draft.printingServices2;

                        if (printingServices2 != null)
                        {
                            // Se for um printingService NOVO, Adiciona-se tudo
                            if (printingServices2.ID == null)
                            {
                                AddNewPrintingService(printingServices2, ProposalID);
                            }
                            //Se for o printingService JÁ EXISTIR..
                            else
                            {
                                BB_Proposal_PrintingServices2 toUpdate = db.BB_Proposal_PrintingServices2.FirstOrDefault(x => x.ID == printingServices2.ID);
                                toUpdate.ActivePrintingService = printingServices2.ActivePrintingService;

                                db.SaveChanges();

                                UpdatePrintingService(printingServices2, toUpdate.ID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }


                    //BB_PROPOSAL_Cliente
                    BB_Proposal_Client cliente1 = new BB_Proposal_Client();
                    cliente1.ClientID = p.Draft.client.accountnumber;
                    cliente1.IsNewClient = p.Draft.client.isNewClient;
                    cliente1.ProposalID = ProposalID;
                    cliente1.Name = p.Draft.client.Name;
                    cliente1.IsPublicSector = p.Draft.client.isPublicSector;
                    cliente1.IsGMA = p.Draft.client.isGMA;
                    db.BB_Proposal_Client.Add(cliente1);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    if (p.Draft.client.modeId.GetValueOrDefault() == 1)
                    {
                        BB_Clientes count = db.BB_Clientes.Where(x => x.accountnumber == p.Draft.client.accountnumber).FirstOrDefault();
                        if (count == null)
                        {
                            string usename = "";
                            using (var db1 = new masterEntities())
                            {
                                usename = db1.AspNetUsers.Where(x => x.Email == p.Draft.details.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                            }
                            BB_Clientes c = new BB_Clientes();
                            c.accountnumber = p.Draft.client.accountnumber;
                            c.Name = p.Draft.client.Name;
                            c.PostalCode = p.Draft.client.PostalCode;
                            c.NIF = p.Draft.client.NIF;
                            c.City = p.Draft.client.City;
                            c.address1_line1 = p.Draft.client.address1_line1;
                            c.IsClienteBB = true;
                            c.Owner = usename;
                            db.BB_Clientes.Add(c);
                            db.SaveChanges();
                        }
                    }

                    var dl_BillTo = p.Draft.deliveryLocationsBES.deliveryLocationsShipToBillTo.Where(x => x.AccountType == "Bill To");

                    if (dl_BillTo != null)
                    {
                        foreach (var billTo in dl_BillTo)
                        {
                            using (var db = new BB_DB_DEVEntities2())
                            {
                                bool exists = db.BB_Proposal_DeliveryLocation.Any(x => x.IDX == billTo.IDX && x.ProposalID == p.Draft.details.ID);

                                if (!exists)
                                {
                                    db.BB_Proposal_DeliveryLocation.Add(billTo);
                                }

                                db.SaveChanges();

                            }
                        }
                    }

                    //BB_PROPOSAL_COnsigments
                    var configConsigments = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Consignment, BB_Proposal_Consignments>();
                    });

                    IMapper iMapperConsigments = configConsigments.CreateMapper();

                    BB_Proposal_Consignments consignment = iMapperConsigments.Map<Consignment, BB_Proposal_Consignments>(p.Draft.consignment);

                    consignment.ProposalID = ProposalID;

                    db.BB_Proposal_Consignments.Add(consignment);

                    //BB_Permissions
                    if (p.Draft.shareProfileDelegation != null)
                    {
                        List<BB_Permissions> bB_Permissions_db = db.BB_Permissions.Where(x => x.ProposalID == p.Draft.details.ID).ToList();

                        if (p.Draft.shareProfileDelegation.Count != bB_Permissions_db.Count)
                        {
                            bB_Permissions_db.ForEach(permission => permission.ToDelete = !p.Draft.shareProfileDelegation.Any(i => i.ID == permission.ID));

                        }
                    }
                    else
                    {
                        err.ProposalObj.Draft.shareProfileDelegation = new List<BB_Permissions>();
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }

                    //Contacts_Documentation
                    CreateContactsDocumentation(p, ProposalID);

                    //Add Documents
                    //CreateDocuments(p, p.Draft.details.CRM_QUOTE_ID);


                    //TYPE OF CLIENT
                    BB_TypeOfClient typeOfClient = db.BB_TypeOfClient.Where(x => x.ProposalID == p.Draft.details.ID).FirstOrDefault();
                    if (typeOfClient is null)
                    {
                        typeOfClient = new BB_TypeOfClient();
                    }

                    typeOfClient.ProposalID = p.Draft.details.ID;
                    typeOfClient.Prospect = p.Draft.baskets.prospect;
                    typeOfClient.NewBusinessLine = p.Draft.baskets.newBusinessLine;
                    typeOfClient.GMA = p.Draft.baskets.GMA;
                    typeOfClient.BEUSupport = p.Draft.baskets.BEUSupport;

                    try
                    {
                        db.BB_TypeOfClient.AddOrUpdate(typeOfClient);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (p.Draft.printingServices2.PrintingCondition != 0)
                    {
                        BB_Proposal_Condition_Type existPrintingCondition = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == p.Draft.details.ID && x.ConditionType == "ZVBS").FirstOrDefault();

                        if (existPrintingCondition != null)
                        {
                            existPrintingCondition.ConditionValue = p.Draft.printingServices2.PrintingCondition;

                            try
                            {
                                db.BB_Proposal_Condition_Type.AddOrUpdate(existPrintingCondition);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                        else
                        {
                            BB_Proposal_Condition_Type printingConditionType = new BB_Proposal_Condition_Type()
                            {
                                ProposalID = p.Draft.details.ID,
                                ConditionType = "ZVBS",
                                ConditionValue = p.Draft.printingServices2.PrintingCondition
                            };

                            try
                            {
                                db.BB_Proposal_Condition_Type.AddOrUpdate(printingConditionType);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                    }


                    err.ProposalObj = new ProposalRootObject();
                    err.ProposalObj.Draft = p.Draft;

                    // VALIDACOES NO PRODUCCION ---------------------------------
                    bool? isNP = db.BB_Proposal.Where(x => x.ID == proposal.ID).Select(x => x.IsNP).FirstOrDefault();

                    BB_WFA_NP_Approvers approval = db.BB_WFA_NP_Approvers.Where(x => x.ProposalID == proposal.ID).OrderByDescending(x => x.ID).FirstOrDefault();

                    // Se já existir um approver para este proposalID e que tenha aprovado ou reprovado..
                    // se já estiver APROVADO, coloco o IsNP a false <=> processo NAO FICA bloqueado
                    // se já estiver REPROVADO, coloco o IsNP a true <=> processo FICA bloqueado
                    if (approval != null && approval.IsApproved != null)
                    {
                        err.ProposalObj.Draft.baskets.IsPassedNP = approval.IsApproved.Value;
                    }
                    else
                    {
                        if (isNP == true)
                        {
                            err.ProposalObj.Draft.baskets.IsPassedNP = false;
                        }
                        else
                        {
                            err.ProposalObj.Draft.baskets.IsPassedNP = true;
                        }
                    }
                    // ------------------------------------------------------------------

                    return err;


                }
            }
            catch (Exception ex)
            {
                //err.ErrorCode = 1;
                err.Message = ex.Message.ToString();

            }

            return err;
        }

        public ActionResponse LoadProposal(LoadProposalInfo i)
        {
            ActionResponse err = new ActionResponse();

            err.ProposalObj = new ProposalRootObject();
            err.ProposalObj.Draft = new Draft();

            try
            {
                var proposal = db.BB_Proposal.Find(i.ProposalId);
                err.ProposalObj.Draft.details = new Details();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<BB_Proposal, Details>();
                });

                IMapper iMapper = config.CreateMapper();
                LD_Contrato lD_Contrato = db.LD_Contrato.Where(x => x.ProposalID == i.ProposalId).FirstOrDefault();
                BB_Proposal_Status statusProp = db.BB_Proposal_Status.Where(x => x.ID == proposal.StatusID).FirstOrDefault();
                err.ProposalObj.Draft.details = iMapper.Map<BB_Proposal, Details>(proposal);
                err.ProposalObj.Draft.details.Status = new ProposalStatus();
                err.ProposalObj.Draft.details.Status.Name = statusProp.Description;
                err.ProposalObj.Draft.details.Status.IsEdit = statusProp.BB_Edit;
                err.ProposalObj.Draft.details.Status.Phase = statusProp.Phase;
                err.ProposalObj.Draft.details.AccountManager = proposal.AccountManager;
                err.ProposalObj.Draft.details.CampaignID = proposal.CampaignID;

                err.ProposalObj.Draft.details.IsMultipleContract = proposal.IsMultipleContract ?? false;
                err.ProposalObj.Draft.details.ExistanteContractNumber = proposal.ContractNumberPai ?? "";
                err.ProposalObj.Draft.details.AdministrationComments = lD_Contrato != null ? lD_Contrato.ComentariosDevolucao : "";

                err.ProposalObj.Draft.baskets = new Baskets();

                //ONE SHOTE
                List<BB_Proposal_Quote> proposal_quote = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == i.ProposalId).ToList();

                err.ProposalObj.Draft.baskets.os_basket = new List<OsBasket>();

                foreach (var quote in proposal_quote)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Quote, OsBasket>();
                    });

                    IMapper iMapper1 = config1.CreateMapper();

                    OsBasket basket = iMapper1.Map<BB_Proposal_Quote, OsBasket>(quote);


                    //COUNTERS
                    basket.counters = new List<Counter>();
                    List<BB_Proposal_Counters> lstCounters = db.BB_Proposal_Counters.Where(x => x.OSID == quote.ID).ToList();
                    foreach (var counter in lstCounters)
                    {
                        var configCounter = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<BB_Proposal_Counters, Counter>();
                        });

                        IMapper iMapperCounter = configCounter.CreateMapper();

                        basket.counters.Add(iMapperCounter.Map<BB_Proposal_Counters, Counter>(counter));
                    }

                    //PSCONFIG
                    basket.psConfig = new PsConfig();
                    BB_Proposal_PsConfig psconfig = db.BB_Proposal_PsConfig.Where(x => x.ItemID == quote.ID).FirstOrDefault();
                    var configPSCOnfig = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_PsConfig, PsConfig>();
                    });

                    IMapper iMapperPSCOnfig = configPSCOnfig.CreateMapper();
                    basket.psConfig = iMapperPSCOnfig.Map<BB_Proposal_PsConfig, PsConfig>(psconfig);


                    err.ProposalObj.Draft.baskets.os_basket.Add(basket);
                }

                //Serviços recorrenctes
                err.ProposalObj.Draft.baskets.rs_basket = new List<RsBasket>();
                List<BB_Proposal_Quote_RS> proposal_quote_rs = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == i.ProposalId).ToList();
                foreach (var quote in proposal_quote_rs)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Quote_RS, RsBasket>();
                    });

                    IMapper iMapper1 = config1.CreateMapper();

                    RsBasket basket = iMapper1.Map<BB_Proposal_Quote_RS, RsBasket>(quote);

                    err.ProposalObj.Draft.baskets.rs_basket.Add(basket);
                }


                // lista de comentários do negócio
                err.ProposalObj.Draft.baskets.lst_wfa_comments_business = db.BB_WFA_Comments_Business.Where(q => q.ProposalID == err.ProposalObj.Draft.details.ID).ToList();


                err.ProposalObj.Draft.opsPacks = new OPSPacks();
                List<BB_Proposal_OPSImplement> bb_Proposal_OPSImplements = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == i.ProposalId).OrderBy(x => x.Position).ToList();
                List<BB_Proposal_OPSManage> bb_Proposal_OPSManages = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == i.ProposalId).OrderBy(x => x.Position).ToList();
                foreach (BB_Proposal_OPSImplement item in bb_Proposal_OPSImplements)
                {
                    OPSImplement opsImplement = new OPSImplement
                    {
                        CodeRef = item.CodeRef,
                        Description = item.Description,
                        Family = item.Family,
                        ID = item.ID,
                        InCatalog = item.InCatalog.GetValueOrDefault(),
                        IsFinanced = item.IsFinanced.GetValueOrDefault(),
                        MaxRange = item.MaxRange,
                        MinRange = item.MinRange,
                        Name = item.Name,
                        PVP = item.PVP,
                        Quantity = item.Quantity,
                        Type = item.Type,
                        UnitDiscountPrice = item.UnitDiscountPrice,
                        IsValidated = item.IsValidated.GetValueOrDefault()
                    };
                    err.ProposalObj.Draft.opsPacks.opsImplement.Add(opsImplement);
                }
                foreach (BB_Proposal_OPSManage item in bb_Proposal_OPSManages)
                {
                    OPSManage opsManage = new OPSManage
                    {
                        CodeRef = item.CodeRef,
                        Description = item.Description,
                        Family = item.Family,
                        ID = item.ID,
                        InCatalog = item.InCatalog.GetValueOrDefault(),
                        MaxRange = item.MaxRange,
                        MinRange = item.MinRange,
                        Name = item.Name,
                        PVP = item.PVP,
                        Quantity = item.Quantity,
                        TotalMonths = item.TotalMonths,
                        Type = item.Type,
                        UnitDiscountPrice = item.UnitDiscountPrice,
                        IsValidated = item.IsValidated.GetValueOrDefault()
                    };
                    err.ProposalObj.Draft.opsPacks.opsManage.Add(opsManage);
                }

                // RETOMAS -----------------------------------------
                err.ProposalObj.Draft.upturns = new Upturns();
                err.ProposalObj.Draft.upturns.upturns = db.BB_Proposal_Upturn.Where(x => x.ProposalID == i.ProposalId).OrderBy(x => x.ID).ToList();

                //CLIENTE
                BB_Proposal_Client cli = db.BB_Proposal_Client.Where(x => x.ProposalID == i.ProposalId).FirstOrDefault();
                err.ProposalObj.Draft.client = new Client();
                BB_Clientes infCliente = new BB_Clientes();
                if (cli != null)
                    infCliente = db.BB_Clientes.Where(x => x.accountnumber == cli.ClientID).FirstOrDefault();

                if (cli != null && infCliente != null)
                {
                    var configCliente = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Clientes, Client>();
                    });

                    IMapper iMapperCliente = configCliente.CreateMapper();

                    Client c = iMapperCliente.Map<BB_Clientes, Client>(infCliente);

                    if (infCliente.Territory != null && infCliente.Territory.Length >= 8)
                    {
                        c.SalesGroup = infCliente.Territory.Substring(4, 4);
                    }
                    if (infCliente.Territory != null && infCliente.Territory.Length >= 11)
                    {
                        c.SalesOffice = infCliente.Territory != null ? infCliente.Territory.Substring(8, 3) : "-";
                    }

                    if (infCliente.GMA == null)
                    {
                        c.GMA = "N/A";
                    }

                    if (infCliente.GMA_Identifier == null)
                    {
                        c.GMA_Identifier = "N/A";
                    }

                    c.modeId = infCliente.IsClienteBB == true ? 1
                    : infCliente.IsClienteBB == false ? 0
                    : (int?)null;

                    err.ProposalObj.Draft.client = c;
                    err.ProposalObj.Draft.client.isNewClient = cli.IsNewClient;
                    err.ProposalObj.Draft.client.isPublicSector = cli.IsPublicSector;
                    err.ProposalObj.Draft.client.isGMA = cli.IsGMA;
                }
                else
                {
                    err.ProposalObj.Draft.client.accountnumber = "";
                    err.ProposalObj.Draft.client.isNewClient = false;
                    err.ProposalObj.Draft.client.isPublicSector = false;
                    err.ProposalObj.Draft.client.isGMA = false;
                    //}
                }

                //PRINTING SERVICES 
                err.ProposalObj.Draft.printingServices = new PrintingServices();

                BB_Proposal_PrintingServices bb_Proposal_prtinginservice = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                if (bb_Proposal_prtinginservice != null)
                {
                    var configPrintinService = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_PrintingServices, PrintingServices>();
                    });

                    IMapper iMapperPringinService = configPrintinService.CreateMapper();

                    PrintingServices prService = iMapperPringinService.Map<BB_Proposal_PrintingServices, PrintingServices>(bb_Proposal_prtinginservice);

                    err.ProposalObj.Draft.printingServices = prService;
                }
                //VVA
                err.ProposalObj.Draft.printingServices.vva = new Vva();
                BB_Proposal_Vva bb_Proposal_vva = db.BB_Proposal_Vva.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                if (bb_Proposal_vva != null)
                {
                    var configvva = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Vva, Vva>();
                    });

                    IMapper iMappervva = configvva.CreateMapper();

                    Vva vva = iMappervva.Map<BB_Proposal_Vva, Vva>(bb_Proposal_vva);
                    err.ProposalObj.Draft.printingServices.vva = vva;
                }

                err.ProposalObj.Draft.printingServices2 = new PrintingServices2();
                BB_Proposal_PrintingServices2 printingServices2 = db.BB_Proposal_PrintingServices2
                    .Include(x => x.BB_PrintingServices.Select(ps => ps.BB_VVA))
                    .Include(a => a.BB_PrintingServices.Select(m => m.BB_PrintingService_Machines))
                    .FirstOrDefault(x => x.ProposalID == proposal.ID);
                BB_Proposal_Condition_Type existPrintingCondition = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposal.ID && x.ConditionType == "ZVBS").FirstOrDefault();


                if (printingServices2 != null)
                {
                    PrintingServices2 proposalPS2 = new PrintingServices2()
                    {
                        ID = printingServices2.ID,
                        ActivePrintingService = printingServices2.ActivePrintingService,
                        ApprovedPrintingServices = new List<ApprovedPrintingService>(),
                        PendingServiceQuoteRequests = new List<ApprovedPrintingService>(),
                        PrintingCondition = 0
                    };

                    if (existPrintingCondition != null)
                    {
                        proposalPS2.PrintingCondition = existPrintingCondition.ConditionValue;
                    }
                    foreach (BB_PrintingServices ps in printingServices2.BB_PrintingServices)
                    {
                        ApprovedPrintingService newPS = new ApprovedPrintingService()
                        {
                            BWVolume = ps.BWVolume.Value,
                            ContractDuration = ps.ContractDuration.Value,
                            CVolume = ps.CVolume.Value,
                            ID = ps.ID,
                            Fee = ps.Fee.Value,
                            Machines = new List<Machine>(),
                            IsPrecalc = ps.IsPrecalc.Value,

                        };
                        if (ps.BB_VVA != null)
                        {
                            GlobalClickVVA psVVA = new GlobalClickVVA()
                            {
                                BWExcessPVP = ps.BB_VVA.BWExcessPVP != null ? ps.BB_VVA.BWExcessPVP.Value : 0,
                                CExcessPVP = ps.BB_VVA.CExcessPVP != null ? ps.BB_VVA.CExcessPVP.Value : 0,
                                ExcessBillingFrequency = ps.BB_VVA.ExcessBillingFrequency != null ? ps.BB_VVA.ExcessBillingFrequency.Value : 0,
                                ReturnType = ps.BB_VVA.ReturnType != null ? ps.BB_VVA.ReturnType.Value : 0,
                                PVP = ps.BB_VVA.PVP != null ? ps.BB_VVA.PVP.Value : 0,
                                RentBillingFrequency = ps.BB_VVA.RentBillingFrequency != null ? ps.BB_VVA.RentBillingFrequency.Value : 0,
                                RequestedRent = ps.BB_VVA.RequestedRent,
                                RequestedBWExcess = ps.BB_VVA.RequestedBWExcess,
                                RequestedCExcess = ps.BB_VVA.RequestedCExcess,
                            };
                            newPS.GlobalClickVVA = psVVA;
                        }
                        if (ps.BB_PrintingServices_NoVolume != null)
                        {
                            GlobalClickNoVolume nv = new GlobalClickNoVolume()
                            {
                                GlobalClickBW = (ps.BB_PrintingServices_NoVolume.GlobalClickBW != null ? ps.BB_PrintingServices_NoVolume.GlobalClickBW.Value : 0),
                                GlobalClickC = (ps.BB_PrintingServices_NoVolume.GlobalClickC != null ? ps.BB_PrintingServices_NoVolume.GlobalClickC.Value : 0),
                                PageBillingFrequency = ps.BB_PrintingServices_NoVolume.PageBillingFrequency.Value,
                                RequestedGlobalClickBW = ps.BB_PrintingServices_NoVolume.RequestedGlobalClickBW,
                                RequestedGlobalClickC = ps.BB_PrintingServices_NoVolume.RequestedGlobalClickC,
                            };
                            newPS.GlobalClickNoVolume = nv;
                        }
                        if (ps.BB_PrintingServices_ClickPerModel != null)
                        {
                            ClickPerModel cpm = new ClickPerModel()
                            {
                                PageBillingFrequency = ps.BB_PrintingServices_ClickPerModel.PageBillingFrequency.Value,
                            };
                            newPS.ClickPerModel = cpm;
                        }

                        List<BB_Equipamentos> equipamentos = new List<BB_Equipamentos>();
                        equipamentos = db.BB_Equipamentos.ToList();

                        foreach (BB_PrintingService_Machines machine in ps.BB_PrintingService_Machines)
                        {
                            Machine psMachine = new Machine()
                            {
                                BWVolume = machine.BWVolume,
                                CodeRef = machine.CodeRef,
                                CVolume = machine.CVolume,
                                Description = machine.Description,
                                Qty = machine.Quantity.Value,
                                ID = machine.ID,
                                RequestedBWClickPrice = machine.RequestedBWClickPrice,
                                RequestedCClickPrice = machine.RequestedCClickPrice,
                                ClickPriceBW = equipamentos.Where(x => x.CodeRef == machine.CodeRef).Select(x => x.ClickPriceBW).FirstOrDefault() ?? 0,
                                ClickPriceC = equipamentos.Where(x => x.CodeRef == machine.CodeRef).Select(x => x.ClickPriceC).FirstOrDefault() ?? 0,
                                BWPVP = machine.BWPVP,
                                CPVP = machine.CPVP,
                                ApprovedBW = machine.ApprovedBW,
                                ApprovedC = machine.ApprovedC,

                            };
                            newPS.Machines.Add(psMachine);
                        }
                        BB_Proposal_PrintingServiceValidationRequest validationRequest = ps.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == ps.ID && x.ToDelete == false).FirstOrDefault();
                        if (validationRequest != null)
                        {
                            newPS.RequestedAt = validationRequest.RequestedAt;
                            newPS.SCObservations = validationRequest.SCObservations;
                            newPS.SEObservations = validationRequest.SEObservations;
                            if (validationRequest.IsComplete.Value && validationRequest.IsApproved.Value)
                            {
                                proposalPS2.ApprovedPrintingServices.Add(newPS);
                            }
                            else
                            {
                                if (!validationRequest.IsComplete.Value)
                                {
                                    proposalPS2.PendingServiceQuoteRequests.Add(newPS);
                                }
                            }
                        }
                        else //commented the else because it was adding reproved services on the approved services list
                        {
                            proposalPS2.ApprovedPrintingServices.Add(newPS);
                        }
                    }

                    //if(proposalPS2.ApprovedPrintingServices.Count != 0)
                    //{
                    proposalPS2.ApprovedPrintingServices = proposalPS2.ApprovedPrintingServices.OrderBy(x => x.IsPrecalc).ToList();
                    err.ProposalObj.Draft.printingServices2 = proposalPS2;
                    //}



                }


                //overvaluations
                err.ProposalObj.Draft.overvaluations = new List<Overvaluation>();
                List<BB_Proposal_Overvaluation> bb_Proposal_overlation = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == i.ProposalId).ToList();
                foreach (var item in bb_Proposal_overlation)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Overvaluation, Overvaluation>();
                    });

                    IMapper iMapper1 = config1.CreateMapper();

                    Overvaluation over = iMapper1.Map<BB_Proposal_Overvaluation, Overvaluation>(item);

                    err.ProposalObj.Draft.overvaluations.Add(over);
                }

                //Observcoes
                err.ProposalObj.Draft.details.CRObservations = new List<CRObservations>();
                List<BB_Proposal_Observations> bB_Proposal_Observations = db.BB_Proposal_Observations.Where(x => x.ProposalID == i.ProposalId).ToList();

                foreach (var obs in bB_Proposal_Observations)
                {
                    var configObs = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Observations, CRObservations>();
                    });

                    IMapper iMapperObs = configObs.CreateMapper();

                    CRObservations obs1 = iMapperObs.Map<BB_Proposal_Observations, CRObservations>(obs);

                    err.ProposalObj.Draft.details.CRObservations.Add(obs1);
                }


                //FINANCING
                err.ProposalObj.Draft.financing = new Financing();
                BB_Proposal_Financing bb_Proposal_financing = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                if (bb_Proposal_financing != null)
                {
                    var confiFinancing = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Financing, Financing>();
                    });

                    IMapper iMapperFinancing = confiFinancing.CreateMapper();

                    Financing f = iMapperFinancing.Map<BB_Proposal_Financing, Financing>(bb_Proposal_financing);

                    err.ProposalObj.Draft.financing = f;
                }
                //MONTHLY
                err.ProposalObj.Draft.financing.FinancingFactors = new FinancingFactors();
                err.ProposalObj.Draft.financing.FinancingFactors.Monthly = new List<Monthly>();
                List<BB_Proposal_FinancingMonthly> bb_Proposal_financingMonthly = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == i.ProposalId).ToList();
                foreach (var item in bb_Proposal_financingMonthly)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_FinancingMonthly, Monthly>();
                    });

                    IMapper iMapper1 = config1.CreateMapper();

                    Monthly over = iMapper1.Map<BB_Proposal_FinancingMonthly, Monthly>(item);

                    err.ProposalObj.Draft.financing.FinancingFactors.Monthly.Add(over);
                }

                //Trimestral
                err.ProposalObj.Draft.financing.FinancingFactors.Trimestral = new List<Trimestral>();
                List<BB_Proposal_FinancingTrimestral> bb_Proposal_financingTrimestral = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == i.ProposalId).ToList();
                foreach (var item in bb_Proposal_financingTrimestral)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_FinancingTrimestral, Trimestral>();
                    });

                    IMapper iMapper1 = config1.CreateMapper();

                    Trimestral over = iMapper1.Map<BB_Proposal_FinancingTrimestral, Trimestral>(item);

                    err.ProposalObj.Draft.financing.FinancingFactors.Trimestral.Add(over);
                }

                //Prazo Diferenciado
                BB_Proposal_PrazoDiferenciado bb_Proposal_PrazoDiferenciado = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                if (bb_Proposal_PrazoDiferenciado != null)
                {
                    err.ProposalObj.Draft.financing.diffTerm = new DiffTerm();
                    DiffTerm term = new DiffTerm();
                    term.financedNetsale = bb_Proposal_PrazoDiferenciado.ValorFinanciamento != null ? bb_Proposal_PrazoDiferenciado.ValorFinanciamento.GetValueOrDefault() : 0;
                    term.Factor = bb_Proposal_PrazoDiferenciado.ValorFactor != null ? bb_Proposal_PrazoDiferenciado.ValorFactor.GetValueOrDefault() : 0;
                    term.Months = bb_Proposal_PrazoDiferenciado.PrazoDiferenciado != null ? bb_Proposal_PrazoDiferenciado.PrazoDiferenciado.GetValueOrDefault() : 0;
                    term.Frequency = bb_Proposal_PrazoDiferenciado.Frequency != null ? bb_Proposal_PrazoDiferenciado.Frequency.GetValueOrDefault() : 1;
                    term.Rent = bb_Proposal_PrazoDiferenciado.ValorRenda != null ? bb_Proposal_PrazoDiferenciado.ValorRenda.GetValueOrDefault() : 0;
                    term.Comments = bb_Proposal_PrazoDiferenciado.Commets;
                    term.IsAproved = bb_Proposal_PrazoDiferenciado.IsAproved != null ? bb_Proposal_PrazoDiferenciado.IsAproved.GetValueOrDefault() : false;
                    term.Alocadora = bb_Proposal_PrazoDiferenciado.Alocadora;
                    term.IsComplete = bb_Proposal_PrazoDiferenciado.IsComplete;
                    err.ProposalObj.Draft.financing.diffTerm = term;
                }

                // Client Approval
                err.ProposalObj.ClientApproval = new ClientApproval();
                List<BB_Proposal_Contacts_Signing> signingContacts =
                    db.BB_Proposal_Contacts_Signing.Where(sc => sc.ProposalID == i.ProposalId).ToList();
                List<BB_Proposal_Contacts_Documentation> documentationContacts =
                    db.BB_Proposal_Contacts_Documentation.Where(cd => cd.ProposalID == i.ProposalId).ToList();

                if (signingContacts.Count == 0) signingContacts.Add(new BB_Proposal_Contacts_Signing());
                if (documentationContacts.Count == 0) documentationContacts.Add(new BB_Proposal_Contacts_Documentation());

                err.ProposalObj.ClientApproval.SigningContacts = signingContacts;
                err.ProposalObj.ClientApproval.DocumentationContacts = documentationContacts;

                if (proposal.CRM_QUOTE_ID != null)
                {
                    err.ProposalObj.ClientApproval.Documents = db.LD_DocumentProposal.Where(dp => dp.ProposalID == proposal.ID && dp.ClassificationID != 5).ToList();
                    err.ProposalObj.ClientApproval.DocumentTypes = db.LD_DocumentClassification.ToList();
                }

                err.ProposalObj.ClientApproval.Observations = db.LD_Contrato.Where(x => x.ProposalID == i.ProposalId).Select(x => x.ComentariosGC).FirstOrDefault();

                err.ProposalObj.Draft.deliveryLocations = new List<DeliveryLocation>();
                List<BB_Proposal_DeliveryLocation> bb_Proposal_DeliveryLocation = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == i.ProposalId).ToList();
                foreach (var item in bb_Proposal_DeliveryLocation)
                {
                    var configDelivery = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_DeliveryLocation, DeliveryLocation>();
                    });

                    IMapper iMapperDelivery = configDelivery.CreateMapper();

                    DeliveryLocation dl = iMapperDelivery.Map<BB_Proposal_DeliveryLocation, DeliveryLocation>(item);


                    List<BB_Proposal_ItemDoBasket> itemDoBasket = new List<BB_Proposal_ItemDoBasket>();
                    itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == item.IDX).ToList();

                    dl.items = new List<ItemDoBasket>();
                    foreach (var item1 in itemDoBasket)
                    {
                        var configItem = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<BB_Proposal_ItemDoBasket, ItemDoBasket>();
                        });

                        IMapper iMapperItem = configItem.CreateMapper();

                        ItemDoBasket a = iMapperItem.Map<BB_Proposal_ItemDoBasket, ItemDoBasket>(item1);
                        a.psConfig = new PsConfig();
                        a.counters = new List<Counter>();
                        dl.items.Add(a);
                    }

                    err.ProposalObj.Draft.deliveryLocations.Add(dl);
                }

                err.ProposalObj.Draft.consignment = new Consignment();

                BB_Proposal_Consignments bb_Proposal_Consignments = db.BB_Proposal_Consignments.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                if (bb_Proposal_Consignments != null)
                {
                    var configProposal_Consignments = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Consignments, Consignment>();
                    });

                    IMapper iMapperProposal_Consignments = configProposal_Consignments.CreateMapper();

                    Consignment consignment = iMapperProposal_Consignments.Map<BB_Proposal_Consignments, Consignment>(bb_Proposal_Consignments);

                    err.ProposalObj.Draft.consignment = consignment;
                }

                //BB_Maquinas_Usadas_Gestor
                err.ProposalObj.Draft.Maquinas_Usadas_Gestor = new List<Maquinas_Usadas_Gestor>();
                //List<BB_Maquinas_Usadas_Gestor> bb_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.ProposalID == i.ProposalId).ToList();
                List<BB_Maquinas_Usadas_Gestor> bb_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.ClienteNr == err.ProposalObj.Draft.client.accountnumber).ToList();
                foreach (var item in bb_Maquinas_Usadas_Gestor)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Maquinas_Usadas_Gestor, Maquinas_Usadas_Gestor>();
                    });

                    IMapper iMapper1 = config1.CreateMapper();

                    Maquinas_Usadas_Gestor m = iMapper1.Map<BB_Maquinas_Usadas_Gestor, Maquinas_Usadas_Gestor>(item);

                    m.Modelo = db.BB_Maquinas_Usadas.Where(x => x.NUS_Referencia == item.Codref).Select(x => x.NUS_Modelo).FirstOrDefault();

                    err.ProposalObj.Draft.Maquinas_Usadas_Gestor.Add(m);
                }

                //BB_Permissions
                err.ProposalObj.Draft.shareProfileDelegation = new List<BB_Permissions>();
                List<BB_Permissions> bB_Permissions = db.BB_Permissions.Where(x => x.ProposalID == proposal.ID).Where(x => x.ToDelete == false).ToList();
                if (bB_Permissions != null)
                {
                    err.ProposalObj.Draft.shareProfileDelegation = bB_Permissions;
                }
                else
                {
                    err.ProposalObj.Draft.shareProfileDelegation = new List<BB_Permissions>();
                }

                BB_TypeOfClient typeOfClient = db.BB_TypeOfClient.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                if (typeOfClient != null)
                {
                    err.ProposalObj.Draft.baskets.prospect = typeOfClient.Prospect;
                    err.ProposalObj.Draft.baskets.newBusinessLine = typeOfClient.NewBusinessLine;
                    err.ProposalObj.Draft.baskets.GMA = typeOfClient.GMA;
                    err.ProposalObj.Draft.baskets.BEUSupport = typeOfClient.BEUSupport;
                }

                err.ProposalObj.Draft.baskets.IsNP = proposal.IsNP ?? false;


                // Validacoes "NO PRODUCCION" ----------------------
                bool? isNP = db.BB_Proposal.Where(x => x.ID == i.ProposalId).Select(x => x.IsNP).FirstOrDefault();

                BB_WFA_NP_Approvers approval = db.BB_WFA_NP_Approvers.Where(x => x.ProposalID == i.ProposalId).OrderByDescending(x => x.ID).FirstOrDefault();

                // Se já existir um approver para este proposalID e que tenha aprovado ou reprovado..
                // se já estiver APROVADO, coloco o IsNP a false <=> processo NAO FICA bloqueado
                // se já estiver REPROVADO, coloco o IsNP a true <=> processo FICA bloqueado
                if (approval != null && approval.IsApproved != null)
                {
                    err.ProposalObj.Draft.baskets.IsPassedNP = approval.IsApproved.Value;
                }
                else
                {
                    if (isNP == true)
                    {
                        err.ProposalObj.Draft.baskets.IsPassedNP = false;
                    }
                    else
                    {
                        err.ProposalObj.Draft.baskets.IsPassedNP = true;
                    }
                }
                // ------------------------------------------------



                //LD_DocumentProposal - Contractos
                err.ProposalObj.Draft.contracts = new BusinessContract();
                List<LD_DocumentProposal> contractDocs = db.LD_DocumentProposal.Where(x => x.ProposalID == proposal.ID && x.ClassificationID == 5).ToList();
                if (contractDocs != null)
                {
                    err.ProposalObj.Draft.contracts.contractDocs = contractDocs;
                }

            }
            catch (Exception ex)
            {
                err.Message = ex.Message.ToString();
                err.InnerException = ex.InnerException.ToString();
            }

            //err.ProposalObj.Draft.details.ModifiedBy = "ana.vaz@konicaminolta.pt";
            //err.ProposalObj.Draft.details.AccountManager = "ana.vaz@konicaminolta.pt";

            return err;
        }

        public ActionResponse ProposalDraftSaveAs(ProposalRootObject p)
        {
            ActionResponse err = new ActionResponse();
            int originalID = p.Draft.details.ID;
            int ProposalID = 0;
            try
            {
                bool? IsMultipleContract = null;

                IsMultipleContract = p.Draft.details.IsMultipleContract ?? false;
                DateTime? createDatetime = p.Draft.details.CreatedTime <= DateTime.Parse("01/01/2000") ? DateTime.Now : p.Draft.details.CreatedTime;
                BB_Proposal bb_proposal = new BB_Proposal()
                {
                    AccountManager = p.Draft.details.AccountManager,
                    CampaignID = p.Draft.details.CampaignID,
                    ClientAccountNumber = p.Draft.client.accountnumber,
                    CRM_QUOTE_ID = p.Draft.details.CRM_QUOTE_ID,
                    Description = p.Draft.details.Description,
                    CreatedBy = p.Draft.details.CreatedBy,
                    CreatedTime = createDatetime,
                    ModifiedBy = p.Draft.details.CreatedBy,
                    ModifiedTime = DateTime.Now,
                    Name = p.Draft.details.Name,
                    StatusID = 1,
                    ToDelete = false,
                    ValueTotal = p.Summary.businessTotal,
                    IsMultipleContract = IsMultipleContract,
                    IsNP = p.Draft.baskets.IsNP,
                    ContractNumberPai = p.Draft.details.ExistanteContractNumber
                };


                if (bb_proposal != null)
                {
                    log4net.ThreadContext.Properties["proposal_id"] = bb_proposal.ID;
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(bb_proposal);
                    Exception message = new Exception("Nova Proposta");
                    log.Info(json, message);
                }


                db.BB_Proposal.Add(bb_proposal);
                try
                {
                    db.SaveChanges();
                    p.Draft.details.ID = bb_proposal.ID;
                    BB_Proposal_Status dbStatus = db.BB_Proposal_Status.Where(x => x.ID == bb_proposal.StatusID).FirstOrDefault();
                    if (dbStatus != null)
                    {
                        p.Draft.details.Status = new ProposalStatus
                        {
                            IsEdit = dbStatus.BB_Edit,
                            Name = dbStatus.Description,
                            Phase = dbStatus.Phase
                        };
                    }
                }
                catch (Exception e)
                {
                    e.Message.ToString();
                }

                ProposalID = bb_proposal.ID;
                if (ProposalID != 0)
                {
                    //BB_PROPOSAL_QUOTE
                    List<BB_Maquinas_Usadas_Gestor> lstmaquinsaudasGestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.ProposalID == ProposalID).ToList();
                    foreach (var item in lstmaquinsaudasGestor)
                    {
                        item.ProposalID = null;
                        item.IsReserved = false;
                        db.Entry(item).State = item.ID == 0 ? EntityState.Added : EntityState.Modified;
                        db.SaveChanges();
                    }
                    foreach (var _Quote in p.Draft.baskets.os_basket)
                    {

                        var config1 = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<OsBasket, BB_Proposal_Quote>();
                        });

                        IMapper iMapper1 = config1.CreateMapper();

                        BB_Proposal_Quote quote = iMapper1.Map<OsBasket, BB_Proposal_Quote>(_Quote);

                        quote.Proposal_ID = ProposalID;
                        quote.CreatedBy = p.Draft.details.CreatedBy;
                        quote.CreatedTime = DateTime.Now;
                        quote.ModifiedBy = p.Draft.details.CreatedBy;
                        quote.ModifiedTime = DateTime.Now;

                        db.BB_Proposal_Quote.Add(quote);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }

                        //BB_PROPOSAL_Counters

                        if (_Quote.counters != null)
                        {
                            foreach (var counter in _Quote.counters)
                            {
                                var config10 = new MapperConfiguration(cfg =>
                                {
                                    cfg.CreateMap<Counter, BB_Proposal_Counters>();
                                });

                                IMapper iMapper10 = config10.CreateMapper();

                                BB_Proposal_Counters counters = iMapper10.Map<Counter, BB_Proposal_Counters>(counter);

                                counters.ProposalID = bb_proposal.ID;
                                counters.OSID = quote.ID;
                                db.BB_Proposal_Counters.Add(counters);
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString();
                                }
                                BB_Maquinas_Usadas_Gestor g = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == counter.serialNumber).FirstOrDefault();
                                if (g != null)
                                {
                                    g.ProposalID = ProposalID;
                                    g.IsReserved = true;
                                    db.Entry(g).State = g.ID == 0 ? EntityState.Added : EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        //PS_CONFIG
                        if (_Quote.psConfig != null)
                        {
                            var configPSConfig = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<PsConfig, BB_Proposal_PsConfig>();
                            });

                            IMapper iMapperPSConfig = configPSConfig.CreateMapper();

                            BB_Proposal_PsConfig psconfig = iMapperPSConfig.Map<PsConfig, BB_Proposal_PsConfig>(_Quote.psConfig);

                            psconfig.ProposalID = bb_proposal.ID;
                            psconfig.ItemID = quote.ID;
                            db.BB_Proposal_PsConfig.Add(psconfig);
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

                    //BB_PROPOSAL_QUOTE_RS
                    foreach (var _Quote in p.Draft.baskets.rs_basket)
                    {

                        var config2 = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<RsBasket, BB_Proposal_Quote_RS>();
                        });

                        IMapper iMapper2 = config2.CreateMapper();

                        BB_Proposal_Quote_RS quote = iMapper2.Map<RsBasket, BB_Proposal_Quote_RS>(_Quote);

                        quote.ProposalID = ProposalID;

                        db.BB_Proposal_Quote_RS.Add(quote);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    OPSPacks opsPacks = p.Draft.opsPacks;
                    List<OPSImplement> draftImplements = opsPacks.opsImplement.ToList();
                    int opsImplementPosition = 0;
                    foreach (OPSImplement opsI in draftImplements)
                    {
                        BB_Proposal_OPSImplement newOPSI = new BB_Proposal_OPSImplement
                        {
                            CodeRef = opsI.CodeRef,
                            Description = opsI.Description,
                            Family = opsI.Family,
                            InCatalog = opsI.InCatalog,
                            IsFinanced = opsI.IsFinanced,
                            MaxRange = opsI.MaxRange,
                            MinRange = opsI.MinRange,
                            Name = opsI.Name,
                            Position = opsImplementPosition,
                            PVP = opsI.PVP,
                            ProposalID = ProposalID,
                            Quantity = opsI.Quantity,
                            Type = opsI.Type,
                            IsValidated = opsI.IsValidated
                        };
                        if (opsI != null)
                        {
                            newOPSI.UnitDiscountPrice = opsI.UnitDiscountPrice;
                        }
                        db.BB_Proposal_OPSImplement.Add(newOPSI);
                        try
                        {
                            db.SaveChanges();
                            opsI.ID = newOPSI.ID;
                            opsPacks.opsImplement[opsImplementPosition].ID = newOPSI.ID;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                        opsImplementPosition++;
                    }

                    List<OPSManage> draftManages = new List<OPSManage>();
                    draftManages = opsPacks.opsManage.ToList();
                    int opsManagePosition = 0;
                    foreach (OPSManage opsM in draftManages)
                    {
                        BB_Proposal_OPSManage newOPSM = new BB_Proposal_OPSManage
                        {
                            CodeRef = opsM.CodeRef,
                            Description = opsM.Description,
                            Family = opsM.Family,
                            InCatalog = opsM.InCatalog,
                            MaxRange = opsM.MaxRange,
                            MinRange = opsM.MinRange,
                            Name = opsM.Name,
                            Position = opsManagePosition,
                            PVP = opsM.PVP,
                            ProposalID = ProposalID,
                            Quantity = opsM.Quantity,
                            TotalMonths = opsM.TotalMonths,
                            Type = opsM.Type,
                            UnitDiscountPrice = opsM.UnitDiscountPrice,
                            IsValidated = opsM.IsValidated
                        };
                        if (opsM != null)
                        {
                            newOPSM.UnitDiscountPrice = opsM.UnitDiscountPrice;
                        }
                        db.BB_Proposal_OPSManage.Add(newOPSM);
                        try
                        {
                            db.SaveChanges();
                            opsPacks.opsManage[opsManagePosition].ID = newOPSM.ID;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                        opsManagePosition++;
                    }

                    //BB_PROPOSAL_QUOTE_Financing
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Financing, BB_Proposal_Financing>();
                    });

                    IMapper iMapper = config.CreateMapper();

                    BB_Proposal_Financing fin = iMapper.Map<Financing, BB_Proposal_Financing>(p.Draft.financing);

                    fin.ProposalID = ProposalID;
                    fin.AmountFinanced = Math.Round((double)fin.AmountFinanced, 2);
                    fin.AmountNotFinanced = Math.Round((double)fin.AmountNotFinanced, 2);
                    fin.MonthlyIncome = Math.Round((double)fin.MonthlyIncome, 2);

                    db.BB_Proposal_Financing.Add(fin);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }


                    //BB_PROPOSAL_QUOTE_FinancingFactores Monthly
                    foreach (var monthly in p.Draft.financing.FinancingFactors.Monthly)
                    {

                        var configmonthly = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Monthly, BB_Proposal_FinancingMonthly>();
                        });

                        IMapper iMappermonthly = configmonthly.CreateMapper();

                        BB_Proposal_FinancingMonthly m1 = iMappermonthly.Map<Monthly, BB_Proposal_FinancingMonthly>(monthly);

                        m1.ProposalID = ProposalID;
                        m1.FinancingID = fin.ID;

                        db.BB_Proposal_FinancingMonthly.Add(m1);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    //BB_PROPOSAL_QUOTE_FinancingFactores Trimestral
                    foreach (var trimestral in p.Draft.financing.FinancingFactors.Trimestral)
                    {

                        var configmonthly = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Trimestral, BB_Proposal_FinancingTrimestral>();
                        });

                        IMapper iMappermonthly = configmonthly.CreateMapper();

                        BB_Proposal_FinancingTrimestral t1 = iMappermonthly.Map<Trimestral, BB_Proposal_FinancingTrimestral>(trimestral);

                        t1.ProposalID = ProposalID;
                        t1.FinancingID = fin.ID;

                        db.BB_Proposal_FinancingTrimestral.Add(t1);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    //BB_PROPOSAL_Overvaluation
                    foreach (var _overvaluation in p.Draft.overvaluations)
                    {

                        var config3 = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Overvaluation, BB_Proposal_Overvaluation>();
                        });

                        IMapper iMapper3 = config3.CreateMapper();

                        BB_Proposal_Overvaluation overvaluation111 = iMapper3.Map<Overvaluation, BB_Proposal_Overvaluation>(_overvaluation);

                        overvaluation111.ProposalID = ProposalID;

                        db.BB_Proposal_Overvaluation.Add(overvaluation111);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    //BB_PROPOSAL_Commission --------------------------------------------------------
                    var config4 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Commission, BB_Proposal_Commission>();
                    });

                    IMapper iMapper4 = config4.CreateMapper();

                    BB_Proposal_Commission commission1 = iMapper4.Map<Commission, BB_Proposal_Commission>(p.Summary.commission);

                    commission1.ProposalID = ProposalID;

                    db.BB_Proposal_Commission.Add(commission1);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }

                    // SAVE DAS RETOMAS --------------------------------------------------------

                    SaveUpturns(p.Draft.upturns.upturns, ProposalID, p.Draft.client.accountnumber);

                    p.Draft.upturns.upturns = db.BB_Proposal_Upturn.Where(x => x.ProposalID == ProposalID).ToList();



                    //PRINTING SERVICES --------------------------------------------------------

                    // ISTO ESTÁ A SER UTILIZADO???? EU ACHO QUE NAO
                    if (p.Draft.printingServices != null)
                    {
                        var configpPrintingServices = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<PrintingServices, BB_Proposal_PrintingServices>();
                        });

                        IMapper iMapperPrintinfServices = configpPrintingServices.CreateMapper();

                        BB_Proposal_PrintingServices printingService = iMapperPrintinfServices.Map<PrintingServices, BB_Proposal_PrintingServices>(p.Draft.printingServices);

                        printingService.ProposalID = ProposalID;

                        db.BB_Proposal_PrintingServices.Add(printingService);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }

                        //VVA
                        if (p.Draft.printingServices.vva != null)
                        {
                            var configpVVA = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<Vva, BB_Proposal_Vva>();
                            });

                            IMapper iMapperVVA = configpVVA.CreateMapper();

                            BB_Proposal_Vva vva = iMapperVVA.Map<Vva, BB_Proposal_Vva>(p.Draft.printingServices.vva);

                            vva.ProposalID = ProposalID;


                            db.BB_Proposal_Vva.Add(vva);
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

                    try
                    {
                        PrintingServices2 printingServices2 = p.Draft.printingServices2;
                        //int ps2Id = 0;
                        if (printingServices2 != null)
                        {
                            AddNewPrintingService(p.Draft.printingServices2, ProposalID);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }



                    //BB_PROPOSAL_Cliente --------------------------------------------------------
                    BB_Proposal_Client cliente1 = new BB_Proposal_Client();
                    cliente1.ClientID = p.Draft.client.accountnumber;
                    cliente1.IsNewClient = p.Draft.client.isNewClient;
                    cliente1.ProposalID = ProposalID;
                    cliente1.Name = p.Draft.client.Name;
                    cliente1.IsPublicSector = p.Draft.client.isPublicSector;
                    cliente1.IsGMA = p.Draft.client.isGMA;
                    db.BB_Proposal_Client.Add(cliente1);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    if (p.Draft.client.modeId.GetValueOrDefault() == 1)
                    {
                        BB_Clientes count = db.BB_Clientes.Where(x => x.accountnumber == p.Draft.client.accountnumber).FirstOrDefault();
                        if (count == null)
                        {
                            string usename = "";
                            using (var db1 = new masterEntities())
                            {
                                usename = db1.AspNetUsers.Where(x => x.Email == p.Draft.details.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                            }
                            BB_Clientes c = new BB_Clientes();
                            c.accountnumber = "P2_BB_" + (p.Draft.client.NIF != "" ? p.Draft.client.NIF : DateTime.Now.ToString());
                            c.Name = p.Draft.client.Name;
                            c.PostalCode = p.Draft.client.PostalCode;
                            c.NIF = p.Draft.client.NIF;
                            c.City = p.Draft.client.City;
                            c.address1_line1 = p.Draft.client.address1_line1;
                            c.IsClienteBB = true;
                            c.Owner = usename;
                            db.BB_Clientes.Add(c);
                            db.SaveChanges();
                        }
                    }
                }

                //BB_PROPOSAL_COnsigments --------------------------------------------------------
                var configConsigments = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Consignment, BB_Proposal_Consignments>();
                });

                IMapper iMapperConsigments = configConsigments.CreateMapper();

                BB_Proposal_Consignments consignment = iMapperConsigments.Map<Consignment, BB_Proposal_Consignments>(p.Draft.consignment);

                consignment.ProposalID = ProposalID;

                db.BB_Proposal_Consignments.Add(consignment);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

                //TYPE OF CLIENT
                BB_TypeOfClient typeOfClient = db.BB_TypeOfClient.Where(x => x.ProposalID == p.Draft.details.ID).FirstOrDefault();
                if (typeOfClient is null)
                {
                    typeOfClient = new BB_TypeOfClient();
                }
                typeOfClient.ProposalID = p.Draft.details.ID;
                typeOfClient.Prospect = p.Draft.baskets.prospect;
                typeOfClient.NewBusinessLine = p.Draft.baskets.newBusinessLine;
                typeOfClient.GMA = p.Draft.baskets.GMA;
                typeOfClient.BEUSupport = p.Draft.baskets.BEUSupport;

                try
                {
                    db.BB_TypeOfClient.AddOrUpdate(typeOfClient);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (p.Draft.printingServices2.PrintingCondition != 0)
                {
                    BB_Proposal_Condition_Type printingConditionType = new BB_Proposal_Condition_Type()
                    {
                        ProposalID = p.Draft.details.ID,
                        ConditionType = "ZVBS",
                        ConditionValue = p.Draft.printingServices2.PrintingCondition
                    };

                    try
                    {
                        db.BB_Proposal_Condition_Type.AddOrUpdate(printingConditionType);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }


            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            err.ProposalObj = new ProposalRootObject();
            err.ProposalObj.Draft = p.Draft;
            err.ProposalObj.Draft.baskets.IsPassedNP = false;
            return err;
        }

        private void CreateContactsDocumentation(ProposalRootObject p, int ProposalID)
        {
            if (p.ClientApproval == null) return;
            try
            {
                bool encontrouSigin = db.BB_Proposal_Contacts_Documentation.Any(x => x.ProposalID == ProposalID);
                if (encontrouSigin)
                {
                    List<BB_Proposal_Contacts_Documentation> toRemove = db.BB_Proposal_Contacts_Documentation.Where(x => x.ProposalID == ProposalID).ToList();
                    db.BB_Proposal_Contacts_Documentation.RemoveRange(toRemove);
                    db.SaveChanges();

                }

                List<BB_Proposal_Contacts_Documentation> lstContactsDoc = p.ClientApproval.DocumentationContacts;
                if (lstContactsDoc.Count > 0 && lstContactsDoc[0].Email != "" && lstContactsDoc[0].Name != "" && lstContactsDoc[0].Telefone != "")
                {
                    foreach (var ContactSign in lstContactsDoc)
                    {
                        BB_Proposal_Contacts_Documentation ca = new BB_Proposal_Contacts_Documentation();

                        ca.Email = ContactSign.Email;
                        ca.Name = ContactSign.Name;
                        ca.Telefone = ContactSign.Telefone;
                        ca.ProposalID = ProposalID;
                        db.BB_Proposal_Contacts_Documentation.Add(ca);
                    }

                    db.SaveChanges();
                }


                encontrouSigin = db.BB_Proposal_Contacts_Signing.Any(x => x.ProposalID == ProposalID);
                if (encontrouSigin)
                {
                    List<BB_Proposal_Contacts_Signing> toRemove = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == ProposalID).ToList();
                    db.BB_Proposal_Contacts_Signing.RemoveRange(toRemove);
                    db.SaveChanges();

                }

                List<BB_Proposal_Contacts_Signing> lstSigningContactsDoc = p.ClientApproval.SigningContacts;
                if (lstSigningContactsDoc.Count > 0 && lstSigningContactsDoc[0].Email != ""
                        && lstSigningContactsDoc[0].Name != "" && lstSigningContactsDoc[0].Telefone != "")
                {
                    foreach (var ContactSign in lstSigningContactsDoc)
                    {
                        BB_Proposal_Contacts_Signing ca = new BB_Proposal_Contacts_Signing();

                        ca.Email = ContactSign.Email;
                        ca.Name = ContactSign.Name;
                        ca.Telefone = ContactSign.Telefone;
                        ca.ProposalID = ProposalID;
                        db.BB_Proposal_Contacts_Signing.Add(ca);
                    }

                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw ex;
            }
        }

        //private void CreateDocuments(ProposalRootObject p, string quote)
        //{

        //    if (p == null) return;
        //    try
        //    {
        //        bool existDocs = db.LD_DocumentProposal.Any(x => x.QuoteNumber == quote);
        //        if (existDocs)
        //        {
        //            List<LD_DocumentProposal> toRemove = db.LD_DocumentProposal.Where(x => x.QuoteNumber == quote).ToList();
        //            db.LD_DocumentProposal.RemoveRange(toRemove);
        //            db.SaveChanges();

        //        }

        //        List<LD_DocumentProposal> docs = p.ClientApproval.Documents;
        //        foreach (LD_DocumentProposal doc in docs)
        //        {
        //            doc.
        //            doc.CreatedTime = DateTime.Now;
        //            doc.SystemID = 1;
        //            doc.DocumentIsProcess = false;
        //            doc.DocumentIsValid = false;

        //            db.LD_DocumentProposal.Add(doc);
        //        }

        //        db.SaveChanges();

        //    } catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public ProposalRootObject GetProposta(int? proposalID)
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
            BB_Clientes cliente = new BB_Clientes();
            BB_Proposal_Client pCliente = new BB_Proposal_Client();

            List<BB_Equipamentos> bb_Equipamentos = new List<BB_Equipamentos>();

            using (var db = new BB_DB_DEVEntities2())
            {
                bb_Equipamentos = db.BB_Equipamentos.ToList();

                pr1 = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                cliente = db.BB_Clientes.Where(x => x.accountnumber == pr1.ClientAccountNumber).FirstOrDefault();
                pCliente = db.BB_Proposal_Client.Where(x => x.ProposalID == pr1.ID).FirstOrDefault();
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

                a.ProposalObj.Draft.financingDetails = new FinancingDetails();
                string contractType = db.BB_FinancingContractType.Where(x => x.ID == a.ProposalObj.Draft.financing.ContractTypeId).Select(x => x.Company).FirstOrDefault();
                a.ProposalObj.Draft.financingDetails.ContractType = contractType;
                string financingType = db.BB_FinancingType.Where(x => x.Code == a.ProposalObj.Draft.financing.FinancingTypeCode).Select(x => x.Type).FirstOrDefault();
                a.ProposalObj.Draft.financingDetails.FinancingType = financingType;
            }

            a.ProposalObj.Draft.financing.DateApproval = a.ProposalObj.Draft.financing.DateApproval;

            //sobrevalorizacao = (sobrevalorizacao + retomas) - retomas;

            //a.ProposalObj.Draft.details.ValueTotal = a.ProposalObj.Draft.details.ValueTotal + LeiDaCopiaPriada.Value + sobrevalorizacao.Value;
            a.ProposalObj.Draft.details.ValueTotal = a.ProposalObj.Draft.details.ValueTotal;

            double? sobrevalorizacao1 = 0;
            sobrevalorizacao1 = sobrevalorizacao - retomas;
            //double? sobrevalorizacao = 0;
            //sobrevalorizacao = a.ProposalObj.Draft.overvaluations.Select(x => x.Total).FirstOrDefault();
            double? OPSHWvalorTotal = 0;
            double? OPSHWUnti = 0;
            //if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0 && sobrevalorizacao1 > 0)
            //{
            //    if (a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").Count() > 0)
            //    {


            //        OPSHWvalorTotal = a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(c => c.TotalNetsale)).First();

            //        OPSHWUnti = a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(c => c.UnitDiscountPrice)).First();
            //    }
            //}

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

                    vt.VVA = Math.Round(activePS.GlobalClickVVA.PVP, 5);
                    activePS.GlobalClickVVA.BWExcessPVP = Math.Round(activePS.GlobalClickVVA.BWExcessPVP, 5);
                    activePS.GlobalClickVVA.CExcessPVP = Math.Round(activePS.GlobalClickVVA.CExcessPVP, 5);
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

                foreach (var machine in activePS.Machines)
                {
                    machine.BWCost = bb_Equipamentos.Where(x => x.CodeRef == machine.CodeRef).Select(x => x.BWBaseCost).FirstOrDefault();
                    machine.CCost = bb_Equipamentos.Where(x => x.CodeRef == machine.CodeRef).Select(x => x.CBaseCost).FirstOrDefault();

                    machine.BWCost = machine.BWCost.HasValue ? Math.Round(machine.BWCost.Value, 5) : (double?)0;
                    machine.CCost = machine.CCost.HasValue ? Math.Round(machine.CCost.Value, 5) : (double?)0;

                    machine.ClickPriceBW = machine.ClickPriceBW.HasValue ? Math.Round(machine.ClickPriceBW.Value, 5) : (double?)0;
                    machine.ClickPriceC = machine.ClickPriceC.HasValue ? Math.Round(machine.ClickPriceC.Value, 5) : (double?)0;

                    machine.RequestedBWClickPrice = machine.RequestedBWClickPrice != null ? Math.Round((double)machine.RequestedBWClickPrice, 5) : 0;
                    machine.RequestedCClickPrice = machine.RequestedCClickPrice != null ? Math.Round((double)machine.RequestedCClickPrice, 5) : 0;


                    machine.ApprovedBW = machine.ApprovedBW.HasValue ? Math.Round(machine.ApprovedBW.Value, 5) : (double?)0;
                    machine.ApprovedC = machine.ApprovedC.HasValue ? Math.Round(machine.ApprovedC.Value, 5) : (double?)0;

                }

            }

            if (retomas.Value > 0)
            {
                a.ProposalObj.Draft.details.ValueTotal -= retomas.Value;
            }

            //a.ProposalObj.Draft.details.ValueTotal = pr1 != null && pr1.SubTotal != null ? pr1.SubTotal.Value : a.ProposalObj.Draft.details.ValueTotal;

            if (a.ProposalObj.Draft.baskets.rs_basket.Count() > 0 || a.ProposalObj.Draft.opsPacks.opsManage.Count() > 0)
            {
                vt.ServicosRecorentesMes = a.ProposalObj.Draft.baskets.rs_basket.Sum(x => x.MonthlyFee) + a.ProposalObj.Draft.opsPacks.opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity);
                vt.ServicosRecorentesMes = Math.Round((double)vt.ServicosRecorentesMes, 2);
            }
            else
            {
                vt.ServicosRecorentesMes = 0;
            }
            if (a.ProposalObj.Draft.baskets.rs_basket.Count() > 0 || a.ProposalObj.Draft.opsPacks.opsManage.Count() > 0)
            {
                vt.ServicosRecorentesTotal = a.ProposalObj.Draft.baskets.rs_basket.Sum(x => x.TotalNetsale) + a.ProposalObj.Draft.opsPacks.opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity * x.TotalMonths);
                vt.ServicosRecorentesTotal = Math.Round((double)vt.ServicosRecorentesTotal, 2);
            }
            else
            {
                vt.ServicosRecorentesTotal = 0;
            }


            vt.ConfiguracaoOneShotValor = Math.Round((double)(a.ProposalObj.Draft.details.ValueTotal - vt.ServicosRecorentesTotal), 2);

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
            vt.RendaTotal = Math.Round((double)(vt.VVA + vt.RendaFinanciada + vt.ServicosRecorentesMes + fee), 2);
            if (prazoDiferenciado1 != null && prazoDiferenciado1.FinancingID == 6)
            {
                vt.RendaTotal = vt.RendaFinanciada;
            }
            vt.sobrevalorizacaoTotal = sobrevalorizacao != null && sobrevalorizacao.HasValue && sobrevalorizacao.Value != 0 ? sobrevalorizacao.Value : 0;
            vt.retomasTotal = retomas != null && retomas.HasValue && retomas.Value != 0 ? retomas.Value : 0;
            a.ProposalObj.valoretotais = vt;

            LeaseDeskBLL lDBll = new LeaseDeskBLL();

            ConditionsTotais condPvp = lDBll.FinancingDetailsPerMachine(proposalID);

            a.ProposalObj.ConditionsPvpPerMachine = condPvp;

            //foreach(var manage in a.ProposalObj.Draft.opsPacks.opsManage)
            //{
            //    manage.UnitDiscountPrice = Math.Round((double)manage.UnitDiscountPrice, 3);
            //}

            List<HW_SW> configuratorInfo = GetGroupedConfigurator(proposalID);

            a.ProposalObj.Draft.configuratorInfo = configuratorInfo;
            string delegation = "";
            if (cliente != null)
            {
                switch (cliente.Territory.Substring(4, 4))
                {
                    case "5241":
                        delegation = "Barcelona";
                        break;
                    case "5203":
                        delegation = "Sales Area 3";
                        break;
                    case "5243":
                        delegation = "Algaciras";
                        break;
                    case "5200":
                        delegation = "Sales Area 0";
                        break;
                    case "5248":
                        delegation = "Sevilla";
                        break;
                    case "5245":
                        delegation = "Cádiz";
                        break;
                    case "5246":
                        delegation = "Málaga";
                        break;
                    case "5242":
                        delegation = "Valencia";
                        break;
                    case "5247":
                        delegation = "Santander";
                        break;
                    case "5204":
                        delegation = "Sales Area 4";
                        break;
                    case "5220":
                        delegation = "Dealer Service";
                        break;
                    case "5201":
                        delegation = "Sales Area 1";
                        break;
                    case "5202":
                        delegation = "Sales Area 2";
                        break;
                    case "5244":
                        delegation = "Bilbao";
                        break;
                    default:
                        delegation = "Madrid";
                        break;
                }
            }

            string office = "";
            if (cliente != null)
            {
                switch (cliente.Territory.Substring(8, 3))
                {
                    case "543":
                        office = "Production Printing";
                        break;
                    case "5VT":
                        office = "Inside Sales";
                        break;
                    case "521":
                        office = "PREMIUM";
                        break;
                    case "522":
                        office = "ADVANCED";
                        break;
                    case "523":
                        office = "PARTNER";
                        break;
                    case "540":
                        office = "Regular Customers";
                        break;
                    case "520":
                        office = "ELITE";
                        break;
                    case "544":
                        office = "Industrial Printing";
                        break;
                    case "541":
                        office = "Major Accounts";
                        break;
                    case "542":
                        office = "Production Printing";
                        break;

                }
            }


            a.ProposalObj.Draft.client.SalesGroup = cliente != null ? cliente.Territory.Substring(4, 4) + " - " + delegation : "-";
            a.ProposalObj.Draft.client.SalesOffice = cliente != null ? cliente.Territory.Substring(8, 3) + " - " + office : "-";

            a.ProposalObj.SAPNumber = pr1.Pedido_SAP == null ? "" : pr1.Pedido_SAP.Value.ToString();
            a.ProposalObj.IsClientPublicSector = (bool)pCliente.IsPublicSector;

            using (var dbMaster = new masterEntities())
            {

                //AspNetUsers user = dbMaster.AspNetUsers.Contains(x => x.Territory == cliente.Territory).FirstOrDefault();
                //BES-52425VTA;BES-52435VTA;BES-52475VTA
                AspNetUsers user = dbMaster.AspNetUsers.Where(x => x.Territory.Contains(cliente.Territory)).FirstOrDefault();

                if (user != null)
                {
                    a.ProposalObj.Draft.client.GestorCuenta = user.DisplayName;
                }
                else
                {
                    a.ProposalObj.Draft.client.GestorCuenta = cliente.Owner;
                }



            }

            return a.ProposalObj;
        }

        public List<HW_SW> GetGroupedConfigurator(int? proposalID)
        {
            List<HW_SW> configurator = new List<HW_SW>();

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    //List<BB_Proposal_Quote> pp_quote = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                    List<BB_Proposal_Quote_RS> pp_quote_rs = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).ToList();
                    List<BB_Proposal_OPSManage> opsManage_lst = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).ToList();

                    List<BB_Equipamentos> equipamentos = db.BB_Equipamentos.ToList();

                    HashSet<string> equipamentosCodeRefs = equipamentos
                    .Select(e => e.CodeRef)
                    .ToHashSet();

                    HashSet<int> locations_IDX = db.BB_Proposal_DeliveryLocation
                        .Where(x => x.ProposalID == proposalID && x.AccountType == "Ship To")
                        .Select(x => x.IDX)
                        .ToHashSet(); // Melhor performance do que List para Contains()

                    // todos os ItemDoBaskets que sejam Ship To
                    List<BB_Proposal_ItemDoBasket> itemsDoBasket_lst = db.BB_Proposal_ItemDoBasket
                        .Where(x => locations_IDX.Contains((int)x.DeliveryLocationID))
                        .ToList();

                    List<BB_Proposal_DeliveryLocationResumoModel> DeliveriesSummary_lst = PontosDeEnvioResumo(proposalID);

                    foreach (var item in itemsDoBasket_lst)
                    {
                        HW_SW element = new HW_SW();

                        // se o codeRef do ItemDoBasket existir dentro dos equipamentos
                        // significa que é uma máquina e entao, vou criar um HW_SW
                        if (equipamentosCodeRefs.Contains(item.CodeRef) || item.Description.Contains("MAIN MATERIAL"))
                        {
                            element.Family = item.Family;
                            element.CodeRef = item.CodeRef;
                            element.Description = item.Description;
                            element.Group = item.Group;
                            element.UnitDiscountPrice = (double)item.UnitDiscountPrice;
                            element.Qty = (int)item.Qty;
                            element.TotalNetsale = (double)item.TotalNetsale;
                            element.IsUsed = (bool)item.IsUsedMachine;
                            element.GroupPrice = item.UnitDiscountPrice;
                            element.Accessories = new List<OsBasket>();
                            element.DeliverySummary = DeliveriesSummary_lst.Where(x => x.Group == element.Group).FirstOrDefault();
                            element.SerialNumber = item.SerialNumber != null ? item.SerialNumber : "-";
                            configurator.Add(element);
                        }
                    };


                    foreach (var configMissingItems in configurator)
                    {
                        // Lista dos ACESSORIOS de apenas 1 grupo em especifico
                        List<BB_Proposal_ItemDoBasket> groupListFiltered = itemsDoBasket_lst.Where(g => g.Group == configMissingItems.Group
                                                                                                        && g.CodeRef != configMissingItems.CodeRef
                                                                                                        && g.Description != "MAIN MATERIAL"
                                                                                             ).ToList();

                        foreach (var item in groupListFiltered)
                        {
                            // Acessórios
                            HW_SW HW_SW_GroupX = configurator.Where(x => x.Group == item.Group).FirstOrDefault();

                            bool isRS = pp_quote_rs.Any(x => x.CodeRef == item.CodeRef);

                            bool isOPSPackage = opsManage_lst.Any(x => x.CodeRef == item.CodeRef && x.UnitDiscountPrice != 0);

                            // SERVICO RECURRENTE
                            if (isRS == true)
                            {
                                BB_Proposal_Quote_RS quoteRS = pp_quote_rs.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                OsBasket basketItem = new OsBasket
                                {
                                    CodeRef = item.CodeRef,
                                    Description = item.Description,
                                    Family = item.Family,
                                    UnitDiscountPrice = (double)(item.UnitDiscountPrice * quoteRS.TotalMonths),
                                    Qty = (int)item.Qty,
                                    TotalNetsale = (double)(item.UnitDiscountPrice * quoteRS.TotalMonths),
                                    Group = item.Group,
                                    IsUsed = (bool)item.IsUsedMachine,
                                    SerialNumber = "-"
                                };


                                HW_SW_GroupX.GroupPrice += basketItem.TotalNetsale;
                                HW_SW_GroupX.Accessories.Add(basketItem);
                            }
                            // OPS PACKAGE
                            else if (isOPSPackage)
                            {
                                BB_Proposal_OPSManage opsManage = opsManage_lst.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                OsBasket basketItem = new OsBasket
                                {
                                    CodeRef = item.CodeRef,
                                    Description = item.Description,
                                    Family = item.Family,
                                    UnitDiscountPrice = (double)item.UnitDiscountPrice,
                                    Qty = (int)item.Qty,
                                    TotalNetsale = Math.Round((double)item.UnitDiscountPrice * (double)opsManage.TotalMonths),
                                    Group = item.Group,
                                    IsUsed = (bool)item.IsUsedMachine,
                                    SerialNumber = "-"
                                };


                                HW_SW_GroupX.GroupPrice += Math.Round((double)item.UnitDiscountPrice * (double)opsManage.TotalMonths);
                                HW_SW_GroupX.Accessories.Add(basketItem);
                            }
                            // PROPOSAL QUOTE
                            else
                            {
                                OsBasket basketItem = new OsBasket
                                {
                                    CodeRef = item.CodeRef,
                                    Description = item.Description,
                                    Family = item.Family,
                                    UnitDiscountPrice = (double)item.UnitDiscountPrice,
                                    Qty = (int)item.Qty,
                                    TotalNetsale = (double)item.TotalNetsale,
                                    Group = item.Group,
                                    IsUsed = (bool)item.IsUsedMachine,
                                    SerialNumber = "-"
                                };

                                HW_SW_GroupX.GroupPrice += basketItem.UnitDiscountPrice;
                                HW_SW_GroupX.Accessories.Add(basketItem);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return configurator;
        }

        public List<BB_Proposal_DeliveryLocationResumoModel> PontosDeEnvioResumo(int? proposalID)
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
                            int parseID = Convert.ToInt32(i.ID);
                            BB_LocaisEnvio currentLocal = db.BB_LocaisEnvio.Where(x => x.ID == parseID).FirstOrDefault();
                            BB_Proposal_DL_ClientContacts contact = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == i.DeliveryContact).FirstOrDefault();

                            foreach (KeyValuePair<int?, List<BB_Proposal_ItemDoBasket>> p in lstBB_Proposal_ItemDoBasket)
                            {
                                foreach (var it in p.Value)
                                {

                                    BB_Equipamentos isEquip = db.BB_Equipamentos.Where(x => x.CodeRef == it.CodeRef).FirstOrDefault();
                                    if (isEquip != null || it.Description.Contains("MAIN MATERIAL"))
                                    {
                                        BB_Proposal_DeliveryLocationResumoModel resumo = new BB_Proposal_DeliveryLocationResumoModel();
                                        resumo.Group = p.Key;
                                        resumo.Adress1 = currentLocal.IsNewAddress == true ? currentLocal.RoadType + " " + currentLocal.RoadName + " " + currentLocal.RoadNumber : currentLocal.Adress1;
                                        resumo.Adress2 = currentLocal.Adress2;
                                        resumo.PostalCode = i.PostalCode;
                                        resumo.City = i.City;
                                        resumo.Contacto = contact != null ? contact.Name + " " + contact.Surname : "";
                                        resumo.Phone = contact != null ? contact.Movil.ToString() : "";
                                        resumo.Email = contact != null ? contact.Email : "";
                                        resumo.AddressType = i.AccountType;
                                        resumo.CodeRef = it.CodeRef;
                                        resumo.Qty = it.Qty;
                                        resumo.Description = it.Description;
                                        resumo.IsNewAddress = currentLocal.IsNewAddress == null ? false : currentLocal.IsNewAddress;
                                        resumo.Department = i.Department;
                                        resumo.Floor = i.Floor;
                                        resumo.Building = i.Building;
                                        resumo.Room = i.Room;
                                        resumo.Schedule = i.Schedule;
                                        resumo.DeliveryDate = i.DeliveryDate;
                                        resumo.IsUsedMachine = (bool)it.IsUsedMachine ? "Si" : "No";
                                        resumo.SerialNumber = it.SerialNumber != null ? it.SerialNumber : "-";
                                        resumo.Comments = i.Comments != null || i.Comments != "" ? i.Comments : "-";
                                        resumo.IDX = i.IDX;
                                        lstBB_Proposal_DeliveryLocationResumoModel.Add(resumo);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return lstBB_Proposal_DeliveryLocationResumoModel;
        }


        public void SaveUpturns(List<BB_Proposal_Upturn> upturns, int ProposalID, string clientAccountNumber)
        {
            try
            {
                foreach (var upturn_fromDraft in upturns)
                {
                    var newUpturn = new BB_Proposal_Upturn
                    {
                        ProposalID = ProposalID,
                        Total = upturn_fromDraft.Total,
                        Contact = upturn_fromDraft.Contact,
                        Description = upturn_fromDraft.Description,
                        Retirada = upturn_fromDraft.Retirada,
                        Type = upturn_fromDraft.Type,

                        Brand = upturn_fromDraft.Brand,
                        Model = upturn_fromDraft.Model,
                        Equipment_Number = upturn_fromDraft.Equipment_Number,
                        Motive = upturn_fromDraft.Motive,

                        Client_Name = upturn_fromDraft.Client_Name,
                        CIF_NIF = upturn_fromDraft.CIF_NIF,
                        Address_And_Name = upturn_fromDraft.Address_And_Name,
                        Street_Number = upturn_fromDraft.Street_Number,
                        Complement_1 = upturn_fromDraft.Complement_1,
                        Complement_2 = upturn_fromDraft.Complement_2,
                        PostalCode_City = upturn_fromDraft.PostalCode_City,
                        Country = upturn_fromDraft.Country,
                        SapNumber = upturn_fromDraft.SapNumber,

                        Schedule = upturn_fromDraft.Schedule,
                        Department = upturn_fromDraft.Department,
                        Plant = upturn_fromDraft.Plant,

                        Stairs = upturn_fromDraft.Stairs,
                        DNI_LicensePlate = upturn_fromDraft.DNI_LicensePlate,
                        DifficultAccess = upturn_fromDraft.DifficultAccess,
                        Elevator = upturn_fromDraft.Elevator,
                        ServiceLift = upturn_fromDraft.ServiceLift,

                        Comments = upturn_fromDraft.Comments,

                        Name = upturn_fromDraft.Name,
                        Surname = upturn_fromDraft.Surname,
                        Tel = upturn_fromDraft.Tel,
                        Movil = upturn_fromDraft.Movil,
                        Email = upturn_fromDraft.Email
                    };

                    if (upturn_fromDraft.ContactID == 0)
                    {
                        var newContact = new BB_Proposal_DL_ClientContacts
                        {
                            Name = upturn_fromDraft.Name,
                            Surname = upturn_fromDraft.Surname,
                            Tel = upturn_fromDraft.Tel,
                            Movil = upturn_fromDraft.Movil,
                            Email = upturn_fromDraft.Email,
                            ClientID = clientAccountNumber
                        };

                        db.BB_Proposal_DL_ClientContacts.Add(newContact);
                        db.SaveChanges();

                        newUpturn.ContactID = newContact.ID;
                    }
                    else
                    {
                        newUpturn.ContactID = upturn_fromDraft.ContactID;
                    }

                    db.BB_Proposal_Upturn.Add(newUpturn);
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                {
                    string error = ex.Message;
                    throw;
                }
            }
        }

        public void UpdateCommissions(Commission commission, int proposalID)
        {
            try
            {
                var config4 = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Commission, BB_Proposal_Commission>();
                });

                IMapper iMapper4 = config4.CreateMapper();

                List<BB_Proposal_Commission> commision = db.BB_Proposal_Commission.Where(x => x.ProposalID == proposalID).ToList();
                db.BB_Proposal_Commission.RemoveRange(commision);


                BB_Proposal_Commission commission1 = iMapper4.Map<Commission, BB_Proposal_Commission>(commission);

                commission1.ProposalID = proposalID;

                db.BB_Proposal_Commission.Add(commission1);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public void UpdateFinancing(Financing financing, int proposalID)
        {
            try
            {
                List<BB_Proposal_Financing> finToDel = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).ToList();
                db.BB_Proposal_Financing.RemoveRange(finToDel);

                List<BB_Proposal_FinancingMonthly> financingMontlhyToDel = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == proposalID).ToList();
                db.BB_Proposal_FinancingMonthly.RemoveRange(financingMontlhyToDel);

                List<BB_Proposal_FinancingTrimestral> financingtriToDel = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == proposalID).ToList();
                db.BB_Proposal_FinancingTrimestral.RemoveRange(financingtriToDel);


                // Mappers ....
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Financing, BB_Proposal_Financing>();
                });
                IMapper iMapper = config.CreateMapper();


                var configmonthly = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Monthly, BB_Proposal_FinancingMonthly>();
                });
                IMapper iMappermonthly = configmonthly.CreateMapper();


                // FINANCING
                BB_Proposal_Financing fin = iMapper.Map<Financing, BB_Proposal_Financing>(financing);

                if (fin != null)
                {
                    // Função auxiliar para validar e corrigir valores NaN
                    double Sanitize(double value) => double.IsNaN(value) ? 0 : value;

                    // Validação dos campos
                    fin.AmountFinanced = Sanitize((double)fin.AmountFinanced);
                    fin.AmountNotFinanced = Sanitize((double)fin.AmountNotFinanced);
                    fin.MonthlyIncome = Sanitize((double)fin.MonthlyIncome);

                    fin.ProposalID = proposalID;
                    fin.AmountFinanced = Math.Round((double)fin.AmountFinanced, 2);
                    fin.AmountNotFinanced = Math.Round((double)fin.AmountNotFinanced, 2);
                    fin.MonthlyIncome = Math.Round((double)fin.MonthlyIncome, 2);


                    db.BB_Proposal_Financing.Add(fin);
                    db.SaveChanges();
                }


                // FINANCING MONTHLY
                foreach (var monthly in financing.FinancingFactors.Monthly)
                {
                    BB_Proposal_FinancingMonthly m1 = iMappermonthly.Map<Monthly, BB_Proposal_FinancingMonthly>(monthly);

                    m1.ProposalID = proposalID;
                    m1.FinancingID = fin.ID;

                    db.BB_Proposal_FinancingMonthly.Add(m1);
                    db.SaveChanges();
                }

                // FINANCING TRIMESTRAL
                foreach (var trimestral in financing.FinancingFactors.Trimestral)
                {

                    BB_Proposal_FinancingTrimestral t1 = iMappermonthly.Map<Trimestral, BB_Proposal_FinancingTrimestral>(trimestral);

                    t1.ProposalID = proposalID;
                    t1.FinancingID = fin.ID;

                    db.BB_Proposal_FinancingTrimestral.Add(t1);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public void UpdateOvervaluation(List<Overvaluation> overvaluations, int proposalID)
        {
            try
            {
                List<BB_Proposal_Overvaluation> over = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalID).ToList();
                db.BB_Proposal_Overvaluation.RemoveRange(over);

                var config3 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<Overvaluation, BB_Proposal_Overvaluation>();
                    });
                IMapper iMapper3 = config3.CreateMapper();


                foreach (var _overvaluation in overvaluations)
                {
                    BB_Proposal_Overvaluation overvaluation111 = iMapper3.Map<Overvaluation, BB_Proposal_Overvaluation>(_overvaluation);

                    overvaluation111.ProposalID = proposalID;

                    db.BB_Proposal_Overvaluation.Add(overvaluation111);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public void UpdateUpturns(List<BB_Proposal_Upturn> upturns, int proposalID, string clientAccountNumber)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {

                    // Retomas existentes na BD
                    List<BB_Proposal_Upturn> dbUpturns = db.BB_Proposal_Upturn
                        .Where(x => x.ProposalID == proposalID)
                        .ToList();

                    // IDs que vieram no draft
                    List<int> incomingIds = upturns.Select(x => x.ID).ToList();

                    // Atualiza existentes e adiciona novas
                    //int position = 0;
                    foreach (var upturn_fromDraft in upturns)
                    {
                        if (upturn_fromDraft.ID == 0)
                        {
                            // Novo registo
                            var newUpturn = new BB_Proposal_Upturn
                            {
                                ProposalID = proposalID,
                                Total = upturn_fromDraft.Total,
                                Contact = upturn_fromDraft.Contact,
                                Description = upturn_fromDraft.Description,
                                Retirada = upturn_fromDraft.Retirada,
                                Type = upturn_fromDraft.Type,
                                //Position = position,

                                Brand = upturn_fromDraft.Brand,
                                Model = upturn_fromDraft.Model,
                                Equipment_Number = upturn_fromDraft.Equipment_Number,
                                Motive = upturn_fromDraft.Motive,

                                Client_Name = upturn_fromDraft.Client_Name,
                                CIF_NIF = upturn_fromDraft.CIF_NIF,
                                Address_And_Name = upturn_fromDraft.Address_And_Name,
                                Street_Number = upturn_fromDraft.Street_Number,
                                Complement_1 = upturn_fromDraft.Complement_1,
                                Complement_2 = upturn_fromDraft.Complement_2,
                                PostalCode_City = upturn_fromDraft.PostalCode_City,
                                Country = upturn_fromDraft.Country,
                                SapNumber = upturn_fromDraft.SapNumber,

                                Schedule = upturn_fromDraft.Schedule,
                                Department = upturn_fromDraft.Department,
                                Plant = upturn_fromDraft.Plant,

                                Stairs = upturn_fromDraft.Stairs,
                                DNI_LicensePlate = upturn_fromDraft.DNI_LicensePlate,
                                DifficultAccess = upturn_fromDraft.DifficultAccess,
                                Elevator = upturn_fromDraft.Elevator,
                                ServiceLift = upturn_fromDraft.ServiceLift,

                                Comments = upturn_fromDraft.Comments,

                                Name = upturn_fromDraft.Name,
                                Surname = upturn_fromDraft.Surname,
                                Tel = upturn_fromDraft.Tel,
                                Movil = upturn_fromDraft.Movil,
                                Email = upturn_fromDraft.Email
                            };

                            if (upturn_fromDraft.ContactID == 0)
                            {
                                var newContact = new BB_Proposal_DL_ClientContacts
                                {
                                    Name = upturn_fromDraft.Name,
                                    Surname = upturn_fromDraft.Surname,
                                    Tel = upturn_fromDraft.Tel,
                                    Movil = upturn_fromDraft.Movil,
                                    Email = upturn_fromDraft.Email,
                                    ClientID = clientAccountNumber
                                };

                                db.BB_Proposal_DL_ClientContacts.Add(newContact);
                                db.SaveChanges();

                                newUpturn.ContactID = newContact.ID;
                            }
                            else
                            {
                                newUpturn.ContactID = upturn_fromDraft.ContactID;
                            }

                            db.BB_Proposal_Upturn.Add(newUpturn);
                        }
                        else
                        {
                            // Atualizar se já existir em BD
                            var existingUpturn = dbUpturns.FirstOrDefault(x => x.ID == upturn_fromDraft.ID);
                            if (existingUpturn != null)
                            {
                                existingUpturn.Total = upturn_fromDraft.Total;
                                existingUpturn.Contact = upturn_fromDraft.Contact;
                                existingUpturn.Description = upturn_fromDraft.Description;
                                existingUpturn.Type = upturn_fromDraft.Type;
                                existingUpturn.Retirada = upturn_fromDraft.Retirada;
                                //existingUpturn.Position = position;

                                existingUpturn.Brand = upturn_fromDraft.Brand;
                                existingUpturn.Model = upturn_fromDraft.Model;
                                existingUpturn.Equipment_Number = upturn_fromDraft.Equipment_Number;
                                existingUpturn.Motive = upturn_fromDraft.Motive;

                                existingUpturn.Client_Name = upturn_fromDraft.Client_Name;
                                existingUpturn.CIF_NIF = upturn_fromDraft.CIF_NIF;
                                existingUpturn.Address_And_Name = upturn_fromDraft.Address_And_Name;
                                existingUpturn.Street_Number = upturn_fromDraft.Street_Number;
                                existingUpturn.Complement_1 = upturn_fromDraft.Complement_1;
                                existingUpturn.Complement_2 = upturn_fromDraft.Complement_2;
                                existingUpturn.PostalCode_City = upturn_fromDraft.PostalCode_City;
                                existingUpturn.Country = upturn_fromDraft.Country;
                                existingUpturn.SapNumber = upturn_fromDraft.SapNumber;

                                existingUpturn.Schedule = upturn_fromDraft.Schedule;
                                existingUpturn.Department = upturn_fromDraft.Department;
                                existingUpturn.Plant = upturn_fromDraft.Plant;

                                existingUpturn.Stairs = upturn_fromDraft.Stairs;
                                existingUpturn.DNI_LicensePlate = upturn_fromDraft.DNI_LicensePlate;
                                existingUpturn.DifficultAccess = upturn_fromDraft.DifficultAccess;
                                existingUpturn.Elevator = upturn_fromDraft.Elevator;
                                existingUpturn.ServiceLift = upturn_fromDraft.ServiceLift;

                                existingUpturn.Comments = upturn_fromDraft.Comments;

                                existingUpturn.Name = upturn_fromDraft.Name;
                                existingUpturn.Surname = upturn_fromDraft.Surname;
                                existingUpturn.Tel = upturn_fromDraft.Tel;
                                existingUpturn.Movil = upturn_fromDraft.Movil;
                                existingUpturn.Email = upturn_fromDraft.Email;

                                // se o contacto for alterado
                                if (upturn_fromDraft.ContactID != existingUpturn.ContactID)
                                {
                                    // se for um contacto NOVO na bd
                                    if (upturn_fromDraft.ContactID == 0)
                                    {
                                        var newContact = new BB_Proposal_DL_ClientContacts
                                        {
                                            Name = upturn_fromDraft.Name,
                                            Surname = upturn_fromDraft.Surname,
                                            Tel = upturn_fromDraft.Tel,
                                            Movil = upturn_fromDraft.Movil,
                                            Email = upturn_fromDraft.Email,
                                            ClientID = clientAccountNumber
                                        };

                                        db.BB_Proposal_DL_ClientContacts.Add(newContact);
                                        db.SaveChanges();

                                        existingUpturn.ContactID = newContact.ID;
                                    }
                                    // se for um contacto EXISTENTE na bd
                                    else
                                    {
                                        existingUpturn.ContactID = upturn_fromDraft.ContactID;
                                    }
                                }
                            }
                        }

                        //position++;
                    }

                    // Remove os que existem em BD mas não estão no draft (foram apagados)
                    var toRemove = dbUpturns
                        .Where(x => !incomingIds.Contains(x.ID))
                        .ToList();

                    if (toRemove.Any())
                    {
                        db.BB_Proposal_Upturn.RemoveRange(toRemove);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string error = ex.Message;
                    throw;

                }
            }
        }

        public void UpdatePrintingService(PrintingServices2 printingServices2, int newID)
        {
            try
            {
                if (printingServices2.ApprovedPrintingServices != null)
                {
                    foreach (ApprovedPrintingService aps in printingServices2.ApprovedPrintingServices)
                    {
                        if (aps.ID == null)
                        {
                            BB_PrintingServices newPS = new BB_PrintingServices()
                            {
                                BWVolume = aps.BWVolume,
                                CVolume = aps.CVolume,
                                ContractDuration = aps.ContractDuration,
                                PrintingServices2ID = newID,
                                IsPrecalc = aps.IsPrecalc,
                                Fee = 0,
                            };

                            db.BB_PrintingServices.Add(newPS);
                            db.SaveChanges();
                            aps.ID = newPS.ID;

                            if (aps.GlobalClickVVA != null)
                            {
                                BB_VVA vva = new BB_VVA()
                                {
                                    BWExcessPVP = aps.GlobalClickVVA.BWExcessPVP,
                                    CExcessPVP = aps.GlobalClickVVA.CExcessPVP,
                                    ExcessBillingFrequency = aps.GlobalClickVVA.ExcessBillingFrequency,
                                    PVP = aps.GlobalClickVVA.PVP,
                                    RentBillingFrequency = aps.GlobalClickVVA.RentBillingFrequency,
                                    PrintingServiceID = newPS.ID,
                                    ReturnType = aps.GlobalClickVVA.ReturnType,
                                };

                                db.BB_VVA.Add(vva);
                                db.SaveChanges();

                            }
                            if (aps.GlobalClickNoVolume != null)
                            {
                                BB_PrintingServices_NoVolume nv = new BB_PrintingServices_NoVolume()
                                {
                                    GlobalClickBW = aps.GlobalClickNoVolume.GlobalClickBW,
                                    GlobalClickC = aps.GlobalClickNoVolume.GlobalClickC,
                                    PageBillingFrequency = aps.GlobalClickNoVolume.PageBillingFrequency,
                                    PrintingServiceID = newPS.ID,
                                };

                                db.BB_PrintingServices_NoVolume.Add(nv);
                                db.SaveChanges();


                            }
                            if (aps.ClickPerModel != null)
                            {
                                BB_PrintingServices_ClickPerModel cpm = new BB_PrintingServices_ClickPerModel()
                                {
                                    PageBillingFrequency = aps.ClickPerModel.PageBillingFrequency,
                                    PrintingServiceID = newPS.ID,
                                };

                                db.BB_PrintingServices_ClickPerModel.Add(cpm);
                                db.SaveChanges();

                            }
                            if (aps.Machines != null)
                            {
                                foreach (Machine m in aps.Machines)
                                {
                                    BB_PrintingService_Machines machine = new BB_PrintingService_Machines()
                                    {
                                        BWVolume = m.BWVolume,
                                        CodeRef = m.CodeRef,
                                        CVolume = m.CVolume,
                                        Description = m.Description,
                                        PrintingServiceID = newPS.ID,
                                        Quantity = m.Qty,
                                        ApprovedBW = m.ClickPriceBW,
                                        ApprovedC = m.ClickPriceC
                                    };

                                    db.BB_PrintingService_Machines.Add(machine);
                                    db.SaveChanges();
                                }
                            }
                            if (aps.VVA_PerModel_lst != null)
                            {
                                foreach (BB_PrintingServices_ClickPerModel_VVA m in aps.VVA_PerModel_lst)
                                {
                                    BB_PrintingServices_ClickPerModel_VVA ps_vva_model = new BB_PrintingServices_ClickPerModel_VVA()
                                    {
                                        PrintingServiceID = newPS.ID,
                                        CodeRef = m.CodeRef,
                                        Quantity = m.Quantity,
                                        Description = m.Description,
                                        BWVolume = m.BWVolume,
                                        CVolume = m.CVolume,
                                        BWPVP = m.BWPVP,
                                        CPVP = m.CPVP,
                                        BWCost = m.BWCost,
                                        CCost = m.CCost,
                                        ApprovedBW = m.ApprovedBW,
                                        ApprovedC = m.ApprovedC,
                                        IsInClient = m.IsInClient,
                                        IsUsed = m.IsUsed,
                                        RequestedBWClickPrice = m.RequestedBWClickPrice,
                                        RequestedCClickPrice = m.RequestedCClickPrice,
                                        BWExcessPVP = m.BWExcessPVP,
                                        CExcessPVP = m.CExcessPVP,
                                        PVP = m.PVP,
                                        ExcessBillingFrequency = m.ExcessBillingFrequency,
                                        RentBillingFrequency = m.RentBillingFrequency,
                                        ReturnType = m.ReturnType,
                                        RequestedBWExcess = m.RequestedBWExcess,
                                        RequestedCExcess = m.RequestedCExcess,
                                        RequestedRent = m.RequestedRent,
                                    };

                                    db.BB_PrintingServices_ClickPerModel_VVA.Add(ps_vva_model);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public List<OsBasket> Update_BB_Proposal_Quote(ProposalRootObject p, int proposalID)
        {
            try
            {
                // ------------------------ Mappers ------------------------

                var configQuote = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<OsBasket, BB_Proposal_Quote>()
                    .ForMember(dest => dest.ID, opt => opt.Ignore());
                });

                IMapper iMapper1 = configQuote.CreateMapper();

                var configCounter = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Counter, BB_Proposal_Counters>()
                    .ForMember(dest => dest.ID, opt => opt.Ignore());
                });

                IMapper iMapper10 = configCounter.CreateMapper();

                var configPSConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<PsConfig, BB_Proposal_PsConfig>()
                    .ForMember(dest => dest.ID, opt => opt.Ignore());
                });

                IMapper iMapperPSConfig = configPSConfig.CreateMapper();


                // BB_PROPOSAL_QUOTE ----------------------------------------------------

                // configurador guardado na Base de dados
                List<BB_Proposal_Quote> proposal_quote_lst = db.BB_Proposal_Quote.Where(q => q.Proposal_ID == proposalID).ToList();

                // Obter os IDs incomming
                var incomingQuoteIDs = new HashSet<int>(
                    p.Draft.baskets.os_basket
                        .Where(q => q.ID > 0)
                        .Select(q => q.ID)
                );

                // registos que devem ser removidos
                var quotesToDelete = proposal_quote_lst
                    .Where(q => !incomingQuoteIDs.Contains(q.ID))
                    .ToList();

                // Remover e guardar
                if (quotesToDelete.Any())
                {
                    // trabalhar apenas com os registos reais
                    proposal_quote_lst = proposal_quote_lst.Where(q => incomingQuoteIDs.Contains(q.ID)).ToList();
                    db.BB_Proposal_Quote.RemoveRange(quotesToDelete);
                    db.SaveChanges();
                }

                var existingQuotesDict = proposal_quote_lst.ToDictionary(q => q.ID);
                var timestamp = DateTime.Now;

                foreach (var _Quote_Incomming in p.Draft.baskets.os_basket)
                {
                    var quote = proposal_quote_lst.FirstOrDefault(q => q.ID == _Quote_Incomming.ID);

                    if (_Quote_Incomming.ID > 0 && existingQuotesDict.TryGetValue(_Quote_Incomming.ID, out var existing))
                    {
                        // Atualizar existente
                        iMapper1.Map(_Quote_Incomming, existing);
                        existing.ModifiedBy = p.Draft.details.CreatedBy;
                        existing.ModifiedTime = timestamp;
                        db.Entry(existing).State = EntityState.Modified;
                    }
                    else
                    {
                        // Criar novo
                        BB_Proposal_Quote newQuote = iMapper1.Map<OsBasket, BB_Proposal_Quote>(_Quote_Incomming);
                        newQuote.Proposal_ID = proposalID;
                        newQuote.CreatedBy = p.Draft.details.CreatedBy;
                        newQuote.CreatedTime = timestamp;
                        newQuote.ModifiedBy = p.Draft.details.CreatedBy;
                        newQuote.ModifiedTime = timestamp;
                        db.BB_Proposal_Quote.Add(newQuote);

                        db.SaveChanges();
                        _Quote_Incomming.ID = newQuote.ID;
                    }

                    db.SaveChanges();


                    // BB_PROPOSAL_COUNTERS ----------------------------------------------------

                    if (_Quote_Incomming.counters != null)
                    {
                        foreach (var counter in _Quote_Incomming.counters)
                        {
                            var counterX = db.BB_Proposal_Counters.Where(q => q.ProposalID == proposalID).FirstOrDefault();

                            if (counterX == null)
                            {
                                // Não existe -> criar novo
                                counterX = iMapper10.Map<Counter, BB_Proposal_Counters>(counter);

                                db.BB_Proposal_Counters.Add(counterX);
                            }
                            else
                            {
                                // Já existe -> fazer update
                                iMapper1.Map(counter, counterX);
                                db.Entry(counterX).State = EntityState.Modified;
                            }
                            db.SaveChanges();


                            BB_Maquinas_Usadas_Gestor g = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == counter.serialNumber).FirstOrDefault();
                            if (g != null)
                            {
                                g.ProposalID = proposalID;
                                g.IsReserved = true;
                                db.Entry(g).State = g.ID == 0 ? EntityState.Added : EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }

                    // PS_CONFIG ----------------------------------------------------
                    if (_Quote_Incomming.psConfig != null)
                    {
                        var psconfig = db.BB_Proposal_PsConfig.Where(q => q.ProposalID == proposalID).FirstOrDefault();

                        if (psconfig == null)
                        {
                            // Não existe -> criar novo
                            psconfig = iMapper10.Map<PsConfig, BB_Proposal_PsConfig>(_Quote_Incomming.psConfig);

                            psconfig.ProposalID = proposalID;
                            psconfig.ItemID = quote.ID;
                            db.BB_Proposal_PsConfig.Add(psconfig);
                        }
                        else
                        {
                            // Já existe -> fazer update
                            iMapper1.Map(_Quote_Incomming.psConfig, psconfig);
                            psconfig.ProposalID = proposalID;
                            psconfig.ItemID = quote.ID;
                            db.Entry(psconfig).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }

                return p.Draft.baskets.os_basket;

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;

            }
        }

        public void Update_BB_Proposal_Quote_RS(List<RsBasket> rs_basket, int proposalID)
        {
            try
            {
                // Mappers ----------------------------------
                var config2 = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RsBasket, BB_Proposal_Quote_RS>();
                });
                IMapper iMapper2 = config2.CreateMapper();

                // Remover todos os registos ----------------
                List<BB_Proposal_Quote_RS> quotesRS = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).ToList();
                db.BB_Proposal_Quote_RS.RemoveRange(quotesRS);
                db.SaveChanges();

                // adicionar registos todos novamente -------
                foreach (var _Quote in rs_basket)
                {
                    BB_Proposal_Quote_RS quote = iMapper2.Map<RsBasket, BB_Proposal_Quote_RS>(_Quote);
                    quote.ProposalID = proposalID;

                    db.BB_Proposal_Quote_RS.Add(quote);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }

        public void Update_OPS(BB_Proposal proposal, Draft draft, int proposalID)
        {
            try
            {
                //BB_PROPOSAL_OPSIMPLEMENT
                OPSPacks opsPacks = draft.opsPacks;
                List<OPSImplement> draftImplements = opsPacks.opsImplement.ToList();
                List<BB_Proposal_OPSImplement> dbImplement = proposal.BB_Proposal_OPSImplement.ToList();
                List<int> toDeleteImplementIds = dbImplement.Select(x => x.ID).Except(draftImplements.Select(x => x.ID.GetValueOrDefault())).ToList();
                if (toDeleteImplementIds.Count > 0)
                {
                    db.BB_Proposal_OPSImplement.RemoveRange(db.BB_Proposal_OPSImplement.Where(x => toDeleteImplementIds.Contains(x.ID)).ToList());
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
                int opsImplementPosition = 0;
                foreach (OPSImplement opsI in draftImplements)
                {
                    if (opsI.ID == null)
                    {
                        BB_Proposal_OPSImplement newOPSI = new BB_Proposal_OPSImplement
                        {
                            CodeRef = opsI.CodeRef,
                            Description = opsI.Description,
                            Family = opsI.Family,
                            InCatalog = opsI.InCatalog,
                            IsFinanced = opsI.IsFinanced,
                            MaxRange = opsI.MaxRange,
                            MinRange = opsI.MinRange,
                            Name = opsI.Name,
                            Position = opsImplementPosition,
                            PVP = opsI.PVP,
                            ProposalID = proposalID,
                            Quantity = opsI.Quantity,
                            Type = opsI.Type,
                            IsValidated = opsI.IsValidated
                        };
                        if (opsI != null)
                        {
                            newOPSI.UnitDiscountPrice = opsI.UnitDiscountPrice;
                        }
                        db.BB_Proposal_OPSImplement.Add(newOPSI);
                        try
                        {
                            db.SaveChanges();
                            opsPacks.opsImplement[opsImplementPosition].ID = newOPSI.ID;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    else
                    {
                        BB_Proposal_OPSImplement toEditImplement = db.BB_Proposal_OPSImplement.Where(x => x.ID == opsI.ID).FirstOrDefault();
                        if (toEditImplement != null)
                        {
                            toEditImplement.CodeRef = opsI.CodeRef;
                            toEditImplement.Description = opsI.Description;
                            toEditImplement.Family = opsI.Family;
                            toEditImplement.InCatalog = opsI.InCatalog;
                            toEditImplement.IsFinanced = opsI.IsFinanced;
                            toEditImplement.MaxRange = opsI.MaxRange;
                            toEditImplement.MinRange = opsI.MinRange;
                            toEditImplement.Name = opsI.Name;
                            toEditImplement.PVP = opsI.PVP;
                            toEditImplement.Position = opsImplementPosition;
                            toEditImplement.Quantity = opsI.Quantity;
                            toEditImplement.Type = opsI.Type;
                            toEditImplement.IsValidated = opsI.IsValidated;
                        }
                        if (opsI != null)
                        {
                            toEditImplement.UnitDiscountPrice = opsI.UnitDiscountPrice;
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
                    opsImplementPosition++;
                }

                //BB_PROPOSAL_OPSManage
                List<OPSManage> draftManages = new List<OPSManage>();
                draftManages = opsPacks.opsManage.ToList();
                List<BB_Proposal_OPSManage> dbManages = proposal.BB_Proposal_OPSManage.ToList();
                List<int> toDeleteManageIds = dbManages.Select(x => x.ID).Except(draftManages.Select(x => x.ID.GetValueOrDefault())).ToList();
                if (toDeleteManageIds.Count > 0)
                {
                    db.BB_Proposal_OPSManage.RemoveRange(db.BB_Proposal_OPSManage.Where(x => toDeleteManageIds.Contains(x.ID)).ToList());
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
                int opsManagePosition = 0;
                foreach (OPSManage opsM in draftManages)
                {
                    if (opsM.ID == null)
                    {
                        BB_Proposal_OPSManage newOPSM = new BB_Proposal_OPSManage
                        {
                            CodeRef = opsM.CodeRef,
                            Description = opsM.Description,
                            Family = opsM.Family,
                            InCatalog = opsM.InCatalog,
                            MaxRange = opsM.MaxRange,
                            MinRange = opsM.MinRange,
                            Name = opsM.Name,
                            Position = opsManagePosition,
                            PVP = opsM.PVP,
                            ProposalID = proposalID,
                            Quantity = opsM.Quantity,
                            TotalMonths = opsM.TotalMonths,
                            Type = opsM.Type,
                            IsValidated = opsM.IsValidated
                        };
                        if (opsM != null)
                        {
                            newOPSM.UnitDiscountPrice = opsM.UnitDiscountPrice;
                        }
                        db.BB_Proposal_OPSManage.Add(newOPSM);
                        try
                        {
                            db.SaveChanges();
                            opsPacks.opsManage[opsManagePosition].ID = newOPSM.ID;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    else
                    {
                        BB_Proposal_OPSManage toEditManage = db.BB_Proposal_OPSManage.Where(x => x.ID == opsM.ID).FirstOrDefault();
                        if (toEditManage != null)
                        {
                            toEditManage.CodeRef = opsM.CodeRef;
                            toEditManage.Description = opsM.Description;
                            toEditManage.Family = opsM.Family;
                            toEditManage.InCatalog = opsM.InCatalog;
                            toEditManage.MaxRange = opsM.MaxRange;
                            toEditManage.MinRange = opsM.MinRange;
                            toEditManage.Name = opsM.Name;
                            toEditManage.PVP = opsM.PVP;
                            toEditManage.Position = opsManagePosition;
                            toEditManage.TotalMonths = opsM.TotalMonths;
                            toEditManage.Quantity = opsM.Quantity;
                            toEditManage.Type = opsM.Type;
                            toEditManage.IsValidated = opsM.IsValidated;
                        }
                        if (opsM != null)
                        {
                            toEditManage.UnitDiscountPrice = opsM.UnitDiscountPrice;
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
                    opsManagePosition++;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
        }
        public void AddNewPrintingService(PrintingServices2 printingServices2, int proposalID)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    BB_Proposal_PrintingServices2 db_ps2 = new BB_Proposal_PrintingServices2()
                    {
                        ProposalID = proposalID,
                        ActivePrintingService = printingServices2.ActivePrintingService
                    };

                    db.BB_Proposal_PrintingServices2.Add(db_ps2);
                    db.SaveChanges();
                    printingServices2.ID = db_ps2.ID;


                    if (printingServices2.ApprovedPrintingServices != null)
                    {
                        foreach (ApprovedPrintingService aps in printingServices2.ApprovedPrintingServices)
                        {
                            var currentAPSID = aps.ID;

                            BB_PrintingServices newPS = new BB_PrintingServices()
                            {
                                BWVolume = aps.BWVolume,
                                CVolume = aps.CVolume,
                                ContractDuration = aps.ContractDuration,
                                PrintingServices2ID = (int)printingServices2.ID,
                                IsPrecalc = aps.IsPrecalc,
                                Fee = 0,
                            };

                            db.BB_PrintingServices.Add(newPS);
                            db.SaveChanges();
                            aps.ID = newPS.ID;

                            if (aps.GlobalClickVVA != null)
                            {
                                BB_VVA vva = new BB_VVA()
                                {
                                    BWExcessPVP = aps.GlobalClickVVA.BWExcessPVP,
                                    CExcessPVP = aps.GlobalClickVVA.CExcessPVP,
                                    ExcessBillingFrequency = aps.GlobalClickVVA.ExcessBillingFrequency,
                                    PVP = aps.GlobalClickVVA.PVP,
                                    RentBillingFrequency = aps.GlobalClickVVA.RentBillingFrequency,
                                    PrintingServiceID = newPS.ID,
                                    ReturnType = aps.GlobalClickVVA.ReturnType,
                                };

                                db.BB_VVA.Add(vva);
                                db.SaveChanges();
                            }
                            if (aps.GlobalClickNoVolume != null)
                            {
                                BB_PrintingServices_NoVolume nv = new BB_PrintingServices_NoVolume()
                                {
                                    GlobalClickBW = aps.GlobalClickNoVolume.GlobalClickBW,
                                    GlobalClickC = aps.GlobalClickNoVolume.GlobalClickC,
                                    PageBillingFrequency = aps.GlobalClickNoVolume.PageBillingFrequency,
                                    PrintingServiceID = newPS.ID,
                                };

                                db.BB_PrintingServices_NoVolume.Add(nv);
                                db.SaveChanges();

                            }
                            if (aps.ClickPerModel != null)
                            {
                                BB_PrintingServices_ClickPerModel cpm = new BB_PrintingServices_ClickPerModel()
                                {
                                    PageBillingFrequency = aps.ClickPerModel.PageBillingFrequency,
                                    PrintingServiceID = newPS.ID,
                                };

                                db.BB_PrintingServices_ClickPerModel.Add(cpm);
                                db.SaveChanges();

                            }
                            if (aps.Machines != null)
                            {
                                foreach (Machine m in aps.Machines)
                                {
                                    BB_PrintingService_Machines machine = new BB_PrintingService_Machines()
                                    {
                                        BWVolume = m.BWVolume,
                                        CodeRef = m.CodeRef,
                                        CVolume = m.CVolume,
                                        Description = m.Description,
                                        PrintingServiceID = newPS.ID,
                                        Quantity = m.Qty,
                                        ApprovedBW = m.ClickPriceBW,
                                        ApprovedC = m.ClickPriceC
                                    };

                                    db.BB_PrintingService_Machines.Add(machine);
                                    db.SaveChanges();
                                }
                            }
                            if (currentAPSID != null)
                            {
                                BB_Proposal_PrintingServiceValidationRequest psvr = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == currentAPSID).FirstOrDefault();
                                if (psvr != null)
                                {
                                    BB_Proposal_PrintingServiceValidationRequest newPSVR = new BB_Proposal_PrintingServiceValidationRequest()
                                    {
                                        ApprovedAt = psvr.ApprovedAt,
                                        ApprovedBy = psvr.ApprovedBy,
                                        IsApproved = psvr.IsApproved,
                                        IsComplete = psvr.IsComplete,
                                        RequestedAt = psvr.RequestedAt,
                                        PrintingServiceID = newPS.ID,
                                        RequestedBy = psvr.RequestedBy,
                                        SCObservations = psvr.SCObservations,
                                        SEObservations = psvr.SEObservations,
                                        ToDelete = psvr.ToDelete,
                                    };
                                    db.BB_Proposal_PrintingServiceValidationRequest.Add(newPSVR);
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
                            if (aps.VVA_PerModel_lst != null)
                            {
                                foreach (BB_PrintingServices_ClickPerModel_VVA m in aps.VVA_PerModel_lst)
                                {
                                    BB_PrintingServices_ClickPerModel_VVA ps_vva_model = new BB_PrintingServices_ClickPerModel_VVA()
                                    {
                                        PrintingServiceID = newPS.ID,
                                        CodeRef = m.CodeRef,
                                        Quantity = m.Quantity,
                                        Description = m.Description,
                                        BWVolume = m.BWVolume,
                                        CVolume = m.CVolume,
                                        BWPVP = m.BWPVP,
                                        CPVP = m.CPVP,
                                        BWCost = m.BWCost,
                                        CCost = m.CCost,
                                        ApprovedBW = m.ApprovedBW,
                                        ApprovedC = m.ApprovedC,
                                        IsInClient = m.IsInClient,
                                        IsUsed = m.IsUsed,
                                        RequestedBWClickPrice = m.RequestedBWClickPrice,
                                        RequestedCClickPrice = m.RequestedCClickPrice,
                                        BWExcessPVP = m.BWExcessPVP,
                                        CExcessPVP = m.CExcessPVP,
                                        PVP = m.PVP,
                                        ExcessBillingFrequency = m.ExcessBillingFrequency,
                                        RentBillingFrequency = m.RentBillingFrequency,
                                        ReturnType = m.ReturnType,
                                        RequestedBWExcess = m.RequestedBWExcess,
                                        RequestedCExcess = m.RequestedCExcess,
                                        RequestedRent = m.RequestedRent,
                                    };

                                    db.BB_PrintingServices_ClickPerModel_VVA.Add(ps_vva_model);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string error = ex.Message;
                    throw;

                }
            }
        }
    }
}