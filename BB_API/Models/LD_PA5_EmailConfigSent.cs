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
    
    public partial class LD_PA5_EmailConfigSent
    {
        public int ID { get; set; }
        public Nullable<int> ContractID { get; set; }
        public Nullable<int> Nr_Reminder { get; set; }
        public Nullable<bool> IsStarted { get; set; }
        public Nullable<bool> IsFinish { get; set; }
        public Nullable<System.DateTime> NextDateSent { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> Mode { get; set; }
        public Nullable<int> TypeEmail { get; set; }
        public string EmailCC { get; set; }
    }
}
