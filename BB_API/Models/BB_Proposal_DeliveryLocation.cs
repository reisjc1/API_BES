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
    
    public partial class BB_Proposal_DeliveryLocation
    {
        public int IDX { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public string ID { get; set; }
        public string Adress1 { get; set; }
        public string Adress2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Contacto { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string Department { get; set; }
        public string Floor { get; set; }
        public string Building { get; set; }
        public string Room { get; set; }
        public string Schedule { get; set; }
        public Nullable<bool> Payer { get; set; }
        public Nullable<bool> BillReceiver { get; set; }
        public Nullable<int> DeliveryContact { get; set; }
        public Nullable<int> ITContact { get; set; }
        public Nullable<int> ServiceContact { get; set; }
        public Nullable<int> CopiesContact { get; set; }
        public Nullable<int> DeliveryDelegation { get; set; }
        public string EquipmentID { get; set; }
        public string AccessoryID { get; set; }
        public string AccountType { get; set; }
        public Nullable<int> ContractNumber { get; set; }
    }
}
