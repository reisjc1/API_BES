using AutoMapper;
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
        public BB_DB_DEVEntities2 dbX = new BB_DB_DEVEntities2();

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAllDeliveryLocations")]
        public IHttpActionResult GetAllDeliveryLocations(int proposalID, string AccountNumber)
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

                    // #############################################################################################

                    dl.AssignedItems = new List<AssignedItems>();

                    foreach (var item in dl.lst_BB_Proposal_DeliveryLocation)
                    {
                        List<BB_Proposal_ItemDoBasket> itemsDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == item.IDX).ToList();

                        foreach (var itemX in itemsDoBasket)
                        {
                            var configDelivery = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<BB_Proposal_ItemDoBasket, AssignedItems>();
                            });

                            IMapper iMapperDelivery = configDelivery.CreateMapper();

                            AssignedItems assignItem = iMapperDelivery.Map<BB_Proposal_ItemDoBasket, AssignedItems>(itemX);

                            assignItem.DeliveryLocationAssociated = item.IDX;

                            dl.AssignedItems.Add(assignItem);
                        }
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
        public IHttpActionResult GetAllSavedContacts(string accountnumber)
        {
            try
            {
                List<BB_Proposal_DL_ClientContacts> lst_contacts = new List<BB_Proposal_DL_ClientContacts>();
                int ClientID = 0;
                using (var db = new BB_DB_DEVEntities2())
                {
                    ClientID = db.BB_Clientes.Where(x => x.accountnumber == accountnumber).Select(x => x.ID).FirstOrDefault();
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

        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveDeliveryLocation")]
        public IHttpActionResult SaveDeliveryLocation(ProposalRootObject p)
        {
            try
            {
                var DLfromDraft = p.Draft.deliveryLocationsBES.deliveryLocationsShipToBillTo;

                List<int> IDX_Included = DLfromDraft.Select(x => x.IDX).ToList();

                if (DLfromDraft.Count() > 0)
                {
                    List<BB_Proposal_DeliveryLocation> dl_lst_toDelete = dbX.BB_Proposal_DeliveryLocation
                        .Where(x => x.ProposalID == p.Draft.details.ID && !IDX_Included.Contains(x.IDX))
                        .ToList();


                    if (dl_lst_toDelete != null)
                    {
                        // "Updating" BB_Proposal_ItemDoBasket

                        List<int> lst_IDX = dl_lst_toDelete.Select(x => x.IDX).ToList();

                        List<BB_Proposal_ItemDoBasket> basketItems_lst_toDelete = new List<BB_Proposal_ItemDoBasket>();

                        foreach (int IDX in lst_IDX)
                        {
                            List<BB_Proposal_ItemDoBasket> IDX_items = dbX.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == IDX).ToList();
                            basketItems_lst_toDelete.AddRange(IDX_items);
                        }

                        if (basketItems_lst_toDelete.Count() > 0)
                        {
                            dbX.BB_Proposal_ItemDoBasket.RemoveRange(basketItems_lst_toDelete);
                            try
                            {
                                dbX.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }

                        // "Updating" BB_Proposal_DeliveryLocation
                        if (dl_lst_toDelete.Count() > 0)
                        {
                            dbX.BB_Proposal_DeliveryLocation.RemoveRange(dl_lst_toDelete);
                            try
                            {
                                dbX.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                ex.Message.ToString();
                            }
                        }
                    }


                    //List<BB_Proposal_DeliveryLocation> dl_lst_toSave = p.Draft.deliveryLocationsBES.deliveryLocationsShipToBillTo
                    //                                                .Select(dl => new BB_Proposal_DeliveryLocation
                    //                                                {
                    //                                                    ProposalID = dl.ProposalID,
                    //                                                    ID = dl.ID,
                    //                                                    Adress1 = dl.Adress1,
                    //                                                    Adress2 = dl.Adress2,
                    //                                                    PostalCode = dl.PostalCode,
                    //                                                    City = dl.City,
                    //                                                    County = dl.County,
                    //                                                    Contacto = dl.Contacto,
                    //                                                    Email = dl.Email,
                    //                                                    Phone = dl.Phone,
                    //                                                    DeliveryDate = dl.DeliveryDate,
                    //                                                    Department = dl.Department,
                    //                                                    Floor = dl.Floor,
                    //                                                    Building = dl.Building,
                    //                                                    Room = dl.Room,
                    //                                                    Schedule = dl.Schedule,
                    //                                                    Payer = dl.Payer,
                    //                                                    BillReceiver = dl.BillReceiver,
                    //                                                    DeliveryContact = dl.DeliveryContact,
                    //                                                    ITContact = dl.ITContact,
                    //                                                    ServiceContact = dl.ServiceContact,
                    //                                                    CopiesContact = dl.CopiesContact,
                    //                                                    DeliveryDelegation = dl.DeliveryDelegation,
                    //                                                    AccountType = dl.AccountType
                    //                                                })
                    //                                                .ToList();

                    //foreach (var DL_toSave in dl_lst_toSave)
                    //{
                    //    // existe algum location com o proposalID e o ID já na BD ?
                    //    var existentDL = dbX.BB_Proposal_DeliveryLocation
                    //                    .Where(x => x.ProposalID == DL_toSave.ProposalID && 
                    //                            x.ID == DL_toSave.ID)
                    //                    .OrderBy(x => x.IDX)
                    //                    .FirstOrDefault();

                    //    DL_toSave.DupPosition = (existentDL == null) ? 1 : existentDL.DupPosition + 1;

                    //    dbX.BB_Proposal_DeliveryLocation.Add(DL_toSave);

                    //    // tenho que gravar por cada interacao, pq senao nao consigo fazer a pesquisa inicial da variavel 'existentDL'
                    //    try
                    //    {
                    //        dbX.SaveChanges();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        ex.Message.ToString();
                    //    }
                    //}

                    //dbX.BB_Proposal_DeliveryLocation.AddRange(dl_lst_toSave);

                        try
                        {
                            dbX.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    
                }




                if (p.Draft.deliveryLocationsBES.AssignedItems.Count() > 0)
                {

                    foreach (var assignItem in p.Draft.deliveryLocationsBES.AssignedItems)
                    {

                        var configpItemns = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<AssignedItems, BB_Proposal_ItemDoBasket>();
                        });

                        IMapper iMapperItems = configpItemns.CreateMapper();

                        //int DL_IDX = 0;

                        foreach (var dl in DLfromDraft)
                        {

                            if (assignItem.DeliveryLocationAssociated == dl.IDX)
                            {
                                // é equipamento
                                //DL_IDX = dbX.BB_Proposal_DeliveryLocation.Where(x => 
                                //            x.ProposalID == p.Draft.details.ID && 
                                //            x.ID == item.ID).Select(x => x.IDX).FirstOrDefault();

                                BB_Proposal_ItemDoBasket bb_Proposal_ItemDoBasket = iMapperItems.Map<AssignedItems, BB_Proposal_ItemDoBasket>(assignItem);
                                //bb_Proposal_ItemDoBasket.DeliveryLocationID = DL_IDX;
                                dbX.BB_Proposal_ItemDoBasket.Add(bb_Proposal_ItemDoBasket);
                                try
                                {
                                    dbX.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString();
                                }
                            }
                        }
                    }
                }
                return Ok(p.Draft.deliveryLocationsBES);
            }
            catch(Exception ex)
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

                        object objBillReceiver = rdr["BillReceiver"];
                        object objPayer = rdr["Payer"];

                        if (objBillReceiver != null)
                        {
                            dl.BillReceiver = ConvertToBoolean(objBillReceiver);
                        }
                        else
                        {
                            dl.BillReceiver = false;
                        }

                        if (objPayer != null)
                        {
                            dl.Payer = ConvertToBoolean(objPayer);
                        }
                        else
                        {
                            dl.Payer = false;
                        }


                        //dl.Payer = rdr["Payer"] != null ? Boolean.Parse(rdr["Payer"].ToString()) : false;
                        //dl.BillReceiver = rdr["BillReceiver"] != null ? Boolean.Parse(rdr["BillReceiver"].ToString()) : false;
                        dl.DeliveryContact = !rdr.IsDBNull(rdr.GetOrdinal("DeliveryContact")) ? (int)rdr["DeliveryContact"] : 1;
                        dl.ITContact = !rdr.IsDBNull(rdr.GetOrdinal("ITContact")) ? (int)rdr["ITContact"] : 1;
                        dl.ServiceContact = !rdr.IsDBNull(rdr.GetOrdinal("ServiceContact")) ? (int)rdr["ServiceContact"] : 1;
                        dl.CopiesContact = !rdr.IsDBNull(rdr.GetOrdinal("CopiesContact")) ? (int)rdr["CopiesContact"] : 1;
                        dl.DeliveryDelegation = !rdr.IsDBNull(rdr.GetOrdinal("DeliveryDelegation")) ? (int)rdr["DeliveryDelegation"] : 1;
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

        // ##################################################################################
        // ##################################################################################


        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveDeliveryLocationIDX")]
        public IHttpActionResult SaveDeliveryLocationIDX(BB_Proposal_DeliveryLocation bb_dl)
        {
            try
            {
                BB_Proposal_DeliveryLocation saved_DL = new BB_Proposal_DeliveryLocation();
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_Proposal_DeliveryLocation.Add(bb_dl);
                    db.SaveChanges();

                    saved_DL = bb_dl;
                }


                return Ok(saved_DL);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // Converter um obj para um bool
        bool ConvertToBoolean(object value)
        {
            if (value != null && value != DBNull.Value)
            {
                string stringValue = value.ToString().ToLower();

                if (stringValue == "true")
                {
                    return true;
                }
                else if (stringValue == "false")
                {
                    return false;
                }
            }

            return false;
        }
    }
    public class DeliveryLocations
    {
        public List<BB_LocaisEnvio> lst_LocaisEnvio { get; set; }

        public List<BB_Proposal_DeliveryLocation> lst_BB_Proposal_DeliveryLocation { get; set; }

        public List<AssignedItems> AssignedItems { get; set; }
    }

    public class AssignedItems
    {
        public double? ClickPriceBW { get; set; }
        public double? ClickPriceC { get; set; }
        public string CodeRef { get; set; }
        public int DeliveryLocationAssociated { get; set; }
        public string Description { get; set; }
        public double DiscountPercentage { get; set; }
        public string Family { get; set; }
        public double GPPercentage { get; set; }
        public double GPTotal { get; set; }
        public int Group { get; set; }
        public bool? IsFinanced { get; set; }
        public bool? IsInClient { get; set; }
        public bool? IsMarginBEU { get; set; }
        public bool? IsUsed { get; set; }
        public bool? Locked { get; set; }
        public double Margin { get; set; }
        public string Name { get; set; }
        public double PVP { get; set; }
        public int Qty { get; set; }
        public double? TCP { get; set; }
        public double TotalCost { get; set; }
        public double TotalNetsale { get; set; }
        public double TotalPVP { get; set; }
        public double UnitDiscountPrice { get; set; }
        public double UnitPriceCost { get; set; }
        public int? addValues { get; set; }
        public List<Counter> counters { get; set; }
        public int? parentIndex { get; set; }
        public PsConfig psConfig { get; set; }

    }
}
