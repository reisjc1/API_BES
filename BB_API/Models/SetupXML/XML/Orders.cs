
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.BLL;
using WebApplication1.Controllers;
using WebApplication1.Models.ViewModels;
using static WebApplication1.Models.SetupXML.XSD;


namespace WebApplication1.Models.SetupXML.XML
{
    public class Orders
    {
        //public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> ConfigOrders(int proposalId, string randomLetterNunber)
        public OrdersPartnersList ConfigOrders(int proposalId, string randomLetterNunber, string financing, string contractDoc)
        {
            try
            {

                //string contractDoc = "";
                List<OrdersPartners> sdDocOrdersPartners = new List<OrdersPartners>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    //List<BB_Equipamentos> maquinas = new List<BB_Equipamentos>();
                    BB_Equipamentos  maquina = new BB_Equipamentos();
                    LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_Proposal d = db.BB_Proposal.Where(x => x.ID == proposalId).FirstOrDefault();
                    //LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID != d.ID).FirstOrDefault();
                    BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == d.ID).FirstOrDefault();
                    BB_FinancingContractType ct = db.BB_FinancingContractType.Where(x => x.ID == pf.ContractTypeId).FirstOrDefault();
                    BB_Campanha ca = db.BB_Campanha.Where(x => x.ID == d.CampaignID).FirstOrDefault();



                    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == d.ID).ToList();
                   

                    Random random = new Random();
                    int randomNumberOrderDoc = random.Next(1000000, 10000000);
                    string randomNumberOrderString = randomNumberOrderDoc.ToString();


                    int index = 1;
                    var collectionOrders = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();

                    foreach (var deliveryLocation in dl)
                    {
                        int? groupNumber = null;
                        Dictionary<BB_Proposal_ItemDoBasket, int> groups = new Dictionary<BB_Proposal_ItemDoBasket, int>();
                        //List<BB_Proposal_ItemDoBasket> groups = new List<BB_Proposal_ItemDoBasket>();
                        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX).OrderBy(x => x.Group).ToList();
                     
                        foreach (var itemdoBsket in itemsDoBasket)
                        {
                            if (itemdoBsket.Group != groupNumber)
                            {
                                maquina = db.BB_Equipamentos.Where(m => m.CodeRef == itemdoBsket.CodeRef).FirstOrDefault();
                                
                                if(maquina != null)
                                {

                                    groups.Add(itemdoBsket,1);
                                    groupNumber = itemdoBsket.Group;
                                }
                                else
                                {

                                    if(itemdoBsket.Description.Contains("MAIN MATERIAL"))
                                    {
                                        groups.Add(itemdoBsket,2);
                                        groupNumber = itemdoBsket.Group;
                                    }
                                }


                            }
                        }


                        foreach (var order in groups.Where(x => x.Value == 1))
                        {

                            var collectionOrderItems = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                            var collectionOrdersContact = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 
                            var collectionOrdersFinance = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE>();
                            var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>(); 
                            string contractIndexString = index.ToString();
                            string bundelCodeRef = "";

                            string orderDoc = $"O_L{randomNumberOrderString}_{contractIndexString}_{randomLetterNunber}";

                            OrdersPartners sdDocOrderPartner = new OrdersPartners();
                            sdDocOrderPartner.OrderId = order.Key.ID;
                            sdDocOrderPartner.Sd_Doc = orderDoc;
                            sdDocOrdersPartners.Add(sdDocOrderPartner);
                            bool firstItemGroup = true;

                            //Utilizado no Z1ZVOE_ORDERS
                            string contractItm = contractIndexString + "0";
                            List<BB_Proposal_ItemDoBasket> group = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX && x.Group == order.Key.Group).ToList();
                            
                            int itm_number = 10;
                            bool isMachine = false;
                            foreach (var item in group)
                            {
                                BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                                

                                BB_Proposal_ItemDoBasket lastItemGroup = group.Last();

                                if (bB_Equipamentos != null)
                                {
                                    isMachine = true;
                                    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                    {
                                        SD_DOC = orderDoc,
                                        ITM_NUMBER = itm_number.ToString(), // contractItm,
                                        MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                        REQ_QTY = item.Qty.ToString(),
                                        MODEL_YN = "Y" // Perguntar ao Luis
                                    });

                                    bundelCodeRef = item.CodeRef;
                                    firstItemGroup = false;
                                    itm_number = itm_number + 10;
                                }
                                else
                                {
                                    if (item == lastItemGroup && isMachine == false)
                                    {
                                        collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                        {
                                            SD_DOC = orderDoc,
                                            ITM_NUMBER = itm_number.ToString(), // contractItm,
                                            MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                            REQ_QTY = item.Qty.ToString(),
                                            HG_LV_ITEM = "10",
                                            MODEL_YN = "Y" // Perguntar ao Luis
                                        });
                                        bundelCodeRef = item.CodeRef;
                                    }
                                    else
                                    {
                                        collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                        {
                                            SD_DOC = orderDoc,
                                            ITM_NUMBER = itm_number.ToString(), // contractItm,
                                            MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                            REQ_QTY = item.Qty.ToString(),
                                            HG_LV_ITEM = "10",
                                            MODEL_YN = "Y" // Perguntar ao Luis
                                        });

                                    }

                                    itm_number = itm_number + 10;
                                }
                            }
                            //BB_Proposal_OPSManage ops = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                            //if(ops != null)
                            //{
                            //    string line1 = ops.CodeRef;
                            //    string line2 = "9960DRC-HTTP ";
                            //    BB_OPS_Manage_Packs opsPack = db.BB_OPS_Manage_Packs.Where(x => x.CodeRef == line2).FirstOrDefault();
                            //    double? opsPvp = (ops.PVP * ops.TotalMonths) - opsPack.PVP;
                            //    double? opsPvpLine2 = opsPack.PVP;
                            //    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                            //    {
                            //        SD_DOC = orderDoc,
                            //        ITM_NUMBER = itm_number.ToString(), // contractItm,
                            //        MATERIAL = line1, //"A6DR021",//order.CodeRef,
                            //        REQ_QTY = "1",
                            //        MODEL_YN = "Y" // Perguntar ao Luis
                            //    });
                            //    itm_number += 10;
                            //    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                            //    {
                            //        SD_DOC = orderDoc,
                            //        ITM_NUMBER = itm_number.ToString(), // contractItm,
                            //        MATERIAL = line2, //"A6DR021",//order.CodeRef,
                            //        REQ_QTY = "1",
                            //        MODEL_YN = "Y" // Perguntar ao Luis
                            //    });

                            //}

                            BB_Proposal_DL_ClientContacts dLClient = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == deliveryLocation.DeliveryContact).FirstOrDefault();

                            collectionOrderCLickPrices = ClickPrices(d.ID, orderDoc, order.Key.CodeRef);

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
                            //if (financing == "AL")
                            //{
                            var LEAS_ZTERM = "";
                            if (pf.Months == 60)
                            {
                                LEAS_ZTERM = "E30D";
                            }else if(pf.Months == 48 || pf.Months == 0)
                            {
                                LEAS_ZTERM = "E60D";

                            }
                            collectionOrdersFinance.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE
                            {
                                SD_DOC = orderDoc,
                                FINANCE_TYPE = financing,
                                LEAS_KUNNR = ct.CompanyCode,
                                LEAS_LVTNR = pf.AgreementNumber,
                                LEAS_LFAKT = "1",
                                LEAS_ZTERM = LEAS_ZTERM,
                                LEAS_LEABG = String.Format("{0:yyyyMMdd}",pf.DateApproval),
                                KBETR1 = "",
                                KBETR2 = "",
                                LEAS_LEPER = "",
                                LEAS_LRYTH = "1",
                                LEAS_LKAUP = "2.5",
                                BILL_TO = d.ClientAccountNumber
                            }) ;
                            //}

                            collectionOrderItems = new Collection<WebApplication1.Models.SetupXML.XSD.Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>(collectionOrderItems.OrderBy(x => x.ITM_NUMBER).ToList());

                            DateTime currentDate = DateTime.Now;
                            string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                            collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                            {
                                SD_DOC = orderDoc,
                                DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                                REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                                REF_1 = order.Key.Name, //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                PURCH_NO_C = d.CRM_QUOTE_ID,  //Nome interno da oferta
                                SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                                PMNTTRMS = "303E", //TODO: manter  || FinancingPaymentMethods.
                                CONTRACT_DOC = contractDoc, //$"C_{c.ID}_1_{randomLetterNunber}",   //contractDoc,
                                CONTRACT_ITM = contractItm, // add +10 no foreach de orders  
                                MACHINE = bundelCodeRef, //"A63R021",      /*dataIntegration.CodeRef, *///"A63R021",       //order.CodeRef,   // order.CodeRef,                  //"A6DR021",                  //order.CodeRef,
                                ORDER_FLAG = "O1", //TODO: MANTER ESTE VALOR;
                                LINKING_PIN = proposalId.ToString(),
                                Z1ZVOE_ORDER_CONTACT = collectionOrdersContact,
                                Z1ZVOE_ORDER_ITEMS = collectionOrderItems,
                                Z1ZVOE_CLICK_PRICES = collectionOrderCLickPrices,
                                Z1ZVOE_FINANCE = collectionOrdersFinance


                            });


                            index++;
                        }

                        foreach (var order in groups.Where(x => x.Value == 2))
                        {

                            var collectionOrderItems = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                            var collectionOrdersContact = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 
                            var collectionOrdersFinance = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE>();
                            //var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>();
                            string contractIndexString = index.ToString();
                            string bundelCodeRef = "";

                            string orderDoc = $"O_L{randomNumberOrderString}_{contractIndexString}_{randomLetterNunber}";

                            OrdersPartners sdDocOrderPartner = new OrdersPartners();
                            sdDocOrderPartner.OrderId = order.Key.ID;
                            sdDocOrderPartner.Sd_Doc = orderDoc;
                            sdDocOrdersPartners.Add(sdDocOrderPartner);
                            bool firstItemGroup = true;

                            //Utilizado no Z1ZVOE_ORDERS
                            string contractItm = contractIndexString + "0";
                            List<BB_Proposal_ItemDoBasket> group = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocation.IDX && x.Group == order.Key.Group).ToList();

                            int itm_number = 20;
                            bool isMachine = false;
                            foreach (var item in group)
                            {
                                //BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();


                                //BB_Proposal_ItemDoBasket lastItemGroup = group.Last();

                                if (item.Description.Contains("MAIN MATERIAL"))
                                {
                                    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                    {
                                        SD_DOC = orderDoc,
                                        ITM_NUMBER = "10", // contractItm,
                                        MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                        REQ_QTY = item.Qty.ToString(),
                                        MODEL_YN = "Y" // Perguntar ao Luis
                                    });

                                    bundelCodeRef = item.CodeRef;
                                    itm_number = itm_number + 10;
                                }
                                else
                                {
                                    collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                    {
                                        SD_DOC = orderDoc,
                                        ITM_NUMBER = itm_number.ToString(), // contractItm,
                                        MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                        REQ_QTY = item.Qty.ToString(),
                                        HG_LV_ITEM = "10",
                                        MODEL_YN = "Y" // Perguntar ao Luis
                                    });

                                    itm_number = itm_number + 10;
                                }
                            }
                            

                            BB_Proposal_DL_ClientContacts dLClient = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == deliveryLocation.DeliveryContact).FirstOrDefault();

                            //collectionOrderCLickPrices = ClickPrices(d.ID, orderDoc, order.Key.CodeRef);

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
                            //if (financing == "AL")
                            //{
                            var LEAS_ZTERM = "";
                            if (pf.Months == 60)
                            {
                                LEAS_ZTERM = "E30D";
                            }
                            else if (pf.Months == 48 || pf.Months == 0)
                            {
                                LEAS_ZTERM = "E60D";

                            }
                            collectionOrdersFinance.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE
                            {
                                SD_DOC = orderDoc,
                                FINANCE_TYPE = financing,
                                LEAS_KUNNR = ct.CompanyCode,
                                LEAS_LVTNR = pf.AgreementNumber,
                                LEAS_LFAKT = "1",
                                LEAS_ZTERM = LEAS_ZTERM,
                                LEAS_LEABG = String.Format("{0:yyyyMMdd}", pf.DateApproval),
                                KBETR1 = "",
                                KBETR2 = "",
                                LEAS_LEPER = "",
                                LEAS_LRYTH = "1",
                                LEAS_LKAUP = "2.5",
                                BILL_TO = d.ClientAccountNumber
                            });
                            //}

                            collectionOrderItems = new Collection<WebApplication1.Models.SetupXML.XSD.Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>(collectionOrderItems.OrderBy(x => x.ITM_NUMBER).ToList());

                            DateTime currentDate = DateTime.Now;
                            string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                            collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                            {
                                SD_DOC = orderDoc,
                                DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                                REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                                REF_1 = order.Key.Name, //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                PURCH_NO_C = d.CRM_QUOTE_ID,  //Nome interno da oferta
                                SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                                PMNTTRMS = "303E", //TODO: manter  || FinancingPaymentMethods.
                                CONTRACT_DOC = contractDoc, //$"C_{c.ID}_1_{randomLetterNunber}",   //contractDoc,
                                CONTRACT_ITM = contractItm, // add +10 no foreach de orders  
                                MACHINE = bundelCodeRef, //"A63R021",      /*dataIntegration.CodeRef, *///"A63R021",       //order.CodeRef,   // order.CodeRef,                  //"A6DR021",                  //order.CodeRef,
                                ORDER_FLAG = "O1", //TODO: MANTER ESTE VALOR;
                                LINKING_PIN = proposalId.ToString(),
                                Z1ZVOE_ORDER_CONTACT = collectionOrdersContact,
                                Z1ZVOE_ORDER_ITEMS = collectionOrderItems,
                                //Z1ZVOE_CLICK_PRICES = collectionOrderCLickPrices,
                                Z1ZVOE_FINANCE = collectionOrdersFinance


                            });


                            index++;
                        }

                    }

                    var collectionOrderItemsRetiradas = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                    var collectionOrdersContactRetiradas = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 

                    BB_Proposal_Upturn bB_Proposal_Upturn = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    if(bB_Proposal_Upturn != null)
                    {
                        if((bool)bB_Proposal_Upturn.Retirada)
                    {
                        Random randomRetirada = new Random();
                        int randomNumberOrderDocRetirada = randomRetirada.Next(1000000, 10000000);
                        string randomNumberOrderRetiradaString = randomNumberOrderDocRetirada.ToString();

                        string orderRetiradaDoc = $"O_R{randomNumberOrderRetiradaString}_{randomLetterNunber}";
                        collectionOrderItemsRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                        {
                            SD_DOC = orderRetiradaDoc,
                            ITM_NUMBER = "10", // contractItm,
                            MATERIAL = "9960DX00058", //"A6DR021",//order.CodeRef,
                            REQ_QTY = "1"
                        });

                            if (bB_Proposal_Upturn.Contact != null)
                            {
                                string[] splitContact = bB_Proposal_Upturn.Contact.Split(';');

                                collectionOrdersContactRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                {
                                    SD_DOC = orderRetiradaDoc,
                                    APLF_NAME = (splitContact != null && !string.IsNullOrEmpty(splitContact[0]) ? splitContact[0] : ""), //"M. LUIS ALVAREZ",
                                    APLF_PHON = (splitContact != null && !string.IsNullOrEmpty(splitContact[1]) ? splitContact[1] : ""),       //"66666666",
                                    APLF_OPEN = (splitContact != null && !string.IsNullOrEmpty(splitContact[3]) ? splitContact[3] : ""),//"9h 17h",
                                    APLF_INFO = "",//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                    APLF_INFO2 = "Marque/Modèle: Konica Minolta / " + (splitContact != null && !string.IsNullOrEmpty(splitContact[2]) ? splitContact[2] : "") + "-N°:" +  bB_Proposal_Upturn.Description,//"Asc: Oui -Connexion: PRINTFLEET",
                                    APLF_INFO3 = "2024431493"

                                });
                            }
                            else
                            {
                                collectionOrdersContactRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                {
                                    SD_DOC = orderRetiradaDoc,
                                    APLF_NAME = "M. LUIS ALVAREZ",
                                    APLF_PHON = "66666666",
                                    APLF_OPEN = "9h 17h",
                                    APLF_INFO = "Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                    APLF_INFO2 = "Asc: Oui -Connexion: PRINTFLEET",
                                    APLF_INFO3 = "comentario ship to 14733442024402907 ESC COM IDMON ASC"
                                });
                            }
                        DateTime currentDate = DateTime.Now;
                        string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                        collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                        {
                            SD_DOC = orderRetiradaDoc,
                            DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                            REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                            REF_1 = "SDR252146", //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                            PURCH_NO_C = "RETIRAR bizhub 36",  //Nome interno da oferta
                            SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                            PMNTTRMS = "E6CD", //TODO: manter  || FinancingPaymentMethods.
                            MACHINE = "5R", //"A63R021",      /*dataIntegration.CodeRef, *///"A63R021",       //order.CodeRef,   // order.CodeRef,                  //"A6DR021",                  //order.CodeRef,
                            ORDER_FLAG = "5R", //TODO: MANTER ESTE VALOR;
                            EQUIPMENT_NO = bB_Proposal_Upturn.Description,
                            DWERK = "5300",
                            LINKING_PIN = proposalId.ToString(),
                            Z1ZVOE_ORDER_CONTACT = collectionOrdersContactRetiradas,
                            Z1ZVOE_ORDER_ITEMS = collectionOrderItemsRetiradas
                        });
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

                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo i = new LoadProposalInfo();
                i.ProposalId = proposalId;
                ActionResponse a = p1.LoadProposal(i);

                using (var db = new BB_DB_DEVEntities2())
                {
                    DateTime FirstDayofThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime FirstDayofTheNextMonth = FirstDayofThisMonth.AddMonths(1);
                    string FirstDayNextMonthString = FirstDayofTheNextMonth.ToString("yyyyMMdd");
                    //BB_Proposal_PrintingServices2 printingServices2 = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //BB_PrintingServices printingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingServices2.ID).FirstOrDefault();

                    //BB_Proposal_PrintingServiceValidationRequest bB_Proposal_PrintingServiceValidationRequest = db.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == printingServices.ID && x.IsApproved == true).FirstOrDefault();

                    ////BB_Proposal_Quote_RS quoteRs = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //BB_PrintingServices_NoVolume noVolume = db.BB_PrintingServices_NoVolume.Where(x => x.PrintingServiceID == printingServices.ID).FirstOrDefault();
                    //BB_PrintingServices_ClickPerModel clickPerModel = db.BB_PrintingServices_ClickPerModel.Where(x => x.PrintingServiceID == printingServices.ID).FirstOrDefault();
                    //BB_VVA vVA = db.BB_VVA.Where(x => x.PrintingServiceID == printingServices.ID).FirstOrDefault();
                    int? copiasIncludias = 0;
                    ApprovedPrintingService activePS = null;

                    if(a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                    {
                        activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];

                        if(activePS != null)
                        {

                            if (activePS.BWVolume > 0 && activePS.CVolume > 0)
                            {
                                mATNR = "TOCO";
                                kLFN = "2";
                                if (activePS.GlobalClickNoVolume != null)
                                {
                                    kBETR = Math.Round(activePS.GlobalClickNoVolume.GlobalClickC, 5).ToString().Replace(",", ".");
                                }
                                else if (activePS.ClickPerModel != null)
                                {
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    if (pSM != null)
                                    {
                                        kBETR = Math.Round((double)pSM.ApprovedC, 5).ToString().Replace(",", ".");
                                    }
                                    //kBETR = clickPerModel
                                }
                                else if (activePS.GlobalClickVVA != null)
                                {
                                    kBETR = Math.Round(activePS.GlobalClickVVA.CExcessPVP, 5).ToString().Replace(",", ".");
                                    copiasIncludias = activePS.CVolume;
                                    kSTBM = copiasIncludias.ToString();
                                }

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

                                mATNR = "TOBW";
                                kLFN = "1";
                            
                                if (activePS.GlobalClickNoVolume != null)
                                {
                                    kBETR = Math.Round(activePS.GlobalClickNoVolume.GlobalClickBW, 5).ToString().Replace(",", ".");
                                }
                                else if (activePS.ClickPerModel != null)
                                {
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    if (pSM != null)
                                    {
                                        kBETR = Math.Round((double)pSM.ApprovedBW, 5).ToString().Replace(",", ".");
                                    }
                                }
                                else if (activePS.GlobalClickVVA != null)
                                {
                                    kBETR = Math.Round(activePS.GlobalClickVVA.BWExcessPVP, 5).ToString().Replace(",", ".");
                                    copiasIncludias = activePS.BWVolume;
                                    kSTBM = copiasIncludias.ToString();
                                }


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
                            if (activePS.BWVolume > 0 && activePS.CVolume == 0)
                            {
                                mATNR = "TOBW";
                                kLFN = "1";
                                if (activePS.GlobalClickNoVolume != null)
                                {
                                    kBETR = Math.Round(activePS.GlobalClickNoVolume.GlobalClickBW,5).ToString().Replace(",", ".");
                                }
                                else if (activePS.ClickPerModel != null)
                                {
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    if (pSM != null)
                                    {
                                        kBETR = Math.Round((double)pSM.ApprovedBW, 5).ToString().Replace(",", ".");
                                    }
                                }
                                else if (activePS.GlobalClickVVA != null)
                                {
                                    kBETR = Math.Round(activePS.GlobalClickVVA.BWExcessPVP, 5).ToString().Replace(",", ".");
                                }

                                copiasIncludias = activePS.BWVolume + activePS.CVolume;
                                kSTBM = copiasIncludias.ToString();

                                collectionOrderCLickPrices.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES
                                {
                                    SD_DOC = orderDoc,
                                    MATNR = mATNR, // códigos de cor ou black and white     TOCO -> cor    TOBW-> black and white 
                                    KLFN1 = kLFN, // se for TOBW - 1         se for TOCO->2 
                                    DATAB = FirstDayNextMonthString, //data a partir do momento que é valido  -- primeiro do mês seguinte
                                    DATBI = "99991231", // data de até quando é válido -- deixar default
                                    KSTBM = activePS.GlobalClickVVA != null ? kSTBM : "0", //copias incluidas 
                                    KBETR = kBETR //preço do excedente 
                                });
                            }

                        }

                    }
                    
                                       
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
