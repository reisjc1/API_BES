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

        //[AcceptVerbs("GET", "POST")]
        //[ActionName("Save_DeliveryLocationInfo")]
        //public IHttpActionResult Save_DeliveryLocationInfo(ProposalRootObject p)
        //{
        //    try
        //    {

        //        using (var db = new BB_DB_DEVEntities2())
        //        {
        //            lst_Locais = db.BB_LocaisEnvio.ToList();
        //        }

        //        return Ok(lst_Locais);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound();
        //    }
        //}

        // ##################################################################################
        // ##################################################################################

    }
}
