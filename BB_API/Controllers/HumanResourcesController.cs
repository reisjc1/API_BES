using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels.HumanResourcesViewModels;


namespace WebApplication1.Controllers
{
    public class HumanResourcesController : ApiController
    {
        public HumanResourcesBLL humanResourcesBLL = new HumanResourcesBLL();
        // GET: HumanResources
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCommissionEntries")]
        public List<CommissionEntry> GetCommissionEntries()
        {
            return humanResourcesBLL.GetCommissionEntries();
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCommissionEntries_New")]
        public List<LD_ContratoModel> GetCommissionEntries_New(string year)
        {
            return humanResourcesBLL.GetCommissionEntries_new(year);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCommissionEntryByID")]
        public CommissionEntry GetCommissionEntryByID(int id)
        {
            return humanResourcesBLL.GetCommissionEntryByID(id);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ExportCommissionExcel")]
        public HttpResponseMessage ExportCommissionExcel()
        {
            return humanResourcesBLL.ExportCommissionExcel(); 
        }
    }
}