using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;
using WebApplication1.BLL;
using WebApplication1.Models;

namespace BB_API.Controllers
{
    public class HomeController : Controller
    {

        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public ActionResult Index()
        {
            ViewBag.Title = "Test 123";
            return View();
        }

        public async Task<ActionResult> TestEmail()
        {
            EmailService emailSend = new EmailService();
            EmailMesage message = new EmailMesage();

            message.Destination = "diogo.cruz@konicaminolta.pt";
            message.Subject = "Test Table";
            StringBuilder strBuilder = new StringBuilder();

            string tableStyle = "border: 1px solid #ddd; border-collapse: collapse";
            string thStyle = "background-color: #245982;text-align: center;color: white;border: 1px solid #ddd; border-collapse: collapse";

            strBuilder.Append("<table style = '" + tableStyle + "'><tr><th style='" + thStyle + "'> Column 1</th><th style='" + thStyle + "'>Column 2</th><th style='" + thStyle + "'>Column 3</th></tr>");
            strBuilder.Append("<tr><td style = '" + tableStyle + "'>Value 1</td><td style = '" + tableStyle + "'>Value 2</td><td style = '" + tableStyle + "'>Value 3</td></tr>");
            strBuilder.Append("<tr><td style = '" + tableStyle + "'>Value 4</td><td style = '" + tableStyle + "'>Value 5</td><td style = '" + tableStyle + "'>Value 6</td></tr>");
            strBuilder.Append("<tr><td style = '" + tableStyle + "'>Value 7</td><td style = '" + tableStyle + "'>Value 8</td><td style = '" + tableStyle + "'>Value 9</td></tr>");
            strBuilder.Append("</table>");
            message.Body = strBuilder.ToString();

            await emailSend.SendEmailaync(message);
            return View("Index", "Home");
        }
    }
}
