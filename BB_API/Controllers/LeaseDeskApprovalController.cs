using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LeaseDeskApprovalController : ApiController
    {

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetFinancingApprovalNotComplete")]
        public IHttpActionResult GetFinancingApprovalNotComplete()
        {

            List<BB_Proposal_FinancingApprovalModel> lstBB_Proposal_FinancingApprovalModel = new List<BB_Proposal_FinancingApprovalModel>();

            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {

                    List<BB_Proposal_FinancingApproval> lstFinancingApproval = db.BB_Proposal_FinancingApproval.Where(x => x.IsComplete == false).ToList();

                    foreach (var item in lstFinancingApproval)
                    {
                        string ProdutoFinanceiro = "";

                        using (var db1 = new BB_DB_DEVEntities2())
                        {

                            var config1 = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<BB_Proposal_FinancingApproval, BB_Proposal_FinancingApprovalModel>();
                            });

                            IMapper iMapper1 = config1.CreateMapper();

                            BB_Proposal_FinancingApprovalModel b = iMapper1.Map<BB_Proposal_FinancingApproval, BB_Proposal_FinancingApprovalModel>(item);


                            BB_Proposal pro = db1.BB_Proposal.Where(x => x.ID == item.ProposalID).FirstOrDefault();

                            b.ClienteName = db1.BB_Clientes.Where(x => x.accountnumber == pro.ClientAccountNumber).Select(x => x.Name).FirstOrDefault();

                            lstBB_Proposal_FinancingApprovalModel.Add(b);

                        }

                    }

                }


            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return Ok(lstBB_Proposal_FinancingApprovalModel);
        }
    }
}
