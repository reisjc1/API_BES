using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML.XML
{
    public class Contracts
    {
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS> ConfigContracts(int proposalId, string randomLetterNunber, LD_Contrato c, BB_Proposal_Financing pf,  BB_FinancingType ft)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();


                string contractType = "";
                string formattedDtCont = "";
                string vtLaufk = "";

                //List<BB_Equipamentos> maquinas = new List<BB_Equipamentos>();
                //List<BB_Data_Integration> maquinas = new List<BB_Data_Integration>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_PrintingServices2 printingServices2 = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_PrintingServices bB_Printing = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingServices2.ID).FirstOrDefault();

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
                       contractType = "008";
                        // contractType = "002"; //Renting por enquanto enviar 002 e o ideal é enviar 008
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

                    if (bB_Printing.ContractDuration == 12)
                    {
                        vtLaufk = "Z1";
                    }
                    if (bB_Printing.ContractDuration == 24)
                    {
                        vtLaufk = "Z2";
                    }
                    if (bB_Printing.ContractDuration == 36)
                    {
                        vtLaufk = "Z3";
                    }
                    if (bB_Printing.ContractDuration == 48)
                    {
                        vtLaufk = "Z4";
                    }
                    if (bB_Printing.ContractDuration == 60)
                    {
                        vtLaufk = "Z5";
                    }
                    if (bB_Printing.ContractDuration == 72)
                    {
                        vtLaufk = "Z6";
                    }
                    if (bB_Printing.ContractDuration == 84)
                    {
                        vtLaufk = "Z7";
                    }
                    if (bB_Printing.ContractDuration == 96)
                    {
                        vtLaufk = "Z8";
                    }

                    var collectionContracts = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS>();
                
                    List<XMLOrders> groups = new List<XMLOrders>();

                    string bdConnect = ConfigurationManager.AppSettings["BasedadosConnect"].ToString();
                    int i = 0;
                    Random random = new Random();
                    int randomNumberAddress = random.Next(1000000, 10000000);

                    using (SqlConnection conn = new SqlConnection(bdConnect))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("GetXMLOrders", conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ProposalId", proposalId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            try
                            {
                                XMLOrders order = new XMLOrders
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    CodeRef = reader["CodeRef"].ToString(),
                                    ItemGroup = Convert.ToInt32(reader["ItemGroup"]),
                                    IDX = Convert.ToInt32(reader["IDX"]),
                                    ContactName = reader["ContactName"].ToString(),
                                    ContactSurname = reader["ContactSurname"].ToString(),
                                    ContactMovil = reader["ContactMovil"].ToString(),
                                    Schedule = reader["Schedule"].ToString(),
                                    DLFloor = reader["DLFloor"].ToString(),
                                    Department = reader["Department"].ToString(),
                                    Building = reader["Building"].ToString(),
                                    Room = reader["Room"].ToString(),
                                    City = reader["City"].ToString(),
                                    TypeOfOrder = Convert.ToInt32(reader["TypeOfOrder"])
                                };

                                groups.Add(order);

                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                                return null;
                            }
                        }

                    }


                    int noOrders = groups.Count();
                    DateTime? createdTimeContractN = c.CreatedTime;
                    if (createdTimeContractN.HasValue)
                    {
                        DateTime createdTime = createdTimeContractN.Value;
                        formattedDtCont = createdTime.ToString("yyyyMMdd");
                    }

                    BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == proposalId).FirstOrDefault();
                    
                    if ((bool)proposal.IsMultipleContract)
                    {
                        int indexMC = 1;
                        string contractIndexStringMC = indexMC.ToString();
                        collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                        {


                            CONTR_DOC = $"C_D{proposalId}_{randomLetterNunber}",
                            //CONTR_DOC = $"C_D3924_1_{randomLetterNunber}",
                            VT_AUART = "ZWV1",
                            VT_BEGDA = formattedDtCont,
                            VT_ABNDA = "",
                            VT_VTART = contractType,
                            VT_SERWI = "BES",             //"BES",
                            VT_ESCAL = "08",
                            VT_AUGRU = "ZCC",             //input de um campo -> ZCC = só 1 contrato   // ZCS
                            VT_LAUFK = vtLaufk,
                            VT_VLAUFZ = bB_Printing.ContractDuration.ToString(),
                            VT_VLAUFE = "3",
                            VT_ANZPOS = noOrders.ToString(),
                            VT_VUNDAT = FirstDayNextMonthString,
                            VT_ZTERM = "453E",
                            VT_FAKSK = "ZN",
                            VT_SAP_CONTRACT = proposal.ContractNumberPai
                        });
                    }
                    else
                    {
                        int index = 1;
                        string contractIndexString = index.ToString();
                        collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                        {
                            CONTR_DOC = $"C_D{proposalId}_{randomLetterNunber}",
                            //CONTR_DOC = $"C_D3924_1_{randomLetterNunber}",
                            VT_AUART = "ZWV1",
                            VT_BEGDA = formattedDtCont,
                            VT_ABNDA = "",
                            VT_VTART = contractType,
                            VT_SERWI = "BES",             //"BES",
                            VT_ESCAL = "08",
                            VT_AUGRU = "ZCS",             //input de um campo -> ZCC = só 1 contrato   // ZCS
                            VT_LAUFK = vtLaufk,
                            VT_VLAUFZ = bB_Printing.ContractDuration.ToString(),
                            VT_VLAUFE = "3",
                            VT_ANZPOS = noOrders.ToString(),
                            VT_VUNDAT = FirstDayNextMonthString,
                            VT_ZTERM = "453E",
                            VT_FAKSK = "ZN"
                        });
                    }
                    //int index = 1;
                    //string contractIndexString = index.ToString();
                    //collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                    //{


                    //    CONTR_DOC = $"C_D{c.ID}_{contractIndexString}_{randomLetterNunber}",
                    //    //CONTR_DOC = $"C_D3924_1_{randomLetterNunber}",
                    //    VT_AUART = "ZWV1",
                    //    VT_BEGDA = formattedDtCont,
                    //    VT_ABNDA = "",
                    //    VT_VTART = contractType,
                    //    VT_SERWI = "BES",             //"BES",
                    //    VT_ESCAL = "08",
                    //    VT_AUGRU = isMultipleContract == false ? "ZCS" : "ZCC",             //input de um campo -> ZCC = só 1 contrato   // ZCS
                    //    VT_LAUFK = vtLaufk,
                    //    VT_VLAUFZ = bB_Printing.ContractDuration.ToString(),
                    //    VT_VLAUFE = "3",
                    //    VT_ANZPOS = noOrders.ToString(),
                    //    VT_VUNDAT = FirstDayNextMonthString,
                    //    VT_ZTERM = "453E",
                    //    VT_FAKSK = "ZN"
                    //});
                    //if (contracts != null)
                    //{
                    //    List<BB_Proposal_ItemDoBasket> bb_itemsDoBasket = new List<BB_Proposal_ItemDoBasket>();
                    //    int? firstItemGroup = 0;
                    //    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).ToList();
                    //    foreach (var dLocations in dl)
                    //    {
                    //        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == dLocations.IDX).OrderBy(x => x.Group).ToList();
                    //        foreach (var items in itemsDoBasket)
                    //        {
                    //            BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == items.CodeRef).FirstOrDefault();
                    //            if (bB_Equipamentos != null)
                    //            {
                    //                if (items.Group != firstItemGroup)
                    //                {
                    //                    firstItemGroup = items.Group;

                    //                    bb_itemsDoBasket.Add(items);
                    //                }
                    //            }
                    //        }

                    //    }
                    //    DateTime currentDate = DateTime.Now;
                    //    string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                    //    int noOrders = bb_itemsDoBasket.Count();
                    //    bool? isMultipleContract = db.BB_Proposal.Where(x => x.ID == proposalId).Select(x => x.IsMultipleContract).FirstOrDefault();

                    //    collectionContracts.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_CONTRACTS
                    //    {


                    //        CONTR_DOC = $"C_D3924_1_{randomLetterNunber}",
                    //        VT_AUART = "ZWV1",
                    //        VT_BEGDA = formattedCurrentDate,
                    //        VT_ABNDA = "",
                    //        VT_VTART = contractType,
                    //        VT_SERWI = "BES",       //"BES",
                    //        VT_ESCAL = "08",        //TODO: Para depois do GO LIVE -- ver com a adm os possiveis valores. 
                    //        VT_AUGRU = isMultipleContract == false ? "ZCS" : "ZCC",       //input de um campo -> ZCC = só 1 contrato   // ZCS           //TODO: Falar com a Mylene -- BB_Proposal
                    //        VT_LAUFK = vtLaufk,
                    //        VT_VLAUFZ = pd.PrazoDiferenciado.ToString(),
                    //        VT_VLAUFE = "3",        //TODO: Para depois do GO LIVE                                                                                                                                                               
                    //        VT_ANZPOS = "1",//noOrders.ToString(),
                    //        VT_VUNDAT = FirstDayNextMonthString,
                    //        VT_ZTERM = "453E",       //TODO: FinancingPaymentMethods.
                    //        VT_FAKSK = "X"
                    //    });
                    //}

                    stopwatch.Stop();

                    Console.WriteLine($"Order - ConfigOrders: {stopwatch.ElapsedMilliseconds} ms");

                    return collectionContracts;
                }

            }catch(Exception ex)
            {
                ex.Message.ToString();
                return null;
            }

        }
    }
}