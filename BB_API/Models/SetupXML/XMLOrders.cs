using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.SetupXML
{
    public class XMLOrders
    {
        public int ID { get; set; }
        public string CodeRef { get; set; }
        public bool? IsUsedMachine { get; set; }
        public string SerialNumber { get; set; }
        public int ItemGroup { get; set; }
        public int IDX { get; set; }
        public string ContactName { get; set; }
        public string ContactSurname { get; set; }
        public string ContactMovil { get; set; }
        public string Schedule { get; set; }
        public string DLFloor { get; set; }
        public string Department { get; set; }
        public string Building { get; set; }
        public string Room { get; set; }
        public string City { get; set; }
        public int TypeOfOrder { get; set; }
        public string Comments { get; set; }

    }
}