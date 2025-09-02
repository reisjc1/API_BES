using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels.PrintingServicesViewModels
{
    public class VVAClickPerModel
    {
        public int PageBillingFrequency { get; set; }
        public int ExcessBillingFrequency { get; set; }
        public Nullable<double> RequestedRent { get; set; }
        public Nullable<double> RecommendedRent { get; set; }
        public int RentBillingFrequency { get; set; }
        public int ReturnType { get; set; }
        public List<BB_PrintingServices_ClickPerModel_VVA> ps_basket { get; set; }
    }
}