using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ServiceValidationRequestEquipment
    {
        public Nullable<int> ID { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool IsUsed { get; set; }
        public bool IsInClient { get; set; }
        public string Type { get; set; }
        public Nullable<int> RecBWVolume { get; set; }
        public Nullable<int> RecCVolume { get; set; }
        public Nullable<double> BWCoefficient { get; set; }
        public Nullable<int> BWPages { get; set; }
        public Nullable<double> CCoefficient { get; set; }
        public Nullable<int> CPages { get; set; }
        public Nullable<double> ClickPriceBW { get; set; }
        public Nullable<double> ClickPriceC { get; set; }
        public Nullable<double> BWBaseCost { get; set; }
        public Nullable<double> CBaseCost { get; set; }
        public Nullable<double> ApprovedBW { get; set; }
        public Nullable<double> ApprovedC { get; set; }
        public Nullable<double> RequestedBWClickPrice { get; set; }
        public Nullable<double> RequestedCClickPrice { get; set; }
    }
}