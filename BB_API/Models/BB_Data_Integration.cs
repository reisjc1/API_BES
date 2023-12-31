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
    
    public partial class BB_Data_Integration
    {
        public int ID { get; set; }
        public string CodeRef { get; set; }
        public string Family { get; set; }
        public string Description_English { get; set; }
        public string Description_Portuguese { get; set; }
        public Nullable<double> PVP { get; set; }
        public string Category_PT { get; set; }
        public Nullable<bool> Category_PT_Processed { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> UpdateDatetime { get; set; }
        public string UpdateBy { get; set; }
        public string BinaryImage { get; set; }
        public Nullable<double> MarginBEU { get; set; }
        public Nullable<bool> IsMarginBEU { get; set; }
        public Nullable<double> TCP { get; set; }
        public Nullable<bool> Enable { get; set; }
        public Nullable<double> BEU { get; set; }
    }
}
