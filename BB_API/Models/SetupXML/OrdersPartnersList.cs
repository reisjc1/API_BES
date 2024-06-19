using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML
{
    public class OrdersPartnersList
    {
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS> z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERs = new System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ORDERS>();
        public List<OrdersPartners> SdDocOrderPartner = new List<OrdersPartners>();
    }
}