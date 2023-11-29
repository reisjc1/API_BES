using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    public class ServiceRequestHistory
    {
        public ServiceRequestHistory()
        {
            Equipments = new List<ServiceValidationRequestEquipment>();
            OPSImplement = new List<OPSImplement>();
            OPSManage = new List<OPSManage>();
        }

        public int ID { get; set; }
        public string Type { get; set; }
        public int BWVolume { get; set; }
        public int CVolume { get; set; }
        public double Fee { get; set; }
        public int ContractDuration { get; set; }        
        public string RequestedBy { get; set; }
        public string ClientAccountNumber { get; set; }
        public string ClientName { get; set; }
        public bool IsNewClient { get; set; }
        public string SEObservations { get; set; }
        public string SCObservations { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime ApprovedAt { get; set; }
        public ICollection<ServiceValidationRequestEquipment> Equipments { get; set; }
        public List<OPSImplement> OPSImplement { get; set; }
        public List<OPSManage> OPSManage { get; set; }
    }

    public class VVAServiceRequestHistory : ServiceRequestHistory
    {
        public double PVP { get; set; }
        public double ApprovedRent { get; set; }
        public double ApprovedExcessBW { get; set; }
        public double ApprovedExcessC { get; set; }
        public double RequestedRent { get; set; }
        public double RequestedExcessBW { get; set; }
        public double RequestedExcessC { get; set; }
        public double ExcessBWPVP { get; set; }
        public double ExcessCPVP { get; set; }
        public double AverageCostBW { get; set; }
        public double AverageCostC { get; set; }
        public double TotalCost { get; set; }
        public int ExcessBillingFrequency { get; set; }
        public int RentBillingFrequency { get; set; }
    }

    public class NoVolumeServiceRequestHistory : ServiceRequestHistory
    {
        public int PageBillingFrequency { get; set; }
        public double ApprovedExcessBW { get; set; }
        public double ApprovedExcessC { get; set; }
        public double ClickPriceBW { get; set; }
        public double ClickPriceC { get; set; }
        public double AverageCostBW { get; set; }
        public double AverageCostC { get; set; }
        public double TotalCost { get; set; }
    }

    public class ClickPerModelServiceRequestHistory : ServiceRequestHistory
    {
        public int PageBillingFrequency { get; set; }
    }
}