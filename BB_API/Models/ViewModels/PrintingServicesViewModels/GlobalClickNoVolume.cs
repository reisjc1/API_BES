using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class GlobalClickNoVolume
    {
        public int PageBillingFrequency { get; set; }
        public double GlobalClickBW { get; set; }
        public double GlobalClickC { get; set; }
        public Nullable<double> RequestedGlobalClickBW { get; set; }
        public Nullable<double> RequestedGlobalClickC { get; set; }
    }
}