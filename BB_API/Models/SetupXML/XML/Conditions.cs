using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                        BB_Proposal_ItemDoBasket itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == item.MATERIAL).FirstOrDefault();
                        string financingCode = ConditionMaterial(itemDoBasket.CodeRef, contracts[0].VT_VTART);

                        ConditionPVP conditionPvp = conditionsPvp.Find(x => x.ConditionCode == financingCode);
                        if (conditionPvp != null)
                        {
                            double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                            double? totalPvp = (itemDoBasket.TotalPVP / contratoMeses) * 1.5;
                            conditionPvp.PVP = conditionPvp.PVP + totalPvp;
                        }
                        else
                        {
                            ConditionPVP condPvp = new ConditionPVP();
                            double contratoMeses = double.Parse(contracts[0].VT_VLAUFZ);
                            double? totalPvp = (itemDoBasket.TotalPVP / contratoMeses) * 1.5;

                            condPvp.PVP = totalPvp;
                            condPvp.ConditionCode = financingCode;
                            conditionsPvp.Add(condPvp);
                        }
                        //pvpItems = pvpItems + itemDoBasket.TotalPVP;
                    }

                }
                if (contracts[0].VT_VTART == "003" || contracts[0].VT_VTART == "002") //Renting
                {
                    condFlag = "A";
                }
                if (contracts[0].VT_VTART == "005") //AL
                {
                    condFlag = "A";
                }
                foreach (var condition in conditionsPvp)
                {
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
            if (financingType == "002")
            {
                financingCode = "ZVBS";
            }
            //string excelFilePath = "C:\\Users\\EXT0022\\Downloads\\INT26022024 Simply Deal 2.xlsm";
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
                    if ((financingType == "003" || financingType == "002") && !string.IsNullOrWhiteSpace(dataIntegration.COND_TYPE_RENTAL))
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
    }
}