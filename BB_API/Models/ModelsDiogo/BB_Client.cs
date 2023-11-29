using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.ModelsDiogo
{
    public class BB_Client
    {
        public int ID { get; set; }
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public string OwnerEmail { get; set; }
        public virtual AspNetUsers Owner { get; set; } //later replaced by an ASPUser
        public int NIF { get; set; }
        //Ask João if there is any way we can get a structure that's similar to that of the NUS Address
        public string Address { get; set; } 
        public string PostalCode { get; set; }
        public string GMA { get; set; }
        public string GMA_Identifier { get; set; }
        public string Holding { get; set; }
        public bool Blocked { get; set; }
        public string Segment { get; set; }
        public string Industry { get; set; }
    }
}