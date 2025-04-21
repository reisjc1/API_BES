using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.SetupXML.XML;
using WebApplication1.Models.SetupXML;
using WebApplication1.Models;

namespace WebApplication1.BLL
{
    public class LeaseDeskBLL
    {
        public List<ConditionPVP> FinancingDetails(int? proposalId)
        {
            string financingType = "";
            string contractType = "";
            Conditions cond = new Conditions();
            using (var db = new BB_DB_DEVEntities2())
            {

                BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == proposalId).FirstOrDefault();
                BB_Proposal_Financing financing = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                BB_FinancingType ft = db.BB_FinancingType.Where(x => x.Code == financing.FinancingTypeCode).FirstOrDefault();


                switch (ft.Code)
                {
                    case 0:
                    case 1:
                        financingType = "SA";
                        contractType = "002";
                        break;

                    case 2:
                        financingType = "DL";
                        //contractType = "008";
                        contractType = "002";//Renting por enquanto enviar 002 e o ideal é enviar 008
                        break;

                    case 3:
                        financingType = "AL";
                        contractType = "005";
                        break;

                    case 5:
                        financingType = "AS";
                        contractType = "003";
                        break;

                    default:
                        break;
                }

                List<ItemGroups> groups = GetGroups(proposalId);


                List<ConditionPVP> condPvp = cond.ConditionsVariables(groups, contractType, financing.Months, proposalId, ft.Code);


                return condPvp;
            }
        }
        public List<ItemGroups> GetGroups(int? proposalId)
        {
            try
            {
                List<ItemGroups> listGroups = new List<ItemGroups>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<string> quoteLst = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).Select(x => x.CodeRef).ToList();
                    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).ToList();
                    BB_Proposal_OPSManage opsM = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    List<string> quoteRSLst = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).Select(x => x.CodeRef).ToList();
                    List<BB_Proposal_Quote_RS> quoteRSList = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).ToList();
                    foreach (var deliveryLocation in dl)
                    {
                        int? groupNumber = null;
                        List<BB_Proposal_ItemDoBasket> groups = new List<BB_Proposal_ItemDoBasket>();
                        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX).OrderBy(x => x.Group).ToList();
                        foreach (var itemdoBasket in itemsDoBasket)
                        {
                            if (itemdoBasket.Group != groupNumber)
                            {
                                groups.Add(itemdoBasket);
                                groupNumber = itemdoBasket.Group;
                            }
                        }
                        foreach (var group in groups)
                        {
                            bool firstItemGroup = true;
                            ItemGroups Bundles = new ItemGroups();
                            Bundles.Items = new List<ItemGroup>();
                            List<BB_Proposal_ItemDoBasket> items = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX && x.Group == group.Group).ToList();

                            foreach (var item in items)
                            {
                                BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                if (bB_Equipamentos != null)
                                {
                                    ItemGroup group1 = new ItemGroup();
                                    group1.CodeRef = item.CodeRef;
                                    group1.BundleRef = true;
                                    group1.Qty = item.Qty;
                                    group1.UnitDiscountPrice = (double)item.UnitDiscountPrice;
                                    group1.TotalMonths = 0;
                                    group1.LPI = (double)item.TCP;
                                    Bundles.Items.Add(group1);

                                    firstItemGroup = false;

                                }
                                else
                                {
                                    ItemGroup group2 = new ItemGroup();
                                    if (quoteLst.Contains(item.CodeRef))
                                    {
                                        group2.CodeRef = item.CodeRef;
                                        group2.BundleRef = false;
                                        group2.Qty = item.Qty;
                                        group2.UnitDiscountPrice = (double)item.UnitDiscountPrice;
                                        group2.TotalMonths = 0;
                                        group2.LPI = 0;
                                        Bundles.Items.Add(group2);
                                    }
                                    else if (quoteRSLst.Contains(item.CodeRef))
                                    {
                                        BB_Proposal_Quote_RS quoteRS = quoteRSList.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                        group2.CodeRef = item.CodeRef;
                                        group2.BundleRef = false;
                                        group2.Qty = item.Qty;
                                        group2.UnitDiscountPrice = (double)item.UnitDiscountPrice;
                                        group2.TotalMonths = (int)quoteRS.TotalMonths;
                                        Bundles.Items.Add(group2);
                                    }
                                    else
                                    {
                                        if(opsM.CodeRef == item.CodeRef)
                                        {
                                            group2.CodeRef = item.CodeRef;
                                            group2.BundleRef = false;
                                            group2.Qty = item.Qty;
                                            group2.UnitDiscountPrice = (double)item.UnitDiscountPrice;
                                            group2.TotalMonths = (int)opsM.TotalMonths;
                                            group2.LPI = 0;
                                            Bundles.Items.Add(group2);
                                        }
                                    }
                                }

                            }
                            listGroups.Add(Bundles);
                        }

                    }
                }
                return listGroups;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public class ItemGroups
        {
            public List<ItemGroup> Items { get; set; }
        }
        public class ItemGroup
        {
            public string CodeRef { get; set; }
            public bool BundleRef { get; set; }
            public double UnitDiscountPrice { get; set; }
            public int? Qty { get; set; }
            public int TotalMonths { get; set; }
            public double LPI { get; set; }
        }
        
    }
}