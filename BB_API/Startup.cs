using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApplication1.Provider;

[assembly: OwinStartupAttribute(typeof(BB_API.Startup))]

namespace BB_API
{
    public  partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app); 
            HttpConfiguration config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();

            //ConfigureOAuth(app);

            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //var cors = new EnableCorsAttribute("http://dev.bb.km.com:8083", "*", "*");
            //config.EnableCors();

            app.UseWebApi(config);

            ConfigureOAuth(app);

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //    //var issuer = "http://jwtauthzsrv.azurewebsites.net";
            //    //var audience = "099153c2625149bc8ecb3e85e03f0022";
            //    //var secret = TextEncodings.Base64Url.Decode("IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw");

            // Api controllers with an [Authorize] attribute will be validated with JWT
            var opcoesConfiguracaoToken = new OAuthAuthorizationServerOptions()
        {
            AllowInsecureHttp = true,
            TokenEndpointPath = new PathString("/token"),
            AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
            Provider = new CustomOAuthProvider()
        };



        // OAuth 2.0 Bearer Access Token Generation
        app.UseOAuthAuthorizationServer(opcoesConfiguracaoToken);
        //app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }
    }
}
