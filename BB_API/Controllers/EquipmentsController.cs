
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using WebApplication1.Models;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace BB_API.Controllers
{
    public class EquipmentsController : ApiController
    {
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        

        [HttpGet]
        public List<BB_Equipamentos> Get()
        {
            return db.BB_Equipamentos.ToList();
        }

        public BB_Equipamentos Get(int id)
        {
            return db.BB_Equipamentos.FirstOrDefault(e => e.ID == id);
        }
    }
}
