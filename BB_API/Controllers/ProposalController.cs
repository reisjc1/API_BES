using AutoMapper;
using log4net;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Provider;

namespace WebApplication1.Controllers
{
    public class ProposalController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public masterEntities dbUsers = new masterEntities();

        [ClaimsAuthorize]
        [HttpPost]
        public HttpResponseMessage PostProposalDraft(ProposalRootObject p)
        {

            try
            {
                ActionResponse ac = new ActionResponse();
                ProposalBLL pBLL = new ProposalBLL();
                if (p.Draft.details.ID == 0)
                {
                    ac = pBLL.ProposalDraftSaveAs(p);
                    ac.Message = "Propuesta guardada éxito!";
                }
                else
                {
                    ac = pBLL.ProposalDraftSave(p);
                    ac.Message = "Propuesta guardada éxito!";
                }

                if (ac.ProposalObj == null)
                {
                    throw new Exception("ac.ProposalObj = null");
                }

                return Request.CreateResponse(HttpStatusCode.OK, ac);

            }
            catch (Exception ex)

            {
                log4net.ThreadContext.Properties["proposal_id"] = p.Draft.details.ID;
                log.Error(ex.Message.ToString(), ex);
                throw new ActionFailResponse("No gravou")
                {
                    StatusCode = 501
                };
            }
        }

        [ClaimsAuthorize]
        [HttpPost]
        public HttpResponseMessage PostProposalDuplicate(ProposalRootObject p)
        {
            try
            {
                ActionResponse ac = new ActionResponse();
                ProposalBLL pBLL = new ProposalBLL();
                ac = pBLL.ProposalDraftSaveAs(p);
                ac.Message = "Proposta duplicada com sucesso!";
                return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = p.Draft.details.ID;
                log.Error(ex.Message.ToString(), ex);
                throw new ActionFailResponse("Nao gravou")
                {
                    StatusCode = 501
                };
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ProposalResetStatus")]
        public HttpResponseMessage ProposalResetStatus(ProposalRootObject p)
        {
            try
            {
                ActionResponse ac = new ActionResponse();
                ProposalBLL pBLL = new ProposalBLL();
                LoadProposalInfo lpi = new LoadProposalInfo();

                LD_Contrato l = new LD_Contrato();

                using (var db1 = new BB_DB_DEV_LeaseDesk())
                {
                    l = db1.LD_Contrato.Where(x => x.ProposalID == p.Draft.details.ID).FirstOrDefault();
                }

                if (l != null && l.StatusID != 4)
                {
                    lpi.ProposalId = p.Draft.details.ID;
                    ac = pBLL.LoadProposal(lpi);
                    ac.Message = "Proceso de venta en curso, ¡contacta a Leasdesk!";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
                }


                BB_Proposal ppro = db.BB_Proposal.Where(x => x.ID == p.Draft.details.ID).FirstOrDefault();
                if (ppro != null)
                {
                    ppro.StatusID = 1;
                    db.Entry(ppro).State = EntityState.Modified;
                    db.SaveChanges();
                }

                lpi.ProposalId = p.Draft.details.ID;
                ac = pBLL.LoadProposal(lpi);
                ac.Message = "¡Propuesta en borrador!";
                return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = p.Draft.details.ID;
                log.Error(ex.Message.ToString(), ex);
                throw new ActionFailResponse("No grabó")
                {
                    StatusCode = 501
                };
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("LoadProposal")]
        public HttpResponseMessage LoadProposal(int? id)
        {
            string a = User.Identity.Name;
            ActionResponse err = new ActionResponse();
            try
            {
                ProposalBLL pBLL = new ProposalBLL();
                LoadProposalInfo lpi = new LoadProposalInfo();
                lpi.ProposalId = id.Value;
                err = pBLL.LoadProposal(lpi);
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = id;
                log.Error(ex.Message.ToString(), ex);
                err.Message = ex.Message.ToString();
                err.InnerException = ex.InnerException.ToString();
            }
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err); ;
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetBB_Proposals")]
        public List<BB_PROPOSALS_GET> GetBB_Proposals([FromBody] Owner_ Owner)
        {
            AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            List<BB_PROPOSALS_GET> propostas = new List<BB_PROPOSALS_GET>();
            try
            {
                foreach (var item in db.BB_Proposal.Where(x => (x.CreatedBy == user.Email || x.ModifiedBy == user.Email || x.AccountManager == user.Email) && x.ToDelete == false).ToList())
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal, BB_PROPOSALS_GET>();
                    });
                    IMapper iMapper1 = config1.CreateMapper();
                    BB_PROPOSALS_GET b = new BB_PROPOSALS_GET();
                    b = iMapper1.Map<BB_Proposal, BB_PROPOSALS_GET>(item);
                    b.StatusObj = new BB_Proposal_Status();
                    b.StatusName = db.BB_Proposal_Status.Where(x => x.ID == item.StatusID).Select(x => x.Description).FirstOrDefault();
                    b.StatusObj = db.BB_Proposal_Status.Where(x => x.ID == item.StatusID).FirstOrDefault();
                    b.CreatedBy = dbUsers.AspNetUsers.Where(x => x.Email == item.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                    b.ModifiedBy = dbUsers.AspNetUsers.Where(x => x.Email == item.ModifiedBy).Select(x => x.DisplayName).FirstOrDefault();
                    b.ClientName = db.BB_Proposal_Client.Where(x => x.ProposalID == item.ID).Select(x => x.Name).FirstOrDefault();
                    b.CreatedTime = item.CreatedTime;
                    b.ModifiedTime = item.ModifiedTime;
                    b.AccountManager = dbUsers.AspNetUsers.Where(x => x.Email == item.AccountManager).Select(x => x.DisplayName).FirstOrDefault();
                    b.StatusCRM = item.StatusCRM1;
                    propostas.Add(b);
                }
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = 0;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }

            propostas = propostas.OrderByDescending(x => x.CreatedTime).ToList();
            return propostas;
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetBB_Proposals_v1")]
        public List<BB_PROPOSALS_GET_V1> GetBB_Proposals_v1([FromBody] Owner_ Owner)
        {
            //AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            List<BB_PROPOSALS_GET_V1> lst = new List<BB_PROPOSALS_GET_V1>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_Proposal_ALL", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        BB_PROPOSALS_GET_V1 m = new BB_PROPOSALS_GET_V1();
                        m.ID = (int)rdr["ID"];
                        m.AccountNumber = rdr["NCliente"].ToString();
                        m.ClientName = rdr["Cliente"].ToString();
                        m.Name = rdr["Name"].ToString();
                        m.Description = rdr["Description"].ToString();
                        m.TotalValue = rdr["valor"].ToString() != "" ? (float.Parse(rdr["valor"].ToString())) : 0;
                        m.QuoteCRM = rdr["QuoteCRM"].ToString();
                        m.ModifiedTime = rdr["UltimaModificacao"] != null ? DateTime.Parse(rdr["UltimaModificacao"].ToString()) : new DateTime();
                        m.Status = new BB_Proposal_Status
                        {
                            Description = rdr["Estado"].ToString(),
                            Phase = (int)rdr["Phase"],
                            ID = (int)rdr["EstadoID"],
                        };
                        m.FinancingStatus = rdr["Financeiro"].ToString();
                        m.ServiceStatus = rdr["Servico"].ToString();
                        m.LeaseDeskStatus = rdr["PosVenda"].ToString();
                        m.CreatedByEmail = rdr["CreatedByEmail"].ToString();
                        m.CreatedByName = rdr["CreatedByName"].ToString();
                        m.AccountManagerName = rdr["AccountManagerName"].ToString();
                        m.AccountManagerEmail = rdr["AccountManagerEmail"].ToString();
                        m.ModifiedByEmail = rdr["ModifiedByEmail"].ToString();
                        m.ModifiedByName = rdr["ModifiedByName"].ToString();
                        lst.Add(m);
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = 0;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }


            return lst;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_CRM_Oportunity_Status")]
        public HttpResponseMessage Get_CRM_Oportunity_Status(string quote)
        {
            Model_sp_Get_CRM_Status status = new Model_sp_Get_CRM_Status();
            CRM_ImpersonatedUser sp = new CRM_ImpersonatedUser();
            return Request.CreateResponse<Model_sp_Get_CRM_Status>(HttpStatusCode.OK, status);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_CRM_MyOportunidades_Status")]
        public HttpResponseMessage Get_CRM_MyOportunidades_Status(cEmail email)
        {
            CRM_ImpersonatedUser sp = new CRM_ImpersonatedUser();
            List<Model_sp_Get_CRM_Oportunity_Status> lst = new List<Model_sp_Get_CRM_Oportunity_Status>();
            return Request.CreateResponse<List<Model_sp_Get_CRM_Oportunity_Status>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetBB_Proposals_DRV")]
        public List<BB_PROPOSALS_GET> GetBB_Proposals_DRV([FromBody] Owner_ Owner)
        {
            List<BB_PROPOSALS_GET> listProposals = new List<BB_PROPOSALS_GET>();
            List<string> userEmails = new List<string>() { Owner.Owner };
            userEmails.AddRange(dbUsers.AspNetUsers.Where(x => x.ManagerEmail == Owner.Owner).Select(u => u.Email));
            List<BB_Proposal> proposals = db.BB_Proposal.Where(x => (userEmails.Contains(x.CreatedBy) || userEmails.Contains(x.AccountManager)) && x.ToDelete == false).ToList();
            foreach (BB_Proposal item in proposals)
            {
                var config1 = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<BB_Proposal, BB_PROPOSALS_GET>();
                });
                IMapper iMapper1 = config1.CreateMapper();
                BB_PROPOSALS_GET b = iMapper1.Map<BB_Proposal, BB_PROPOSALS_GET>(item);
                b.StatusName = db.BB_Proposal_Status.Where(x => x.ID == item.StatusID).Select(x => x.Description).FirstOrDefault();
                b.StatusObj = db.BB_Proposal_Status.Where(x => x.ID == item.StatusID).FirstOrDefault();
                b.CreatedBy = dbUsers.AspNetUsers.Where(x => x.Email == item.CreatedBy).Select(x => x.DisplayName).FirstOrDefault();
                b.ModifiedBy = dbUsers.AspNetUsers.Where(x => x.Email == item.ModifiedBy).Select(x => x.DisplayName).FirstOrDefault();
                b.ClientName = db.BB_Clientes.Where(x => x.accountnumber == item.ClientAccountNumber).Select(x => x.Name).FirstOrDefault();
                b.AccountManager = dbUsers.AspNetUsers.Where(x => x.Email == item.AccountManager).Select(x => x.DisplayName).FirstOrDefault();
                b.StatusCRM = "";
                b.CreatedTime = item.CreatedTime;
                b.ModifiedTime = item.ModifiedTime;
                listProposals.Add(b);
            }
            listProposals = listProposals.OrderByDescending(x => x.CreatedTime).ToList();
            return listProposals;
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("DeleteProposal")]
        public HttpResponseMessage DeleteProposal(int? proposalID)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            try
            {
                var p = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                p.ToDelete = true;
                p.ModifiedTime = DateTime.Now;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposalID;
                log.Error(ex.Message.ToString(), ex);
            }
            return response;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("AssginAccountManager")]
        public HttpResponseMessage AssginAccountManager(UserAssign p)
        {
            int? proposalID = p.Draft.details.ID;
            ActionResponse err = new ActionResponse();
            if (proposalID == null || proposalID == 0)
            {
                ProposalRootObject a = new ProposalRootObject();
                a.Draft = p.Draft;
                a.Summary = p.Summary;
                ProposalBLL pro = new ProposalBLL();
                err = pro.ProposalDraftSaveAs(a);
                proposalID = err.ProposalIDReturn;
            }
            try
            {
                var pa = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                pa.AccountManager = p.coworkerEmail;
                pa.ModifiedTime = DateTime.Now;
                pa.StatusID = 6;
                db.Entry(pa).State = EntityState.Modified;
                db.SaveChanges();
                err.Message = "Gestor asignado con éxito.";
                err.ProposalIDReturn = pa.ID;
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposalID;
                log.Error(ex.Message.ToString(), ex);
            }
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("AssignDRV")]
        public HttpResponseMessage AssignDRV(int? proposalID, string Owner)
        {
            ActionResponse err = new ActionResponse();
            try
            {
                var p = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                p.ModifiedBy = Owner;
                p.ModifiedTime = DateTime.Now;

                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                err.ProposalIDReturn = p.ID;
                err.Message = "DRV Assignado";
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposalID;
                log.Error(ex.Message.ToString(), ex);
            }
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Encomenda")]
        public async Task<HttpResponseMessage> Encomenda()
        {
            int? ProposalID = Int32.Parse(HttpContext.Current.Request.Params["ProposalID"]);
            ActionResponse err = new ActionResponse();
            StringBuilder c = new StringBuilder();
            try
            {

                if (ProposalID == null)
                {
                    err.Message = "Por favor, grabe su propuesta y vuelva a intentarlo.";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
                }

                BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ProposalID).FirstOrDefault();
                BB_Proposal_Financing ft = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                BB_Proposal_PrazoDiferenciado pz = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();

                //List<ProposalContact> lstContactDocumentantion = JsonConvert.DeserializeObject<List<ProposalContact>>(HttpContext.Current.Request.Params["DocumentationContacts"]);
                //List<ProposalContact> lstContactSign = JsonConvert.DeserializeObject<List<ProposalContact>>(HttpContext.Current.Request.Params["SigningContacts"]);

                //string SigningType = HttpContext.Current.Request.Params["SigningType"];
                //string SigningTypeObservations = HttpContext.Current.Request.Params["SigningTypeObservations"];

                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo i1 = new LoadProposalInfo();
                i1.ProposalId = ProposalID.Value;
                ActionResponse a1 = p1.LoadProposal(i1);
                err.ProposalObj = a1.ProposalObj;
                err.Message = "Por motivos técnicos, utilice el botón 'Terminar Proceso' (temporalmente).";
                //return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
                StringBuilder err1 = ValidarProcessoVenda_encomenda(pz, ft, a1, proposal);



                if (err1 != null && err1.Length > 0)
                {
                    err.Message = err1.ToString();
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
                }

                if (pz != null || ft.FinancingTypeCode == 0)
                {
                    List<LD_FinancingDocument> fd = db.LD_FinancingDocument.Where(x => x.TipoFinanciamentoID == ft.FinancingTypeCode).ToList();

                    if (proposal != null)
                    {
                        String Observations = HttpContext.Current.Request.Params["Observations"];
                        int ContractType = Int32.Parse(HttpContext.Current.Request.Params["ContractType"]);
                        int ContractoId = 0;
                        //List<string> contactos = JsonConvert.DeserializeObject<List<string>>(HttpContext.Current.Request.Params["Contacts"]);
                        //foreach (var str in contactos)
                        //{
                        //    BB_Proposal_Contacts ca = new BB_Proposal_Contacts();
                        //    ca.Name = str;
                        //    ca.ProposalID = ProposalID;
                        //    db.BB_Proposal_Contacts.Add(ca);
                        //}

                        //db.SaveChanges();



                        LD_Contrato ld = new LD_Contrato();
                        using (var db = new BB_DB_DEV_LeaseDesk())
                        {
                            //int? assnaturaID = db.LD_Assinatura_System.Where(x => x.System == SigningType).Select(x => x.ID).FirstOrDefault();
                            ld = db.LD_Contrato.Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID).FirstOrDefault();
                            if (ld == null)
                            {
                                ld = new LD_Contrato();
                                ld.ProposalID = proposal.ID;
                                ld.QuoteNumber = proposal.CRM_QUOTE_ID;
                                ld.CreatedBy = proposal.CreatedBy;
                                ld.ModifiedBy = proposal.CreatedBy;
                                ld.CreatedTime = DateTime.Now;
                                ld.ModifiedTime = DateTime.Now;
                                ld.TipoContratoID = ContractType;
                                ld.Comments = "Pedido de fabricación del pedido.";
                                //ld.SystemAssinaturaID = assnaturaID;
                                ld.ComentariosGC = "Pedido de fabricación en fábrica " + "- " + Observations;
                                ld.IsClosed = false;
                                ld.Retorno = false;
                                ld.StatusID = 1;
                                db.LD_Contrato.Add(ld);
                                db.SaveChanges();

                                ContractoId = ld.ID;
                            }
                            else
                            {
                                ld.ProposalID = proposal.ID;
                                ld.ModifiedBy = proposal.CreatedBy;
                                ld.ModifiedTime = DateTime.Now;
                                ld.TipoContratoID = ContractType;
                                ld.ComentariosGC += " " + Observations;
                                ld.IsClosed = false;
                                ld.Retorno = false;
                                //ld.SystemAssinaturaID = assnaturaID;
                                ld.StatusID = 1;
                                db.Entry(ld).State = EntityState.Modified;
                                db.SaveChanges();
                                ContractoId = ld.ID;
                            }
                        }
                        err.Message = "Pedido de encargo a la fábrica, por favor después debe finalizar el proceso de venta, completando la información que falta.";
                        BB_Proposal p = new BB_Proposal();
                        using (var db = new BB_DB_DEVEntities2())
                        {
                            p = db.BB_Proposal.Where(x => x.ID == ProposalID).FirstOrDefault();
                            if (p != null)
                            {
                                p.StatusID = 11;
                                db.Entry(p).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }




                        //List<String> contactsArray = JsonConvert.DeserializeObject<List<String>>(HttpContext.Current.Request.Params["Contacts"]);
                        string root = @AppSettingsGet.LeaseDesk_UploadFile_Contrato + ContractoId + "\\";

                        if (!Directory.Exists(root))
                            System.IO.Directory.CreateDirectory(root);

                        var docfiles = new List<string>();
                        int documentCount = HttpContext.Current.Request.Files.Count;
                        if (documentCount > 0)
                        {
                            for (int j = 0; j < documentCount; j++)
                            {

                                var document = HttpContext.Current.Request.Files["document" + j];
                                //var documentData = HttpContext.Current.Request.Params["documentData" + j];
                                DocumentoData documentData = JsonConvert.DeserializeObject<DocumentoData>(HttpContext.Current.Request.Params["documentData" + j]);

                                var postedFile = document;
                                var filePath = root + postedFile.FileName;
                                postedFile.SaveAs(filePath);
                                docfiles.Add(filePath);

                                using (var db = new BB_DB_DEV_LeaseDesk())
                                {
                                    LD_DocumentProposal documentoSave = new LD_DocumentProposal();
                                    //documentoSave.ClassificationID = documentData.Type != null ? documentData.Type.ID : 11;
                                    documentoSave.ClassificationID = documentData.Type != null ? documentData.Type : 11;

                                    documentoSave.CreatedBy = "";
                                    documentoSave.CreatedTime = DateTime.Now;
                                    documentoSave.QuoteNumber = proposal.CRM_QUOTE_ID;
                                    documentoSave.SystemID = 1;
                                    documentoSave.FileFullPath = filePath;
                                    documentoSave.DocumentIsValid = false;
                                    documentoSave.DocumentIsProcess = false;
                                    documentoSave.FileName = Path.GetFileName(filePath);
                                    documentoSave.ContratoID = ContractoId;
                                    documentoSave.Comments = documentData != null ? documentData.Comments : "";
                                    db.LD_DocumentProposal.Add(documentoSave);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = ProposalID;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
                err.Message = "No ha sido posible realizar el pedido, por favor, póngase en contacto con el equipo de BB.";
            }

            //ProposalBLL p1 = new ProposalBLL();
            //LoadProposalInfo i = new LoadProposalInfo();
            //i.ProposalId = ProposalID.Value;
            //ActionResponse a = p1.LoadProposal(i);

            //err.ProposalObj = a1.ProposalObj;

            //err.Message = "Pedido de encomenda à fábrica, por favor depois tem que terminar o processo de venda, preenchendo a informação que falta!";
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("TerminarProcesso")]
        public async Task<HttpResponseMessage> TerminarProcesso()
        {
            int? ProposalID = Int32.Parse(HttpContext.Current.Request.Params["ProposalID"]);
            ActionResponse err = new ActionResponse();
            StringBuilder c = new StringBuilder();
            String Observations = null;
            bool isRetorno = false;

            bool isFirstTime = false;
            try
            {
                if (ProposalID == null)
                {

                    err.Message = "Por favor, guarde su propuesta y vuelva a intentarlo.";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
                }

                BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ProposalID).FirstOrDefault();
                BB_Proposal_Financing ft = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                BB_Proposal_PrazoDiferenciado pz = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();

                List<ProposalContact> lstContactDocumentantion = JsonConvert.DeserializeObject<List<ProposalContact>>(HttpContext.Current.Request.Params["DocumentationContacts"]);
                List<ProposalContact> lstContactSign = JsonConvert.DeserializeObject<List<ProposalContact>>(HttpContext.Current.Request.Params["SigningContacts"]);

                string SigningType = HttpContext.Current.Request.Params["SigningType"];
                string SigningTypeObservations = HttpContext.Current.Request.Params["SigningTypeObservations"];

                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo i1 = new LoadProposalInfo();
                i1.ProposalId = ProposalID.Value;
                ActionResponse a1 = p1.LoadProposal(i1);
                err.ProposalObj = a1.ProposalObj;

                StringBuilder err1 = ValidarProcessoVenda(lstContactDocumentantion, lstContactSign, proposal, pz, SigningType, SigningTypeObservations, ft, a1);

                if (proposal != null && proposal.StatusID == 11)
                {

                    err.Message = "Proceso está en el de departamento administración, no es posible volver a enviarlo.";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.NotAcceptable, err);
                }

                if (err1 != null && err1.Length > 0)
                {
                    err.Message = err1.ToString();
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.NotAcceptable, err);
                }


                List<LD_FinancingDocument> fd = db.LD_FinancingDocument.Where(x => x.TipoFinanciamentoID == ft.FinancingTypeCode).ToList();

                if (proposal != null)
                {
                    Observations = HttpContext.Current.Request.Params["Comments"];
                    int ContractType = Int32.Parse(HttpContext.Current.Request.Params["ContractType"]);
                    int ContractoId = 0;
                    //List<string> contactos = JsonConvert.DeserializeObject<List<string>>(HttpContext.Current.Request.Params["Contacts"]);
                    //foreach (var str in contactos)
                    //{
                    //    BB_Proposal_Contacts ca = new BB_Proposal_Contacts();
                    //    ca.Name = str;
                    //    ca.ProposalID = ProposalID;
                    //    db.BB_Proposal_Contacts.Add(ca);
                    //}

                    //db.SaveChanges();



                    LD_Contrato ld = new LD_Contrato();
                    using (var db = new BB_DB_DEV_LeaseDesk())
                    {
                        int? assnaturaID = db.LD_Assinatura_System.Where(x => x.System == SigningType).Select(x => x.ID).FirstOrDefault();
                        ld = db.LD_Contrato.Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID).FirstOrDefault();
                        if (ld == null)
                        {
                            ld = new LD_Contrato();
                            ld.ProposalID = proposal.ID;
                            ld.QuoteNumber = proposal.CRM_QUOTE_ID;
                            ld.CreatedBy = proposal.CreatedBy;
                            //ld.ModifiedBy = proposal.CreatedBy;
                            ld.CreatedTime = DateTime.Now;
                            ld.ModifiedTime = DateTime.Now;
                            ld.TipoContratoID = ContractType;
                            ld.SystemAssinaturaID = assnaturaID;
                            ld.ComentariosGC = Observations;
                            ld.IsClosed = false;
                            ld.Retorno = false;
                            ld.StatusID = 1;
                            db.LD_Contrato.Add(ld);
                            db.SaveChanges();
                            ContractoId = ld.ID;
                            isFirstTime = true;
                        }
                        else
                        {
                            ld.ProposalID = proposal.ID;
                            ld.ModifiedBy = proposal.CreatedBy;
                            ld.ModifiedTime = DateTime.Now;
                            ld.TipoContratoID = ContractType;
                            ld.ComentariosGC += " " + Observations;
                            ld.IsClosed = false;
                            ld.Retorno = false;
                            ld.SystemAssinaturaID = assnaturaID;
                            ld.StatusID = 1;
                            ld.DevolucaoMotivoID = 5;
                            db.Entry(ld).State = EntityState.Modified;
                            db.SaveChanges();
                            isRetorno = true;
                            ContractoId = ld.ID;
                        }
                    }
                    BB_Proposal p = new BB_Proposal();
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        p = db.BB_Proposal.Where(x => x.ID == ProposalID).FirstOrDefault();
                        if (p != null)
                        {
                            p.StatusID = 11;
                            db.Entry(p).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }



                    List<LD_DocumentProposal> lstDocumentProposal = db.LD_DocumentProposal.Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID).ToList();
                    foreach (var item in lstDocumentProposal)
                    {
                        item.ContratoID = ld.ID;
                        using (var db = new BB_DB_DEV_LeaseDesk())
                        {
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }


                    //List<String> contactsArray = JsonConvert.DeserializeObject<List<String>>(HttpContext.Current.Request.Params["Contacts"]);
                    //string root = @AppSettingsGet.LeaseDesk_UploadFile_Contrato + ContractoId + "\\";

                    //if (!Directory.Exists(root))
                    //    System.IO.Directory.CreateDirectory(root);

                    //var docfiles = new List<string>();
                    //int documentCount = HttpContext.Current.Request.Files.Count;
                    //if (documentCount > 0)
                    //{
                    //    for (int j = 0; j < documentCount; j++)
                    //    {

                    //        var document = HttpContext.Current.Request.Files["document" + j];
                    //        //var documentData = HttpContext.Current.Request.Params["documentData" + j];
                    //        if (document != null)
                    //        {
                    //            DocumentoData documentData = JsonConvert.DeserializeObject<DocumentoData>(HttpContext.Current.Request.Params["documentData" + j]);

                    //            var postedFile = document;
                    //            var filePath = root + postedFile.FileName;
                    //            postedFile.SaveAs(filePath);
                    //            docfiles.Add(filePath);

                    //            using (var db = new BB_DB_DEV_LeaseDesk())
                    //            {
                    //                LD_DocumentProposal documentoSave = new LD_DocumentProposal();
                    //                //documentoSave.ClassificationID = documentData.Type != null ? documentData.Type.ID : 11;
                    //                documentoSave.ClassificationID = documentData.Type != null ? documentData.Type : 11;

                    //                documentoSave.CreatedBy = proposal.CreatedBy;
                    //                documentoSave.CreatedTime = DateTime.Now;
                    //                documentoSave.QuoteNumber = proposal.CRM_QUOTE_ID;
                    //                documentoSave.SystemID = 1;
                    //                documentoSave.FileFullPath = filePath;
                    //                documentoSave.DocumentIsValid = false;
                    //                documentoSave.DocumentIsProcess = false;
                    //                documentoSave.FileName = Path.GetFileName(filePath);
                    //                documentoSave.ContratoID = ContractoId;
                    //                documentoSave.Comments = documentData != null ? documentData.Comments : "";
                    //                db.LD_DocumentProposal.Add(documentoSave);
                    //                db.SaveChanges();
                    //            }
                    //        }
                    //    }
                    //}

                    ////Contracts
                    //root = @AppSettingsGet.LeaseDesk_UploadFile_Contrato + ProposalID + "\\";

                    //var contractfiles = new List<string>();
                    //int contractCount = HttpContext.Current.Request.Files.Count;
                    //if(contractCount > 0)
                    //{
                    //    if (!Directory.Exists(root))
                    //        System.IO.Directory.CreateDirectory(root);

                    //    for (int k = 0; k < documentCount; k++)
                    //    {

                    //        var contract = HttpContext.Current.Request.Files["contract" + k];
                    //        if (contract != null)
                    //        {
                    //            DocumentoData contractData = JsonConvert.DeserializeObject<DocumentoData>(
                    //                                            HttpContext.Current.Request.Params["contractData" + k]);

                    //            var postedFile = contract;
                    //            var filePath = root + postedFile.FileName;
                    //            postedFile.SaveAs(filePath);
                    //            contractfiles.Add(filePath);

                    //            using (var db = new BB_DB_DEV_LeaseDesk())
                    //            {
                    //                LD_DocumentProposal exists = db.LD_DocumentProposal.Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID && x.FileName == postedFile.FileName).FirstOrDefault();

                    //                if(exists != null)
                    //                {
                    //                    exists.ContratoID = ContractoId;
                    //                    db.Entry(exists).State = EntityState.Modified;
                    //                    db.SaveChanges();
                    //                }
                    //                else
                    //                {
                    //                    LD_DocumentProposal contractSave = new LD_DocumentProposal();
                    //                    //contractSave.ClassificationID = contractData.Type != null ? contractData.Type.ID : 5;
                    //                    contractSave.ClassificationID = contractData.Type != null ? contractData.Type : 5;

                    //                    contractSave.CreatedBy = proposal.CreatedBy;
                    //                    contractSave.CreatedTime = DateTime.Now;
                    //                    contractSave.QuoteNumber = proposal.CRM_QUOTE_ID;
                    //                    contractSave.SystemID = 1;
                    //                    contractSave.FileFullPath = filePath;
                    //                    contractSave.DocumentIsValid = false;
                    //                    contractSave.DocumentIsProcess = false;
                    //                    contractSave.FileName = Path.GetFileName(filePath);
                    //                    contractSave.ContratoID = ContractoId;
                    //                    contractSave.Comments = contractData != null ? contractData.Comments : "";
                    //                    db.LD_DocumentProposal.Add(contractSave);
                    //                    db.SaveChanges();
                    //                }

                    //            }
                    //        }
                    //    }

                    //}
                }



                if (isFirstTime)
                {
                    SentEmailPPAsync(proposal);
                }

                //ValidarTonersQulidade(proposal);

                if (isRetorno)
                {
                    await EnviarEmailleasedeskAsync(proposal, Observations);
                }

            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = ProposalID;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }

            //ProposalBLL p1 = new ProposalBLL();
            //LoadProposalInfo i = new LoadProposalInfo();
            //i.ProposalId = ProposalID.Value;
            //ActionResponse a = p1.LoadProposal(i);

            //err.ProposalObj = a1.ProposalObj;

            err.Message = "Proceso concluido y transferido al administración.";



            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UploadContract")]
        public async Task<HttpResponseMessage> UploadContract()
        {
            int? ProposalID = Int32.Parse(HttpContext.Current.Request.Params["ProposalID"]);
            ActionResponse err = new ActionResponse();
            StringBuilder c = new StringBuilder();
            
            try
            {
                if (ProposalID == null)
                {

                    err.Message = "Por favor, guarde su propuesta y vuelva a intentarlo.";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
                }
                BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ProposalID).FirstOrDefault();

                //Contracts
                string root = @AppSettingsGet.LeaseDesk_UploadFile_Contrato + ProposalID + "\\";

                var contractfiles = new List<string>();
                int filesCount = HttpContext.Current.Request.Files.Count;
                if (filesCount > 0)
                {
                    if (!Directory.Exists(root))
                        System.IO.Directory.CreateDirectory(root);

                    for (int k = 0; k < filesCount; k++)
                    {

                        var contract = HttpContext.Current.Request.Files["contract" + k];
                        if (contract != null)
                        {
                            DocumentoData contractData = JsonConvert.DeserializeObject<DocumentoData>(
                                                            HttpContext.Current.Request.Params["contractData" + k]);

                            var postedFile = contract;
                            var filePath = root + postedFile.FileName;
                            postedFile.SaveAs(filePath);
                            contractfiles.Add(filePath);

                            int? contractID = null;

                            using (var db = new BB_DB_DEV_LeaseDesk())
                            {
                                LD_Contrato ld = db.LD_Contrato.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                                if (ld != null)
                                {
                                    contractID = ld.ID;
                                }
                                LD_DocumentProposal contractSave = new LD_DocumentProposal();
                                //contractSave.ClassificationID = contractData.Type != null ? contractData.Type.ID : 5;
                                contractSave.ClassificationID = 5;

                                contractSave.CreatedBy = proposal.CreatedBy;
                                contractSave.CreatedTime = DateTime.Now;
                                contractSave.QuoteNumber = proposal.CRM_QUOTE_ID;
                                contractSave.SystemID = 1;
                                contractSave.FileFullPath = filePath;
                                contractSave.DocumentIsValid = false;
                                contractSave.DocumentIsProcess = false;
                                contractSave.FileName = Path.GetFileName(filePath);
                                contractSave.ContratoID = contractID;
                                contractSave.Comments = contractData != null ? contractData.Comments : "";
                                db.LD_DocumentProposal.Add(contractSave);
                                db.SaveChanges();
                            }
                        }
                    }
                } else if(HttpContext.Current.Request.Params["contract0"].Length > 0)
                {
                    int ind = 0;
                    string contractName = "contract" + ind;
                    do
                    {
                        using(var db = new BB_DB_DEV_LeaseDesk())
                        {
                            string fileName = HttpContext.Current.Request.Params["fileName" + ind];

                            LD_DocumentProposal conDoc = db.LD_DocumentProposal
                                .Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID && x.FileName == fileName)
                                .FirstOrDefault();

                            conDoc.Comments = JsonConvert.DeserializeObject<DocumentoData>(
                                                    HttpContext.Current.Request.Params["contractData" + ind])
                                                    .Comments;

                            db.LD_DocumentProposal.AddOrUpdate(conDoc);
                            db.SaveChanges();

                            ind++;
                            contractName = "contract" + ind;
                        }
                    } while (HttpContext.Current.Request.Params[contractName] != null);
                   /* List<LD_DocumentProposal> contracts = HttpContext.Current.Request.Params["contract"].ToList();
                    foreach (LD_DocumentProposal con in )
                    {
                        using (var db = new BB_DB_DEV_LeaseDesk())
                        {
                            LD_DocumentProposal conDoc = db.LD_DocumentProposal
                                .Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID && x.FileName == con.FileName)
                                .FirstOrDefault();

                        }
                    }*/
                }
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = ProposalID;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }

            err.Message = "Documento añadido con éxito";
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UploadDocument")]
        public async Task<HttpResponseMessage> UploadDocument()
        {
            int? ProposalID = Int32.Parse(HttpContext.Current.Request.Params["ProposalID"]);
            ActionResponse err = new ActionResponse();
            StringBuilder c = new StringBuilder();

            try
            {
                if (ProposalID == null)
                {

                    err.Message = "Por favor, guarde su propuesta y vuelva a intentarlo.";
                    return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
                }
                BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ProposalID).FirstOrDefault();

                //Contracts
                string root = @AppSettingsGet.LeaseDesk_UploadFile_Contrato + ProposalID + "\\";

                var documentfiles = new List<string>();
                int filesCount = HttpContext.Current.Request.Files.Count;
                if (filesCount > 0)
                {
                    if (!Directory.Exists(root))
                        System.IO.Directory.CreateDirectory(root);

                    for (int k = 0; k < filesCount; k++)
                    {

                        var document = HttpContext.Current.Request.Files["document" + k];
                        if (document != null)
                        {
                            DocumentoData documentData = JsonConvert.DeserializeObject<DocumentoData>(
                                                            HttpContext.Current.Request.Params["documentData" + k]);

                            var postedFile = document;
                            var filePath = root + postedFile.FileName;
                            postedFile.SaveAs(filePath);
                            documentfiles.Add(filePath);

                            int? contractID = null;
                            using (var db = new BB_DB_DEV_LeaseDesk())
                            {
                                LD_Contrato ld = db.LD_Contrato.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                                if(ld != null)
                                {
                                    contractID = ld.ID;
                                }
                                LD_DocumentProposal contractSave = new LD_DocumentProposal();
                                //contractSave.ClassificationID = documentData.Type != null ? documentData.Type.ID : 5;
                                contractSave.ClassificationID = documentData.Type != null ? documentData.Type : 11;

                                contractSave.CreatedBy = proposal.CreatedBy;
                                contractSave.CreatedTime = DateTime.Now;
                                contractSave.QuoteNumber = proposal.CRM_QUOTE_ID;
                                contractSave.SystemID = 1;
                                contractSave.FileFullPath = filePath;
                                contractSave.DocumentIsValid = false;
                                contractSave.DocumentIsProcess = false;
                                contractSave.FileName = Path.GetFileName(filePath);
                                contractSave.ContratoID = contractID;
                                contractSave.Comments = documentData != null ? documentData.Comments : "";
                                db.LD_DocumentProposal.Add(contractSave);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else if (HttpContext.Current.Request.Params["document0"].Length > 0)
                {
                    int ind = 0;
                    string documentName = "document" + ind;
                    do
                    {
                        using (var db = new BB_DB_DEV_LeaseDesk())
                        {
                            string fileName = HttpContext.Current.Request.Params["fileName" + ind];

                            LD_DocumentProposal conDoc = db.LD_DocumentProposal
                                .Where(x => x.QuoteNumber == proposal.CRM_QUOTE_ID && x.FileName == fileName)
                                .FirstOrDefault();

                            conDoc.Comments = JsonConvert.DeserializeObject<DocumentoData>(
                                                    HttpContext.Current.Request.Params["documentData" + ind])
                                                    .Comments;

                            db.LD_DocumentProposal.AddOrUpdate(conDoc);
                            db.SaveChanges();

                            ind++;
                            documentName = "contract" + ind;
                        }
                    } while (HttpContext.Current.Request.Params[documentName] != null);
                    
                }
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = ProposalID;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }

            List<LD_DocumentProposal> documentsDocs = new List<LD_DocumentProposal>();
            using (var db = new BB_DB_DEVEntities2())
            {
                var CRM_QUOTE_ID = db.BB_Proposal.Where(x => x.ID == ProposalID).Select(x => x.CRM_QUOTE_ID).FirstOrDefault();

                documentsDocs = db.LD_DocumentProposal.Where(x => x.QuoteNumber == CRM_QUOTE_ID && x.ClassificationID != 5).ToList();
            }

            err.Message = "Documento añadido con éxito";
            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetContrats")]
        public IHttpActionResult GetContrats(int proposalID)
        {
            try
            {
                List<LD_DocumentProposal> contractDocs = new List<LD_DocumentProposal>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    //LD_DocumentProposal - Contractos
                    var CRM_QUOTE_ID = db.BB_Proposal.Where(x => x.ID == proposalID).Select(x => x.CRM_QUOTE_ID).FirstOrDefault();

                    contractDocs = db.LD_DocumentProposal.Where(x => x.QuoteNumber == CRM_QUOTE_ID && x.ClassificationID == 5).ToList();
                }
                    
                return Ok(contractDocs);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocuments")]
        public IHttpActionResult GetDocuments(int proposalID)
        {
            try
            {
                List<LD_DocumentProposal> documentsDocs = new List<LD_DocumentProposal>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    //LD_DocumentProposal - Contractos
                    var CRM_QUOTE_ID = db.BB_Proposal.Where(x => x.ID == proposalID).Select(x => x.CRM_QUOTE_ID).FirstOrDefault();

                    documentsDocs = db.LD_DocumentProposal.Where(x => x.QuoteNumber == CRM_QUOTE_ID && x.ClassificationID != 5).ToList();
                }

                return Ok(documentsDocs);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        private async Task SentEmailPPAsync(BB_Proposal proposal)
        {
            bool isFamiliaPP = false;
            bool emailSDM = false;
            bool emailPMO = false;
            bool emailOthers = false;

            bool isPRS = false;
            bool isIMS = false;
            bool isMCS = false;
            bool isOPD = false;
            bool isPMO = false;
            bool isSDM = false;
            bool isPP = false;

            bool isQualidade = false;


            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    List<BB_Proposal_Quote> lstBB_Proposal_Quote = new List<BB_Proposal_Quote>();

                    List<BB_Proposal_OPSImplement> lstBB_Proposal_OPSImplement = new List<BB_Proposal_OPSImplement>();

                    List<BB_Proposal_OPSManage> lsBB_Proposal_OPSManage = new List<BB_Proposal_OPSManage>();

                    List<BB_Proposal_Quote_RS> lstBB_Proposal_Quote_RS = new List<BB_Proposal_Quote_RS>();

                    BB_Clientes cliente = db.BB_Clientes.Where(x => x.accountnumber == proposal.ClientAccountNumber).FirstOrDefault();

                    lstBB_Proposal_Quote = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposal.ID).ToList();
                    lstBB_Proposal_Quote_RS = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposal.ID).ToList();

                    lsBB_Proposal_OPSManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposal.ID).ToList();

                    lstBB_Proposal_OPSImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposal.ID).ToList();

                    if (lsBB_Proposal_OPSManage != null)
                    {
                        foreach (var item in lstBB_Proposal_Quote_RS)
                        {
                            if (item.CodeRef == "9960OD32006"
                               || item.CodeRef == "SRTU PPD"
                               || item.CodeRef == "SRTU AV"
                                || item.CodeRef == "996919-SP00048A")
                            {
                                isQualidade = true;
                                isFamiliaPP = true;
                            }
                        }
                    }

                    if (lstBB_Proposal_OPSImplement != null)
                    {
                        foreach (var item in lstBB_Proposal_OPSImplement)
                        {
                            if (item.CodeRef == "9960OD32006"
                               || item.CodeRef == "SRTU PPD"
                               || item.CodeRef == "SRTU AV"
                                || item.CodeRef == "996919-SP00048A")
                            {
                                isQualidade = true;
                                isFamiliaPP = true;
                            }
                        }
                    }



                    if (lstBB_Proposal_Quote != null && lstBB_Proposal_Quote.Count > 0)
                    {
                        foreach (var item in lstBB_Proposal_Quote)
                        {
                            if (item.Family == "PRSSW" || item.Family == "PRSPSV" || item.Family == "PRSMSV" ||
                                item.Family == "PRSCSV")
                            {
                                isPRS = true;
                                isFamiliaPP = true;
                            }

                            if (item.Family == "IMSPSV" || item.Family == "IMSMSV")
                            {
                                isIMS = true; isFamiliaPP = true;
                            }

                            if (item.Family == "MCSCSV" || item.Family == "MCSMSV" || item.Family == "MCSPSV")
                            {
                                isMCS = true; isFamiliaPP = true;
                            }

                            if (item.Family == "PRSHW" || item.Family == "OPSSV" || item.Family == "OPSHW")
                            {
                                isOPD = true; isFamiliaPP = true;
                            }


                            if (item.CodeRef == "9969PT00140"
                                || item.CodeRef == "9969PT00141"
                                || item.CodeRef == "9969PT00142"
                                || item.CodeRef == "9969PT00156"
                                || item.CodeRef == "9969PT00157"
                                || item.CodeRef == "9969PT00158"
                                || item.CodeRef == "9969PT00203")
                            {
                                isPMO = true; isFamiliaPP = true;
                            }

                            if (item.CodeRef == "9969PT00203")
                            {
                                isSDM = true; isFamiliaPP = true;

                            }

                            if (item.Family == "PPHW" || item.Family == "PPSW" || item.Family == "PPSRV")
                            {
                                isPP = true;
                                isFamiliaPP = true;
                            }

                            if (item.CodeRef == "9960OD32006"
                                || item.CodeRef == "SRTU PPD"
                                || item.CodeRef == "SRTU AV"
                                 || item.CodeRef == "996919-SP00048A")
                            {
                                isQualidade = true;
                                isFamiliaPP = true;
                            }

                        }
                    }

                    if (lstBB_Proposal_Quote_RS != null && lstBB_Proposal_Quote_RS.Count > 0)
                    {
                        foreach (var item in lstBB_Proposal_Quote_RS)
                        {
                            if (item.Family == "PRSSW" || item.Family == "PRSPSV" || item.Family == "PRSMSV" ||
                                item.Family == "PRSCSV")
                            {
                                isPRS = true; isFamiliaPP = true;
                            }

                            if (item.Family == "IMSPSV" || item.Family == "IMSMSV")
                            {
                                isIMS = true; isFamiliaPP = true;
                            }

                            if (item.Family == "MCSCSV" || item.Family == "MCSMSV" || item.Family == "MCSPSV")
                            {
                                isMCS = true; isFamiliaPP = true;
                            }

                            if (item.Family == "PRSHW" || item.Family == "OPSSV" || item.Family == "OPSHW")
                            {
                                isOPD = true; isFamiliaPP = true;
                            }


                            if (item.CodeRef == "9969PT00140"
                              || item.CodeRef == "9969PT00141"
                              || item.CodeRef == "9969PT00142"
                              || item.CodeRef == "9969PT00156"
                              || item.CodeRef == "9969PT00157"
                              || item.CodeRef == "9969PT00158"
                              || item.CodeRef == "9969PT00203")
                            {
                                isPMO = true; isFamiliaPP = true;
                            }

                            if (item.CodeRef == "9969PT00203")
                            {
                                isSDM = true; isFamiliaPP = true;

                            }

                            if (item.Family == "PPHW" || item.Family == "PPSW" || item.Family == "PPSRV")
                            {
                                isPP = true;
                                isFamiliaPP = true;
                            }

                            if (item.CodeRef == "9960OD32006"
                               || item.CodeRef == "SRTU PPD"
                               || item.CodeRef == "SRTU AV"
                                || item.CodeRef == "996919-SP00048A")
                            {
                                isQualidade = true;
                                isFamiliaPP = true;
                            }

                        }
                    }

                    if (isFamiliaPP)
                    {

                        StringBuilder builder = new StringBuilder();
                        builder.Append("<p style='color: #000000; background-color: #ffffff'>Configuración del negocio <br> </ p>");
                        builder.Append("<table border=1 ><tr>");

                        builder.Append("<table border=1 ><tr>");
                        builder.Append("<th style='font-family: Arial; font-size: 10pt;'>" + "Family" + "</th>");
                        builder.Append("<th style='font-family: Arial; font-size: 10pt;'>" + "CodRef" + "</th>");
                        builder.Append("<th style='font-family: Arial; font-size: 10pt;'>" + "Description" + "</th>");
                        builder.Append("<th style='font-family: Arial; font-size: 10pt;'>" + "Quantity" + "</th>");
                        builder.Append("</tr>");

                        foreach (var data in lstBB_Proposal_Quote)
                        {
                            builder.Append("<tr>");
                            builder.Append("<td>" + data.Family + "</td>");
                            builder.Append("<td>" + data.CodeRef + "</td>");
                            builder.Append("<td>" + data.Description + "</td>");
                            builder.Append("<td>" + data.Qty + "</td>");
                            builder.Append("</tr>");
                        }

                        foreach (var data in lstBB_Proposal_Quote_RS)
                        {
                            builder.Append("<tr>");
                            builder.Append("<td>" + data.Family + "</td>");
                            builder.Append("<td>" + data.CodeRef + "</td>");
                            builder.Append("<td>" + data.Description + "</td>");
                            builder.Append("<td>" + data.Qty + "</td>");
                            builder.Append("</tr>");
                        }

                        foreach (var data in lsBB_Proposal_OPSManage)
                        {
                            builder.Append("<tr>");
                            builder.Append("<td>" + data.Family + "</td>");
                            builder.Append("<td>" + data.CodeRef + "</td>");
                            builder.Append("<td>" + data.Description + "</td>");
                            builder.Append("<td>" + data.Quantity + "</td>");
                            builder.Append("</tr>");
                        }

                        foreach (var data in lstBB_Proposal_OPSImplement)
                        {
                            builder.Append("<tr>");
                            builder.Append("<td>" + data.Family + "</td>");
                            builder.Append("<td>" + data.CodeRef + "</td>");
                            builder.Append("<td>" + data.Description + "</td>");
                            builder.Append("<td>" + data.Quantity + "</td>");
                            builder.Append("</tr>");
                        }

                        builder.Append("</table>");


                        //Enviar email para o seguinte destinario
                        EmailService emailSend = new EmailService();
                        EmailMesage message = new EmailMesage();

                        // Corpo do email ----
                        message.Subject = "BusinessBuilder - Cierre del proceso - Quote: " + proposal.CRM_QUOTE_ID + " - Gestor de cuenta: " + proposal.CreatedBy;
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append("<p>Estimado(a),</p>");
                        strBuilder.Append("<p>Se ha registrado un nuevo proceso con la siguiente cotización: " + proposal.CRM_QUOTE_ID + "</p>");
                        strBuilder.Append("<br/>");
                        strBuilder.Append("<p>Gerente de cuenta: " + proposal.CreatedBy + "</p>");
                        strBuilder.Append("<p>Cliente: " + cliente.Name + "</p>");
                        strBuilder.Append("<p>NrConta: " + cliente.accountnumber + " </p>");
                        strBuilder.Append("<br/>");
                        strBuilder.Append(builder);
                        strBuilder.Append("<br/>");
                        strBuilder.Append("<p>A su disposición,</p>");
                        strBuilder.Append("<p>Business Builder</p>");
                        message.Body = strBuilder.ToString();



                        StringBuilder email = new StringBuilder();
                        if (isPRS)
                        {
                            email.Append("BPT_PRS_CE_DL@konicaminolta.pt;");
                            message.Destination = "BPT_PRS_CE_DL_@connectkonicaminolta.onmicrosoft.com;";

                            await emailSend.SendEmailaync(message);
                        }
                        if (isIMS)
                        {
                            email.Append("BPT_IMS_CE_DL@konicaminolta.pt;");
                            message.Destination = "BPT_IMS_CE_DL_@connectkonicaminolta.onmicrosoft.com;";

                            await emailSend.SendEmailaync(message);
                        }
                        if (isMCS)
                        {
                            email.Append("bpt_mcs_dl@konicaminolta.pt;");
                            message.Destination = "bpt_mcs_dl_@connectkonicaminolta.onmicrosoft.com;";

                            await emailSend.SendEmailaync(message);
                        }
                        if (isOPD)
                        {
                            email.Append("BPT_OPD_CE_DL@konicaminolta.pt;");
                            message.Destination = "BPT_OPD_CE_DL_@connectkonicaminolta.onmicrosoft.com;";

                            await emailSend.SendEmailaync(message);
                            //email.Append("nuno.silva@konicaminolta.pt");
                        }

                        if (isPMO)
                        {
                            email.Append("bpt_pmo_dl@konicaminolta.pt;");
                            message.Destination = "bpt_pmo_dl_@connectkonicaminolta.onmicrosoft.com;";

                            await emailSend.SendEmailaync(message);
                        }

                        if (isSDM)
                        {
                            email.Append("BPT_SDM_DL@konicaminolta.pt;");
                            message.Destination = "BPT_SDM_DL_@connectkonicaminolta.onmicrosoft.com;";

                            await emailSend.SendEmailaync(message);
                        }

                        if (isPP)
                        {
                            email.Append("BPT_DL_PP_Presale@connectkonicaminolta.onmicrosoft.com;");
                            message.Destination = "BPT_DL_PP_Presale_@konicaminolta.pt;";

                            await emailSend.SendEmailaync(message);
                        }

                        if (isQualidade)
                        {
                            email.Append("qualidade@konicaminolta.pt;");
                            message.Destination = "qualidade@konicaminolta.pt;";

                            await emailSend.SendEmailaync(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposal.ID;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }
        }

        private void ValidarTonersQulidade(BB_Proposal proposal)
        {
            //using (var db = new BB_DB_DEVEntities2())
            //{
            //    //List<BB_Proposal_Quote> produtos = 
            //}

            //    //EMAIL SEND
            //    EmailService emailSend = new EmailService();
            //EmailMesage message = new EmailMesage();

            //message.Destination = "Qualidade@konicaminolta.pt";
            //message.Subject = "BB - Qualidade Toners - Quote: " + proposal.CRM_QUOTE_ID +"" ;
            ////message.Subject = "Business Team - ";
            //StringBuilder strBuilder = new StringBuilder();
            //strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
            //strBuilder.Append("<br/>");
            //strBuilder.Append("Foi solicitado um pedido de informação para o Prazo de " + p.Months + " mes(es), para o seguinte cliente:");
            //strBuilder.Append("<br/>");
            //strBuilder.Append("Cliente: " + nameCliente);
            //strBuilder.Append("<br/>");
            //strBuilder.Append("NIF: " + nif);
            //strBuilder.Append("<br/>");
            //strBuilder.Append("Para aceder à aplicação Business Builder e responder, por favor utilize o seguinte link: " + "https://bb.konicaminolta.pt/LeaseDeskRent/Index" + " e selecionar a opção Aprovações Pendentes.");
            //strBuilder.Append("<br/>");
            //strBuilder.Append("<br/>");
            //strBuilder.Append("Business Builder - Por favor, não responder a este email.");
            //message.Body = strBuilder.ToString();

            //await emailSend.SendEmailaync(message);
        }

        private async Task EnviarEmailleasedeskAsync(BB_Proposal proposal, string observaçoes)
        {
            //EMAIL SEND
            EmailService emailSend = new EmailService();
            EmailMesage message = new EmailMesage();

            message.Destination = AppSettingsGet.PedidoPrazoDiferenciadoEmail;
            message.Subject = "Leasedesk - Proceso reenviado - Quote: " + proposal.CRM_QUOTE_ID + "";
            //message.Subject = "Business Team - ";
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
            strBuilder.Append("<br/>");
            strBuilder.Append("Fue reenviado un proceso al Leasedesk por " + proposal.CreatedBy);
            strBuilder.Append("<br/>");
            strBuilder.Append("Quote: " + proposal.CRM_QUOTE_ID);
            strBuilder.Append("<br/>");
            strBuilder.Append("Observaciones: " + (!string.IsNullOrEmpty(observaçoes) ? observaçoes : "NA"));
            strBuilder.Append("<br/>");
            strBuilder.Append("Para acceder a la aplicación Business Builder y responder, por favor utilice el siguiente enlace: " + "https://bb.konicaminolta.pt/LeaseDeskRent/Index" + " y seleccione la opción 'Aprobaciones Pendientes.");
            strBuilder.Append("<br/>");
            strBuilder.Append("<br/>");
            strBuilder.Append("Business Builder - Por favor, no responda a este correo electrónico.");
            message.Body = strBuilder.ToString();

            await emailSend.SendEmailaync(message);
        }

        private StringBuilder ValidarProcessoVenda(List<ProposalContact> lstContactDocumentantion, List<ProposalContact> lstContactSign, BB_Proposal proposal, BB_Proposal_PrazoDiferenciado pz, string SigningType, string SigningTypeObservations, BB_Proposal_Financing ft, ActionResponse a)
        {
            StringBuilder strErro = new StringBuilder();
            try
            {

                //COntacto Documentacao
                //bool encontrouDocumentation = db.BB_Proposal_Contacts_Documentation.Any(x => x.ProposalID == proposal.ID);
                //if (encontrouDocumentation)
                //{
                //    if (lstContactDocumentantion.Count > 0)
                //    {
                //        foreach (var ContactDocumentantion in lstContactDocumentantion)
                //        {
                //            BB_Proposal_Contacts_Documentation ca = new BB_Proposal_Contacts_Documentation();

                //            ca.Email = ContactDocumentantion.Email;
                //            ca.Name = ContactDocumentantion.Name;
                //            ca.Telefone = ContactDocumentantion.Phone;
                //            ca.ProposalID = proposal.ID;
                //            db.BB_Proposal_Contacts_Documentation.Add(ca);
                //        }

                //        db.SaveChanges();
                //    }
                //}

                //if (!encontrouDocumentation)
                //{
                //    if (lstContactDocumentantion.Count > 0)
                //    {
                //        foreach (var ContactDocumentantion in lstContactDocumentantion)
                //        {
                //            BB_Proposal_Contacts_Documentation ca = new BB_Proposal_Contacts_Documentation();

                //            ca.Email = ContactDocumentantion.Email;
                //            ca.Name = ContactDocumentantion.Name;
                //            ca.Telefone = ContactDocumentantion.Phone;
                //            ca.ProposalID = proposal.ID;
                //            db.BB_Proposal_Contacts_Documentation.Add(ca);
                //        }

                //        db.SaveChanges();
                //    }
                //    else
                //    {
                //        strErro.AppendLine("É preciso preencher o contacto de Documentação!");
                //    }
                //}

                //Contacto Assinaturara
                /*
                bool encontrouSigin = db.BB_Proposal_Contacts_Signing.Any(x => x.ProposalID == proposal.ID);
                if (encontrouSigin)
                {
                    if (lstContactSign.Count > 0 && lstContactSign[0].Email != "" && lstContactSign[0].Name != "" && lstContactSign[0].Telefone != "")
                    {
                        foreach (var ContactSign in lstContactSign)
                        {
                            BB_Proposal_Contacts_Signing ca = new BB_Proposal_Contacts_Signing();

                            ca.Email = ContactSign.Email;
                            ca.Name = ContactSign.Name;
                            ca.Telefone = ContactSign.Telefone;
                            ca.ProposalID = proposal.ID;
                            db.BB_Proposal_Contacts_Signing.Add(ca);
                        }

                        db.SaveChanges();
                    }
                }
                
                if (!encontrouSigin)
                {
                    if (lstContactSign.Count > 0 && lstContactSign[0].Email != "" && lstContactSign[0].Name != "" && lstContactSign[0].Telefone != "")
                    {
                        foreach (var ContactSign in lstContactSign)
                        {
                            BB_Proposal_Contacts_Signing ca = new BB_Proposal_Contacts_Signing();

                            ca.Email = ContactSign.Email;
                            ca.Name = ContactSign.Name;
                            ca.Telefone = ContactSign.Telefone;
                            ca.ProposalID = proposal.ID;
                            db.BB_Proposal_Contacts_Signing.Add(ca);
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        strErro.AppendFormat("¡Es necesario completar el contacto de la firma!", Environment.NewLine);
                    }
                }
                */

                //DOCUMENTacAo
                bool encontrouConct = db.BB_Proposal_Contacts_Documentation.Any(x => x.ProposalID == proposal.ID);
                if (encontrouConct)
                {
                    if (lstContactDocumentantion.Count > 0 && lstContactDocumentantion[0].Email != "" && lstContactDocumentantion[0].Name != "" && lstContactDocumentantion[0].Telefone != "")
                    {
                        foreach (var ContactSign1 in lstContactDocumentantion)
                        {
                            BB_Proposal_Contacts_Documentation ca = new BB_Proposal_Contacts_Documentation();

                            ca.Email = ContactSign1.Email;
                            ca.Name = ContactSign1.Name;
                            ca.Telefone = ContactSign1.Telefone;
                            ca.ProposalID = proposal.ID;
                            db.BB_Proposal_Contacts_Documentation.Add(ca);
                        }

                        db.SaveChanges();
                    }
                }

                if (!encontrouConct)
                {
                    if (lstContactDocumentantion.Count > 0 && 
                        lstContactDocumentantion[0].Email != "" && 
                        lstContactDocumentantion[0].Name != "" && 
                        lstContactDocumentantion[0].Telefone != "" && 
                        !string.IsNullOrWhiteSpace(lstContactDocumentantion[0].Email) &&
                        !string.IsNullOrWhiteSpace(lstContactDocumentantion[0].Name) &&
                        !string.IsNullOrWhiteSpace(lstContactDocumentantion[0].Telefone))
                    {
                        foreach (var ContactSign in lstContactDocumentantion)
                        {
                            BB_Proposal_Contacts_Documentation ca = new BB_Proposal_Contacts_Documentation();
                            ca.Email = ContactSign.Email;
                            ca.Name = ContactSign.Name;
                            ca.Telefone = ContactSign.Telefone;
                            ca.ProposalID = proposal.ID;
                            db.BB_Proposal_Contacts_Documentation.Add(ca);
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        strErro.AppendFormat("Es necesario completar el contacto para el pedido de documentación.", Environment.NewLine);
                    }
                }

                if (SigningType == null || SigningType == "")
                {
                    strErro.AppendFormat("Es necesario completar el tipo de firma en la página de Adjudicación.", Environment.NewLine);
                }

                if (SigningType == "Outro" && (SigningTypeObservations == null || SigningTypeObservations == ""))
                {
                    strErro.AppendFormat("En el tipo de firma 'Otro', debe completar el campo de Observaciones.", Environment.NewLine);
                }
                /*
                if (ft.FinancingTypeCode != 0 && pz == null)
                {
                    strErro.AppendFormat("Debe realizar el pedido de Aprobación Financiera.", Environment.NewLine);

                }

                if (ft.FinancingTypeCode != 0 && pz != null)
                {
                    if (!pz.IsComplete.Value)
                        strErro.AppendFormat("Solicitud de aprobación en análisis.", Environment.NewLine);
                    if (pz.IsComplete.Value && pz.IsAproved != null && !pz.IsAproved.Value)
                        strErro.AppendFormat("Solicitud de aprobación rechazada.", Environment.NewLine);
                }


                double servicosReecorrents = 0;
                servicosReecorrents = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposal.ID).Sum(x => x.TotalNetsale).GetValueOrDefault() + db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposal.ID).Sum(x => x.UnitDiscountPrice * x.TotalMonths * x.Quantity).GetValueOrDefault();

                if (pz != null && ft.FinancingTypeCode != 0)
                {
                    double diferenca1Euro = 0;
                    if ((proposal.SubTotal.Value - servicosReecorrents) > pz.ValorFinanciamento.Value)
                        diferenca1Euro = (proposal.SubTotal.Value - servicosReecorrents) - pz.ValorFinanciamento.Value;
                    else
                        diferenca1Euro = pz.ValorFinanciamento.Value - (proposal.SubTotal.Value - servicosReecorrents);

                    if (diferenca1Euro > 1)
                        strErro.AppendFormat("O valor financiado do negócio " + (proposal.SubTotal.Value - servicosReecorrents) + " € es diferente de la solicitud de aprobación en el momento en que fue solicitada (" + pz.ValorFinanciamento.Value + "€), Por favor, haga una nueva solicitud.", Environment.NewLine);
                }

                if (pz != null && ft.FinancingTypeCode != 0 && pz.FinancingID != ft.FinancingTypeCode)
                {
                    strErro.AppendFormat("El tipo de financiamiento es diferente a la solicitud de aprobación financiera.");
                }
                */
                string frequencyPrinting = "";
                ApprovedPrintingService activePS = null;
                if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                    if (activePS != null && activePS.GlobalClickVVA != null)
                    {
                        frequencyPrinting = activePS.GlobalClickVVA.RentBillingFrequency == 1 ? "M" : "T";
                    }
                    if (activePS != null && activePS.GlobalClickNoVolume != null)
                    {
                        frequencyPrinting = activePS.GlobalClickNoVolume.PageBillingFrequency == 1 ? "M" : "T";
                    }
                }

                if (pz != null && pz.FinancingID == 6)
                {
                    string frequnecyPrazo = pz.Frequency == 1 ? "M" : "T";

                    if (frequnecyPrazo != frequencyPrinting)
                    {
                        strErro.AppendFormat("La periodicidad (mensual o trimestral) del financiamiento es diferente a la cotización activa del servicio (mensual o trimestral).");
                    }
                }


                if (pz != null && pz.DataExpiracao != null)
                {
                    if (pz.DataExpiracao <= DateTime.Now)
                        strErro.AppendFormat("Se ha superado la fecha de expiración de la aprobación de crédito. ¡Solicite un nuevo Pedido de Financiamiento!");
                }


                //BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                //if(ps2 != null)
                //{
                //    BB_PrintingServices activePS = ps2.BB_PrintingServices.ToList()[ps2.ActivePrintingService.Value - 1];
                //    if(activePS != null)
                //    {
                //        BB_Proposal_PrintingServiceValidationRequest serviceRequest = activePS.BB_Proposal_PrintingServiceValidationRequest.FirstOrDefault();
                //        if(serviceRequest != null)
                //        {
                //            var requestedAt = serviceRequest.RequestedAt;
                //            var approvedAt = serviceRequest.ApprovedAt;
                //        }
                //    }
                //}

                //ApprovedPrintingService activePS = null;
                //if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null && (a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1) >= 0 && a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1] != null)
                //{
                //    activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                //    if (activePS != null)
                //    {

                //    }

                //}

                //if (pz != null && a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                //{
                //    a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[0].GlobalClickVVA.RentBillingFrequency
                //}



            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposal.ID;
                log.Error(ex.Message.ToString(), ex);
                return strErro;
            }



            return strErro;
        }
        private StringBuilder ValidarProcessoVenda_encomenda(BB_Proposal_PrazoDiferenciado pz, BB_Proposal_Financing ft, ActionResponse a, BB_Proposal proposal)
        {
            StringBuilder strErro = new StringBuilder();
            try
            {
                if (ft.FinancingTypeCode != 0 && pz != null)
                {
                    if (!pz.IsComplete.Value)
                        strErro.AppendFormat("Solicitud de aprobación en análisis.", Environment.NewLine);
                    if (pz.IsComplete.Value && pz.IsAproved != null && !pz.IsAproved.Value)
                        strErro.AppendFormat("Solicitud de aprobación rechazada.", Environment.NewLine);
                }

                double servicosReecorrents = 0;
                servicosReecorrents = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposal.ID).Sum(x => x.TotalNetsale).GetValueOrDefault() + db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposal.ID).Sum(x => x.UnitDiscountPrice * x.TotalMonths * x.Quantity).GetValueOrDefault();
                if (pz != null && ft.FinancingTypeCode != 0)
                {
                    if (pz.ValorFinanciamento.Value != (proposal.SubTotal.Value - servicosReecorrents))
                        strErro.AppendFormat("El valor financiado del negocio " + (proposal.SubTotal.Value - servicosReecorrents) + " € es diferente al pedido de aprobación en el momento en que fue solicitado (" + pz.ValorFinanciamento.Value + "€), por favor, haga una nueva solicitud..", Environment.NewLine);
                }

                if (pz != null && ft.FinancingTypeCode != 0 && pz.FinancingID != ft.FinancingTypeCode)
                {
                    strErro.AppendFormat("El tipo de financiamiento es diferente al solicitado en la aprobación financiera.");
                }


                //BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                //if(ps2 != null)
                //{
                //    BB_PrintingServices activePS = ps2.BB_PrintingServices.ToList()[ps2.ActivePrintingService.Value - 1];
                //    if(activePS != null)
                //    {
                //        BB_Proposal_PrintingServiceValidationRequest serviceRequest = activePS.BB_Proposal_PrintingServiceValidationRequest.FirstOrDefault();
                //        if(serviceRequest != null)
                //        {
                //            var requestedAt = serviceRequest.RequestedAt;
                //            var approvedAt = serviceRequest.ApprovedAt;
                //        }
                //    }
                //}

                //ApprovedPrintingService activePS = null;
                //if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null && (a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1) >= 0 && a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1] != null)
                //{
                //    activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                //    if (activePS != null)
                //    {

                //    }

                //}

                //if (pz != null && a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                //{
                //    a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[0].GlobalClickVVA.RentBillingFrequency
                //}



            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposal.ID;
                log.Error(ex.Message.ToString(), ex);
                return strErro;
            }



            return strErro;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetFinancingDocumentos")]
        public List<string> GetFinancingDocumentos(int? id)
        {
            List<LD_FinancingDocument> fd = db.LD_FinancingDocument.Where(x => x.TipoFinanciamentoID == id).ToList();
            List<string> lst = new List<string>();


            foreach (var item in fd)
            {
                string docuemnto = db.LD_DocumentClassification.Where(x => x.ID == item.DocumentID && item.Email.Value == true).Select(x => x.Classification).FirstOrDefault();

                if (!String.IsNullOrEmpty(docuemnto))
                {
                    lst.Add(docuemnto);
                }
            }

            return lst;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ClienteAdjudicadoEmail_Apagar")]
        public async Task<HttpResponseMessage> ClienteAdjudicadoEmail_Apagar(int? id)
        {
            ActionResponse err = new ActionResponse();
            BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == id).FirstOrDefault();

            try
            {

                BB_Proposal_Financing ft = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposal.ID).FirstOrDefault();
                BB_Proposal_PrazoDiferenciado pz = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposal.ID && x.IsComplete.Value == true).FirstOrDefault();

                if (pz != null)
                {
                    List<LD_FinancingDocument> fd = db.LD_FinancingDocument.Where(x => x.TipoFinanciamentoID == ft.FinancingTypeCode).ToList();

                    if (proposal != null)
                    {
                        EmailService emailSend = new EmailService();
                        EmailMesage message = new EmailMesage();

                        message.Destination = "joao.reis@konicaminolta.pt";
                        message.Subject = "Solicitud de documentos para seguimiento del proceso - Quote: " + proposal.CRM_QUOTE_ID;
                        //message.Subject = "Business Team - ";
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append("<p>Estimado/a cliente,</p>");
                        strBuilder.Append("Agradecemos la confianza por haber elegido nuestros servicios.");
                        strBuilder.Append("<p>Para dar seguimiento a su proceso, solicitamos el envío de la documentación que figura en la lista a continuación.</p>");
                        strBuilder.Append("<b>Lista de documentos a enviar:</b>");
                        strBuilder.Append("<br/>");
                        foreach (var item in fd)
                        {
                            string docuemnto = db.LD_DocumentClassification.Where(x => x.ID == item.DocumentID && item.Email.Value == true).Select(x => x.Classification).FirstOrDefault();

                            if (!String.IsNullOrEmpty(docuemnto))
                            {
                                strBuilder.Append("- " + docuemnto + ".");
                                strBuilder.Append("<br/>");
                            }


                        }
                        strBuilder.Append("<p>Para enviarnos los documentos, simplemente responda a este correo electrónico adjuntando los documentos solicitados. Por favor, no cambie el  “Assunto/Subject”.</p>");
                        strBuilder.Append("<p>A su disposición,</p>");
                        strBuilder.Append("<p>Konica Minolta</p>");
                        message.Body = strBuilder.ToString();

                        await emailSend.SendEmailaync(message);

                        err.Message = "Correo electrónico enviado al cliente.";

                        proposal.StatusID = 11;
                        db.Entry(proposal).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposal.ID;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }



            return Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, err);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProcessosVendas_OLD")]
        public IHttpActionResult GetProcessosVendas_OLD(UserName1 a)
        {

            List<BB_ProposalModel> listModel = new List<BB_ProposalModel>();
            List<BB_Proposal> list = new List<BB_Proposal>();
            try
            {


                using (var db = new BB_DB_DEVEntities2())
                {

                    List<BB_PROPOSALS_GET> listProposals = new List<BB_PROPOSALS_GET>();
                    List<string> userEmails = new List<string>() { a.username };
                    userEmails.AddRange(dbUsers.AspNetUsers.Where(x => x.ManagerEmail == a.username).Select(u => u.Email));
                    //List<BB_Proposal> proposals = db.BB_Proposal.Where(x => (userEmails.Contains(x.CreatedBy) || userEmails.Contains(x.AccountManager)) && x.ToDelete == false).ToList();


                    if (a.username == "Joao.Reis@konicaminolta.pt" || a.username == "Claudia.Nunes@KonicaMinolta.pt"
                        || a.username == "Maria.Sousa@KonicaMinolta.pt" || a.username == "Diogo.Pimenta@konicaminolta.pt"
                        || a.username == "Vitor.Medeiros@konicaminolta.pt")
                    {
                        list = db.BB_Proposal.Where(x => x.ToDelete == false).ToList();
                    }
                    else if (userEmails.Count() > 0)
                    {
                        list = db.BB_Proposal.Where(x => (userEmails.Contains(x.CreatedBy) || userEmails.Contains(x.AccountManager)) && x.ToDelete == false).ToList();
                    }
                    else
                    {
                        list = db.BB_Proposal.Where(x => x.CreatedBy == a.username && x.ToDelete == false).ToList();
                    }


                    foreach (var i in list)
                    {
                        try
                        {
                            var config1 = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<BB_Proposal, BB_ProposalModel>();
                            });

                            IMapper iMapper1 = config1.CreateMapper();

                            BB_ProposalModel m = iMapper1.Map<BB_Proposal, BB_ProposalModel>(i);

                            BB_Proposal b = db.BB_Proposal.Where(x => x.ID == m.ID).FirstOrDefault();

                            m.StatusName = db.BB_Proposal_Status.Where(x => x.ID == i.StatusID).Select(x => x.Description).FirstOrDefault();

                            using (var db1 = new masterEntities())
                            {

                                m.AccountManager = db1.AspNetUsers.Where(x => x.Email == i.ModifiedBy).Select(x => x.ManagerEmail).FirstOrDefault();
                            }
                            using (var db1 = new BB_DB_DEV_LeaseDesk())
                            {
                                LD_Contrato c = db1.LD_Contrato.Where(x => x.ProposalID == b.ID).FirstOrDefault();

                                if (c != null)
                                {
                                    m.LeasedeskModifiedBy = c.ModifiedBy;
                                    m.ModifiedTime = c.ModifiedTime;
                                    m.Leasedesk = db1.LD_Observacoes_Motivos.Where(x => x.ID == c.MotivoID).Select(x => x.Motive).FirstOrDefault();
                                }
                                else
                                {
                                    m.Leasedesk = "NA";
                                }

                            }

                            BB_Clientes cli = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();


                            if (cli == null)
                                cli = new BB_Clientes();

                            m.ClientAccountNumber = cli.accountnumber;
                            m.ClientName = cli.Name;

                            listModel.Add(m);
                        }
                        catch (Exception ex)
                        {
                            log4net.ThreadContext.Properties["proposal_id"] = 0;
                            log.Error(ex.Message.ToString(), ex);
                            return NotFound();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = 0;
                log.Error(ex.Message.ToString(), ex);
                return NotFound();
            }

            return Ok(listModel.OrderByDescending(x => x.CreatedTime).ToList());
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProcessosVendas")]
        public List<BB_PROPOSALS_GET_V1> GetProcessosVendas([FromBody] UserName1 Owner)
        {
            //AspNetUsers user = dbUsers.AspNetUsers.Where(x => x.UserName == Owner.Owner).FirstOrDefault();
            List<BB_PROPOSALS_GET_V1> lst = new List<BB_PROPOSALS_GET_V1>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_Proposal_Resumo", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userEmail", Owner.username);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        try
                        {
                            BB_PROPOSALS_GET_V1 m = new BB_PROPOSALS_GET_V1();
                            m.ID = (int)rdr["ID"];
                            m.AccountNumber = rdr["NCliente"].ToString();
                            m.ClientName = rdr["Cliente"].ToString();
                            m.Name = rdr["Name"].ToString();
                            //m.Description = rdr["Description"].ToString();
                            m.TotalValue = rdr["valor"].ToString() != "" ? (double.Parse(rdr["valor"].ToString())) : 0;
                            m.QuoteCRM = rdr["QuoteCRM"].ToString();
                            m.ModifiedTime = rdr["UltimaModificacao"] != null ? DateTime.Parse(rdr["UltimaModificacao"].ToString()) : new DateTime();
                            m.CreatedTime = rdr["CreatedTime"] != null ? DateTime.Parse(rdr["CreatedTime"].ToString()) : new DateTime();
                            //m.Status = new BB_Proposal_Status
                            //{
                            //    Description = rdr["Estado"].ToString(),
                            //    Phase = (int)rdr["Phase"],
                            //    ID = (int)rdr["EstadoID"],
                            //};
                            m.FinancingStatus = rdr["Financeiro"].ToString();
                            m.ServiceStatus = rdr["Servico"].ToString();
                            m.LeaseDeskStatus = rdr["PosVenda"].ToString();
                            m.CreatedByEmail = rdr["CreatedByEmail"].ToString();
                            m.CreatedByName = rdr["CreatedByName"].ToString();
                            m.AccountManagerName = rdr["AccountManagerName"].ToString();
                            m.AccountManagerEmail = rdr["AccountManagerEmail"].ToString();
                            m.ModifiedByEmail = rdr["ModifiedByEmail"].ToString();
                            m.ModifiedByName = rdr["ModifiedByName"].ToString();
                            lst.Add(m);
                        }
                        catch (Exception ex)
                        {
                            log4net.ThreadContext.Properties["proposal_id"] = 0;
                            log.Error(ex.Message.ToString(), ex);
                            ex.Message.ToString();
                        }
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = 0;
                log.Error(ex.Message.ToString(), ex);
                ex.Message.ToString();
            }


            return lst;
        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProposta")]
        public IHttpActionResult GetProposta(int? proposalID)
        {
            ProposalBLL p1 = new ProposalBLL();
            LoadProposalInfo i = new LoadProposalInfo();
            i.ProposalId = proposalID.Value;
            ActionResponse a = p1.LoadProposal(i);

            double? LeiDaCopiaPriada = 0;
            double? sobrevalorizacao = 0;
            double? retomas = 0;
            BB_Proposal pr1 = new BB_Proposal();
            BB_Proposal_PrazoDiferenciado prazoDiferenciado1 = new BB_Proposal_PrazoDiferenciado();
            using (var db = new BB_DB_DEVEntities2())
            {
                pr1 = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();

                List<BB_Proposal_Quote> lstQuotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                foreach (var quote in lstQuotes)
                {
                    double? TCP = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).Select(x => x.TCP).FirstOrDefault();

                    var contador = db.BB_Proposal_Counters.Where(x => x.OSID == quote.ID).Count();

                    if (TCP is null)
                        TCP = 0;

                    if (quote.IsUsed.GetValueOrDefault() == false)
                        LeiDaCopiaPriada = (TCP * quote.Qty) + LeiDaCopiaPriada;

                }

                List<BB_Proposal_Overvaluation> o = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalID).ToList();
                foreach (var quote in o)
                {
                    sobrevalorizacao += quote.Total;
                }

                List<BB_Proposal_Upturn> ret = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposalID).ToList();
                foreach (var quote1 in ret)
                {
                    retomas += quote1.Total;
                }

                string nLocadora = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).Select(x => x.NLocadora).FirstOrDefault();
                prazoDiferenciado1 = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).FirstOrDefault();


                if (a.ProposalObj.Draft.financing.diffTerm != null)
                    a.ProposalObj.Draft.financing.diffTerm.Nlocadora = nLocadora;
            }


            //sobrevalorizacao = (sobrevalorizacao + retomas) - retomas;

            a.ProposalObj.Draft.details.ValueTotal = a.ProposalObj.Draft.details.ValueTotal + LeiDaCopiaPriada.Value + sobrevalorizacao.Value;


            double? sobrevalorizacao1 = 0;
            sobrevalorizacao1 = sobrevalorizacao - retomas;
            //double? sobrevalorizacao = 0;
            //sobrevalorizacao = a.ProposalObj.Draft.overvaluations.Select(x => x.Total).FirstOrDefault();
            double? OPSHWvalorTotal = 0;
            double? OPSHWUnti = 0;
            if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0 && sobrevalorizacao1 > 0)
            {
                if (a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").Count() > 0)
                {


                    OPSHWvalorTotal = a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(c => c.TotalNetsale)).First();

                    OPSHWUnti = a.ProposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(c => c.UnitDiscountPrice)).First();
                }
            }

            if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0)
            {

                a.ProposalObj.Draft.details.ValueTotal = 0;
                foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                {
                    quote.TotalNetsale = sobrevalorizacao1 != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao1) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                    quote.UnitDiscountPrice = Math.Round((quote.TotalNetsale / quote.Qty), 2);
                    //quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;

                    a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                }
                a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
            }
            else
            {
                //a.ProposalObj.Draft.details.ValueTotal = 0;
                //foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                //{
                //    a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                //}
                //a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
            }

            using (var db1 = new BB_DB_DEV_LeaseDesk())
            {
                LD_Contrato c = db1.LD_Contrato.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                if (c != null)
                {
                    a.ProposalObj.NUS = db1.LD_Contrato_Facturacao.Where(x => x.LDID == c.ID).Select(x => x.NUS).FirstOrDefault();
                    a.ProposalObj.FolderDoc = c.Pasta;
                    a.ProposalObj.LeasedeskComentariosGC = c.ComentariosGC;
                    a.ProposalObj.LeasedeskComentarios = c.Comments;
                    a.ProposalObj.LeasedeskComentariosDevolucao = c.ComentariosDevolucao;
                    a.ProposalObj.LeasedeskStatus = db1.LD_Observacoes_Motivos.Where(x => x.ID == c.MotivoID).Select(x => x.Motive).FirstOrDefault();
                }
                else
                {
                    a.ProposalObj.NUS = "NA";
                    a.ProposalObj.FolderDoc = "NA";
                    a.ProposalObj.LeasedeskComentariosGC = "NA";
                    a.ProposalObj.LeasedeskComentarios = "NA";
                    a.ProposalObj.LeasedeskComentariosDevolucao = "NA";
                    a.ProposalObj.LeasedeskStatus = "NA";
                }

            }

            ValoresTotaisRenda vt = new ValoresTotaisRenda();
            vt.RendaFinanciada = 0;
            vt.VVA = 0;
            if (a.ProposalObj.Draft.financing.FinancingTypeCode != 0 && a.ProposalObj.Draft.financing.diffTerm != null)
            {
                vt.RendaFinanciada = (a.ProposalObj.Draft.financing.diffTerm.Rent != null ? a.ProposalObj.Draft.financing.diffTerm.Rent : 0);


            }

            ApprovedPrintingService activePS = null;
            if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
            {
                activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                if (activePS != null && activePS.GlobalClickVVA != null)
                {

                    vt.VVA = activePS.GlobalClickVVA.PVP;
                    switch (activePS.GlobalClickVVA.RentBillingFrequency)
                    {
                        case 3:
                            activePS.BWVolume = activePS.BWVolume * 3;
                            activePS.CVolume = activePS.CVolume * 3;
                            vt.VVA = activePS.GlobalClickVVA.PVP * 3;
                            break;
                        case 6:
                            activePS.BWVolume = activePS.BWVolume * 6;
                            activePS.CVolume = activePS.CVolume * 6;
                            vt.VVA = activePS.GlobalClickVVA.PVP * 3;
                            break;
                        default: break;
                    }

                    //switch (activePS.GlobalClickVVA.RentBillingFrequency)
                    //{
                    //    case 3:
                    //        activePS.GlobalClickVVA.PVP = activePS.GlobalClickVVA != null ? activePS.GlobalClickVVA.PVP * 3 : 0;
                    //        break;
                    //    case 6:
                    //        activePS.GlobalClickVVA.PVP = activePS.GlobalClickVVA != null ? activePS.GlobalClickVVA.PVP * 6 : 0;
                    //        break;
                    //    default: break;
                    //}
                    //vt.VVA = activePS.GlobalClickVVA != null ? activePS.GlobalClickVVA.PVP : 0;
                }

            }

            if (retomas.Value > 0)
            {
                a.ProposalObj.Draft.details.ValueTotal -= retomas.Value;
            }

            a.ProposalObj.Draft.details.ValueTotal = pr1 != null && pr1.SubTotal != null ? pr1.SubTotal.Value : a.ProposalObj.Draft.details.ValueTotal;

            if (a.ProposalObj.Draft.baskets.rs_basket.Count() > 0 || a.ProposalObj.Draft.opsPacks.opsManage.Count() > 0)
                vt.ServicosRecorentesMes = a.ProposalObj.Draft.baskets.rs_basket.Sum(x => x.MonthlyFee) + a.ProposalObj.Draft.opsPacks.opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity);
            else
                vt.ServicosRecorentesMes = 0;

            if (a.ProposalObj.Draft.baskets.rs_basket.Count() > 0 || a.ProposalObj.Draft.opsPacks.opsManage.Count() > 0)
                vt.ServicosRecorentesTotal = a.ProposalObj.Draft.baskets.rs_basket.Sum(x => x.TotalNetsale) + a.ProposalObj.Draft.opsPacks.opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity * x.TotalMonths);
            else
                vt.ServicosRecorentesTotal = 0;


            vt.ConfiguracaoOneShotValor = a.ProposalObj.Draft.details.ValueTotal - vt.ServicosRecorentesTotal;

            double fee = (activePS != null && activePS.Fee != null ? activePS.Fee : 0);

            if (activePS != null && activePS.GlobalClickVVA != null)
            {
                switch (activePS.GlobalClickVVA.RentBillingFrequency)
                {
                    case 3:
                        //vt.RendaFinanciada = vt.RendaFinanciada * 3;
                        vt.ServicosRecorentesMes = vt.ServicosRecorentesMes * 3;
                        fee = fee * 3;
                        break;
                    case 6:
                        //vt.RendaFinanciada = vt.RendaFinanciada * 6;
                        vt.ServicosRecorentesMes = vt.ServicosRecorentesMes * 6;
                        fee = fee * 6;
                        break;
                    default: break;
                }
            }




            vt.LeiCopiaPrivada = LeiDaCopiaPriada;
            vt.RendaTotal = vt.VVA + vt.RendaFinanciada + vt.ServicosRecorentesMes + fee;
            if (prazoDiferenciado1 != null && prazoDiferenciado1.FinancingID == 6)
            {
                vt.RendaTotal = vt.RendaFinanciada;
            }
            vt.sobrevalorizacaoTotal = sobrevalorizacao != null && sobrevalorizacao.HasValue && sobrevalorizacao.Value != 0 ? sobrevalorizacao.Value : 0;
            vt.retomasTotal = retomas != null && retomas.HasValue && retomas.Value != 0 ? retomas.Value : 0;
            a.ProposalObj.valoretotais = vt;


            return Ok(a.ProposalObj);

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_BB_Proposal_Contract_Signin")]
        public IHttpActionResult Get_BB_Proposal_Contract_Signin(int? proposalID)
        {

            LD_Contrato contrcto = new LD_Contrato();
            List<BB_Proposal_Contacts_Signing> a = new List<BB_Proposal_Contacts_Signing>();
            using (var db = new BB_DB_DEVEntities2())
            {
                a = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();

            }

            return Ok(a);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocumentsProposal")]
        public IHttpActionResult GetDocumentsProposal(int? proposalID)
        {
            var httpRequest = HttpContext.Current.Request;

            List<DocumentProposal> list = new List<DocumentProposal>();
            try
            {
                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    //COUNTERS
                    LD_Contrato ld = db.LD_Contrato.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    if (ld != null)
                    {
                        List<LD_DocumentProposal> lstDocuments = db.LD_DocumentProposal.Where(x => x.ContratoID == ld.ID).ToList();
                        foreach (var item in lstDocuments)
                        {
                            var configCounter = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<LD_DocumentProposal, DocumentProposal>();
                            });

                            IMapper iMapperCounter = configCounter.CreateMapper();

                            var dc = iMapperCounter.Map<LD_DocumentProposal, DocumentProposal>(item);
                            dc.Classification = db.LD_DocumentClassification.Where(x => x.ID == item.ClassificationID).FirstOrDefault();
                            dc.System = db.LD_System.Where(x => x.ID == item.SystemID).FirstOrDefault();
                            list.Add(dc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log4net.ThreadContext.Properties["proposal_id"] = proposalID;
                log.Error(ex.Message.ToString(), ex);
                NotFound();
            }

            return Ok(list);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetHistorico")]
        public IHttpActionResult GetHistorico(int? proposalID)
        {
            List<HistoricoModel> lstH = new List<HistoricoModel>();
            List<LD_Observacoes_Motivos> lstLD_Observacoes_Motivos = new List<LD_Observacoes_Motivos>();
            List<BB_Logs_Proposal> l = new List<BB_Logs_Proposal>();

            List<BB_Proposal_PrazoDiferenciado_History> lstP = new List<BB_Proposal_PrazoDiferenciado_History>();
            HistoricoModel h = new HistoricoModel();
            using (var db = new BB_DB_DEVEntities2())
            {
                l = db.BB_Logs_Proposal.Where(x => x.ProposalID == proposalID).ToList();

                foreach (var i in l)
                {
                    h = new HistoricoModel();
                    h.ModifiedTime = i.ModifiedDate;
                    h.ModifiedBy = i.ModifiedBy;
                    switch (i.Value_New)
                    {
                        case "1":
                            h.Status = "Borrador";
                            break;
                        case "5":
                            h.Status = "Adjudicación";
                            break;
                        case "7":
                            h.Status = "Presentado en CRM";
                            break;
                        case "11":
                            h.Status = "LeaseDesk";
                            break;
                        default:
                            h.Status = "";
                            break;

                    };
                    h.Tipo = "BB";
                    lstH.Add(h);
                }

                lstP = db.BB_Proposal_PrazoDiferenciado_History.Where(x => x.ProposalID == proposalID).ToList();


                BB_Proposal_PrazoDiferenciado pd = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                if (pd != null)
                {
                    HistoricoModel h1 = new HistoricoModel();
                    h1.Tipo = "Aprocação Financeira";
                    h1.Status = "Pedido de Aprovação";
                    h1.ModifiedTime = pd.CreatedTime;
                    h1.ModifiedBy = pd.ModifiedBy;
                    lstH.Add(h1);
                }

                foreach (var j in lstP)
                {
                    HistoricoModel h1 = new HistoricoModel();
                    h1.Tipo = "Aprovação Financeira";
                    h1.Status = "Análise";
                    h1.ModifiedBy = j.ModifiedBy;
                    h1.ModifiedTime = j.ModifiedTime;
                    lstH.Add(h1);
                }

                if (pd != null && pd.IsComplete == true)
                {
                    HistoricoModel h1 = new HistoricoModel();
                    h1.Tipo = "Aprovação Financeira";
                    h1.Status = "Resposta da Aprovação";
                    h1.ModifiedTime = pd.ModifiedTime;
                    h1.ModifiedBy = pd.ModifiedBy;
                    h1.Description = pd.IsAproved != null && pd.IsAproved == true ? "Aprovado" : "Não Aprovado";
                    lstH.Add(h1);
                }
            }

            using (var db = new BB_DB_DEV_LeaseDesk())
            {
                List<LD_Contrato_History> listHistory = db.LD_Contrato_History.Where(x => x.ProposalID == proposalID).ToList();
                lstLD_Observacoes_Motivos = db.LD_Observacoes_Motivos.ToList();
                //foreach (var hist in listHistory)
                //{
                //    HistoricoModel h1 = new HistoricoModel();
                //    h1.Tipo = "LeaseDesk";
                //    h1.Status = 
                //    h1.Description = 
                //    h1.ModifiedTime = hist.ModifiedTime;
                //    h1.ModifiedBy = hist.ModifiedBy;
                //    lstH.Add(h1);
                //}

                foreach (var hist in listHistory)
                {
                    HistoricoModel h1 = new HistoricoModel();
                    //h1.Tipo = "LeaseDesk";
                    //h1.Status = db.LD_Status.Where(x => x.ID == hist.StatusID).Select(x => x.Status).FirstOrDefault();
                    h1.Tipo = db.LD_Status.Where(x => x.ID == hist.StatusID).Select(x => x.Status).FirstOrDefault();
                    h1.Status = db.LD_Observacoes_Motivos.Where(x => x.ID == hist.MotivoID).Select(x => x.Motive).FirstOrDefault();
                    h1.Description = lstLD_Observacoes_Motivos.Where(x => x.ID == hist.MotivoID).Select(x => x.Motive).FirstOrDefault() + " - " + hist.Comments;
                    h1.ModifiedTime = hist.ModifiedTime;
                    h1.ModifiedBy = hist.ModifiedBy;
                    lstH.Add(h1);
                }

                foreach (var hist in listHistory.Where(x => x.StatusID == 4).ToList())
                {
                    HistoricoModel h1 = new HistoricoModel();
                    h1.Tipo = "Retorno";
                    h1.Status = db.LD_Devolucao_Motivo.Where(x => x.ID == hist.MotivoID).Select(x => x.Motivo).FirstOrDefault();
                    h1.Description = hist.ComentariosDevolucao;
                    h1.ModifiedTime = hist.ModifiedTime;
                    h1.ModifiedBy = hist.ModifiedBy;
                    lstH.Add(h1);
                }

                List<LD_DocSign_Control_History> lstLD_DocSign_Control_History = db.LD_DocSign_Control_History.Where(x => x.ProposalID == proposalID).ToList();

                foreach (var hist in lstLD_DocSign_Control_History)
                {
                    if (hist.DocSignStatusSent == 0)
                    {
                        HistoricoModel h1 = new HistoricoModel();
                        h1.Tipo = "DocSign";
                        h1.Status = "Ready";
                        h1.Description = "";
                        h1.ModifiedTime = hist.ModifiedTime;
                        h1.ModifiedBy = hist.ModifiedBy;
                        lstH.Add(h1);
                    }

                    if (hist.DocSignStatusSent == 1)
                    {
                        HistoricoModel h1 = new HistoricoModel();
                        h1.Tipo = "DocSign";
                        h1.Status = "On progress";
                        h1.Description = "";
                        h1.ModifiedTime = hist.ModifiedTime;
                        h1.ModifiedBy = "Dep. Leasedesk";
                        lstH.Add(h1);
                    }

                    if (hist.DocSignStatusSent == 2)
                    {
                        HistoricoModel h1 = new HistoricoModel();
                        h1.Tipo = "DocSign";
                        h1.Status = "Waiting for the customer";
                        h1.Description = "";
                        h1.ModifiedTime = hist.ModifiedTime;
                        h1.ModifiedBy = "Dep. Leasedesk";
                        lstH.Add(h1);
                    }

                }

            }

            lstH = lstH.OrderByDescending(x => x.ModifiedTime).ToList();
            return Ok(lstH);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ValidateShareRequest")]
        public HttpResponseMessage ValidateShareRequest(ShareOportunity p)
        {
            int? proposalID = p.Draft.details.ID;
            ActionResponse ac = new ActionResponse();

            using (var db = new BB_DB_DEVEntities2())
            {
                BB_Permissions newPermission = new BB_Permissions();

                newPermission.UserEmaill = p.user;
                if (p.initialDate == "-" && p.endDate == "-")
                {
                    newPermission.InitialDate = null;
                    newPermission.EndDate = null;
                }
                else
                {
                    newPermission.InitialDate = DateTime.Parse(p.initialDate);
                    newPermission.EndDate = DateTime.Parse(p.endDate);
                }
                if (p.isPermanent == "Sim" || p.isPermanent == "Sí")
                {
                    newPermission.IsPermanent = true;
                }
                else
                {
                    newPermission.IsPermanent = false;
                }
                newPermission.CreatedUser = p.CreatedBy;
                newPermission.CreatedTime = DateTime.Now;
                newPermission.ModifiedTime = DateTime.Now;
                newPermission.ProposalID = proposalID;
                newPermission.PermissionType = p.PermissionType;
                newPermission.ToDelete = false;

                db.BB_Permissions.Add(newPermission);
                db.SaveChanges();
            }
            ac.Message = "Permissão gravada com sucesso!";
            return Request.CreateResponse(HttpStatusCode.OK, ac);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("DeleteContractDoc")]
        public IHttpActionResult DeleteContractDoc(string quoteNumber, string fileName)
        {
            try
            {
                LD_DocumentProposal dp = new LD_DocumentProposal();

                using (var db = new BB_DB_DEV_LeaseDesk())
                {
                    dp = db.LD_DocumentProposal.Where(x => x.QuoteNumber == quoteNumber && x.FileName == fileName).FirstOrDefault();

                    if (dp != null)
                    {
                        if (File.Exists(dp.FileFullPath))
                        {
                            File.Delete(dp.FileFullPath);
                            Console.WriteLine("O ficheiro foi apagado com sucesso.");
                        }
                        db.LD_DocumentProposal.Remove(dp);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok("O ficheiro foi apagado com sucesso.");
        }

    }
    public class cEmail
    {
        public string email { get; set; }
    }

    public class HistoricoModel
    {
        public string Tipo { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }

        public string Description { get; set; }
    }

    public class DocumentoData
    {
        //public LD_DocumentClassification Type { get; set; }
        public int Type { get; set; }

        public string Comments { get; set; }

    }
    public class ClientEmailSend
    {
        public int? ProposalID { get; set; }
        public List<String> Contacts { get; set; }

        public int? ContractType { get; set; }

        public string Observations { get; set; }
    }

    public class ProposalContact
    {
        public int? ProposalID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
    }

    public class UserName1
    {
        public string username { get; set; }

    }
}
