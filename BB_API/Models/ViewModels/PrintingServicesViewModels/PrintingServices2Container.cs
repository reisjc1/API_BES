using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class PrintingServices2Container
    {
        public PrintingServices2 ActivePrintingService { get; set; }
        public ICollection<PrintingServices2> PrintingServiceRequestHistory { get; set; }
    }
}