using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class WorkflowAprovacoesController : ApiController
    {
        [AcceptVerbs("POST")]
        [ActionName("CreateMilestone")]
        // Chamar dentro "ProposalDraftSave" do ProposalController
        public IHttpActionResult CreateMilestone(int proposalID)
        {
            try
            {
                using (var bd = new BB_DB_DEVEntities2())
                {
                    List<BB_Proposal_Milestone> milestones = bd.BB_Proposal_Quote
                        .Where(x => x.Proposal_ID == proposalID)
                        .ToList().Select(m => new BB_Proposal_Milestone()
                        {
                            Proposal_ID = m.Proposal_ID,
                            Family = m.Family,
                            CodeRef = m.CodeRef,
                            Description = m.Description,
                            UnitPriceCost = m.UnitPriceCost,
                            Qty = m.Qty,
                            TotalCost = m.TotalCost,
                            UnitDiscountPrice = m.UnitDiscountPrice,
                            TotalNetSale = m.TotalNetsale
                        }).ToList();

                    //Se não existir na tabela
                    if (!bd.BB_Proposal_Milestone.Select(m => m.Proposal_ID == proposalID).FirstOrDefault())
                    {
                        bd.BB_Proposal_Milestone.AddRange(milestones);
                    }
                    //Se existir na tabela adiciona ou faz update
                    else
                    {
                        foreach (var m in milestones)
                        {
                            bd.BB_Proposal_Milestone.AddOrUpdate(x => new { x.Proposal_ID, x.CodeRef }, m);
                        }
                    }

                    bd.SaveChanges();

                }
                return Ok();
            }
            catch
            {
                //TODO Criar mensagem de erro para base de dados
                return BadRequest("AAAAAA - porque sim");
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("CreateCRMApprovalHistory")]
        public IHttpActionResult CreateCRMApprovalHistory(int proposalID, string userLogged, string comment)
        {
            try
            {
                List<BB_CRM_Approval_Comments> lstComentarios = new List<BB_CRM_Approval_Comments>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_CRM_Approval_Comments.Add(new BB_CRM_Approval_Comments()
                    {
                        ProposalID = proposalID,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userLogged,
                        Comment = comment
                    });

                    db.SaveChanges();

                    lstComentarios = db.BB_CRM_Approval_Comments.Where(q => q.ProposalID == proposalID).ToList();
                }

                return Ok(lstComentarios);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GettAllMilestones")]
        public Milestone_ProposalPlan GettAllMilestones(int proposalID)
        {
            try
            {
                List<BB_CRM_Approval_Comments> lstComentarios = new List<BB_CRM_Approval_Comments>();
                List<BB_Proposal_Milestone> milestones = new List<BB_Proposal_Milestone>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lstComentarios = db.BB_CRM_Approval_Comments.Where(q => q.ProposalID == proposalID).ToList();

                    milestones = db.BB_Proposal_Milestone.Where(q => q.Proposal_ID == proposalID).ToList();

                }

                Milestone_ProposalPlan CRM_Milestone = new Milestone_ProposalPlan()
                {
                    Milestones = milestones,
                    MilestoneComments = lstComentarios
                };

                return CRM_Milestone;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }


        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("UpdateMilestoneStatus")]
        public IHttpActionResult UpdateMilestoneStatus(int proposalID, string buttonName)
        {
            try
            {
                BB_Milestone_Status lastMilestoneStatus = new BB_Milestone_Status();
                using (var db = new BB_DB_DEVEntities2())
                {
                    lastMilestoneStatus = db.BB_Milestone_Status.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    if(lastMilestoneStatus != null)
                    {
                        lastMilestoneStatus.CreatedDate = DateTime.Now;

                        if (buttonName == "Submeter")
                            lastMilestoneStatus.Status = 1;
                        else if (buttonName == "Pedido de Aprovação")
                        {
                            lastMilestoneStatus.Status = 2;
                        }
                        else
                        {
                            lastMilestoneStatus.Status = 3;
                        }
                        db.SaveChanges();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return NotFound();
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetMilestoneRequests")]
        public IHttpActionResult GetMilestoneRequests(int proposalID)
        {
            try
            {
                // isto só vai buscar um registo
                // falta fazer os inner joins para devolver uma lista
                MilestoneRequest milestonesRequests = new MilestoneRequest();
                using (var db = new BB_DB_DEVEntities2())
                {
                    string client = db.BB_Proposal.Where(p => p.ID == proposalID).FirstOrDefault().ClientAccountNumber;
                    milestonesRequests.Client = db.BB_Clientes.Where(c => c.accountnumber == client).FirstOrDefault().Name;

                    string manager = db.BB_Proposal.Where(p => p.ID == proposalID).FirstOrDefault().AccountManager;

                    string lastComment = db.BB_CRM_Approval_Comments.Where(c => c.ProposalID == proposalID).LastOrDefault().Comment;


                }
                // a Carolina quer que devolva a lista dentro do "Ok()"
                return Ok(milestonesRequests);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return NotFound();
            }
        }

    }

    public class Milestone_ProposalPlan
    {
        public List<BB_Proposal_Milestone> Milestones { get; set; }
        public List<BB_CRM_Approval_Comments> MilestoneComments { get; set; }
    }
}
