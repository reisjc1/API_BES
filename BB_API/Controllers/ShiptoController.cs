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
                    if (!string.IsNullOrEmpty(AccountNumber))
                    {
                        List<BB_LocaisEnvio> lst_Locations = db.BB_LocaisEnvio.ToList();

                        string parentAccountNumber = lst_Locations.Where(x => x.AccountNumber == AccountNumber).FirstOrDefault().ParentAccountNumber;

                        if (!string.IsNullOrEmpty(parentAccountNumber))
                        {
                            lst_Locais = lst_Locations.Where(x => x.ParentAccountNumber == parentAccountNumber).ToList();
                        }


                        switch (selectedTab)
                        {
                            case 2:
                                lst_Locais = lst_Locations.Where(x => x.AddressType == "Entrega/Recogida").ToList();
                                break;
                            case 3:
                                lst_Locais = lst_Locations.Where(x => x.AddressType == "Pagador/Receptor de la factura").ToList();
                                break;
                        }

                        lst_Locais = lst_Locais.ToList();
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
