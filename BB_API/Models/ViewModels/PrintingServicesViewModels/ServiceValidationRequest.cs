using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ServiceValidationRequest
    {
        public ServiceValidationRequest()
        {
            Equipments = new List<ServiceValidationRequestEquipment>();
            OPSImplement = new List<OPSImplement>();
            OPSManage = new List<OPSManage>();
        }

        public int ID { get; set; }
        public string Type { get; set; }
        public string RequestedBy { get; set; }
        public string SEObservations { get; set; }
        public int ContractDuration { get; set; }
        public SVRClient Client { get; set; }
        public SVRVolumes Volumes { get; set; }
        public ICollection<ServiceValidationRequestEquipment> Equipments { get; set; }
        public List <OPSImplement> OPSImplement { get; set; }
        public List<OPSManage> OPSManage { get; set; }
    }

    public class SVRVolumes
    {
        public int BWVolume { get; set; }
        public int CVolume { get; set; }
    }

    public class SVRClient
    {
        public string ClientAccountNumber { get; set; }
        public string ClientName { get; set; }
        public bool IsNewClient { get; set; }
    }

    public class VVAServiceValidationRequest : ServiceValidationRequest
    {
        public double RequestedPVP { get; set; }
        public double RequestedExcessBWPVP { get; set; }
        public double RequestedExcessCPVP { get; set; }
        public double RecommendedPVP { get; set; }
        public double RecommendedExcessBWPVP { get; set; }
        public double RecommendedExcessCPVP { get; set; }
        public double AverageCostBW { get; set; }
        public double AverageCostC { get; set; }
        public int ExcessBillingFrequency { get; set; }
        public int RentBillingFrequency { get; set; }
    }

    public class GlobalClickNoVolumeServiceValidationRequest : ServiceValidationRequest
    {
        public int PageBillingFrequency { get; set; }
        public double RequestedClickPriceBW { get; set; }
        public double RequestedClickPriceC { get; set; }
        public double RecommendedClickPriceBW { get; set; }
        public double RecommendedClickPriceC { get; set; }
        public double AverageCostBW { get; set; }
        public double AverageCostC { get; set; }
    }

    public class ClickPerModelServiceValidationRequest : ServiceValidationRequest
    {
        public int PageBillingFrequency { get; set; }
        public double AverageCostBW { get; set; }
        public double AverageCostC { get; set; }
    }
}