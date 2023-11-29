using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Models.RequestModels
{
    public class ServiceQuoteDeleteRequest
    {
        public int ID { get; set; }
        public List<ApprovedPrintingService> PSList { get; set; }
    }
}