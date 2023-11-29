using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class HomepageModel
    {
        public string createdbyname { get; set; }
        public string customeridname { get; set; }
        public string estimatedclosedate { get; set; }
        public string modifiedbyyominame { get; set; }
        public string name { get; set; }
        public string new_primaryquoteidname { get; set; }
        public string new_currentbeusalesstagename { get; set; }
        public string new_currentlocalsalesstagename { get; set; }
        public string new_opportunitynumber { get; set; }
        public string new_primaryquotenumber { get; set; }
        public string new_opportunitytypename { get; set; }
        public string new_probabilityname { get; set; }
        public string new_quotestatusreasonname { get; set; }
        public string stepname { get; set; }
        public string statuscodename { get; set; }
    }

    public class Model_sp_Get_CRM_Oportunity_Status
    {
       public string CurrentStatus { get; set; }
        public string TotalCount { get; set; }
    }

    public class Model_sp_Get_CRM_MyOportunity_Status
    {
        public string CurrentStatus { get; set; }
        public string TotalCount { get; set; }
    }

    public class Model_sp_Get_CRM_Status
    {
        public string CurrentStatus { get; set; }
        public string TotalCount { get; set; }
    }

    
}