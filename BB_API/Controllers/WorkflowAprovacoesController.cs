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
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Office2010.Excel;
using static WebApplication1.Controllers.WorkflowAprovacoesController;
using AutoMapper;
using WebApplication1.Models.SLAs;
using DocumentFormat.OpenXml.Math;

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
        public List<BB_RD_WFA> Get_BB_WFA()
        {
            try
            {
                List<BB_RD_WFA> lst_BB_WFA = new List<BB_RD_WFA>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_BB_WFA = db.BB_RD_WFA.ToList();
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
        public IHttpActionResult ADD_BB_WFA_Control(int? BU_ID, int? Elements_ID, int? Level_ID, string TypeOfCustomer)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_WFA_Control.Add(new BB_WFA_Control()
                    {
                        BU_ID = BU_ID,
                        Elements_ID = Elements_ID
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
                    db.BB_RD_WFA.Add(new BB_RD_WFA()
                    {
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
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetWFATable")]
        public IHttpActionResult GetWFATable()
        {
            try
            {
                List<WFA_Listagem> WFA_lst = new List<WFA_Listagem>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_WFA_Control> WFA_Control_lst = db.BB_WFA_Control.ToList();

                    foreach (var item in WFA_Control_lst)
                    {
                        List<BB_WFA_Levels> wfa_levels_lst = db.BB_WFA_Levels.Where(x => x.WFA_Control_ID == item.ID).ToList();


                        WFA_Listagem wfa = new WFA_Listagem()
                        {
                            Line = item.Line_ID,
                            BU = db.BB_RD_WFA_BU.Where(x => x.ID == item.BU_ID).Select(x => x.Description).FirstOrDefault(),
                            DealElements = db.BB_RD_WFA_Elements.Where(x => x.ID == item.Elements_ID).Select(x => x.Description).FirstOrDefault(),
                            lstLevel = new List<Level>()
                        };

                        foreach (BB_WFA_Levels level in wfa_levels_lst)
                        {
                            using (var dbX = new masterEntities())
                            {
                                int? approverID = db.BB_WFA_Levels
                                                    .Where(x => x.WFA_Control_ID == item.ID && x.Level == level.Level)
                                                    .Select(x => x.WFA_Approver_ID).FirstOrDefault();

                                string userID = db.BB_RD_WFA_Approvers.Where(x => x.ID == approverID).Select(x => x.User_ID).FirstOrDefault();

                                string userName = dbX.AspNetUsers.Where(x => x.Id == userID).Select(x => x.DisplayName).FirstOrDefault();

                                Level levelX = new Level()
                                {
                                    Approver = userName,
                                    Condition = db.BB_RD_WFA_Condition.Where(x => x.ID == level.Condition_ID).Select(x => x.Condition).FirstOrDefault() + " " + level.Condition_Value,
                                    Type = db.BB_RD_WFA_Condition_Type.Where(x => x.ID == level.Type_ID).Select(x => x.Description).FirstOrDefault()
                                };

                                wfa.lstLevel.Add(levelX);
                            }

                        }
                        WFA_lst.Add(wfa);
                    }
                }

                return Ok(WFA_lst);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCreateDropdowns")]
        public IHttpActionResult GetCreateDropdowns()
        {
            try
            {
                WFA_Create wfa_create_obj = new WFA_Create();
                List<BB_RD_WFA_Approvers> approversList = new List<BB_RD_WFA_Approvers>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    // Popular a lista dos approvers do objeto WFA_Create
                    approversList = db.BB_RD_WFA_Approvers.ToList();


                    // Popular o resto das dropdowns
                    wfa_create_obj.Lst_Condition = db.BB_RD_WFA_Condition.ToList();
                    wfa_create_obj.Lst_Type = db.BB_RD_WFA_Condition_Type.ToList();
                    wfa_create_obj.Lst_BU = db.BB_RD_WFA_BU.ToList();
                    wfa_create_obj.Lst_DealElements = db.BB_RD_WFA_Elements.ToList();


                    wfa_create_obj.Lst_Approver = new List<WFA_Approvers>();
                    using (var dbX = new masterEntities())
                    {
                        foreach (var approver in approversList)
                        {
                            string userName = dbX.AspNetUsers.Where(x => x.Id == approver.User_ID).Select(x => x.DisplayName).FirstOrDefault();

                            WFA_Approvers approverX = new WFA_Approvers()
                            {
                                ID = approver.ID,
                                User_ID = approver.User_ID,
                                Name = userName
                            };
                            // Popular a lista dos approvers do objeto WFA_Create
                            wfa_create_obj.Lst_Approver.Add(approverX);
                        }
                    }

                    return Ok(wfa_create_obj);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("AddNewWFALine")]
        public IHttpActionResult AddNewWFALine(WFA_Create newLine)
        {
            try
            {
                WFA_Create wfa_create_obj = new WFA_Create();

                using (var db = new BB_DB_DEVEntities2())
                {
                    //HARCODED 1 NO WFA_ID, DEVERÁ SER RECEBIDO POR PARAMETRO
                    var aux = db.BB_WFA_Control.Where(w => w.WFA_ID == 1)
                                               .OrderByDescending(x => x.Line_ID)
                                               .Select(x => x.Line_ID)
                                               .ToList();


                    int? lastLine = db.BB_WFA_Control.Any() ? aux.FirstOrDefault() : 0;

                    BB_WFA_Control bb_wfa_control = new BB_WFA_Control()
                    {
                        WFA_ID = 1,
                        Line_ID = lastLine + 1,
                        BU_ID = newLine.BU,
                        Elements_ID = newLine.DealElement,
                    };

                    if (bb_wfa_control.BU_ID != null && bb_wfa_control.Elements_ID != null)
                    {
                        db.BB_WFA_Control.Add(bb_wfa_control);
                        db.SaveChanges();

                        //ADICIONAR LEVELS.....

                        BB_WFA_Levels bb_wfa_level = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            Level = 1,
                            WFA_Approver_ID = newLine.Level1_Approver,
                            Condition_ID = newLine.Level1_Condition,
                            Condition_Value = newLine.Percentage_1,
                            Type_ID = newLine.Level1_Type,
                        };

                        BB_WFA_Levels bb_wfa_level_2 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            Level = 2,
                            WFA_Approver_ID = newLine.Level2_Approver,
                            Condition_ID = newLine.Level2_Condition,
                            Condition_Value = newLine.Percentage_2,
                            Type_ID = newLine.Level2_Type
                        };

                        BB_WFA_Levels bb_wfa_level_3 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            Level = 3,
                            WFA_Approver_ID = newLine.Level3_Approver,
                            Condition_ID = newLine.Level3_Condition,
                            Condition_Value = newLine.Percentage_3,
                            Type_ID = newLine.Level3_Type
                        };

                        BB_WFA_Levels bb_wfa_level_4 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            Level = 4,
                            WFA_Approver_ID = newLine.Level4_Approver,
                            Condition_ID = newLine.Level4_Condition,
                            Condition_Value = newLine.Percentage_4,
                            Type_ID = newLine.Level4_Type
                        };

                        BB_WFA_Levels bb_wfa_level_5 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            Level = 5,
                            WFA_Approver_ID = newLine.Level5_Approver,
                            Condition_ID = newLine.Level5_Condition,
                            Condition_Value = newLine.Percentage_5,
                            Type_ID = newLine.Level5_Type
                        };


                        // VERIFICAR SE OS LEVELS TÊM TUDO PREENCHIDO...

                        if (bb_wfa_level.WFA_Control_ID != null && bb_wfa_level.WFA_Approver_ID != null && bb_wfa_level.Condition_ID != null &&
                            bb_wfa_level.Condition_Value != null && bb_wfa_level.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level);
                        }

                        if (bb_wfa_level_2.WFA_Control_ID != null && bb_wfa_level_2.WFA_Approver_ID != null && bb_wfa_level_2.Condition_ID != null &&
                            bb_wfa_level_2.Condition_Value != null && bb_wfa_level_2.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_2);
                        }
                        if (bb_wfa_level_3.WFA_Control_ID != null && bb_wfa_level_3.WFA_Approver_ID != null && bb_wfa_level_3.Condition_ID != null &&
                            bb_wfa_level_3.Condition_Value != null && bb_wfa_level_3.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_3);
                        }
                        if (bb_wfa_level_4.WFA_Control_ID != null && bb_wfa_level_4.WFA_Approver_ID != null && bb_wfa_level_4.Condition_ID != null &&
                            bb_wfa_level_4.Condition_Value != null && bb_wfa_level_4.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_4);
                        }
                        if (bb_wfa_level_5.WFA_Control_ID != null && bb_wfa_level_5.WFA_Approver_ID != null && bb_wfa_level_5.Condition_ID != null &&
                            bb_wfa_level_5.Condition_Value != null && bb_wfa_level_5.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_5);
                        }
                    }

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

        //####################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("EditWFALine")]
        public IHttpActionResult EditWFALine(int lineNr)
        {
            try
            {
                WFA_Create wfa_obj = GetWFAWithDropdowns();

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_WFA_Control bb_wfa_control = db.BB_WFA_Control.Where(x => x.Line_ID == lineNr).FirstOrDefault();

                    // procurar toda a info sobre o objeto
                    wfa_obj.ID = bb_wfa_control.ID;
                    wfa_obj.BU = bb_wfa_control.BU_ID;
                    wfa_obj.DealElement = bb_wfa_control.Elements_ID;

                    List<BB_WFA_Levels> bb_wfa_levels = db.BB_WFA_Levels.Where(x => x.WFA_Control_ID == bb_wfa_control.ID).ToList();

                    for (int i = 0; i < bb_wfa_levels.Count(); i++)
                    {
                        if (bb_wfa_levels[i].WFA_Control_ID != null)
                        {
                            switch (i)
                            {
                                case 0:
                                    wfa_obj.Level1_Approver = bb_wfa_levels[i].WFA_Approver_ID;
                                    wfa_obj.Level1_Condition = bb_wfa_levels[i].Condition_ID;
                                    wfa_obj.Level1_Type = bb_wfa_levels[i].Type_ID;
                                    wfa_obj.Percentage_1 = (int?)bb_wfa_levels[i].Condition_Value;
                                    break;
                                case 1:
                                    wfa_obj.Level2_Approver = bb_wfa_levels[i].WFA_Approver_ID;
                                    wfa_obj.Level2_Condition = bb_wfa_levels[i].Condition_ID;
                                    wfa_obj.Level2_Type = bb_wfa_levels[i].Type_ID;
                                    wfa_obj.Percentage_2 = (int?)bb_wfa_levels[i].Condition_Value;
                                    break;
                                case 2:
                                    wfa_obj.Level3_Approver = bb_wfa_levels[i].WFA_Approver_ID;
                                    wfa_obj.Level3_Condition = bb_wfa_levels[i].Condition_ID;
                                    wfa_obj.Level3_Type = bb_wfa_levels[i].Type_ID;
                                    wfa_obj.Percentage_3 = (int?)bb_wfa_levels[i].Condition_Value;
                                    break;
                                case 3:
                                    wfa_obj.Level4_Approver = bb_wfa_levels[i].WFA_Approver_ID;
                                    wfa_obj.Level4_Condition = bb_wfa_levels[i].Condition_ID;
                                    wfa_obj.Level4_Type = bb_wfa_levels[i].Type_ID;
                                    wfa_obj.Percentage_4 = (int?)bb_wfa_levels[i].Condition_Value;
                                    break;
                                case 4:
                                    wfa_obj.Level5_Approver = bb_wfa_levels[i].WFA_Approver_ID;
                                    wfa_obj.Level5_Condition = bb_wfa_levels[i].Condition_ID;
                                    wfa_obj.Level5_Type = bb_wfa_levels[i].Type_ID;
                                    wfa_obj.Percentage_5 = (int?)bb_wfa_levels[i].Condition_Value;
                                    break;
                            }
                        }
                    }


                }

                return Ok(wfa_obj);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("AddNewWFALineException")]
        public IHttpActionResult AddNewWFALineException(WFA_CreateException WFA_Create_Exception)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    if (!db.BB_WFA_Exception.Where(x => x.Line_ID == WFA_Create_Exception.LineNr).Any())
                    {
                        //criar o objeto da exceção, com base no que é passado por parâmero
                        BB_WFA_Exception exception = new BB_WFA_Exception()
                        {
                            Line_ID = WFA_Create_Exception.LineNr,
                            Type_ID = WFA_Create_Exception.Type,
                            Condition_ID = WFA_Create_Exception.Condition,
                            Condition_Value = WFA_Create_Exception.Condition_Value,
                            Action_ID = WFA_Create_Exception.Action,
                            Level_ID = WFA_Create_Exception.LevelNr
                        };

                        //verificar se está tudo preenchido
                        if (exception.Line_ID != null && exception.Type_ID != null && exception.Condition_ID != null &&
                            exception.Condition_Value != null && exception.Action_ID != null && exception.Level_ID != null)
                        {

                            //adicionar à BD
                            db.BB_WFA_Exception.Add(exception);
                            db.SaveChanges();
                        }

                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        //####################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("EditWFAExceptiom")]
        public IHttpActionResult EditWFAExceptiom(int lineNr)
        {
            try
            {
                WFA_CreateException wfa_exception_obj = GetWFAExceptionWithDropdowns();

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_WFA_Exception bb_wfa_exception = db.BB_WFA_Exception.Where(x => x.Line_ID == lineNr).FirstOrDefault();

                    wfa_exception_obj.ID = bb_wfa_exception.ID;
                    wfa_exception_obj.LevelNr = bb_wfa_exception.Level_ID;
                    wfa_exception_obj.LineNr = bb_wfa_exception.Line_ID;
                    wfa_exception_obj.Condition_Value = bb_wfa_exception.Condition_Value;

                    wfa_exception_obj.Type = db.BB_RD_WFA_Condition_Type.Where(x => x.ID == bb_wfa_exception.Type_ID).Select(x => x.ID).FirstOrDefault();
                    wfa_exception_obj.Condition = db.BB_RD_WFA_Condition.Where(x => x.ID == bb_wfa_exception.Condition_ID).Select(x => x.ID).FirstOrDefault();
                    wfa_exception_obj.Action = db.BB_RD_WFA_Exception_Action.Where(x => x.ID == bb_wfa_exception.Action_ID).Select(x => x.ID).FirstOrDefault();
                }

                return Ok(wfa_exception_obj);
            }

            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        //####################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("InitiateWFAProcess")]

        public IHttpActionResult InitiateWFAProcess(int ProposalID)
        {
            string message = "";
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_WFA_Workflow_Proposal> checkExistent = db.BB_WFA_Workflow_Proposal
                                                          .Where(w => w.Proposal_ID == ProposalID && w.Finished == false)
                                                          .ToList();

                    //Verifica se já existe um pedido iniciado mas não terminado.
                    //Termina o método retornando uma mensagem para o user
                    if (checkExistent.Find(x => x.Started == true) != null)
                    {
                        message = "Ya hay un proceso en marcha. Tendrá que esperar una respuesta.";
                        return Ok(message);
                    }
                    //Verifica se existe um pedido criado mas não iniciado.
                    //Salta para a chamada da SP
                    else if (checkExistent.Find(x => x.Started == false) != null)
                    {
                        if (CallWFASP(ProposalID, 1))
                        {
                            message = "Proceso creado e iniciado con éxito.";
                            return Ok(message);
                        }
                        else
                        {
                            message = "El proceso se creó, pero hubo un problema al iniciarlo." +
                                      "Vuelva a realizar el pedido. Si el error persiste, póngase en contacto con el departamento de Business Builder.";
                            return Ok(message);
                        }
                    }

                    //Criação do objeto e inserção na BD
                    BB_WFA_Workflow_Proposal toInsert = new BB_WFA_Workflow_Proposal()
                    {
                        Proposal_ID = ProposalID,
                        Created_Date = DateTime.Now,
                        Started = false,
                        Finished = false,
                        IsApproved = false,
                        IsCompleted = false
                    };

                    db.BB_WFA_Workflow_Proposal.Add(toInsert);
                    db.SaveChanges();
                }

                if (CallWFASP(ProposalID, 1))
                {
                    message = "Proceso creado e iniciado con éxito.";
                    return Ok(message);
                }
                else
                {
                    message = "El proceso se creó, pero hubo un problema al iniciarlo." +
                              "Vuelva a realizar el pedido. Si el error persiste, póngase en contacto con el departamento de Business Builder.";
                    return Ok(message);
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return null;
            }
        }


        //####################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("DeleteWFAExceptiom")]
        public IHttpActionResult DeleteWFAExceptiom(int lineNr)
        {
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_WFA_Exception bb_wfa_exception = db.BB_WFA_Exception.Where(x => x.Line_ID == lineNr).FirstOrDefault();

                    db.BB_WFA_Exception.Remove(bb_wfa_exception);
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

        // HELPERS --------------------------------------------------------------------------------------------

        public WFA_Create GetWFAWithDropdowns()
        {
            try
            {
                WFA_Create wfa_obj = new WFA_Create();
                List<BB_RD_WFA_Approvers> approversList = new List<BB_RD_WFA_Approvers>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    // Popular a lista dos approvers do objeto WFA_Create
                    approversList = db.BB_RD_WFA_Approvers.ToList();


                    // Popular o resto das dropdowns
                    wfa_obj.Lst_Condition = db.BB_RD_WFA_Condition.ToList();
                    wfa_obj.Lst_Type = db.BB_RD_WFA_Condition_Type.ToList();
                    wfa_obj.Lst_BU = db.BB_RD_WFA_BU.ToList();
                    wfa_obj.Lst_DealElements = db.BB_RD_WFA_Elements.ToList();


                    wfa_obj.Lst_Approver = new List<WFA_Approvers>();
                    using (var dbX = new masterEntities())
                    {
                        foreach (var approver in approversList)
                        {
                            string userName = dbX.AspNetUsers.Where(x => x.Id == approver.User_ID).Select(x => x.DisplayName).FirstOrDefault();

                            WFA_Approvers approverX = new WFA_Approvers()
                            {
                                ID = approver.ID,
                                User_ID = approver.User_ID,
                                Name = userName
                            };
                            // Popular a lista dos approvers do objeto WFA_Create
                            wfa_obj.Lst_Approver.Add(approverX);
                        }
                    }

                    return wfa_obj;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        private bool CallWFASP(int ProposalID, int WFA_ID)
        {
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_WFA_Proposal_Control", conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Proposal_ID", ProposalID);
                    cmd.Parameters.AddWithValue("@WFA_ID", WFA_ID);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    bool result = rdr.Read();
                    rdr.Close();
                    return result;

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }
        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveEditLine")]
        public IHttpActionResult SaveEditLine(WFA_Create newLine)
        {
            try
            {

                WFA_Create wfa_create_obj = new WFA_Create();

                using (var db = new BB_DB_DEVEntities2())
                {
                    // apagar os registos anteriores

                    BB_WFA_Control savedWFA = db.BB_WFA_Control.Where(x => x.ID == newLine.ID).FirstOrDefault();

                    int savedLineNr = (int)savedWFA.Line_ID;

                    List<BB_WFA_Levels> levelsToDelete = db.BB_WFA_Levels.Where(x => x.WFA_Control_ID == savedWFA.ID).ToList();

                    db.BB_WFA_Levels.RemoveRange(levelsToDelete);
                    db.SaveChanges();
                    db.BB_WFA_Control.Remove(savedWFA);
                    db.SaveChanges();


                    BB_WFA_Control bb_wfa_control = new BB_WFA_Control()
                    {
                        WFA_ID = 1,
                        Line_ID = savedLineNr,
                        BU_ID = newLine.BU,
                        Elements_ID = newLine.DealElement,
                    };

                    if (bb_wfa_control.BU_ID != null && bb_wfa_control.Elements_ID != null)
                    {
                        db.BB_WFA_Control.Add(bb_wfa_control);
                        db.SaveChanges();

                        //ADICIONAR LEVELS.....

                        BB_WFA_Levels bb_wfa_level = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            WFA_Approver_ID = newLine.Level1_Approver,
                            Condition_ID = newLine.Level1_Condition,
                            Condition_Value = newLine.Percentage_1,
                            Type_ID = newLine.Level1_Type,
                        };

                        BB_WFA_Levels bb_wfa_level_2 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            WFA_Approver_ID = newLine.Level2_Approver,
                            Condition_ID = newLine.Level2_Condition,
                            Condition_Value = newLine.Percentage_2,
                            Type_ID = newLine.Level2_Type
                        };

                        BB_WFA_Levels bb_wfa_level_3 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            WFA_Approver_ID = newLine.Level3_Approver,
                            Condition_ID = newLine.Level3_Condition,
                            Condition_Value = newLine.Percentage_3,
                            Type_ID = newLine.Level3_Type
                        };

                        BB_WFA_Levels bb_wfa_level_4 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            WFA_Approver_ID = newLine.Level4_Approver,
                            Condition_ID = newLine.Level4_Condition,
                            Condition_Value = newLine.Percentage_4,
                            Type_ID = newLine.Level4_Type
                        };

                        BB_WFA_Levels bb_wfa_level_5 = new BB_WFA_Levels()
                        {
                            WFA_Control_ID = bb_wfa_control.ID,
                            WFA_Approver_ID = newLine.Level5_Approver,
                            Condition_ID = newLine.Level5_Condition,
                            Condition_Value = newLine.Percentage_5,
                            Type_ID = newLine.Level5_Type
                        };


                        // VERIFICAR SE OS LEVELS TÊM TUDO PREENCHIDO...

                        if (bb_wfa_level.WFA_Control_ID != null && bb_wfa_level.WFA_Approver_ID != null && bb_wfa_level.Condition_ID != null &&
                            bb_wfa_level.Condition_Value != null && bb_wfa_level.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level);
                        }

                        if (bb_wfa_level_2.WFA_Control_ID != null && bb_wfa_level_2.WFA_Approver_ID != null && bb_wfa_level_2.Condition_ID != null &&
                            bb_wfa_level_2.Condition_Value != null && bb_wfa_level_2.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_2);
                        }
                        if (bb_wfa_level_3.WFA_Control_ID != null && bb_wfa_level_3.WFA_Approver_ID != null && bb_wfa_level_3.Condition_ID != null &&
                            bb_wfa_level_3.Condition_Value != null && bb_wfa_level_3.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_3);
                        }
                        if (bb_wfa_level_4.WFA_Control_ID != null && bb_wfa_level_4.WFA_Approver_ID != null && bb_wfa_level_4.Condition_ID != null &&
                            bb_wfa_level_4.Condition_Value != null && bb_wfa_level_4.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_4);
                        }
                        if (bb_wfa_level_5.WFA_Control_ID != null && bb_wfa_level_5.WFA_Approver_ID != null && bb_wfa_level_5.Condition_ID != null &&
                            bb_wfa_level_5.Condition_Value != null && bb_wfa_level_5.Type_ID != null)
                        {
                            db.BB_WFA_Levels.Add(bb_wfa_level_5);
                        }
                    }

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

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetWFAExceptions")]
        public IHttpActionResult GetWFAExceptions()
        {
            try
            {
                List<BB_WFA_Exception> wfa_ex_lst = new List<BB_WFA_Exception>();
                List<BB_WFA_Exception_Translated> wfa_ex_translated_lst = new List<BB_WFA_Exception_Translated>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    wfa_ex_lst = db.BB_WFA_Exception.ToList();

                    foreach (var exception in wfa_ex_lst)
                    {
                        BB_WFA_Exception_Translated translated_ex = new BB_WFA_Exception_Translated()
                        {
                            ID = exception.ID,
                            WFA_Control_ID = exception.WFA_Control_ID,
                            Line_ID = exception.Line_ID,
                            Level_ID = exception.Level_ID,
                            Condition_Value = exception.Condition_Value,
                            Action_ID = db.BB_RD_WFA_Exception_Action.Where(x => x.ID == exception.Action_ID).Select(x => x.Name).FirstOrDefault(),
                            Condition_ID = db.BB_RD_WFA_Condition.Where(x => x.ID == exception.Condition_ID).Select(x => x.Condition).FirstOrDefault(),
                            Type_ID = db.BB_RD_WFA_Condition_Type.Where(x => x.ID == exception.Type_ID).Select(x => x.Description).FirstOrDefault(),
                        };

                        wfa_ex_translated_lst.Add(translated_ex);
                    }
                }

                return Ok(wfa_ex_translated_lst);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCreateWFAExceptionDropdowns")]
        public IHttpActionResult GetCreateWFAExceptionDropdowns()
        {
            try
            {
                WFA_CreateException wfa_create_exception_obj = new WFA_CreateException();

                using (var db = new BB_DB_DEVEntities2())
                {
                    // Popular as dropdowns
                    wfa_create_exception_obj.Lst_LineNr = db.BB_WFA_Control.OrderBy(l => l.Line_ID).Select(x => x.Line_ID).ToList();
                    wfa_create_exception_obj.Lst_Type = db.BB_RD_WFA_Condition_Type.ToList();
                    wfa_create_exception_obj.Lst_Condition = db.BB_RD_WFA_Condition.ToList();
                    wfa_create_exception_obj.Lst_Action = db.BB_RD_WFA_Exception_Action.ToList();

                    return Ok(wfa_create_exception_obj);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################
        public WFA_CreateException GetWFAExceptionWithDropdowns()
        {
            try
            {
                WFA_CreateException wfa_exception_obj = new WFA_CreateException();

                using (var db = new BB_DB_DEVEntities2())
                {

                    // Popular o resto das dropdowns
                    wfa_exception_obj.Lst_LineNr = db.BB_WFA_Exception.Select(x => x.Line_ID).ToList();
                    wfa_exception_obj.Lst_Action = db.BB_RD_WFA_Exception_Action.ToList();
                    wfa_exception_obj.Lst_Condition = db.BB_RD_WFA_Condition.ToList();
                    wfa_exception_obj.Lst_Type = db.BB_RD_WFA_Condition_Type.ToList();

                    return wfa_exception_obj;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetWFAExceptionTranslated")]
        public IHttpActionResult GetWFAExceptionTranslated(int lineNr)
        {
            try
            {
                BB_WFA_Exception wfa_ex_lst = new BB_WFA_Exception();
                BB_WFA_Exception_Translated wfa_ex_translated_lst = new BB_WFA_Exception_Translated();

                using (var db = new BB_DB_DEVEntities2())
                {
                    wfa_ex_lst = db.BB_WFA_Exception.Where(x => x.Line_ID == lineNr).FirstOrDefault();

                    wfa_ex_translated_lst.ID = wfa_ex_lst.ID;
                    wfa_ex_translated_lst.WFA_Control_ID = wfa_ex_lst.WFA_Control_ID;
                    wfa_ex_translated_lst.Line_ID = wfa_ex_lst.Line_ID;
                    wfa_ex_translated_lst.Level_ID = wfa_ex_lst.Level_ID;
                    wfa_ex_translated_lst.Condition_Value = wfa_ex_lst.Condition_Value;
                    wfa_ex_translated_lst.Action_ID = db.BB_RD_WFA_Exception_Action.Where(x => x.ID == wfa_ex_lst.Action_ID).Select(x => x.Name).FirstOrDefault();
                    wfa_ex_translated_lst.Condition_ID = db.BB_RD_WFA_Condition.Where(x => x.ID == wfa_ex_lst.Action_ID).Select(x => x.Condition).FirstOrDefault();
                    wfa_ex_translated_lst.Type_ID = db.BB_RD_WFA_Condition_Type.Where(x => x.ID == wfa_ex_lst.Action_ID).Select(x => x.Description).FirstOrDefault();
                }

                return Ok(wfa_ex_translated_lst);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveEditException")]
        public IHttpActionResult SaveEditException(WFA_CreateException exceptionToEdit)
        {
            try
            {
                WFA_Create wfa_create_obj = new WFA_Create();

                using (var db = new BB_DB_DEVEntities2())
                {
                    // apagar os registos anteriores

                    BB_WFA_Exception savedException = db.BB_WFA_Exception.Where(x => x.Line_ID == exceptionToEdit.LineNr).FirstOrDefault();

                    db.BB_WFA_Exception.Remove(savedException);
                    db.SaveChanges();

                    BB_WFA_Exception newException = new BB_WFA_Exception()
                    {
                        Line_ID = exceptionToEdit.LineNr,
                        Type_ID = exceptionToEdit.Type,
                        Condition_ID = exceptionToEdit.Condition,
                        Condition_Value = exceptionToEdit.Condition_Value,
                        Action_ID = exceptionToEdit.Action,
                        Level_ID = exceptionToEdit.LevelNr
                    };

                    db.BB_WFA_Exception.Add(newException);
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

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetWFAAndExceptions")]
        public IHttpActionResult GetWFAAndExceptions()
        {
            WFA_And_Exceptions wfa_and_exceptions = new WFA_And_Exceptions();
            List<WFA_Listagem> WFA_lst = new List<WFA_Listagem>();
            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_WFA_Control> WFA_Control_lst = db.BB_WFA_Control.ToList();

                    foreach (var item in WFA_Control_lst)
                    {
                        List<BB_WFA_Levels> wfa_levels_lst = db.BB_WFA_Levels.Where(x => x.WFA_Control_ID == item.ID).ToList();


                        WFA_Listagem wfa = new WFA_Listagem()
                        {
                            Line = item.Line_ID,
                            BU = db.BB_RD_WFA_BU.Where(x => x.ID == item.BU_ID).Select(x => x.Description).FirstOrDefault(),
                            DealElements = db.BB_RD_WFA_Elements.Where(x => x.ID == item.Elements_ID).Select(x => x.Description).FirstOrDefault(),
                            lstLevel = new List<Level>()
                        };

                        foreach (BB_WFA_Levels level in wfa_levels_lst)
                        {
                            using (var dbX = new masterEntities())
                            {
                                int? approverID = db.BB_WFA_Levels
                                                    .Where(x => x.WFA_Control_ID == item.ID && x.Level == level.Level)
                                                    .Select(x => x.WFA_Approver_ID).FirstOrDefault();

                                string userID = db.BB_RD_WFA_Approvers.Where(x => x.ID == approverID).Select(x => x.User_ID).FirstOrDefault();

                                string userName = dbX.AspNetUsers.Where(x => x.Id == userID).Select(x => x.DisplayName).FirstOrDefault();

                                Level levelX = new Level()
                                {
                                    Approver = userName,
                                    Condition = db.BB_RD_WFA_Condition.Where(x => x.ID == level.Condition_ID).Select(x => x.Condition).FirstOrDefault() + " " + level.Condition_Value,
                                    Type = db.BB_RD_WFA_Condition_Type.Where(x => x.ID == level.Type_ID).Select(x => x.Description).FirstOrDefault()
                                };

                                wfa.lstLevel.Add(levelX);
                            }

                        }
                        WFA_lst.Add(wfa);
                    }
                }
                wfa_and_exceptions.Lst_WFA = WFA_lst;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }


            // EXCEÇÕES
            try
            {
                List<BB_WFA_Exception> wfa_ex_lst = new List<BB_WFA_Exception>();
                List<BB_WFA_Exception_Translated> wfa_ex_translated_lst = new List<BB_WFA_Exception_Translated>();

                using (var db = new BB_DB_DEVEntities2())
                {
                    wfa_ex_lst = db.BB_WFA_Exception.ToList();

                    foreach (var exception in wfa_ex_lst)
                    {
                        BB_WFA_Exception_Translated translated_ex = new BB_WFA_Exception_Translated()
                        {
                            ID = exception.ID,
                            WFA_Control_ID = exception.WFA_Control_ID,
                            Line_ID = exception.Line_ID,
                            Level_ID = exception.Level_ID,
                            Condition_Value = exception.Condition_Value,
                            Action_ID = db.BB_RD_WFA_Exception_Action.Where(x => x.ID == exception.Action_ID).Select(x => x.Name).FirstOrDefault(),
                            Condition_ID = db.BB_RD_WFA_Condition.Where(x => x.ID == exception.Condition_ID).Select(x => x.Condition).FirstOrDefault(),
                            Type_ID = db.BB_RD_WFA_Condition_Type.Where(x => x.ID == exception.Type_ID).Select(x => x.Description).FirstOrDefault(),
                        };

                        wfa_ex_translated_lst.Add(translated_ex);
                    }
                }
                wfa_and_exceptions.Lst_Exception = wfa_ex_translated_lst;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }

            return Ok(wfa_and_exceptions);
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("ValidateWFARules")]
        public IHttpActionResult ValidateWFARules(int proposalID)
        {
            WFAValidations_OneShot wrp = new WFAValidations_OneShot();
            wrp.Lst_BBP_Quote = new List<BB_Proposal_Quote_WFA>();
            wrp.Lst_BBP_RS_Quote = new List<BB_Proposal_Quote_RS_WFA>();

            try { 

                // LISTA BB_PROPOSAL_QUOTE ---------------------------------------------------------------------
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_Workflow_Aprovacoes_OneShot_Validation", conn);
                        cmd.CommandTimeout = 180;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Proposal_ID", proposalID);
                        cmd.Parameters.AddWithValue("@WFA_ID", 1);
                        cmd.Parameters.AddWithValue("@is_RS", 0);
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            BB_Proposal_Quote_WFA bbp_quote = new BB_Proposal_Quote_WFA
                            {
                                ID = (int)rdr["ID"],
                                Proposal_ID = rdr["Proposal_ID"] != DBNull.Value ? (int?)rdr["Proposal_ID"] : null,
                                CreatedBy = rdr["CreatedBy"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("CreatedBy")) : "",
                                ModifiedBy = rdr["ModifiedBy"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("ModifiedBy")) : "",
                                CreatedTime = rdr["CreatedTime"] != DBNull.Value ? (DateTime?)rdr["CreatedTime"] : null,
                                ModifiedTime = rdr["ModifiedTime"] != DBNull.Value ? (DateTime?)rdr["ModifiedTime"] : null,
                                Locked = rdr["Locked"] != DBNull.Value ? (bool?)rdr["Locked"] : null,
                                Family = rdr["Family"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Family")) : "",
                                CodeRef = rdr["CodeRef"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("CodeRef")) : "",
                                Description = rdr["Description"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Description")) : "",
                                UnitPriceCost = rdr["UnitPriceCost"] != DBNull.Value ? (double?)rdr["UnitPriceCost"] : null,
                                Qty = rdr["Qty"] != DBNull.Value ? (int?)rdr["Qty"] : null,
                                TotalCost = rdr["TotalCost"] != DBNull.Value ? (double?)rdr["TotalCost"] : null,
                                Margin = rdr["Margin"] != DBNull.Value ? (double?)rdr["Margin"] : null,
                                PVP = rdr["PVP"] != DBNull.Value ? (double?)rdr["PVP"] : null,
                                TotalPVP = rdr["TotalPVP"] != DBNull.Value ? (double?)rdr["TotalPVP"] : null,
                                DiscountPercentage = rdr["DiscountPercentage"] != DBNull.Value ? (double?)rdr["DiscountPercentage"] : null,
                                UnitDiscountPrice = rdr["UnitDiscountPrice"] != DBNull.Value ? (double?)rdr["UnitDiscountPrice"] : null,
                                GPTotal = rdr["GPTotal"] != DBNull.Value ? (double?)rdr["GPTotal"] : null,
                                GPPercentage = rdr["GPPercentage"] != DBNull.Value ? (double?)rdr["GPPercentage"] : null,
                                TotalNetsale = rdr["TotalNetsale"] != DBNull.Value ? (double?)rdr["TotalNetsale"] : null,
                                IsFinanced = rdr["IsFinanced"] != DBNull.Value ? (bool?)rdr["IsFinanced"] : null,
                                TCP = rdr["TCP"] != DBNull.Value ? (double?)rdr["TCP"] : null,
                                Name = rdr["Name"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Name")) : "",
                                ClickPriceC = rdr["ClickPriceC"] != DBNull.Value ? (double?)rdr["ClickPriceC"] : null,
                                ClickPriceBW = rdr["ClickPriceBW"] != DBNull.Value ? (double?)rdr["ClickPriceBW"] : null,
                                IsMarginBEU = rdr["IsMarginBEU"] != DBNull.Value ? (bool?)rdr["IsMarginBEU"] : null,
                                IsUsed = rdr["IsUsed"] != DBNull.Value ? (bool?)rdr["IsUsed"] : null,
                                IsInClient = rdr["IsInClient"] != DBNull.Value ? (bool?)rdr["IsInClient"] : null,
                                TotalCost_ = rdr["TotalCost_"] != DBNull.Value ? (double?)rdr["TotalCost_"] : null,
                                passedValidation = rdr["passedValidation"] != DBNull.Value ? (bool?)rdr["passedValidation"] : null,
                                alertMessage = rdr["alertMessage"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("alertMessage")) : "",
                            };

                        wrp.Lst_BBP_Quote.Add(bbp_quote);
                    }
                        rdr.Close();
                }

                // LISTA BB_PROPOSAL_RS_QUOTE ---------------------------------------------------------------------
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_Workflow_Aprovacoes_OneShot_Validation", conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Proposal_ID", proposalID);
                    cmd.Parameters.AddWithValue("@WFA_ID", 1);
                    cmd.Parameters.AddWithValue("@is_RS", 1);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    while (rdr.Read())
                    {
                        BB_Proposal_Quote_RS_WFA bbp_rs_quote = new BB_Proposal_Quote_RS_WFA
                        {
                            ID = (int)rdr["ID"],
                            CodeRef = rdr["CodeRef"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("CodeRef")) : "",
                            Description = rdr["Description"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Description")) : "",
                            DiscountPercentage = rdr["DiscountPercentage"] != DBNull.Value ? (double?)rdr["DiscountPercentage"] : null,
                            Family = rdr["Family"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Family")) : "",
                            GPPercentage = rdr["GPPercentage"] != DBNull.Value ? (double?)rdr["GPPercentage"] : null,
                            GPTotal = rdr["GPTotal"] != DBNull.Value ? (double?)rdr["GPTotal"] : null,
                            Locked = rdr["Locked"] != DBNull.Value ? (bool?)rdr["Locked"] : null,
                            Margin = rdr["Margin"] != DBNull.Value ? (double?)rdr["Margin"] : null,
                            MonthlyFee = rdr["MonthlyFee"] != DBNull.Value ? (double?)rdr["MonthlyFee"] : null,
                            MonthlyGP = rdr["MonthlyGP"] != DBNull.Value ? (double?)rdr["MonthlyGP"] : null,
                            PVP = rdr["PVP"] != DBNull.Value ? (double?)rdr["PVP"] : null,
                            Qty = rdr["Qty"] != DBNull.Value ? (int?)rdr["Qty"] : null,
                            TotalCost = rdr["TotalCost"] != DBNull.Value ? (double?)rdr["TotalCost"] : null,
                            MonthlyFeeCost = rdr["MonthlyFeeCost"] != DBNull.Value ? (double?)rdr["MonthlyFeeCost"] : null,
                            TotalMonths = rdr["TotalMonths"] != DBNull.Value ? (int?)rdr["TotalMonths"] : null,
                            TotalNetsale = rdr["TotalNetsale"] != DBNull.Value ? (double?)rdr["TotalNetsale"] : null,
                            TotalPVP = rdr["TotalPVP"] != DBNull.Value ? (double?)rdr["TotalPVP"] : null,
                            UnitDiscountPrice = rdr["UnitDiscountPrice"] != DBNull.Value ? (double?)rdr["UnitDiscountPrice"] : null,
                            UnitPriceCost = rdr["UnitPriceCost"] != DBNull.Value ? (double?)rdr["UnitPriceCost"] : null,
                            ProposalID = rdr["ProposalID"] != DBNull.Value ? (int?)rdr["ProposalID"] : null,
                            IsFinanced = rdr["IsFinanced"] != DBNull.Value ? (bool?)rdr["IsFinanced"] : null,
                            passedValidation = rdr["passedValidation"] != DBNull.Value ? (bool?)rdr["passedValidation"] : null,
                            alertMessage = rdr["alertMessage"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("alertMessage")) : "",
                        };

                        wrp.Lst_BBP_RS_Quote.Add(bbp_rs_quote);
                    }
                    rdr.Close();
                }
            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }          
            return Ok(wrp);
        }

        // #######################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("getWFAApproverProposals")]
        public IHttpActionResult getWFAApproverProposals(string user_ID)
        {
            List<WFA_Approver_Proposal> lst_approver_proposal = new List<WFA_Approver_Proposal>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {
                    conn.Open();

                    string sql = "SELECT * FROM " + "ft_get_WFA_Approver_Proposals"+ "(@User_ID)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@User_ID", user_ID);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    while (rdr.Read())
                    {
                        WFA_Approver_Proposal wfa_approver_proposal = new WFA_Approver_Proposal
                        {
                            QuoteNr = rdr["QuoteNr"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("QuoteNr")) : "",
                            Client = rdr["Client"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Client")) : "",
                            CreatedBy = rdr["CreatedBy"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("CreatedBy")) : "",
                            ApprovedRequestDate = rdr["ApprovedRequestDate"] != DBNull.Value ? (DateTime?)rdr["ApprovedRequestDate"] : null,
                            Status = rdr["Status"] != DBNull.Value ? (bool?)rdr["Status"] : null,
                            ProposalName = rdr["ProposalName"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("ProposalName")) : "",
                            ProposalID = rdr["ProposalID"] != DBNull.Value ? (int?)rdr["ProposalID"] : null,
                        };

                        lst_approver_proposal.Add(wfa_approver_proposal);
                    }
                    rdr.Close();              
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ok(lst_approver_proposal);
        }



        // ----------------------------------------------------------------------------------------------------
        // CLASSES --------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        public class Milestone_ProposalPlan
        {
            public List<BB_Proposal_Milestone> Milestones { get; set; }
            public List<BB_CRM_Approval_Comments> MilestoneComments { get; set; }
        }

        public class WFA_Listagem
        {
            public int? Line { get; set; }
            public string BU { get; set; }
            public string DealElements { get; set; }
            public string TypeOfCustomer { get; set; }
            public List<Level> lstLevel { get; set; }

        }
        public class Level
        {
            public string Approver { get; set; }
            public string Condition { get; set; }
            public double? Percentage { get; set; }
            public string Type { get; set; }
        }

        public class WFA_Create
        {
            // dropdowns
            public List<WFA_Approvers> Lst_Approver { get; set; }
            public List<BB_RD_WFA_Condition> Lst_Condition { get; set; }
            public List<BB_RD_WFA_Condition_Type> Lst_Type { get; set; }
            public List<BB_RD_WFA_BU> Lst_BU { get; set; }
            public List<BB_RD_WFA_Elements> Lst_DealElements { get; set; }

            public int ID { get; set; }
            public int? BU { get; set; }
            public int? DealElement { get; set; }
            public string TypeOfCustomer { get; set; }

            public int? Level1_Approver { get; set; }
            public int? Level2_Approver { get; set; }
            public int? Level3_Approver { get; set; }
            public int? Level4_Approver { get; set; }
            public int? Level5_Approver { get; set; }

            public int? Level1_Condition { get; set; }
            public int? Level2_Condition { get; set; }
            public int? Level3_Condition { get; set; }
            public int? Level4_Condition { get; set; }
            public int? Level5_Condition { get; set; }

            public int? Percentage_1 { get; set; }
            public int? Percentage_2 { get; set; }
            public int? Percentage_3 { get; set; }
            public int? Percentage_4 { get; set; }
            public int? Percentage_5 { get; set; }

            public int? Level1_Type { get; set; }
            public int? Level2_Type { get; set; }
            public int? Level3_Type { get; set; }
            public int? Level4_Type { get; set; }
            public int? Level5_Type { get; set; }
        }

        public partial class WFA_Approvers
        {
            public int ID { get; set; }
            public string User_ID { get; set; }
            public string Name { get; set; }
        }

        public partial class BB_WFA_Exception_Translated
        {
            public int ID { get; set; }
            public Nullable<int> WFA_Control_ID { get; set; }
            public Nullable<int> Line_ID { get; set; }
            public string Type_ID { get; set; }
            public string Condition_ID { get; set; }
            public Nullable<double> Condition_Value { get; set; }
            public string Action_ID { get; set; }
            public Nullable<int> Level_ID { get; set; }
        }

        public class WFA_CreateException
        {
            // dropdowns
            public List<int?> Lst_LineNr { get; set; }
            public List<BB_RD_WFA_Condition_Type> Lst_Type { get; set; }
            public List<BB_RD_WFA_Condition> Lst_Condition { get; set; }
            public List<BB_RD_WFA_Exception_Action> Lst_Action { get; set; }

            public int ID { get; set; }
            public int? LineNr { get; set; }
            public int? Type { get; set; }
            public int? Condition { get; set; }
            public int? Action { get; set; }
            public double? Condition_Value { get; set; }
            public int? LevelNr { get; set; }

        }

        //Objeto com as linhas WFA e Exceções (tudo strings)
        public class WFA_And_Exceptions
        {
            public List<WFA_Listagem> Lst_WFA { get; set; }
            public List<BB_WFA_Exception_Translated> Lst_Exception { get; set; }
        }


        public class WFAValidations_OneShot
        {
            public List<BB_Proposal_Quote_WFA> Lst_BBP_Quote { get; set; }
            public List<BB_Proposal_Quote_RS_WFA> Lst_BBP_RS_Quote { get; set; }
        }


        public partial class BB_Proposal_Quote_WFA
        {
            public int ID { get; set; }
            public Nullable<int> Proposal_ID { get; set; }
            public string CreatedBy { get; set; }
            public string ModifiedBy { get; set; }
            public Nullable<System.DateTime> CreatedTime { get; set; }
            public Nullable<System.DateTime> ModifiedTime { get; set; }
            public Nullable<bool> Locked { get; set; }
            public string Family { get; set; }
            public string CodeRef { get; set; }
            public string Description { get; set; }
            public Nullable<double> UnitPriceCost { get; set; }
            public Nullable<int> Qty { get; set; }
            public Nullable<double> TotalCost { get; set; }
            public Nullable<double> Margin { get; set; }
            public Nullable<double> PVP { get; set; }
            public Nullable<double> TotalPVP { get; set; }
            public Nullable<double> DiscountPercentage { get; set; }
            public Nullable<double> UnitDiscountPrice { get; set; }
            public Nullable<double> GPTotal { get; set; }
            public Nullable<double> GPPercentage { get; set; }
            public Nullable<double> TotalNetsale { get; set; }
            public Nullable<bool> IsFinanced { get; set; }
            public Nullable<double> TCP { get; set; }
            public string Name { get; set; }
            public Nullable<double> ClickPriceC { get; set; }
            public Nullable<double> ClickPriceBW { get; set; }
            public Nullable<bool> IsMarginBEU { get; set; }
            public Nullable<bool> IsUsed { get; set; }
            public Nullable<bool> IsInClient { get; set; }
            public Nullable<double> UnitPriceCost_ { get; set; }
            public Nullable<double> TotalCost_ { get; set; }
            public Nullable<bool> passedValidation { get; set; }
            public string alertMessage { get; set; }
        }

        public partial class BB_Proposal_Quote_RS_WFA
        {
            public int ID { get; set; }
            public string CodeRef { get; set; }
            public string Description { get; set; }
            public Nullable<double> DiscountPercentage { get; set; }
            public string Family { get; set; }
            public Nullable<double> GPPercentage { get; set; }
            public Nullable<double> GPTotal { get; set; }
            public Nullable<bool> Locked { get; set; }
            public Nullable<double> Margin { get; set; }
            public Nullable<double> MonthlyFee { get; set; }
            public Nullable<double> MonthlyGP { get; set; }
            public Nullable<double> PVP { get; set; }
            public Nullable<int> Qty { get; set; }
            public Nullable<double> TotalCost { get; set; }
            public Nullable<double> MonthlyFeeCost { get; set; }
            public Nullable<int> TotalMonths { get; set; }
            public Nullable<double> TotalNetsale { get; set; }
            public Nullable<double> TotalPVP { get; set; }
            public Nullable<double> UnitDiscountPrice { get; set; }
            public Nullable<double> UnitPriceCost { get; set; }
            public Nullable<int> ProposalID { get; set; }
            public Nullable<bool> IsFinanced { get; set; }
            public Nullable<bool> passedValidation { get; set; }
            public string alertMessage { get; set; }
        }

        public class WFA_Approver_Proposal
        {

            public string QuoteNr { get; set; }
            public string Client { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? ApprovedRequestDate { get; set; }
            public bool? Status { get; set; }
            public string ProposalName  { get; set; }
            public int? ProposalID { get; set; }
        }
    }
}
