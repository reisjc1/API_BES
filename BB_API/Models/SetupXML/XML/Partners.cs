using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML.XML
{
    public class Partners
    {

        public PartnersAdressesList ConfigPartners(int proposalId, List<OrdersPartners> orders, string randomLetterNumber)
        {
            try
            {
                Addresses address = new Addresses();
                AddressesAdd addresseAdd = new AddressesAdd();

                PartnersAdressesList partnersAdressesList = new PartnersAdressesList();

                var collectionPartners = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS>();
                var collectionAddresses = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES>();
                var collectionAddressesAdd = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD>();

                PartnerInfo partnerInfo = new PartnerInfo();
                string bdConnect = ConfigurationManager.AppSettings["BasedadosConnect"].ToString();
                int i = 0;
                Random random = new Random();
                int randomNumberAddress = random.Next(1000000, 10000000);
                foreach (var order in orders)
                {
                    using (SqlConnection conn = new SqlConnection(bdConnect))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_GET_ORDER_PARTNER_INFO", conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@OrderID", order.OrderId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                partnerInfo.PARTN_ROLE = reader["PARTN_ROLE"].ToString();
                                partnerInfo.CUSTOMER = reader["CUSTOMER"].ToString();
                                partnerInfo.CP_NAME = reader["CP_NAME"].ToString();
                                partnerInfo.CP_PHONE = reader["CP_PHONE"].ToString();
                                partnerInfo.NAME1 = reader["NAME1"].ToString();
                                partnerInfo.NAME2 = reader["NAME2"].ToString();
                                partnerInfo.NAME_CO = reader["NAME_CO"].ToString();
                                partnerInfo.CITY1 = reader["CITY1"].ToString();
                                partnerInfo.POST_CODE1 = reader["POST_CODE1"].ToString();
                                partnerInfo.STREET = reader["STREET"].ToString();
                                partnerInfo.FLOOR = reader["FLOOR"].ToString();
                                partnerInfo.ROOMNUMBER = reader["ROOMNUMBER"].ToString();
                                partnerInfo.COUNTRY = reader["COUNTRY"].ToString();
                                partnerInfo.LANGU = reader["LANGU"].ToString();
                                partnerInfo.REGION = reader["REGION"].ToString();
                                partnerInfo.TEL_NUMBER = reader["TEL_NUMBER"].ToString();
                                partnerInfo.BUILD_LONG = reader["BUILD_LONG"].ToString();
                                partnerInfo.TAX_NO_1 = reader["TAX_NO_1"].ToString();
                                partnerInfo.TAX_NO_2 = reader["TAX_NO_2"].ToString();
                            }
                        }
                    }
                    //string cp_Name1 = 
                    //SD_DocOrdersPartner SDocPartner = orders.Where(x => x.OrderId == order.ID).FirstOrDefault();
                    if (partnerInfo.PARTN_ROLE != null)
                    {
                        switch (partnerInfo.PARTN_ROLE)
                        {
                            case "Ship To":
                                partnerInfo.PARTN_ROLE = "WE";
                                break;
                            case "Bill To":
                                partnerInfo.PARTN_ROLE = "RE";
                                break;
                            case "Sold To":
                                partnerInfo.PARTN_ROLE = "AG";
                                break;



                        }

                    }
                    string[] nameParts = partnerInfo.CP_NAME.Split(' ');
                    partnerInfo.CUSTOMER = "1132257";//"1161897"; //null;//
                    if (!string.IsNullOrEmpty(partnerInfo.CUSTOMER))
                    {

                        collectionPartners.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS
                        {
                            SD_DOC = order.Sd_Doc,  //"Teste",//order.SD_DOC,
                            PARTN_ROLE = partnerInfo.PARTN_ROLE, //"WE",
                            CUSTOMER = partnerInfo.CUSTOMER,
                            CP_NAMEV = nameParts[nameParts.Length - 1],     //"ALVAREZ",
                            CP_NAME1 = string.Join(" ", nameParts.Take(nameParts.Length - 1)),
                            CP_PHONE = partnerInfo.CP_PHONE//"66666666"
                        });


                    }
                    else
                    {

                        Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES addressObj = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES();
                        Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD addressAddObj = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD();

                        int randomNumberI = randomNumberAddress + i;

                        addressObj = address.ConfigAddress(order.OrderId, randomLetterNumber, randomNumberI);
                        addressAddObj = addresseAdd.ConfigAddressAdd(order.OrderId, addressObj.ADDRNUMBER);

                        Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS partner = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS();
                        partner.SD_DOC = order.Sd_Doc;
                        partner.PARTN_ROLE = partnerInfo.PARTN_ROLE;
                        partner.ADDRNUMBER = addressObj.ADDRNUMBER;
                        partner.CP_NAMEV = nameParts[nameParts.Length - 1];
                        partner.CP_NAME1 = string.Join(" ", nameParts.Take(nameParts.Length - 1));
                        partner.CP_PHONE = partnerInfo.CP_PHONE;

                        //partnersAdressesList.Partners.Add(partner);
                        collectionPartners.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS
                        {
                            SD_DOC = order.Sd_Doc,  //"Teste",//order.SD_DOC,
                            PARTN_ROLE = partnerInfo.PARTN_ROLE, //"WE",
                            ADDRNUMBER = addressObj.ADDRNUMBER,      //$"A_3686501_{randomLetterNumber}",  //$"A_3686499_{randomLetterNunber}",
                            CP_NAMEV = nameParts[nameParts.Length - 1],     //"ALVAREZ",
                            CP_NAME1 = string.Join(" ", nameParts.Take(nameParts.Length - 1)),
                            CP_PHONE = partnerInfo.CP_PHONE//"66666666"
                        });
                        collectionAddresses.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES
                        {
                            ADDRNUMBER = addressObj.ADDRNUMBER,     //$"A_3686499_{randomLetterNunber}",
                            NAME1 = "EUROPEA DE EXPEDICIONES SL", // partnerInfo.NAME1,
                            NAME2 = addressObj.NAME2, //"COMPLEMENTO 1",               //NOTA: Ir buscar o dado a base de dados de ESPANHA -- Falar com João reis  (Para Antonio e Tiago)
                            NAME_CO = "DEPART",
                            CITY1 = addressObj.CITY1,// "CADIZ",
                            POST_CODE1 = addressObj.POST_CODE1,//"11006",
                            STREET = addressObj.STREET,//"AVENIDA DEL PUERTO 2  3º ED FEN",
                            FLOOR = addressObj.FLOOR,//"3",
                            ROOMNUMBER = addressObj.ROOMNUMBER,//"A",
                            COUNTRY = addressObj.COUNTRY,//"ES",
                            LANGU = addressObj.LANGU,//"E",
                            REGION = addressObj.REGION,//"11",
                            TEL_NUMBER = addressObj.TEL_NUMBER,//"66666666", //int no DB
                            BUILD_LONG = addressObj.BUILD_LONG//"FENOSA"
                        });
                        collectionAddressesAdd.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD
                        {
                            TAX_NO_1 = addressAddObj.TAX_NO_1,
                            TAX_NO_2 = addressAddObj.TAX_NO_2,
                            ADDRNUMBER_2 = addressAddObj.ADDRNUMBER_2
                        });
                        i++;
                    }


                    using (var db = new BB_DB_DEVEntities2())
                    {
                        List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).Where(x => x.AccountType == "Bill To").ToList();

                        if (dl.Count > 0)
                        {
                            foreach (var dLocation in dl)
                            {
                                BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == dLocation.ProposalID).FirstOrDefault();
                                int fkLocaisEvnio = int.Parse(dLocation.ID);
                                BB_LocaisEnvio lEnvio = db.BB_LocaisEnvio.Where(x => x.ID == fkLocaisEvnio).FirstOrDefault();
                                BB_Clientes c = db.BB_Clientes.Where(x => x.accountnumber == proposal.ClientAccountNumber).FirstOrDefault();
                                string[] namePartsBT = c.Owner.Split(' ');

                                if (dLocation.Payer == false)
                                {
                                    if (lEnvio.SAPCustomerNr != null)
                                    {
                                        collectionPartners.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS
                                        {
                                            SD_DOC = order.Sd_Doc,  //"Teste",//order.SD_DOC,
                                            PARTN_ROLE = "RE", //"WE",
                                            CUSTOMER = "1137222",       //lEnvio.SAPCustomerNr.ToString(),             // partnerInfo.CUSTOMER, //
                                            CP_NAMEV = namePartsBT[namePartsBT.Length - 1],     //"ALVAREZ",
                                            CP_NAME1 = string.Join(" ", namePartsBT.Take(namePartsBT.Length - 1)),
                                            CP_PHONE = c.telephone1//"66666666"
                                        });
                                    }
                                }

                                if (dLocation.Payer == true)
                                {

                                    if (lEnvio.SAPCustomerNr != null)
                                    {
                                        collectionPartners.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS
                                        {
                                            SD_DOC = order.Sd_Doc,  //"Teste",//order.SD_DOC,
                                            PARTN_ROLE = "RG", //"WE",
                                            CUSTOMER = "1132367",       //lEnvio.SAPCustomerNr.ToString(),             // partnerInfo.CUSTOMER, //
                                            CP_NAMEV = namePartsBT[namePartsBT.Length - 1],     //"ALVAREZ",
                                            CP_NAME1 = string.Join(" ", namePartsBT.Take(namePartsBT.Length - 1)),
                                            CP_PHONE = c.telephone1//"66666666"
                                        });
                                    }
                                }
                            }
                        }

                    }

                }

                partnersAdressesList.Partners = collectionPartners;
                partnersAdressesList.Adresses = collectionAddresses;
                partnersAdressesList.AdressesAdd = collectionAddressesAdd;

                return partnersAdressesList;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }





        }
    }
}