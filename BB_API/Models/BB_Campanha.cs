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
    
    public partial class BB_Campanha
    {
        public int ID { get; set; }
        public string Campanha { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedTime { get; set; }
        public Nullable<bool> Enable { get; set; }
        public Nullable<int> ApplySettings { get; set; }
    }
}
