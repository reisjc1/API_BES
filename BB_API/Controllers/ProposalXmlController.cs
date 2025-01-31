using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.SetupXML.XML;

namespace WebApplication1.Controllers
{
    public class ProposalXmlController : ApiController
    {
        // GET: ProposalXml
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.ActionName("GetProposalId")]
        public IHttpActionResult GetProposalId(int contractId, string name)
        {
            try
            {
                bool statusMessage = true;
                using (var db = new BB_DB_DEVEntities2())
                {
                    LD_Contrato lD_Contrato = db.LD_Contrato.Where(x => x.ID == contractId).FirstOrDefault();

                    List<BB_Proposal_DeliveryLocation> locaisEnvioIds = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == lD_Contrato.ProposalID).ToList();

                    bool isMissingSAPNumber = false;

                    foreach(var localEnvio in locaisEnvioIds)
                    {
                        //BB_LocaisEnvio le = db.BB_LocaisEnvio.Where(x => x.ID.ToString() == localEnvio).FirstOrDefault();
               
                        if(localEnvio != null && localEnvio.SAPCustomerNr == null)
                        {
                            isMissingSAPNumber = true;
                            break;
                        }
                    }

                    Deal deal = new Deal();
                    if (!isMissingSAPNumber)
                    {
                        deal.DealXML(contractId, true);

                        lD_Contrato.StatusID = 9;
                        lD_Contrato.ModifiedBy = name;

                        db.Entry(lD_Contrato).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        statusMessage = false;
                    }

                }

                return Ok(statusMessage);
            }catch(Exception ex)
            {
                return Ok(ex);
            }


        }


        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        //[System.Web.Http.ActionName("GetProposalId")]
        //public IHttpActionResult UpdateProposalByID(int contractId, bool result, string name)
        //{

        //    using (var db = new BB_DB_DEVEntities2())
        //    {
        //        LD_Contrato lD_Contrato = db.LD_Contrato.Where(x => x.ID == contractId).FirstOrDefault();

        //        if (result != null)
        //        {
        //            lD_Contrato.StatusID = 9;
        //            lD_Contrato.ModifiedBy = name;

        //            db.Entry(lD_Contrato).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }

        //    }

        //    return Ok();
        //}
    }
}