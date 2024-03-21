using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.Models;
using WebApplication1.Models.SLAs;

namespace WebApplication1.Controllers
{
    public class ShiptoController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAllDeliveryLocations")]
        public IHttpActionResult GetAllDeliveryLocations(string AccountNumber, int selectedTab)
        {
            try
            {
                List<BB_LocaisEnvio> lst_Locais = new List<BB_LocaisEnvio>();

                using (var db = new BB_DB_DEVEntities2())
                {

                    lst_Locais = GetDeliveryLocationsFromSP(AccountNumber, null);

                    string parentAccountNr = lst_Locais.Select(x => x.ParentAccountNumber).FirstOrDefault();

                    if (parentAccountNr != "")
                    {
                        lst_Locais = GetDeliveryLocationsFromSP(null, parentAccountNr);
                        switch (selectedTab)
                        {
                            case 2:
                                lst_Locais = lst_Locais.Where(x => x.TypeAccount == "Ship To").ToList();
                        break;
                            case 3:
                                lst_Locais = lst_Locais.Where(x => x.TypeAccount == "Bill To").ToList();
                        break;
                    } }
                    
                    else
                    {
                        switch (selectedTab)
                        {                     
                            case 2:
                                lst_Locais = lst_Locais.Where(x => x.TypeAccount == "Ship To").ToList();
                                break;
                            case 3:
                                lst_Locais = lst_Locais.Where(x => x.TypeAccount == "Bill To").ToList();
                                break;
                        }
                    }
                }

                return Ok(lst_Locais);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("AddNewDeliveryLocation")]
        public IHttpActionResult AddNewDeliveryLocation(BB_LocaisEnvio deliveryLocation)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_LocaisEnvio.Add(deliveryLocation);
                    db.SaveChanges();
                }

                return Ok(deliveryLocation);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("AddNewBBProposalDeliveryLocation")]
        public IHttpActionResult AddNewBBProposalDeliveryLocation(BB_Proposal_DeliveryLocation bb_proposal_deliveryLocation)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_Proposal_DeliveryLocation.Add(bb_proposal_deliveryLocation);
                    db.SaveChanges();
                }

                return Ok(bb_proposal_deliveryLocation);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        // ##################################################################################
        // ##################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("AddNewContact")]
        public IHttpActionResult AddNewContact(BB_Proposal_DL_ClientContacts newContact)
        {
            try
            {
                int contactID = 0;
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_Proposal_DL_ClientContacts.Add(newContact);
                    db.SaveChanges();

                    contactID = newContact.ID;
                }

                return Ok(contactID);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAllSavedContacts")]
        public IHttpActionResult GetAllSavedContacts()
        {
            try
            {
                List <BB_Proposal_DL_ClientContacts> lst_contacts = new List<BB_Proposal_DL_ClientContacts>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_contacts = db.BB_Proposal_DL_ClientContacts.ToList();
                    db.SaveChanges();
                }

                return Ok(lst_contacts);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ----------------------------- HELPERS -----------------------------

        public List<BB_LocaisEnvio> GetDeliveryLocationsFromSP(string AccountNumber, string ParentAccountNumber)
        {
            try
            {
                List<BB_LocaisEnvio> lst_Locais_Saved = new List<BB_LocaisEnvio>();

                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_GetAllDeliveryLocations", conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);
                    cmd.Parameters.AddWithValue("@ParentAccountNumber", ParentAccountNumber);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        BB_LocaisEnvio le = new BB_LocaisEnvio();

                        le.ID = (int)rdr["Id"];
                        le.Documento = rdr["Documento"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Documento")) : "";
                        le.CodRef = rdr["CodRef"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("CodRef")) : "";
                        le.Description = rdr["Description"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Description")) : "";
                        le.AccountNumber = rdr["AccountNumber"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("AccountNumber")) : "";
                        le.NomeCliente = rdr["NomeCliente"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("NomeCliente")) : "";
                        le.Quote = rdr["Quote"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Quote")) : "";
                        le.ShipCode = rdr["ShipCode"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("ShipCode")) : "";
                        le.Adress1 = rdr["Adress1"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Adress1")) : "";
                        le.Adress2 = rdr["Adress2"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Adress2")) : "";
                        le.City = rdr["City"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("City")) : "";
                        le.Contacto = rdr["Contacto"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Contacto")) : "";
                        le.PostalCode = rdr["PostalCode"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("PostalCode")) : "";
                        le.County = rdr["County"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("County")) : "";
                        le.AddressType = rdr["AddressType"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("AddressType")) : "";
                        le.SAPCustomerNr = rdr["SAPCustomerNr"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("SAPCustomerNr")) : "";
                        le.NIF_CIF = rdr["NIF_CIF"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("NIF_CIF")) : "";
                        le.BusinessCode = rdr["BusinessCode"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("BusinessCode")) : "";
                        le.Customer1 = rdr["Customer1"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Customer1")) : "";
                        le.Customer2 = rdr["Customer2"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Customer2")) : "";
                        le.Country = rdr["Country"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Country")) : "";
                        le.RoadType = rdr["RoadType"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("RoadType")) : "";
                        le.RoadName = rdr["RoadName"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("RoadName")) : "";
                        le.RoadNumber = rdr["RoadNumber"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("RoadNumber")) : "";
                        le.ParentAccountNumber = rdr["ParentAccountNumber"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("ParentAccountNumber")) : "";
                        le.TypeAccount = rdr["TypeAccount"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("TypeAccount")) : "";
                        le.AccountID = rdr["AccountID"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("AccountID")) : "";

                        lst_Locais_Saved.Add(le);
                    }
                    rdr.Close();

                }

                return lst_Locais_Saved;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
