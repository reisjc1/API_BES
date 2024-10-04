using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class DocumentProposal
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public Nullable<int> ContratoID { get; set; }

        public string QuoteNumber { get; set; }
        public string Name { get; set; }
        public string Drive { get; set; }
        public string Folder { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileFullPath { get; set; }
        public LD_DocumentClassification Classification { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<int> SystemID { get; set; }
        public Nullable<bool> DocumentIsProcess { get; set; }
        public Nullable<bool> DocumentIsValid { get; set; }

        public LD_System System { get; set; }
        public string Comments { get; set; }

    }

    public class FecharProcessoModel
    {
        public int? id { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class DevolverProcessoModel
    {
        public int? id { get; set; }
        public string ModifiedBy { get; set; }

        public string Comentarios { get; set; }

        public int? DevolucacoMotivoID { get; set; }
    }

    public class ValidateDocumentarionModel
    {
        public List<DocumentProposal> docContrato { get; set; }
        public string quoteNumber { get; set; }

        public int? contractoID { get; set; }
    }

    public class RespostaLeaseDeskValidateDocumentacao
    {
        public List<string> documentosEmFalta { get; set; }
        public string ErrMensagem { get; set; }

        public int errCode { get; set; }

    }

    public class GerarContrato
    {
        public int? proposalID { get; set; }
        public string ContractType { get; set; }
        public string FinancingType { get; set; }
        public string FinancingPaymentMethod { get; set; }
        public List<Monthly> monthly { get; set; }
        public List<Trimestral> trimestral { get; set; }

        public int? Selectmonthly { get; set; }

        public string AbreviaturaCliente { get; set; }

        public string ENDEREÇODEMAIL { get; set; }
        public string ClienteName { get; set; }

        public string ClienteAccountNumber { get; set; }

        public string Locadora { get; set; }


        public string NomeProposta { get; set; }

        public double? Renda { get; set; }

        public int? NrMeses { get; set; }

        public string TipoFicheiro { get; set; }

        public string QuoteNumber { get; set; }

        public List<BB_Proposal_Contacts_Signing> lst_BB_Proposal_Contacts_Signing { get; set; }

        public List<BB_Proposal_Contacts_Documentation> lst_BB_Proposal_Contacts_Documentation { get; set; }

    }
    public class GerarContratoFormal
    {
        public int? proposalID { get; set; }
        public string ContractType { get; set; }
        public string FinancingType { get; set; }
        public string FinancingPaymentMethod { get; set; }
        public List<Monthly> monthly { get; set; }
        public List<Trimestral> trimestral { get; set; }

        public int? Selectmonthly { get; set; }


        public string Cliente { get; set; }

        public string quotasAnonimas { get; set; }

        public string Morada { get; set; }

        public string NIF { get; set; }

        public string LegalRepresentante { get; set; }
        public string QualidadeRepresentante { get; set; }

        public string AbreviaturaCliente { get; set; }

        public string NrMesesContrato { get; set; }

        public string CapitalSocial { get; set; }

        public string NrPaginasPreto { get; set; }
        public string ValorPaginaPreto { get; set; }

        public string ValorExtensoPaginaPreto { get; set; }
        public string ValorPaginaPretoExcedente { get; set; }
        public string ValorExtensoPaginaPretoExcedente { get; set; }
        public string ENDEREÇODEMAIL { get; set; }

        public string DesignacaoComercialCliente { get; set; }


    }

    public class GravarPastaModel
    {
        public int? id { get; set; }

        public string Pasta { get; set; }


    }
    public class GravarNUSModel
    {
        public int? id { get; set; }

        public string ModifiedBy { get; set; }

        public string NUS { get; set; }
    }

    public partial class LD_ContratoModel
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public string QuoteNumber { get; set; }
        public string PathContracto { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<bool> ContratoValidado { get; set; }
        public Nullable<bool> ContratoGerado { get; set; }
        public Nullable<System.DateTime> Assinatura { get; set; }
        public Nullable<int> SystemAssinaturaID { get; set; }
        public Nullable<int> StatusID { get; set; }

        public string StatusName { get; set; }

        public string SistemaAssinatura { get; set; }

        public string FilenameContracto { get; set; }

        public string NCliente { get; set; }
        public string NomeCliente { get; set; }

        public string TipoContrato { get; set; }

        public string ComentariosDevolucao { get; set; }

        public Nullable<int> MotivoID { get; set; }
        public Nullable<int> DevolucaoMotivoID { get; set; }

        public string MotivoDescricao { get; set; }
        public string DevolucaoMotivoDescricao { get; set; }

        public bool? isClosed { get; set; }

        public string Financiamento { get; set; }

        public string NUS { get; set; }
        public string FacuracaoModifiedby { get; set; }
        public DateTime? FacuracaoModifiedtime { get; set; }

        public string FolderDOC { get; set; }

        public string InvoiceAssistant { get; set; }

        public string TipoNegocio { get; set; }
    }

    public  class LD_DocSign_Control_Model_History
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public Nullable<int> LeasedeskID { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<int> DocSignStatusSent { get; set; }
        public Nullable<System.DateTime> DocSignDateTimeSent { get; set; }
        public Nullable<int> NrPageSign { get; set; }
        public string DocPath { get; set; }
        public string DocSignEnvelopeID { get; set; }

        public string DescricaoStatus { get; set; }
    }

    public class LD_ContratoHistoricoModel
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public string QuoteNumber { get; set; }
        public string PathContracto { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<bool> ContratoValidado { get; set; }
        public Nullable<bool> ContratoGerado { get; set; }
        public Nullable<System.DateTime> Assinatura { get; set; }
        public Nullable<int> SystemAssinaturaID { get; set; }
        public Nullable<int> StatusID { get; set; }

        public string StatusName { get; set; }

        public string SistemaAssinatura { get; set; }

        public string FilenameContracto { get; set; }

        public string Cliente { get; set; }

        public string ComentariosDevolucao { get; set; }

        public string MotivoDescricao { get; set; }

        public string DevolucaoDescricao { get; set; }
    }



    public class BB_Proposal_FinancingApprovalModel
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public Nullable<int> FinancingID { get; set; }
        public string Createby { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<bool> IsComplete { get; set; }


        public Nullable<bool> IsAproved { get; set; }
        public string Comments { get; set; }

        public string ClienteName { get; set; }

        public string AccountNumber { get; set; }

        public int? BankID { get; set; }

        public string Bank { get; set; }
    }

    public class EmitirContractoModel
    {


        public string dataAssintaura { get; set; }
        public string tipoDeEnvio { get; set; }

        public int? id { get; set; }

        public string EmailPara { get; set; }
        public string EmailCC { get; set; }

        public string gestorContaEmail { get; set; }
    }

    public class BB_Proposal_DeliveryLocationResumoModel
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

        public string Family { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public Nullable<int> Qty { get; set; }

        public Nullable<int> DeliveryLocationID { get; set; }
        public Nullable<int> Group { get; set; }

        public Nullable<int> PropostaID { get; set; }
        public string AddressType { get; set; }
    }

    public class BB_Proposal_DeliveryGroup
    {
        public int? nrMorada { get; set; }
        public int? nrGroup { get; set; }
    }

    public class PA5Documents
    {

        public int ID { get; set; }
        public string Name { get; set; }

        public bool IsValid { get; set; }

        public bool IsToSend { get; set; }
    }

    public class ListPA5Documents
    {
        public List<PA5Documents> LstPA5Documents { get; set; }
        public int? ContractID { get; set; }

        public string ClientesEmails { get; set; }

        public string PA5DocumentosEmailNotas { get; set; }

        public int Mode { get; set; }

        public string EmailCreatedBy { get; set; }

        public string EmailCC { get; set; }

    }

    public class SignatureContact
    {
        public int? ID { get; set; }
        [DisplayName("Nome")]
        public string Name { get; set; }
        public string Email { get; set; }
        [DisplayName("Telefone")]
        public string Telephone { get; set; }
    }

    public class SalesProcessViewModel
    {
        public SalesProcessViewModel()
        {
            SignatureTypes = new List<SelectListItem>();
            SignatureContacts = new List<SignatureContact>();
        }
        public int ContractID { get; set; }
        public int? ProposalID { get; set; }
        [DisplayName("Quote ID CRM")]
        public string QuoteNumber { get; set; }
        [DisplayName("Nº de Cliente")]
        public string ClientNumber { get; set; }
        [DisplayName("Cliente")]
        public string ClientName { get; set; }
        [DisplayName("Nome da Proposta")]
        public string ProposalName { get; set; }
        [DisplayName("Locadora")]
        public string LeasingCompany { get; set; }
        [DisplayName("Tipo de Financiamento")]
        public string FinancingType { get; set; }
        [DisplayName("Método de Pagamento")]
        public string FinancingPaymentMethod { get; set; }
        [DisplayName("Nº de Meses")]
        public int Frequency { get; set; }
        [DisplayName("Renda")]
        public double Rent { get; set; }
        [DisplayName("Gestor de Conta")]
        public string AccountManager { get; set; }
        [DisplayName("Tipo de Contracto")]
        public string ContractType { get; set; }
        [DisplayName("Data de Envio")]
        public DateTime SendDate { get; set; }
        public string AMObservations { get; set; }
        public string Observations { get; set; }
        public string ReturnObservations { get; set; }
        public string DocumentationFolder { get; set; }
        [DisplayName("Tipo de Assinatura")]
        public int SignatureType { get; set; }
        [DisplayName("Tipo de Assinatura")]
        public List<SelectListItem> SignatureTypes { get; set; }
        [DisplayName("Contacto C&C Manager")]
        public SignatureContact KMContact { get; set; }
        [DisplayName("Contactos de Assinatura")]
        public List<SignatureContact> SignatureContacts { get; set; }
        public string DocusignEnvelopeID { get; set; }
        public string DocusignSignatureStatus { get; set; }
        public string PageNum { get; set; }
        public List<LD_DocSign_Control> ListagemFicheirosDocuSign { get; set; }
    }


}