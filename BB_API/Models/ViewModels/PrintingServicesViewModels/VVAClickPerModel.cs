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
    }
}