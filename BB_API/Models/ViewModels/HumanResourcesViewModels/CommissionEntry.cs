using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels.HumanResourcesViewModels
{
    public class CommissionEntry
    {
        public CommissionEntry()
        {
            oneShotHardware = new List<OsBasket>();
            oneShotUsedHardware = new List<OsBasket>();
            recurringServicesHardware = new List<RsBasket>();
            oneShotITSProducts = new List<OsBasket>();
            recurringServicesITSProducts = new List<RsBasket>();
            oneShotITSServices = new List<OsBasket>();
            recurringServicesITSServices = new List<RsBasket>();
            oneShotIP = new List<OsBasket>();
        }

        public int ProposalID { get; set; }
        public string CRMQuoteID { get; set; }
        public Nullable<double> HardwareNetsale { get; set; }
        public Nullable<double> UsedHardwarePVP { get; set; }
        public Nullable<double> UsedHardwareNetsale { get; set; }
        public Nullable<double> ITSProductsNetsale { get; set; }
        public Nullable<double> ITSServicesNetsale { get; set; }
        public Nullable<double> IndustrialPrintingNetsale { get; set; }
        public Nullable<double> HardwarePVP { get; set; }
        public Nullable<double> ITSProductsPVP { get; set; }
        public Nullable<double> TFT { get; set; }
        public List<OsBasket> oneShotHardware { get; set; }
        public List<OsBasket> oneShotUsedHardware { get; set; }
        public List<RsBasket> recurringServicesHardware { get; set; }
        public List<OsBasket> oneShotITSProducts { get; set; }
        public List<RsBasket> recurringServicesITSProducts { get; set; }
        public List<OsBasket> oneShotITSServices { get; set; }
        public List<RsBasket> recurringServicesITSServices { get; set; }
        public List<OsBasket> oneShotIP { get; set; }

        public DateTime? Createtime { get; set; }
    }
}