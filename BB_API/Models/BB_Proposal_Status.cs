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
    
    public partial class BB_Proposal_Status
    {
        public int ID { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Nullable<int> Status_Order { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<bool> BB_Edit { get; set; }
        public Nullable<int> Phase { get; set; }
    }
}
