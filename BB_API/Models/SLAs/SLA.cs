using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.SLAs
{
    public class SLA
    {
        public string SLA_State { get; set; }
        public int? SLA_ToMeet { get; set; }
        public string SLA_Color { get; set; }
        public string SLA_Responsable { get; set; }
        public DateTime SLA_InitialDate { get; set; }
        public DateTime SLA_FinalDate { get; set; }

    }
}