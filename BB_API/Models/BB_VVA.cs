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
    
    public partial class BB_VVA
    {
        public Nullable<double> BWExcessPVP { get; set; }
        public Nullable<double> CExcessPVP { get; set; }
        public Nullable<double> PVP { get; set; }
        public Nullable<int> ExcessBillingFrequency { get; set; }
        public Nullable<int> RentBillingFrequency { get; set; }
        public int PrintingServiceID { get; set; }
        public Nullable<int> ReturnType { get; set; }
        public Nullable<double> RequestedBWExcess { get; set; }
        public Nullable<double> RequestedCExcess { get; set; }
        public Nullable<double> RequestedRent { get; set; }
    
        public virtual BB_PrintingServices BB_PrintingServices { get; set; }
    }
}
