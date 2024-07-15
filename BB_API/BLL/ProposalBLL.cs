using AutoMapper;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using WebApplication1.App_Start;
using WebApplication1.Controllers;
using WebApplication1.Models;
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


                    List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposal.ID).ToList();
                    db.BB_Proposal_Quote.RemoveRange(quotes);

                    List<BB_Proposal_Quote_RS> quotesRS = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Quote_RS.RemoveRange(quotesRS);

                    List<BB_Proposal_Commission> commision = db.BB_Proposal_Commission.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Commission.RemoveRange(commision);

                    List<BB_Proposal_PsConfig> psConfi = db.BB_Proposal_PsConfig.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_PsConfig.RemoveRange(psConfi);

                    List<BB_Proposal_Financing> fin = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Financing.RemoveRange(fin);

                    List<BB_Proposal_Overvaluation> over = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Overvaluation.RemoveRange(over);

                    List<BB_Proposal_Vva> vva = db.BB_Proposal_Vva.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Vva.RemoveRange(vva);

                    List<BB_Proposal_PrintingServices> pritningService = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_PrintingServices.RemoveRange(pritningService);

                    List<BB_Proposal_PsConfig> psconfig1 = db.BB_Proposal_PsConfig.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_PsConfig.RemoveRange(psconfig1);

                    List<BB_Proposal_FinancingMonthly> financingMOntlhy = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_FinancingMonthly.RemoveRange(financingMOntlhy);

                    List<BB_Proposal_FinancingTrimestral> financingtri = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_FinancingTrimestral.RemoveRange(financingtri);

                    List<BB_Proposal_Client> lstCliente = db.BB_Proposal_Client.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Client.RemoveRange(lstCliente);

                    List<BB_Proposal_Consignments> lstConsignacoes = db.BB_Proposal_Consignments.Where(x => x.ProposalID == proposal.ID).ToList();
                    db.BB_Proposal_Consignments.RemoveRange(lstConsignacoes);

                    //List<BB_Proposal_DeliveryLocation> lstd = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposal.ID).ToList();

                    //foreach (var item in lstd)
                    //{
                    //    List<BB_Proposal_ItemDoBasket> bb_Proposal_ItemDoBasket1 = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == item.IDX).ToList();

                    //    db.BB_Proposal_ItemDoBasket.RemoveRange(bb_Proposal_ItemDoBasket1);

                    //}
                    //db.BB_Proposal_DeliveryLocation.RemoveRange(lstd);
                    db.Entry(proposal).State = proposal.ID == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                    /*
                    if(proposal != null)
                    {
                        foreach(var opsManage in proposal.BB_Proposal_OPSManage)
                        {
                            opsManage.BB_Proposal = null;
                        }
                        foreach (var opsImplement in proposal.BB_Proposal_OPSImplement)
                        {
                            opsImplement.BB_Proposal = null;
                        }
                        log4net.ThreadContext.Properties["proposal_id"] = proposal.ID;
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(proposal);
                        Exception message = new Exception("Proposta gravada com sucesso");
                        log.Info(json, message);
                    }
                    */
                }

                

                using (var context = new BB_DB_DEVEntities2())
                {
                    BB_Proposal proposal = context.BB_Proposal.Find(p.Draft.details.ID);
                    int ProposalID = proposal.ID;

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

                                counters.ProposalID = ProposalID;
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

                            psconfig.ProposalID = ProposalID;
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

                    //BB_PROPOSAL_OPSIMPLEMENT
                    OPSPacks opsPacks = p.Draft.opsPacks;
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
                                ProposalID = ProposalID,
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

                    //BB_PROPOSAL_QUOTE_Financing
                    var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Financing, BB_Proposal_Financing>();
                        });

                    IMapper iMapper = config.CreateMapper();

                    BB_Proposal_Financing fin = iMapper.Map<Financing, BB_Proposal_Financing>(p.Draft.financing);

                    fin.ProposalID = ProposalID;

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

                    //BB_PROPOSAL_Commission
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

                    List<Upturn> upturns = p.Draft.upturns;
                    List<BB_Proposal_Upturn> dbUpturns = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposal.ID).ToList();
                    List<int> toDeleteIds = dbUpturns.Select(x => x.ID).Except(upturns.Select(x => x.ID.GetValueOrDefault())).ToList();
                    if (toDeleteIds.Count > 0)
                    {
                        db.BB_Proposal_Upturn.RemoveRange(dbUpturns.Where(x => toDeleteIds.Contains(x.ID)).ToList());
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    int position = 0;
                    foreach (Upturn ut in upturns)
                    {
                        if (ut.ID == null)
                        {
                            BB_Proposal_Upturn newUpturn = new BB_Proposal_Upturn
                            {
                                ProposalID = ProposalID,
                                Total = ut.Total,
                                Contact = ut.Contact,
                                Description = ut.Description,
                                Type = ut.Type,
                                Position = position
                            };
                            db.BB_Proposal_Upturn.Add(newUpturn);
                            try
                            {
                                db.SaveChanges();
                                ut.ID = newUpturn.ID;
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                        else
                        {
                            BB_Proposal_Upturn toEdit = dbUpturns.Where(x => x.ID == ut.ID).FirstOrDefault();
                            if (toEdit != null)
                            {
                                toEdit.Total = ut.Total;
                                toEdit.Contact = ut.Contact;
                                toEdit.Description = ut.Description;
                                toEdit.Type = ut.Type;
                                toEdit.Position = position;
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
                        position++;
                    }

                    //PRINTING SERVICES 
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

                    PrintingServices2 printingServices2 = p.Draft.printingServices2;
                    if (printingServices2 != null)
                    {
                        if (printingServices2.ID == null)
                        {
                            BB_Proposal_PrintingServices2 db_ps2 = new BB_Proposal_PrintingServices2()
                            {
                                ProposalID = ProposalID,
                                ActivePrintingService = printingServices2.ActivePrintingService
                            };
                            db.BB_Proposal_PrintingServices2.Add(db_ps2);
                            try
                            {
                                db.SaveChanges();
                                printingServices2.ID = db_ps2.ID;
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                            if (printingServices2.ApprovedPrintingServices != null)
                            {
                                foreach (ApprovedPrintingService aps in printingServices2.ApprovedPrintingServices)
                                {
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
                                    try
                                    {
                                        db.SaveChanges();
                                        aps.ID = newPS.ID;
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Message.ToString();
                                    }
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
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Message.ToString();
                                        }
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
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Message.ToString();
                                        }
                                    }
                                    if (aps.ClickPerModel != null)
                                    {
                                        BB_PrintingServices_ClickPerModel cpm = new BB_PrintingServices_ClickPerModel()
                                        {
                                            PageBillingFrequency = aps.ClickPerModel.PageBillingFrequency,
                                            PrintingServiceID = newPS.ID,
                                        };
                                        db.BB_PrintingServices_ClickPerModel.Add(cpm);
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Message.ToString();
                                        }
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
                                }
                            }
                        }
                        else
                        {
                            BB_Proposal_PrintingServices2 toUpdate = db.BB_Proposal_PrintingServices2.FirstOrDefault(x => x.ID == printingServices2.ID);
                            toUpdate.ActivePrintingService = printingServices2.ActivePrintingService;
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
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
                                            PrintingServices2ID = toUpdate.ID,
                                            IsPrecalc = aps.IsPrecalc,
                                            Fee = 0,
                                        };
                                        db.BB_PrintingServices.Add(newPS);
                                        try
                                        {
                                            db.SaveChanges();
                                            aps.ID = newPS.ID;
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Message.ToString();
                                        }
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
                                            try
                                            {
                                                db.SaveChanges();
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.Message.ToString();
                                            }
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
                                            try
                                            {
                                                db.SaveChanges();
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.Message.ToString();
                                            }
                                        }
                                        if (aps.ClickPerModel != null)
                                        {
                                            BB_PrintingServices_ClickPerModel cpm = new BB_PrintingServices_ClickPerModel()
                                            {
                                                PageBillingFrequency = aps.ClickPerModel.PageBillingFrequency,
                                                PrintingServiceID = newPS.ID,
                                            };
                                            db.BB_PrintingServices_ClickPerModel.Add(cpm);
                                            try
                                            {
                                                db.SaveChanges();
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.Message.ToString();
                                            }
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
                                    }
                                }
                            }
                        }
                    }

                    //BB_PROPOSAL_Cliente
                    BB_Proposal_Client cliente1 = new BB_Proposal_Client();
                    cliente1.ClientID = p.Draft.client.accountnumber;
                    cliente1.IsNewClient = p.Draft.client.isNewClient;
                    cliente1.ProposalID = ProposalID;
                    cliente1.Name = p.Draft.client.Name;
                    cliente1.IsPublicSector = p.Draft.client.isPublicSector;
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

                    //var DLfromDraft = p.Draft.deliveryLocationsBES.deliveryLocationsShipToBillTo;

                    //List<int> IDX_Included = DLfromDraft.Select(x => x.IDX).ToList();

                    //if (DLfromDraft.Count() > 0)
                    //{
                    //    List<BB_Proposal_DeliveryLocation> dl_lst_toDelete = db.BB_Proposal_DeliveryLocation
                    //        .Where(x => x.ProposalID == p.Draft.details.ID && !IDX_Included.Contains(x.IDX))
                    //        .ToList();


                    //    if (dl_lst_toDelete.Count() > 0)
                    //    {
                    //        // "Updating" BB_Proposal_ItemDoBasket

                    //        List<int> lst_IDX = dl_lst_toDelete.Select(x => x.IDX).ToList();

                    //        List<BB_Proposal_ItemDoBasket> basketItems_lst_toDelete = new List<BB_Proposal_ItemDoBasket>();

                    //        foreach (int IDX in lst_IDX)
                    //        {
                    //            List<BB_Proposal_ItemDoBasket> IDX_items = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == IDX).ToList();
                    //            basketItems_lst_toDelete.AddRange(IDX_items);
                    //        }

                    //        if (basketItems_lst_toDelete.Count() > 0)
                    //        {
                    //            db.BB_Proposal_ItemDoBasket.RemoveRange(basketItems_lst_toDelete);
                    //            try
                    //            {
                    //                db.SaveChanges();
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                ex.Message.ToString();
                    //            }
                    //        }

                    //        // "Updating" BB_Proposal_DeliveryLocation
                    //        if (dl_lst_toDelete.Count() > 0)
                    //        {
                    //            db.BB_Proposal_DeliveryLocation.RemoveRange(dl_lst_toDelete);
                    //            try
                    //            {
                    //                db.SaveChanges();
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                ex.Message.ToString();
                    //            }
                    //        }
                    //    }


                    //    try
                    //    {
                    //        db.SaveChanges();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        ex.Message.ToString();
                    //    }

                    //}




                    //if (p.Draft.deliveryLocationsBES.AssignedItems.Count() > 0)
                    //{

                    //    foreach (var assignItem in p.Draft.deliveryLocationsBES.AssignedItems)
                    //    {

                    //        var configpItemns = new MapperConfiguration(cfg =>
                    //        {
                    //            cfg.CreateMap<AssignedItems, BB_Proposal_ItemDoBasket>();
                    //        });

                    //        IMapper iMapperItems = configpItemns.CreateMapper();

                    //        int DL_IDX = 0;

                    //        foreach (var dl in DLfromDraft)
                    //        {

                    //            if (assignItem.DeliveryLocationAssociated == dl.IDX)
                    //            {
                    //                // é equipamento
                    //                DL_IDX = db.BB_Proposal_DeliveryLocation.Where(x =>
                    //                            x.ProposalID == p.Draft.details.ID &&
                    //                            x.ID == dl.ID).Select(x => x.IDX).FirstOrDefault();

                    //                BB_Proposal_ItemDoBasket bb_Proposal_ItemDoBasket = iMapperItems.Map<AssignedItems, BB_Proposal_ItemDoBasket>(assignItem);
                    //                bb_Proposal_ItemDoBasket.DeliveryLocationID = DL_IDX;
                    //                db.BB_Proposal_ItemDoBasket.Add(bb_Proposal_ItemDoBasket);
                    //                try
                    //                {
                    //                    db.SaveChanges();
                    //                }
                    //                catch (Exception ex)
                    //                {
                    //                    ex.Message.ToString();
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

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
                        
                        if(p.Draft.shareProfileDelegation.Count != bB_Permissions_db.Count)
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


                    err.ProposalObj = new ProposalRootObject();
                    err.ProposalObj.Draft = p.Draft;
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

                BB_Proposal_Status statusProp = db.BB_Proposal_Status.Where(x => x.ID == proposal.StatusID).FirstOrDefault();
                err.ProposalObj.Draft.details = iMapper.Map<BB_Proposal, Details>(proposal);
                err.ProposalObj.Draft.details.Status = new ProposalStatus();
                err.ProposalObj.Draft.details.Status.Name = statusProp.Description;
                err.ProposalObj.Draft.details.Status.IsEdit = statusProp.BB_Edit;
                err.ProposalObj.Draft.details.Status.Phase = statusProp.Phase;
                err.ProposalObj.Draft.details.AccountManager = proposal.AccountManager;
                err.ProposalObj.Draft.details.CampaignID = proposal.CampaignID;

                err.ProposalObj.Draft.details.IsMultipleContract = proposal.IsMultipleContract ?? false;


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

                err.ProposalObj.Draft.upturns = new List<Upturn>();
                List<BB_Proposal_Upturn> bb_Proposal_Upturn = db.BB_Proposal_Upturn.Where(x => x.ProposalID == i.ProposalId).OrderBy(x => x.Position).ToList();
                foreach (BB_Proposal_Upturn item in bb_Proposal_Upturn)
                {
                    Upturn upturn = new Upturn
                    {
                        Contact = item.Contact,
                        Description = item.Description,
                        ID = item.ID,
                        Total = item.Total,
                        Type = item.Type
                    };
                    err.ProposalObj.Draft.upturns.Add(upturn);
                }

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

                    c.modeId = infCliente.IsClienteBB.GetValueOrDefault() ? 1 : 0;
                    err.ProposalObj.Draft.client = c;
                    err.ProposalObj.Draft.client.isNewClient = cli.IsNewClient;
                    err.ProposalObj.Draft.client.isPublicSector = cli.IsPublicSector;
                }
                else
                {
                    err.ProposalObj.Draft.client.accountnumber = "";
                    err.ProposalObj.Draft.client.isNewClient = false;
                    err.ProposalObj.Draft.client.isPublicSector = false;
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
                if (printingServices2 != null)
                {
                    PrintingServices2 proposalPS2 = new PrintingServices2()
                    {
                        ID = printingServices2.ID,
                        ActivePrintingService = printingServices2.ActivePrintingService,
                        ApprovedPrintingServices = new List<ApprovedPrintingService>(),
                        PendingServiceQuoteRequests = new List<ApprovedPrintingService>()
                    };
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
                                ClickPriceBW = machine.ApprovedBW,
                                ClickPriceC = machine.ApprovedC,
                                BWPVP = machine.BWPVP,
                                CPVP = machine.CPVP
                            };
                            newPS.Machines.Add(psMachine);
                        }
                        BB_Proposal_PrintingServiceValidationRequest validationRequest = ps.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.ToDelete == false).FirstOrDefault();
                        if (validationRequest != null)
                        {
                            newPS.RequestedAt = validationRequest.RequestedAt;
                            newPS.SCObservations = validationRequest.SCObservations;
                            newPS.SEObservations = validationRequest.SEObservations;
                            if (validationRequest.IsComplete.Value)
                            {
                                proposalPS2.ApprovedPrintingServices.Add(newPS);
                            }
                            else
                            {
                                proposalPS2.PendingServiceQuoteRequests.Add(newPS);
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
                    err.ProposalObj.ClientApproval.Documents = db.LD_DocumentProposal.Where(dp => dp.QuoteNumber == proposal.CRM_QUOTE_ID && dp.ClassificationID != 5).ToList();
                    err.ProposalObj.ClientApproval.DocumentTypes = db.LD_DocumentClassification.ToList();
                }

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


                //LD_DocumentProposal - Contractos
                err.ProposalObj.Draft.contracts = new BusinessContract();
                List<LD_DocumentProposal> contractDocs = db.LD_DocumentProposal.Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID && x.ClassificationID == 5).ToList();
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
                    IsMultipleContract = IsMultipleContract
                };

                if(bb_proposal != null)
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

                    //BB_PROPOSAL_Commission
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


                    List<Upturn> upturns = p.Draft.upturns;
                    foreach (Upturn ut in upturns)
                    {
                        if (ut.ID == null)
                        {
                            BB_Proposal_Upturn newUpturn = new BB_Proposal_Upturn
                            {
                                ProposalID = ProposalID,
                                Total = ut.Total,
                                Contact = ut.Contact,
                                Description = ut.Description,
                                Type = ut.Type,
                            };
                            db.BB_Proposal_Upturn.Add(newUpturn);
                            try
                            {
                                db.SaveChanges();
                                ut.ID = newUpturn.ID;
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }

                    //PRINTING SERVICES 
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

                    PrintingServices2 printingServices2 = p.Draft.printingServices2;
                    //int ps2Id = 0;
                    if (printingServices2 != null)
                    {
                        BB_Proposal_PrintingServices2 db_ps2 = new BB_Proposal_PrintingServices2()
                        {
                            ProposalID = ProposalID,
                            ActivePrintingService = printingServices2.ActivePrintingService
                        };
                        db.BB_Proposal_PrintingServices2.Add(db_ps2);
                        try
                        {
                            db.SaveChanges();
                            printingServices2.ID = db_ps2.ID;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
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
                                    Fee = aps.Fee,
                                };
                                db.BB_PrintingServices.Add(newPS);
                                try
                                {
                                    db.SaveChanges();
                                    aps.ID = newPS.ID;
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString();
                                }
                                if (aps.GlobalClickVVA != null)
                                {
                                    BB_VVA vva = new BB_VVA()
                                    {
                                        BWExcessPVP = aps.GlobalClickVVA.BWExcessPVP,
                                        CExcessPVP = aps.GlobalClickVVA.CExcessPVP,
                                        ExcessBillingFrequency = aps.GlobalClickVVA.ExcessBillingFrequency,
                                        ReturnType = aps.GlobalClickVVA.ReturnType,
                                        PVP = aps.GlobalClickVVA.PVP,
                                        RentBillingFrequency = aps.GlobalClickVVA.RentBillingFrequency,
                                        PrintingServiceID = newPS.ID,
                                    };
                                    db.BB_VVA.Add(vva);
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Message.ToString();
                                    }
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
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Message.ToString();
                                    }
                                }
                                if (aps.ClickPerModel != null)
                                {
                                    BB_PrintingServices_ClickPerModel cpm = new BB_PrintingServices_ClickPerModel()
                                    {
                                        PageBillingFrequency = aps.ClickPerModel.PageBillingFrequency,
                                        PrintingServiceID = newPS.ID,
                                    };
                                    db.BB_PrintingServices_ClickPerModel.Add(cpm);
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Message.ToString();
                                    }
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
                                            ApprovedBW = m.ClickPriceC,
                                            ApprovedC = m.ClickPriceC,
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
                            }
                        }
                    }

                    //BB_PROPOSAL_Cliente
                    BB_Proposal_Client cliente1 = new BB_Proposal_Client();
                    cliente1.ClientID = p.Draft.client.accountnumber;
                    cliente1.IsNewClient = p.Draft.client.isNewClient;
                    cliente1.ProposalID = ProposalID;
                    cliente1.Name = p.Draft.client.Name;
                    cliente1.IsPublicSector = p.Draft.client.isPublicSector;
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

                //BB_PROPOSAL_COnsigments
                var configConsigments = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Consignment, BB_Proposal_Consignments>();
                });

                IMapper iMapperConsigments = configConsigments.CreateMapper();

                BB_Proposal_Consignments consignment = iMapperConsigments.Map<Consignment, BB_Proposal_Consignments>(p.Draft.consignment);

                consignment.ProposalID = ProposalID;

                db.BB_Proposal_Consignments.Add(consignment);

                ////BB_Permissions
                //if (p.Draft.shareProfileDelegation != null)
                //{
                //    err.ProposalObj.Draft.shareProfileDelegation = p.Draft.shareProfileDelegation;
                //}
                //else
                //{
                //    err.ProposalObj.Draft.shareProfileDelegation = new List<BB_Permissions>();
                //}

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
                if(typeOfClient is null)
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
                } catch(Exception ex)
                {
                    throw ex;
                }


            }
            catch (Exception e)
            {
                e.Message.ToString();
            }
            err.ProposalObj = new ProposalRootObject();
            err.ProposalObj.Draft = p.Draft;
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
                        && lstSigningContactsDoc[0].Name != "" && lstSigningContactsDoc[0].Telefone != ""){
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
    }
}