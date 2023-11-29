using System;
using System.Collections.Generic;

namespace WebApplication1.Models.ViewModels
{
    public class ReevaluateVVARequest
    {
        public int ID { get; set; }
        public int ActivePrintingService { get; set; }
        public Nullable<double> RequestedRent { get; set; }
        public Nullable<double> RequestedCExcess { get; set; }
        public Nullable<double> RequestedBWExcess { get; set; }
        public string Observations { get; set; }
        public List<ApprovedPrintingService> ApprovedPrintingServices { get; set; }
        public List<ApprovedPrintingService> PendingServiceQuoteRequests { get; set; }
    }
}