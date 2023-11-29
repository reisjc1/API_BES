using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.SLAs
{
    public class SLAsDetails
    {
        public int ID { get; set; }
        public int ProposalId { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
        public string Quote { get; set; }
        public string Client { get; set; }
        public string AccountManager { get; set; }
        public string DSO { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public int CategoriaID { get; set; }
        public int SLA_To_Meet { get; set; }
        public string Responsable { get; set; }
        public string Cor { get; set; }
        public int DuracaoAdjuc { get; set; }
        public int DuracaoTotal { get; set; }
        public int TempoRealConsumido { get; set; }

        public List<Categoria> Categorias { get;set; }
        public List<BB_SLA_History> ListComentarios { get; set; }

    }

    public class Categoria
    {
        public string Name { get; set; }
        public int SLA_To_Meet { get; set; }
        public string Cor { get; set; }
        public string Responsable { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
        public int TempoRealConsumido { get; set; }
        public int CategoriaID { get; set; }
        
    }

    //public class SLAComentario
    //{
    //    public int ID { get; set; }
    //    public string QuoteNumber { get; set; }
    //    public int CategoriaID { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public string CreatedBy { get; set; }

    //}
}
