﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BB_DB_DEVEntities2 : DbContext
    {
        public BB_DB_DEVEntities2()
            : base("name=BB_DB_DEVEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BB_Acessories> BB_Acessories { get; set; }
        public virtual DbSet<BB_Bank_Bnp> BB_Bank_Bnp { get; set; }
        public virtual DbSet<BB_Consumables> BB_Consumables { get; set; }
        public virtual DbSet<BB_Data_Integration> BB_Data_Integration { get; set; }
        public virtual DbSet<BB_Equipamentos> BB_Equipamentos { get; set; }
        public virtual DbSet<BB_Margin> BB_Margin { get; set; }
        public virtual DbSet<BB_REL_Equipament_Acessorie> BB_REL_Equipament_Acessorie { get; set; }
        public virtual DbSet<BB_REL_Equipament_Consumable> BB_REL_Equipament_Consumable { get; set; }
        public virtual DbSet<BB_WorkSheet_Metadata> BB_WorkSheet_Metadata { get; set; }
        public virtual DbSet<BB_WorkSheet_PT_Metadata> BB_WorkSheet_PT_Metadata { get; set; }
        public virtual DbSet<BB_Proposal_Status> BB_Proposal_Status { get; set; }
        public virtual DbSet<BB_Commission> BB_Commission { get; set; }
        public virtual DbSet<BB_Proposal_Quote_RS> BB_Proposal_Quote_RS { get; set; }
        public virtual DbSet<BB_Proposal_PsConfig> BB_Proposal_PsConfig { get; set; }
        public virtual DbSet<BB_Proposal_Commission> BB_Proposal_Commission { get; set; }
        public virtual DbSet<BB_FinancingContractType> BB_FinancingContractType { get; set; }
        public virtual DbSet<BB_FinancingPaymentMethod> BB_FinancingPaymentMethod { get; set; }
        public virtual DbSet<BB_FinancingType> BB_FinancingType { get; set; }
        public virtual DbSet<BB_Proposal_Counters> BB_Proposal_Counters { get; set; }
        public virtual DbSet<BB_Proposal_FinancingMonthly> BB_Proposal_FinancingMonthly { get; set; }
        public virtual DbSet<BB_Proposal_FinancingTrimestral> BB_Proposal_FinancingTrimestral { get; set; }
        public virtual DbSet<BB_Proposal_Financing> BB_Proposal_Financing { get; set; }
        public virtual DbSet<BB_Proposal_Overvaluation> BB_Proposal_Overvaluation { get; set; }
        public virtual DbSet<BB_Clientes> BB_Clientes { get; set; }
        public virtual DbSet<BB_Proposal_Observations> BB_Proposal_Observations { get; set; }
        public virtual DbSet<BB_CRM_Quotes> BB_CRM_Quotes { get; set; }
        public virtual DbSet<BB_Machines_Compatibility> BB_Machines_Compatibility { get; set; }
        public virtual DbSet<BB_Coderef_Generic> BB_Coderef_Generic { get; set; }
        public virtual DbSet<BB_Contactos> BB_Contactos { get; set; }
        public virtual DbSet<BB_CRM_Workflow> BB_CRM_Workflow { get; set; }
        public virtual DbSet<BB_Campanha> BB_Campanha { get; set; }
        public virtual DbSet<BB_Campanha_Items> BB_Campanha_Items { get; set; }
        public virtual DbSet<BB_Campanha_Financing> BB_Campanha_Financing { get; set; }
        public virtual DbSet<BB_Campanhas_Printing> BB_Campanhas_Printing { get; set; }
        public virtual DbSet<BB_Financing_Documentos> BB_Financing_Documentos { get; set; }
        public virtual DbSet<LD_DocumentClassification> LD_DocumentClassification { get; set; }
        public virtual DbSet<BB_Proposal_PrintingServices> BB_Proposal_PrintingServices { get; set; }
        public virtual DbSet<LD_FinancingDocument> LD_FinancingDocument { get; set; }
        public virtual DbSet<BB_Service_Collab> BB_Service_Collab { get; set; }
        public virtual DbSet<BB_Proposal_Contacts> BB_Proposal_Contacts { get; set; }
        public virtual DbSet<BB_Proposal_Vva> BB_Proposal_Vva { get; set; }
        public virtual DbSet<BB_Proposal_Contacts_Documentation> BB_Proposal_Contacts_Documentation { get; set; }
        public virtual DbSet<BB_Proposal_Contacts_Signing> BB_Proposal_Contacts_Signing { get; set; }
        public virtual DbSet<BB_Proposal_SigningType> BB_Proposal_SigningType { get; set; }
        public virtual DbSet<BB_Proposal_LeaseDesk_Detalhe> BB_Proposal_LeaseDesk_Detalhe { get; set; }
        public virtual DbSet<BB_Proposal_Aprovacao_Quote> BB_Proposal_Aprovacao_Quote { get; set; }
        public virtual DbSet<BB_PrintingServices> BB_PrintingServices { get; set; }
        public virtual DbSet<BB_Proposal_PrintingServices2> BB_Proposal_PrintingServices2 { get; set; }
        public virtual DbSet<BB_Logs_Proposal> BB_Logs_Proposal { get; set; }
        public virtual DbSet<BB_Product> BB_Product { get; set; }
        public virtual DbSet<BB_Product_Category> BB_Product_Category { get; set; }
        public virtual DbSet<BB_Product_Group> BB_Product_Group { get; set; }
        public virtual DbSet<BB_Product_SubCategory> BB_Product_SubCategory { get; set; }
        public virtual DbSet<BB_Product_SubType> BB_Product_SubType { get; set; }
        public virtual DbSet<BB_Product_Type> BB_Product_Type { get; set; }
        public virtual DbSet<BB_Product_Unit> BB_Product_Unit { get; set; }
        public virtual DbSet<BB_Proposal_Consignments> BB_Proposal_Consignments { get; set; }
        public virtual DbSet<BB_Proposal_PrintingServiceValidationRequest> BB_Proposal_PrintingServiceValidationRequest { get; set; }
        public virtual DbSet<BB_Proposal_Client> BB_Proposal_Client { get; set; }
        public virtual DbSet<BB_VVA> BB_VVA { get; set; }
        public virtual DbSet<BB_PrintingServices_NoVolume> BB_PrintingServices_NoVolume { get; set; }
        public virtual DbSet<BB_Proposal_Upturn> BB_Proposal_Upturn { get; set; }
        public virtual DbSet<BB_OPS_Implement_Packs> BB_OPS_Implement_Packs { get; set; }
        public virtual DbSet<BB_OPS_Manage_Packs> BB_OPS_Manage_Packs { get; set; }
        public virtual DbSet<BB_Proposal_Quote> BB_Proposal_Quote { get; set; }
        public virtual DbSet<BB_PrintingService_Machines> BB_PrintingService_Machines { get; set; }
        public virtual DbSet<BB_PrintingServices_ClickPerModel> BB_PrintingServices_ClickPerModel { get; set; }
        public virtual DbSet<BB_Proposal_PrazoDiferenciado_History> BB_Proposal_PrazoDiferenciado_History { get; set; }
        public virtual DbSet<BB_Proposal_OPSManage> BB_Proposal_OPSManage { get; set; }
        public virtual DbSet<BB_Proposal_PrazoDiferenciado> BB_Proposal_PrazoDiferenciado { get; set; }
        public virtual DbSet<BB_Maquinas_Usadas> BB_Maquinas_Usadas { get; set; }
        public virtual DbSet<BB_Maquinas_Usadas_Pedidos> BB_Maquinas_Usadas_Pedidos { get; set; }
        public virtual DbSet<BB_Maquinas_Usadas_Gestor> BB_Maquinas_Usadas_Gestor { get; set; }
        public virtual DbSet<BB_Maquinas_Usadas_Tecnico> BB_Maquinas_Usadas_Tecnico { get; set; }
        public virtual DbSet<BB_Maquinas_Usadas_Logistica> BB_Maquinas_Usadas_Logistica { get; set; }
        public virtual DbSet<BB_Permissions> BB_Permissions { get; set; }
        public virtual DbSet<BB_Permissions_History> BB_Permissions_History { get; set; }
        public virtual DbSet<BB_SLA_History> BB_SLA_History { get; set; }
        public virtual DbSet<BB_RD_MIF_Renovation_Programs> BB_RD_MIF_Renovation_Programs { get; set; }
        public virtual DbSet<BB_RD_MIF_Renovations_Program_Cost_Increments> BB_RD_MIF_Renovations_Program_Cost_Increments { get; set; }
        public virtual DbSet<BB_RD_MIF_Renovations_State> BB_RD_MIF_Renovations_State { get; set; }
        public virtual DbSet<BB_MIF_Renovations_Comments> BB_MIF_Renovations_Comments { get; set; }
        public virtual DbSet<BB_MIF_Renovations> BB_MIF_Renovations { get; set; }
        public virtual DbSet<BB_DeliveryContactsInfo> BB_DeliveryContactsInfo { get; set; }
        public virtual DbSet<BB_LocaisEnvio> BB_LocaisEnvio { get; set; }
        public virtual DbSet<RD_AddressAcronyms> RD_AddressAcronyms { get; set; }
        public virtual DbSet<BB_Proposal_DL_ClientContacts> BB_Proposal_DL_ClientContacts { get; set; }
        public virtual DbSet<BB_Proposal_DeliveryLocation> BB_Proposal_DeliveryLocation { get; set; }
        public virtual DbSet<BB_Proposal_ItemDoBasket> BB_Proposal_ItemDoBasket { get; set; }
        public virtual DbSet<BB_Proposal> BB_Proposal { get; set; }
        public virtual DbSet<BB_Proposal_OPSImplement> BB_Proposal_OPSImplement { get; set; }
    }
}
