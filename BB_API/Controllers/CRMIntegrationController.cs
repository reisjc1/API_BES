using Microsoft.Office.Interop.Excel;
using Microsoft.Win32.SafeHandles;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CRMIntegrationController : ApiController
    {
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();

        [AcceptVerbs("GET", "POST")]
        [ActionName("CRM_Quote_Integration")]
        public async Task<HttpResponseMessage> CRM_Quote_Integration(int? proposalID)
        {
            ActionResponse response = new ActionResponse();
            BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();

            List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
            List<BB_Proposal_OPSImplement> opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();

            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            string path = "";

            try
            {
                BB_Proposal p = new BB_Proposal();
                AspNetUsers c = new AspNetUsers();
                using (var db = new BB_DB_DEVEntities2())
                {
                    p = db.BB_Proposal.Where(x => x.ID == proposalID).First();

                }
                using (var db = new masterEntities())
                {
                    c = db.AspNetUsers.Where(x => x.Email == p.CreatedBy).FirstOrDefault();

                }

                string strUser = c.DisplayName;
                string strFolder = "CRM_Integration";
                string strCliente = c.DisplayName;
                path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                string paht2 = AppSettingsGet.CRM_Integration_Template_ClickPrice;
                string paht1 = AppSettingsGet.CRM_Integration_Template_Quote;
                using (var stream = File.Open(paht1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                    try
                    {
                        using (var outputFile = new FileStream(path + "\\Quote.xlsx", FileMode.Create))
                        {
                            stream.CopyTo(outputFile);
                        }
                    }
                    catch (IOException)
                    {
                        if (stream != null)
                            stream.Close();
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }

                Write_Quote_CRM_Integration(@path + "\\Quote.xlsx", p, quotes, opsImplement);

                // Create new instance of Excel
                Application excelApplication = new Application();
                Workbook excelWorkbook;

                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(@path + "\\Quote.xlsx");

                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;
                }

                try
                {
                    excelWorkbook.Application.ActiveWorkbook.SaveAs(@path + "\\Quote.xml", XlFileFormat.xlXMLSpreadsheet, Missing.Value,
                    Missing.Value, false, false, XlSaveAsAccessMode.xlNoChange,
                    XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);
                }
                catch (System.Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    // Close the workbook, quit the Excel, and clean up regardless of the results...
                    excelWorkbook.Close(0);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);

                    excelApplication.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                    excelApplication = null;
                    excelWorkbook = null;
                }

                //MUDAR para apro
                //proposal.StatusID = 7;
                db.Entry(proposal).State = proposal.ID == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            response.Content = new StreamContent(new FileStream(@path + "\\Quote.xml", FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = @path + "\\Quote.xml";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            response.ProposalIDReturn = proposalID ?? default(int);

            return response;
        }

        private void Write_Quote_CRM_Integration(string path, BB_Proposal proposal, List<BB_Proposal_Quote> quotes, List<BB_Proposal_OPSImplement> opsImplement)
        {
            try
            {
                FileInfo newFile = new FileInfo(path);
                ExcelPackage pck = new ExcelPackage(newFile);
                var ws = pck.Workbook.Worksheets["Produto da Proposta"];

                int idxCollumn = 2;
                foreach (var quote in quotes)
                {
                    bool IsValid = ValidateQuoteCRM(quote.CodeRef);
                    if (IsValid)
                    {
                        ws.Cells["D" + idxCollumn].Value = quote.CodeRef;
                        ws.Cells["E" + idxCollumn].Value = "Primary Unit";
                        ws.Cells["AB" + idxCollumn].Value = quote.Qty;
                        ws.Cells["Y" + idxCollumn].Value = quote.DiscountPercentage;
                        ws.Cells["BH" + idxCollumn].Value = proposal.CRM_QUOTE_ID;
                        ws.Cells["AE" + idxCollumn].Value = quote.UnitDiscountPrice;
                        ws.Cells["AH" + idxCollumn].Value = Math.Round(quote.DiscountPercentage.Value, 3);
                        ws.Cells["AI" + idxCollumn].Value = quote.TotalPVP - quote.TotalNetsale;
                        idxCollumn++;
                    }
                    else
                    {
                        BB_Coderef_Generic generic = db.BB_Coderef_Generic.Where(x => x.Family == quote.Family).FirstOrDefault();
                        if (generic != null)
                        {
                            ws.Cells["D" + idxCollumn].Value = generic.Coderef;
                            ws.Cells["E" + idxCollumn].Value = "Primary Unit";
                            ws.Cells["AB" + idxCollumn].Value = quote.Qty;
                            ws.Cells["Y" + idxCollumn].Value = quote.DiscountPercentage;
                            ws.Cells["BH" + idxCollumn].Value = proposal.CRM_QUOTE_ID;
                            ws.Cells["AE" + idxCollumn].Value = quote.UnitDiscountPrice;
                            ws.Cells["AH" + idxCollumn].Value = Math.Round(quote.DiscountPercentage.Value, 3);
                            ws.Cells["AI" + idxCollumn].Value = quote.TotalPVP - quote.TotalNetsale;
                            idxCollumn++;
                        }
                    }
                }
                foreach(BB_Proposal_OPSImplement opsi in opsImplement)
                {
                    double discountPercentage = 0;
                    if (opsi.PVP != null)
                    {
                        discountPercentage = (1 - opsi.UnitDiscountPrice.GetValueOrDefault() / opsi.PVP.GetValueOrDefault()) * 100;
                    }                    
                    ws.Cells["D" + idxCollumn].Value = opsi.CodeRef;
                    ws.Cells["E" + idxCollumn].Value = "Primary Unit";
                    ws.Cells["AB" + idxCollumn].Value = opsi.Quantity;
                    ws.Cells["Y" + idxCollumn].Value = discountPercentage;
                    ws.Cells["BH" + idxCollumn].Value = proposal.CRM_QUOTE_ID;
                    ws.Cells["AE" + idxCollumn].Value = opsi.UnitDiscountPrice;
                    ws.Cells["AH" + idxCollumn].Value = Math.Round(discountPercentage, 3);
                    ws.Cells["AI" + idxCollumn].Value = (opsi.PVP * opsi.Quantity) - (opsi.UnitDiscountPrice * opsi.Quantity);
                    idxCollumn++;
                }

                pck.Save();


                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");

            }


        }

        private bool ValidateQuoteCRM(string coderef)
        {
            BB_Data_Integration quote = db.BB_Data_Integration.Where(x => x.CodeRef == coderef).FirstOrDefault();
            if (quote != null)
            {
                return true;
            }
            return false;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ClickPrice_ID")]
        public async Task<HttpResponseMessage> ClickPrice_ID(int? proposalID)
        {
            ActionResponse response = new ActionResponse();
            BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();

            List<BB_Proposal_Quote> quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();

            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            string path = "";

            try
            {
                BB_Proposal p = new BB_Proposal();
                AspNetUsers c = new AspNetUsers();
                using (var db = new BB_DB_DEVEntities2())
                {
                    p = db.BB_Proposal.Where(x => x.ID == proposalID).First();

                }
                using (var db = new masterEntities())
                {
                    c = db.AspNetUsers.Where(x => x.Email == p.CreatedBy).FirstOrDefault();

                }

                string strUser = c.DisplayName;
                string strFolder = "CRM_Integration";
                string strCliente = c.DisplayName;
                path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                string paht2 = AppSettingsGet.CRM_Integration_Template_ClickPrice;
                string paht1 = AppSettingsGet.CRM_Integration_Template_Quote;
                using (var stream = File.Open(paht2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                    try
                    {
                        using (var outputFile = new FileStream(path + "\\PeriodicService.xlsx", FileMode.Create))
                        {
                            stream.CopyTo(outputFile);
                        }
                    }
                    catch (IOException)
                    {
                        if (stream != null)
                            stream.Close();
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }

                Write_Quote_CRM_Integration_ClickPrice(@path + "\\PeriodicService.xlsx", p, quotes);

                // Create new instance of Excel
                Application excelApplication = new Application();
                Workbook excelWorkbook;

                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(@path + "\\PeriodicService.xlsx");

                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;
                }

                try
                {
                    excelWorkbook.Application.ActiveWorkbook.SaveAs(@path + "\\PeriodicService.xml", XlFileFormat.xlXMLSpreadsheet, Missing.Value,
                    Missing.Value, false, false, XlSaveAsAccessMode.xlNoChange,
                    XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);
                }
                catch (System.Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    // Close the workbook, quit the Excel, and clean up regardless of the results...
                    excelWorkbook.Close(0);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);

                    excelApplication.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                    excelApplication = null;
                    excelWorkbook = null;
                }

                //MUDAR para apro
                //proposal.StatusID = 7;
                db.Entry(proposal).State = proposal.ID == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            response.Content = new StreamContent(new FileStream(@path + "\\PeriodicService.xml", FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = @path + "\\PeriodicService.xml";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

            response.ProposalIDReturn = proposalID ?? default(int);

            return response;
        }

        private void Write_Quote_CRM_Integration_ClickPrice(string path, BB_Proposal proposal, List<BB_Proposal_Quote> quotes)
        {
            try
            {
                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);

                var ws = pck.Workbook.Worksheets["Quote Periodical Service"];

                //ExcelWorksheet ws = xlPackage.Workbook.Worksheets["Quote Product"];

                List<BB_Proposal_PsConfig> psconfig = new List<BB_Proposal_PsConfig>();
                psconfig = db.BB_Proposal_PsConfig.Where(x => x.ProposalID == proposal.ID).ToList();

                int idxCollumn = 2;
                foreach (var ps in psconfig)
                {
                    if (ps.BWCost != null && ps.BWCost != 0)
                    {
                        string coderef = quotes.Where(x => x.ID == ps.ItemID).Select(x => x.CodeRef).FirstOrDefault();
                        string codRefclickPreto = db.BB_Equipamentos.Where(x => x.CodeRef == coderef).Select(x => x.CRM_BW).FirstOrDefault();
                        string name = db.BB_Equipamentos.Where(x => x.CodeRef == coderef).Select(x => x.Name).FirstOrDefault();

                        ws.Cells["D" + idxCollumn].Value = codRefclickPreto;
                        ws.Cells["E" + idxCollumn].Value = "Mensal"; //
                        ws.Cells["H" + idxCollumn].Value = "1"; //Quantidade
                        ws.Cells["W" + idxCollumn].Value = ps.BWCost; //Backpaid Unit Price 
                        ws.Cells["X" + idxCollumn].Value = ps.BWCost; //Extended Backpaid Unit Price
                        ws.Cells["AC" + idxCollumn].Value = "Maria Sousa"; //OWNER
                        ws.Cells["AD" + idxCollumn].Value = proposal.CRM_QUOTE_ID; //QUOTEID
                        ws.Cells["AE" + idxCollumn].Value = ""; //Parent Quote Product ID
                        ws.Cells["AF" + idxCollumn].Value = ""; //NAME
                        ws.Cells["AJ" + idxCollumn].Value = name; //Parent Quote Product Name

                        idxCollumn++;
                    }

                    if (ps.CCost != null && ps.CCost != 0)
                    {
                        string coderef = quotes.Where(x => x.ID == ps.ItemID).Select(x => x.CodeRef).FirstOrDefault();
                        string codRefclick = db.BB_Equipamentos.Where(x => x.CodeRef == coderef).Select(x => x.CRM_COLOR).FirstOrDefault();
                        string name = db.BB_Equipamentos.Where(x => x.CodeRef == coderef).Select(x => x.Name).FirstOrDefault();

                        ws.Cells["D" + idxCollumn].Value = codRefclick;
                        ws.Cells["E" + idxCollumn].Value = "Mensal"; //
                        ws.Cells["H" + idxCollumn].Value = "1"; //Quantidade
                        ws.Cells["W" + idxCollumn].Value = ps.CCost; //Backpaid Unit Price 
                        ws.Cells["X" + idxCollumn].Value = ps.CCost; //Extended Backpaid Unit Price
                        ws.Cells["AC" + idxCollumn].Value = "Maria Sousa"; //OWNER
                        ws.Cells["AD" + idxCollumn].Value = proposal.CRM_QUOTE_ID; //QUOTEID
                        ws.Cells["AE" + idxCollumn].Value = ""; //Parent Quote Product ID
                        ws.Cells["AF" + idxCollumn].Value = ""; //NAME
                        ws.Cells["AJ" + idxCollumn].Value = name; //Parent Quote Product Name

                        idxCollumn++;
                    }




                }

                pck.Save();


                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");

            }


        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("QuoteProposal")]
        public HttpResponseMessage QuoteProposal(ProposalRootObject p)
        {
            ProposalBLL pro = new ProposalBLL();
            ActionResponse ac = new ActionResponse();

            int? proposalID = p.Draft.details.ID;
            if (proposalID == null || proposalID == 0)
            {
                ac = pro.ProposalDraftSaveAs(p);
            }
            else
            {
                ac = pro.ProposalDraftSave(p);
            }



            BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == ac.ProposalObj.Draft.details.ID).FirstOrDefault();
            proposal.StatusID = 7;

            LD_Contrato ld = new LD_Contrato();
            using (var db1 = new BB_DB_DEV_LeaseDesk())
            {
                ld = db1.LD_Contrato.Where(x => x.ProposalID == proposalID).FirstOrDefault();

            }

            if(ld != null || proposal.StatusCRM1 == "Active-Approved")
            {
                proposal.StatusID = 5;
            }
            db.Entry(proposal).State = proposal.ID == 0 ? EntityState.Added : EntityState.Modified;

            try
            {
                db.SaveChanges();
                BB_Proposal_Status dbStatus = db.BB_Proposal_Status.Where(x => x.ID == proposal.StatusID).FirstOrDefault();
                if (dbStatus != null)
                {
                    ac.ProposalObj.Draft.details.Status = new ProposalStatus
                    {
                        IsEdit = dbStatus.BB_Edit,
                        Name = dbStatus.Description,
                        Phase = dbStatus.Phase
                    };
                }
            }
            catch (Exception ex)
            {
                throw new ActionFailResponse("Não Submeteu")
                {
                    StatusCode = 501
                };
            }
            return Request.CreateResponse(HttpStatusCode.OK, ac);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_Workflow_Status")]
        public HttpResponseMessage Get_Workflow_Status(string quote)
        {


            List<BB_CRM_Workflow> lst = new List<BB_CRM_Workflow>();
            lst = db.BB_CRM_Workflow.Where(x => x.subject.Contains(quote)).ToList();

            return Request.CreateResponse<List<BB_CRM_Workflow>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetCRMStatus")]
        public async Task<HttpResponseMessage> GetCRMStatus(int? id)
        {
            StatusActionResponse err = new StatusActionResponse();


            BB_Proposal proposal = db.BB_Proposal.Where(x => x.ID == id).FirstOrDefault();

            string quoteCRM = proposal.CRM_QUOTE_ID;

            BB_CRM_Quotes quoteStatus = db.BB_CRM_Quotes.Where(x => x.quotenumber == quoteCRM).FirstOrDefault();

            err.State = quoteStatus.statecodename;
            err.Status = quoteStatus.statuscodename;



            return Request.CreateResponse<StatusActionResponse>(HttpStatusCode.OK, err);
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
         int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        [AcceptVerbs("GET", "POST")]
        [ActionName("getProposalStatus")]
        public List<ProposalStatus1> getProposalStatus()
        {
            List<ProposalStatus1> lstProposal = new List<ProposalStatus1>();
            string domainName = "BS";

            //Console.Write("Enter the login of a user on {0} that you wish to impersonate: ", domainName);
            string userName = "BPT.CRM-Master";



            const int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token.   
            const int LOGON32_LOGON_INTERACTIVE = 2;

            // Call LogonUser to obtain a handle to an access token.   
            SafeAccessTokenHandle safeAccessTokenHandle;
            bool returnValue = LogonUser(userName, domainName, "664GfxcJ",
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeAccessTokenHandle);

            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
                Console.WriteLine("LogonUser failed with error code : {0}", ret);
                throw new System.ComponentModel.Win32Exception(ret);
            }

            Console.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
            // Check the identity.  
            Console.WriteLine("Before impersonation: " + WindowsIdentity.GetCurrent().Name);

            // Note: if you want to run as unimpersonated, pass  
            //       'SafeAccessTokenHandle.InvalidHandle' instead of variable 'safeAccessTokenHandle'  
            WindowsIdentity.RunImpersonated(
                safeAccessTokenHandle,
                // User action  
                () =>
                {
                    // Check the identity.  

                    string bdConnect = @AppSettingsGet.BasedadosConnect;
                    using (SqlConnection conn = new SqlConnection(bdConnect))
                    {
                        conn.Open();

                        // 1.  create a command object identifying the stored procedure
                        SqlCommand cmd = new SqlCommand("sp_GetCRMQuoteStatus", conn);

                        // 2. set the command object so it knows to execute a stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 3. add parameter to command, which will be passed to the stored procedure
                        //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                        // execute the command
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            // iterate through results, printing each to console
                            while (rdr.Read())
                            {
                                ProposalStatus1 p = new ProposalStatus1();
                                p.revisionnumber = rdr["revisionnumber"].ToString();
                                p.statecodename = rdr["statecodename"].ToString();
                                p.statuscodename = rdr["statuscodename"].ToString();
                                lstProposal.Add(p);
                            }
                        }
                    }
                }
                );

            return lstProposal;
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("getContactosByAccountNumber")]
        public async Task<HttpResponseMessage> getContactosByAccountNumber(string accountnumber)
        {
            ContactosResponse response = new ContactosResponse();
            List<BB_Contactos> lstContactos = db.BB_Contactos.Where(x => x.ClientAccountNumberID == accountnumber).ToList();
            response.Contactos = lstContactos;

            return Request.CreateResponse<HttpResponseMessage>(HttpStatusCode.OK, response);
        }



    }
    public class ProposalStatus1
    {
        public string revisionnumber { get; set; }
        public string statecodename { get; set; }
        public string statuscodename { get; set; }
    }


}
