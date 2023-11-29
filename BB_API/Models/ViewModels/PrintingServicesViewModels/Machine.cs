using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class Machine
    {
        public Nullable<int> ID { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public Nullable<int> BWVolume { get; set; }
        public Nullable<int> CVolume { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<double> RequestedBWClickPrice { get; set; }
        public Nullable<double> RequestedCClickPrice { get; set; }
        public Nullable<int> bwPages { get; set; }
        public Nullable<int> cPages { get; set; }
        public Nullable<double> ClickPriceBW { get; set; }
        public Nullable<double> ClickPriceC { get; set; }
        public Nullable<double> BWPVP { get; set; }
        public Nullable<double> CPVP { get; set; }
        public Nullable<bool> IsInClient { get; set; }
        public Nullable<bool> IsUsed { get; set; }
        public Nullable<double> BWCost { get; set; }
        public Nullable<double> CCost { get; set; }
    }
}