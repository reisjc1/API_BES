using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ServiceValidationRequestIndexEntry
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public int TypeID { get; set; }
        public string ClientName { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Observations { get; set; }
    }
}