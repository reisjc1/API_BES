using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication1.Models.SetupXML.XML;

namespace WebApplication1.Controllers
{
    public class ProposalXmlController : ApiController
    {
        // GET: ProposalXml
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.ActionName("ProposalXml")]
        public IHttpActionResult ProposalXml(int contractId)
        {
            Deal deal = new Deal();
            deal.DealXML(contractId);

            return Ok();
        }
    }
}