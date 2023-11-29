using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models.SLAs
{
    public class SLAsProcess
    {
        public int? ProposalId { get; set; }
        public string Quote { get; set; }
        public string Name { get; set; }
        public string Client { get; set; }

        public string Category { get; set; }
        public string State { get; set; }
        public int DeptCatEstimateCfgId { get; set; }
        public int OpResult { get; set; }


        public string AprovacaoServicos { get; set; }
        public string AprovacaoFinanceira { get; set; }
        public string AprovacaoCRM { get; set; }
        public string AguardarInfoCliente { get; set; }
        public string AssContratoKM { get; set; }
        public string AssContratoCliente { get; set; }
        public string EmitirGuias { get; set; }
        public string Entregue { get; set; }
        public string Faturado { get; set; }

    }
}