using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Owin;
using Owin;
using WebApplication1.Controllers;
using WebApplication1.Models;

[assembly: OwinStartup(typeof(WebApplication1.Provider.ClaimsAuthorizeAttribute))]

namespace WebApplication1.Provider
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private string claimType;
        private string claimValue;
        public ClaimsAuthorizeAttribute()
        {
            //this.claimType = type;
            //this.claimValue = value;
        }
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //string token = actionContext.Request.Headers.Authorization.ToString();

            //ClaimsPrincipal p = TokenManager.GetPrincipal(token);

            //bool isAuthorized = base.IsAuthorized(actionContext);
            //bool isRequestHeaderOk = false;
            //return isAuthorized && isRequestHeaderOk;


            //throw new ActionFailResponse("Nao autorizado")
            //{
            //    StatusCode = 403,


            //};
            ActionResponse ac = new ActionResponse();
            ac.Message = "ffff";
            ac.ProposalIDReturn = 1;
            //throw new HttpResponseException(actionContext.Request.CreateResponse<ActionResponse>(HttpStatusCode.NotFound, ac));

            return true;

        }
    }
}
