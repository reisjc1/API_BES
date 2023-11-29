using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ViewModels
{
    namespace Portal_KM.Models.ViewModels.ServiceViewModels
    {
        public class ServiceValidationReply
        {
            public ServiceValidationReply()
            {
                Equipments = new List<ServiceValidationRequestEquipment>();
                OPSImplement = new List<OPSImplement>();
                OPSManage = new List<OPSManage>();
            }
            public int RequestID { get; set; }
            public double Fee { get; set; }
            public string SCObservations { get; set; }
            public string ModifiedBy { get; set; }
            public ICollection<ServiceValidationRequestEquipment> Equipments { get; set; }
            public List<OPSImplement> OPSImplement { get; set; }
            public List<OPSManage> OPSManage { get; set; }

            public bool isApproved { get; set; }
        }

        public class VVAServiceValidationReply : ServiceValidationReply
        {
            public double ApprovedRent { get; set; }
            public double ApprovedExcessClickBW { get; set; }
            public double ApprovedExcessClickC { get; set; }
        }

        public class GlobalClickNoVolumeServiceValidationReply : ServiceValidationReply
        {
            public double ApprovedClickPriceBW { get; set; }
            public double ApprovedClickPriceC { get; set; }
        }
    }
}