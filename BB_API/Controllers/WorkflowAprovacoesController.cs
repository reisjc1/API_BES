using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using WebApplication1.Models;
using WebApplication1.App_Start;

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
        [ActionName("Get_BB_RD_WFA_Elements")]
        public List<BB_RD_WFA_Elements> Get_BB_RD_WFA_Elements()
        {
            try
            {
                List<BB_RD_WFA_Elements> lst_BB_RD_WFA_Elements = new List<BB_RD_WFA_Elements>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_RD_WFA_Elements = db.BB_RD_WFA_Elements.ToList();
                }

                return lst_BB_RD_WFA_Elements;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_RD_WFA_BU")]
        public List<BB_RD_WFA_BU> Get_BB_RD_WFA_BU()
        {
            try
            {
                List<BB_RD_WFA_BU> lst_BB_RD_WFA_BU = new List<BB_RD_WFA_BU>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_RD_WFA_BU = db.BB_RD_WFA_BU.ToList();
                }

                return lst_BB_RD_WFA_BU;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_RD_WFA_Approvers")]
        public List<UserInfo> Get_BB_RD_WFA_Approvers()
        {
            try
            {
                List<UserInfo> usersInfo = new List<UserInfo>();
                List<BB_RD_WFA_Approvers> lst_BB_RD_WFA_Approvers = new List<BB_RD_WFA_Approvers>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_RD_WFA_Approvers = db.BB_RD_WFA_Approvers.ToList();
                }

                foreach (var user in lst_BB_RD_WFA_Approvers)
                {
                    using (var dbX = new masterEntities())
                    {
                        usersInfo = dbX.AspNetUsers
                                        .Where(x => x.Id == user.User_ID)
                                        .Select(x => new UserInfo
                                        {
                                            Email = x.Email,
                                            DisplayName = x.DisplayName
                                        })
                                        .ToList();
                    }
                }
                return usersInfo;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        public class UserInfo
        {
            public string Email { get; set; }
            public string DisplayName { get; set; }
            public string Department { get; set; }
        }


        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_RD_WFA_Condition")]
        public List<BB_RD_WFA_Condition> Get_BB_RD_WFA_Condition()
        {
            try
            {
                List<BB_RD_WFA_Condition> lst_BB_RD_WFA_Condition = new List<BB_RD_WFA_Condition>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_RD_WFA_Condition = db.BB_RD_WFA_Condition.ToList();
                }

                return lst_BB_RD_WFA_Condition;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_RD_WFA_Condition_Type")]
        public List<BB_RD_WFA_Condition_Type> Get_BB_RD_WFA_Condition_Type()
        {
            try
            {
                List<BB_RD_WFA_Condition_Type> lst_BB_RD_WFA_Condition_Type = new List<BB_RD_WFA_Condition_Type>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_RD_WFA_Condition_Type = db.BB_RD_WFA_Condition_Type.ToList();
                }

                return lst_BB_RD_WFA_Condition_Type;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_WFA_Levels")]
        public List<BB_WFA_Levels> Get_BB_WFA_Levels()
        {
            try
            {
                List<BB_WFA_Levels> lst_BB_WFA_Levels = new List<BB_WFA_Levels>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_WFA_Levels = db.BB_WFA_Levels.ToList();
                }

                return lst_BB_WFA_Levels;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_WFA_Control")]
        public List<BB_WFA_Control> Get_BB_WFA_Control()
        {
            try
            {
                List<BB_WFA_Control> lst_BB_WFA_Control = new List<BB_WFA_Control>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_WFA_Control = db.BB_WFA_Control.ToList();
                }

                return lst_BB_WFA_Control;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_WFA")]
        public List<BB_WFA> Get_BB_WFA()
        {
            try
            {
                List<BB_WFA> lst_BB_WFA = new List<BB_WFA>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_WFA = db.BB_WFA.ToList();
                }

                return lst_BB_WFA;
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
                    if (lastMilestoneStatus != null)
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
        public IHttpActionResult ADD_BB_WFA_Levels(int? WFA_Control_ID, string Name, int? WFA_Approver_ID, int? Condition_ID, float? Condition_Value, int? Type_ID)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_WFA_Levels.Add(new BB_WFA_Levels()
                    {
                        WFA_Control_ID = WFA_Control_ID,
                        Name = Name,
                        WFA_Approver_ID = WFA_Approver_ID,
                        Condition_ID = Condition_ID,
                        Condition_Value = Condition_Value,
                        Type_ID = Type_ID
                    });

                    db.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################
        public IHttpActionResult ADD_BB_WFA_Control(int? BU_ID, int? Elements_ID, int? Level_ID)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_WFA_Control.Add(new BB_WFA_Control()
                    {
                        BU_ID = BU_ID,
                        Elements_ID = Elements_ID,
                        Level_ID = Level_ID
                    });

                    db.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################
        public IHttpActionResult ADD_BB_WFA(int? WFA_Control_ID, string Name)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_WFA.Add(new BB_WFA()
                    {
                        WFA_Control_ID = WFA_Control_ID,
                        Name = Name
                    });

                    db.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // ######################################################################################
        /*
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
        */
    }
    public class Milestone_ProposalPlan
    {
        public List<BB_Proposal_Milestone> Milestones { get; set; }
        public List<BB_CRM_Approval_Comments> MilestoneComments { get; set; }
    }
}
