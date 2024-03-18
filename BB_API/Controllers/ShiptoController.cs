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
        public IHttpActionResult GetAllDeliveryLocations(string NIF, int selectedTab)
        {
            try
            {
                List<BB_LocaisEnvio> lst_Locais = new List<BB_LocaisEnvio>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    IQueryable<BB_LocaisEnvio> locations = db.BB_LocaisEnvio.Where(x => x.NIF_CIF == NIF);

                    switch (selectedTab)
                    {
                        case 2:
                            locations = locations.Where(x => x.AddressType == "Entrega/Recogida");
                            break;
                        case 3:
                            locations = locations.Where(x => x.AddressType == "Pagador/Receptor de la factura");
                            break;
                    }

                    lst_Locais = locations.ToList();
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

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################

    }
}
