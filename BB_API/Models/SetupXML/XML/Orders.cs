
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using static WebApplication1.Models.SetupXML.XSD;


namespace WebApplication1.Models.SetupXML.XML
{
    public class Orders
    {
        //public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> ConfigOrders(int proposalId, string randomLetterNunber)
        public OrdersPartnersList ConfigOrders(int proposalId, string randomLetterNunber, string financing)
        {
            try
            {

                string contractDoc = "";
                List<OrdersPartners> sdDocOrdersPartners = new List<OrdersPartners>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_Equipamentos> maquinas = new List<BB_Equipamentos>();

                    BB_Proposal d = db.BB_Proposal.Where(x => x.ID == proposalId).FirstOrDefault();
                    //LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID != proposalId).FirstOrDefault();
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_FinancingContractType ct = db.BB_FinancingContractType.Where(x => x.ID == pf.ContractTypeId).FirstOrDefault();
                    BB_Campanha ca = db.BB_Campanha.Where(x => x.ID == d.CampaignID).FirstOrDefault();



                    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).ToList();
                    //try
                    //{
                    //    contractDoc = $"C_D{c.ID}_1_{randomLetterNunber}";
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.ToString());
                    //}


                    Random random = new Random();
                    int randomNumberOrderDoc = random.Next(1000000, 10000000);
                    string randomNumberOrderString = randomNumberOrderDoc.ToString();


                    int index = 1;
                    var collectionOrders = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();



                    foreach (var deliveryLocation in dl)
                    {
                        int? groupNumber = null;
                        List<BB_Proposal_ItemDoBasket> groups = new List<BB_Proposal_ItemDoBasket>();
                        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX).OrderBy(x => x.Group).ToList();
                        //List<BB_Proposal_ItemDoBasket> groups = db.BB_Proposal_ItemDoBasket.W
                        foreach (var itemdoBsket in itemsDoBasket)
                        {
                            if (itemdoBsket.Group != groupNumber)
                            {
                                groups.Add(itemdoBsket);
                                groupNumber = itemdoBsket.Group;
                            }
                        }


                        foreach (var order in groups)
                        {

                            var collectionOrderItems = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                            var collectionOrdersFinance = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE>();
                            var collectionOrdersContact = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 
                            var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>(); ;
                            string contractIndexString = index.ToString();
                            string bundelCodeRef = "";

                            string orderDoc = $"O_L{randomNumberOrderString}_{contractIndexString}_{randomLetterNunber}";

                            OrdersPartners sdDocOrderPartner = new OrdersPartners();
                            sdDocOrderPartner.OrderId = order.ID;
                            sdDocOrderPartner.Sd_Doc = orderDoc;
                            sdDocOrdersPartners.Add(sdDocOrderPartner);
                            bool firstItemGroup = true;

                            string contractItm = contractIndexString + "0";
                            List<BB_Proposal_ItemDoBasket> group = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX && x.Group == order.Group).ToList();

                            int itm_number = 30;

                            foreach (var item in group)
                            {
                                //BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                                if (firstItemGroup == true)
                                {
                                    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                    {
                                        SD_DOC = orderDoc,
                                        ITM_NUMBER = "20", // contractItm,
                                        MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                        REQ_QTY = order.Qty.ToString(),
                                        MODEL_YN = "Y" // Perguntar ao Luis
                                    });

                                    bundelCodeRef = item.CodeRef;
                                    firstItemGroup = false;
                                }
                                else
                                {
                                    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                    {
                                        SD_DOC = orderDoc,
                                        ITM_NUMBER = itm_number.ToString(), // contractItm,
                                        MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                        REQ_QTY = order.Qty.ToString(),
                                        MODEL_YN = "Y" // Perguntar ao Luis
                                    });
                                    itm_number = itm_number + 10;
                                }
                            }

                            BB_Proposal_DL_ClientContacts dLClient = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == deliveryLocation.DeliveryContact).FirstOrDefault();

                            collectionOrderCLickPrices = ClickPrices(proposalId, orderDoc, order.CodeRef);

                            //List<Accessories> accessories = GetAcesseries("A63R021");
                            if (dLClient != null)
                            {
                                collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                {
                                    SD_DOC = orderDoc,
                                    APLF_NAME = dLClient.Name + "" + dLClient.Surname, //"M. LUIS ALVAREZ",
                                    APLF_PHON = dLClient.Tel.ToString(),       //"66666666",
                                    APLF_OPEN = deliveryLocation.Schedule,//"9h 17h",
                                    APLF_INFO = deliveryLocation.Floor + "" + deliveryLocation.Department + "" + deliveryLocation.Building + "" + deliveryLocation.Room,//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                    APLF_INFO2 = deliveryLocation.City,//"Asc: Oui -Connexion: PRINTFLEET",

                                });
                            }
                            else
                            {
                                collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                {
                                    SD_DOC = orderDoc,
                                    APLF_NAME = "M. LUIS ALVAREZ",
                                    APLF_PHON = "66666666",
                                    APLF_OPEN = "9h 17h",
                                    APLF_INFO = "Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                    APLF_INFO2 = "Asc: Oui -Connexion: PRINTFLEET",
                                    APLF_INFO3 = "comentario ship to 14733442024402907 ESC COM IDMON ASC"
                                });
                            }
                            if (financing == "AL")
                            {

                                collectionOrdersFinance.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE
                                {
                                    SD_DOC = orderDoc,
                                    FINANCE_TYPE = financing,
                                    LEAS_LVTNR = pd.Alocadora,
                                    LEAS_ZTERM = "E60D"

                                });
                            }
                            DateTime currentDate = DateTime.Now;
                            string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                            collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                            {
                                SD_DOC = orderDoc,
                                DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                                REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                                REF_1 = order.Name, //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                PURCH_NO_C = d.Name,  //Nome interno da oferta
                                SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                                PMNTTRMS = "303E", //TODO: manter  || FinancingPaymentMethods.
                                CONTRACT_DOC = $"C_D3924_1_{randomLetterNunber}",   //contractDoc,
                                CONTRACT_ITM = contractItm, // add +10 no foreach de orders  
                                MACHINE = bundelCodeRef, //"A63R021",      /*dataIntegration.CodeRef, *///"A63R021",       //order.CodeRef,   // order.CodeRef,                  //"A6DR021",                  //order.CodeRef,
                                ORDER_FLAG = "O1", //TODO: MANTER ESTE VALOR;
                                Z1ZVOE_ORDER_CONTACT = collectionOrdersContact,
                                Z1ZVOE_ORDER_ITEMS = collectionOrderItems,
                                Z1ZVOE_CLICK_PRICES = collectionOrderCLickPrices,
                                Z1ZVOE_FINANCE = collectionOrdersFinance


                            });

                            index++;
                        }




                    }
                    OrdersPartnersList sdocOrders = new OrdersPartnersList();
                    sdocOrders.z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERs = collectionOrders;
                    sdocOrders.SdDocOrderPartner = sdDocOrdersPartners;
                    return sdocOrders;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }


        }
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES> ClickPrices(int proposalId, string orderDoc, string codeRef)
        {
            try
            {
                string mATNR = null;
                string kLFN = null;
                string kSTBM = null;
                string kBETR = null;
                var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_PrintingServices2 printingServices2 = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_PrintingServices printingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingServices2.ID).FirstOrDefault();

                    //BB_Proposal_Quote_RS quoteRs = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_PrintingServices_NoVolume noVolume = db.BB_PrintingServices_NoVolume.Where(x => x.PrintingServiceID == printingServices.ID).FirstOrDefault();
                    BB_PrintingServices_ClickPerModel clickPerModel = db.BB_PrintingServices_ClickPerModel.Where(x => x.PrintingServiceID == printingServices.ID).FirstOrDefault();
                    BB_VVA vVA = db.BB_VVA.Where(x => x.PrintingServiceID == printingServices.ID).FirstOrDefault();

                    if (printingServices != null)
                    {
                        if (printingServices.BWVolume > 0 && printingServices.CVolume > 0)
                        {
                            mATNR = "TOCO";
                            kLFN = "2";
                            if (noVolume != null)
                            {
                                kBETR = noVolume.GlobalClickC.ToString();
                            }
                            else if (clickPerModel != null)
                            {
                                BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == printingServices.ID && x.CodeRef == codeRef).FirstOrDefault();
                                if (pSM != null)
                                {
                                    kBETR = pSM.ApprovedC.ToString();
                                }
                                //kBETR = clickPerModel
                            }
                            else if (vVA != null)
                            {
                                kBETR = vVA.PVP.ToString();
                            }
                        }
                        if (printingServices.BWVolume > 0 && printingServices.CVolume == 0)
                        {
                            mATNR = "TOBW";
                            kLFN = "1";
                            if (noVolume != null)
                            {
                                kBETR = noVolume.GlobalClickBW.ToString();
                            }
                            else if (clickPerModel != null)
                            {
                                BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == printingServices.ID && x.CodeRef == codeRef).FirstOrDefault();
                                if (pSM != null)
                                {
                                    kBETR = pSM.ApprovedBW.ToString();
                                }
                            }
                            else if (vVA != null)
                            {
                                kBETR = vVA.PVP.ToString();
                            }
                        }
                        int? copiasIncludias = printingServices.BWVolume + printingServices.CVolume;
                        kSTBM = copiasIncludias.ToString();
                    }




                    DateTime FirstDayofThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime FirstDayofTheNextMonth = FirstDayofThisMonth.AddMonths(1);
                    string FirstDayNextMonthString = FirstDayofTheNextMonth.ToString("yyyyMMdd");

                    collectionOrderCLickPrices.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES
                    {
                        SD_DOC = orderDoc,
                        MATNR = mATNR, // códigos de cor ou black and white     TOCO -> cor    TOBW-> black and white 
                        KLFN1 = kLFN, // se for TOBW - 1         se for TOCO->2 
                        DATAB = FirstDayNextMonthString, //data a partir do momento que é valido  -- primeiro do mês seguinte
                        DATBI = "99991231", // data de até quando é válido -- deixar default
                        KSTBM = kSTBM, //copias incluidas 
                        KBETR = kBETR //preço do excedente 
                    });
                }
                return collectionOrderCLickPrices;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }

        }
      

    }
}

public class Accessories
{
    public string CodeRef { get; set; }
}
