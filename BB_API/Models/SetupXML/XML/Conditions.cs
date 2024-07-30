using DocumentFormat.OpenXml.Spreadsheet;
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
        System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> contracts)
        {
            var collectionConditions = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS>();
            foreach (var order in orders)
            {
                string condFlag = null;
                List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
                double? pvpItems = 0;
                double? KBETR = 0;

                using (var db = new BB_DB_DEVEntities2())
                {
                    foreach (var item in order.Z1ZVOE_ORDER_ITEMS)
                    {
                        if (contracts[0].VT_VTART == "002" || contracts[0].VT_VTART == "005" || contracts[0].VT_VTART == "008")
                        {
                            BB_Proposal_Quote quote = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                            BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.MATERIAL).FirstOrDefault();
                            ConditionPVP conditionPVP = new ConditionPVP();


                            if (quote != null)
                            {
                                if (quote.Family.Contains("HW"))
                                {
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = quote.TotalPVP;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = cond.PVP + quote.TotalPVP;
                                    }

                                }
                                else
                                {
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZSW4";
                                        conditionPVP.PVP = quote.TotalPVP;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = cond.PVP + quote.TotalPVP;
                                    }
                                }
                            }
                            if (ops != null)
                            {
                                ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                if (cond == null)
                                {
                                    conditionPVP.ConditionCode = "ZSW4";
                                    conditionPVP.PVP = ops.PVP * ops.Quantity;
                                    conditionsPvp.Add(conditionPVP);
                                }
                                else
                                {
                                    cond.PVP = cond.PVP + (ops.PVP * ops.Quantity);
                                }
                            }
                        }
                        if (contracts[0].VT_VTART != "002")
                        {
                            //BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                            BB_Proposal_Quote quote = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                            BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.MATERIAL).FirstOrDefault();
                            string financingCode = ConditionMaterial(item.MATERIAL, contracts[0].VT_VTART);

                            ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);
                            if (quote != null)
                            {
                                if (conditionPvp != null)
                                {
                                    double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                                    double? totalPvp = (quote.UnitDiscountPrice / contratoMeses) * 1.5;
                                    conditionPvp.PVP = conditionPvp.PVP + totalPvp;
                                }
                                else
                                {
                                    ConditionPVP condPvp = new ConditionPVP();
                                    double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                                    double? totalPvp = (quote.UnitDiscountPrice / contratoMeses) * 1.5;

                                    condPvp.PVP = totalPvp;
                                    condPvp.ConditionCode = financingCode;
                                    conditionsPvp.Add(condPvp);
                                }
                            }
                            if (ops != null)
                            {
                                if (conditionPvp != null)
                                {
                                    double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                                    double? totalPvp = (ops.UnitDiscountPrice / contratoMeses) * 1.5;
                                    conditionPvp.PVP = conditionPvp.PVP + totalPvp;
                                }
                                else
                                {
                                    ConditionPVP condPvp = new ConditionPVP();
                                    double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                                    double? totalPvp = (ops.UnitDiscountPrice / contratoMeses) * 1.5;

                                    condPvp.PVP = totalPvp;
                                    condPvp.ConditionCode = financingCode;
                                    conditionsPvp.Add(condPvp);
                                }
                            }

                        }
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
                        KBETR = condition.PVP.ToString()
                    });
                }


            }
            return collectionConditions;

        }
        public string ConditionMaterial(string codeRef, string financingType)
        //public void ConditionMaterial(string codeRef, string financingType)
        {


            string financingCode = null;

            if (financingType == "002" || financingType == "008")
            {
                financingCode = "ZVBS";
            }

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
        public List<ConditionPVP> ConditionsVariables(List<ItemGroups> orders, string financingType, int? contractMonths)
        {
            List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
            double? pvpItems = 0;
            foreach (var order in orders)
            {



                using (var db = new BB_DB_DEVEntities2())
                {
                    foreach (var items in orders)
                    {
                        foreach (var item in items.Items)
                        {
                            if (financingType == "002")
                            {
                                BB_Proposal_Quote quote = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.CodeRef).FirstOrDefault();
                                ConditionPVP conditionPVP = new ConditionPVP();


                                if (quote != null)
                                {
                                    if (quote.Family.Contains("HW"))
                                    {
                                        ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                        if (cond == null)
                                        {
                                            conditionPVP.ConditionCode = "ZPD4";
                                            conditionPVP.PVP = quote.TotalPVP;
                                            conditionsPvp.Add(conditionPVP);
                                        }
                                        else
                                        {
                                            cond.PVP = cond.PVP + quote.TotalPVP;
                                        }

                                    }
                                    else
                                    {
                                        ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                        if (cond == null)
                                        {
                                            conditionPVP.ConditionCode = "ZSW4";
                                            conditionPVP.PVP = quote.TotalPVP;
                                            conditionsPvp.Add(conditionPVP);
                                        }
                                        else
                                        {
                                            cond.PVP = cond.PVP + quote.TotalPVP;
                                        }
                                    }
                                }
                                if (ops != null)
                                {
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZSW4");
                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZSW4";
                                        conditionPVP.PVP = ops.PVP * ops.Quantity;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = cond.PVP + (ops.PVP * ops.Quantity);
                                    }
                                }

                            }
                            else
                            {
                                //BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                                BB_Proposal_Quote quote = db.BB_Proposal_Quote.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                BB_Proposal_OPSImplement ops = db.BB_Proposal_OPSImplement.Where(X => X.CodeRef == item.CodeRef).FirstOrDefault();
                                string financingCode = ConditionMaterial(item.CodeRef, financingType);

                                ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);
                                if (quote != null)
                                {
                                    if (conditionPvp != null)
                                    {
                                        double contratoMeses = double.Parse(contractMonths.ToString());
                                        double? totalPvp = (quote.UnitDiscountPrice / contratoMeses) * 1.5;
                                        conditionPvp.PVP = conditionPvp.PVP + totalPvp;
                                    }
                                    else
                                    {
                                        ConditionPVP condPvp = new ConditionPVP();
                                        double contratoMeses = double.Parse(contractMonths.ToString());
                                        double? totalPvp = (quote.UnitDiscountPrice / contratoMeses) * 1.5;

                                        condPvp.PVP = totalPvp;
                                        condPvp.ConditionCode = financingCode;
                                        conditionsPvp.Add(condPvp);
                                    }
                                }
                                if (ops != null)
                                {
                                    if (conditionPvp != null)
                                    {
                                        double contratoMeses = double.Parse(contractMonths.ToString());
                                        double? totalPvp = (ops.UnitDiscountPrice / contratoMeses) * 1.5;
                                        conditionPvp.PVP = conditionPvp.PVP + totalPvp;
                                    }
                                    else
                                    {
                                        ConditionPVP condPvp = new ConditionPVP();
                                        double contratoMeses = double.Parse(contractMonths.ToString());
                                        double? totalPvp = (ops.UnitDiscountPrice / contratoMeses) * 1.5;

                                        condPvp.PVP = totalPvp;
                                        condPvp.ConditionCode = financingCode;
                                        conditionsPvp.Add(condPvp);
                                    }
                                }

                            }

                        }

                        //pvpItems = pvpItems + itemDoBasket.TotalPVP;
                    }


                }

            }
            return conditionsPvp;
        }




    }
}