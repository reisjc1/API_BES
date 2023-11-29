using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.SLAs
{
    public class SLAsFromDB
    {
        public int? ProposalId { get; set; }
        public string Quote { get; set; }
        public string Name { get; set; }
        public string Client { get; set; }
        public string State { get; set; }
        public int DeptCatEstimateCfgId { get; set; }
        public int OpResult { get; set; }
        public List<Categoria> Categorias { get; set; }
        public string Category { get; set; }

        //public Dictionary<int, int> Dic { get; set; }

    }

    }