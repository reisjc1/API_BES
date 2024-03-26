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
        public IHttpActionResult GetAllDeliveryLocations(int proposalID, string AccountNumber, int? selectedTab)
        {
            try
            {
                List<BB_LocaisEnvio> lst_Locais = new List<BB_LocaisEnvio>();
                List<BB_Proposal_DeliveryLocation> lst_DeliverLocations = new List<BB_Proposal_DeliveryLocation>();
                DeliveryLocations dl = new DeliveryLocations();

                using (var db = new BB_DB_DEVEntities2())
                {
                    //nunca enviar ambos os parametros != null
                    lst_Locais = GetDeliveryLocationsFromSP(AccountNumber, null);
                    string parentAccountNr = lst_Locais.Select(x => x.ParentAccountNumber).FirstOrDefault();
                    
                    lst_DeliverLocations = GetDeliveryLocationsByProposalIDFromSP(proposalID);

                    dl.lst_LocaisEnvio = new List<BB_LocaisEnvio>();
                    dl.lst_LocaisEnvio = lst_Locais;

                    dl.lst_BB_Proposal_DeliveryLocation = new List<BB_Proposal_DeliveryLocation>();
                    dl.lst_BB_Proposal_DeliveryLocation = lst_DeliverLocations;




                    if (parentAccountNr != "")
                    {
                        //nunca enviar ambos os parametros != null
                        lst_Locais = GetDeliveryLocationsFromSP(null, parentAccountNr);
                        dl.lst_LocaisEnvio = lst_Locais;
                    }
                }


                return Ok(dl);
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
        public IHttpActionResult GetAllSavedContacts(int ClientID)
        {
            try
            {
                List<BB_Proposal_DL_ClientContacts> lst_contacts = new List<BB_Proposal_DL_ClientContacts>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_contacts = db.BB_Proposal_DL_ClientContacts.Where(x => x.ClientID == ClientID).ToList();
                }

                return Ok(lst_contacts);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAllAddressAcronyms")]
        public IHttpActionResult GetAllAddressAcronyms()
        {
            try
            {
                List<RD_AddressAcronyms> lst_AddressAcronyms = new List<RD_AddressAcronyms>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_AddressAcronyms = db.RD_AddressAcronyms.ToList();
                }

                return Ok(lst_AddressAcronyms);
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

        // ##############################################################################################################

        public List<BB_Proposal_DeliveryLocation> GetDeliveryLocationsByProposalIDFromSP(int proposalID)
        {
            try
            {
                List<BB_Proposal_DeliveryLocation> lst_BBP_DL_Saved = new List<BB_Proposal_DeliveryLocation>();

                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_GetAllDeliveryLocationsByProposalID", conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProposalID", proposalID);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        BB_Proposal_DeliveryLocation dl = new BB_Proposal_DeliveryLocation();

                        dl.IDX = (int)rdr["IDX"];
                        dl.ProposalID = (int)rdr["ProposalID"];
                        dl.ID = rdr["ID"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("ID")) : "";
                        dl.Adress1 = rdr["Adress1"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Adress1")) : "";
                        dl.Adress2 = rdr["Adress2"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Adress2")) : "";
                        dl.PostalCode = rdr["PostalCode"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("PostalCode")) : "";
                        dl.City = rdr["City"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("City")) : "";
                        dl.County = rdr["County"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("County")) : "";
                        dl.Contacto = rdr["Contacto"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Contacto")) : "";
                        dl.Email = rdr["Email"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Email")) : "";
                        dl.Phone = rdr["Phone"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Phone")) : "";
                        dl.DeliveryDate = rdr["DeliveryDate"] != DBNull.Value ? (DateTime?)rdr["DeliveryDate"] : null;
                        dl.Department = rdr["Department"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Department")) : "";
                        dl.Floor = rdr["Floor"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Floor")) : "";
                        dl.Building = rdr["Building"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Building")) : "";
                        dl.Room = rdr["Room"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Room")) : "";
                        dl.Schedule = rdr["Schedule"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Schedule")) : "";
                        dl.Payer = rdr["Payer"] != null ? Boolean.Parse(rdr["Payer"].ToString()) : false;
                        dl.BillReceiver = rdr["BillReceiver"] != null ? Boolean.Parse(rdr["BillReceiver"].ToString()) : false;
                        dl.DeliveryContact = (int)rdr["DeliveryContact"];
                        dl.ITContact = (int)rdr["ITContact"];
                        dl.ServiceContact = (int)rdr["ServiceContact"];
                        dl.CopiesContact = (int)rdr["CopiesContact"];
                        dl.DeliveryDelegation = (int)rdr["DeliveryDelegation"];
                        dl.EquipmentID = (int)rdr["EquipmentID"];
                        dl.AccessoryID = (int)rdr["AccessoryID"];
                        dl.AccountType = rdr["AccountType"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("AccountType")) : "";

                        lst_BBP_DL_Saved.Add(dl);
                    }
                    rdr.Close();

                }

                return lst_BBP_DL_Saved;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class DeliveryLocations
    {
        public List<BB_LocaisEnvio> lst_LocaisEnvio { get; set; }

        public List<BB_Proposal_DeliveryLocation> lst_BB_Proposal_DeliveryLocation { get; set; }
    }
}
