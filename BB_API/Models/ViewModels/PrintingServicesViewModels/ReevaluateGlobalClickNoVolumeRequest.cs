using System;
using System.Collections.Generic;

namespace WebApplication1.Models.ViewModels
{
    public class ReevaluateGlobalClickNoVolumeRequest
    {
        public int ID { get; set; }
        public int ActivePrintingService { get; set; }
        public Nullable<double> RequestedGlobalClickBW { get; set; }
        public Nullable<double> RequestedGlobalClickC { get; set; }
        public string Observations { get; set; }
        public List<ApprovedPrintingService> ApprovedPrintingServices { get; set; }
        public List<ApprovedPrintingService> PendingServiceQuoteRequests { get; set; }
    }
}