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
    
    public partial class BB_Proposal_PrazoDiferenciado_History
    {
        public int ID { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public Nullable<int> PrazoDiferenciado { get; set; }
        public Nullable<double> ValorRenda { get; set; }
        public Nullable<double> ValorFactor { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<int> FinancingID { get; set; }
        public Nullable<double> ValorFinanciamento { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public string Alocadora { get; set; }
        public Nullable<bool> IsAproved { get; set; }
        public string Commets { get; set; }
        public string Type { get; set; }
        public string GestorContaObservacoes { get; set; }
        public string NLocadora { get; set; }
        public Nullable<int> Frequency { get; set; }
    }
}
