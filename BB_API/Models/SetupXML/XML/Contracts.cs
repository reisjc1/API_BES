﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML.XML
{
    public class Contracts
    {
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> ConfigContracts(int proposalId, string randomLetterNunber)
        {
            string contractType = "";
            string formattedDtCont = "";
            string vtLaufk = "";

            //List<BB_Equipamentos> maquinas = new List<BB_Equipamentos>();
            //List<BB_Data_Integration> maquinas = new List<BB_Data_Integration>();
            using (var db = new BB_DB_DEVEntities2())
            {
                List<LD_Contrato> contracts = db.LD_Contrato.Where(x => x.ProposalID == proposalId).ToList();
                BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID != proposalId).FirstOrDefault();
                BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                BB_FinancingContractType ct = db.BB_FinancingContractType.Where(x => x.ID == pf.ContractTypeId).FirstOrDefault();
                List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();



                BB_FinancingType ft = db.BB_FinancingType.Where(x => x.Code == pf.FinancingTypeCode).FirstOrDefault();
                if(proposalId == 9536)
                {
                    ft.Code = 5;
                }

                if (ft.Code == 0 || ft.Code == 1)
                {
                    contractType = "002"; //Manutenção
                }
                if (ft.Code == 2) //|| ft.Code == 4
                {
                    contractType = "002"; //Renting por enquanto enviar 002 e o ideal é enviar 008
                }
                if (ft.Code == 3)
                {
                    contractType = "005"; //AL
                }
                if (ft.Code == 5)
                {
                    contractType = "003"; //Renting 
                }




                DateTime FirstDayofThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime FirstDayofTheNextMonth = FirstDayofThisMonth.AddMonths(1);
                string FirstDayNextMonthString = FirstDayofTheNextMonth.ToString("yyyyMMdd");

                if (pd.PrazoDiferenciado == 12)
                {
                    vtLaufk = "Z1";
                }
                if (pd.PrazoDiferenciado == 24)
                {
                    vtLaufk = "Z2";
                }
                if (pd.PrazoDiferenciado == 36)
                {
                    vtLaufk = "Z3";
                }
                if (pd.PrazoDiferenciado == 48)
                {
                    vtLaufk = "Z4";
                }
                if (pd.PrazoDiferenciado == 60)
                {
                    vtLaufk = "Z5";
                }
                if (pd.PrazoDiferenciado == 72)
                {
                    vtLaufk = "Z6";
                }
                if (pd.PrazoDiferenciado == 84)
                {
                    vtLaufk = "Z7";
                }
                if (pd.PrazoDiferenciado == 96)
                {
                    vtLaufk = "Z8";
                }

                var collectionContracts = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS>();
                foreach (var contract in contracts)
                {
                    List<BB_Proposal_ItemDoBasket> bb_itemsDoBasket = new List<BB_Proposal_ItemDoBasket>();
                    int? firstItemGroup = 0;
                    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).ToList();
                    foreach (var dLocations in dl)
                    {
                        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == dLocations.IDX).OrderBy(x => x.Group).ToList();
                        foreach (var items in itemsDoBasket)
                        {
                            BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == items.CodeRef).FirstOrDefault();
                            if (bB_Equipamentos != null)
                            {
                                if (items.Group != firstItemGroup)
                                {
                                    bb_itemsDoBasket.Add(items);
                                }
                            }

                        }

                    }


                    int noOrders = bb_itemsDoBasket.Count();
                    DateTime? createdTimeContractN = contract.CreatedTime;
                    if (createdTimeContractN.HasValue)
                    {
                        DateTime createdTime = createdTimeContractN.Value;
                        formattedDtCont = createdTime.ToString("yyyyMMdd");
                    }

                    bool? isMultipleContract = db.BB_Proposal.Where(x => x.ID == proposalId).Select(x => x.IsMultipleContract).FirstOrDefault();

                    int index = contracts.IndexOf(contract) + 1;
                    string contractIndexString = index.ToString();
                    collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                    {


                        //CONTR_DOC = $"C_D{contract.ID}_{contractIndexString}_{randomLetterNunber}",
                        CONTR_DOC = $"C_D3924_1_{randomLetterNunber}",
                        VT_AUART = "ZWV1",
                        VT_BEGDA = formattedDtCont,
                        VT_ABNDA = "",
                        VT_VTART = contractType,
                        VT_SERWI = "BES",             //"BES",
                        VT_ESCAL = "08",
                        VT_AUGRU = isMultipleContract == false ? "ZCS" : "ZCC",             //input de um campo -> ZCC = só 1 contrato   // ZCS
                        VT_LAUFK = vtLaufk,
                        VT_VLAUFZ = pd.PrazoDiferenciado.ToString(),
                        VT_VLAUFE = "3",
                        VT_ANZPOS = noOrders.ToString(),
                        VT_VUNDAT = FirstDayNextMonthString,
                        VT_ZTERM = "453E",
                        VT_FAKSK = "ZN"
                    });

                }
                if (contracts.Count() <= 0)
                {
                    List<BB_Proposal_ItemDoBasket> bb_itemsDoBasket = new List<BB_Proposal_ItemDoBasket>();
                    int? firstItemGroup = 0;
                    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).ToList();
                    foreach (var dLocations in dl)
                    {
                        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == dLocations.IDX).OrderBy(x => x.Group).ToList();
                        foreach (var items in itemsDoBasket)
                        {
                            BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == items.CodeRef).FirstOrDefault();
                            if (bB_Equipamentos != null)
                            {
                                if (items.Group != firstItemGroup)
                                {
                                    firstItemGroup = items.Group;

                                    bb_itemsDoBasket.Add(items);
                                }
                            }
                        }

                    }
                    DateTime currentDate = DateTime.Now;
                    string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                    int noOrders = bb_itemsDoBasket.Count();
                    bool? isMultipleContract = db.BB_Proposal.Where(x => x.ID == proposalId).Select(x => x.IsMultipleContract).FirstOrDefault();

                    collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                    {


                        CONTR_DOC = $"C_D3924_1_{randomLetterNunber}",
                        VT_AUART = "ZWV1",
                        VT_BEGDA = formattedCurrentDate,
                        VT_ABNDA = "",
                        VT_VTART = contractType,
                        VT_SERWI = "BES",       //"BES",
                        VT_ESCAL = "08",        //TODO: Para depois do GO LIVE -- ver com a adm os possiveis valores. 
                        VT_AUGRU = isMultipleContract == false ? "ZCS" : "ZCC",       //input de um campo -> ZCC = só 1 contrato   // ZCS           //TODO: Falar com a Mylene -- BB_Proposal
                        VT_LAUFK = vtLaufk,
                        VT_VLAUFZ = pd.PrazoDiferenciado.ToString(),
                        VT_VLAUFE = "3",        //TODO: Para depois do GO LIVE                                                                                                                                                               
                        VT_ANZPOS = "1",//noOrders.ToString(),
                        VT_VUNDAT = FirstDayNextMonthString,
                        VT_ZTERM = "453E",       //TODO: FinancingPaymentMethods.
                        VT_FAKSK = "X"
                    });
                }

                ////Apagar futuramente
                //collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                //{


                //    CONTR_DOC = $"C_D{contracts[0].ID}_1_{randomLetterNunber}",
                //    VT_AUART = "ZWV1",
                //    VT_BEGDA = formattedDtCont,
                //    VT_ABNDA = "",
                //    VT_VTART = contractType,
                //    VT_SERWI = "EBB",             //"BES",
                //    VT_ESCAL = "08",
                //    VT_AUGRU = "ZCC",
                //    VT_LAUFK = vtLaufk,
                //    VT_VLAUFZ = pd.PrazoDiferenciado.ToString(),
                //    VT_VLAUFE = "3",
                //    VT_ANZPOS = noMaquinas.ToString(),
                //    VT_VUNDAT = FirstDayNextMonthString,
                //    VT_ZTERM = "453E"
                //}); ;

                return collectionContracts;
            }

        }
    }
}