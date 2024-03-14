using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.Models;
using WebApplication1.Models.MIF_Renovations;
using WebApplication1.Models.SLAs;

namespace WebApplication1.Controllers
{
    public class Shipto : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetAllDeliverLocations")]
        public IHttpActionResult GetAllDeliverLocations()
        {
            try
            {
                List<BB_LocaisEnvio> lst_Locais = new List<BB_LocaisEnvio>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_Locais = db.BB_LocaisEnvio.ToList();
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
    }
}
