using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ServiceValidationRequestHistoryEntry
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public string Type { get; set; }
        public string ClientName { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime ApprovedAt { get; set; }
        public Nullable<double> Fee { get; set; }
        public Nullable<double> ApprovedValue { get; set; }
        public Nullable<double> ApprovedClickBW { get; set; }
        public Nullable<double> ApprovedClickC { get; set; }

        public string Leasedesk { get; set; }
        public string QuoteNumber { get; set; }

    }
}