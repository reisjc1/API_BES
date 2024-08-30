using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML.XML
{
    public class AddressesAdd
    {
        public Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD ConfigAddressAdd(int orderId, string AddNumber)
        {
            Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD addressAdd = new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD();
            PartnerInfo partnerInfo = new PartnerInfo();
            string bdConnect = ConfigurationManager.AppSettings["BasedadosConnect"].ToString();

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
            }
            addressAdd.TAX_NO_1 = partnerInfo.TAX_NO_1;
            addressAdd.TAX_NO_2 = partnerInfo.TAX_NO_2;
            //addressAdd.ADDRNUMBER_2 = AddNumber;
            addressAdd.ADDRNUMBER = AddNumber;
            return addressAdd;


        }
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD> ConfigAddressesAdd(int proposalId, string randomLetterNumber)
        {
            var collectionAdressesAdd = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD>();
            collectionAdressesAdd.Add(new Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD
            {
                TAX_NO_1 = "B11518297",
                TAX_NO_2 = "B11518297",
                ADDRNUMBER_2 = $"A_3686499_{randomLetterNumber}"
            });
            return collectionAdressesAdd;
        }
    }
}