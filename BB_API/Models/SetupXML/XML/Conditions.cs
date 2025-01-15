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
            double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
            var collectionConditions = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS>();
            foreach (var order in orders)
            {
                string condFlag = null;
                List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
                double? pvpItems = 0;
                double? KBETR = 0;
                double? totalPvp = 0;
                using (var db = new BB_DB_DEVEntities2())
                {
                    foreach (var item in order.Z1ZVOE_ORDER_ITEMS)
                    {
                        //if (contracts[0].VT_VTART == "002" || contracts[0].VT_VTART == "005" || contracts[0].VT_VTART == "008")
                        //{
                            BB_Proposal_Quote quote = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.MATERIAL && x.Proposal_ID == proposalId ).FirstOrDefault();
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
                                        conditionPVP.PVP = Math.Round(quote.TotalNetsale ?? 0.0, 2);
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = Math.Round((cond.PVP + quote.TotalNetsale) ?? 0.0, 2);
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
                        if (contracts[0].VT_VTART != "002")
                        {
                            //BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                            BB_Proposal_Quote quote1 = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.MATERIAL && x.Proposal_ID == proposalId).FirstOrDefault();
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
                                        if (ftCode == 5)
                                        {
                                            if (pf.Factor > 0)
                                            {
                                                totalPvp = Math.Round(((quote1.UnitDiscountPrice * (pf.Factor / 100)) + totalPvp) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                totalPvp = Math.Round(((quote1.UnitDiscountPrice * (pf.Factor)) + totalPvp) ?? 0.0, 2);
                                            }

                                        }
                                        else
                                        {
                                            if (pf.Factor > 0)
                                            {
                                                totalPvp = Math.Round((((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) + totalPvp) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                totalPvp = Math.Round((((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor)) + totalPvp) ?? 0.0, 2);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        totalPvp = Math.Round(((quote1.UnitDiscountPrice / contratoMeses) + totalPvp) ?? 0.0, 2);
                                    }
                                    totalPvp = Math.Round(totalPvp ?? 0.0, 2);
                                    conditionPvp.PVP = totalPvp;
                                }
                                else
                                {

                                    ConditionPVP condPvp = new ConditionPVP();
                               
                                    totalPvp = 0;
                                    if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                    {
                                        if (ftCode == 5)
                                        {
                                            if (pf.Factor > 0)
                                            {
                                                totalPvp = Math.Round(((quote1.UnitDiscountPrice) * (pf.Factor / 100)) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                totalPvp = Math.Round(((quote1.UnitDiscountPrice) * (pf.Factor)) ?? 0.0, 2);
                                            }

                                        }
                                        else
                                        {
                                            if (pf.Factor > 0)
                                            {
                                                totalPvp = Math.Round(((quote1.UnitDiscountPrice / contratoMeses) * (pf.Factor / 100)) ?? 0.0, 2);
                                            }
                                            else
                                            {
                                                totalPvp = Math.Round(((quote.UnitDiscountPrice / contratoMeses) * (pf.Factor)) ?? 0.0, 2);
                                            }
                                            
                                        }
                                    }
                                    else
                                    {
                                        totalPvp = Math.Round(((quote1.UnitDiscountPrice / contratoMeses)) ?? 0.0, 2);
                                    }

                                    condPvp.PVP = Math.Round(totalPvp ?? 0.0, 2);
                                    condPvp.ConditionCode = financingCode;
                                    if(condPvp.ConditionCode != null)
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
                    BB_Proposal_OPSManage ops = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    
                    if (ops != null)
                    {
                        string line1 = ops.CodeRef;
                        string line2 = "9960DRC-HTTP ";
                        //BB_OPS_Manage_Packs opsPack = db.BB_OPS_Manage_Packs.Where(x => x.CodeRef == line2).FirstOrDefault();
                        BB_OPS_Manage_Packs opsPack = db.BB_OPS_Manage_Packs.Where(x => x.CodeRef == line1).FirstOrDefault();
                        //Get ops package from configurator
                        BB_Proposal_Quote OPSPack = db.BB_Proposal_Quote.Where(x => x.CodeRef == line1 && x.Proposal_ID == proposalId).FirstOrDefault();

                        if(OPSPack == null && ops.UnitDiscountPrice != 0)
                        {
                            //double? opsPvp = (ops.PVP * ops.TotalMonths) - opsPack.PVP;
                            //double? opsPvpLine2 = opsPack.PVP;
                            double? opsPvp = (ops.PVP * ops.TotalMonths* ops.Quantity);

                            //bool condExists = false;
                            foreach (var cond in conditionsPvp)
                            {
                                if (cond.ConditionCode == "ZPD4")
                                {
                                    //condExists = true;
                                    //double opsPvpRounded = Math.Round((opsPvp / contractMonths) ?? 0.0, 2);
                                    cond.PVP = cond.PVP + (opsPvp);
                                }

                            }

                        }
                        else if (OPSPack == null && ops.UnitDiscountPrice == 0)
                        {
                            //double? opsPvp = (ops.PVP * ops.TotalMonths) - opsPack.PVP;
                            //double? opsPvpLine2 = opsPack.PVP;
                            double? opsPvp = (ops.PVP * ops.TotalMonths);

                            //bool condExists = false;
                            foreach (var cond in conditionsPvp)
                            {
                                if (cond.ConditionCode == "ZPD4")
                                {
                                    //condExists = true;
                                    //double opsPvpRounded = Math.Round((opsPvp / contractMonths) ?? 0.0, 2);
                                    cond.PVP = cond.PVP + (opsPvp);
                                }

                            }

                        }
                        else
                        {
                            
                            foreach (var cond in conditionsPvp)
                            {
                                if (cond.ConditionCode == "ZPD4")
                                {
                                    //condExists = true;
                                    cond.PVP = cond.PVP + (OPSPack.TotalNetsale);
                                }

                            }
                        }
                    }

                    int printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).Select(x => x.ID).FirstOrDefault();

                    BB_PrintingServices bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID).FirstOrDefault();

                    BB_VVA bB_VVA = db.BB_VVA.Where(x => x.PrintingServiceID == bB_PrintingServices.ID).FirstOrDefault();

                    if(bB_VVA != null)
                    {
                        BB_Proposal_Condition_Type zvbs = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                        collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                        {
                            DOC = order.SD_DOC,
                            COND_FLAG = "A",
                            KSCHL = zvbs != null ? zvbs.ConditionType : "ZVBS",
                            KBETR = zvbs != null ? Math.Round(zvbs.ConditionValue ?? 0.0, 2).ToString("F2").Replace(",", ".") : "0.00"
                        });
                    }

                }
                //if (contracts[0].VT_VTART == "003" || contracts[0].VT_VTART == "002") //Renting
                //{
                //    condFlag = "O";
                //}
                //if (contracts[0].VT_VTART == "005") //AL
                //{
                //    condFlag = "A";
                //}
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
            return collectionConditions;

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
            List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
            List<ConditionPVP> conditionsPvpAux = new List<ConditionPVP>();

            double? pvpItems = 0;
            double? totalPvp = 0;
            //foreach (var order in orders)
            //{

                using (var db = new BB_DB_DEVEntities2())
                {
                    foreach (var items in orders)
                    {
                        foreach (var item in items.Items)
                        {
                                BB_Proposal_Quote quote = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.CodeRef && x.Proposal_ID == proposalId).FirstOrDefault();
                                //BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.CodeRef).FirstOrDefault();
                                ConditionPVP conditionPVP = new ConditionPVP();


                                if (quote != null)
                                {
                                    if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                    {
                                        ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                        if (cond == null)
                                        {
                                            conditionPVP.ConditionCode = "ZPD4";
                                            conditionPVP.PVP = quote.TotalNetsale;
                                            conditionsPvp.Add(conditionPVP);
                                        }
                                        else
                                        {
                                            cond.PVP = cond.PVP + quote.TotalNetsale;
                                        }

                                    }
                                    else
                                    {
                                        ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                        if (cond == null)
                                        {
                                            conditionPVP.ConditionCode = "ZSW4";
                                            conditionPVP.PVP = quote.TotalNetsale;
                                            conditionsPvp.Add(conditionPVP);
                                        }
                                        else
                                        {
                                            cond.PVP = cond.PVP + quote.TotalNetsale;
                                        }
                                    }
                                }
                            

                            if (financingType == "003" || financingType == "005")
                            {
                                BB_Proposal_Quote quote1 = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.CodeRef && x.Proposal_ID == proposalId).FirstOrDefault();

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
                                                    totalPvp = (quote1.UnitDiscountPrice) * (pf.Factor / 100) + totalPvp;
                                                }
                                                else
                                                {
                                                    totalPvp = (quote1.UnitDiscountPrice) * (pf.Factor) + totalPvp;
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
                                            totalPvp = (quote1.UnitDiscountPrice / contractMonths) + totalPvp;
                                        }


                                        totalPvp = Double.Parse(Math.Round(totalPvp ?? 0.0, 2).ToString("F2"));;
                                        conditionPvp.PVP = totalPvp;
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
                                                    totalPvp = (quote1.UnitDiscountPrice) * (pf.Factor / 100) + totalPvp;
                                                }
                                                else
                                                {
                                                    totalPvp = (quote1.UnitDiscountPrice) * (pf.Factor) + totalPvp;
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
                                            totalPvp = (quote1.UnitDiscountPrice / contractMonths) + +totalPvp;
                                        }

                                        condPvp.PVP = Double.Parse(Math.Round(totalPvp ?? 0.0, 2).ToString("F2"));;
                                        condPvp.ConditionCode = financingCode;
                                        conditionsPvp.Add(condPvp);
                                    }
                                }

                            }

                           

                        }
                        //conditionsPvpAux = conditionsPvp;
                        BB_Proposal_OPSManage ops = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                        if (ops != null)
                        {
                            string line1 = ops.CodeRef;
                            string line2 = "9960DRC-HTTP ";
                            //BB_OPS_Manage_Packs opsPack = db.BB_OPS_Manage_Packs.Where(x => x.CodeRef == line2).FirstOrDefault();
                            BB_OPS_Manage_Packs opsPack = db.BB_OPS_Manage_Packs.Where(x => x.CodeRef == line1).FirstOrDefault();

                            //Get ops package from configurator
                            BB_Proposal_Quote OPSPack = db.BB_Proposal_Quote.Where(x => x.CodeRef == line1 && x.Proposal_ID == proposalId).FirstOrDefault();

                            if (OPSPack == null && ops.UnitDiscountPrice != 0)
                            {
                                //double? opsPvp = (ops.PVP * ops.TotalMonths) - opsPack.PVP;
                                //double? opsPvpLine2 = opsPack.PVP;
                                double? opsPvp = (ops.PVP * ops.TotalMonths) * ops.Quantity;

                            //bool condExists = false;
                                foreach (var cond in conditionsPvp)
                                {
                                    if (cond.ConditionCode == "ZPD4")
                                    {
                                        //condExists = true;
                                        //double opsPvpRounded = Math.Round((opsPvp / contractMonths) ?? 0.0, 2);
                                        cond.PVP = cond.PVP + (opsPvp);
                                    }

                                }
                                //if (condExists == false && contractMonths > 0)
                                //{
                                //    ConditionPVP condPvp = new ConditionPVP();
                                //    double opsPvpRounded  = Math.Round((opsPvpLine2 / contractMonths) ?? 0.0, 2);
                                //    condPvp.PVP = opsPvpRounded;
                                //    condPvp.ConditionCode = "ZVBM";
                                //    conditionsPvp.Add(condPvp);
                                //}
                                //else
                                //{
                                //    ConditionPVP condPvp = new ConditionPVP();
                                //    condPvp.PVP = 0;
                                //    condPvp.ConditionCode = "ZVBM";
                                //    conditionsPvp.Add(condPvp);
                                //}
                            }
                        else if (OPSPack == null && ops.UnitDiscountPrice == 0)
                        {
                            //double? opsPvp = (ops.PVP * ops.TotalMonths) - opsPack.PVP;
                            //double? opsPvpLine2 = opsPack.PVP;
                            double? opsPvp = (ops.PVP * ops.TotalMonths) * ops.Quantity;

                            //bool condExists = false;
                            foreach (var cond in conditionsPvp)
                            {
                                if (cond.ConditionCode == "ZPD4")
                                {
                                    //condExists = true;
                                    //double opsPvpRounded = Math.Round((opsPvp / contractMonths) ?? 0.0, 2);
                                    cond.PVP = cond.PVP + (opsPvp);
                                }

                            }
                            
                        }
                        else
                            {
                                double? opsPvpLine2 = OPSPack.TotalNetsale;
                                foreach (var cond in conditionsPvp)
                                {
                                    if (cond.ConditionCode == "ZPD4")
                                    {
                                        //condExists = true;
                                        cond.PVP = cond.PVP + (opsPvpLine2);
                                    }

                                }
                            }
                        }

                    

                }

                int printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).Select(x => x.ID).FirstOrDefault();

                BB_PrintingServices bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID).FirstOrDefault();

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

            //}

            return conditionsPvp;
        }




    }
}