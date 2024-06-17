using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WebApplication1.Models.SetupXML.XSD;

namespace WebApplication1.Models.SetupXML
{
    public class PartnersAdressesList
    {
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_PARTNERS> Partners { get; set; }
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES> Adresses { get; set; }
        public System.Collections.ObjectModel.Collection<Z1ZVOE_DEAL_1IDOCZ1ZVOE_ADDRESSES_ADD> AdressesAdd { get; set; }
    }
}