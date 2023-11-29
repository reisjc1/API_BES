using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class PrintingServices2
    {
        public PrintingServices2()
        {
            this.ID = null;
            this.ActivePrintingService = null;
            this.ApprovedPrintingServices = new List<ApprovedPrintingService>();
            this.PendingServiceQuoteRequests = new List<ApprovedPrintingService>();
        }

        public Nullable<int> ID { get; set; }
        public Nullable<int> ActivePrintingService { get; set; }
        public List<ApprovedPrintingService> ApprovedPrintingServices { get; set; }
        public List<ApprovedPrintingService> PendingServiceQuoteRequests { get; set; }
    }
}