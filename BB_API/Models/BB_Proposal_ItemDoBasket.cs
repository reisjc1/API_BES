//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BB_Proposal_ItemDoBasket
    {
        public int ID { get; set; }
        public Nullable<int> parentIndex { get; set; }
        public string Family { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public Nullable<double> UnitPriceCost { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<double> TotalCost { get; set; }
        public Nullable<double> Margin { get; set; }
        public Nullable<double> PVP { get; set; }
        public Nullable<double> TotalPVP { get; set; }
        public Nullable<double> DiscountPercentage { get; set; }
        public Nullable<double> UnitDiscountPrice { get; set; }
        public Nullable<double> GPTotal { get; set; }
        public Nullable<double> GPPercentage { get; set; }
        public Nullable<double> TotalNetsale { get; set; }
        public Nullable<bool> IsFinanced { get; set; }
        public Nullable<bool> IsMarginBEU { get; set; }
        public Nullable<double> TCP { get; set; }
        public Nullable<double> ClickPriceBW { get; set; }
        public Nullable<double> ClickPriceC { get; set; }
        public Nullable<int> CounterID { get; set; }
        public Nullable<int> psConfigID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<int> DeliveryLocationID { get; set; }
        public Nullable<int> Group { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
    }
}
