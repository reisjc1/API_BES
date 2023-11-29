using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.Models;
using WebApplication1.Models.MIF_Renovations;
using WebApplication1.Models.SLAs;

namespace WebApplication1.Controllers
{
    public class MIFRenovationsController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("GetMIFContracts")]
        public IHttpActionResult GetMIFContracts(string email)
        {
            List<BB_MIF_Renovations> lst_Registos = new List<BB_MIF_Renovations>();

            //email = "helder.fonseca@konicaminolta.pt";

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_Registos = db.BB_MIF_Renovations
                        .Where(x => x.ExpirationDate >= DateTime.Today 
                            && x.ExpirationDate <= DbFunctions.AddMonths(DateTime.Today,6) 
                            && x.SalespersonEmail == email)
                        .OrderBy(x => x.ExpirationDate)
                        .ToList();
                }

                return Ok(lst_Registos);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetMIFContracts_DRV")]
        public IHttpActionResult GetMIFContracts_DRV(string email)
        {
            List<BB_MIF_Renovations> lst_Registos_DRV = new List<BB_MIF_Renovations>();

            //email = "helder.fonseca@konicaminolta.pt";

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {

                    lst_Registos_DRV = db.BB_MIF_Renovations
                        .Where(x => x.ExpirationDate >= DateTime.Today
                            && x.ExpirationDate <= DbFunctions.AddMonths(DateTime.Today, 6)
                            && x.ManagerEmail == email
                            && x.State == 1)
                        .OrderBy(x => x.ExpirationDate)
                        .ToList();
                }

                return Ok(lst_Registos_DRV);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetMIFContracts_LeaseDesk")]
        public IHttpActionResult GetMIFContracts_LeaseDesk()
        {
            List<BB_MIF_Renovations> lst_Registos_DRV = new List<BB_MIF_Renovations>();

            //email = "helder.fonseca@konicaminolta.pt";

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {

                    lst_Registos_DRV = db.BB_MIF_Renovations
                        .Where(x => x.ExpirationDate >= DateTime.Today
                            && x.ExpirationDate <= DbFunctions.AddMonths(DateTime.Today, 6)
                            && x.State == 2)
                        .OrderBy(x => x.ExpirationDate)
                        .ToList();
                }

                return Ok(lst_Registos_DRV);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################


        [AcceptVerbs("GET", "POST")]
        [ActionName("MIFContractInfo")]
        public IHttpActionResult MIFContractInfo(string contractNumber)
        {
            try
            {
                ContratoMIF contratoMIF = new ContratoMIF();
                BB_MIF_Renovations contrato = new BB_MIF_Renovations();
                List<BB_MIF_Renovations_Comments> renovationComments = new List<BB_MIF_Renovations_Comments>();
                List<string> statusList = new List<string>();

                BB_MIF_Renovations resultFINAL = new BB_MIF_Renovations();

                using (var db = new BB_DB_DEVEntities2())
                {
                    contrato = db.BB_MIF_Renovations.Where(x => x.ContractNr == contractNumber).FirstOrDefault();
                    renovationComments = db.BB_MIF_Renovations_Comments.Where(x => x.ContractNr == contractNumber).ToList();
                    statusList = db.BB_RD_MIF_Renovation_Programs.Select(x => x.Program_Name).ToList();
                }
                contratoMIF.Contrato = contrato;
                contratoMIF.ContratoComments = renovationComments;
                contratoMIF.StatusList = statusList;

                return Ok(contratoMIF);            
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################

        [AcceptVerbs("GET", "POST")]
        [ActionName("MIFContractSubmitEdit")]
        public IHttpActionResult MIFContractSubmitEdit(ContractDRVApproval dRVApproval)
        {
            //List<BB_MIF_Renovations_Comments> renovationComments = new List<BB_MIF_Renovations_Comments>();

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_MIF_Renovations dbContrat = db.BB_MIF_Renovations.Where(x => x.ContractNr == dRVApproval.Contract.ContractNr).FirstOrDefault();
                    //renovationComments = db.BB_MIF_Renovations_Comments.Where(x => x.ContractNr == contract.ContractNr).ToList();

                    if(dRVApproval.Contract.SalesServiceExceptions == null || dRVApproval.Contract.SalesServiceExceptions == "")
                    {
                        // é o GESTOR que aprovou ou reprovou 
                        dbContrat.State = 1;
                        dbContrat.ProposalProgramCustomer = dRVApproval.Contract.ProposalProgramCustomer;
                        dbContrat.Increase = dRVApproval.Contract.Increase;
                    }
                    else
                    {
                        //é O DRV que submeteu
                        if (dRVApproval.SelectedButton == "Aprovar")
                        {
                            dbContrat.State = 2;
                            dbContrat.SalesServiceExceptions = dRVApproval.Contract.SalesServiceExceptions;
                        }
                        else if (dRVApproval.SelectedButton == "Reprovar")
                        {
                            dbContrat.State = 3;
                            dbContrat.SalesServiceExceptions = dRVApproval.Contract.SalesServiceExceptions;
                        }
                    }

                    db.Entry(dbContrat).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // ##################################################################################
        // ##################################################################################


        [AcceptVerbs("GET", "POST")]
        [ActionName("AddMIFContractComment")]
        public List<BB_MIF_Renovations_Comments> AddMIFContractComment(string contractComment, string contractNumber, string userLogged)
        {
            ContratoMIF result = new ContratoMIF();
            try {

                List<BB_MIF_Renovations_Comments> lstComentarios = new List<BB_MIF_Renovations_Comments>();
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_MIF_Renovations_Comments.Add(new BB_MIF_Renovations_Comments()
                    {
                        ContractNr = contractNumber,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userLogged,
                        Comment = contractComment
                    });

                    db.SaveChanges();

                    lstComentarios = db.BB_MIF_Renovations_Comments.Where(q => q.ContractNr == contractNumber).ToList();
                }

                return lstComentarios;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        ///
        ///////////////////////////////  TESTES  /////////////////////////////////////
        ///

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetChildrenFromContract")]
        public IHttpActionResult GetChildrenFromContract(string parentNr)
        {
            List<BB_MIF_Renovations> lst_Registos = new List<BB_MIF_Renovations>();

            // TO DELETE ********************
            parentNr = "14CTS00410";     //**
            //*******************************

            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    lst_Registos = db.BB_MIF_Renovations
                        .Where(x => x.ExpirationDate >= DateTime.Today
                            && x.ExpirationDate <= DbFunctions.AddMonths(DateTime.Today, 6)
                            && x.ParentContractNr == parentNr)
                        .OrderBy(x => x.ExpirationDate)
                        .ToList();
                }

                return Ok(lst_Registos);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
