using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.RequestModels;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.ViewModels.Portal_KM.Models.ViewModels.ServiceViewModels;
using WebApplication1.Models.ViewModels.PrintingServicesViewModels;

namespace WebApplication1.Controllers
{
    public class ServiceController : ApiController
    {
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public ServiceBLL serviceBLL = new ServiceBLL();

        [AcceptVerbs("GET", "POST")]
        [ActionName("RequestServiceController")]
        public async Task<HttpResponseMessage> RequestServiceController([FromBody] ServiceRequest sr)
        {
            ActionResponse err = new ActionResponse();
            try
            {
                ProposalBLL pro = new ProposalBLL();
                err = pro.ProposalDraftSave(sr.p);
                int ps2ID = sr.p.Draft.printingServices2.ID.Value;
                ServiceBLL sBLL = new ServiceBLL();
                err = sBLL.RequestServiceController(sr.si, err.ProposalObj);


                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var response = Request.CreateResponse(HttpStatusCode.OK, err);
                response.Content = new StringContent(JsonConvert.SerializeObject(err, settings), Encoding.UTF8, "application/json");
                return response;


                //return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
            }
            catch (Exception ex)
            {
                err.Message = "Falhou Pedido de serviço! Contactar a equipa do BB.";
                return Request.CreateResponse<ActionResponse>(HttpStatusCode.InternalServerError, err);
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetServiceRequestIndexEntries")]
        public List<ServiceValidationRequestIndexEntry> GetServiceRequestIndexEntries()
        {
            return serviceBLL.GetServiceRequestIndexEntries();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetServiceRequestById")]
        public ServiceValidationRequest GetServiceRequestById(int id)
        {
            return serviceBLL.GetServiceRequestById(id);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetServiceRequestHistory")]
        public List<ServiceValidationRequestHistoryEntry> GetServiceRequestHistory()
        {
            return serviceBLL.GetServiceRequestHistory();
        }

        /// <summary>
        /// Defines the endpoint for the GetServiceRequestHistory GET verb.
        /// </summary>
        /// <param name="id">The id of the ServiceRequest to be shown.</param>
        /// <returns>The details of a previously processed ServiceRequest.</returns>
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetServiceRequestHistoryById")]
        public ServiceRequestHistory GetServiceRequestHistoryById(int id)
        {
            return serviceBLL.GetServiceRequestHistoryById(id);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DeleteServiceById")]
        public IHttpActionResult DeleteServiceById(int id)
        {
            serviceBLL.DeleteServiceById(id);
            return Ok();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DeleteServiceQuoteRequest")]
        public List<ApprovedPrintingService> DeleteServiceQuoteRequest(ServiceQuoteDeleteRequest sqdr)
        {
            return serviceBLL.DeleteServiceQuoteRequest(sqdr);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ReevaluateVVARequest")]
        public ReevaluateVVARequest ReevaluateVVARequest(ReevaluateVVARequest rr)
        {
            return serviceBLL.ReevaluateVVARequest(rr);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ReevaluateGlobalClickNoVolumeRequest")]
        public ReevaluateGlobalClickNoVolumeRequest ReevaluateGlobalClickNoVolumeRequest(ReevaluateGlobalClickNoVolumeRequest rr)
        {
            return serviceBLL.ReevaluateGlobalClickNoVolumeRequest(rr);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ReevaluateClickPerModelRequest")]
        public ReevaluateClickPerModelRequest ReevaluateClickPerModelRequest(ReevaluateClickPerModelRequest rr)
        {
            return serviceBLL.ReevaluateClickPerModelRequest(rr);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ProcessVVAServiceValidationReply")]
        public IHttpActionResult ProcessVVAServiceValidationReply(VVAServiceValidationReply svr)
        {
            try
            {
                serviceBLL.ProcessVVAServiceValidationReplyAsync(svr);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ProcessVVAServiceValidationReplyReject")]
        public IHttpActionResult ProcessVVAServiceValidationReplyReject(VVAServiceValidationReply svr)
        {
            try
            {
                serviceBLL.ProcessVVAServiceValidationReplyAsync(svr);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ProcessGlobalClickNoVolumeServiceValidationReply")]
        public IHttpActionResult ProcessGlobalClickNoVolumeServiceValidationReply(GlobalClickNoVolumeServiceValidationReply svr)
        {
            try
            {
                serviceBLL.ProcessGlobalClickNoVolumeServiceValidationReplyAsync(svr);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ProcessClickPerModelServiceValidationReply")]
        public IHttpActionResult ProcessClickPerModelServiceValidationReply(ServiceValidationReply svr)
        {
            try
            {
                serviceBLL.ProcessClickPerModelServiceValidationReplyAsync(svr);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("ProcessClickPerModelVVAServiceValidationReply")]
        public IHttpActionResult ProcessClickPerModelVVAServiceValidationReply(ServiceValidationReply svr)
        {
            try
            {
                serviceBLL.ProcessClickPerModelVVAServiceValidationReplyAsync(svr);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }

    public class ServiceRequest
    {
        public ServiceInputs si { get; set; }
        public ProposalRootObject p { get; set; }
    }
}
