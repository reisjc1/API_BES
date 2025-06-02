using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.SetupXML
{
    public class ConditionPVP
    {
        public double? PVP { get; set; }
        public string ConditionCode { get; set; }
    }

    public class ConditionPVPPerMachine
    {
        public string MachineModel { get; set; }
        public int Group { get; set; }
        public List<ConditionPVP> Conditions { get; set; }

    }

    public class ConditionsTotais
    {
        public List<ConditionPVP> ConditionsTotal { get; set; }
        public List<ConditionPVPPerMachine> ConditionsPerMachine { get; set; }
    }
}