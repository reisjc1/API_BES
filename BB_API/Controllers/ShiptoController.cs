using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebApplication1.Models;

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
                    List<BB_LocaisEnvio> locations = db.BB_LocaisEnvio.Where(x => x.TypeAccount == "Ship To" || x.TypeAccount == "Bill To").ToList();

                    string parentAccountNr = locations.Where(x => x.AccountNumber == AccountNumber).Select(x => x.ParentAccountNumber).FirstOrDefault();

                    if(parentAccountNr != "")
                    {
                        switch (selectedTab)
                        {
                            case 1:
                                lst_Locais = locations.Where(x => x.ParentAccountNumber == parentAccountNr).ToList();
                                break;
                            case 2:
                                lst_Locais = locations.Where(x => x.ParentAccountNumber == parentAccountNr).Where(x => x.TypeAccount == "Ship To").ToList();
                                break;
                            case 3:
                                lst_Locais = locations.Where(x => x.ParentAccountNumber == parentAccountNr).Where(x => x.TypeAccount == "Bill To").ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (selectedTab)
                        {
                            case 1:
                                lst_Locais = locations.Where(x => x.AccountNumber == AccountNumber).ToList();
                                break;
                            case 2:
                                lst_Locais = locations.Where(x => x.AccountNumber == AccountNumber).Where(x => x.TypeAccount == "Ship To").ToList();
                                break;
                            case 3:
                                lst_Locais = locations.Where(x => x.AccountNumber == AccountNumber).Where(x => x.TypeAccount == "Bill To").ToList();
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
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_Proposal_DL_ClientContacts.Add(newContact);
                    db.SaveChanges();
                }

                return Ok("Nuevo contacto añadido exitosamente.");
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
    }
}
