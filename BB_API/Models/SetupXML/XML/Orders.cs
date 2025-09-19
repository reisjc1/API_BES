
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using WebApplication1.BLL;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.ViewModels.PrintingServicesViewModels;
using static WebApplication1.Models.SetupXML.XSD;


namespace WebApplication1.Models.SetupXML.XML
{
    public class Orders
    {
        //public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> ConfigOrders(int proposalId, string randomLetterNunber)
        //Nao esta a ser usado
        public OrdersPartnersList ConfigOrders(int proposalId, string randomLetterNunber, string financing, string contractDoc, LD_Contrato c, BB_Proposal d, BB_Proposal_Financing pf, BB_FinancingContractType ct)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                //string contractDoc = "";
                List<OrdersPartners> sdDocOrdersPartners = new List<OrdersPartners>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    //List<BB_Equipamentos> maquinas = new List<BB_Equipamentos>();
                    BB_Equipamentos  maquina = new BB_Equipamentos();
                    //LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //BB_Proposal d = db.BB_Proposal.Where(x => x.ID == proposalId).FirstOrDefault();
                    //LD_Contrato c = db.LD_Contrato.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID != d.ID).FirstOrDefault();
                    //BB_Proposal_Financing pf = db.BB_Proposal_Financing.Where(x => x.ProposalID == d.ID).FirstOrDefault();
                    //BB_FinancingContractType ct = db.BB_FinancingContractType.Where(x => x.ID == pf.ContractTypeId).FirstOrDefault();
                    BB_Campanha ca = db.BB_Campanha.Where(x => x.ID == d.CampaignID).FirstOrDefault();



                    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == d.ID && x.AccountType == "Ship To").ToList();
                   

                    Random random = new Random();
                    int randomNumberOrderDoc = random.Next(1000000, 10000000);
                    string randomNumberOrderString = randomNumberOrderDoc.ToString();


                    int index = 1;
                    var collectionOrders = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();

                    Dictionary<BB_Proposal_ItemDoBasket, int> groups = new Dictionary<BB_Proposal_ItemDoBasket, int>();

                    foreach (var deliveryLocation in dl)
                    {
                        int? groupNumber = null;
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

                    }

                    foreach (var order in groups)
                    {
                        BB_Proposal_DeliveryLocation deliveryLocationIDX = db.BB_Proposal_DeliveryLocation.Where(x => x.IDX == order.Key.DeliveryLocationID).FirstOrDefault();
                        if (order.Value == 1)
                        {
                            try
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
                                List<BB_Proposal_ItemDoBasket> group = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocationIDX.IDX && x.Group == order.Key.Group).ToList();
                            
                                int itm_number = 20;
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
                                            ITM_NUMBER = "10", // contractItm,
                                            MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                            REQ_QTY = item.Qty.ToString(),
                                            MODEL_YN = "Y" // Perguntar ao Luis
                                        });

                                        bundelCodeRef = item.CodeRef;
                                        firstItemGroup = false;
                                        //itm_number = itm_number + 10;
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
                            
                                BB_Proposal_DL_ClientContacts dLClient = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == deliveryLocationIDX.DeliveryContact).FirstOrDefault();

                                BB_PrintingServices_ClickPerModel_VVA psClickPerModelVVA = new BB_PrintingServices_ClickPerModel_VVA();
                                collectionOrderCLickPrices = ClickPrices(d.ID, orderDoc, order.Key.CodeRef, 1, psClickPerModelVVA);

                                //List<Accessories> accessories = GetAcesseries("A63R021");
                                if (dLClient != null)
                                {
                                    collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                    {
                                        SD_DOC = orderDoc,
                                        APLF_NAME = dLClient.Name + "" + dLClient.Surname, //"M. LUIS ALVAREZ",
                                        APLF_PHON = dLClient.Tel.ToString(),       //"66666666",
                                        APLF_OPEN = deliveryLocationIDX.Schedule,//"9h 17h",
                                        APLF_INFO = deliveryLocationIDX.Floor + "" + deliveryLocationIDX.Department + "" + deliveryLocationIDX.Building + "" + deliveryLocationIDX.Room,//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                        APLF_INFO2 = deliveryLocationIDX.City,//"Asc: Oui -Connexion: PRINTFLEET",

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
                                    LEAS_KUNNR = ct.CompanyCode == null ? d.ClientAccountNumber : ct.CompanyCode,
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
                                    //PAYER = d.ClientAccountNumber
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
                                    PURCH_NO_C = proposalId.ToString(),  //Nome interno da oferta
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

                            }catch(Exception ex)
                            {
                                ex.Message.ToString();
                                return null;
                            }
                        }
                        else
                        {
                            try
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
                                List<BB_Proposal_ItemDoBasket> group = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == deliveryLocationIDX.IDX && x.Group == order.Key.Group).ToList();

                                int itm_number = 20;
                                bool isMachine = false;
                                foreach (var item in group)
                                {
                                    //BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();


                                    //BB_Proposal_ItemDoBasket lastItemGroup = group.Last();

                                    if (item.Description.Contains("MAIN MATERIAL"))
                                    {
                                        string bomMM = db.BB_Data_Integration.Where(M => M.CodeRef == item.CodeRef).Select(x => x.BOM).FirstOrDefault();
                                        collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                        {
                                            SD_DOC = orderDoc,
                                            ITM_NUMBER = "10", // contractItm,
                                            MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                            REQ_QTY = item.Qty.ToString(),
                                            MODEL_YN = "Y" // Perguntar ao Luis
                                        });

                                        bundelCodeRef = bomMM;
                                        //itm_number = itm_number + 10;
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


                                BB_Proposal_DL_ClientContacts dLClient = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == deliveryLocationIDX.DeliveryContact).FirstOrDefault();

                                //collectionOrderCLickPrices = ClickPrices(d.ID, orderDoc, order.Key.CodeRef);

                                //List<Accessories> accessories = GetAcesseries("A63R021");
                                if (dLClient != null)
                                {
                                    collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                    {
                                        SD_DOC = orderDoc,
                                        APLF_NAME = dLClient.Name + "" + dLClient.Surname, //"M. LUIS ALVAREZ",
                                        APLF_PHON = dLClient.Tel.ToString(),       //"66666666",
                                        APLF_OPEN = deliveryLocationIDX.Schedule,//"9h 17h",
                                        APLF_INFO = deliveryLocationIDX.Floor + "" + deliveryLocationIDX.Department + "" + deliveryLocationIDX.Building + "" + deliveryLocationIDX.Room,//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                        APLF_INFO2 = deliveryLocationIDX.City,//"Asc: Oui -Connexion: PRINTFLEET",

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
                                    LEAS_KUNNR = ct.CompanyCode == "" ? d.ClientAccountNumber : ct.CompanyCode,
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
                                    //PAYER = d.ClientAccountNumber
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
                                    PURCH_NO_C = proposalId.ToString(),  //Nome interno da oferta
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
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                                return null;
                            }
                        }
                    }

                        


                    var collectionOrderItemsRetiradas = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                    var collectionOrdersContactRetiradas = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 

                    BB_Proposal_Upturn bB_Proposal_Upturn = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposalId).FirstOrDefault();

                    if(bB_Proposal_Upturn != null)
                    {
                    //    if((bool)bB_Proposal_Upturn.Retirada)
                    //{
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

                        string contactName = bB_Proposal_Upturn.Name + " " + bB_Proposal_Upturn.Surname;
                        string contactPhone = bB_Proposal_Upturn.Tel.ToString();
                        string contactSchedule = bB_Proposal_Upturn.Schedule;
                        string contactInfo = "Depart: " + bB_Proposal_Upturn.Department + " -Plant: " + bB_Proposal_Upturn.Plant;
                        string contactInfo2 = "Marque/Modèle: "+ bB_Proposal_Upturn.Brand + " / " + bB_Proposal_Upturn.Model + "-N°:" + bB_Proposal_Upturn.Equipment_Number;
                        //string contactInfo3 = "";

                        collectionOrdersContactRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                        {
                            SD_DOC = orderRetiradaDoc,
                            APRT_NAME = contactName, //"M. LUIS ALVAREZ",
                            APRT_PHON = contactPhone,       //"66666666",
                            APRT_OPEN = contactSchedule,//"9h 17h",
                            APRT_INFO = contactInfo,//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                            APRT_INFO2 = contactInfo2,//"Asc: Oui -Connexion: PRINTFLEET",
                            APRT_INFO3 = "2024431493"

                        });
                            
                        DateTime currentDate = DateTime.Now;
                        string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                        collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                        {
                            SD_DOC = orderRetiradaDoc,
                            DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                            REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                            REF_1 = "SDR252146", //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                            PURCH_NO_C = "RETIRAR " + bB_Proposal_Upturn.Model,  //Nome interno da oferta
                            SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                            PMNTTRMS = "E6CD", //TODO: manter  || FinancingPaymentMethods.
                            MACHINE = "5R", //"A63R021",      /*dataIntegration.CodeRef, *///"A63R021",       //order.CodeRef,   // order.CodeRef,                  //"A6DR021",                  //order.CodeRef,
                            ORDER_FLAG = "5R", //TODO: MANTER ESTE VALOR;
                            EQUIPMENT_NO = bB_Proposal_Upturn.Equipment_Number,
                            DWERK = "5300",
                            LINKING_PIN = proposalId.ToString(),
                            Z1ZVOE_ORDER_CONTACT = collectionOrdersContactRetiradas,
                            Z1ZVOE_ORDER_ITEMS = collectionOrderItemsRetiradas
                        });
                    //}
                    }


                    OrdersPartnersList sdocOrders = new OrdersPartnersList();
                    sdocOrders.z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERs = collectionOrders;
                    sdocOrders.SdDocOrderPartner = sdDocOrdersPartners;

                    stopwatch.Stop();

                    Console.WriteLine($"Order - ConfigOrders: {stopwatch.ElapsedMilliseconds} ms");

                    return sdocOrders;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }


        }
        //---------------------//
        public OrdersPartnersList ConfigOrdersV2(int proposalId, string randomLetterNunber, string financing, string contractDoc, LD_Contrato c, BB_Proposal d, BB_Proposal_Financing pf, BB_FinancingContractType ct)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                List<OrdersPartners> sdDocOrdersPartners = new List<OrdersPartners>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Equipamentos maquina = new BB_Equipamentos();

                    BB_Campanha ca = db.BB_Campanha.Where(x => x.ID == d.CampaignID).FirstOrDefault();
                    List<BB_Proposal_Quote> quote_lst = db.BB_Proposal_Quote.AsNoTracking().Where(x => x.Proposal_ID == proposalId && x.IsUsed == true).ToList();
                    Random random = new Random();
                    int randomNumberOrderDoc = random.Next(1000000, 10000000);
                    string randomNumberOrderString = randomNumberOrderDoc.ToString();


                    int index = 1;
                    var collectionOrders = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();

                    List<XMLOrders> groups = new List<XMLOrders>();

                    string bdConnect = ConfigurationManager.AppSettings["BasedadosConnect"].ToString();
                    int i = 0;
                    //Random random = new Random();
                    int randomNumberAddress = random.Next(1000000, 10000000);
                    int machineCounter = 0;

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
                                    IsUsedMachine = (bool?)reader["IsUsedMachine"],
                                    SerialNumber = reader["SerialNumber"].ToString(),
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
                                    TypeOfOrder = Convert.ToInt32(reader["TypeOfOrder"]),
                                    Comments = reader["Comments"].ToString()
                                };

                                groups.Add(order);

                                if(order.TypeOfOrder == 1)
                                {
                                    machineCounter++;
                                }

                            }catch(Exception ex)
                            {
                                ex.Message.ToString();
                                return null;
                            }   
                        }
                        
                    }
                    List<BB_Equipamentos> bB_EquipamentosLst = db.BB_Equipamentos.ToList();
                    List<int> orderIds = groups.Select(order => order.IDX).ToList();

                    List<BB_Proposal_ItemDoBasket> itemsBasketGroup = db.BB_Proposal_ItemDoBasket
                        .Where(x => x.DeliveryLocationID.HasValue && orderIds.Contains(x.DeliveryLocationID.Value))
                        .ToList();
                    List<BB_Proposal_DeliveryLocation> list_ShipTo = db.BB_Proposal_DeliveryLocation
                        .Where(x => x.ProposalID == proposalId && x.AccountType == "Ship To").ToList();
                    List<BB_Proposal_DeliveryLocation> list_DeliveryLocations = db.BB_Proposal_DeliveryLocation
                        .Where(x => x.ProposalID == proposalId && x.AccountType == "Bill To").ToList();

                    //var count = 0;

                    //foreach(var used in quoteUsed_lst)
                    //{
                    //    count += Convert.ToInt32(used.Qty);
                    //}
                    //Serviço de printing que está associado ao negócio
                    var printingService2ID = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposalId).FirstOrDefault();
                    //Guardar número de serviço de printing ativo
                    int indexActivePs = (int)printingService2ID.ActivePrintingService;
                    List<BB_VVA> bB_VVA_lst = db.BB_VVA.AsNoTracking().ToList();
                    List<BB_PrintingService_Machines> bB_PrintingService_Machine = db.BB_PrintingService_Machines.AsNoTracking().ToList();


                    //Vamos buscar o id do serviço de printing ativo
                    BB_PrintingServices bB_PrintingServices = null;
                    //Caso exista mais do que um serviço de printing associado ao negócio, temos que ir buscar o que está ativo
                    // Senão vamos o único que existe
                    if (indexActivePs > 1)
                    {
                        bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).OrderBy(x => x.ID).Skip(index - 1).FirstOrDefault();
                    }
                    else
                    {
                        bB_PrintingServices = db.BB_PrintingServices.Where(x => x.PrintingServices2ID == printingService2ID.ID).FirstOrDefault();
                    }

                    //Com o serviço de printing ativo, vamos buscar as máquinas que estão associadas a esse serviço através do ID da tabela BB_PrintingServices
                    List<BB_PrintingServices_ClickPerModel_VVA> perModel_VVA_lst = db.BB_PrintingServices_ClickPerModel_VVA.Where(x => x.PrintingServiceID == bB_PrintingServices.ID).ToList();

                    foreach (var order in groups)
                    {
                        if (order.TypeOfOrder == 1)
                        {
                            try
                            {
                                var collectionOrderItems = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                                var collectionOrdersContact = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 
                                var collectionOrdersFinance = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE>();
                                var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>();
                                string contractIndexString = index.ToString();
                                string bundelCodeRef = "";
                                bool isUsedMachine = false;
                                string serialNumber = "";

                                string orderDoc = $"O_L{randomNumberOrderString}_{contractIndexString}_{randomLetterNunber}";

                                OrdersPartners sdDocOrderPartner = new OrdersPartners();
                                sdDocOrderPartner.OrderId = order.ID;
                                sdDocOrderPartner.Sd_Doc = orderDoc;
                                sdDocOrdersPartners.Add(sdDocOrderPartner);
                                bool firstItemGroup = true;

                                //Utilizado no Z1ZVOE_ORDERS
                                string contractItm = contractIndexString + "0";
                                List<BB_Proposal_ItemDoBasket> group = itemsBasketGroup.Where(x => x.DeliveryLocationID == order.IDX && x.Group == order.ItemGroup).ToList();

                                int itm_number = 20;
                                bool isMachine = false;
                                foreach (var item in group)
                                {
                                    BB_Equipamentos bB_Equipamentos = bB_EquipamentosLst.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();

                                    BB_Proposal_ItemDoBasket lastItemGroup = group.Last();

                                    if (bB_Equipamentos != null)
                                    {
                                        isMachine = true;

                                        //bundelCodeRef = item.CodeRef;
                                        firstItemGroup = false;

                                        if(item.IsUsedMachine == true)
                                        {
                                            collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                            {
                                                SD_DOC = orderDoc,
                                                ITM_NUMBER = "10", // contractItm,
                                                MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                                REQ_QTY = item.Qty.ToString(),
                                                MODEL_YN = "Y",// Perguntar ao Luis
                                                PLANT = "5400"
                                            });
                                            isUsedMachine = true;
                                            serialNumber = item.SerialNumber;
                                        }
                                        else
                                        {
                                            collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                            {
                                                SD_DOC = orderDoc,
                                                ITM_NUMBER = "10", // contractItm,
                                                MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                                REQ_QTY = item.Qty.ToString(),
                                                MODEL_YN = "Y",// Perguntar ao Luis
                                                PLANT = "5200"
                                            });
                                            bundelCodeRef = item.CodeRef;
                                        }

                                        //itm_number = itm_number + 10;
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
                                                PLANT = item.CodeRef == "9960DX00056" ? "5460" : "5200"
                                                //MODEL_YN = "Y" // Perguntar ao Luis
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
                                                PLANT = item.CodeRef == "9960DX00056" ? "5460": "5200"
                                                //MODEL_YN = "Y" // Perguntar ao Luis
                                            });

                                        }
                                        itm_number = itm_number + 10;

                                    }

                                }

                                //BB_Proposal_DL_ClientContacts dLClient = db.BB_Proposal_DL_ClientContacts.Where(x => x.ID == order.DeliveryContact).FirstOrDefault();

                                

                                BB_PrintingServices_ClickPerModel_VVA psClickPerModelVVA = perModel_VVA_lst.Where(x => x.CodeRef == order.CodeRef).FirstOrDefault();
                                collectionOrderCLickPrices = ClickPrices(d.ID, orderDoc, order.CodeRef, machineCounter, psClickPerModelVVA);

                                perModel_VVA_lst.Remove(psClickPerModelVVA);
                                
                                    collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                    {
                                        SD_DOC = orderDoc,
                                        APLF_NAME = order.ContactName + "" + order.ContactSurname, //"M. LUIS ALVAREZ",
                                        APLF_PHON = order.ContactMovil,       //"66666666",
                                        APLF_OPEN = order.Schedule,//"9h 17h",
                                        APLF_INFO2 = order.Comments//"Asc: Oui -Connexion: PRINTFLEET",

                                    });
                                //}
                                //else
                                //{
                                //    collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                //    {
                                //        SD_DOC = orderDoc,
                                //        APLF_NAME = "M. LUIS ALVAREZ",
                                //        APLF_PHON = "66666666",
                                //        APLF_OPEN = "9h 17h",
                                //        APLF_INFO = "Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                //        APLF_INFO2 = "Asc: Oui -Connexion: PRINTFLEET",
                                //        APLF_INFO3 = "comentario ship to 14733442024402907 ESC COM IDMON ASC"
                                //    });
                                //}
                                //if (financing == "AL")
                                //{
                                var LEAS_ZTERM = "";
                                if (pf.PaymentAfter == 60)
                                {
                                    LEAS_ZTERM = "E60D";
                                }
                                else if (pf.PaymentAfter == 30)
                                {
                                    LEAS_ZTERM = "E30D";

                                }

                                string billToNumber = "";

                                string payerNumber = "";

                                foreach(var dl in list_DeliveryLocations)
                                {
                                    if(dl.BillReceiver == true && dl.Payer == true) {
                                        billToNumber = dl.SAPCustomerNr;
                                        payerNumber = dl.SAPCustomerNr;
                                    }else if(dl.BillReceiver == true && dl.Payer == false)
                                    {
                                        billToNumber = dl.SAPCustomerNr;
                                    }
                                    else
                                    {
                                        payerNumber = dl.SAPCustomerNr;
                                    }
                                }
                                collectionOrdersFinance.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE
                                {
                                    SD_DOC = orderDoc,
                                    FINANCE_TYPE = financing,
                                    LEAS_KUNNR = ct.CompanyCode == null ? payerNumber : ct.CompanyCode,
                                    LEAS_LVTNR = pf.AgreementNumber,
                                    LEAS_LFAKT = "1",
                                    LEAS_ZTERM = LEAS_ZTERM,
                                    LEAS_LEABG = String.Format("{0:yyyyMMdd}", pf.DateApproval),
                                    KBETR1 = "",
                                    KBETR2 = "",
                                    LEAS_LEPER = "",
                                    LEAS_LRYTH = "1",
                                    LEAS_LKAUP = "2.5",
                                    BILL_TO = billToNumber
                                    //PAYER = payerNumber
                                });
                                //}

                                collectionOrderItems = new Collection<WebApplication1.Models.SetupXML.XSD.Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>(collectionOrderItems.OrderBy(x => x.ITM_NUMBER).ToList());

                                DateTime currentDate = DateTime.Now;
                                string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                                if (!isUsedMachine)
                                {
                                    collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                                    {
                                        SD_DOC = orderDoc,
                                        DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                                        REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                                        //REF_1 = order.Key.Name, //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                        PURCH_NO_C = "BB" + DateTime.Today.Year + proposalId.ToString(),  //Nome interno da oferta
                                        SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                                        PMNTTRMS = LEAS_ZTERM, //TODO: manter  || FinancingPaymentMethods.
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
                                }
                                else
                                {
                                    collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                                    {
                                        SD_DOC = orderDoc,
                                        DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                                        REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                                        //REF_1 = order.Key.Name, //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                        PURCH_NO_C = "BB" + DateTime.Today.Year + proposalId.ToString(),  //Nome interno da oferta
                                        SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                                        PMNTTRMS = "303E", //TODO: manter  || FinancingPaymentMethods.
                                        CONTRACT_DOC = contractDoc, //$"C_{c.ID}_1_{randomLetterNunber}",   //contractDoc,
                                        CONTRACT_ITM = contractItm, // add +10 no foreach de orders  
                                        USED_MACHINE = "1",               //"A6DR021",                  //order.CodeRef,
                                        ORDER_FLAG = "O1U", //TODO: MANTER ESTE VALOR;
                                        ORDER_INFO = serialNumber,
                                        LINKING_PIN = proposalId.ToString(),
                                        Z1ZVOE_ORDER_CONTACT = collectionOrdersContact,
                                        Z1ZVOE_ORDER_ITEMS = collectionOrderItems,
                                        Z1ZVOE_CLICK_PRICES = collectionOrderCLickPrices,
                                        Z1ZVOE_FINANCE = collectionOrdersFinance


                                    });
                                }


                                index++;

                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                                return null;
                            }
                        }
                        else
                        {
                            try
                            {
                                var collectionOrderItems = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                                var collectionOrdersContact = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 
                                var collectionOrdersFinance = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_FINANCE>();
                                //var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>();
                                string contractIndexString = index.ToString();
                                string bundelCodeRef = "";

                                string orderDoc = $"O_L{randomNumberOrderString}_{contractIndexString}_{randomLetterNunber}";

                                OrdersPartners sdDocOrderPartner = new OrdersPartners();
                                sdDocOrderPartner.OrderId = order.ID;
                                sdDocOrderPartner.Sd_Doc = orderDoc;
                                sdDocOrdersPartners.Add(sdDocOrderPartner);
                                bool firstItemGroup = true;

                                //Utilizado no Z1ZVOE_ORDERS
                                string contractItm = contractIndexString + "0";
                                List<BB_Proposal_ItemDoBasket> group = itemsBasketGroup.Where(x => x.DeliveryLocationID == order.IDX && x.Group == order.ItemGroup).ToList();

                                int itm_number = 20;
                                bool isMachine = false;
                                foreach (var item in group)
                                {

                                    if (item.Description.Contains("MAIN MATERIAL"))
                                    {
                                        string bomMM = db.BB_Data_Integration.Where(M => M.CodeRef == item.CodeRef).Select(x => x.BOM).FirstOrDefault();
                                        collectionOrderItems.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS
                                        {
                                            SD_DOC = orderDoc,
                                            ITM_NUMBER = "10", // contractItm,
                                            MATERIAL = item.CodeRef, //"A6DR021",//order.CodeRef,
                                            REQ_QTY = item.Qty.ToString(),
                                            MODEL_YN = "Y" // Perguntar ao Luis
                                        });

                                        bundelCodeRef = bomMM;
                                        //itm_number = itm_number + 10;
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
                                            //MODEL_YN = "Y" // Perguntar ao Luis
                                        });

                                        itm_number = itm_number + 10;
                                    }
                                }
                                                                
                                collectionOrdersContact.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                {
                                    SD_DOC = orderDoc,
                                    APLF_NAME = order.ContactName + "" + order.ContactSurname, //"M. LUIS ALVAREZ",
                                    APLF_PHON = order.ContactMovil,       //"66666666",
                                    APLF_OPEN = order.Schedule,//"9h 17h",
                                    APLF_INFO = order.Comments,//"Asc: Oui -Connexion: PRINTFLEET",

                                });
                                
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
                                    LEAS_KUNNR = ct.CompanyCode == "" ? d.ClientAccountNumber : ct.CompanyCode,
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
                                    //PAYER = d.ClientAccountNumber
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
                                    //REF_1 = order.Key.Name, //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                    PURCH_NO_C = "BB" + DateTime.Today.Year + proposalId.ToString(),  //Nome interno da oferta
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
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                                return null;
                            }
                        }
                    }


                    var collectionOrderItemsRetiradas = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_ITEMS>();
                    var collectionOrdersContactRetiradas = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT>(); //Informação igual 

                    List<BB_Proposal_Upturn> bB_Proposal_Upturn = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposalId).ToList();

                    if (bB_Proposal_Upturn != null)
                    {
                        foreach(var upturn in bB_Proposal_Upturn)
                        {
                            //if ((bool)upturn.Retirada)
                            //{
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

                                string contactName = upturn.Name + " " + upturn.Surname;
                                string contactPhone = upturn.Tel.ToString();
                                string contactSchedule = upturn.Schedule;
                                string contactInfo = "Depart: " + upturn.Department + " - Plant: " + upturn.Plant;
                                string contactInfo2 = "";

                            if(upturn.Comments != null && upturn.Comments != "")
                            {
                                contactInfo2 += upturn.Comments + " - ";
                            }
                            if (upturn.Stairs)
                            {
                                contactInfo2 += "ESC ";
                            }
                            if (upturn.DifficultAccess)
                            {
                                contactInfo2 += "COM ";
                            }
                            if (upturn.DNI_LicensePlate)
                            {
                                contactInfo2 += "ID ";
                            }
                            if (upturn.Elevator)
                            {
                                contactInfo2 += "ASC ";
                            }
                            if (upturn.DifficultAccess)
                            {
                                contactInfo2 += "MON";
                            }

                            //string contactInfo3 = "";

                            collectionOrdersContactRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                {
                                    SD_DOC = orderRetiradaDoc,
                                    APRT_NAME = contactName, //"M. LUIS ALVAREZ",
                                    APRT_PHON = contactPhone,       //"66666666",
                                    APRT_OPEN = contactSchedule,//"9h 17h",
                                    APRT_INFO = contactInfo,//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                    APRT_INFO2 = contactInfo2,//"Asc: Oui -Connexion: PRINTFLEET",
                                    APRT_INFO3 = ""

                                });

                                //string[] splitDesciption = upturn.Description.Split(';');
                                //if (upturn.Contact != null)
                                //{
                                //    string[] splitContact = upturn.Contact.Split(';');

                                //    collectionOrdersContactRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                //    {
                                //        SD_DOC = orderRetiradaDoc,
                                //        APLF_NAME = (splitContact != null && !string.IsNullOrEmpty(splitContact[0]) ? splitContact[0] : ""), //"M. LUIS ALVAREZ",
                                //        APLF_PHON = (splitContact != null && !string.IsNullOrEmpty(splitContact[1]) ? splitContact[1] : ""),       //"66666666",
                                //        APLF_OPEN = (splitContact != null && !string.IsNullOrEmpty(splitContact[2]) ? splitContact[2] : ""),//"9h 17h",
                                //        APLF_INFO = "",//"Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                //        APLF_INFO2 = "Marque/Modèle: Konica Minolta / " + (splitDesciption != null && !string.IsNullOrEmpty(splitDesciption[0]) ? splitDesciption[0] : "") + "-N°:" + splitDesciption[1],//"Asc: Oui -Connexion: PRINTFLEET",
                                //        APLF_INFO3 = "2024431493"

                                //    });
                                //}
                                //else
                                //{
                                //    collectionOrdersContactRetiradas.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_ORDER_CONTACT
                                //    {
                                //        SD_DOC = orderRetiradaDoc,
                                //        APLF_NAME = "M. LUIS ALVAREZ",
                                //        APLF_PHON = "66666666",
                                //        APLF_OPEN = "9h 17h",
                                //        APLF_INFO = "Et: 3 -Dept: DEPART -Bat: FENOSA -Salle: A",
                                //        APLF_INFO2 = "Asc: Oui -Connexion: PRINTFLEET",
                                //        APLF_INFO3 = "comentario ship to 14733442024402907 ESC COM IDMON ASC"
                                //    });
                                //}
                                DateTime currentDate = DateTime.Now;
                                string formattedCurrentDate = currentDate.ToString("yyyyMMdd");
                                collectionOrders.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS
                                {
                                    SD_DOC = orderRetiradaDoc,
                                    DOC_TYPE = "ZDO1",      //TODO: Falar com o Luis MAIS TARDE   --- SERVIÇOS = ZD05 ||  MAQUINAS = ZDO1 
                                    REQ_DATE_H = formattedCurrentDate,          //"20240215", //implementar data do pedido a fabrica
                                    REF_1 = "SDR252146", //Nome de referencia da oferta que tem o cliente (o que está escrito na oferta)
                                    PURCH_NO_C = "BB" + DateTime.Today.Year + proposalId.ToString(),  //Nome interno da oferta
                                    SHIP_COND = "50", //TODO: manter || PARA DEPOIS DO GO LIVE -- VER se tem sentido deixar de ser Hardcoded
                                    PMNTTRMS = "E6CD", //TODO: manter  || FinancingPaymentMethods.
                                    MACHINE = "5R", //"A63R021",      /*dataIntegration.CodeRef, *///"A63R021",       //order.CodeRef,   // order.CodeRef,                  //"A6DR021",                  //order.CodeRef,
                                    ORDER_FLAG = "5R", //TODO: MANTER ESTE VALOR;
                                    EQUIPMENT_NO = upturn.Equipment_Number,
                                    DWERK = "5300",
                                    LINKING_PIN = proposalId.ToString(),
                                    Z1ZVOE_ORDER_CONTACT = collectionOrdersContactRetiradas,
                                    Z1ZVOE_ORDER_ITEMS = collectionOrderItemsRetiradas
                                });
                            //}
                        }
                    }


                    OrdersPartnersList sdocOrders = new OrdersPartnersList();
                    sdocOrders.z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERs = collectionOrders;
                    sdocOrders.SdDocOrderPartner = sdDocOrdersPartners;

                    stopwatch.Stop();

                    Console.WriteLine($"Order - ConfigOrders: {stopwatch.ElapsedMilliseconds} ms");

                    return sdocOrders;
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }


        }
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES> ClickPrices(int proposalId, string orderDoc, string codeRef, int machineCounter, BB_PrintingServices_ClickPerModel_VVA psClickPerModelVVA)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                string mATNR = null;
                string kLFN = null;
                string kSTBM = null;
                string kBETR = null;
                var collectionOrderCLickPrices = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES>();

                //ProposalBLL p1 = new ProposalBLL();
                //LoadProposalInfo i = new LoadProposalInfo();
                //i.ProposalId = proposalId;
                //ActionResponse a = p1.LoadProposal(i);

                PrintingServices2 printingServices2 = GetPrintingServices(proposalId);

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

                    

                    if (printingServices2.ActivePrintingService != null)
                    {
                        activePS = printingServices2.ApprovedPrintingServices[printingServices2.ActivePrintingService.Value - 1];

                        if(activePS != null)
                        {

                            if (activePS.BWVolume > 0 && activePS.CVolume > 0)
                            {
                                mATNR = "TOBW";
                                kLFN = "1";
                            
                                if (activePS.GlobalClickNoVolume != null)
                                {
                                    string formatNumber = activePS.GlobalClickNoVolume.GlobalClickBW.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                }
                                else if (activePS.ClickPerModel != null)
                                {
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    if (pSM != null)
                                    {
                                        string formatNumber = pSM.ApprovedBW != null ? ((double)pSM.ApprovedBW).ToString("F5") : ((double)pSM.RequestedBWClickPrice).ToString("F5");
                                        kBETR = formatNumber.Replace(",", ".");
                                    }
                                }
                                else if (activePS.GlobalClickVVA != null)
                                {
                                    string formatNumber = "";
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    //if (pSM != null)
                                    //{
                                    //    if (pSM.ApprovedBW != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.ApprovedBW).ToString("F5");

                                    //    }
                                    //    else if (pSM.RequestedBWClickPrice != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.RequestedBWClickPrice).ToString("F5");

                                    //    }
                                    //    else
                                    //    {
                                    //        formatNumber = "0,00000";
                                    //    }
                                    //}
                                    formatNumber = activePS.GlobalClickVVA.BWExcessPVP.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                    copiasIncludias = pSM.BWVolume;
                                    kSTBM = copiasIncludias.ToString();
                                }else if(activePS.VVAClickPerModel != null)
                                {
                                    string formatNumber = "";
                                    //BB_PrintingServices_ClickPerModel_VVA pSM = db.BB_PrintingServices_ClickPerModel_VVA.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    //if (pSM != null)
                                    //{
                                    //    if (pSM.ApprovedBW != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.ApprovedBW).ToString("F5");

                                    //    }
                                    //    else if (pSM.RequestedBWClickPrice != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.RequestedBWClickPrice).ToString("F5");

                                    //    }
                                    //    else
                                    //    {
                                    //        formatNumber = "0,00000";
                                    //    }
                                    //}
                                    formatNumber = psClickPerModelVVA.BWExcessPVP?.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                    copiasIncludias = Convert.ToInt32(psClickPerModelVVA.BWVolume);
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

                                mATNR = "TOCO";
                                kLFN = "2";
                                if (activePS.GlobalClickNoVolume != null)
                                {
                                    string formatNumber = activePS.GlobalClickNoVolume.GlobalClickC.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                }
                                else if (activePS.ClickPerModel != null)
                                {
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    if (pSM != null)
                                    {
                                        string formatNumber = pSM.ApprovedC != null ? ((double)pSM.ApprovedC).ToString("F5") : ((double)pSM.RequestedCClickPrice).ToString("F5");
                                        kBETR = formatNumber.Replace(",", ".");
                                    }
                                    else
                                    {
                                        kBETR = "0.00000";
                                    }
                                    //kBETR = clickPerModel
                                }
                                else if (activePS.GlobalClickVVA != null)
                                {
                                    string formatNumber = "";
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    //if (pSM != null)
                                    //{
                                    //    if (pSM.ApprovedBW != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.ApprovedC).ToString("F5");

                                    //    }
                                    //    else if (pSM.RequestedBWClickPrice != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.RequestedCClickPrice).ToString("F5");

                                    //    }
                                    //    else
                                    //    {
                                    //        formatNumber = "0,00000";
                                    //    }
                                    //}
                                    formatNumber = activePS.GlobalClickVVA.CExcessPVP.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                    copiasIncludias = pSM.CVolume;
                                    kSTBM = copiasIncludias.ToString();
                                }
                                else if (activePS.VVAClickPerModel != null)
                                {
                                    string formatNumber = "";
                                    //BB_PrintingServices_ClickPerModel_VVA pSM = db.BB_PrintingServices_ClickPerModel_VVA.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    //if (pSM != null)
                                    //{
                                    //    if (pSM.ApprovedBW != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.ApprovedBW).ToString("F5");

                                    //    }
                                    //    else if (pSM.RequestedBWClickPrice != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.RequestedBWClickPrice).ToString("F5");

                                    //    }
                                    //    else
                                    //    {
                                    //        formatNumber = "0,00000";
                                    //    }
                                    //}
                                    formatNumber = psClickPerModelVVA.CExcessPVP?.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                    copiasIncludias = Convert.ToInt32(psClickPerModelVVA.CVolume);
                                    kSTBM = copiasIncludias.ToString();
                                }

                                collectionOrderCLickPrices.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES
                                {
                                    SD_DOC = orderDoc,
                                    MATNR = mATNR, // códigos de cor ou black and white     TOCO -> cor    TOBW-> black and white 
                                    KLFN1 = kLFN, // se for TOBW - 1         se for TOCO->2 
                                    DATAB = FirstDayNextMonthString, //data a partir do momento que é valido  -- primeiro do mês seguinte
                                    DATBI = "99991231", // data de até quando é válido -- deixar default
                                    KSTBM = activePS.GlobalClickVVA != null || activePS.VVAClickPerModel != null ? kSTBM : "0", //copias incluidas 
                                    KBETR = kBETR //preço do excedente 
                                });


                            }
                            if (activePS.BWVolume > 0 && activePS.CVolume == 0)
                            {
                                mATNR = "TOBW";
                                kLFN = "1";
                                if (activePS.GlobalClickNoVolume != null)
                                {
                                    string formatNumber = activePS.GlobalClickNoVolume.GlobalClickBW.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                }
                                else if (activePS.ClickPerModel != null)
                                {
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    if (pSM != null)
                                    {
                                        string formatNumber = ((double)pSM.ApprovedBW).ToString("F5");
                                        kBETR = formatNumber.Replace(",", ".");
                                    }
                                }
                                else if (activePS.GlobalClickVVA != null)
                                {
                                    string formatNumber = "";
                                    BB_PrintingService_Machines pSM = db.BB_PrintingService_Machines.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    //if (pSM != null)
                                    //{
                                    //    if (pSM.ApprovedBW != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.ApprovedBW).ToString("F5");

                                    //    }
                                    //    else if (pSM.RequestedBWClickPrice != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.RequestedBWClickPrice).ToString("F5");

                                    //    }
                                    //    else
                                    //    {
                                    //        formatNumber = "0,00000";
                                    //    }
                                    //}
                                    formatNumber = activePS.GlobalClickVVA.BWExcessPVP.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                    copiasIncludias = pSM.BWVolume;
                                    kSTBM = copiasIncludias.ToString();
                                }
                                else if (activePS.VVAClickPerModel != null)
                                {
                                    string formatNumber = "";
                                    //BB_PrintingServices_ClickPerModel_VVA pSM = db.BB_PrintingServices_ClickPerModel_VVA.Where(x => x.PrintingServiceID == activePS.ID && x.CodeRef == codeRef).FirstOrDefault();
                                    //if (pSM != null)
                                    //{
                                    //    if (pSM.ApprovedBW != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.ApprovedBW).ToString("F5");

                                    //    }
                                    //    else if (pSM.RequestedBWClickPrice != null)
                                    //    {
                                    //        formatNumber = ((double)pSM.RequestedBWClickPrice).ToString("F5");

                                    //    }
                                    //    else
                                    //    {
                                    //        formatNumber = "0,00000";
                                    //    }
                                    //}
                                    formatNumber = psClickPerModelVVA.BWExcessPVP?.ToString("F5");
                                    kBETR = formatNumber.Replace(",", ".");
                                    copiasIncludias = Convert.ToInt32(psClickPerModelVVA.BWVolume);
                                    kSTBM = copiasIncludias.ToString();
                                }


                                collectionOrderCLickPrices.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERSZ1ZVOE_CLICK_PRICES
                                {
                                    SD_DOC = orderDoc,
                                    MATNR = mATNR, // códigos de cor ou black and white     TOCO -> cor    TOBW-> black and white 
                                    KLFN1 = kLFN, // se for TOBW - 1         se for TOCO->2 
                                    DATAB = FirstDayNextMonthString, //data a partir do momento que é valido  -- primeiro do mês seguinte
                                    DATBI = "99991231", // data de até quando é válido -- deixar default
                                    KSTBM = activePS.GlobalClickVVA != null || activePS.VVAClickPerModel != null ? kSTBM : "0", //copias incluidas 
                                    KBETR = kBETR //preço do excedente 
                                });
                            }

                        }

                    }
                    
                                       
                }

                stopwatch.Stop();

                Console.WriteLine($"Order - ConfigOrders: {stopwatch.ElapsedMilliseconds} ms");

                return collectionOrderCLickPrices;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }

        }
      

        public PrintingServices2 GetPrintingServices(int proposalID)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_PrintingServices2 printingServices2 = db.BB_Proposal_PrintingServices2.FirstOrDefault(x => x.ProposalID == proposalID);
                    //BB_Proposal_Condition_Type existPrintingCondition = db.BB_Proposal_Condition_Type.Where(x => x.ProposalID == proposal.ID && x.ConditionType == "ZVBS").FirstOrDefault();
                    PrintingServices2 proposalPS2 = null;

                    if (printingServices2 != null)
                    {
                        proposalPS2 = new PrintingServices2()
                        {
                            ID = printingServices2.ID,
                            ActivePrintingService = printingServices2.ActivePrintingService,
                            ApprovedPrintingServices = new List<ApprovedPrintingService>(),
                            PendingServiceQuoteRequests = new List<ApprovedPrintingService>(),
                            PrintingCondition = 0
                        };

                        //if (existPrintingCondition != null)
                        //{
                        //    proposalPS2.PrintingCondition = existPrintingCondition.ConditionValue;
                        //}
                        foreach (BB_PrintingServices ps in printingServices2.BB_PrintingServices)
                        {
                            ApprovedPrintingService newPS = new ApprovedPrintingService()
                            {
                                BWVolume = ps.BWVolume.Value,
                                ContractDuration = ps.ContractDuration.Value,
                                CVolume = ps.CVolume.Value,
                                ID = ps.ID,
                                Fee = ps.Fee.Value,
                                Machines = new List<Machine>(),
                                IsPrecalc = ps.IsPrecalc.Value,

                            };
                            if (ps.BB_VVA != null)
                            {
                                GlobalClickVVA psVVA = new GlobalClickVVA()
                                {
                                    BWExcessPVP = ps.BB_VVA.BWExcessPVP != null ? ps.BB_VVA.BWExcessPVP.Value : 0,
                                    CExcessPVP = ps.BB_VVA.CExcessPVP != null ? ps.BB_VVA.CExcessPVP.Value : 0,
                                    ExcessBillingFrequency = ps.BB_VVA.ExcessBillingFrequency != null ? ps.BB_VVA.ExcessBillingFrequency.Value : 0,
                                    ReturnType = ps.BB_VVA.ReturnType != null ? ps.BB_VVA.ReturnType.Value : 0,
                                    PVP = ps.BB_VVA.PVP != null ? ps.BB_VVA.PVP.Value : 0,
                                    RentBillingFrequency = ps.BB_VVA.RentBillingFrequency != null ? ps.BB_VVA.RentBillingFrequency.Value : 0,
                                    RequestedRent = ps.BB_VVA.RequestedRent,
                                    RequestedBWExcess = ps.BB_VVA.RequestedBWExcess,
                                    RequestedCExcess = ps.BB_VVA.RequestedCExcess,
                                };
                                newPS.GlobalClickVVA = psVVA;
                            }
                            if (ps.BB_PrintingServices_NoVolume != null)
                            {
                                GlobalClickNoVolume nv = new GlobalClickNoVolume()
                                {
                                    GlobalClickBW = (ps.BB_PrintingServices_NoVolume.GlobalClickBW != null ? ps.BB_PrintingServices_NoVolume.GlobalClickBW.Value : 0),
                                    GlobalClickC = (ps.BB_PrintingServices_NoVolume.GlobalClickC != null ? ps.BB_PrintingServices_NoVolume.GlobalClickC.Value : 0),
                                    PageBillingFrequency = ps.BB_PrintingServices_NoVolume.PageBillingFrequency.Value,
                                    RequestedGlobalClickBW = ps.BB_PrintingServices_NoVolume.RequestedGlobalClickBW,
                                    RequestedGlobalClickC = ps.BB_PrintingServices_NoVolume.RequestedGlobalClickC,
                                };
                                newPS.GlobalClickNoVolume = nv;
                            }
                            if (ps.BB_PrintingServices_ClickPerModel != null)
                            {
                                ClickPerModel cpm = new ClickPerModel()
                                {
                                    PageBillingFrequency = ps.BB_PrintingServices_ClickPerModel.PageBillingFrequency.Value,
                                };
                                newPS.ClickPerModel = cpm;
                            }
                            foreach (BB_PrintingService_Machines machine in ps.BB_PrintingService_Machines)
                            {
                                Machine psMachine = new Machine()
                                {
                                    BWVolume = machine.BWVolume,
                                    CodeRef = machine.CodeRef,
                                    CVolume = machine.CVolume,
                                    Description = machine.Description,
                                    Qty = machine.Quantity.Value,
                                    ID = machine.ID,
                                    RequestedBWClickPrice = machine.RequestedBWClickPrice,
                                    RequestedCClickPrice = machine.RequestedCClickPrice,
                                    ClickPriceBW = machine.ApprovedBW,
                                    ClickPriceC = machine.ApprovedC,
                                    BWPVP = machine.BWPVP,
                                    CPVP = machine.CPVP
                                };
                                newPS.Machines.Add(psMachine);
                            }

                            if(ps.BB_PrintingServices_ClickPerModel_VVA != null)
                            {
                                BB_PrintingServices_ClickPerModel_VVA psVVAcpm = db.BB_PrintingServices_ClickPerModel_VVA.Where(x => x.PrintingServiceID == ps.ID).FirstOrDefault();
                                List<BB_PrintingServices_ClickPerModel_VVA> ps_basket = db.BB_PrintingServices_ClickPerModel_VVA.Where(x => x.PrintingServiceID == ps.ID).ToList();
                                VVAClickPerModel vvaCPM = new VVAClickPerModel()
                                {
                                    PageBillingFrequency = psVVAcpm.PageBillingFrequency != null ? psVVAcpm.PageBillingFrequency.Value : 0,
                                    ExcessBillingFrequency = (int)(psVVAcpm.ExcessBillingFrequency != null ? psVVAcpm.ExcessBillingFrequency.Value : 0),
                                    RequestedRent = 0,
                                    RecommendedRent = 0,
                                    RentBillingFrequency = (int)(psVVAcpm.RentBillingFrequency != null ? psVVAcpm.RentBillingFrequency.Value : 0),
                                    ReturnType = 0,
                                    ps_basket = ps_basket
                                };
                                newPS.VVAClickPerModel = vvaCPM;
                            }
                            BB_Proposal_PrintingServiceValidationRequest validationRequest = ps.BB_Proposal_PrintingServiceValidationRequest.Where(x => x.PrintingServiceID == ps.ID && x.ToDelete == false).FirstOrDefault();
                            if (validationRequest != null)
                            {
                                newPS.RequestedAt = validationRequest.RequestedAt;
                                newPS.SCObservations = validationRequest.SCObservations;
                                newPS.SEObservations = validationRequest.SEObservations;
                                if (validationRequest.IsComplete.Value && validationRequest.IsApproved.Value)
                                {
                                    proposalPS2.ApprovedPrintingServices.Add(newPS);
                                }
                                else
                                {
                                    if (!validationRequest.IsComplete.Value)
                                    {
                                        proposalPS2.PendingServiceQuoteRequests.Add(newPS);
                                    }
                                }
                            }
                            else //commented the else because it was adding reproved services on the approved services list
                            {
                                proposalPS2.ApprovedPrintingServices.Add(newPS);
                            }
                        }

                        proposalPS2.ApprovedPrintingServices = proposalPS2.ApprovedPrintingServices.OrderBy(x => x.IsPrecalc).ToList();
                       
                        

                    }
                    return proposalPS2;
                }

            }
            catch(Exception ex)
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

