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
    
    public partial class BB_DB_DEV_LeaseDesk : DbContext
    {
        public BB_DB_DEV_LeaseDesk()
            : base("name=BB_DB_DEV_LeaseDesk")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<LD_DocumentClassification> LD_DocumentClassification { get; set; }
        public virtual DbSet<LD_System> LD_System { get; set; }
        public virtual DbSet<LD_Assinatura_System> LD_Assinatura_System { get; set; }
        public virtual DbSet<LD_DocumentProposal> LD_DocumentProposal { get; set; }
        public virtual DbSet<LD_Status> LD_Status { get; set; }
        public virtual DbSet<BB_Proposal_FinancingApproval> BB_Proposal_FinancingApproval { get; set; }
        public virtual DbSet<LD_Logs> LD_Logs { get; set; }
        public virtual DbSet<LD_Logs_Type> LD_Logs_Type { get; set; }
        public virtual DbSet<BB_Clientes> BB_Clientes { get; set; }
        public virtual DbSet<LD_TipoContrato> LD_TipoContrato { get; set; }
        public virtual DbSet<LD_Contrato_History> LD_Contrato_History { get; set; }
        public virtual DbSet<LD_Observacoes_Motivos> LD_Observacoes_Motivos { get; set; }
        public virtual DbSet<LD_Devolucao_Motivo> LD_Devolucao_Motivo { get; set; }
        public virtual DbSet<LD_Contrato_Facturacao> LD_Contrato_Facturacao { get; set; }
        public virtual DbSet<LD_Contrato> LD_Contrato { get; set; }
        public virtual DbSet<BB_Documents_Type> BB_Documents_Type { get; set; }
        public virtual DbSet<LD_PA5_DocumentProposal> LD_PA5_DocumentProposal { get; set; }
        public virtual DbSet<LD_PA5_DocumentType> LD_PA5_DocumentType { get; set; }
        public virtual DbSet<LD_PA5_EmailConfigSent> LD_PA5_EmailConfigSent { get; set; }
        public virtual DbSet<LD_Email_Log> LD_Email_Log { get; set; }
        public virtual DbSet<BB_Proposal_Contacts_Signing> BB_Proposal_Contacts_Signing { get; set; }
        public virtual DbSet<BB_Proposal_Contacts_Documentation> BB_Proposal_Contacts_Documentation { get; set; }
        public virtual DbSet<LD_DocSign_Control> LD_DocSign_Control { get; set; }
        public virtual DbSet<LD_DocSign_Control_Files> LD_DocSign_Control_Files { get; set; }
        public virtual DbSet<LD_DocSign_Control_History> LD_DocSign_Control_History { get; set; }
        public virtual DbSet<BB_Proposal> BB_Proposal { get; set; }
    }
}
