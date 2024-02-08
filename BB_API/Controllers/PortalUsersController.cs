using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.BLL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PortalUsersController : ApiController
    {
        public BB_DB_DEVEntities2 diDB = new BB_DB_DEVEntities2();
        public masterEntities usersDB = new masterEntities();

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetUsers")]
        public List<AspNetUsers> GetUsers([FromBody]Owner_ Owner)
        {
            return usersDB.AspNetUsers.Where(x =>
            (x.Function == "Bid Management" ||
            x.Function == "BPO Business Developer" ||
            x.Function == "IMS Business Developer" ||
            x.Function == "MCS Business Developer" ||
            x.Function == "Sales Executive" ||
            x.Function == "OPD Lisboa Sales Manager" ||
            x.Function == "OPD Porto Sales Manager") && 
            x.Email != Owner.Owner &&
            x.Country == "BES").ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetSalesExecutive")]
        public List<AspNetUsers> GetSalesExecutive()
        {
            return usersDB.AspNetUsers.Where(x => x.Function == "Sales Executive" && x.Country == "BES").ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("CoworkerAssign")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> CoworkerAssignAsync(UserAssign a)
        {
            BB_Proposal proposal = null;
            ActionResponse err = new ActionResponse();
            try
            {
                ProposalRootObject p = new ProposalRootObject();
                p.Draft = a.Draft;
                p.Summary = a.Summary;

                //Accao de SAVE
                ProposalBLL pBLL = new ProposalBLL();
                ActionResponse ac = pBLL.ProposalDraftSave(p);
                AspNetUsers user = usersDB.AspNetUsers.Where(x => x.Email == a.coworkerEmail && x.Country == "BES").FirstOrDefault();

                proposal = diDB.BB_Proposal.Where(x => x.ID == ac.ProposalObj.Draft.details.ID).FirstOrDefault();
                //Default colaboracao
                proposal.StatusID = 6;
                proposal.ModifiedBy = user.Email;
                proposal.ModifiedTime = DateTime.Now;

                BB_Proposal_Observations obs1 = new BB_Proposal_Observations();
                obs1.CreatedBy = a.createdBy;
                obs1.Observation = a.observation;
                obs1.Created = DateTime.Now;
                obs1.ProposalID = ac.ProposalObj.Draft.details.ID;

                diDB.BB_Proposal_Observations.Add(obs1);


                //Cotacao Atribuida
                if (a.coworkerEmail == proposal.CreatedBy)
                    proposal.StatusID = 8;

                diDB.Entry(proposal).State = EntityState.Modified;
                diDB.SaveChanges();

                err.Message = "Propuesta Asignada";
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            //err.ErrorCode = 0; 

            //EMAIL SEND
            EmailService emailSend = new EmailService();
            EmailMesage message = new EmailMesage();

            message.Destination = a.coworkerEmail;
            message.Subject = "Business Builder - Solicitud de Colaboración - " + a.Draft.details.Name;
            //message.Subject = "Business Team - ";
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
            strBuilder.Append("<br/>");
            strBuilder.Append("Se le ha solicitado una solicitud de colaboración en la plataforma Business Builder.");
            strBuilder.Append("<br/>");
            strBuilder.Append("Para acceder a él utilice el siguiente enlace " + "https://bb.konicaminolta.pt/BusinessBuilder/Draft/" + proposal.ID + "/Overview" + " . Alternativamente, puede acceder a la aplicación y seleccionar la opción Oportunidades.");
            strBuilder.Append("<br/>");
            strBuilder.Append("Gracias");
            strBuilder.Append("<br/>");
            strBuilder.Append("<br/>");
            strBuilder.Append("Business Builder - Por favor no responda a este correo electrónico.");
            message.Body = strBuilder.ToString();

            await emailSend.SendEmailaync(message);


            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err); ;
        }


    }


}
