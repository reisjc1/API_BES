using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;
using WebApplication1.Models.SetupXML;

namespace WebApplication1.Models.SetupXML.XML
{
    public class Addresses
    {
        public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES ConfigAddress(int orderId, string randomLetterNumber, int randomNumber)
        {
            Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES address = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES();
            string bdConnect = ConfigurationManager.AppSettings["BasedadosConnect"].ToString();
            PartnerInfo partnerInfo = new PartnerInfo();
            using (SqlConnection conn = new SqlConnection(bdConnect))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SP_GET_ORDER_PARTNER_INFO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@OrderID", orderId);
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
                address.ADDRNUMBER = $"A_{randomNumber}_{randomLetterNumber}";             //$"A_368650_{randomLetterNumber}";      //$"A_3686499_{randomLetterNunber}",
                address.NAME1 = "EUROPEA DE EXPEDICIONES SL"; // partnerInfo.NAME1,
                address.NAME2 = partnerInfo.NAME2;//"COMPLEMENTO 1",               //NOTA: Ir buscar o dado a base de dados de ESPANHA -- Falar com João reis  (Para Antonio e Tiago)
                address.NAME_CO = "DEPART";
                address.CITY1 = partnerInfo.CITY1;// "CADIZ",
                address.POST_CODE1 = partnerInfo.POST_CODE1;//"11006",
                address.STREET = partnerInfo.STREET;//"AVENIDA DEL PUERTO 2  3º ED FEN",
                address.FLOOR = partnerInfo.FLOOR;//"3",
                address.ROOMNUMBER = partnerInfo.ROOMNUMBER;//"A",
                address.COUNTRY = partnerInfo.COUNTRY;//"ES",
                address.LANGU = partnerInfo.LANGU;//"E",
                address.REGION = partnerInfo.REGION;//"11",
                address.TEL_NUMBER = partnerInfo.TEL_NUMBER;//"66666666", //int no DB
                address.BUILD_LONG = partnerInfo.BUILD_LONG;//"FENOSA"
                return address;
            }


        }
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES> ConfigAddresses(int proposalId, List<OrdersPartners> orders, string randomLetterNumber)
        {
            try
            {
                var collectionAdresses = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES>();
                PartnerInfo partnerInfo = new PartnerInfo();

                string bdConnect = ConfigurationManager.AppSettings["BasedadosConnect"].ToString();
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
                    collectionAdresses.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES
                    {
                        ADDRNUMBER = $"A_368650_{randomLetterNumber}",      //$"A_3686499_{randomLetterNunber}",
                        NAME1 = "EUROPEA DE EXPEDICIONES SL", // partnerInfo.NAME1,
                        NAME2 = partnerInfo.NAME2, //"COMPLEMENTO 1",               //NOTA: Ir buscar o dado a base de dados de ESPANHA -- Falar com João reis  (Para Antonio e Tiago)
                        NAME_CO = "DEPART",
                        CITY1 = partnerInfo.CITY1,// "CADIZ",
                        POST_CODE1 = partnerInfo.POST_CODE1,//"11006",
                        STREET = partnerInfo.STREET,//"AVENIDA DEL PUERTO 2  3º ED FEN",
                        FLOOR = partnerInfo.FLOOR,//"3",
                        ROOMNUMBER = partnerInfo.ROOMNUMBER,//"A",
                        COUNTRY = partnerInfo.COUNTRY,//"ES",
                        LANGU = partnerInfo.LANGU,//"E",
                        REGION = partnerInfo.REGION,//"11",
                        TEL_NUMBER = partnerInfo.TEL_NUMBER,//"66666666", //int no DB
                        BUILD_LONG = partnerInfo.BUILD_LONG//"FENOSA"
                    });
                }
                //using (var db = new BB_DB_DEVEntities())
                //{
                //    List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalId).ToList();
                //    List<BB_Proposal_Quote_RS> quotesRs = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalId).ToList();
                //    List<BB_Proposal_DeliveryLocation> dl = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == proposalId).ToList();
                //    foreach (var quote in quotes)
                //    {
                //        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == quote.CodeRef).ToList();
                //        foreach (var order in itemsDoBasket)
                //        {
                //            BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == order.CodeRef).FirstOrDefault();
                //            if (bB_Equipamentos != null)
                //            {
                //                using (SqlConnection conn = new SqlConnection(bdConnect))
                //                {
                //                    conn.Open();

                //                    SqlCommand cmd = new SqlCommand("SP_GET_ORDER_PARTNER_INFO", conn);
                //                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //                    cmd.Parameters.AddWithValue("@OrderID", order.ID);
                //                    using (SqlDataReader reader = cmd.ExecuteReader())
                //                    {
                //                        if (reader.Read())
                //                        {
                //                            partnerInfo.PARTN_ROLE = reader["PARTN_ROLE"].ToString();
                //                            partnerInfo.CUSTOMER = reader["CUSTOMER"].ToString();
                //                            partnerInfo.CP_NAME = reader["CP_NAME"].ToString();
                //                            partnerInfo.CP_PHONE = reader["CP_PHONE"].ToString();
                //                            partnerInfo.NAME1 = reader["NAME1"].ToString();
                //                            partnerInfo.NAME2 = reader["NAME2"].ToString();
                //                            partnerInfo.NAME_CO = reader["NAME_CO"].ToString();
                //                            partnerInfo.CITY1 = reader["CITY1"].ToString();
                //                            partnerInfo.POST_CODE1 = reader["POST_CODE1"].ToString();
                //                            partnerInfo.STREET = reader["STREET"].ToString();
                //                            partnerInfo.FLOOR = reader["FLOOR"].ToString();
                //                            partnerInfo.ROOMNUMBER = reader["ROOMNUMBER"].ToString();
                //                            partnerInfo.COUNTRY = reader["COUNTRY"].ToString();
                //                            partnerInfo.LANGU = reader["LANGU"].ToString();
                //                            partnerInfo.REGION = reader["REGION"].ToString();
                //                            partnerInfo.TEL_NUMBER = reader["TEL_NUMBER"].ToString();
                //                            partnerInfo.BUILD_LONG = reader["BUILD_LONG"].ToString();
                //                            partnerInfo.TAX_NO_1 = reader["TAX_NO_1"].ToString();
                //                            partnerInfo.TAX_NO_2 = reader["TAX_NO_2"].ToString();
                //                        }
                //                    }
                //                }
                //                collectionAdresses.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES
                //                {
                //                    ADDRNUMBER = $"A_3686501_{randomLetterNumber}",      //$"A_3686499_{randomLetterNunber}",
                //                    NAME1 = partnerInfo.NAME1,//"EUROPEA DE EXPEDICIONES SL",
                //                    NAME2 = partnerInfo.NAME2, //"COMPLEMENTO 1",               //NOTA: Ir buscar o dado a base de dados de ESPANHA -- Falar com João reis  (Para Antonio e Tiago)
                //                    NAME_CO = "DEPART",
                //                    CITY1 = partnerInfo.CITY1,// "CADIZ",
                //                    POST_CODE1 = partnerInfo.POST_CODE1,//"11006",
                //                    STREET = partnerInfo.STREET,//"AVENIDA DEL PUERTO 2  3º ED FEN",
                //                    FLOOR = partnerInfo.FLOOR,//"3",
                //                    ROOMNUMBER = partnerInfo.ROOMNUMBER,//"A",
                //                    COUNTRY = partnerInfo.COUNTRY,//"ES",
                //                    LANGU = partnerInfo.LANGU,//"E",
                //                    REGION = partnerInfo.REGION,//"11",
                //                    TEL_NUMBER = partnerInfo.TEL_NUMBER,//"66666666", //int no DB
                //                    BUILD_LONG = partnerInfo.BUILD_LONG//"FENOSA"
                //                });
                //            }

                //        }

                //    }
                //    foreach (var quoteRs in quotesRs)
                //    {
                //        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.CodeRef == quoteRs.CodeRef).ToList();
                //        foreach (var order in itemsDoBasket)
                //        {
                //            BB_Equipamentos bB_Equipamentos = db.BB_Equipamentos.Where(x => x.CodeRef == order.CodeRef).FirstOrDefault();
                //            if (bB_Equipamentos != null)
                //            {
                //                using (SqlConnection conn = new SqlConnection(bdConnect))
                //                {
                //                    conn.Open();

                //                    SqlCommand cmd = new SqlCommand("SP_GET_ORDER_PARTNER_INFO", conn);
                //                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //                    cmd.Parameters.AddWithValue("@OrderID", order.ID);
                //                    using (SqlDataReader reader = cmd.ExecuteReader())
                //                    {
                //                        if (reader.Read())
                //                        {
                //                            partnerInfo.PARTN_ROLE = reader["PARTN_ROLE"].ToString();
                //                            partnerInfo.CUSTOMER = reader["CUSTOMER"].ToString();
                //                            partnerInfo.CP_NAME = reader["CP_NAME"].ToString();
                //                            partnerInfo.CP_PHONE = reader["CP_PHONE"].ToString();
                //                            partnerInfo.NAME1 = reader["NAME1"].ToString();
                //                            partnerInfo.NAME2 = reader["NAME2"].ToString();
                //                            partnerInfo.NAME_CO = reader["NAME_CO"].ToString();
                //                            partnerInfo.CITY1 = reader["CITY1"].ToString();
                //                            partnerInfo.POST_CODE1 = reader["POST_CODE1"].ToString();
                //                            partnerInfo.STREET = reader["STREET"].ToString();
                //                            partnerInfo.FLOOR = reader["FLOOR"].ToString();
                //                            partnerInfo.ROOMNUMBER = reader["ROOMNUMBER"].ToString();
                //                            partnerInfo.COUNTRY = reader["COUNTRY"].ToString();
                //                            partnerInfo.LANGU = reader["LANGU"].ToString();
                //                            partnerInfo.REGION = reader["REGION"].ToString();
                //                            partnerInfo.TEL_NUMBER = reader["TEL_NUMBER"].ToString();
                //                            partnerInfo.BUILD_LONG = reader["BUILD_LONG"].ToString();
                //                            partnerInfo.TAX_NO_1 = reader["TAX_NO_1"].ToString();
                //                            partnerInfo.TAX_NO_2 = reader["TAX_NO_2"].ToString();
                //                        }
                //                    }
                //                }
                //                collectionAdresses.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES
                //                {
                //                    ADDRNUMBER = $"A_3686501_{randomLetterNumber}",      //$"A_3686499_{randomLetterNunber}",
                //                    NAME1 = partnerInfo.NAME1,//"EUROPEA DE EXPEDICIONES SL",
                //                    NAME2 = partnerInfo.NAME2, //"COMPLEMENTO 1",               //NOTA: Ir buscar o dado a base de dados de ESPANHA -- Falar com João reis  (Para Antonio e Tiago)
                //                    NAME_CO = "DEPART",
                //                    CITY1 = partnerInfo.CITY1,// "CADIZ",
                //                    POST_CODE1 = partnerInfo.POST_CODE1,//"11006",
                //                    STREET = partnerInfo.STREET,//"AVENIDA DEL PUERTO 2  3º ED FEN",
                //                    FLOOR = partnerInfo.FLOOR,//"3",
                //                    ROOMNUMBER = partnerInfo.ROOMNUMBER,//"A",
                //                    COUNTRY = partnerInfo.COUNTRY,//"ES",
                //                    LANGU = partnerInfo.LANGU,//"E",
                //                    REGION = partnerInfo.REGION,//"11",
                //                    TEL_NUMBER = partnerInfo.TEL_NUMBER,//"66666666", //int no DB
                //                    BUILD_LONG = partnerInfo.BUILD_LONG//"FENOSA"
                //                });
                //            }

                //        }

                //    }
                //    return collectionAdresses;
                //}
                return collectionAdresses;
            }
            catch (Exception ex)
            {

                ex.Message.ToString();
                return null;
            }


        }
    }
}