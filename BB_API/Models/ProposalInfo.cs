using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Controllers;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Models
{
    public class ProposalInfo
    {
    }
    public class UserAssign
    {
        public string coworkerEmail { get; set; }
        public string observation { get; set; }
        public string createdBy { get; set; }

        //public ProposalRootObject proposalData { get; set; }
        public Draft Draft { get; set; }

        public Summary Summary { get; set; }
    }
    public class ProposalRootObject
    {
        public Nullable<int> Frequency { get; set; }
        public int Months { get; set; }
        public Draft Draft { get; set; }

        public Summary Summary { get; set; }

        public string Type { get; set; }

        public string GestorContaObservacoes { get; set; }
        public string NUS { get; set; }
        public string FolderDoc { get; set; }

        public ValoresTotaisRenda valoretotais { get; set; }

        public string LeasedeskMotivo { get; set; }
        public string LeasedeskStatus { get; set; }
        public string LeasedeskComentariosGC { get; set; }
        public string LeasedeskComentarios { get; set; }

        public string LeasedeskComentariosDevolucao { get; set; }


        public Comission Comission { get; set; }

        //public List<BB_Equipamentos> Equipamentos { get; set; }
    }


    public class Comission
    {
        public HW_Printing Hw_Printing { get; set; }

        public ITS_Comissao ITS { get; set; }
        public TFM_Comissao TFM { get; set; }

        public NewClient_Comissao Newclient { get; set; }
        public DebitoDirecto_Commisao DebitoDirecto { get; set; }

        public double? TotalComissao { get; set; }
    }

    public class HW_Printing
    {
        public double? HW_PrintingComissao { get; set; }
        public double? ValueHardware { get; set; }
        public double? Percentagem { get; set; }

    }
    public class TFM_Comissao
    {
        public double? TFMComissao { get; set; }

        public double? ValueValor { get; set; }
        public double? Percentagem { get; set; }

    }

    public class DebitoDirecto_Commisao
    {
        public double? DebitoDirectoComissao { get; set; }
        public double? ValueValor { get; set; }
        public double? Percentagem { get; set; }

        public bool Isdebit { get; set; }

    }

    public class NewClient_Comissao
    {
        public double? NewClientComissao { get; set; }

        public bool IsNewClient { get; set; }
        public double? ValueValor { get; set; }
        public double? Percentagem { get; set; }

    }


    public class ITS_Comissao
    {
        public double? ITSComissao { get; set; }
        public double? GP2 { get; set; }
        public double? Percetangem { get; set; }

        public double? PRS { get; set; }

        public double? MCS { get; set; }
        public double? IMS { get; set; }
        public double? VSS { get; set; }

        public double? HW { get; set; }

        public double? SW { get; set; }

        public double? BPO { get; set; }


    }


    public class ValoresTotaisRenda
    {
        public double? RendaFinanciada { get; set; }
        public double? RendaTotal { get; set; }
        public double? VVA { get; set; }

        public double? clickPriceBW { get; set; }
        public double? clickPriceC { get; set; }

        public int? VolumeBW { get; set; }
        public int? VolumeC { get; set; }

        public double? sobrevalorizacaoTotal { get; set; }

        public double? retomasTotal { get; set; }

        public double? ServicosRecorentesTotal { get; set; }

        public double? ServicosRecorentesMes { get; set; }

        public double? ConfiguracaoOneShotValor { get; set; }

        public double? LeiCopiaPrivada { get; set; }


    public string ServiçoComentarios { get; set; }

    }

    public class Draft
    {
        //public bool hasStateChanged { get; set; }
        //public AssignedEmployee assignedEmployee { get; set; }
        public Baskets baskets { get; set; }
        public Client client { get; set; }
        public Details details { get; set; }
        public Financing financing { get; set; }
        public OPSPacks opsPacks { get; set; }
        public List<Overvaluation> overvaluations { get; set; }
        public PrintingServices printingServices { get; set; }
        public PrintingServices2 printingServices2 { get; set; }

        public List<Upturn> upturns { get; set; }

        public List<DeliveryLocation> deliveryLocations { get; set; }
        public DeliveryLocationsBes deliveryLocationsBES { get; set; }

        public Consignment consignment { get; set; }

        public List<Maquinas_Usadas_Gestor> Maquinas_Usadas_Gestor { get; set; }
        public List<BB_Permissions> shareProfileDelegation { get; set; }
    }

    public class OPSPacks
    {
        public OPSPacks()
        {
            opsImplement = new List<OPSImplement>();
            opsManage = new List<OPSManage>();
        }
        public List<OPSImplement> opsImplement { get; set; }
        public List<OPSManage> opsManage { get; set; }
    }

    public class OPSImplement
    {
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public string Family { get; set; }
        public int? ID { get; set; }
        public bool InCatalog { get; set; }
        public bool IsFinanced { get; set; }
        public int? MaxRange { get; set; }
        public int? MinRange { get; set; }
        public string Name { get; set; }
        public double? PVP { get; set; }
        public int? Quantity { get; set; }
        public string Type { get; set; }
        public double? UnitDiscountPrice { get; set; }
        public bool IsValidated { get; set; }
    }
    public class OPSManage
    {
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public string Family { get; set; }
        public int? ID { get; set; }
        public bool InCatalog { get; set; }
        public int? MaxRange { get; set; }
        public int? MinRange { get; set; }
        public string Name { get; set; }
        public double? PVP { get; set; }
        public int? Quantity { get; set; }
        public int? TotalMonths { get; set; }
        public string Type { get; set; }
        public double? UnitDiscountPrice { get; set; }
        public bool IsValidated { get; set; }
    }

    public class Consignment
    {

        public Nullable<int> Duration { get; set; }
        public string Motive { get; set; }

    }

    public class Client
    {
        //public string ClientAccountNumber { get; set; }


        public int ID { get; set; }
        public string accountnumber { get; set; }
        public string Name { get; set; }
        public string new_accountfullname { get; set; }
        public string new_fullname { get; set; }
        public string createdbyname { get; set; }
        public string owneridname { get; set; }
        public string address1_postalcode { get; set; }
        public string address1_line1 { get; set; }
        public string emailaddress1 { get; set; }
        public string new_companyregistrationnumber { get; set; }
        public bool? Blocked { get; set; }
        public string Holding { get; set; }
        public string GMA { get; set; }
        public bool? isNewClient { get; set; }
        public bool? isPublicSector { get; set; }
        public int? modeId { get; set; }

        public string NIF { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

    }
    public class Details
    {
        public int ID { get; set; }
        public string CRM_QUOTE_ID { get; set; }
        public bool? Locked { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        //public int Status { get; set; }
        public double ValueComission { get; set; }
        public double ValueTotal { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }

        public ProposalStatus Status { get; set; }
        public int StatusID { get; set; }

        public List<CRObservations> CRObservations { get; set; }

        public string AccountManager { get; set; }

        public int? CampaignID { get; set; }

        public bool? IsMultipleContract { get; set; }

    }

    public class CRObservations
    {
        public string CreatedBy { get; set; }
        public string Observation { get; set; }

        public DateTime? Created { get; set; }
    }

    public class Counter
    {
        public int bwCounter { get; set; }
        public int cCounter { get; set; }

        public string serialNumber { get; set; }
    }

    public class ProposalStatus
    {
        public string Name { get; set; }
        public bool? IsEdit { get; set; }

        public int? Phase { get; set; }
    }

    public class PsConfig
    {
        public double BWExcessCost { get; set; }
        public int BWVolume { get; set; }
        public double BWCost { get; set; }
        public double CExcessCost { get; set; }
        public int CVolume { get; set; }
        public double CCost { get; set; }
        public double BWDiscount { get; set; }
        public double CDiscount { get; set; }
    }
    public class Summary
    {
        public double businessTotal { get; set; }
        public double totalCost { get; set; }
        public List<FinancingFactor> financingFactors { get; set; }
        public Commission commission { get; set; }

        public double financedNetsale { get; set; }

        public double IVA { get; set; }

        public double totalIVA { get; set; }

        public double? subTotal { get; set; }
    }

    public class Commission
    {
        public double PrintingHW_Commission { get; set; }
        public double ProductsITS_Commission { get; set; }
        public double ServicesITS_Commission { get; set; }
        public double IP_Commission { get; set; }
        public double WPH_Commission { get; set; }
        public double totalCommission { get; set; }
    }

    public class FinancingFactor
    {
        public int ID { get; set; }
        public string Type_Contract { get; set; }
        public string Type_Duration { get; set; }
        public int Contracto { get; set; }
        public int Volume_Start { get; set; }
        public int Volume_End { get; set; }
        public double Value { get; set; }
        public int DespesaContracto { get; set; }
        public int Iva { get; set; }
        public object UpdateDatetime { get; set; }
        public object UpdateBy { get; set; }

        public List<Monthly> Monthly { get; set; }
        public List<Trimestral> Trimestral { get; set; }
        public List<SimulationMonthly> SimulationMonthly { get; set; }
        public List<SimulationTrimestral> SimulationTrimestral { get; set; }
    }

    public class Baskets
    {
        public List<OsBasket> os_basket { get; set; }
        public List<RsBasket> rs_basket { get; set; }
    }

    public class DeliveryLocationsBes
    {
        public List<BB_Proposal_DeliveryLocation> deliveryLocationsShipToBillTo { get; set; }
        public List<AssignedItems> AssignedItems { get; set; }
    }

    public class Financing
    {
        public int ContractTypeId { get; set; }
        public int FinancingTypeCode { get; set; }
        public int PaymentMethodId { get; set; }
        public int PaymentAfter { get; set; }
        public bool IncludeServices { get; set; }
        public int Months { get; set; }
        public int MonthlyIncome { get; set; }
        public FinancingFactors FinancingFactors { get; set; }

        public DiffTerm diffTerm { get; set; }

        public DateTime? DataExpiracao { get; set; }
        public DateTime? DateApproval { get; set; }
        public DateTime? DateExpired { get; set; }
    }

    public class DiffTerm
    {
        public double? Factor { get; set; }
        public double? Rent { get; set; }
        public double financedNetsale { get; set; }
        public int Months { get; set; }
        public int Frequency { get; set; }

        public bool? IsAproved { get; set; }

        public bool? IsComplete { get; set; }

        public string Comments { get; set; }

        public string Alocadora { get; set; }

        public string Nlocadora { get; set; }

    }
    public class FinancingFactors
    {
        public List<Monthly> Monthly { get; set; }
        public List<Trimestral> Trimestral { get; set; }
        public List<SimulationMonthly> SimulationMonthly { get; set; }
        public List<SimulationTrimestral> SimulationTrimestral { get; set; }
    }

    public class Monthly
    {
        public int Contracto { get; set; }
        public double Value { get; set; }
    }

    public class Trimestral
    {
        public int Contracto { get; set; }
        public double Value { get; set; }
    }

    public class OsBasket
    {

        public string Family { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public double UnitPriceCost { get; set; }
        public int Qty { get; set; }
        public double TotalCost { get; set; }
        public double Margin { get; set; }
        public double PVP { get; set; }
        public double TotalPVP { get; set; }
        public double DiscountPercentage { get; set; }
        public double UnitDiscountPrice { get; set; }
        public double GPTotal { get; set; }
        public double GPPercentage { get; set; }
        public double TotalNetsale { get; set; }
        public bool? IsFinanced { get; set; }
        public bool? IsMarginBEU { get; set; }
        public double? TCP { get; set; }
        public double? ClickPriceBW { get; set; }
        public double? ClickPriceC { get; set; }
        public List<Counter> counters { get; set; }
        public PsConfig psConfig { get; set; }
        public string Name { get; set; }
        public bool? Locked { get; set; }
        public bool? IsUsed { get; set; }
        public bool? IsInClient { get; set; }
    }



    public class RsBasket
    {
        public int ID { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public double DiscountPercentage { get; set; }
        public string Family { get; set; }
        public double GPPercentage { get; set; }
        public double GPTotal { get; set; }
        public bool Locked { get; set; }
        public double Margin { get; set; }
        public double MonthlyFee { get; set; }
        public double MonthlyGP { get; set; }
        public double PVP { get; set; }
        public int Qty { get; set; }
        public double TotalCost { get; set; }
        public double MonthlyFeeCost { get; set; }
        public int TotalMonths { get; set; }
        public double TotalNetsale { get; set; }
        public double TotalPVP { get; set; }
        public double UnitDiscountPrice { get; set; }
        public double UnitPriceCost { get; set; }
        public int ProposalID { get; set; }
        public bool? IsFinanced { get; set; }

    }

    public class BB_PROPOSALS
    {
        public int ID { get; set; }
        public string CRM_QUOTE_ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<double> ValueComission { get; set; }
        public Nullable<double> ValueTotal { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<int> ClientID { get; set; }

        public string StatusName { get; set; }
    }

    public class BB_PROPOSALS_GET
    {
        public int ID { get; set; }
        public string CRM_QUOTE_ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<double> ValueComission { get; set; }
        public Nullable<double> ValueTotal { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<int> ClientID { get; set; }

        public string ClientName { get; set; }
        public string StatusName { get; set; }

        public string AccountManager { get; set; }

        public BB_Proposal_Status StatusObj { get; set; }

        public string StatusCRM { get; set; }
    }

    public class BB_PROPOSALS_GET_V1
    {
        public int? ID { get; set; }
        public string AccountNumber { get; set; }
        public string ClientName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double TotalValue { get; set; }
        public string QuoteCRM { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? CreatedTime { get; set; }

        public string FinancingStatus { get; set; }
        public string ServiceStatus { get; set; }
        public string LeaseDeskStatus { get; set; }
        public string AccountManagerEmail { get; set; }
        public string CreatedByEmail { get; set; }
        public string ModifiedByEmail { get; set; }
        public string AccountManagerName { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public BB_Proposal_Status Status { get; set; }
    }

    public class Overvaluation
    {
        public string Motive { get; set; }
        public double? Total { get; set; }
        public string Description { get; set; }
    }

    public class Vva
    {
        public int? BWVolume { get; set; }
        public int? CVolume { get; set; }
        public double? MonthlyFee { get; set; }
        public bool? UseGlobalValues { get; set; }

        public bool? UseGlobalExcess { get; set; }

        public double? BWExcess { get; set; }
        public double? CExcess { get; set; }
    }

    public class PrintingServices
    {
        public int modeId { get; set; }
        public int ContractPeriod { get; set; }
        public double FeeFrequency { get; set; }
        public int BillingFrequency { get; set; }
        public Vva vva { get; set; }

        public BB_Service_Collab_Model serviceController { get; set; }
    }


    public class BB_Service_Collab_Model
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<bool> IsAproved { get; set; }
        public string CommentsGC { get; set; }
        public string CommentsSC { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }

    public class Upturn
    {
        public Nullable<int> ID { get; set; }
        public double? Total { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Contact { get; set; }
    }

    public class Owner_
    {

        public string Owner { get; set; }
    }

    public class BB_Clientes_
    {
        public int ID { get; set; }
        public string accountnumber { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string PostalCode { get; set; }
        public string address1_line1 { get; set; }
        public string emailaddress1 { get; set; }
        public string NIF { get; set; }

        public string Holding { get; set; }

        public string GMA { get; set; }

        public string Segment { get; set; }

        public bool? Blocked { get; set; }

    }
    public class Campanhas
    {
        public int? ID { get; set; }
        public string Campanha { get; set; }

        public Baskets baskets { get; set; }

        public Financing financing { get; set; }
        public PrintingServices printingServices { get; set; }
    }
    public class GerarProposta
    {
        public int? ProposalID { get; set; }
        //Proposta Financeira
        public bool Ocultar_PVP_DESCONTO { get; set; } //CheckBox
        public bool Ocultar_PRECOFINAL { get; set; } //CheckBox
        public bool Ocultar_TOTAIS { get; set; } //CheckBox

        public string ObservacoesPropostaFinanceira { get; set; } //TextBox

        //Financiamento
        public double? ValorTransferenciaPropriedade { get; set; } //Inputbox

        public bool? MostrarPeriodoDeContracto { get; set; } //CheckBox
        public string PeriodoDeContracto { get; set; } //DropDown - Depois dou te os valores, para ja podes adicionar "."; "24 Meses"; "36 Meses"

        public bool? Mandatadas_Directo_RetirarDespesas { get; set; } //CheckBox

        public string ObservacoesFinanciamento { get; set; } //TextBox

        //Outras Condiçoes
        public string Prazo_Entega { get; set; } //DropDow - Valores: "Plano em Anexo"; "8 dias"; "15 dias"

        public string ValidadeProposta { get; set; }  //Dropdown valores: "30 dias"; "60 dias";

        public string ObservacoesOutrasCondiçoes { get; set; } //textbox
    }

    public class DeliveryLocation


    {
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
        public List<ItemDoBasket> items { get; set; }
    }

    public class ItemDoBasket
    {
        public int? parentIndex { get; set; }
        public string Family { get; set; }
        public string CodeRef { get; set; }
        public string Description { get; set; }
        public double? UnitPriceCost { get; set; }
        public int? Qty { get; set; }
        public double? TotalCost { get; set; }
        public double? Margin { get; set; }
        public double? PVP { get; set; }
        public double? TotalPVP { get; set; }
        public double? DiscountPercentage { get; set; }
        public double? UnitDiscountPrice { get; set; }
        public double? GPTotal { get; set; }
        public double? GPPercentage { get; set; }
        public double? TotalNetsale { get; set; }
        public bool? IsFinanced { get; set; }

        public bool? IsMarginBEU { get; set; }

        public double? TCP { get; set; }

        public double? ClickPriceBW { get; set; }
        public double? ClickPriceC { get; set; }
        public List<Counter> counters { get; set; }

        public PsConfig psConfig { get; set; }

        public string Name { get; set; }

        public bool? Locked { get; set; }

        public int? Group { get; set; }

        public Nullable<System.DateTime> DeliveryDate { get; set; }
    }

    public  class Maquinas_Usadas_Gestor
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public string ReservedBy { get; set; }
        public Nullable<System.DateTime> ReservedDateTime { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public string PS_NUS { get; set; }
        public string Codref { get; set; }
        public string Type_MFP { get; set; }
        public string NrSerie { get; set; }
        public Nullable<int> NrCopyBW { get; set; }
        public Nullable<int> NrCopyCOLOR { get; set; }
        public Nullable<bool> IsReserved { get; set; }

        public string Modelo { get; set; }
    }

    public class ShareOportunity
    {
        public int ID { get; set; }
        public string user { get; set; }
        public string initialDate { get; set; }
        public string endDate { get; set; }
        public string isPermanent { get; set; }         //public ProposalRootObject proposalData { get; set; }
        public string PermissionType { get; set; }
        public string CreatedBy { get; set; }
        public Draft Draft { get; set; }
        public Summary Summary { get; set; }
    }
    public class BB_ProposalModel
    {
        public int ID { get; set; }
        public string CRM_QUOTE_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<double> ValueComission { get; set; }
        public Nullable<double> ValueTotal { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string ClientAccountNumber { get; set; }
        public Nullable<int> StatusCRM { get; set; }
        public string AccountManager { get; set; }
        public Nullable<bool> ToDelete { get; set; }
        public Nullable<int> CampaignID { get; set; }
        public string StatusCRM1 { get; set; }
        public Nullable<double> SobreValorizacao { get; set; }
        public Nullable<double> TaxaCopiaTotal { get; set; }
        public Nullable<double> SubTotal { get; set; }

        public string StatusName { get; set; }

        public string Leasedesk { get; set; }


        public string LeasedeskModifiedBy { get; set; }
        public string ClientName { get; set; }

    }
    

    public class SimulationMonthly
    {
        public int contract { get; set; }
        public double factor { get; set; }
        public double rent { get; set; }
        public double manageTotal { get; set; }
        public double monthlyFee { get; set; }

        public double vva { get; set; }
        public double total { get; set; }
    }

    public class SimulationTrimestral
    {
        public int contractT { get; set; }
        public double factorT { get; set; }
        public double rentT { get; set; }
        public double manageTotalT { get; set; }
        public double monthlyFeeT { get; set; }

        public double vvaT { get; set; }
        public double totalT { get; set; }
    }
}
