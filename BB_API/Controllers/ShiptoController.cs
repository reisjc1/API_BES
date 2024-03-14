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
        public IHttpActionResult GetAllDeliveryLocations(string NIF)
        {
            try
            {
                List<BB_LocaisEnvio> lst_Locais = new List<BB_LocaisEnvio>();


                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_Locais = db.BB_LocaisEnvio.Where(x => x.NIF_CIF == NIF).ToList();
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
