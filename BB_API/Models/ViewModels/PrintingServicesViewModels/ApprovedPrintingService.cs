using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.ViewModels.PrintingServicesViewModels;

namespace WebApplication1.Models.ViewModels
{
    public class ApprovedPrintingService
    {
        public Nullable<int> ID { get; set; }
        public int BWVolume { get; set; }
        public int CVolume { get; set; }
        public int ContractDuration { get; set; }
        public double Fee { get; set; }
        public bool IsPrecalc { get; set; }
        public string SEObservations { get; set; }
        public string SCObservations { get; set; }
        public Nullable<DateTime> RequestedAt { get; set; }
        public List<Machine> Machines { get; set; }
        public GlobalClickVVA GlobalClickVVA { get; set; }
        public GlobalClickNoVolume GlobalClickNoVolume { get; set; }
        public ClickPerModel ClickPerModel { get; set; }
    }
}