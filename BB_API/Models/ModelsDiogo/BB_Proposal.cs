using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ModelsDiogo
{
    public class BB_Proposal
    {
        public int ID { get; set; }
        public string CRM_Quote_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StatusID { get; set; }
        public BB_Proposal_Status Status { get; set; }
        public float TotalValue { get; set; }
        public string ClientAccountNumber { get; set; }
        public BB_Client Client { get; set; }
        public string AccountManagerEmail { get; set; }
        public AspNetUsers AccountManager { get; set; }
        public bool ToDelete { get; set; }
        public int CampaignID { get; set; }
    }
}