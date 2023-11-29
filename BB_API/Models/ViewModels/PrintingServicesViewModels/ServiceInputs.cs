using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels.PrintingServicesViewModels
{
    public class ServiceInputs
    {
        public Nullable<double> RecommendedRent { get; set; }
        public Nullable<double> RecommendedCExcess { get; set; }
        public Nullable<double> RecommendedBWExcess { get; set; }
        public Nullable<double> RequestedRent { get; set; }
        public Nullable<double> RequestedCExcess { get; set; }
        public Nullable<double> RequestedBWExcess { get; set; }
        public Nullable<double> RecommendedGlobalClickBW { get; set; }
        public Nullable<double> RecommendedGlobalClickC { get; set; }
        public Nullable<double> RequestedGlobalClickBW { get; set; }
        public Nullable<double> RequestedGlobalClickC { get; set; }
        public string Observations { get; set; }
        public Nullable<int> RentBillingFrequency { get; set; }
        public Nullable<int> ExcessBillingFrequency { get; set; }
        public Nullable<int> PageBillingFrequency { get; set; }

        public List<Machine> PS_Basket { get; set; }
        public int BWVolume { get; set; }
        public int CVolume { get; set; }
        public int ContractDuration { get; set; }
        public int ReturnType { get; set; }
    }
}