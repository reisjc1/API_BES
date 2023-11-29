using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.MIF_Renovations
{

    public class ContratoMIF
    {
        public BB_MIF_Renovations Contrato { get; set; }
        public List<BB_MIF_Renovations_Comments> ContratoComments { get; set; }
        public List<string> StatusList { get; set; }
    }

    public class ContractDRVApproval
    {
        public BB_MIF_Renovations Contract { get; set; }
        public string SelectedButton { get; set; }
    }
    //    public class ContratoMIF2
    //{
    //    public string ContractNr { get; set; }
    //    public string ContractClass { get; set; }
    //    public DateTime StartingDate { get; set; }
    //    public DateTime ExpirationDate { get; set; }
    //    public DateTime NewExpirationDate { get; set; }
    //    public int? CustomerNr { get; set; }
    //    public string CustomerName { get; set; }
    //    public int InvoiceCustomerNr { get; set; }
    //    public string InvoiceCustomerName { get; set; }
    //    public string Segmentation { get; set; }
    //    public string TypeOfContract { get; set; }
    //    public string Delegation { get; set; }
    //    public string SellerCode { get; set; }
    //    public string Seller { get; set; }
    //    public string Model { get; set; }
    //    public string SerialNumber { get; set; }
    //    public string PSNumber { get; set; }
    //    public string Antiquity { get; set; }
    //    public bool EndOfSales { get; set; }
    //    public string SupplyBreakOffPlanDate { get; set; }
    //    public string SupplyClass { get; set; }
    //    public int? TotalMif { get; set; }
    //    public string ProposalProgramCustomer { get; set; }
    //    public string Increase { get; set; }
    //    public string Parecer { get; set; }
    //    public DateTime DeadLine { get; set; }
    //    public string Comments { get; set; }
    //    public List<MIFContractComment> CommentsList { get; set; }
    //}

    //public class MIFContractComment
    //{
    //    public int Id { get; set; }
    //    public string ContractNumber { get; set; }
    //    public Nullable<System.DateTime> CreatedDate { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string Comentario { get; set; }
    //}
}