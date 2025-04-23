using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WebApplication1.Controllers;
using static WebApplication1.BLL.LeaseDeskBLL;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML.XML
{
    public class Conditions
    {
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS> ConfigConditions(System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> orders,
        System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> contracts, int proposalId, int? ftCode)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                var collectionConditions = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_Proposal_Quote> quote_lst = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                    List<BB_Data_Integration> dataIntegration_lst = db.BB_Data_Integration.AsNoTracking().ToList();
                    //int? numberOfMachines = 0;
                    //foreach(var equip in quote_lst)
                    //{
                    //    BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == equip.CodeRef).FirstOrDefault();

                    //    if(bB_Equipamentos != null)
                    //    {
                    //        numberOfMachines += equip.Qty;
                    //    }
                    //}
                    int? numberOfMachines = db.BB_Proposal_Quote
                                            .Where(x => x.Proposal_ID == proposalId)
                                            .Join(db.BB_Equipamentos,
                                            quote => quote.CodeRef,
                                            equip => equip.CodeRef,
                                            (quote, equip) => new { quote.Qty })
                                            .Sum(x => (int?)x.Qty) ?? 0;
                    foreach (var order in orders)
                    {
                        string condFlag = null;
                        List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
                        double? pvpItems = 0;
                        double? KBETR = 0;
                        double? totalPvp = 0;

                        foreach (var item in order.Z1ZVOE_ORDER_ITEMS)
                        {
                            //if (contracts[0].VT_VTART == "002" || contracts[0].VT_VTART == "005" || contracts[0].VT_VTART == "008")
                            //{
                            BB_Proposal_Quote quote = quote_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                            //BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.MATERIAL).FirstOrDefault();

                            ConditionPVP conditionPVP = new ConditionPVP();


                            if (quote != null)
                            {
                                //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                //{
                                ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                if (cond == null)
                                {
                                    conditionPVP.ConditionCode = "ZPD4";
                                    conditionPVP.PVP = Math.Round(quote.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) ?? 0.0, 2);
                                    conditionsPvp.Add(conditionPVP);
                                }
                                else
                                {
                                    cond.PVP = Math.Round((cond.PVP + (quote.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY))) ?? 0.0, 2);
                                }

                                //}
                                //else if(quote.Family.Contains("PSV") || quote.Family.Contains("MSV") || quote.Family.Contains("CSV")|| quote.Family.Contains("SV"))
                                //{
                                //    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                //    if (cond == null)
                                //    {
                                //        conditionPVP.ConditionCode = "ZSW4";
                                //        conditionPVP.PVP = Math.Round(quote.TotalNetsale ?? 0.0, 2);
                                //        conditionsPvp.Add(conditionPVP);
                                //    }
                                //    else
                                //    {
                                //        cond.PVP = Math.Round((cond.PVP + quote.TotalNetsale) ?? 0.0, 2);
                                //    }
                                //}
                            }
                            //if (ops != null)
                            //{
                            //    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                            //    if (cond == null)
                            //    {
                            //        conditionPVP.ConditionCode = "ZSW4";
                            //        conditionPVP.PVP = ops.PVP * ops.Quantity;
                            //        conditionsPvp.Add(conditionPVP);
                            //    }
                            //    else
                            //    {
                            //        cond.PVP = cond.PVP + (ops.PVP * ops.Quantity);
                            //    }
                            //}
                            //}
                            if (contracts[0].VT_VTART == "003" || contracts[0].VT_VTART == "005")
                            {
                                //BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                                BB_Proposal_Quote quote1 = quote_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                                //BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(x => x.CodeRef == item.MATERIAL && x.ProposalID == proposalId).FirstOrDefault();
                                BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                                string financingCode = ConditionMaterial(item.MATERIAL, contracts[0].VT_VTART, dataIntegration_lst);

                                ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);
                                if (quote1 != null)
                                {
                                    if (conditionPvp != null)
                                    {
                                        if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                        {
                                            //if (ftCode == 5)
                                            //{
                                            if (pf.Factor > 0)
                                            {
                                                conditionPvp.PVP = Math.Round((((quote1.TCP + (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY))) * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = Math.Round((((quote1.TCP + (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY))) * (pf.Factor)) + conditionPvp.PVP) ?? 0.0, 2);
                                            }

                                            //}
                                            //else
                                            //{
                                            //    if (pf.Factor > 0)
                                            //    {
                                            //        totalPvp = Math.Round((((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) + totalPvp) ?? 0.0, 2);
                                            //    }
                                            //    else
                                            //    {
                                            //        totalPvp = Math.Round((((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor)) + totalPvp) ?? 0.0, 2);
                                            //    }

                                            //}
                                        }
                                        else
                                        {
                                            conditionPvp.PVP = Math.Round((((quote1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY)) / pf.Months) + conditionPvp.PVP) ?? 0.0, 2);
                                        }
                                        //totalPvp = Math.Round(totalPvp ?? 0.0, 2);
                                        //conditionPvp.PVP = totalPvp;
                                    }
                                    else
                                    {

                                        ConditionPVP condPvp = new ConditionPVP();

                                        totalPvp = 0;
                                        if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                        {
                                            //if (ftCode == 5)
                                            //{
                                            if (pf.Factor > 0)
                                            {
                                                condPvp.PVP = Math.Round(((quote1.TCP + (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY))) * (pf.Factor / 100)) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                condPvp.PVP = Math.Round(((quote1.TCP + (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY))) * (pf.Factor)) ?? 0.0, 2);
                                            }

                                            //}
                                            //else
                                            //{
                                            //    if (pf.Factor > 0)
                                            //    {
                                            //        totalPvp = Math.Round(((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) ?? 0.0, 2);
                                            //    }
                                            //    else
                                            //    {
                                            //        totalPvp = Math.Round(((quote.UnitDiscountPrice / contratoMeses) * (pf.Factor)) ?? 0.0, 2);
                                            //    }

                                            //}
                                        }
                                        else
                                        {
                                            condPvp.PVP = Math.Round(((quote1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months)) ?? 0.0, 2);
                                        }

                                        //condPvp.PVP = Math.Round(totalPvp ?? 0.0, 2);
                                        condPvp.ConditionCode = financingCode;
                                        if (condPvp.ConditionCode != null)
                                        {
                                            conditionsPvp.Add(condPvp);
                                        }

                                    }
                                }
                                //if (ops != null)
                                //{
                                //    if (conditionPvp != null)
                                //    {
                                //        double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                                //        totalPvp = (ops.UnitDiscountPrice / contratoMeses) * pf.Factor;
                                //        conditionPvp.PVP = conditionPvp.PVP + totalPvp;
                                //    }
                                //    else
                                //    {
                                //        ConditionPVP condPvp = new ConditionPVP();
                                //        double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                                //        totalPvp = (ops.UnitDiscountPrice / contratoMeses) * pf.Factor;

                                //        condPvp.PVP = totalPvp;
                                //        condPvp.ConditionCode = financingCode;
                                //        conditionsPvp.Add(condPvp);
                                //    }
                                //}


                            }
                        }

                        var printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                        int index = (int)printingService2ID.ActivePrintingService;
                        BB_PrintingServices bB_PrintingServices = null;
                        if (index > 1)
                        {
                            bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).OrderBy(x => x.ID).Skip(index - 1).FirstOrDefault();
                        }
                        else
                        {
                            bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).FirstOrDefault();
                        }

                        if (bB_PrintingServices != null)
                        {
                            BB_VVA bB_VVA = db.BB_VVA.Where(x => x.PrintingServiceID == bB_PrintingServices.ID).FirstOrDefault();

                            if (bB_VVA != null)
                            {
                                //BB_Proposal_Condition_Type zvbs = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                                collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                                {
                                    DOC = order.SD_DOC,
                                    COND_FLAG = "A",
                                    KSCHL = "ZVBS",
                                    KBETR = bB_VVA.PVP != 0 ? Math.Round(bB_VVA.PVP / numberOfMachines ?? 0.0, 2).ToString("F2").Replace(",", ".") : "0.00"
                                });
                            }
                        }



                        foreach (var condition in conditionsPvp)
                        {
                            if (condition.ConditionCode == "ZPD4" || condition.ConditionCode == "ZSW4")
                            {
                                condFlag = "O";
                            }
                            else
                            {
                                condFlag = "A";
                            }
                            collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                            {
                                DOC = order.SD_DOC,
                                COND_FLAG = condFlag,
                                KSCHL = condition.ConditionCode,
                                KBETR = Math.Round(condition.PVP ?? 0.0, 2).ToString("F2").Replace(",", ".")
                            });
                        }

                    }
                }

                stopwatch.Stop();

                Console.WriteLine($"Order - ConfigOrders: {stopwatch.ElapsedMilliseconds} ms");

                return collectionConditions;

            }catch(Exception e)
            {
                return null;
            }

        }

        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS> ConfigConditionsV2(System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> orders,
        System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> contracts, int proposalId, int? ftCode, BB_Proposal_Financing pf)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                var collectionConditions = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_Proposal_Quote> quote_lst = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                    List<BB_Proposal_Quote_RS> quoteRs_lst = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).ToList();
                    List<BB_Data_Integration> dataIntegration_lst = db.BB_Data_Integration.AsNoTracking().ToList();
                    var printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<BB_PrintingServices> bB_PrintingServices_lst = db.BB_PrintingServices.AsNoTracking().ToList();
                    List<BB_VVA> bB_VVA_lst = db.BB_VVA.AsNoTracking().ToList();
                    BB_Proposal_OPSManage opsM = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    //int? numberOfMachines = 0;
                    //foreach(var equip in quote_lst)
                    //{
                    //    BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == equip.CodeRef).FirstOrDefault();

                    //    if(bB_Equipamentos != null)
                    //    {
                    //        numberOfMachines += equip.Qty;
                    //    }
                    //}
                    int? numberOfMachines = db.BB_Proposal_Quote
                                            .Where(x => x.Proposal_ID == proposalId)
                                            .Join(db.BB_Equipamentos,
                                            quote => quote.CodeRef,
                                            equip => equip.CodeRef,
                                            (quote, equip) => new { quote.Qty })
                                            .Sum(x => (int?)x.Qty) ?? 0;
                    foreach (var order in orders)
                    {
                        string condFlag = null;
                        List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
                        double? pvpItems = 0;
                        double? KBETR = 0;
                        double? totalPvp = 0;

                        foreach (var item in order.Z1ZVOE_ORDER_ITEMS)
                        {
                            //if (contracts[0].VT_VTART == "002" || contracts[0].VT_VTART == "005" || contracts[0].VT_VTART == "008")
                            //{
                            BB_Proposal_Quote quote = new BB_Proposal_Quote();
                            //BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.MATERIAL).FirstOrDefault();

                            if (order.USED_MACHINE == "1" && item.ITM_NUMBER == "10") {
                                quote = quote_lst.Where(x => x.CodeRef == item.MATERIAL && x.IsUsed == true).FirstOrDefault();
                            }
                            else
                            {
                                quote = quote_lst.Where(x => x.CodeRef == item.MATERIAL && x.IsUsed == false).FirstOrDefault();
                            }
                            ConditionPVP conditionPVP = new ConditionPVP();


                            if (quote != null)
                            {
                                //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                //{
                                ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                if (cond == null)
                                {
                                    conditionPVP.ConditionCode = "ZPD4";
                                    conditionPVP.PVP = Math.Round(quote.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) ?? 0.0, 2);
                                    conditionsPvp.Add(conditionPVP);
                                }
                                else
                                {
                                    cond.PVP = Math.Round((cond.PVP + (quote.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY))) ?? 0.0, 2);
                                }

                                //}
                                //else if(quote.Family.Contains("PSV") || quote.Family.Contains("MSV") || quote.Family.Contains("CSV")|| quote.Family.Contains("SV"))
                                //{
                                //    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                //    if (cond == null)
                                //    {
                                //        conditionPVP.ConditionCode = "ZSW4";
                                //        conditionPVP.PVP = Math.Round(quote.TotalNetsale ?? 0.0, 2);
                                //        conditionsPvp.Add(conditionPVP);
                                //    }
                                //    else
                                //    {
                                //        cond.PVP = Math.Round((cond.PVP + quote.TotalNetsale) ?? 0.0, 2);
                                //    }
                                //}
                            }
                            else
                            {
                                BB_Proposal_Quote_RS quoteRS = quoteRs_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                                ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                if(quoteRS != null)
                                {
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = Math.Round((quoteRS.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) ?? 0.0, 2);
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = Math.Round((cond.PVP + ((quoteRS.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)))) ?? 0.0, 2);
                                    }
                                }
                                else
                                {
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = Math.Round((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * opsM.TotalMonths  ?? 0.0, 2);
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = Math.Round((cond.PVP + ((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * opsM.TotalMonths)) ?? 0.0, 2);
                                    }
                                }
                            }
                            //if (ops != null)
                            //{
                            //    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                            //    if (cond == null)
                            //    {
                            //        conditionPVP.ConditionCode = "ZSW4";
                            //        conditionPVP.PVP = ops.PVP * ops.Quantity;
                            //        conditionsPvp.Add(conditionPVP);
                            //    }
                            //    else
                            //    {
                            //        cond.PVP = cond.PVP + (ops.PVP * ops.Quantity);
                            //    }
                            //}
                            //}
                            if (contracts[0].VT_VTART == "003" || contracts[0].VT_VTART == "005")
                            {
                                //BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                BB_Proposal_Quote quote1 = quote_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                                BB_Proposal_Quote_RS quoteRS1 = quoteRs_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();

                                //BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(x => x.CodeRef == item.MATERIAL && x.ProposalID == proposalId).FirstOrDefault();
                                //BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                                string financingCode = ConditionMaterial(item.MATERIAL, contracts[0].VT_VTART, dataIntegration_lst);

                                ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);
                                if (quote1 != null)
                                {
                                    if (conditionPvp != null)
                                    {
                                        if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                        {
                                            //if (ftCode == 5)
                                            //{
                                            if (pf.Factor > 0)
                                            {
                                                conditionPvp.PVP = Math.Round((((quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = Math.Round((((quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor)) + conditionPvp.PVP) ?? 0.0, 2);
                                            }

                                            //}
                                            //else
                                            //{
                                            //    if (pf.Factor > 0)
                                            //    {
                                            //        totalPvp = Math.Round((((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) + totalPvp) ?? 0.0, 2);
                                            //    }
                                            //    else
                                            //    {
                                            //        totalPvp = Math.Round((((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor)) + totalPvp) ?? 0.0, 2);
                                            //    }

                                            //}
                                        }
                                        else
                                        {
                                            conditionPvp.PVP = Math.Round((((quote1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY)) / pf.Months) + conditionPvp.PVP) ?? 0.0, 2);
                                        }
                                        //totalPvp = Math.Round(totalPvp ?? 0.0, 2);
                                        //conditionPvp.PVP = totalPvp;
                                    }
                                    else
                                    {

                                        ConditionPVP condPvp = new ConditionPVP();

                                        totalPvp = 0;
                                        if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                        {
                                            //if (ftCode == 5)
                                            //{
                                            if (pf.Factor > 0)
                                            {
                                                condPvp.PVP = Math.Round(((quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                condPvp.PVP = Math.Round(((quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor)) ?? 0.0, 2);
                                            }

                                            //}
                                            //else
                                            //{
                                            //    if (pf.Factor > 0)
                                            //    {
                                            //        totalPvp = Math.Round(((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) ?? 0.0, 2);
                                            //    }
                                            //    else
                                            //    {
                                            //        totalPvp = Math.Round(((quote.UnitDiscountPrice / contratoMeses) * (pf.Factor)) ?? 0.0, 2);
                                            //    }

                                            //}
                                        }
                                        else
                                        {
                                            condPvp.PVP = Math.Round(((quote1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months)) ?? 0.0, 2);
                                        }

                                        //condPvp.PVP = Math.Round(totalPvp ?? 0.0, 2);
                                        condPvp.ConditionCode = financingCode;
                                        if (condPvp.ConditionCode != null)
                                        {
                                            conditionsPvp.Add(condPvp);
                                        }

                                    }
                                }
                                else
                                {
                                    if(quoteRS1 != null)
                                    {
                                        if (conditionPvp != null)
                                        {
                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                //if (ftCode == 5)
                                                //{
                                                if (pf.Factor > 0)
                                                {
                                                    conditionPvp.PVP = Math.Round((((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                                }
                                                else
                                                {
                                                    conditionPvp.PVP = Math.Round((((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor)) + conditionPvp.PVP) ?? 0.0, 2);
                                                }

                                                //}
                                                //else
                                                //{
                                                //    if (pf.Factor > 0)
                                                //    {
                                                //        totalPvp = Math.Round((((opsM.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) + totalPvp) ?? 0.0, 2);
                                                //    }
                                                //    else
                                                //    {
                                                //        totalPvp = Math.Round((((opsM.UnitDiscountPrice / contratoMeses) * (pf.Factor)) + totalPvp) ?? 0.0, 2);
                                                //    }

                                                //}
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = Math.Round((((quoteRS1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY)) / pf.Months) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                            //totalPvp = Math.Round(totalPvp ?? 0.0, 2);
                                            //conditionPvp.PVP = totalPvp;
                                        }
                                        else
                                        {

                                            ConditionPVP condPvp = new ConditionPVP();

                                            totalPvp = 0;
                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                //if (ftCode == 5)
                                                //{
                                                if (pf.Factor > 0)
                                                {
                                                    condPvp.PVP = Math.Round(((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) ?? 0.0, 2);
                                                }
                                                else
                                                {
                                                    condPvp.PVP = Math.Round(((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor)) ?? 0.0, 2);
                                                }

                                                //}
                                                //else
                                                //{
                                                //    if (pf.Factor > 0)
                                                //    {
                                                //        totalPvp = Math.Round(((opsM.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) ?? 0.0, 2);
                                                //    }
                                                //    else
                                                //    {
                                                //        totalPvp = Math.Round(((quote.UnitDiscountPrice / contratoMeses) * (pf.Factor)) ?? 0.0, 2);
                                                //    }

                                                //}
                                            }
                                            else
                                            {
                                                condPvp.PVP = Math.Round(((quoteRS1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months)) ?? 0.0, 2);
                                            }

                                            //condPvp.PVP = Math.Round(totalPvp ?? 0.0, 2);
                                            condPvp.ConditionCode = financingCode;
                                            if (condPvp.ConditionCode != null)
                                            {
                                                conditionsPvp.Add(condPvp);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (conditionPvp != null)
                                        {
                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                //if (ftCode == 5)
                                                //{
                                                if (pf.Factor > 0)
                                                {
                                                    conditionPvp.PVP = Math.Round((((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                                }
                                                else
                                                {
                                                    conditionPvp.PVP = Math.Round((((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor)) + conditionPvp.PVP) ?? 0.0, 2);
                                                }

                                                //}
                                                //else
                                                //{
                                                //    if (pf.Factor > 0)
                                                //    {
                                                //        totalPvp = Math.Round((((opsM.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) + totalPvp) ?? 0.0, 2);
                                                //    }
                                                //    else
                                                //    {
                                                //        totalPvp = Math.Round((((opsM.UnitDiscountPrice / contratoMeses) * (pf.Factor)) + totalPvp) ?? 0.0, 2);
                                                //    }

                                                //}
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = Math.Round((((opsM.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY)) / pf.Months) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                            //totalPvp = Math.Round(totalPvp ?? 0.0, 2);
                                            //conditionPvp.PVP = totalPvp;
                                        }
                                        else
                                        {

                                            ConditionPVP condPvp = new ConditionPVP();

                                            totalPvp = 0;
                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                //if (ftCode == 5)
                                                //{
                                                if (pf.Factor > 0)
                                                {
                                                    condPvp.PVP = Math.Round(((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) ?? 0.0, 2);
                                                }
                                                else
                                                {
                                                    condPvp.PVP = Math.Round(((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor)) ?? 0.0, 2);
                                                }

                                                //}
                                                //else
                                                //{
                                                //    if (pf.Factor > 0)
                                                //    {
                                                //        totalPvp = Math.Round(((opsM.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) ?? 0.0, 2);
                                                //    }
                                                //    else
                                                //    {
                                                //        totalPvp = Math.Round(((quote.UnitDiscountPrice / contratoMeses) * (pf.Factor)) ?? 0.0, 2);
                                                //    }

                                                //}
                                            }
                                            else
                                            {
                                                condPvp.PVP = Math.Round(((opsM.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months)) ?? 0.0, 2);
                                            }

                                            //condPvp.PVP = Math.Round(totalPvp ?? 0.0, 2);
                                            condPvp.ConditionCode = financingCode;
                                            if (condPvp.ConditionCode != null)
                                            {
                                                conditionsPvp.Add(condPvp);
                                            }

                                        }
                                    }
                                }
                               

                            }
                        }

                        //var printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                        int index = (int)printingService2ID.ActivePrintingService;
                        BB_PrintingServices bB_PrintingServices = null;
                        if (index > 1)
                        {
                            bB_PrintingServices = bB_PrintingServices_lst.Where(x => x.PrintingServices2ID == printingService2ID.ID).OrderBy(x => x.ID).Skip(index - 1).FirstOrDefault();
                        }
                        else
                        {
                            bB_PrintingServices = bB_PrintingServices_lst.Where(x => x.PrintingServices2ID == printingService2ID.ID).FirstOrDefault();
                        }

                        if (bB_PrintingServices != null)
                        {
                            BB_VVA bB_VVA = bB_VVA_lst.Where(x => x.PrintingServiceID == bB_PrintingServices.ID).FirstOrDefault();

                            if (bB_VVA != null)
                            {
                                //BB_Proposal_Condition_Type zvbs = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                                collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                                {
                                    DOC = order.SD_DOC,
                                    COND_FLAG = "A",
                                    KSCHL = "ZVBS",
                                    KBETR = bB_VVA.PVP != 0 ? Math.Round(bB_VVA.PVP / numberOfMachines ?? 0.0, 2).ToString("F2").Replace(",", ".") : "0.00"
                                });
                            }
                        }



                        foreach (var condition in conditionsPvp)
                        {
                            if (condition.ConditionCode == "ZPD4" || condition.ConditionCode == "ZSW4")
                            {
                                condFlag = "O";
                            }
                            else
                            {
                                condFlag = "A";
                            }
                            collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                            {
                                DOC = order.SD_DOC,
                                COND_FLAG = condFlag,
                                KSCHL = condition.ConditionCode,
                                KBETR = Math.Round(condition.PVP ?? 0.0, 2).ToString("F2").Replace(",", ".")
                            });
                        }

                    }
                }

                stopwatch.Stop();

                Console.WriteLine($"Order - ConfigOrders: {stopwatch.ElapsedMilliseconds} ms");

                return collectionConditions;

            }
            catch (Exception e)
            {
                return null;
            }

        }
        public string ConditionMaterial(string codeRef, string financingType, List<BB_Data_Integration> dataIntegration_lst)
        //public void ConditionMaterial(string codeRef, string financingType)
        {


            string financingCode = null;

            //if (financingType == "008")
            //{
            //    financingCode = "ZVBS";
            //}

            if (financingType == "003")
            {
                financingCode = "ZVBR";
            }
            if (financingType == "005")
            {
                financingCode = "ZVBA";
            }

            using (var db = new BB_DB_DEVEntities2())
            {
                BB_Data_Integration dataIntegration = dataIntegration_lst.Where(x => x.CodeRef == codeRef).FirstOrDefault();

                if (dataIntegration != null)
                {
                    if ((financingType == "003") && !string.IsNullOrWhiteSpace(dataIntegration.COND_TYPE_RENTAL))
                    {
                        financingCode = dataIntegration.COND_TYPE_RENTAL;
                    }
                    if (financingType == "005" && !string.IsNullOrWhiteSpace(dataIntegration.COND_TYPE_AL))
                    {
                        financingCode = dataIntegration.COND_TYPE_AL;
                    }
                }


            }

            return financingCode;
        }
        public List<ConditionPVP> ConditionsVariables(List<ItemGroups> orders, string financingType, int? contractMonths, int? proposalId, int? ftCode)
        {
            try
            {
                List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
                List<ConditionPVP> conditionsPvpAux = new List<ConditionPVP>();

                double? pvpItems = 0;
                double? totalPvp = 0;
                //foreach (var order in orders)
                //{

                using (var db = new BB_DB_DEVEntities2())
                {
                    //List<BB_Proposal_Quote> quote_lst = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<BB_Data_Integration> dataIntegration_lst = db.BB_Data_Integration.AsNoTracking().ToList();


                    foreach (var items in orders)
                    {
                        foreach (var item in items.Items)
                        {

                            //BB_Proposal_Quote quote = quote_lst.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                            ConditionPVP conditionPVP = new ConditionPVP();

                                if(item.TotalMonths == 0)
                                {

                                    //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                    //{
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = item.UnitDiscountPrice * item.Qty;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP += (item.UnitDiscountPrice * item.Qty);
                                    }

                                //}
                                //else
                                //{
                                //    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                //    if (cond == null)
                                //    {
                                //        conditionPVP.ConditionCode = "ZSW4";
                                //        conditionPVP.PVP = quote.UnitDiscountPrice;
                                //        conditionsPvp.Add(conditionPVP);
                                //    }
                                //    else
                                //    {
                                //        cond.PVP = cond.PVP + quote.UnitDiscountPrice;
                                //    }
                                //}
                                }
                                else
                                {
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = (item.UnitDiscountPrice * item.Qty) * item.TotalMonths;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP += (item.UnitDiscountPrice * item.Qty) * item.TotalMonths;
                                }
                                }
                            //if (quote != null)
                            //{
                            //}


                            if (financingType == "003" || financingType == "005")
                            {
                                //BB_Proposal_Quote quote1 = quote_lst.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                                string financingCode = ConditionMaterial(item.CodeRef, financingType, dataIntegration_lst);

                                ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);
                                //if (quote1 != null)
                                //{
                                //}
                                if (conditionPvp != null)
                                {

                                    if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                    {
                                        //if (ftCode == 5)
                                        //{
                                        if (pf.Factor > 0)
                                        {
                                            if(item.LPI > 0)
                                            {
                                                conditionPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor / 100) + conditionPvp.PVP;
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor / 100) + conditionPvp.PVP;
                                            }
                                        }
                                        else
                                        {
                                            if (item.LPI > 0)
                                            {
                                                conditionPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor) + conditionPvp.PVP;
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor) + conditionPvp.PVP;

                                            }
                                        }

                                        //}
                                        //else
                                        //{
                                        //    if (pf.Factor > 0)
                                        //    {
                                        //        totalPvp = (quote1.UnitDiscountPrice / contractMonths) * (pf.Factor / 100);
                                        //    }
                                        //    else
                                        //    {
                                        //        totalPvp = (quote1.UnitDiscountPrice / contractMonths) * (pf.Factor);
                                        //    }

                                        //}
                                    }
                                    else
                                    {
                                        if (item.TotalMonths == 0)
                                        {
                                            conditionPvp.PVP = ((item.UnitDiscountPrice / item.Qty) / contractMonths) + conditionPvp.PVP;
                                        }
                                        else
                                        {
                                            conditionPvp.PVP = (item.UnitDiscountPrice / item.Qty) + conditionPvp.PVP;
                                        }
                                    }


                                    conditionPvp.PVP = Convert.ToDouble(conditionPvp.PVP);
                                    //conditionPvp.PVP = totalPvp;
                                }
                                else
                                {
                                    ConditionPVP condPvp = new ConditionPVP();
                                    totalPvp = 0;
                                    if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                    {
                                        //if (ftCode == 5)
                                        //{
                                        if (pf.Factor > 0)
                                        {
                                            if (condPvp.PVP != null)
                                            {
                                                if (item.LPI > 0)
                                                {
                                                    condPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor / 100) + condPvp.PVP;
                                                }
                                                else
                                                {
                                                    condPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor / 100) + condPvp.PVP;
                                                }
                                            }
                                            else
                                            {
                                                if (item.LPI > 0)
                                                {
                                                    condPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor / 100);
                                                }
                                                else
                                                {
                                                    condPvp.PVP = (item.LPI + (item.UnitDiscountPrice * item.Qty)) * (pf.Factor / 100);

                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (condPvp.PVP != null)
                                            {
                                                condPvp.PVP = (item.UnitDiscountPrice * item.Qty) * (pf.Factor) + condPvp.PVP;
                                            }
                                            else
                                            {
                                                condPvp.PVP = (item.UnitDiscountPrice * item.Qty) * (pf.Factor);
                                            }
                                        }

                                        //}
                                        //else
                                        //{
                                        //    if(pf.Factor > 0)
                                        //    {
                                        //        totalPvp = (quote1.UnitDiscountPrice / contractMonths) * (pf.Factor / 100);
                                        //    }
                                        //    else
                                        //    {
                                        //        totalPvp = (quote1.UnitDiscountPrice / contractMonths) * (pf.Factor);
                                        //    }

                                        //}
                                    }
                                    else
                                    {
                                        if (condPvp.PVP != null)
                                        {
                                            condPvp.PVP = ((item.UnitDiscountPrice / item.Qty) / contractMonths) + condPvp.PVP;
                                        }
                                        else
                                        {
                                            if (item.TotalMonths == 0)
                                            {
                                                condPvp.PVP = ((item.UnitDiscountPrice / item.Qty) / contractMonths);
                                            }
                                            else
                                            {
                                                condPvp.PVP = (item.UnitDiscountPrice / item.Qty);
                                            }
                                        }

                                    }

                                    condPvp.PVP = Convert.ToDouble(condPvp.PVP); 
                                    condPvp.ConditionCode = financingCode;
                                    conditionsPvp.Add(condPvp);
                                }

                            }



                        }

                    }
                    var printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    int index = (int)printingService2ID.ActivePrintingService;
                    BB_PrintingServices bB_PrintingServices = null;
                    if (index > 1)
                    {
                        bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).OrderBy(x => x.ID).Skip(index - 1).FirstOrDefault();
                    }
                    else
                    {
                        bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).FirstOrDefault();
                    }

                    if (bB_PrintingServices != null)
                    {
                        BB_VVA bB_VVA = db.BB_VVA.Where(x => x.PrintingServiceID == bB_PrintingServices.ID).FirstOrDefault();

                        if (bB_VVA != null)
                        {
                            //BB_Proposal_Condition_Type zvbs = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                            ConditionPVP condPvp = new ConditionPVP();
                            condPvp.PVP = Convert.ToDouble(Math.Round(bB_VVA.PVP ?? 0.0, 2).ToString("F2"));
                            condPvp.ConditionCode = "ZVBS";
                            conditionsPvp.Add(condPvp);
                        }
                    }
                }
            
                return conditionsPvp;
            }catch(Exception e)
            {
                string err = e.Message;
                return null;
            }

        }




    }
}