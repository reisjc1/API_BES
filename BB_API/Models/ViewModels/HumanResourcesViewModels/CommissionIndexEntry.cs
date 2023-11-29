using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels.HumanResourcesViewModels
{
    public class CommissionIndexEntry
    {
        //changes 12/01/2020
        public string CRMQuoteID { get; set; }
        public Nullable<double> HardwareNetsale { get; set; }
        public Nullable<double> ITSProductsNetsale { get; set; }
        public Nullable<double> ITSServicesNetsale { get; set; }
        public Nullable<double> IndustrialPrintingNetsale { get; set; }
        public Nullable<double> HardwarePVP { get; set; }
        public Nullable<double> ITSProductsPVP { get; set; }
        public Nullable<double> TFT { get; set; }
    }
}