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
    
    public partial class BB_Proposal_OPSManage
    {
        public int ProposalID { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public string Family { get; set; }
        public Nullable<bool> InCatalog { get; set; }
        public Nullable<int> MaxRange { get; set; }
        public Nullable<int> MinRange { get; set; }
        public string Name { get; set; }
        public Nullable<double> PVP { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Type { get; set; }
        public Nullable<double> UnitDiscountPrice { get; set; }
        public Nullable<int> TotalMonths { get; set; }
        public Nullable<int> Position { get; set; }
        public int ID { get; set; }
        public Nullable<bool> IsValidated { get; set; }
    
        public virtual BB_Proposal BB_Proposal { get; set; }
    }
}
