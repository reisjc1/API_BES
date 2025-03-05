using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Collections.Generic;
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
                double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                var collectionConditions = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_Proposal_Quote> quote_lst = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                    int? numberOfMachines = 0;
                    foreach(var equip in quote_lst)
                    {
                        BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == equip.CodeRef).FirstOrDefault();

                        if(bB_Equipamentos != null)
                        {
                            numberOfMachines += equip.Qty;
                        }
                    }

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
                                string financingCode = ConditionMaterial(item.MATERIAL, contracts[0].VT_VTART);

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
                            bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).Skip(index).FirstOrDefault();
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
                                BB_Proposal_Condition_Type zvbs = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                                collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                                {
                                    DOC = order.SD_DOC,
                                    COND_FLAG = "A",
                                    KSCHL = zvbs != null ? zvbs.ConditionType : "ZVBS",
                                    KBETR = zvbs != null ? Math.Round(zvbs.ConditionValue / numberOfMachines ?? 0.0, 2).ToString("F2").Replace(",", ".") : "0.00"
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
                return collectionConditions;

            }catch(Exception e)
            {
                return null;
            }

        }
        public string ConditionMaterial(string codeRef, string financingType)
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
                BB_Data_Integration dataIntegration = db.BB_Data_Integration.Where(x => x.CodeRef == codeRef).FirstOrDefault();

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
                    List<BB_Proposal_Quote> quote_lst = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();


                    foreach (var items in orders)
                    {
                        foreach (var item in items.Items)
                        {

                            BB_Proposal_Quote quote = quote_lst.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                            //BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.CodeRef).FirstOrDefault();
                            ConditionPVP conditionPVP = new ConditionPVP();


                            if (quote != null)
                            {
                                //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                //{
                                ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                if (cond == null)
                                {
                                    conditionPVP.ConditionCode = "ZPD4";
                                    conditionPVP.PVP = quote.UnitDiscountPrice * item.Qty;
                                    conditionsPvp.Add(conditionPVP);
                                }
                                else
                                {
                                    cond.PVP += (quote.UnitDiscountPrice * item.Qty);
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


                            if (financingType == "003" || financingType == "005")
                            {
                                BB_Proposal_Quote quote1 = quote_lst.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                                string financingCode = ConditionMaterial(item.CodeRef, financingType);
                                BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();

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
                                                conditionPvp.PVP = (quote1.UnitDiscountPrice * item.Qty) * (pf.Factor / 100) + conditionPvp.PVP;
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = (quote1.UnitDiscountPrice * item.Qty) * (pf.Factor) + conditionPvp.PVP;
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
                                            conditionPvp.PVP = ((quote1.UnitDiscountPrice / quote1.Qty) / contractMonths) + conditionPvp.PVP;
                                        }


                                        conditionPvp.PVP = Convert.ToDouble(Math.Round(conditionPvp.PVP ?? 0.0, 2).ToString("F2")); ;
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
                                                    condPvp.PVP = (quote1.UnitDiscountPrice * item.Qty) * (pf.Factor / 100) + condPvp.PVP;
                                                }
                                                else
                                                {
                                                    condPvp.PVP = (quote1.UnitDiscountPrice * item.Qty) * (pf.Factor / 100);
                                                }
                                            }
                                            else
                                            {
                                                if (condPvp.PVP != null)
                                                {
                                                    condPvp.PVP = (quote1.UnitDiscountPrice * item.Qty) * (pf.Factor) + condPvp.PVP;
                                                }
                                                else
                                                {
                                                    condPvp.PVP = (quote1.UnitDiscountPrice * item.Qty) * (pf.Factor);
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
                                                condPvp.PVP = ((quote1.UnitDiscountPrice / quote1.Qty) / contractMonths) + condPvp.PVP;
                                            }
                                            else
                                            {
                                                condPvp.PVP = ((quote1.UnitDiscountPrice / quote1.Qty) / contractMonths);
                                            }

                                        }

                                        condPvp.PVP = Convert.ToDouble(Math.Round(condPvp.PVP ?? 0.0, 2).ToString("F2")); ;
                                        condPvp.ConditionCode = financingCode;
                                        conditionsPvp.Add(condPvp);
                                    }
                                }

                            }



                        }

                    }
                    var printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    int index = (int)printingService2ID.ActivePrintingService;
                    BB_PrintingServices bB_PrintingServices = null;
                    if (index > 1)
                    {
                        bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).Skip(index).FirstOrDefault();
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
                            BB_Proposal_Condition_Type zvbs = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                            ConditionPVP condPvp = new ConditionPVP();
                            condPvp.PVP = zvbs != null ? zvbs.ConditionValue : 0;
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