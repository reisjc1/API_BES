using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using WebApplication1.BLL;
using WebApplication1.Controllers;
using static WebApplication1.BLL.LeaseDeskBLL;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML.XML
{
    public class Conditions
    {
        // NAO ESTA A SER UTILIZADO
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

            }
            catch (Exception e)
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
                    List<BB_PrintingService_Machines> bB_PrintingService_Machine = db.BB_PrintingService_Machines.AsNoTracking().ToList();
                    BB_Proposal_OPSManage opsM = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_Proposal_Overvaluation overvaluation = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<BB_Equipamentos> equipamentos = db.BB_Equipamentos.AsNoTracking().ToList();

                    int? numberOfMachines = db.BB_Proposal_Quote
                                            .Where(x => x.Proposal_ID == proposalId)
                                            .Join(db.BB_Equipamentos,
                                            quote => quote.CodeRef,
                                            equip => equip.CodeRef,
                                            (quote, equip) => new { quote.Qty })
                                            .Sum(x => x.Qty) ?? 0;


                    double? cancelationPerMachine = 0;
                    if (overvaluation != null)
                    {
                        cancelationPerMachine = overvaluation.Total / numberOfMachines;
                    }

                    var leaseDeskBLL = new LeaseDeskBLL();

                    List<ItemGroups> groups = leaseDeskBLL.GetGroups(proposalId);
                    int? totalQty = groups.Sum(p => p.Items.Sum(i => i.Qty));


                    foreach (var order in orders)
                    {
                        List<ConditionPVP> conditionsPvp = new List<ConditionPVP>();
                        BB_Equipamentos isMachine = null;
                        foreach (var item in order.Z1ZVOE_ORDER_ITEMS)
                        {
                            bool isUsed = order.USED_MACHINE == "1" && item.ITM_NUMBER == "10";
                            BB_Proposal_Quote quote = quote_lst.FirstOrDefault(x => x.CodeRef == item.MATERIAL && x.IsUsed == isUsed);
                            BB_Equipamentos equp = equipamentos.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                            if (equp != null)
                            {
                                isMachine = equp;
                            }

                            ConditionPVP conditionPVP = new ConditionPVP();

                            if (quote != null)
                            {
                                if (quote_lst.Any(x => x.CodeRef == quote.CodeRef))
                                {
                                    //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                    //{
                                    double pvpToAdd = Math.Round((quote.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) ?? 0.0, 2);
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");

                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = pvpToAdd;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP = Math.Round((cond.PVP + pvpToAdd) ?? 0.0, 2);
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
                            }
                            //else
                            //{
                            //    BB_Proposal_Quote_RS quoteRS = quoteRs_lst.FirstOrDefault(x => x.CodeRef == item.MATERIAL);
                            //    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");

                            //    double? unitDiscountPrice = quoteRS?.UnitDiscountPrice ?? opsM.UnitDiscountPrice;
                            //    int? totalMonths = quoteRS?.TotalMonths ?? opsM.TotalMonths;

                            //    double calculatedPVP = Math.Round((unitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * totalMonths) ?? 0.0, 2);

                            //    if (cond == null)
                            //    {
                            //        conditionPVP.ConditionCode = "ZPD4";
                            //        conditionPVP.PVP = calculatedPVP;
                            //        conditionsPvp.Add(conditionPVP);
                            //    }
                            //    else
                            //    {
                            //        cond.PVP = Math.Round((cond.PVP + calculatedPVP) ?? 0.0, 2);
                            //    }
                            //}

                            if (contracts[0].VT_VTART == "003" || contracts[0].VT_VTART == "005")
                            {
                                BB_Proposal_Quote quote1 = quote_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                                BB_Proposal_Quote_RS quoteRS1 = quoteRs_lst.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();


                                string financingCode = ConditionMaterial(item.MATERIAL, contracts[0].VT_VTART, dataIntegration_lst);

                                //Verificar se é serviço recurrente, porque se for tem que ser criada a condicao que esta gravada na
                                //base de dados (ex: ZVBI, ZVBM, etc) caso não exista nos servicos recurrentes entao tem que ser adicionado o valor
                                // ao ZVBA

                                if ((quoteRs_lst == null || quoteRs_lst.Count == 0) && contracts[0].VT_VTART == "005")
                                {
                                    financingCode = "ZVBA";
                                }
                                ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);

                                // Caso exista a referencia no configurador
                                if (quote1 != null)
                                {
                                    if (conditionPvp != null)
                                    {
                                        if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                        {
                                            double? cPVP = 0;
                                            //AssigmentLease
                                            if (contracts[0].VT_VTART == "005" && quote1.Family.Contains("HW"))
                                            {
                                                //Se for máquina vai somar o TCP ao valor da máquina
                                                cPVP = (quote1.TCP ?? 0) + (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY));
                                            }
                                            else if (contracts[0].VT_VTART == "005" && !quote1.Family.Contains("HW"))
                                            {
                                                cPVP = (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY));
                                            }
                                            //Rental Direto(003)
                                            else if (quote1.Family.Contains("HW"))
                                            {
                                                cPVP = quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY);
                                            }

                                            if (pf.Factor >= 1)
                                            {

                                                conditionPvp.PVP = Math.Round(((cPVP * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                            else if (pf.Factor > 0 && pf.Factor < 1)
                                            {
                                                conditionPvp.PVP = Math.Round(((cPVP * pf.Factor) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                        }
                                        else
                                        {
                                            conditionPvp.PVP = Math.Round(((quote1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months) + conditionPvp.PVP) ?? 0.0, 2);
                                        }
                                    }
                                    else
                                    {
                                        ConditionPVP condPvp = new ConditionPVP();
                                        double? cPVP = 0;

                                        if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                        {
                                            //AssigmentLease
                                            if (contracts[0].VT_VTART == "005" && quote1.Family.Contains("HW"))
                                            {
                                                //Se for máquina vai somar o TCP ao valor da máquina
                                                cPVP = (quote1.TCP ?? 0) + (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY));
                                            }
                                            else if (contracts[0].VT_VTART == "005" && !quote1.Family.Contains("HW"))
                                            {
                                                cPVP = (quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY));
                                            }
                                            //Rental Direto(003)
                                            // Só vai somar os items que forem do tipo HW
                                            else if (quote1.Family.Contains("HW"))
                                            {
                                                cPVP = quote1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY);
                                            }

                                            if (pf.Factor >= 1)
                                            {
                                                condPvp.PVP = Math.Round((cPVP * (pf.Factor / 100)) ?? 0.0, 2);
                                            }
                                            else if (pf.Factor > 0 && pf.Factor < 1)
                                            {
                                                condPvp.PVP = Math.Round((cPVP * pf.Factor) ?? 0.0, 2);
                                            }
                                        }
                                        else
                                        {
                                            condPvp.PVP = Math.Round((quote1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months) ?? 0.0, 2);
                                        }

                                        condPvp.ConditionCode = financingCode;
                                        if (condPvp.ConditionCode != null)
                                        {
                                            conditionsPvp.Add(condPvp);
                                        }
                                    }
                                }
                                else
                                {
                                    // Caso a referencia esteja nos Servico recurrente
                                    if (quoteRS1 != null)
                                    {
                                        if (conditionPvp != null)
                                        {
                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                if (pf.Factor >= 1)
                                                {
                                                    conditionPvp.PVP = Math.Round(((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                                }
                                                else if (pf.Factor > 0 && pf.Factor < 1)
                                                {
                                                    conditionPvp.PVP = Math.Round(((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * pf.Factor) + conditionPvp.PVP) ?? 0.0, 2);
                                                }
                                            }
                                            else
                                            {
                                                double? cPVPRS = 0;
                                                // Caso o número de meses for 1, é necessários dividir o valor do UnitDiscountPrice pela quantidade
                                                // e pelos meses de maneira a obtermos o valor mensal por unidade
                                                if (quoteRS1.TotalMonths == 1)
                                                {
                                                    cPVPRS = (quoteRS1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months);
                                                }
                                                else
                                                {
                                                    cPVPRS = quoteRS1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY);
                                                }
                                                conditionPvp.PVP = Math.Round((cPVPRS + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                        }
                                        else
                                        {
                                            ConditionPVP condPvp = new ConditionPVP();

                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                if (pf.Factor >= 1)
                                                {
                                                    condPvp.PVP = Math.Round(((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY)) * (pf.Factor / 100)) ?? 0.0, 2);
                                                }
                                                else if (pf.Factor > 0 && pf.Factor < 1)
                                                {
                                                    condPvp.PVP = Math.Round((quoteRS1.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * (pf.Factor)) ?? 0.0, 2);
                                                }
                                            }
                                            else
                                            {
                                                double? cPVPRS = 0;
                                                // Caso o número de meses for 1, é necessários dividir o valor do UnitDiscountPrice pela quantidade
                                                // e pelos meses de maneira a obtermos o valor mensal por unidade
                                                if (quoteRS1.TotalMonths == 1)
                                                {
                                                    cPVPRS = (quoteRS1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY) / pf.Months);
                                                }
                                                else
                                                {
                                                    cPVPRS = quoteRS1.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY);
                                                }
                                                condPvp.PVP = Math.Round((cPVPRS) ?? 0.0, 2);
                                            }

                                            condPvp.ConditionCode = financingCode;

                                            if (condPvp.ConditionCode != null)
                                            {
                                                conditionsPvp.Add(condPvp);
                                            }
                                        }
                                    }
                                    // Caso seja pacote ops manage
                                    else
                                    {
                                        if (conditionPvp != null)
                                        {
                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {
                                                if (pf.Factor >= 1)
                                                {
                                                    //Talvez falte multiplicar pelos meses
                                                    conditionPvp.PVP = Math.Round(((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * (pf.Factor / 100)) + conditionPvp.PVP) ?? 0.0, 2);
                                                }
                                                else if (pf.Factor > 0 && pf.Factor < 1)
                                                {
                                                    conditionPvp.PVP = Math.Round(((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * pf.Factor) + conditionPvp.PVP) ?? 0.0, 2);
                                                }
                                            }
                                            else
                                            {
                                                conditionPvp.PVP = Math.Round(((opsM.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY)) + conditionPvp.PVP) ?? 0.0, 2);
                                            }
                                        }
                                        else
                                        {
                                            ConditionPVP condPvp = new ConditionPVP();

                                            if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                            {

                                                if (pf.Factor >= 1)
                                                {
                                                    //Talvez falte multiplicar pelos meses
                                                    condPvp.PVP = Math.Round((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * (pf.Factor / 100)) ?? 0.0, 2);
                                                }
                                                else if (pf.Factor > 0 && pf.Factor < 1)
                                                {
                                                    condPvp.PVP = Math.Round((opsM.UnitDiscountPrice * Convert.ToDouble(item.REQ_QTY) * pf.Factor) ?? 0.0, 2);
                                                }

                                            }
                                            else
                                            {
                                                condPvp.PVP = Math.Round((opsM.UnitDiscountPrice / Convert.ToDouble(item.REQ_QTY)) ?? 0.0, 2);
                                            }

                                            condPvp.ConditionCode = financingCode;

                                            if (condPvp.ConditionCode != null)
                                            {
                                                conditionsPvp.Add(condPvp);
                                            }
                                        }
                                    }
                                }
                            }
                            if (overvaluation != null)
                            {
                                double? factorValue = pf.Factor >= 1 ? (pf.Factor / 100) : pf.Factor;

                                var condZVBA = conditionsPvp.Where(x => x.ConditionCode == "ZVBA").FirstOrDefault();
                                double? overvaluationValue = (overvaluation.Total / totalQty) * factorValue;
                                condZVBA.PVP = condZVBA.PVP + overvaluationValue;
                            }
                        }

                        //Criação da condição ZVBS quando negocio tem VVA como tipo de servico de printing
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
                            BB_VVA bB_VVA = bB_VVA_lst.FirstOrDefault(x => x.PrintingServiceID == bB_PrintingServices.ID);
                            List<BB_PrintingService_Machines> machines = bB_PrintingService_Machine.Where(x => x.PrintingServiceID == bB_PrintingServices.ID).ToList();

                            if (bB_VVA != null)
                            {
                                BB_PrintingService_Machines machineItem = machines.FirstOrDefault(x => x.CodeRef == order.MACHINE);

                                // Se não encontrar e for máquina usada, tenta obter pela tabela de basket
                                if (machineItem == null)
                                {
                                    //Quando maquina usada, temos que ir buscar o codigo de referencia atraves da BB_Proposal_ItemDoBasket 
                                    BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.FirstOrDefault(x => x.SerialNumber == order.ORDER_INFO);
                                    machineItem = machines.FirstOrDefault(x => x.CodeRef == itemDoBasket?.CodeRef);
                                }

                                if (machineItem != null)
                                {
                                    //Cálculo do valor do PVP do ZVBS é sempre o volume BW * click preto aprovador + volume C * click cor aprovado
                                    double? pvpZVBS = (machineItem.BWVolume * machineItem.ApprovedBW) + (machineItem.CVolume * machineItem.ApprovedC);

                                    collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                                    {
                                        DOC = order.SD_DOC,
                                        COND_FLAG = "A",
                                        KSCHL = "ZVBS",
                                        KBETR = Math.Round(pvpZVBS ?? 0.0, 2).ToString("F2").Replace(",", ".")
                                    });
                                }
                            }
                        }


                        //Quando negocio tem sobrevalorizacao, adiciona a condicao ZEBB 
                        if (overvaluation != null && isMachine != null)
                        {
                            collectionConditions.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS
                            {
                                DOC = order.SD_DOC,
                                COND_FLAG = "O",
                                KSCHL = "ZEBB",
                                KBETR = cancelationPerMachine.ToString().Replace(",", ".")
                            });
                        }

                        //Percorre a lista de condiçoes criadas e define o condFlag
                        foreach (var condition in conditionsPvp)
                        {
                            // Condição do tipo ZPD4 ou ZSW4 deve ter flag 'O', caso contrário 'A'
                            string condFlag = (condition.ConditionCode == "ZPD4" || condition.ConditionCode == "ZSW4") ? "O" : "A";

                            Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS cond = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONDITIONS()
                            {
                                DOC = order.SD_DOC,
                                COND_FLAG = condFlag,
                                KSCHL = condition.ConditionCode,
                                KBETR = Math.Round(condition.PVP ?? 0.0, 2).ToString("F2").Replace(",", ".")
                            };
                            collectionConditions.Add(cond);
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

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<BB_Data_Integration> dataIntegration_lst = db.BB_Data_Integration.AsNoTracking().ToList();
                    BB_Proposal_Overvaluation overvaluation = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    List<BB_Proposal_Quote> oneShot = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                    List<BB_Proposal_Quote_RS> rsShot = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).ToList();

                    foreach (var order in orders)
                    {
                        foreach (var item in order.Items)
                        {

                            if (oneShot.Any(x => x.CodeRef == item.CodeRef))
                            {
                                ConditionPVP conditionPVP = new ConditionPVP();
                                // Se for um codigo de referencia do oneshot o total de meses esta definido como 0
                                if (item.TotalMonths == 0)
                                {
                                    //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                    //{
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                    double? value = item.UnitDiscountPrice * item.Qty;

                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = value;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP += value;
                                    }
                                }
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
                                //}
                                //Entra aqui caso seja servico recurrente ou pacote ops manage
                                else
                                {
                                    ConditionPVP cond = conditionsPvp.Find(x => x.ConditionCode == "ZPD4");
                                    double? value = (item.UnitDiscountPrice * item.Qty) * item.TotalMonths;

                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = value;
                                        conditionsPvp.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP += value;
                                    }
                                }
                            }

                            //003 - Rental Direto
                            //005 - AssigmentLease
                            if (financingType == "003" || financingType == "005")
                            {
                                string financingCode = ConditionMaterial(item.CodeRef, financingType, dataIntegration_lst);

                                //Verificar se é serviço recurrente, porque se for tem que ser criada a condicao que esta gravada na
                                //base de dados (ex: ZVBI, ZVBM, etc) caso não exista nos servicos recurrentes entao tem que ser adicionado o valor
                                // ao ZVBA


                                if((rsShot == null || rsShot.Count == 0) && financingType == "005")
                                {
                                    financingCode = "ZVBA";
                                }

                                ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);

                                double? baseValue = item.UnitDiscountPrice * item.Qty;
                                double? factorValue = pf.Factor >= 1 ? (pf.Factor / 100) : pf.Factor;
                                bool isZVBR_or_ZVBA = financingCode == "ZVBR" || financingCode == "ZVBA";
                                bool isHW = item.Family.Contains("HW");

                                if (conditionPvp != null)
                                {
                                    if (isZVBR_or_ZVBA)
                                    {
                                        if (pf.Factor > 0)
                                        {
                                            // Quando é AssigmentLease temos que adicionar a LPI
                                            if (financingType == "005" && item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += ((item.LPI + baseValue) * factorValue);
                                            }
                                            else if (financingType == "005" && !item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += (baseValue * factorValue);
                                            }
                                            // caso seja Rental Direto
                                            else if (item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += (baseValue * factorValue);
                                            }
                                        }
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
                                }
                                else
                                {
                                    ConditionPVP condPvp = new ConditionPVP();

                                    if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                    {
                                        bool hasPvp = condPvp.PVP != null;

                                        if (pf.Factor > 0)
                                        {
                                            // Condicao ZVBA corresponde ao 005
                                            if (financingCode == "ZVBA" && item.Family.Contains("HW"))
                                            {
                                                double? value = ((item.LPI + baseValue) * factorValue);
                                                condPvp.PVP = hasPvp ? condPvp.PVP + value : value;
                                            }
                                            else if (financingCode == "ZVBA" && !item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += (baseValue * factorValue);
                                            }
                                            // Entra aqui quando na condicao ZVBR que corresponde ao 003
                                            else if (item.Family.Contains("HW"))
                                            {
                                                double? value = (baseValue * factorValue);
                                                condPvp.PVP = hasPvp ? condPvp.PVP + value : value;
                                            }
                                        }
                                    }

                                    //Caso a condicao seja diferente de ZVBR e ZVBA (ex: ZVBI, ZVBM)
                                    else
                                    {
                                        if (condPvp.PVP != null)
                                        {
                                            if (item.TotalMonths == 0)
                                            {
                                                condPvp.PVP += (item.UnitDiscountPrice / item.Qty) / contractMonths;
                                            }
                                            else
                                            {
                                                condPvp.PVP += (item.UnitDiscountPrice / item.Qty);
                                            }
                                        }
                                        else
                                        {
                                            if (item.TotalMonths == 0)
                                            {
                                                condPvp.PVP = ((item.UnitDiscountPrice / item.Qty) / contractMonths);
                                            }
                                            else
                                            {
                                                condPvp.PVP = item.UnitDiscountPrice / item.Qty;
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

                    if (overvaluation != null)
                    {
                        ConditionPVP condPvp = new ConditionPVP();
                        condPvp.PVP = overvaluation.Total;
                        condPvp.ConditionCode = "ZEBB";
                        conditionsPvp.Add(condPvp);
                    }

                    if (overvaluation != null)
                    {
                        double? factorValue = pf.Factor >= 1 ? (pf.Factor / 100) : pf.Factor;

                        var condZVBA = conditionsPvp.Where(x => x.ConditionCode == "ZVBA").FirstOrDefault();
                        condZVBA.PVP += (overvaluation.Total * factorValue);
                    }
                }

                return conditionsPvp;
            }
            catch (Exception e)
            {
                string err = e.Message;
                return null;
            }

        }

        public List<ConditionPVPPerMachine> ConditionsVariablesPerMachine(List<ItemGroups> orders, string financingType, int? contractMonths, int? proposalId, int? ftCode)
        {
            try
            {
                List<ConditionPVPPerMachine> conditionsPvp = new List<ConditionPVPPerMachine>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<BB_Data_Integration> dataIntegration_lst = db.BB_Data_Integration.AsNoTracking().ToList();
                    BB_Proposal_Overvaluation overvaluation = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<BB_Equipamentos> equipamentos = db.BB_Equipamentos.AsNoTracking().ToList();
                    List<BB_Proposal_Quote> oneShot = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                    List<BB_Proposal_Quote_RS> rsShot = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).ToList();

                    int? numberOfMachines = db.BB_Proposal_Quote
                                            .Where(x => x.Proposal_ID == proposalId)
                                            .Join(db.BB_Equipamentos,
                                            quote => quote.CodeRef,
                                            equip => equip.CodeRef,
                                            (quote, equip) => new { quote.Qty })
                                            .Sum(x => (int?)x.Qty) ?? 0;

                    double? cancelationPerMachine = 0;
                    if (overvaluation != null)
                    {
                        cancelationPerMachine = overvaluation.Total / numberOfMachines;
                    }

                    int? totalQty = orders.Sum(p => p.Items.Sum(i => i.Qty));

                    foreach (var order in orders)
                    {
                        ConditionPVPPerMachine conditionPVPPerMachine = new ConditionPVPPerMachine();

                        conditionPVPPerMachine.Group = order.Group;
                        conditionPVPPerMachine.Conditions = new List<ConditionPVP>();
                        string machineName = null;
                        foreach (var item in order.Items)
                        {
                            ConditionPVP conditionPVP = new ConditionPVP();
                            if (item.BundleRef)
                            {
                                machineName = equipamentos.Where(x => x.CodeRef == item.CodeRef).Select(x => x.Name).FirstOrDefault();
                                if (machineName != null)
                                {
                                    conditionPVPPerMachine.MachineModel = machineName;
                                }
                                else
                                {
                                    conditionPVPPerMachine.MachineModel = item.Name;
                                }
                            }

                            if (oneShot.Any(x => x.CodeRef == item.CodeRef))
                            {
                                // Se for um codigo de referencia do oneshot o total de meses esta definido como 0
                                if (item.TotalMonths == 0)
                                {
                                    //if (quote.Family.Contains("HW") || quote.Family.Contains("CS"))
                                    //{
                                    ConditionPVP cond = conditionPVPPerMachine.Conditions.Find(x => x.ConditionCode == "ZPD4");
                                    double? value = item.UnitDiscountPrice * item.Qty;

                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = value;
                                        conditionPVPPerMachine.Conditions.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP += value;
                                    }
                                }
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
                                //}
                                //Entra aqui caso seja servico recurrente ou pacote ops manage
                                else
                                {
                                    ConditionPVP cond = conditionPVPPerMachine.Conditions.Find(x => x.ConditionCode == "ZPD4");
                                    double? value = (item.UnitDiscountPrice * item.Qty) * item.TotalMonths;

                                    if (cond == null)
                                    {
                                        conditionPVP.ConditionCode = "ZPD4";
                                        conditionPVP.PVP = value;
                                        conditionPVPPerMachine.Conditions.Add(conditionPVP);
                                    }
                                    else
                                    {
                                        cond.PVP += value;
                                    }
                                }
                            }

                            //003 - Rental Direto
                            //005 - AssigmentLease
                            if (financingType == "003" || financingType == "005")
                            {
                                string financingCode = ConditionMaterial(item.CodeRef, financingType, dataIntegration_lst);

                                //Verificar se é serviço recurrente, porque se for tem que ser criada a condicao que esta gravada na
                                //base de dados (ex: ZVBI, ZVBM, etc) caso não exista nos servicos recurrentes entao tem que ser adicionado o valor
                                // ao ZVBA

                                if ((rsShot == null && rsShot.Count == 0) && financingType == "005")
                                {
                                    financingCode = "ZVBA";
                                }

                                ConditionPVP conditionPvp = conditionPVPPerMachine.Conditions.Find(x => x.ConditionCode == financingCode);

                                double? baseValue = item.UnitDiscountPrice * item.Qty;
                                double? factorValue = pf.Factor >= 1 ? (pf.Factor / 100) : pf.Factor;
                                bool isZVBR_or_ZVBA = financingCode == "ZVBR" || financingCode == "ZVBA";
                                bool isHW = item.Family.Contains("HW");

                                if (conditionPvp != null)
                                {
                                    if (isZVBR_or_ZVBA)
                                    {
                                        if (pf.Factor > 0)
                                        {
                                            // Quando é AssigmentLease temos que adicionar a LPI
                                            if (financingType == "005" && item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += (item.LPI + baseValue) * factorValue;
                                            }
                                            else if (financingType == "005" && !item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += (baseValue * factorValue);
                                            }
                                            // caso seja Rental Direto
                                            else if (item.Family.Contains("HW"))
                                            {
                                                conditionPvp.PVP += baseValue * factorValue;
                                            }
                                        }
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
                                }
                                else
                                {
                                    ConditionPVP condPvp = new ConditionPVP();

                                    if (financingCode == "ZVBR" || financingCode == "ZVBA")
                                    {
                                        bool hasPvp = condPvp.PVP != null;

                                        if (pf.Factor > 0)
                                        {
                                            // Condicao ZVBA corresponde ao 005
                                            if (financingCode == "ZVBA" && item.Family.Contains("HW"))
                                            {
                                                double? value = (item.LPI + baseValue) * factorValue;
                                                condPvp.PVP = hasPvp ? condPvp.PVP + value : value;
                                            }
                                            else if (financingCode == "ZVBA" && !item.Family.Contains("HW"))
                                            {

                                                double? value = baseValue * factorValue;
                                                condPvp.PVP = hasPvp ? condPvp.PVP + value : value;
                                            }
                                            // Entra aqui quando na condicao ZVBR que corresponde ao 003
                                            else if (item.Family.Contains("HW"))
                                            {
                                                double? value = baseValue * factorValue;
                                                condPvp.PVP = hasPvp ? condPvp.PVP + value : value;
                                            }
                                        }
                                    }

                                    //Caso a condicao seja diferente de ZVBR e ZVBA (ex: ZVBI, ZVBM)
                                    else
                                    {
                                        if (condPvp.PVP != null)
                                        {
                                            if (item.TotalMonths == 0)
                                            {
                                                condPvp.PVP += (item.UnitDiscountPrice / item.Qty) / contractMonths;
                                            }
                                            else
                                            {
                                                condPvp.PVP += (item.UnitDiscountPrice / item.Qty);
                                            }
                                        }
                                        else
                                        {
                                            if (item.TotalMonths == 0)
                                            {
                                                condPvp.PVP = ((item.UnitDiscountPrice / item.Qty) / contractMonths);
                                            }
                                            else
                                            {
                                                condPvp.PVP = item.UnitDiscountPrice / item.Qty;
                                            }
                                        }
                                    }

                                    condPvp.PVP = Convert.ToDouble(condPvp.PVP);
                                    condPvp.ConditionCode = financingCode;
                                    conditionPVPPerMachine.Conditions.Add(condPvp);
                                }
                            }

                            if (overvaluation != null)
                            {
                                double? factorValue = pf.Factor >= 1 ? (pf.Factor / 100) : pf.Factor;

                                var condZVBA = conditionPVPPerMachine.Conditions.Where(x => x.ConditionCode == "ZVBA").FirstOrDefault();
                                double? overvaluationValue = (overvaluation.Total / totalQty) * factorValue;
                                condZVBA.PVP = condZVBA.PVP + overvaluationValue;
                            }

                            if (machineName != null)
                            {
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
                                    BB_PrintingService_Machines machine = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == bB_PrintingServices.ID && x.CodeRef == item.CodeRef).FirstOrDefault();
                                    if (machineName != null && machine != null)
                                    {
                                        if (bB_VVA != null)
                                        {
                                            double valueZVBS = 0;

                                            ConditionPVP condPvp = new ConditionPVP();
                                            condPvp.PVP = Convert.ToDouble(Math.Round((machine.BWVolume * machine.ApprovedBW) + (machine.ApprovedC * machine.CVolume) ?? 0.0, 2).ToString("F2"));
                                            condPvp.ConditionCode = "ZVBS";
                                            conditionPVPPerMachine.Conditions.Add(condPvp);
                                        }
                                    }
                                }
                            }


                        }
                        if (overvaluation != null && machineName != null)
                        {
                            ConditionPVP condPvp = new ConditionPVP();
                            condPvp.PVP = cancelationPerMachine;
                            condPvp.ConditionCode = "ZEBB";
                            conditionPVPPerMachine.Conditions.Add(condPvp);
                        }

                        conditionsPvp.Add(conditionPVPPerMachine);
                    }

                }

                return conditionsPvp;
            }
            catch (Exception e)
            {
                string err = e.Message;
                return null;
            }

        }

    }
}