using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class GlobalClickVVA
    {
        public int ExcessBillingFrequency { get; set; }
        public double BWExcessPVP { get; set; }
        public double CExcessPVP { get; set; }
        public double PVP { get; set; }
        public Nullable<double> RequestedRent { get; set; }
        public Nullable<double> RequestedBWExcess { get; set; }
        public Nullable<double> RequestedCExcess { get; set; }
        public int RentBillingFrequency { get; set; }
        public int ReturnType { get; set; }
    }
}