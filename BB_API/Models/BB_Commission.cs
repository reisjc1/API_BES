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
    
    public partial class BB_Commission
    {
        public int ID { get; set; }
        public Nullable<int> Discount_Start { get; set; }
        public Nullable<int> Discount_End { get; set; }
        public Nullable<double> HW_Printing { get; set; }
        public Nullable<double> Product_ITS { get; set; }
        public Nullable<double> Service_ITS { get; set; }
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public string UpdateBy { get; set; }
    }
}
