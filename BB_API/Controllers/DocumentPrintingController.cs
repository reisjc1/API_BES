

using AutoMapper;
using Microsoft.Office.Interop.Word;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.DAL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DocumentPrintingController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("PrintingProposal")]
        public HttpResponseMessage PrintingProposal(int ID)
        {
            //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
            string erro = "";
            //int proposalID = ID;
            int proposalID = ID;
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
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
                string strFolder = "Proposal";
                string strCliente = c.DisplayName;
                string path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                File.Copy(@AppSettingsGet.DocumentPrintingTemplate, path + "\\Proposal.xlsx", true);
                //File.SetAttributes(path + "\\Proposal.xlsx", FileAttributes.Normal);


                //QUOTE WRITE TO EXCEL
                string resultQuote = WriteToExcelQuote(@path + "\\Proposal.xlsx", proposalID);

                //Cliente WRITE TO EXCEL
                //bool resultClient = WriteToExcelCliente(@path + "\\Proposal.xlsx", proposalID);

                //Thread.Sleep(3000);




                ExportWorkbookToPdf(@path + "\\Proposal.xlsx", @path + "\\Proposal.pdf", proposalID);


                string filePath = @path + "\\Proposal.pdf";


                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: .");
                    throw new HttpResponseException(response);
                }

                byte[] bytes = File.ReadAllBytes(filePath);

                //Set the Response Content.

                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Proposal.pdf";

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Proposal.pdf"));
                return response;

            }
            catch (Exception ex)
            {
                erro = ex.Message.ToString();
            }
            Console.WriteLine(erro);
            return response;



        }

        private string WriteToExcelQuote(string path, int proposalID)
        {
            //GET QUOTE CONFIGURACAO
            List<BB_Proposal_Quote> quote = new List<BB_Proposal_Quote>();
            List<BB_Proposal_OPSImplement> opsImplement = new List<BB_Proposal_OPSImplement>();
            using (var db = new BB_DB_DEVEntities2())
            {
                quote = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).ToList();
            }

            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
            Microsoft.Office.Interop.Excel._Worksheet excelSheet;
            excelApplication = new Microsoft.Office.Interop.Excel.Application();
            string erro = "";

            bool exportSuccessful = true;
            try
            {
                // Create new instance of Excel


                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(path);

                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;

                    return "false";
                }

                excelSheet = excelWorkbook.Sheets["Sheet1"];

                try
                {
                    BB_Clientes cliente = new BB_Clientes();
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        cliente = (from a in db.BB_Proposal_Client join b in db.BB_Clientes on a.ClientID equals b.accountnumber where a.ProposalID == proposalID select b).FirstOrDefault();
                    }

                    excelSheet.Cells[4, 4] = cliente.Name;
                    excelSheet.Cells[5, 4] = cliente.PostalCode;
                    excelSheet.Cells[5, 10] = cliente.PostalCode;
                    excelSheet.Cells[6, 4] = cliente.NIF;
                    excelSheet.Cells[6, 10] = cliente.emailaddress1;

                    int idxRow = 19;
                    // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                    foreach (var item in quote)
                    {
                        excelSheet.Cells[idxRow, 1] = item.Qty;
                        excelSheet.Cells[idxRow, 2] = item.CodeRef;
                        excelSheet.Cells[idxRow, 4] = item.Description;
                        idxRow++;
                    }
                    foreach (var item in opsImplement)
                    {
                        excelSheet.Cells[idxRow, 1] = item.Quantity;
                        excelSheet.Cells[idxRow, 2] = item.CodeRef;
                        excelSheet.Cells[idxRow, 4] = item.Name + " - " + item.Description;
                        idxRow++;
                    }

                    excelWorkbook.Save();
                    excelWorkbook.Close(true);
                    //excelWorkbook.SaveAs(outputPath);
                }
                catch (System.Exception ex)
                {
                    File.Delete(path);
                    // Mark the export as failed for the return value...
                    exportSuccessful = false;

                    // Do something with any exceptions here, if you wish...
                    // MessageBox.Show...        
                }
                finally
                {
                    // Close the workbook, quit the Excel, and clean up regardless of the results...
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelSheet);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelSheet);
                    excelApplication.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);



                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                }



            }
            catch (Exception ex)
            {

                erro = ex.InnerException.ToString();
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                Console.WriteLine(erro);

            }
            Console.WriteLine(erro);
            return erro;
        }

        private string ProFormaWriteToExcelQuote(string path, int? proposalID, double? PropostaValorTotalSemIVA, string accountnumber)
        {
            //GET QUOTE CONFIGURACAO
            List<BB_Proposal_Aprovacao_Quote> quotes = new List<BB_Proposal_Aprovacao_Quote>();
            List<BB_Proposal_Overvaluation> sobrevalorizoes = new List<BB_Proposal_Overvaluation>();
            List<BB_Proposal_Upturn> retomaLst = new List<BB_Proposal_Upturn>();
            double? sobrevalorizacao = 0;
            double? retoma = 0;
            double? OPSHWvalorTotal = 0;
            double? LeiDaCopiaPriada = 0;
            PropostaValorTotalSemIVA = 0;
            BB_Clientes cliente = new BB_Clientes();
            using (var db = new BB_DB_DEVEntities2())
            {
                quotes = db.BB_Proposal_Aprovacao_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                sobrevalorizoes = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == proposalID).ToList();
                retomaLst = db.BB_Proposal_Upturn.Where(x => x.ProposalID == proposalID).ToList();
                foreach (var i in sobrevalorizoes)
                {
                    sobrevalorizacao += i.Total;
                }
                foreach (var i in retomaLst)
                {
                    retoma += i.Total;
                }



                OPSHWvalorTotal = db.BB_Proposal_Aprovacao_Quote.Where(x => x.Proposal_ID == proposalID && x.Family == "OPSHW").GroupBy(x => x.Family).Select(x => x.Sum(i => i.TotalNetsale)).FirstOrDefault();


                List<BB_Proposal_Aprovacao_Quote> lstQuotes = db.BB_Proposal_Aprovacao_Quote.Where(x => x.Proposal_ID == proposalID && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                foreach (var quote in lstQuotes)
                {

                    double? TCP = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).Select(x => x.TCP).FirstOrDefault();
                    //var contador = db.BB_Proposal_Counters.Where(x => x.OSID == quote.ID).Count();

                    if (TCP is null)
                        TCP = 0;

                    if (quote.IsUsed.GetValueOrDefault() == false)
                        LeiDaCopiaPriada = (TCP * quote.Qty) + LeiDaCopiaPriada;

                }

                if (sobrevalorizacao == null)
                    sobrevalorizacao = 0;
                if (retoma == null)
                    retoma = 0;

                if (LeiDaCopiaPriada == null)
                    LeiDaCopiaPriada = 0;

                sobrevalorizacao = sobrevalorizacao - retoma;

            }

            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
            Microsoft.Office.Interop.Excel._Worksheet excelSheet;
            Microsoft.Office.Interop.Excel._Worksheet excelSheet1;
            excelApplication = new Microsoft.Office.Interop.Excel.Application();
            string erro = "";

            bool exportSuccessful = true;
            try
            {
                // Create new instance of Excel


                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(path);

                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;

                    return "false";
                }

                excelSheet = excelWorkbook.Sheets["Sheet1"];
                excelSheet1 = excelWorkbook.Sheets["Sheet2"];

                try
                {

                    using (var db = new BB_DB_DEVEntities2())
                    {
                        cliente = (from a in db.BB_Proposal_Client join b in db.BB_Clientes on a.ClientID equals b.accountnumber where a.ProposalID == proposalID select b).FirstOrDefault();
                    }

                    int idxRow = 15;
                    int idxRow1 = 15;
                    int idx = 0;

                    //QUPTES
                    foreach (var quote in quotes)
                    {
                        //quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                        //quote.UnitDiscountPrice = 
                        if (idx < 31)
                        {
                            PropostaValorTotalSemIVA += quote.TotalNetsale;
                            excelSheet.Cells[idxRow, 1] = quote.Qty;
                            excelSheet.Cells[idxRow, 2] = quote.CodeRef + " - " + quote.Description;

                            //double? Sumvalor = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.UnitDiscountPrice / OPSHWvalorTotal) * sobrevalorizacao) + quote.PVP).Value, 2) : quote.UnitDiscountPrice;
                            excelSheet.Cells[idxRow, 6] = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                            double? total = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                            excelSheet.Cells[idxRow, 5] = Math.Round((total.Value / quote.Qty.Value), 2);
                            idxRow++;
                        }

                        if (idx >= 31)
                        {
                            PropostaValorTotalSemIVA += quote.TotalNetsale;
                            excelSheet1.Cells[idxRow1, 1] = quote.Qty;
                            excelSheet1.Cells[idxRow1, 2] = quote.CodeRef + " - " + quote.Description;

                            //double? Sumvalor = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.UnitDiscountPrice / OPSHWvalorTotal) * sobrevalorizacao) + quote.PVP).Value, 2) : quote.UnitDiscountPrice;
                            excelSheet1.Cells[idxRow1, 6] = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                            double? total = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                            excelSheet1.Cells[idxRow1, 5] = Math.Round((total.Value / quote.Qty.Value), 2);
                            idxRow1++;
                        }
                        idx++;
                    }


                    //Sumario
                    //Total Iliquido

                    excelSheet.Cells[47, 6] = PropostaValorTotalSemIVA + sobrevalorizacao;

                    //Lei da copia privada
                    excelSheet.Cells[48, 6] = LeiDaCopiaPriada;

                    //SUb total
                    excelSheet.Cells[49, 6] = (PropostaValorTotalSemIVA + sobrevalorizacao + LeiDaCopiaPriada);

                    //IVA
                    double TotaValorSemIva = (PropostaValorTotalSemIVA + sobrevalorizacao + LeiDaCopiaPriada).Value;
                    double? IVAValor = Math.Round((TotaValorSemIva * 0.23), 2);
                    excelSheet.Cells[50, 6] = IVAValor;

                    //TotalDocumento
                    excelSheet.Cells[51, 6] = TotaValorSemIva + IVAValor;


                    //Cliente
                    //Nif
                    excelSheet.Cells[13, 3] = cliente.NIF;

                    //Name cliente
                    excelSheet.Cells[7, 3] = cliente.Name;

                    //morada
                    excelSheet.Cells[8, 3] = cliente.address1_line1;

                    //cod_postal
                    excelSheet.Cells[9, 3] = cliente.PostalCode;




                    //Sumario
                    //Total Iliquido

                    excelSheet1.Cells[47, 6] = PropostaValorTotalSemIVA + sobrevalorizacao;

                    //Lei da copia privada
                    excelSheet1.Cells[48, 6] = LeiDaCopiaPriada;

                    //SUb total
                    excelSheet1.Cells[49, 6] = PropostaValorTotalSemIVA + sobrevalorizacao + LeiDaCopiaPriada;

                    //IVA

                    excelSheet1.Cells[50, 6] = IVAValor;

                    //TotalDocumento
                    excelSheet1.Cells[51, 6] = TotaValorSemIva + IVAValor;


                    //Cliente
                    //Nif
                    excelSheet1.Cells[13, 3] = cliente.NIF;

                    //Name cliente
                    excelSheet1.Cells[7, 3] = cliente.Name;

                    //morada
                    excelSheet1.Cells[8, 3] = cliente.address1_line1;

                    //cod_postal
                    excelSheet1.Cells[9, 3] = cliente.PostalCode;


                    if (quotes.Count() < 31)
                        excelSheet1.Delete();

                    excelWorkbook.Save();
                    excelWorkbook.Close(true);
                    //excelWorkbook.SaveAs(outputPath);
                }
                catch (System.Exception ex)
                {
                    File.Delete(path);
                    // Mark the export as failed for the return value...
                    exportSuccessful = false;

                    // Do something with any exceptions here, if you wish...
                    // MessageBox.Show...        
                }
                finally
                {
                    // Close the workbook, quit the Excel, and clean up regardless of the results...
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelSheet);

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelSheet);
                    excelApplication.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);



                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                }



            }
            catch (Exception ex)
            {

                erro = ex.InnerException.ToString();
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                Console.WriteLine(erro);

            }
            Console.WriteLine(erro);
            return erro;
        }

        private bool WriteToExcelCliente(string path, int proposalID)
        {
            //GET CLiente CONFIGURACAO
            BB_Clientes cliente = new BB_Clientes();
            using (var db = new BB_DB_DEVEntities2())
            {
                cliente = (from a in db.BB_Proposal_Client join b in db.BB_Clientes on a.ClientID equals b.accountnumber where a.ProposalID == proposalID select b).FirstOrDefault();
            }

            if (cliente == null)
                return false;

            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
            Microsoft.Office.Interop.Excel._Worksheet excelSheet;

            bool exportSuccessful = true;
            try
            {
                // Create new instance of Excel
                excelApplication = new Microsoft.Office.Interop.Excel.Application();

                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(path);

                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;

                    return false;
                }

                excelSheet = excelWorkbook.Sheets["Ordem de Aquisicao"];

                try
                {

                    excelSheet.Cells[4, 4] = cliente.Name;
                    excelSheet.Cells[5, 4] = cliente.PostalCode;
                    excelSheet.Cells[5, 10] = cliente.PostalCode;
                    excelSheet.Cells[6, 4] = cliente.NIF;
                    excelSheet.Cells[6, 10] = cliente.emailaddress1;

                    excelWorkbook.Save();
                    //excelWorkbook.SaveAs(outputPath);
                }
                catch (System.Exception ex)
                {
                    // Mark the export as failed for the return value...
                    exportSuccessful = false;

                    // Do something with any exceptions here, if you wish...
                    // MessageBox.Show...        
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



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return true;
        }
        public bool ExportWorkbookToPdfProforma(string workbookPath, string outputPath, int? proposalID)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            var exportSuccessful = true;
            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet Worksheet;

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                // Create new instance of Excel
                excelApplication = new Microsoft.Office.Interop.Excel.Application();

                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(workbookPath);
                //Worksheet = excelWorkbook.Worksheets;





                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;

                    return false;
                }


                try
                {

                    // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                    excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
                    //excelWorkbook.SaveAs(outputPath);
                }
                catch (System.Exception ex)
                {
                    // Mark the export as failed for the return value...
                    exportSuccessful = false;

                    // Do something with any exceptions here, if you wish...
                    // MessageBox.Show...        
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

                // You can use the following method to automatically open the PDF after export if you wish
                // Make sure that the file actually exists first...
                if (System.IO.File.Exists(outputPath))
                {
                    System.Diagnostics.Process.Start(outputPath);
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return exportSuccessful;
        }

        public bool ExportWorkbookToPdf(string workbookPath, string outputPath, int? proposalID)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            var exportSuccessful = true;
            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet Worksheet;

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                // Create new instance of Excel
                excelApplication = new Microsoft.Office.Interop.Excel.Application();

                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(workbookPath);
                //Worksheet = excelWorkbook.Worksheets;

                using (var db = new BB_DB_DEVEntities2())
                {
                    int count = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).Count();
                    count += db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == proposalID).Count();
                    count += db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).Count();
                    count += db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).Count();
                    //var i = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                    //if (i == null || i.modeId == 0)
                    //{
                    //    excelWorkbook.Worksheets[7].Delete();
                    //}

                    if (count > 15 && count <= 30)
                    {
                        excelWorkbook.Worksheets[4].Delete();
                    }
                    if (count <= 15)
                    {
                        excelWorkbook.Worksheets[5].Delete();
                        excelWorkbook.Worksheets[4].Delete();

                    }


                }



                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;

                    return false;
                }


                try
                {

                    // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                    excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
                    //excelWorkbook.SaveAs(outputPath);
                }
                catch (System.Exception ex)
                {
                    // Mark the export as failed for the return value...
                    exportSuccessful = false;

                    // Do something with any exceptions here, if you wish...
                    // MessageBox.Show...        
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

                // You can use the following method to automatically open the PDF after export if you wish
                // Make sure that the file actually exists first...
                if (System.IO.File.Exists(outputPath))
                {
                    System.Diagnostics.Process.Start(outputPath);
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return exportSuccessful;
        }

        public bool ExportWorkbookToPdfContrato(string workbookPath, string outputPath, int? proposalID)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            var exportSuccessful = true;
            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet Worksheet;

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                // Create new instance of Excel
                excelApplication = new Microsoft.Office.Interop.Excel.Application();

                // Make the process invisible to the user
                excelApplication.ScreenUpdating = false;

                // Make the process silent
                excelApplication.DisplayAlerts = false;

                // Open the workbook that you wish to export to PDF
                excelWorkbook = excelApplication.Workbooks.Open(workbookPath);
                //Worksheet = excelWorkbook.Worksheets;

                //using (var db = new BB_DB_DEVEntities2())
                //{
                //    int count = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).Count();
                //    count += db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).Count();


                //    if (count > 21 && count <= 57)
                //    {
                //        excelWorkbook.Worksheets[3].Delete();
                //    }
                //    if (count <= 21)
                //    {
                //        excelWorkbook.Worksheets[3].Delete();
                //        excelWorkbook.Worksheets[2].Delete();

                //    }

                //}



                // If the workbook failed to open, stop, clean up, and bail out
                if (excelWorkbook == null)
                {
                    excelApplication.Quit();

                    excelApplication = null;
                    excelWorkbook = null;

                    return false;
                }


                try
                {

                    // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                    excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
                    //excelWorkbook.SaveAs(outputPath);
                }
                catch (System.Exception ex)
                {
                    // Mark the export as failed for the return value...
                    exportSuccessful = false;

                    // Do something with any exceptions here, if you wish...
                    // MessageBox.Show...        
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

                // You can use the following method to automatically open the PDF after export if you wish
                // Make sure that the file actually exists first...
                if (System.IO.File.Exists(outputPath))
                {
                    System.Diagnostics.Process.Start(outputPath);
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return exportSuccessful;
        }
        [AcceptVerbs("GET", "POST")]
        [ActionName("Get_TemplateExcel")]
        public HttpResponseMessage Get_TemplateExcel()
        {

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                string filePath = @"C:\BEU_Excel\ImportExcelTemplate.xlsx";
                byte[] bytes = File.ReadAllBytes(filePath);

                //Set the Response Content.

                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "ImportExcelTemplate.xlsx";

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("ImportExcelTemplate.xlsx"));
                return response;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return response;
        }
        public void WriteCliente(string path, string acocuntNumber, BB_Proposal p)
        {
            try
            {
                BB_Clientes cliente = new BB_Clientes();
                using (var db = new BB_DB_DEVEntities2())
                {
                    cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
                }

                AspNetUsers u = new AspNetUsers();
                using (var db = new masterEntities())
                {
                    u = db.AspNetUsers.Where(x => x.Email == p.CreatedBy).FirstOrDefault();
                }

                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                var ws = pck.Workbook.Worksheets["CLIENTE"];
                //ws.View.ShowGridLines = false;

                ////ws.Column(4).OutlineLevel = 1;
                ////ws.Column(4).Collapsed = true;
                ////ws.Column(5).OutlineLevel = 1;
                ////ws.Column(5).Collapsed = true;
                ////ws.OutLineSummaryRight = true;

                ////Headers
                ws.Cells["E9"].Value = cliente.Name != null ? cliente.Name : "";
                ws.Cells["E10"].Value = "";
                ws.Cells["E11"].Value = cliente.address1_line1 != null ? cliente.address1_line1 : "";
                ws.Cells["E12"].Value = cliente.PostalCode != null ? cliente.PostalCode : "";
                ws.Cells["E13"].Value = cliente.City != null ? cliente.City : "";


                //KOnica Representante
                ws.Cells["A53"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";


                ws.Cells["E18"].Value = p.CRM_QUOTE_ID;
                ws.Cells["H18"].Value = String.Format("{0:dd/MM/yyyy}", DateTime.Now);

                ws.Cells["A44"].Value = u.DisplayName1;
                ws.Cells["A45"].Value = u.Function;

                pck.Save();


                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");

            }
        }
        public void WritePropostaOutrasCondicoes(string path, int? ProposalID, BB_Proposal p, GerarProposta o)
        {
            try
            {



                BB_Proposal_Financing financiamento = new BB_Proposal_Financing();

                BB_FinancingType FinancingType = new BB_FinancingType();
                BB_FinancingPaymentMethod aymentMethod = new BB_FinancingPaymentMethod();
                BB_FinancingContractType contractType = new BB_FinancingContractType();
                using (var db = new BB_DB_DEVEntities2())
                {
                    financiamento = db.BB_Proposal_Financing.Where(x => x.ProposalID == ProposalID).FirstOrDefault();

                    if (financiamento != null)
                    {
                        FinancingType = db.BB_FinancingType.Where(x => x.Code == financiamento.FinancingTypeCode).FirstOrDefault();
                        aymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == financiamento.PaymentMethodId).FirstOrDefault();
                        contractType = db.BB_FinancingContractType.Where(x => x.ID == financiamento.ContractTypeId).FirstOrDefault();
                    }
                }


                string servicosIncluidos = financiamento.IncludeServices.Value == true ? " com serviços incluídos" : "";


                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                var ws = pck.Workbook.Worksheets["OUTRAS_CONDICOES"];


                //Prazo De entrega
                ws.Cells["A14"].Value = "PRAZO DE ENTREGA";
                ws.Cells["A16"].Value = o.Prazo_Entega;

                ws.Cells["A19"].Value = "CONDIÇÕES PAGAMENTO - Produto/Software/Serviços Profissionais";
                ws.Cells["A21"].Value = FinancingType.Type + servicosIncluidos + " - Pagamento a " + financiamento.PaymentAfter + " após a data da factura";

                ws.Cells["A40"].Value = "Formação";
                ws.Cells["A42"].Value = "A formação de operadores a indicar por V.Exas., será realizada conforme proposto, nas vossas ou nossas instalações, mediante marcação e de acordo com a disponibilidade de datas da Konica Minolta Portugal.";


                List<string> lstObs = new List<string>();
                lstObs = o.ObservacoesOutrasCondiçoes.Split('\n').ToList();

                int row = 60;
                foreach (var item in lstObs)
                {
                    ws.Cells["A" + row.ToString()].Value = item.ToString();
                    row++;
                    if (row == 66)
                        break;
                }


                //KOnica Representante
                ws.Cells["A70"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws.Cells["A71"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws.Cells["A72"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                pck.Save();


                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");

            }
        }
        public void WritePropostaServicos(string path, int? ProposalID, BB_Proposal p)
        {
            try
            {

                List<BB_Proposal_Quote> quotes = new List<BB_Proposal_Quote>();
                BB_Proposal_Overvaluation sobrevalorizacao = new BB_Proposal_Overvaluation();
                List<BB_Equipamentos> lstEquipamentos = new List<BB_Equipamentos>();
                //BB_Proposal_PrintingServices printingServices = new BB_Proposal_PrintingServices();

                //List<BB_Proposal_PsConfig> psconfigs = new List<BB_Proposal_PsConfig>();
                List<BB_Proposal_Counters> lstContadores = new List<BB_Proposal_Counters>();
                //BB_Proposal_Vva vva = new BB_Proposal_Vva();

                BB_Proposal_Financing f = new BB_Proposal_Financing();
                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                var ws = pck.Workbook.Worksheets["CONDICOES_SERVICO"];
                BB_PrintingServices active = null;
                BB_VVA vva = null;
                BB_PrintingServices_NoVolume nv = null;

                Nullable<int> BWVolume = 0;
                Nullable<int> CVolume = 0;
                using (var db = new BB_DB_DEVEntities2())
                {
                    quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == ProposalID).ToList();
                    //printingServices = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                    //psconfigs = db.BB_Proposal_PsConfig.Where(x => x.ProposalID == ProposalID).ToList();

                    lstContadores = db.BB_Proposal_Counters.Where(x => x.ProposalID == ProposalID).ToList();
                    lstEquipamentos = db.BB_Equipamentos.ToList();
                    f = db.BB_Proposal_Financing.Where(x => x.ProposalID == ProposalID).FirstOrDefault();

                    BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2
                       .Include(x => x.BB_PrintingServices.Select(y => y.BB_VVA))
                       .Include(x => x.BB_PrintingServices.Select(y => y.BB_PrintingServices_NoVolume))
                       .Where(x => x.ProposalID == ProposalID)
                       .FirstOrDefault();

                    if (ps2 != null && ps2.BB_PrintingServices.Count > 0)
                    {

                        active = ps2.BB_PrintingServices.ToList()[(int)ps2.ActivePrintingService - 1];
                        if (active != null)
                        {
                            BWVolume = active.BWVolume;
                            CVolume = active.CVolume;
                            if (active.BB_VVA != null)
                            {
                                vva = active.BB_VVA;
                            }
                            if (active.BB_PrintingServices_NoVolume != null)
                            {
                                nv = active.BB_PrintingServices_NoVolume;
                            }
                        }
                    }
                }


                string strExcende = " ";
                int? IsMachine = 0;

                if (active == null)
                {
                    pck.Workbook.Worksheets.Delete(ws);
                    pck.Save();


                    pck.Stream.Close();
                    return;
                }
                else
                {

                    int billingFrequency = 0;

                    if (vva != null)
                    {
                        ws.Cells["F27"].Value = "Páginas";
                        ws.Cells["F28"].Value = "Incluidas";
                        ws.Cells["F29"].Value = "Preto";

                        ws.Cells["G27"].Value = "Páginas";
                        ws.Cells["G28"].Value = "Incluidas";
                        ws.Cells["G29"].Value = "Cores";

                        //ws.Cells["E40"].Value = vva.MonthlyFee;
                        ws.Cells["F40"].Value = BWVolume;
                        ws.Cells["G40"].Value = CVolume;

                        ws.Cells["H25"].Value = "Páginas Excedentes";
                        strExcende = " excendentes ";
                        int c = 30;
                        //int? BWVolume = 0;
                        //int? CVolume = 0;
                        List<BB_Proposal_Quote> q = quotes.Where(x => x.TCP != null).ToList();
                        foreach (var item in q)
                        {

                            BB_Equipamentos equi = lstEquipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                            if (equi != null)
                            {
                                List<BB_Proposal_Counters> counter = lstContadores.Where(x => x.OSID == item.ID).ToList();
                                if (counter.Count > 0)
                                {
                                    //ws.Cells["C10"].Value = "Contadores Iniciais";
                                    //ws.Cells["C11"].Value = "Preto";
                                    //ws.Cells["D11"].Value = "Cor";
                                    foreach (var i in counter)
                                    {
                                        ws.Cells["A" + c].Value = equi.Name + "-" + i.serialNumber;
                                        ws.Cells["H" + c].Value = vva.BWExcessPVP != 0 ? vva.BWExcessPVP.ToString() : "";
                                        ws.Cells["I" + c].Value = vva.CExcessPVP != 0 ? vva.CExcessPVP.ToString() : "";
                                        c++;
                                    }

                                }
                                else
                                {
                                    ws.Cells["A" + c].Value = equi.Name;
                                    ws.Cells["H" + c].Value = vva.BWExcessPVP != 0 ? vva.BWExcessPVP.ToString() : "";
                                    ws.Cells["I" + c].Value = vva.CExcessPVP != 0 ? vva.CExcessPVP.ToString() : "";
                                    c++;
                                }

                            }
                        }

                        if (f != null && f.IncludeServices.Value == false)
                        {
                            ws.Cells["E27"].Value = "Valor";
                            ws.Cells["E28"].Value = "Taxa Fixa";
                            ws.Cells["E29"].Value = "Mensal";
                            switch (vva.RentBillingFrequency)
                            {
                                case 1: ws.Cells["E29"].Value = "Mensal"; break;
                                case 3: ws.Cells["E29"].Value = "Trimestral"; break;
                                case 6: ws.Cells["E29"].Value = "Semestral"; break;
                                default: ws.Cells["E29"].Value = "Mensal"; break;
                            }


                            ws.Cells["E40"].Value = vva.PVP != 0 ? vva.PVP.ToString() : "";

                        }
                        else
                        {
                            ws.Cells["E27"].Value = "";
                            ws.Cells["E28"].Value = "";
                            ws.Cells["E29"].Value = "";
                            ws.Cells["E40"].Value = "";
                        }
                    }

                    if (nv != null)
                    {
                        billingFrequency = nv.PageBillingFrequency.Value;
                        ws.Cells["E40"].Value = "";
                        ws.Cells["F40"].Value = "";
                        ws.Cells["G40"].Value = "";
                        ws.Cells["H25"].Value = "";

                        int idx = 30;
                        List<BB_Proposal_Quote> q = quotes.Where(x => x.TCP != null).ToList();
                        ws.Cells["C28"].Value = "";
                        ws.Cells["C29"].Value = "";
                        ws.Cells["D29"].Value = "";
                        foreach (var item in q)
                        {
                            BB_Equipamentos equi = lstEquipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                            if (equi != null)
                            {
                                List<BB_Proposal_Counters> counter = lstContadores.Where(x => x.OSID == item.ID).ToList();
                                if (counter.Count > 0)
                                {
                                    //ws.Cells["C28"].Value = "Contadores Iniciais";
                                    //ws.Cells["C29"].Value = "Preto";
                                    //ws.Cells["D29"].Value = "Cor";
                                    foreach (var i in counter)
                                    {
                                        ws.Cells["A" + idx].Value = equi.Name + "-" + i.serialNumber;
                                        ws.Cells["H" + idx].Value = nv.GlobalClickBW != 0 ? Math.Round(nv.GlobalClickBW.Value, 5) + "€" : "";
                                        ws.Cells["I" + idx].Value = nv.GlobalClickC != 0 ? Math.Round(nv.GlobalClickC.Value, 5) + "€" : "";
                                        idx++;
                                    }
                                }
                                else
                                {
                                    ws.Cells["A" + idx].Value = equi.Name;
                                    ws.Cells["H" + idx].Value = nv.GlobalClickBW != 0 ? Math.Round(nv.GlobalClickBW.Value, 5) + "€" : "";
                                    ws.Cells["I" + idx].Value = nv.GlobalClickC != 0 ? Math.Round(nv.GlobalClickC.Value, 5) + "€" : "";
                                    idx++;
                                }
                            }


                        }
                        string m = "";
                        switch (nv.PageBillingFrequency)
                        {
                            case 1:
                                m = "Mensal";
                                break;
                            case 3:
                                m = "Trimestral";
                                break;
                            case 6:
                                m = "Semestral";
                                break;
                            default:
                                break;
                        }
                        ws.Cells["F46"].Value = m;
                    }
                }



                ws.Cells["G44"].Value = active.ContractDuration + " meses";
                ws.Cells["A46"].Value = "A facturação de páginas" + strExcende + "produzidas terá uma períodicidade:";
                //ws.Cells["F46"].Value = m;

                ws.Cells["A51"].Value = "No final do prazo de vigência, o contrato de serviço considera-se automaticamente renovado por períodos iguais e sucessivos de 12 meses";
                ws.Cells["A52"].Value = "sem prejuízo do disposto nas condições especiais aplicáveis.";

                ws.Cells["A54"].Value = "A KONICA MINOLTA PORTUGAL garante que no caso de haver alguma alteração anual de preço no custo da página, a mesma não";
                ws.Cells["A55"].Value = "será em valor superior ao aumento determinado pelo índice de preços ao consumidor, segundo os dados publicados pelo Instituto";
                ws.Cells["A56"].Value = "Nacional de Estatística.";
                //KOnica Representante
                ws.Cells["A67"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws.Cells["A68"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws.Cells["A69"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                pck.Save();


                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");

            }
        }
        public void WritePropostaFinanceira(string path, int? ProposalID, BB_Proposal p, GerarProposta o)
        {
            try
            {
                BB_Proposal_PrazoDiferenciado prazodiferencido = new BB_Proposal_PrazoDiferenciado();
                BB_Proposal_Overvaluation sobrevalorizacao = new BB_Proposal_Overvaluation();

                BB_Proposal_Financing financiamento = new BB_Proposal_Financing();
                List<BB_Proposal_FinancingMonthly> financiamentoMensal = new List<BB_Proposal_FinancingMonthly>();
                List<BB_Proposal_FinancingTrimestral> financiamentoTrimestreal = new List<BB_Proposal_FinancingTrimestral>();

                BB_FinancingType FinancingType = new BB_FinancingType();
                BB_FinancingPaymentMethod aymentMethod = new BB_FinancingPaymentMethod();
                BB_FinancingContractType contractType = new BB_FinancingContractType();
                string servicosIncluidos = "";
                using (var db = new BB_DB_DEVEntities2())
                {
                    sobrevalorizacao = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == p.ID).FirstOrDefault();
                    financiamento = db.BB_Proposal_Financing.Where(x => x.ProposalID == ProposalID).FirstOrDefault();

                    if (financiamento != null)
                    {
                        financiamentoMensal = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == ProposalID && x.FinancingID == financiamento.ID).ToList();
                        financiamentoTrimestreal = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == ProposalID && x.FinancingID == financiamento.ID).ToList();
                        FinancingType = db.BB_FinancingType.Where(x => x.Code == financiamento.FinancingTypeCode).FirstOrDefault();
                        aymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == financiamento.PaymentMethodId).FirstOrDefault();
                        contractType = db.BB_FinancingContractType.Where(x => x.ID == financiamento.ContractTypeId).FirstOrDefault();
                        prazodiferencido = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == ProposalID && x.IsAproved.Value == true).FirstOrDefault();
                        servicosIncluidos = financiamento.IncludeServices.Value == true ? " com serviços incluídos" : "";
                    }
                }


                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                var ws = pck.Workbook.Worksheets["FINANCIAMENTO"];

                ws.Cells["A10"].Value = FinancingType.Type + servicosIncluidos + " - " + "Pagamento a " + financiamento.PaymentAfter + " dias após data da factura - " + aymentMethod.Type; //" - " + contractType.Company;
                                                                                                                                                                                            //Rendas Mensais

                if (o.MostrarPeriodoDeContracto.Value == true || o.PeriodoDeContracto.Split(' ')[0] == "N/A")
                {
                    if (prazodiferencido == null)
                    {
                        ws.Cells["A18"].Value = "";
                        ws.Cells["A19"].Value = "";
                        ws.Cells["A20"].Value = "";
                        ws.Cells["A21"].Value = "";

                        ws.Cells["A13"].Value = "Rendas Mensais";

                        ws.Cells["A14"].Value = "24 meses";
                        ws.Cells["B14"].Value = "36 meses";
                        ws.Cells["C14"].Value = "48 meses";
                        ws.Cells["D14"].Value = "60 meses";


                        if (financiamentoMensal.Count > 0)
                        {
                            int i = 0;

                            ws.Cells["A15"].Value = (i >= 0 && i < financiamentoMensal.Count) ? financiamentoMensal[i].Value.ToString() : "-"; i++;
                            ws.Cells["B15"].Value = (i >= 0 && i < financiamentoMensal.Count) ? financiamentoMensal[i].Value.ToString() : "-"; i++;
                            ws.Cells["C15"].Value = (i >= 0 && i < financiamentoMensal.Count) ? financiamentoMensal[i].Value.ToString() : "-"; i++;
                            ws.Cells["D15"].Value = (i >= 0 && i < financiamentoMensal.Count) ? financiamentoMensal[i].Value.ToString() : "-"; i++;
                            ws.Cells["E15"].Value = (i >= 0 && i < financiamentoMensal.Count) ? financiamentoMensal[i].Value.ToString() : "-";

                            ws.Cells["E14"].Value = (i >= 0 && i < financiamentoMensal.Count) ? "72 meses" : "-";
                        }
                        //Rendas Trimestre
                        ws.Cells["F13"].Value = "Rendas Trimestrais";

                        ws.Cells["F14"].Value = "8 Trimestres";
                        ws.Cells["G14"].Value = "12 Trimestres";
                        ws.Cells["H14"].Value = "16 Trimestres";
                        ws.Cells["I14"].Value = "20 Trimestres";

                        if (financiamentoTrimestreal.Count > 0)
                        {
                            int j = 0;

                            ws.Cells["F15"].Value = (j >= 0 && j < financiamentoTrimestreal.Count) ? financiamentoTrimestreal[j].Value : 0; j++;
                            ws.Cells["G15"].Value = (j >= 0 && j < financiamentoTrimestreal.Count) ? financiamentoTrimestreal[j].Value : 0; j++;
                            ws.Cells["H15"].Value = (j >= 0 && j < financiamentoTrimestreal.Count) ? financiamentoTrimestreal[j].Value : 0; j++;
                            ws.Cells["I15"].Value = (j >= 0 && j < financiamentoTrimestreal.Count) ? financiamentoTrimestreal[j].Value : 0; j++;
                        }
                    }
                    else if (prazodiferencido.IsComplete.Value && prazodiferencido.IsAproved.Value)
                    {
                        string frequencyString = "meses";
                        if (prazodiferencido.Frequency == 3)
                        {
                            frequencyString = "trimestres";
                        }
                        ws.Cells["A18"].Value = "Período do Contrato";
                        ws.Cells["A19"].Value = prazodiferencido.PrazoDiferenciado + " " + frequencyString;
                        ws.Cells["A20"].Value = "Renda Mensal";
                        ws.Cells["A21"].Value = prazodiferencido.ValorRenda;
                    }
                }
                else
                {
                    ws.Cells["A13"].Value = "";

                    ws.Cells["A14"].Value = "";
                    ws.Cells["B14"].Value = "";
                    ws.Cells["C14"].Value = "";
                    ws.Cells["D14"].Value = "";

                    ws.Cells["F13"].Value = "";

                    ws.Cells["F14"].Value = "";
                    ws.Cells["G14"].Value = "";
                    ws.Cells["H14"].Value = "";
                    ws.Cells["I14"].Value = "";

                    ws.Cells["F15"].Value = "";
                    ws.Cells["G15"].Value = "";
                    ws.Cells["H15"].Value = "";
                    ws.Cells["I15"].Value = "";

                    ws.Cells["A15"].Value = "";
                    ws.Cells["B15"].Value = "";
                    ws.Cells["C15"].Value = "";
                    ws.Cells["D15"].Value = "";
                    ws.Cells["E15"].Value = "";

                    ws.Cells["E14"].Value = "";


                    ws.Cells["A19"].Value = o.PeriodoDeContracto;
                    ws.Cells["A20"].Value = o.PeriodoDeContracto.Contains("Meses") ? "Renda Mensal" : "Renda Trimestral";



                    int? periocontrato = int.Parse(o.PeriodoDeContracto.Split(' ')[0]);

                    if (o.PeriodoDeContracto.Contains("Meses"))
                    {
                        foreach (var i in financiamentoMensal)
                        {
                            if (i.Contracto.Value == periocontrato)
                            {
                                ws.Cells["A21"].Value = i.Value;
                                break;

                            }
                        }

                    }
                    else
                    {
                        foreach (var i in financiamentoTrimestreal)
                        {
                            if (i.Contracto.Value == periocontrato)
                            {
                                ws.Cells["A21"].Value = i.Value;
                                break;

                            }
                        }

                    }

                    if (prazodiferencido != null && prazodiferencido.IsComplete.Value && prazodiferencido.IsAproved.Value)
                    {
                        string frequencyString = "meses";
                        if (prazodiferencido.Frequency == 3)
                        {
                            frequencyString = "trimestres";
                        }
                        ws.Cells["A18"].Value = "Período do Contrato";
                        ws.Cells["A19"].Value = prazodiferencido.PrazoDiferenciado + " " + frequencyString;
                        ws.Cells["A20"].Value = "Renda Mensal";
                        ws.Cells["A21"].Value = prazodiferencido.ValorRenda;
                    }
                }

                if (o.ValorTransferenciaPropriedade > 24 && FinancingType.Code == 3)
                {
                    //ws.Cells["A24"].Value = "No final do contrato o valor a pagar pela transferência de propriedade é de: " + o.ValorTransferenciaPropriedade;
                    //Pedido do Hugo Silva
                    ws.Cells["A24"].Value = "";
                }
                else
                {
                    ws.Cells["A24"].Value = "";
                }

                //Despesas De contrcto
                if (o.Mandatadas_Directo_RetirarDespesas.Value == true)
                {
                    ws.Cells["D18"].Value = "";
                    ws.Cells["D19"].Value = "";
                    ws.Cells["D20"].Value = "";
                }
                else
                {
                    ws.Cells["D20"].Value = "100€";
                }


                if (financiamento.FinancingTypeCode == 0)
                {
                    ws.Cells["A10"].Value = "Venda Directa";

                    ws.Cells["D18"].Value = "";
                    ws.Cells["D19"].Value = "";
                    ws.Cells["D20"].Value = "";


                    ws.Cells["A13"].Value = "";

                    ws.Cells["A14"].Value = "";
                    ws.Cells["B14"].Value = "";
                    ws.Cells["C14"].Value = "";
                    ws.Cells["D14"].Value = "";

                    ws.Cells["F13"].Value = "";

                    ws.Cells["F14"].Value = "";
                    ws.Cells["G14"].Value = "";
                    ws.Cells["H14"].Value = "";
                    ws.Cells["I14"].Value = "";

                    ws.Cells["F15"].Value = "";
                    ws.Cells["G15"].Value = "";
                    ws.Cells["H15"].Value = "";
                    ws.Cells["I15"].Value = "";

                    ws.Cells["A15"].Value = "";
                    ws.Cells["B15"].Value = "";
                    ws.Cells["C15"].Value = "";
                    ws.Cells["D15"].Value = "";
                    ws.Cells["E15"].Value = "";

                    ws.Cells["E14"].Value = "";

                    ws.Cells["A18"].Value = "";
                    ws.Cells["A19"].Value = "";
                    ws.Cells["A20"].Value = "";

                    ws.Cells["A35"].Value = "";
                    ws.Cells["A36"].Value = "";
                    ws.Cells["A37"].Value = "";

                }



                ws.Cells["A35"].Value = "Seguro";
                ws.Cells["A36"].Value = "Anual";
                ws.Cells["A37"].Value = "Incluído";

                //KOnica Representante
                ws.Cells["A59"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws.Cells["A60"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws.Cells["A61"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                pck.Save();


                List<string> lstObs = new List<string>();
                lstObs = o.ObservacoesFinanciamento.Split('\n').ToList();
                int row = 41;
                foreach (var item in lstObs)
                {
                    ws.Cells["A" + row.ToString()].Value = item.ToString();
                    row++;
                    if (row == 44)
                        break;
                }

                pck.Save();
                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");

            }
        }
        public void WriteQuote_1(string path, int? ProposalID, BB_Proposal p, GerarProposta o)
        {
            try
            {

                List<BB_Proposal_Quote> quotes = new List<BB_Proposal_Quote>();
                List<BB_Proposal_OPSImplement> opsImplement = new List<BB_Proposal_OPSImplement>();
                BB_Proposal_Overvaluation sobrevalorizacao = new BB_Proposal_Overvaluation();
                BB_Proposal_Upturn retoma = new BB_Proposal_Upturn();
                double? LeiDaCopiaPriada = 0;

                using (var db = new BB_DB_DEVEntities2())
                {
                    quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == ProposalID).ToList();
                    opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == ProposalID).ToList();
                    sobrevalorizacao = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == p.ID).FirstOrDefault();
                    retoma = db.BB_Proposal_Upturn.Where(x => x.ProposalID == p.ID).FirstOrDefault();

                    foreach (var quote in quotes)
                    {
                        double? TCP = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).Select(x => x.TCP).FirstOrDefault();
                        if (TCP is null)
                            TCP = 0;
                        if (quote.IsUsed.GetValueOrDefault() == false)
                            LeiDaCopiaPriada = (TCP * quote.Qty) + LeiDaCopiaPriada;
                    }

                }
                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                var ws = pck.Workbook.Worksheets["PROPOSTA_FINANCEIRA"];
                var ws1 = pck.Workbook.Worksheets["PROPOSTA_FINANCEIRA_1"];
                var ws2 = pck.Workbook.Worksheets["PROPOSTA_FINANCEIRA_2"];

                int idxRow = 14;
                double? TotalPVP = 0;
                double? SubTotal = 0;
                double? descontoTotal = 0;

                int nrItem = 0;
                int escrever2FOlha = 14;
                int escrever3FOlha = 14;
                //Com sobrevalorizacao
                if (sobrevalorizacao != null || retoma != null)
                {
                    foreach (var quote in quotes)
                    {
                        if (nrItem < 14)
                        {
                            ws.Cells["A" + idxRow.ToString()].Value = quote.Qty + " x " + quote.Description;
                            ws.Cells["F" + idxRow.ToString()].Value = "";
                            ws.Cells["G" + idxRow.ToString()].Value = "";
                            ws.Cells["H" + idxRow.ToString()].Value = "";
                            ws.Cells["I" + idxRow.ToString()].Value = "";

                        }
                        else
                        {
                            ws1.Cells["A" + escrever2FOlha.ToString()].Value = quote.Qty + " x " + quote.Description;
                            ws1.Cells["F" + escrever2FOlha.ToString()].Value = "";
                            ws1.Cells["G" + escrever2FOlha.ToString()].Value = "";
                            ws1.Cells["H" + escrever2FOlha.ToString()].Value = "";
                            ws1.Cells["I" + escrever2FOlha.ToString()].Value = "";
                            escrever2FOlha++;

                        }
                        idxRow++;
                        nrItem++;
                    }
                    foreach (var quote in opsImplement)
                    {
                        if (nrItem < 14)
                        {
                            ws.Cells["A" + idxRow.ToString()].Value = quote.Quantity + " x " + quote.Name + " - " + quote.Description;
                            ws.Cells["F" + idxRow.ToString()].Value = "";
                            ws.Cells["G" + idxRow.ToString()].Value = "";
                            ws.Cells["H" + idxRow.ToString()].Value = "";
                            ws.Cells["I" + idxRow.ToString()].Value = "";
                        }
                        else
                        {
                            ws.Cells["A" + escrever2FOlha.ToString()].Value = quote.Quantity + " x " + quote.Name + " - " + quote.Description;
                            ws.Cells["F" + escrever2FOlha.ToString()].Value = "";
                            ws.Cells["G" + escrever2FOlha.ToString()].Value = "";
                            ws.Cells["H" + escrever2FOlha.ToString()].Value = "";
                            ws.Cells["I" + escrever2FOlha.ToString()].Value = "";
                            escrever2FOlha++;

                        }
                        idxRow++;
                        nrItem++;
                    }
                }
                else
                {
                    foreach (var quote in quotes)
                    {
                        if (nrItem <= 14)
                        {

                            ws.Cells["A" + idxRow.ToString()].Value = quote.Qty + " x " + quote.Description;
                            ws.Cells["F" + idxRow.ToString()].Value = quote.TotalPVP;
                            ws.Cells["G" + idxRow.ToString()].Value = quote.DiscountPercentage > 0 ? Math.Round(quote.DiscountPercentage.Value, 2).ToString() + "%" : "";
                            ws.Cells["H" + idxRow.ToString()].Value = (quote.TotalPVP.Value - quote.TotalNetsale.Value) > 0 ? Math.Round(quote.TotalPVP.Value - quote.TotalNetsale.Value, 2).ToString() + "€" : "";
                            ws.Cells["I" + idxRow.ToString()].Value = quote.TotalNetsale;

                            TotalPVP += quote.TotalPVP;
                            SubTotal += quote.TotalNetsale;
                            descontoTotal += (quote.TotalPVP.Value - quote.TotalNetsale.Value);
                        }
                        if (nrItem > 14 && nrItem <= 29) //2Folha
                        {
                            ws1.Cells["A" + escrever2FOlha.ToString()].Value = quote.Qty + " x " + quote.Description;
                            ws1.Cells["F" + escrever2FOlha.ToString()].Value = quote.TotalPVP;
                            ws1.Cells["G" + escrever2FOlha.ToString()].Value = quote.DiscountPercentage > 0 ? Math.Round(quote.DiscountPercentage.Value, 2).ToString() + "%" : "";
                            ws1.Cells["H" + escrever2FOlha.ToString()].Value = (quote.TotalPVP.Value - quote.TotalNetsale.Value) > 0 ? Math.Round(quote.TotalPVP.Value - quote.TotalNetsale.Value, 2).ToString() + "€" : "";
                            ws1.Cells["I" + escrever2FOlha.ToString()].Value = quote.TotalNetsale;

                            TotalPVP += quote.TotalPVP;
                            SubTotal += quote.TotalNetsale;
                            descontoTotal += (quote.TotalPVP.Value - quote.TotalNetsale.Value);
                            escrever2FOlha++;
                        }
                        if (nrItem > 29) //3Folha
                        {
                            ws2.Cells["A" + escrever3FOlha.ToString()].Value = quote.Qty + " x " + quote.Description;
                            ws2.Cells["F" + escrever3FOlha.ToString()].Value = quote.TotalPVP;
                            ws2.Cells["G" + escrever3FOlha.ToString()].Value = quote.DiscountPercentage > 0 ? Math.Round(quote.DiscountPercentage.Value, 2).ToString() + "%" : "";
                            ws2.Cells["H" + escrever3FOlha.ToString()].Value = (quote.TotalPVP.Value - quote.TotalNetsale.Value) > 0 ? Math.Round(quote.TotalPVP.Value - quote.TotalNetsale.Value, 2).ToString() + "€" : "";
                            ws2.Cells["I" + escrever3FOlha.ToString()].Value = quote.TotalNetsale;

                            TotalPVP += quote.TotalPVP;
                            SubTotal += quote.TotalNetsale;
                            descontoTotal += (quote.TotalPVP.Value - quote.TotalNetsale.Value);
                            escrever3FOlha++;
                        }
                        nrItem++;
                        idxRow++;
                    }
                    foreach (var quote in opsImplement)
                    {
                        double discountPercentage = 0;
                        double totalPVP = 0;
                        if (quote.PVP != null)
                        {
                            discountPercentage = (1 - quote.UnitDiscountPrice.GetValueOrDefault() / quote.PVP.GetValueOrDefault()) * 100;
                            totalPVP = quote.PVP.GetValueOrDefault() * quote.Quantity.GetValueOrDefault();
                        }
                        if (nrItem <= 14)
                        {
                            ws.Cells["A" + idxRow.ToString()].Value = quote.Quantity + " x " + quote.Name + " - " + quote.Description;
                            ws.Cells["F" + idxRow.ToString()].Value = quote.PVP;
                            ws.Cells["G" + idxRow.ToString()].Value = Math.Round(discountPercentage, 2).ToString() + "%";
                            ws.Cells["H" + idxRow.ToString()].Value = quote.UnitDiscountPrice + "€";
                            ws.Cells["I" + idxRow.ToString()].Value = quote.UnitDiscountPrice * quote.Quantity;
                        }
                        if (nrItem > 14 && nrItem <= 29) //2Folha
                        {
                            ws.Cells["A" + escrever2FOlha.ToString()].Value = quote.Quantity + " x " + quote.Name + " - " + quote.Description;
                            ws.Cells["F" + escrever2FOlha.ToString()].Value = quote.PVP;
                            ws.Cells["G" + escrever2FOlha.ToString()].Value = Math.Round(discountPercentage, 2).ToString() + "%";
                            ws.Cells["H" + escrever2FOlha.ToString()].Value = quote.UnitDiscountPrice + "€";
                            ws.Cells["I" + escrever2FOlha.ToString()].Value = quote.UnitDiscountPrice * quote.Quantity;
                            escrever2FOlha++;
                        }
                        if (nrItem > 29) //3Folha
                        {
                            ws.Cells["A" + escrever3FOlha.ToString()].Value = quote.Quantity + " x " + quote.Name + " - " + quote.Description;
                            ws.Cells["F" + escrever3FOlha.ToString()].Value = quote.PVP;
                            ws.Cells["G" + escrever3FOlha.ToString()].Value = Math.Round(discountPercentage, 2).ToString() + "%";
                            ws.Cells["H" + escrever3FOlha.ToString()].Value = quote.UnitDiscountPrice + "€";
                            ws.Cells["I" + escrever3FOlha.ToString()].Value = quote.UnitDiscountPrice * quote.Quantity;
                            escrever3FOlha++;
                        }
                        TotalPVP += quote.PVP;
                        SubTotal += quote.UnitDiscountPrice * quote.Quantity;
                        descontoTotal += totalPVP - (quote.UnitDiscountPrice * quote.Quantity);
                        nrItem++;
                        idxRow++;
                    }
                }

                double? diffSub = (((descontoTotal.Value * 100) / TotalPVP.Value));

                ////Headers Quote  //Com sobrevalorizacao
                if (sobrevalorizacao != null || retoma != null)
                {
                    double dSobrevalorizacao = sobrevalorizacao != null ? sobrevalorizacao.Total.GetValueOrDefault() : 0;
                    double dRetoma = retoma != null ? retoma.Total.GetValueOrDefault() : 0;

                    ws.Cells["F13"].Value = "";
                    ws.Cells["G13"].Value = "";
                    ws.Cells["I13"].Value = "";

                    //Footer
                    ws.Cells["E29"].Value = "";
                    ws.Cells["G29"].Value = "";
                    ws.Cells["I29"].Value = "";

                    ws.Cells["I30"].Value = p.ValueTotal + dSobrevalorizacao + dRetoma;
                    if (retoma != null && retoma.Total != 0)
                    {
                        ws.Cells["I31"].Value = retoma.Total.GetValueOrDefault();
                        ws.Cells["A31"].Value = "Retoma V/Equipamento";
                    }
                    else
                    {
                        ws.Cells["I31"].Value = "";
                        ws.Cells["A31"].Value = "";
                    }

                    ws.Cells["I32"].Value = (p.ValueTotal + dSobrevalorizacao + dRetoma) - dRetoma;

                    //Lei da copia privada
                    ws.Cells["I33"].Value = LeiDaCopiaPriada;

                    //SubTotal
                    ws.Cells["I34"].Value = p.ValueTotal + dSobrevalorizacao + dRetoma + LeiDaCopiaPriada;

                }
                else
                {
                    int totalCount = quotes.Count + opsImplement.Count;
                    if (totalCount > 30)
                    {
                        ws.Cells["F13"].Value = "PVP";
                        ws.Cells["G13"].Value = "Desconto";
                        ws.Cells["I13"].Value = "Preço Final";

                        //Footer
                        ws.Cells["E29"].Value = "";
                        ws.Cells["F30"].Value = "";
                        ws.Cells["A32"].Value = "";
                        ws.Cells["I32"].Value = "";
                        ws.Cells["A33"].Value = "";
                        ws.Cells["I33"].Value = "";
                        ws.Cells["H34"].Value = "";
                        ws.Cells["I34"].Value = "";
                        //double? diffSub = (((descontoTotal.Value * 100) / TotalPVP.Value));

                        ws.Cells["G29"].Value = "";
                        ws.Cells["G30"].Value = "";

                        ws.Cells["G29"].Value = "";
                        ws.Cells["H30"].Value = "";

                        ws.Cells["I29"].Value = "";


                        ws.Cells["I30"].Value = "";
                        ws.Cells["I32"].Value = "";

                        //Lei da copia privada
                        ws.Cells["I33"].Value = "";

                        //SubTotal
                        ws.Cells["I34"].Value = "";
                        ws.Cells["I35"].Value = "";


                        ws.Cells["I31"].Value = "";
                        ws.Cells["A31"].Value = "";

                        ////////////////////// 2 folha
                        ws1.Cells["F13"].Value = "PVP";
                        ws1.Cells["G13"].Value = "Desconto";
                        ws1.Cells["I13"].Value = "Preço Final";

                        //Footer
                        ws.Cells["E29"].Value = "";
                        ws.Cells["F30"].Value = "";
                        ws.Cells["A32"].Value = "";
                        ws.Cells["I32"].Value = "";
                        ws.Cells["A33"].Value = "";
                        ws.Cells["I33"].Value = "";
                        ws.Cells["H34"].Value = "";
                        ws.Cells["I34"].Value = "";

                        ws1.Cells["E29"].Value = "";
                        ws1.Cells["F30"].Value = "";
                        ws1.Cells["A32"].Value = "";
                        ws1.Cells["I32"].Value = "";
                        ws1.Cells["A33"].Value = "";
                        ws1.Cells["I33"].Value = "";
                        ws1.Cells["H34"].Value = "";
                        ws1.Cells["I34"].Value = "";


                        ws1.Cells["F29"].Value = "";
                        ws1.Cells["E29"].Value = "";
                        ws1.Cells["G29"].Value = "";
                        ws1.Cells["G30"].Value = "";

                        ws1.Cells["G29"].Value = "";
                        ws1.Cells["H30"].Value = "";

                        ws1.Cells["I29"].Value = "";


                        ws1.Cells["I30"].Value = "";
                        ws1.Cells["I32"].Value = "";

                        //Lei da copia privada
                        ws1.Cells["I33"].Value = "";

                        //SubTotal
                        ws1.Cells["I34"].Value = "";
                        ws1.Cells["I35"].Value = "";


                        ws1.Cells["I31"].Value = "";
                        ws1.Cells["A31"].Value = "";

                        ////////////////////// 3 folha
                        ws2.Cells["F13"].Value = "PVP";
                        ws2.Cells["G13"].Value = "Desconto";
                        ws2.Cells["I13"].Value = "Preço Final";

                        //Footer
                        ws2.Cells["E29"].Value = "Total PVP";
                        ws2.Cells["F30"].Value = TotalPVP;



                        ws2.Cells["G29"].Value = "Desconto Total";
                        ws2.Cells["G30"].Value = Math.Round(diffSub.Value, 2) + "%";

                        ws2.Cells["G29"].Value = "Desconto Total";
                        ws2.Cells["H30"].Value = Math.Round(descontoTotal.Value, 2);

                        ws2.Cells["I29"].Value = "Sub-Total";


                        ws2.Cells["I30"].Value = p.ValueTotal;
                        ws2.Cells["I32"].Value = p.ValueTotal;

                        //Lei da copia privada
                        ws2.Cells["I33"].Value = LeiDaCopiaPriada;

                        //SubTotal
                        ws2.Cells["I34"].Value = p.ValueTotal + LeiDaCopiaPriada;
                        ws2.Cells["I35"].Value = SubTotal;


                        ws2.Cells["I31"].Value = "";
                        ws2.Cells["A31"].Value = "";
                    }
                    // 2folha
                    if (totalCount > 15 && totalCount <= 30)
                    {
                        ws.Cells["F13"].Value = "PVP";
                        ws.Cells["G13"].Value = "Desconto";
                        ws.Cells["I13"].Value = "Preço Final";

                        //Footer
                        ws.Cells["E29"].Value = "";
                        ws.Cells["F30"].Value = "";
                        ws.Cells["A32"].Value = "";
                        ws.Cells["I32"].Value = "";
                        ws.Cells["A33"].Value = "";
                        ws.Cells["I33"].Value = "";
                        ws.Cells["H34"].Value = "";
                        ws.Cells["I34"].Value = "";
                        //double? diffSub = (((descontoTotal.Value * 100) / TotalPVP.Value));

                        ws.Cells["G29"].Value = "";
                        ws.Cells["G30"].Value = "";

                        ws.Cells["G29"].Value = "";
                        ws.Cells["H30"].Value = "";

                        ws.Cells["I29"].Value = "";


                        ws.Cells["I30"].Value = "";
                        ws.Cells["I32"].Value = "";

                        //Lei da copia privada
                        ws.Cells["I33"].Value = "";

                        //SubTotal
                        ws.Cells["I34"].Value = "";
                        ws.Cells["I35"].Value = "";


                        ws.Cells["I31"].Value = "";
                        ws.Cells["A31"].Value = "";

                        ws1.Cells["F13"].Value = "PVP";
                        ws1.Cells["G13"].Value = "Desconto";
                        ws1.Cells["I13"].Value = "Preço Final";

                        //Footer
                        ws1.Cells["E29"].Value = "Total PVP";
                        ws1.Cells["F30"].Value = TotalPVP;



                        ws1.Cells["G29"].Value = "Desconto Total";
                        ws1.Cells["G30"].Value = Math.Round(diffSub.Value, 2) + "%";

                        ws1.Cells["G29"].Value = "Desconto Total";
                        ws1.Cells["H30"].Value = Math.Round(descontoTotal.Value, 2);

                        ws1.Cells["I29"].Value = "Sub-Total";


                        ws1.Cells["I30"].Value = p.ValueTotal;
                        ws1.Cells["I32"].Value = p.ValueTotal;

                        //Lei da copia privada
                        ws1.Cells["I33"].Value = LeiDaCopiaPriada;

                        //SubTotal
                        ws1.Cells["I34"].Value = p.ValueTotal + LeiDaCopiaPriada;
                        ws1.Cells["I35"].Value = SubTotal;


                        ws1.Cells["I31"].Value = "";
                        ws1.Cells["A31"].Value = "";
                    }
                    if (totalCount <= 15)
                    {
                        ws.Cells["F13"].Value = "PVP";
                        ws.Cells["G13"].Value = "Desconto";
                        ws.Cells["I13"].Value = "Preço Final";

                        //Footer
                        ws.Cells["E29"].Value = "Total PVP";
                        ws.Cells["F30"].Value = TotalPVP;



                        ws.Cells["G29"].Value = "Desconto Total";
                        ws.Cells["G30"].Value = Math.Round(diffSub.Value, 2) + "%";

                        ws.Cells["G29"].Value = "Desconto Total";
                        ws.Cells["H30"].Value = Math.Round(descontoTotal.Value, 2);

                        ws.Cells["I29"].Value = "Sub-Total";


                        ws.Cells["I30"].Value = p.ValueTotal;
                        ws.Cells["I32"].Value = p.ValueTotal;

                        //Lei da copia privada
                        ws.Cells["I33"].Value = LeiDaCopiaPriada;

                        //SubTotal
                        ws.Cells["I34"].Value = p.ValueTotal + LeiDaCopiaPriada;
                        ws.Cells["I35"].Value = SubTotal;


                        ws.Cells["I31"].Value = "";
                        ws.Cells["A31"].Value = "";

                    }
                }

                //KOnica Representante
                ws.Cells["A39"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws.Cells["A40"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws.Cells["A41"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                //KOnica Representante
                ws1.Cells["A39"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws1.Cells["A40"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws1.Cells["A41"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                //KOnica Representante
                ws2.Cells["A39"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                ws2.Cells["A40"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                ws2.Cells["A41"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                idxRow = 14;
                if (o.Ocultar_PVP_DESCONTO)
                {
                    ws.Cells["F13"].Value = "";
                    ws.Cells["G13"].Value = "";

                    ws1.Cells["F13"].Value = "";
                    ws1.Cells["G13"].Value = "";

                    ws2.Cells["F13"].Value = "";
                    ws2.Cells["G13"].Value = "";

                    ws.Cells["E29"].Value = "";
                    ws.Cells["G29"].Value = "";
                    ws.Cells["F30"].Value = "";
                    ws.Cells["G30"].Value = "";
                    ws.Cells["H30"].Value = "";

                    ws1.Cells["E29"].Value = "";
                    ws1.Cells["G29"].Value = "";
                    ws1.Cells["F30"].Value = "";
                    ws1.Cells["G30"].Value = "";
                    ws1.Cells["H30"].Value = "";

                    ws2.Cells["E29"].Value = "";
                    ws2.Cells["G29"].Value = "";
                    ws2.Cells["F30"].Value = "";
                    ws2.Cells["G30"].Value = "";
                    ws2.Cells["H30"].Value = "";


                    foreach (var quote in quotes)
                    {
                        ws.Cells["F" + idxRow.ToString()].Value = "";
                        ws.Cells["G" + idxRow.ToString()].Value = "";
                        ws.Cells["H" + idxRow.ToString()].Value = "";

                        ws1.Cells["F" + idxRow.ToString()].Value = "";
                        ws1.Cells["G" + idxRow.ToString()].Value = "";
                        ws1.Cells["H" + idxRow.ToString()].Value = "";

                        ws2.Cells["F" + idxRow.ToString()].Value = "";
                        ws2.Cells["G" + idxRow.ToString()].Value = "";
                        ws2.Cells["H" + idxRow.ToString()].Value = "";

                        idxRow++;
                    }
                    foreach (var quote in opsImplement)
                    {
                        ws.Cells["F" + idxRow.ToString()].Value = "";
                        ws.Cells["G" + idxRow.ToString()].Value = "";
                        ws.Cells["H" + idxRow.ToString()].Value = "";

                        ws1.Cells["F" + idxRow.ToString()].Value = "";
                        ws1.Cells["G" + idxRow.ToString()].Value = "";
                        ws1.Cells["H" + idxRow.ToString()].Value = "";

                        ws2.Cells["F" + idxRow.ToString()].Value = "";
                        ws2.Cells["G" + idxRow.ToString()].Value = "";
                        ws2.Cells["H" + idxRow.ToString()].Value = "";

                        idxRow++;
                    }
                }

                idxRow = 14;
                if (o.Ocultar_PRECOFINAL)
                {
                    ws.Cells["I13"].Value = "";
                    ws.Cells["I29"].Value = "";

                    ws1.Cells["I13"].Value = "";
                    ws1.Cells["I29"].Value = "";

                    ws2.Cells["I13"].Value = "";
                    ws2.Cells["I29"].Value = "";

                    foreach (var quote in quotes)
                    {
                        ws.Cells["I" + idxRow.ToString()].Value = "";
                        ws1.Cells["I" + idxRow.ToString()].Value = "";
                        ws2.Cells["I" + idxRow.ToString()].Value = "";
                        idxRow++;
                    }
                    foreach (var quote in opsImplement)
                    {
                        ws.Cells["I" + idxRow.ToString()].Value = "";
                        ws1.Cells["I" + idxRow.ToString()].Value = "";
                        ws2.Cells["I" + idxRow.ToString()].Value = "";
                        idxRow++;
                    }
                }


                if (o.Ocultar_TOTAIS)
                {
                    idxRow = 14;
                    foreach (var quote in quotes)
                    {

                        ws.Cells["G29"].Value = "";
                        ws.Cells["G30"].Value = "";
                        ws.Cells["H30"].Value = "";
                        ws.Cells["E29"].Value = "";
                        ws.Cells["F13"].Value = "";
                        ws.Cells["I29"].Value = "";


                        ws.Cells["G13"].Value = "";
                        ws.Cells["G" + idxRow.ToString()].Value = "";
                        ws.Cells["H" + idxRow.ToString()].Value = "";
                        ws.Cells["F" + idxRow.ToString()].Value = "";
                        ws.Cells["I" + idxRow.ToString()].Value = "";

                        //2folha
                        ws1.Cells["G29"].Value = "";
                        ws1.Cells["G30"].Value = "";
                        ws1.Cells["H30"].Value = "";
                        ws1.Cells["F" + idxRow.ToString()].Value = "";
                        ws1.Cells["I" + idxRow.ToString()].Value = "";
                        ws1.Cells["E29"].Value = "";
                        ws1.Cells["F13"].Value = "";
                        ws1.Cells["G13"].Value = "";
                        ws1.Cells["G" + idxRow.ToString()].Value = "";
                        ws1.Cells["H" + idxRow.ToString()].Value = "";
                        ws1.Cells["I29"].Value = "";
                        //3folha
                        ws2.Cells["G29"].Value = "";
                        ws2.Cells["G30"].Value = "";
                        ws2.Cells["H30"].Value = "";
                        ws2.Cells["F" + idxRow.ToString()].Value = "";
                        ws2.Cells["I" + idxRow.ToString()].Value = "";
                        ws2.Cells["E29"].Value = "";
                        ws2.Cells["F13"].Value = "";
                        ws2.Cells["G13"].Value = "";
                        ws2.Cells["G" + idxRow.ToString()].Value = "";
                        ws2.Cells["H" + idxRow.ToString()].Value = "";
                        ws2.Cells["I29"].Value = "";

                        ws.Cells["I13"].Value = "";
                        ws1.Cells["I13"].Value = "";
                        ws2.Cells["I13"].Value = "";

                        ws.Cells["I29"].Value = "";
                        ws.Cells["I" + idxRow.ToString()].Value = "";

                        ws.Cells["H30"].Value = "";
                        ws.Cells["G30"].Value = "";
                        ws.Cells["F30"].Value = "";
                        ws.Cells["I30"].Value = "";
                        ws.Cells["A31"].Value = "";
                        ws.Cells["I31"].Value = "";
                        ws.Cells["A32"].Value = "";
                        ws.Cells["I32"].Value = "";
                        ws.Cells["A33"].Value = "";
                        ws.Cells["I33"].Value = "";
                        ws.Cells["H34"].Value = "";
                        ws.Cells["I34"].Value = "";
                        ws.Cells["I36"].Value = "";

                        ws1.Cells["H30"].Value = "";
                        ws1.Cells["F30"].Value = "";
                        ws1.Cells["G30"].Value = "";
                        ws1.Cells["I30"].Value = "";
                        ws1.Cells["A31"].Value = "";
                        ws1.Cells["I31"].Value = "";
                        ws1.Cells["A32"].Value = "";
                        ws1.Cells["I32"].Value = "";
                        ws1.Cells["A33"].Value = "";
                        ws1.Cells["I33"].Value = "";
                        ws1.Cells["H34"].Value = "";
                        ws1.Cells["I34"].Value = "";
                        ws1.Cells["I36"].Value = "";

                        ws2.Cells["H30"].Value = "";
                        ws2.Cells["G30"].Value = "";
                        ws2.Cells["F30"].Value = "";
                        ws2.Cells["I30"].Value = "";
                        ws2.Cells["A31"].Value = "";
                        ws2.Cells["I31"].Value = "";
                        ws2.Cells["A32"].Value = "";
                        ws2.Cells["I32"].Value = "";
                        ws2.Cells["A33"].Value = "";
                        ws2.Cells["I33"].Value = "";
                        ws2.Cells["H34"].Value = "";
                        ws2.Cells["I34"].Value = "";
                        ws2.Cells["I36"].Value = "";


                        idxRow++;
                    }
                    foreach (var quote in opsImplement)
                    {

                        ws.Cells["G29"].Value = "";
                        ws.Cells["G30"].Value = "";
                        ws.Cells["H30"].Value = "";
                        ws.Cells["E29"].Value = "";
                        ws.Cells["F13"].Value = "";
                        ws.Cells["I29"].Value = "";


                        ws.Cells["G13"].Value = "";
                        ws.Cells["G" + idxRow.ToString()].Value = "";
                        ws.Cells["H" + idxRow.ToString()].Value = "";
                        ws.Cells["F" + idxRow.ToString()].Value = "";
                        ws.Cells["I" + idxRow.ToString()].Value = "";

                        //2folha
                        ws1.Cells["G29"].Value = "";
                        ws1.Cells["G30"].Value = "";
                        ws1.Cells["H30"].Value = "";
                        ws1.Cells["F" + idxRow.ToString()].Value = "";
                        ws1.Cells["I" + idxRow.ToString()].Value = "";
                        ws1.Cells["E29"].Value = "";
                        ws1.Cells["F13"].Value = "";
                        ws1.Cells["G13"].Value = "";
                        ws1.Cells["G" + idxRow.ToString()].Value = "";
                        ws1.Cells["H" + idxRow.ToString()].Value = "";
                        ws1.Cells["I29"].Value = "";
                        //3folha
                        ws2.Cells["G29"].Value = "";
                        ws2.Cells["G30"].Value = "";
                        ws2.Cells["H30"].Value = "";
                        ws2.Cells["F" + idxRow.ToString()].Value = "";
                        ws2.Cells["I" + idxRow.ToString()].Value = "";
                        ws2.Cells["E29"].Value = "";
                        ws2.Cells["F13"].Value = "";
                        ws2.Cells["G13"].Value = "";
                        ws2.Cells["G" + idxRow.ToString()].Value = "";
                        ws2.Cells["H" + idxRow.ToString()].Value = "";
                        ws2.Cells["I29"].Value = "";

                        ws.Cells["I13"].Value = "";
                        ws1.Cells["I13"].Value = "";
                        ws2.Cells["I13"].Value = "";

                        ws.Cells["I29"].Value = "";
                        ws.Cells["I" + idxRow.ToString()].Value = "";

                        ws.Cells["H30"].Value = "";
                        ws.Cells["G30"].Value = "";
                        ws.Cells["F30"].Value = "";
                        ws.Cells["I30"].Value = "";
                        ws.Cells["A31"].Value = "";
                        ws.Cells["I31"].Value = "";
                        ws.Cells["A32"].Value = "";
                        ws.Cells["I32"].Value = "";
                        ws.Cells["A33"].Value = "";
                        ws.Cells["I33"].Value = "";
                        ws.Cells["H34"].Value = "";
                        ws.Cells["I34"].Value = "";
                        ws.Cells["I36"].Value = "";

                        ws1.Cells["H30"].Value = "";
                        ws1.Cells["F30"].Value = "";
                        ws1.Cells["G30"].Value = "";
                        ws1.Cells["I30"].Value = "";
                        ws1.Cells["A31"].Value = "";
                        ws1.Cells["I31"].Value = "";
                        ws1.Cells["A32"].Value = "";
                        ws1.Cells["I32"].Value = "";
                        ws1.Cells["A33"].Value = "";
                        ws1.Cells["I33"].Value = "";
                        ws1.Cells["H34"].Value = "";
                        ws1.Cells["I34"].Value = "";
                        ws1.Cells["I36"].Value = "";

                        ws2.Cells["H30"].Value = "";
                        ws2.Cells["G30"].Value = "";
                        ws2.Cells["F30"].Value = "";
                        ws2.Cells["I30"].Value = "";
                        ws2.Cells["A31"].Value = "";
                        ws2.Cells["I31"].Value = "";
                        ws2.Cells["A32"].Value = "";
                        ws2.Cells["I32"].Value = "";
                        ws2.Cells["A33"].Value = "";
                        ws2.Cells["I33"].Value = "";
                        ws2.Cells["H34"].Value = "";
                        ws2.Cells["I34"].Value = "";
                        ws2.Cells["I36"].Value = "";


                        idxRow++;
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
        public void WriteContractoQuote(string path, int? ProposalID, BB_Proposal p)
        {
            try
            {
                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo ia = new LoadProposalInfo();
                ia.ProposalId = p.ID;
                ActionResponse a = p1.LoadProposal(ia);

                BB_Proposal_PrazoDiferenciado prazodiferencido = new BB_Proposal_PrazoDiferenciado();
                List<BB_Proposal_Quote> quotes = new List<BB_Proposal_Quote>();
                List<BB_Proposal_OPSImplement> opsImplement = new List<BB_Proposal_OPSImplement>();
                List<BB_Proposal_Quote_RS> quotesRs = new List<BB_Proposal_Quote_RS>();
                List<BB_Proposal_OPSManage> opsManage = new List<BB_Proposal_OPSManage>();
                List<BB_Proposal_Overvaluation> sobrevalorizacao = new List<BB_Proposal_Overvaluation>();

                BB_Proposal_Financing financiamento = new BB_Proposal_Financing();
                List<BB_Proposal_FinancingMonthly> financiamentoMensal = new List<BB_Proposal_FinancingMonthly>();
                List<BB_Proposal_FinancingTrimestral> financiamentoTrimestreal = new List<BB_Proposal_FinancingTrimestral>();
                BB_Proposal_PrintingServices psconfigs = new BB_Proposal_PrintingServices();

                BB_FinancingType FinancingType = new BB_FinancingType();
                BB_FinancingPaymentMethod aymentMethod = new BB_FinancingPaymentMethod();
                BB_FinancingContractType contractType = new BB_FinancingContractType();
                BB_Proposal_Vva vva1 = new BB_Proposal_Vva();

                BB_PrintingServices active = null;
                BB_VVA vva = null;
                BB_PrintingServices_NoVolume nv = null;

                string tipoNEgocio = "";

                double? valorServiço = 0;
                double? feeValor = 0;
                int frequency = 1;
                int count = 0;
                string servicosIncluidos = "";
                BB_Proposal_PrintingServices2 ps2 = null;
                double? opsmanageTotal = 0;
                double? ServRecoTotal = 0;
                using (var db = new BB_DB_DEVEntities2())
                {
                    quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == ProposalID).ToList();
                    opsImplement = db.BB_Proposal_OPSImplement.Where(x => x.ProposalID == ProposalID).ToList();
                    quotesRs = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == ProposalID).ToList();
                    opsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == ProposalID).ToList();
                    if (opsManage.Count > 0)
                        opsmanageTotal = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == ProposalID).Sum(x => x.UnitDiscountPrice * x.Quantity * x.TotalMonths);
                    if (quotesRs.Count > 0)
                        ServRecoTotal = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == ProposalID).Sum(x => x.TotalNetsale);


                    financiamento = db.BB_Proposal_Financing.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                    psconfigs = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                    vva1 = db.BB_Proposal_Vva.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
                    count = quotes.Count();
                    count += opsImplement.Count();
                    count += quotesRs.Count();
                    count += opsManage.Count();
                    tipoNEgocio = db.BB_Campanha.Where(x => x.ID == p.CampaignID).Select(x => x.Campanha).FirstOrDefault();
                    if (financiamento != null)
                    {
                        financiamentoMensal = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == ProposalID && x.FinancingID == financiamento.ID).ToList();
                        financiamentoTrimestreal = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == ProposalID && x.FinancingID == financiamento.ID).ToList();
                        FinancingType = db.BB_FinancingType.Where(x => x.Code == financiamento.FinancingTypeCode).FirstOrDefault();
                        aymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == financiamento.PaymentMethodId).FirstOrDefault();
                        contractType = db.BB_FinancingContractType.Where(x => x.ID == financiamento.ContractTypeId).FirstOrDefault();
                        prazodiferencido = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == ProposalID && x.IsAproved.Value == true).FirstOrDefault();
                        servicosIncluidos = financiamento.IncludeServices.Value == true ? " com serviços incluídos" : "";
                    }

                    ps2 = db.BB_Proposal_PrintingServices2
                        .Include(x => x.BB_PrintingServices.Select(y => y.BB_VVA))
                        .Include(x => x.BB_PrintingServices.Select(y => y.BB_PrintingServices_NoVolume))
                        .Where(x => x.ProposalID == ProposalID)
                        .FirstOrDefault();


                    //if (ps2 != null && ps2.BB_PrintingServices.Count > 0)
                    //{

                    //    active = ps2.BB_PrintingServices.ToList()[(int)ps2.ActivePrintingService - 1];
                    //    if (active != null)
                    //    {

                    //        if (active.BB_VVA != null)
                    //        {
                    //            vva = active.BB_VVA;
                    //            frequency = vva.RentBillingFrequency.GetValueOrDefault();

                    //            vva.PVP = vva.PVP * frequency;

                    //            valorServiço = active.Fee.GetValueOrDefault() + (vva.PVP != null ? vva.PVP : 0);
                    //        }
                    //        if (active.BB_PrintingServices_NoVolume != null)
                    //        {
                    //            nv = active.BB_PrintingServices_NoVolume;
                    //            frequency = active.BB_PrintingServices_NoVolume.PageBillingFrequency.GetValueOrDefault();
                    //        }
                    //    }
                    //}
                    vva = new BB_VVA();
                    ApprovedPrintingService activePS = null;


                    if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                    {
                        activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                        feeValor = activePS != null ? activePS.Fee : 0;
                        if (activePS != null && activePS.GlobalClickVVA != null)
                        {

                            vva.PVP = activePS.GlobalClickVVA.PVP;
                            valorServiço = activePS.Fee + (vva.PVP != null ? vva.PVP : 0);
                            switch (activePS.GlobalClickVVA.RentBillingFrequency)
                            {
                                case 3:
                                    activePS.BWVolume = activePS.BWVolume * 3;
                                    activePS.CVolume = activePS.CVolume * 3;
                                    vva.PVP = activePS.GlobalClickVVA.PVP * 3;
                                    break;
                                case 6:
                                    activePS.BWVolume = activePS.BWVolume * 6;
                                    activePS.CVolume = activePS.CVolume * 6;
                                    vva.PVP = activePS.GlobalClickVVA.PVP * 3;
                                    break;
                                default: break;
                            }
                        }

                    }
                }

                double? LeiDaCopiaPriada = 0;

                double totalsobrevalorizacao = 0;

                using (var db = new BB_DB_DEVEntities2())
                {
                    quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == ProposalID).ToList();
                    sobrevalorizacao = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == p.ID).ToList();

                    if (sobrevalorizacao != null)
                    {
                        totalsobrevalorizacao = sobrevalorizacao.Sum(x => x.Total).GetValueOrDefault();
                    }

                    foreach (var quote in quotes)
                    {
                        double? TCP = db.BB_Equipamentos.Where(x => x.CodeRef == quote.CodeRef).Select(x => x.TCP).FirstOrDefault();

                        var contador = db.BB_Proposal_Counters.Where(x => x.OSID == quote.ID).Count();

                        if (TCP is null)
                            TCP = 0;

                        if (quote.IsUsed.GetValueOrDefault() == false)
                            LeiDaCopiaPriada = (TCP * quote.Qty) + LeiDaCopiaPriada;
                    }

                }
                FileInfo newFile = new FileInfo(path);

                ExcelPackage pck = new ExcelPackage(newFile);
                //Add the Content sheet
                var ws = pck.Workbook.Worksheets["Ordem_Aquisicao"];
                var ws1 = pck.Workbook.Worksheets["Ordem de Aquisição_1"];
                var ws2 = pck.Workbook.Worksheets["Ordem de Aquisição_2"];
                var wsServico = pck.Workbook.Worksheets["Serviço"];
                var wsAutoR = pck.Workbook.Worksheets["AutoR"];
                var wsDeclaracao = pck.Workbook.Worksheets["Declaração"];
                var wsService2 = pck.Workbook.Worksheets["Service2"];


                StringBuilder sb = new StringBuilder();
                sb.AppendLine("1. Condições Gerais.");


                if (FinancingType != null && FinancingType.Code == 0)
                {
                    sb.AppendLine("2. Contrato de Compra e Venda");
                }

                if (FinancingType != null && (FinancingType.Code == 3 || FinancingType.Code == 5))
                {
                    sb.AppendLine("3. Contrato de Locação Mandatada");
                }


                if (ps2 != null)
                {
                    sb.AppendLine("4. Contrato de Prestação de Serviços de Cópia, Impressão, Digitalização e Manutenção");
                }



                foreach (var i in quotes)
                {
                    if (i.Family == "MCSPSV" || i.Family == "MCSMSV" || i.Family == "OPSPSV")
                    {
                        sb.AppendLine("5. Contrato de Prestação de Serviços Informáticos e Manutenção");
                        break;
                    }
                }

                foreach (var i in quotesRs)
                {
                    if (i.CodeRef.Contains("FORMPP") || i.Family == "MCSSW" || i.Family == "PPSW" || i.Family == "PRSSW" || i.Family.Contains("PSV") || i.Family.Contains("MSV") || i.Family == "OPSPSV")
                    {
                        sb.AppendLine("6. Contrato de Prestação de Serviços de Consultoria em Soluções de Otimização de Impressão");
                        break;
                    }
                }


                foreach (var i in quotes)
                {
                    if (i.CodeRef == "9960OD32006" || i.CodeRef == "SRTU PPD" || i.CodeRef == "SRTU AV")
                    {
                        sb.AppendLine("7. Contrato de Serviço de Recolha de Toners Usados/ Gestão de Embalagem Vazias de Toner");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.Family.Contains("IMSPSV") || i.Family.Contains("IMSMSV"))
                    {

                        sb.AppendLine("8. Contrato de Prestação de Serviços de Manutenção e Suporte de Infraestruturas de Tecnologias de Informação");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.CodeRef.Contains("FORMPP") || i.Family == "MCSSW" || i.Family == "PPSW" || i.Family == "PRSSW" || i.Family.Contains("PSV") || i.Family.Contains("MSV") || i.Family == "OPSPSV")
                    {
                        sb.AppendLine("9. Contrato de Prestação de Serviços Específico de Implementação, Manutenção e Suporte a Soluções");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.Family.Contains("PRSSW") || i.Family.Contains("MCSSW") || i.Family.Contains("IMSSW"))
                    {

                        sb.AppendLine("10. Contrato de Licenças de Software");
                        break;
                    }
                }

                foreach (var i in quotesRs)
                {
                    if (i.Family.Contains("PRSSW") || i.Family.Contains("MCSSW") || i.Family.Contains("IMSSW"))
                    {

                        sb.AppendLine("10. Contrato de Licenças de Software");
                        break;
                    }
                }

                foreach (var i in quotes)
                {
                    if (i.Family.Contains("CSV"))
                    {
                        sb.AppendLine("11. Contrato de Prestação de Serviços Cloud");
                        break;
                    }
                }

                foreach (var i in quotesRs)
                {
                    if (i.Family.Contains("CSV"))
                    {
                        sb.AppendLine("11. Contrato de Prestação de Serviços Cloud");
                        break;
                    }
                }
                foreach (var i in quotes)
                {
                    if (i.Description.Contains("Evolution"))
                    {
                        sb.AppendLine("12. Bizhub Evolution");
                        break;
                    }
                }





                //if (FinancingType.Code == 3)
                //    sb.AppendLine("1. Contrato de Locação Mandatada.");

                if (prazodiferencido != null && prazodiferencido.Alocadora == "GRENK")
                    prazodiferencido.Alocadora = "GRENKE";


                ws.Cells["A10"].Value = sb.ToString();

                int idxRow = 19;
                int nrItems = 1;
                int escreverPag1 = 8;
                int escreverPag2 = 8;
                foreach (var quote in quotes)
                {
                    if (nrItems <= 21)
                    {
                        ws.Cells["A" + idxRow.ToString()].Value = quote.Qty;
                        ws.Cells["B" + idxRow.ToString()].Value = quote.CodeRef;
                        ws.Cells["D" + idxRow.ToString()].Value = quote.Description;
                        idxRow++;
                    }
                    if (nrItems > 21 && nrItems <= 57)
                    {
                        ws1.Cells["A" + escreverPag1.ToString()].Value = quote.Qty;
                        ws1.Cells["B" + escreverPag1.ToString()].Value = quote.CodeRef;
                        ws1.Cells["D" + escreverPag1.ToString()].Value = quote.Description;
                        escreverPag1++;
                    }
                    if (nrItems > 57)
                    {
                        ws2.Cells["A" + escreverPag2.ToString()].Value = quote.Qty;
                        ws2.Cells["B" + escreverPag2.ToString()].Value = quote.CodeRef;
                        ws2.Cells["D" + escreverPag2.ToString()].Value = quote.Description;
                        escreverPag2++;
                    }
                    nrItems++;
                }
                foreach (var quote in opsImplement)
                {
                    if (nrItems <= 21)
                    {
                        ws.Cells["A" + idxRow.ToString()].Value = quote.Quantity;
                        ws.Cells["B" + idxRow.ToString()].Value = quote.CodeRef;
                        ws.Cells["D" + idxRow.ToString()].Value = quote.Name + " - " + quote.Description;
                        idxRow++;
                    }
                    if (nrItems > 21 && nrItems <= 57)
                    {
                        ws1.Cells["A" + escreverPag1.ToString()].Value = quote.Quantity;
                        ws1.Cells["B" + escreverPag1.ToString()].Value = quote.CodeRef;
                        ws1.Cells["D" + escreverPag1.ToString()].Value = quote.Name + " - " + quote.Description;
                        escreverPag1++;
                    }
                    if (nrItems > 57)
                    {
                        ws2.Cells["A" + escreverPag2.ToString()].Value = quote.Quantity;
                        ws2.Cells["B" + escreverPag2.ToString()].Value = quote.CodeRef;
                        ws2.Cells["D" + escreverPag2.ToString()].Value = quote.Name + " - " + quote.Description;
                        escreverPag2++;
                    }
                    nrItems++;
                }



                ws.Cells["K2"].Value = p.CRM_QUOTE_ID;
                ws1.Cells["K5"].Value = p.CRM_QUOTE_ID;
                ws2.Cells["K5"].Value = p.CRM_QUOTE_ID;
                wsServico.Cells["H4"].Value = p.CRM_QUOTE_ID;
                wsAutoR.Cells["K4"].Value = p.CRM_QUOTE_ID;
                wsDeclaracao.Cells["K6"].Value = p.CRM_QUOTE_ID;
                wsService2.Cells["H5"].Value = p.CRM_QUOTE_ID;


                ws.Cells["A42"].Value = "Produto Contratado:";

                if (FinancingType.Code.Value == 0)
                {
                    ws.Cells["A43"].Value = p.CampaignID == 0 ? "Venda Directa" : tipoNEgocio;
                }
                else
                {

                    if (FinancingType.Code == 2 && aymentMethod.ID == 2 && prazodiferencido != null && prazodiferencido.Alocadora == "BNP")
                    {
                        ws.Cells["A43"].Value = FinancingType.Type + " " + (aymentMethod != null ? aymentMethod.Type : "");
                    }
                    if (FinancingType.Code == 6 && aymentMethod.ID == 2 && prazodiferencido != null && prazodiferencido.Alocadora == "BNP")
                    {
                        ws.Cells["A43"].Value = FinancingType.Type + " com serviços incluídos" + " - FlexPage - " + (aymentMethod != null ? aymentMethod.Type : "");
                    }
                    else
                    {
                        ws.Cells["A43"].Value = FinancingType.Type + " - " + (aymentMethod != null ? aymentMethod.Type : "");
                    }
                }

                //TODO: QUERY para ir buscar o valor da aprovacao financeira
                double? ServicosRecorentesMes = 0;
                if (quotesRs.Count() > 0 || opsManage.Count() > 0)
                    ServicosRecorentesMes = (quotesRs != null ? quotesRs.Sum(x => x.MonthlyFee) : 0) + (opsManage != null ? opsManage.Sum(x => x.UnitDiscountPrice * x.Quantity) : 0);
                else
                    ServicosRecorentesMes = 0;


                ServicosRecorentesMes = ServicosRecorentesMes * ((frequency == 1 || frequency == 0) ? 1 : 3);

                //double? valorRenda = prazodiferencido != null ? prazodiferencido.ValorRenda + (financiamento != null && financiamento.IncludeServices == true && vva1 != null ? vva1.MonthlyFee : 0) : 0;
                double? valorRenda = (prazodiferencido != null ? prazodiferencido.ValorRenda : 0) + (vva != null && vva.PVP != null ? vva.PVP : 0) + (feeValor) + (prazodiferencido != null && (prazodiferencido.FinancingID == 3 || prazodiferencido.FinancingID == 4 || prazodiferencido.FinancingID == 5) ? ServicosRecorentesMes : 0);
                if (FinancingType.Code == 2 && servicosIncluidos != "" && aymentMethod.ID == 2 && prazodiferencido != null && prazodiferencido.Alocadora == "BNP")
                    valorRenda = 0;

                //if flexpage
                if (prazodiferencido != null && prazodiferencido.FinancingID == 6)
                {
                    valorRenda = (prazodiferencido != null ? prazodiferencido.ValorRenda : 0);
                }
                //double? servicosRecorrente = 0;
                //int? nrMEsesMedia = 0;
                //foreach (var i in quotesRs)
                //{
                //    servicosRecorrente += i.MonthlyFee;
                //    nrMEsesMedia += i.TotalMonths;
                //}


                if (prazodiferencido != null && (prazodiferencido.FinancingID == 1 || prazodiferencido.FinancingID == 2 || prazodiferencido.FinancingID == 6))
                {
                    ws.Cells["A44"].Value = "Período do Contrato:";
                    ws.Cells["D44"].Value = "NA";
                    ws.Cells["E44"].Value = "Perioricidade";
                    ws.Cells["F44"].Value = "NA";
                    ws.Cells["G44"].Value = "Valor da Renda";
                    ws.Cells["J44"].Value = "NA";
                }
                if (prazodiferencido != null && (prazodiferencido.FinancingID == 3 || prazodiferencido.FinancingID == 5))
                {
                    string frequencyString = "meses";
                    if (prazodiferencido.Frequency == 3)
                    {
                        frequencyString = "trimestres";
                    }
                    ws.Cells["A44"].Value = "Período do Contrato:";
                    ws.Cells["D44"].Value = prazodiferencido != null ? prazodiferencido.PrazoDiferenciado + " " + frequencyString : "NA";
                    ws.Cells["E44"].Value = "Perioricidade";
                    ws.Cells["F44"].Value = prazodiferencido != null && (prazodiferencido.Frequency == null || prazodiferencido.Frequency == 1) ? "Renda Mensal" : prazodiferencido.Frequency == 3 ? "Renda Trimestral" : "NA";
                    ws.Cells["G44"].Value = "Valor da Renda";
                    ws.Cells["J44"].Value = prazodiferencido != null ? valorRenda.Value != 0 ? valorRenda + "€" : "NA" : "NA";
                }

                if (financiamento.FinancingTypeCode == 0)
                {
                    ws.Cells["A44"].Value = "Período do Contrato:";
                    ws.Cells["D44"].Value = "NA";
                    ws.Cells["E44"].Value = "Perioricidade";
                    ws.Cells["F44"].Value = "NA";
                    ws.Cells["G44"].Value = "Valor da Renda";
                    ws.Cells["J44"].Value = "NA";
                }



                if (financiamento.FinancingTypeCode == 0 || (prazodiferencido != null && prazodiferencido.Alocadora == "GRENKE"))
                {
                    ws.Cells["K44"].Value = "";
                }
                else
                {
                    ws.Cells["K44"].Value = "Seguro Incluído";
                }


                double? serviçosrecorrentesTotais = 0;
                serviçosrecorrentesTotais = opsmanageTotal + ServRecoTotal;

                if (financiamento == null)
                {
                    ws.Cells["J40"].Value = "Preço";
                    ws.Cells["L40"].Value = p.ValueTotal - serviçosrecorrentesTotais + LeiDaCopiaPriada.Value + totalsobrevalorizacao;

                    ws1.Cells["E43"].Value = "Preço";
                    ws1.Cells["L43"].Value = p.ValueTotal - serviçosrecorrentesTotais + LeiDaCopiaPriada.Value;

                    ws2.Cells["E40"].Value = "Preço";
                    ws2.Cells["L40"].Value = p.ValueTotal - serviçosrecorrentesTotais + LeiDaCopiaPriada.Value + totalsobrevalorizacao;
                }

                if (financiamento != null && (financiamento.FinancingTypeCode == null || financiamento.FinancingTypeCode == 0))
                {
                    ws.Cells["J40"].Value = "Preço";
                    ws.Cells["L40"].Value = p.ValueTotal - serviçosrecorrentesTotais + LeiDaCopiaPriada.Value + totalsobrevalorizacao;

                    ws1.Cells["E43"].Value = "Preço";
                    ws1.Cells["L43"].Value = p.ValueTotal - serviçosrecorrentesTotais + LeiDaCopiaPriada.Value;

                    ws2.Cells["E40"].Value = "Preço";
                    ws2.Cells["L40"].Value = p.ValueTotal - serviçosrecorrentesTotais + LeiDaCopiaPriada.Value + totalsobrevalorizacao;
                }





                if (count > 21 && count <= 57)
                {
                    pck.Workbook.Worksheets.Delete(ws2);
                }
                if (count <= 21)
                {
                    pck.Workbook.Worksheets.Delete(ws2);
                    pck.Workbook.Worksheets.Delete(ws1);

                }


                //if(prazodiferencido != null  )
                //{
                //    wsDeclaracao.Cells["A33"] = "A Locadora está autorizada pelo Cliente, a transmitir a sua posição contratual no âmbito do presente contrato ao BNP PARIBAS LEASE GROUP, S.A.";
                //}

                ws.Cells["A47"].Value = "Não Aplicável";
                ////KOnica Representante
                //ws.Cells["A39"].Value = "Konica Minolta Business Solutions Portugal, Unipessoal, Lda";
                //ws.Cells["A40"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
                //ws.Cells["A41"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

                pck.Save();


                pck.Stream.Close();
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Contracto.xlsx");

            }
        }
        private readonly object balancelock = new object();
        [AcceptVerbs("GET", "POST")]
        [ActionName("PrintingProposal_V2")]
        public HttpResponseMessage PrintingProposal_V2(GerarProposta o)
        {
            //int ID = 375;
            //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
            string erro = "";
            //int proposalID = ID;
            int? proposalID = o.ProposalID;

            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

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
                string strFolder = "Proposal";
                string strCliente = c.DisplayName;
                path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente + "\\" + o.ProposalID.ToString();

                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);


                using (var stream = File.Open(@AppSettingsGet.DocumentPrintingTemplate, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                    try
                    {
                        using (var outputFile = new FileStream(path + "\\Proposal.xlsx", FileMode.Create))
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


                WriteCliente(@path + "\\Proposal.xlsx", p.ClientAccountNumber, p);
                //Cliente WRITE TO EXCEL
                //bool resultClient = WriteToExcelCliente(@path + "\\Proposal.xlsx", proposalID);
                WriteQuote_1(@path + "\\Proposal.xlsx", proposalID, p, o);
                //Thread.Sleep(3000);
                WritePropostaFinanceira(@path + "\\Proposal.xlsx", proposalID, p, o);

                WritePropostaServicos(@path + "\\Proposal.xlsx", proposalID, p);

                WritePropostaOutrasCondicoes(@path + "\\Proposal.xlsx", proposalID, p, o);



                ExportWorkbookToPdf(@path + "\\Proposal.xlsx", @path + "\\Proposal.pdf", proposalID);


                string filePath = @path + "\\Proposal.pdf";


                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: .");
                    throw new HttpResponseException(response);
                }

                //byte[] bytes = File.ReadAllBytes(filePath);
                ////Set the Response Content.
                //response.Content = new ByteArrayContent(bytes);

                ////Set the Response Content Length.
                //response.Content.Headers.ContentLength = bytes.LongLength;

                ////Set the Content Disposition Header Value and FileName.
                //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                //response.Content.Headers.ContentDisposition.FileName = "Proposal.pdf";

                ////Set the File Content Type.
                ////response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Proposal.pdf"));
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");


                //Set the Response Content.
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));

                //Set the Response Content Length.
                //response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Proposal.pdf";

                //Set the File Content Type.
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Proposal.pdf"));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                return response;

            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\Proposal.xlsx");
                erro = ex.Message.ToString();
            }
            Console.WriteLine(erro);
            return response;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ExportExcel")]
        public HttpResponseMessage ExportExcel(ProposalRootObject e)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            string path = "";
            try
            {
                BB_Proposal p = new BB_Proposal();
                AspNetUsers c = new AspNetUsers();
                //using (var db = new BB_DB_DEVEntities2())
                //{
                //    p = db.BB_Proposal.Where(x => x.ID == proposalID).First();

                //}
                using (var db = new masterEntities())
                {
                    c = db.AspNetUsers.Where(x => x.Email == e.Draft.details.CreatedBy).FirstOrDefault();

                }


                string strUser = c.DisplayName;
                string strFolder = "Configurador";
                string strCliente = c.DisplayName;
                path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\";

                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);


                using (var stream = File.Open(@AppSettingsGet.ConfiguracaoNegocio, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                    try
                    {
                        using (var outputFile = new FileStream(path + "ConfiguracaoNegocio.xlsx", FileMode.Create))
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
                WriteExportExcel(@path + "ConfiguracaoNegocio.xlsx", e);


                //Create HTTP Response.
                //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);


                string filePath = @path + "ConfiguracaoNegocio.xlsx";


                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: .");
                    throw new HttpResponseException(response);
                }

                //Set the Response Content.
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));

                //Set the Response Content Length.
                //response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "ConfiguracaoNegocio.xlsx";

                //Set the File Content Type.
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Proposal.pdf"));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xlsx");
            }
            catch (Exception ex)

            {
                File.Delete(@path + "\\ConfiguracaoNegocio.xlsx");
            }
            return response;


        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("ExportContrato")]
        public HttpResponseMessage ExportContrato(ProposalRootObject e)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            string path = "";
            try
            {
                BB_Proposal p = new BB_Proposal();
                AspNetUsers c = new AspNetUsers();
                //using (var db = new BB_DB_DEVEntities2())
                //{
                //    p = db.BB_Proposal.Where(x => x.ID == proposalID).First();

                //}
                using (var db = new masterEntities())
                {
                    c = db.AspNetUsers.Where(x => x.Email == e.Draft.details.CreatedBy).FirstOrDefault();

                }


                string strUser = c.DisplayName;
                string strFolder = "Configurador";
                string strCliente = c.DisplayName;
                path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\";

                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);


                using (var stream = File.Open(@AppSettingsGet.ConfiguracaoContrato, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {

                    try
                    {
                        using (var outputFile = new FileStream(path + "ConfiguracaoContrato.xlsx", FileMode.Create))
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
                WriteExportExcelOneShot(@path + "ConfiguracaoContrato.xlsx", e.Draft.details.ID);

                WriteExportExcelServicosRecorrentes(@path + "ConfiguracaoContrato.xlsx", e.Draft.details.ID);
                WriteExportExcelFinanciamento(@path + "ConfiguracaoContrato.xlsx", e.Draft.details.ID);

                //Create HTTP Response.
                //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);


                string filePath = @path + "ConfiguracaoContrato.xlsx";


                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found: .");
                    throw new HttpResponseException(response);
                }

                //Set the Response Content.
                response.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));

                //Set the Response Content Length.
                //response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "ConfiguracaoContrato.xlsx";

                //Set the File Content Type.
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Proposal.pdf"));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xlsx");
            }
            catch (Exception ex)

            {
                File.Delete(@path + "\\ConfiguracaoContrato.xlsx");
            }
            return response;


        }


        private void WriteExportExcelOneShot(string path, int? proposalID)
        {
            try
            {
                List<BB_Proposal_Quote> lstBB_Proposal_Quote = new List<BB_Proposal_Quote>();

                using (var db = new BB_DB_DEVEntities2())
                {

                    lstBB_Proposal_Quote = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalID).ToList();
                    //using (var db = new BB_DB_DEVEntities2())
                    //{
                    //    cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
                    //    lstBB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();
                    //}

                    FileInfo newFile = new FileInfo(path);

                    ExcelPackage pck = new ExcelPackage(newFile);
                    //Add the Content sheet
                    var ws = pck.Workbook.Worksheets["OneShot"];
                    //ws.View.ShowGridLines = false;
                    //ws.Cells["A1"].Value = "Funcionalidade indisponível temporariamente.";

                    BB_Proposal b = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    BB_Clientes cliente = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();


                    ws.Cells["A1"].Value = "Quote:"; ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A2"].Value = "Account Number:"; ws.Cells["A2"].Style.Font.Bold = true;
                    ws.Cells["A3"].Value = "Cliente:"; ws.Cells["A3"].Style.Font.Bold = true;
                    ws.Cells["A4"].Value = "NIF:"; ws.Cells["A4"].Style.Font.Bold = true;

                    ws.Cells["B1"].Value = b.CRM_QUOTE_ID;
                    ws.Cells["B2"].Value = b.ClientAccountNumber;
                    ws.Cells["B3"].Value = cliente.Name;
                    ws.Cells["B4"].Value = cliente.NIF;

                    int idx = 7;

                    ws.Cells["A" + idx].Value = "OneShot";
                    idx++;
                    ws.Cells["A" + idx].Value = "Family"; ws.Cells["A" + idx].Style.Font.Bold = true;
                    ws.Cells["B" + idx].Value = "CodeRef"; ws.Cells["B" + idx].Style.Font.Bold = true;
                    ws.Cells["C" + idx].Value = "Description"; ws.Cells["C" + idx].Style.Font.Bold = true;
                    ws.Cells["D" + idx].Value = "UnitPriceCost"; ws.Cells["D" + idx].Style.Font.Bold = true;
                    ws.Cells["E" + idx].Value = "Qty"; ws.Cells["E" + idx].Style.Font.Bold = true;
                    ws.Cells["F" + idx].Value = "TotalCost"; ws.Cells["F" + idx].Style.Font.Bold = true;
                    ws.Cells["G" + idx].Value = "Margin"; ws.Cells["G" + idx].Style.Font.Bold = true;
                    ws.Cells["H" + idx].Value = "PVP"; ws.Cells["H" + idx].Style.Font.Bold = true;
                    ws.Cells["I" + idx].Value = "TotalPVP"; ws.Cells["I" + idx].Style.Font.Bold = true;
                    ws.Cells["J" + idx].Value = "DiscountPercentage"; ws.Cells["J" + idx].Style.Font.Bold = true;
                    ws.Cells["K" + idx].Value = "UnitDiscountPrice"; ws.Cells["K" + idx].Style.Font.Bold = true;
                    ws.Cells["L" + idx].Value = "GPTotal"; ws.Cells["L" + idx].Style.Font.Bold = true;
                    ws.Cells["M" + idx].Value = "GPPercentage"; ws.Cells["M" + idx].Style.Font.Bold = true;
                    ws.Cells["N" + idx].Value = "TotalNetsale"; ws.Cells["N" + idx].Style.Font.Bold = true;

                    idx++;

                    foreach (var i in lstBB_Proposal_Quote)
                    {
                        double? copia = db.BB_Equipamentos.Where(x => x.CodeRef == i.CodeRef).Select(x => x.TCP).FirstOrDefault();

                        ws.Cells["A" + idx].Value = i.Family;
                        ws.Cells["B" + idx].Value = i.CodeRef;
                        ws.Cells["C" + idx].Value = i.Description;
                        ws.Cells["D" + idx].Value = i.UnitPriceCost.ToString();
                        ws.Cells["E" + idx].Value = i.Qty;
                        ws.Cells["F" + idx].Value = i.TotalCost.ToString();
                        ws.Cells["G" + idx].Value = i.Margin;
                        ws.Cells["H" + idx].Value = i.PVP;
                        ws.Cells["I" + idx].Value = i.TotalPVP;
                        ws.Cells["J" + idx].Value = i.DiscountPercentage;
                        ws.Cells["K" + idx].Value = i.UnitDiscountPrice;
                        ws.Cells["L" + idx].Value = i.GPTotal.ToString();
                        ws.Cells["M" + idx].Value = i.GPPercentage.ToString();
                        ws.Cells["N" + idx].Value = i.TotalNetsale;
                        idx++;
                    }

                    pck.Save();


                    pck.Stream.Close();
                }
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\ConfiguracaoNegocio.xlsx");

            }
        }

        private void WriteExportExcelServicosRecorrentes(string path, int? proposalID)
        {
            try
            {
                List<BB_Proposal_Quote_RS> lstBB_Proposal_Quote_RS = new List<BB_Proposal_Quote_RS>();

                using (var db = new BB_DB_DEVEntities2())
                {

                    lstBB_Proposal_Quote_RS = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).ToList();
                    //using (var db = new BB_DB_DEVEntities2())
                    //{
                    //    cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
                    //    lstBB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();
                    //}

                    FileInfo newFile = new FileInfo(path);

                    ExcelPackage pck = new ExcelPackage(newFile);
                    //Add the Content sheet
                    var ws = pck.Workbook.Worksheets["Servicios Recurrentes"];
                    //ws.View.ShowGridLines = false;
                    //ws.Cells["A1"].Value = "Funcionalidade indisponível temporariamente.";

                    BB_Proposal b = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    BB_Clientes cliente = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();


                    ws.Cells["A1"].Value = "Quote:"; ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A2"].Value = "Account Number:"; ws.Cells["A2"].Style.Font.Bold = true;
                    ws.Cells["A3"].Value = "Cliente:"; ws.Cells["A3"].Style.Font.Bold = true;
                    ws.Cells["A4"].Value = "NIF:"; ws.Cells["A4"].Style.Font.Bold = true;

                    ws.Cells["B1"].Value = b.CRM_QUOTE_ID;
                    ws.Cells["B2"].Value = b.ClientAccountNumber;
                    ws.Cells["B3"].Value = cliente.Name;
                    ws.Cells["B4"].Value = cliente.NIF;



                    int idx = 7;

                    ws.Cells["A" + idx].Value = "Servicios Recurrentes";
                    idx++;
                    ws.Cells["A" + idx].Value = "Family"; ws.Cells["A" + idx].Style.Font.Bold = true;
                    ws.Cells["B" + idx].Value = "CodeRef"; ws.Cells["B" + idx].Style.Font.Bold = true;
                    ws.Cells["C" + idx].Value = "Description"; ws.Cells["C" + idx].Style.Font.Bold = true;
                    ws.Cells["D" + idx].Value = "UnitPriceCost"; ws.Cells["D" + idx].Style.Font.Bold = true;
                    ws.Cells["E" + idx].Value = "Qty"; ws.Cells["E" + idx].Style.Font.Bold = true;
                    ws.Cells["F" + idx].Value = "TotalCost"; ws.Cells["F" + idx].Style.Font.Bold = true;
                    ws.Cells["G" + idx].Value = "Margin"; ws.Cells["G" + idx].Style.Font.Bold = true;
                    ws.Cells["H" + idx].Value = "PVP"; ws.Cells["H" + idx].Style.Font.Bold = true;
                    ws.Cells["I" + idx].Value = "TotalPVP"; ws.Cells["I" + idx].Style.Font.Bold = true;
                    ws.Cells["J" + idx].Value = "DiscountPercentage"; ws.Cells["J" + idx].Style.Font.Bold = true;
                    ws.Cells["K" + idx].Value = "UnitDiscountPrice"; ws.Cells["K" + idx].Style.Font.Bold = true;
                    ws.Cells["L" + idx].Value = "GPTotal"; ws.Cells["L" + idx].Style.Font.Bold = true;
                    ws.Cells["M" + idx].Value = "GPPercentage"; ws.Cells["M" + idx].Style.Font.Bold = true;
                    ws.Cells["N" + idx].Value = "TotalNetsale"; ws.Cells["N" + idx].Style.Font.Bold = true;
                    idx++;

                    foreach (var i in lstBB_Proposal_Quote_RS)
                    {
                        double? copia = db.BB_Equipamentos.Where(x => x.CodeRef == i.CodeRef).Select(x => x.TCP).FirstOrDefault();

                        ws.Cells["A" + idx].Value = i.Family;
                        ws.Cells["B" + idx].Value = i.CodeRef;
                        ws.Cells["C" + idx].Value = i.Description;
                        ws.Cells["D" + idx].Value = i.UnitPriceCost.ToString();
                        ws.Cells["E" + idx].Value = i.Qty;
                        ws.Cells["F" + idx].Value = i.TotalCost.ToString();
                        ws.Cells["G" + idx].Value = i.Margin;
                        ws.Cells["H" + idx].Value = i.PVP;
                        ws.Cells["I" + idx].Value = i.TotalPVP;
                        ws.Cells["J" + idx].Value = i.DiscountPercentage;
                        ws.Cells["K" + idx].Value = i.UnitDiscountPrice;
                        ws.Cells["L" + idx].Value = i.GPTotal.ToString();
                        ws.Cells["M" + idx].Value = i.GPPercentage.ToString();
                        ws.Cells["N" + idx].Value = i.TotalNetsale;
                        idx++;
                    }

                    pck.Save();


                    pck.Stream.Close();
                }
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\ConfiguracaoNegocio.xlsx");

            }
        }

        private void WriteExportExcelFinanciamento(string path, int? proposalID)
        {
            try
            {
                BB_Proposal_Financing finacing = new BB_Proposal_Financing();

                using (var db = new BB_DB_DEVEntities2())
                {

                    finacing = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    //using (var db = new BB_DB_DEVEntities2())
                    //{
                    //    cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
                    //    lstBB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();
                    //}

                    FileInfo newFile = new FileInfo(path);

                    ExcelPackage pck = new ExcelPackage(newFile);
                    //Add the Content sheet
                    var ws = pck.Workbook.Worksheets["Financiacion"];
                    //ws.View.ShowGridLines = false;
                    //ws.Cells["A1"].Value = "Funcionalidade indisponível temporariamente.";

                    BB_Proposal b = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    BB_Clientes cliente = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();


                    ws.Cells["A1"].Value = "Quote:"; ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A2"].Value = "Account Number:"; ws.Cells["A2"].Style.Font.Bold = true;
                    ws.Cells["A3"].Value = "Cliente:"; ws.Cells["A3"].Style.Font.Bold = true;
                    ws.Cells["A4"].Value = "NIF:"; ws.Cells["A4"].Style.Font.Bold = true;

                    ws.Cells["B1"].Value = b.CRM_QUOTE_ID;
                    ws.Cells["B2"].Value = b.ClientAccountNumber;
                    ws.Cells["B3"].Value = cliente.Name;
                    ws.Cells["B4"].Value = cliente.NIF;

                    int idx = 7;
                    BB_FinancingType FinancingType = db.BB_FinancingType.Where(x => x.ID == finacing.FinancingTypeCode).FirstOrDefault();
                    if (finacing != null)
                    {

                        BB_FinancingPaymentMethod FinancingPaymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == finacing.PaymentMethodId).FirstOrDefault();



                        ws.Cells["A" + idx].Value = "Financiacion";
                        idx++;
                        ws.Cells["A" + idx].Value = "Produto Financiero:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = FinancingType.Type.ToString();
                        idx++;

                        ws.Cells["A" + idx].Value = "Método de Pago:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = FinancingPaymentMethod.Type.ToString();
                        idx++;

                        ws.Cells["A" + idx].Value = "Pago para:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = finacing.PaymentAfter.ToString();
                        idx++;
                        idx++;
                        ws.Cells["A" + idx].Value = "Detalles de financiación";
                        idx++;

                        ws.Cells["A" + idx].Value = "Meses:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = finacing.Months.ToString();
                        idx++;

                        ws.Cells["A" + idx].Value = "Cuota Mensual:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = finacing.MonthlyIncome.ToString();
                        idx++;

                        //ws.Cells["A" + idx].Value = "Compañía Financeira:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        //ws.Cells["B" + idx].Value = finacing;
                        //idx++;

                        ws.Cells["A" + idx].Value = "Fecha de aprobación:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = finacing.DateApproval != null ? finacing.DateApproval.ToString() : "NA";
                        idx++;

                        ws.Cells["A" + idx].Value = "Factor:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = finacing.Factor != null ? finacing.Factor.ToString() : "NA";
                        idx++;
                    }

                    else
                    {
                        ws.Cells["A" + idx].Value = "Financiacion";
                        idx++;
                        ws.Cells["A" + idx].Value = "Produto Financiero:"; ws.Cells["A" + idx].Style.Font.Bold = true;
                        ws.Cells["B" + idx].Value = FinancingType.Type.ToString();
                    }
                    pck.Save();


                    pck.Stream.Close();
                }
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\ConfiguracaoNegocio.xlsx");

            }
        }

        private void WriteExportExcelServiciosPrinting(string path, int? proposalID)
        {
            ProposalBLL p1 = new ProposalBLL();
            LoadProposalInfo i = new LoadProposalInfo();
            i.ProposalId = proposalID.Value;
            ActionResponse a = p1.LoadProposal(i);

            try
            {
                BB_Proposal_Financing finacing = new BB_Proposal_Financing();

                using (var db = new BB_DB_DEVEntities2())
                {

                    finacing = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();
                    //using (var db = new BB_DB_DEVEntities2())
                    //{
                    //    cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
                    //    lstBB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();
                    //}

                    FileInfo newFile = new FileInfo(path);

                    ExcelPackage pck = new ExcelPackage(newFile);
                    //Add the Content sheet
                    var ws = pck.Workbook.Worksheets["Servicios Printing"];
                    //ws.View.ShowGridLines = false;
                    //ws.Cells["A1"].Value = "Funcionalidade indisponível temporariamente.";

                    BB_Proposal b = db.BB_Proposal.Where(x => x.ID == proposalID).FirstOrDefault();
                    BB_Clientes cliente = db.BB_Clientes.Where(x => x.accountnumber == b.ClientAccountNumber).FirstOrDefault();


                    ws.Cells["A1"].Value = "Quote:"; ws.Cells["A1"].Style.Font.Bold = true;
                    ws.Cells["A2"].Value = "Account Number:"; ws.Cells["A2"].Style.Font.Bold = true;
                    ws.Cells["A3"].Value = "Cliente:"; ws.Cells["A3"].Style.Font.Bold = true;
                    ws.Cells["A4"].Value = "NIF:"; ws.Cells["A4"].Style.Font.Bold = true;

                    ws.Cells["B1"].Value = b.CRM_QUOTE_ID;
                    ws.Cells["B2"].Value = b.ClientAccountNumber;
                    ws.Cells["B3"].Value = cliente.Name;
                    ws.Cells["B4"].Value = cliente.NIF;

                    
                    ApprovedPrintingService activePS = null;
                    ValoresTotaisRenda vt = new ValoresTotaisRenda();
                    string tipologiaPrintingService = "Haga click por modelo: no incluye volumen";
                    string duracaoContacto = "";
                    string frequenciaRenda = "Mensal";
                    string frequenciaRendaExcecisva = "Mensal";
                    if (a.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                    {
                        activePS = a.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[a.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                        if (activePS != null && activePS.GlobalClickVVA != null)
                        {
                            vt.VolumeBW = activePS.BWVolume;
                            vt.VolumeC = activePS.CVolume;
                            vt.VVA = activePS.GlobalClickVVA.PVP;
                            vt.clickPriceBW = activePS.GlobalClickVVA.BWExcessPVP;
                            vt.clickPriceC = activePS.GlobalClickVVA.CExcessPVP != null? activePS.GlobalClickVVA.CExcessPVP: 0;
                            duracaoContacto = activePS.ContractDuration.ToString();
                           
                            switch (activePS.GlobalClickVVA.RentBillingFrequency)
                            {
                                case 3:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 3;
                                    frequenciaRenda = "Trimestral";
                                    //fee = fee * 3;
                                    break;
                                case 6:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 6;
                                    frequenciaRenda = "Semestral";
                                    //fee = fee * 6;
                                    break;
                                default: break;
                            }

                            switch (activePS.GlobalClickVVA.ExcessBillingFrequency)
                            {
                                case 3:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 3;
                                    frequenciaRendaExcecisva = "Trimestral";
                                    //fee = fee * 3;
                                    break;
                                case 6:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 6;
                                    frequenciaRendaExcecisva = "Semestral";
                                    //fee = fee * 6;
                                    break;
                                default: break;
                            }

                            tipologiaPrintingService = "Haga click en Global: con volumen incluido (VVA)";
                        }
                        if (activePS != null && activePS.GlobalClickNoVolume != null)
                        {
                            vt.VolumeBW = activePS.BWVolume;
                            vt.VolumeC = activePS.CVolume;
                            duracaoContacto = activePS.ContractDuration.ToString();
                            vt.clickPriceBW = activePS.GlobalClickNoVolume.GlobalClickBW;
                            vt.clickPriceC = activePS.GlobalClickNoVolume.GlobalClickC != null ? activePS.GlobalClickNoVolume.GlobalClickC : 0;
                            switch (activePS.GlobalClickNoVolume.PageBillingFrequency)
                            {
                                case 3:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 3;
                                    frequenciaRenda = "Trimestral";
                                    //fee = fee * 3;
                                    break;
                                case 6:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 6;
                                    frequenciaRenda = "Semestral";
                                    //fee = fee * 6;
                                    break;
                                default: break;
                            }

                            switch (activePS.GlobalClickVVA.ExcessBillingFrequency)
                            {
                                case 3:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 3;
                                    frequenciaRendaExcecisva = "Trimestral";
                                    //fee = fee * 3;
                                    break;
                                case 6:
                                    //vt.RendaFinanciada = vt.RendaFinanciada * 6;
                                    frequenciaRendaExcecisva = "Semestral";
                                    //fee = fee * 6;
                                    break;
                                default: break;
                            }

                            tipologiaPrintingService = "Haga click en Global: no incluye volumen";
                        }

                    }


                    int idx = 7;
                    ws.Cells["A" + idx].Value = "Servicios Printing"; ws.Cells["A" + idx].Style.Font.Bold = true;
                    idx++;
                    ws.Cells["A" + idx].Value = "Cotizaciones aprobadas:";
                    ws.Cells["B" + idx].Value = tipologiaPrintingService;
                    idx++;
                    
                    ws.Cells["A" + idx].Value = "Duración del contrato:";
                    ws.Cells["B" + idx].Value = duracaoContacto;
                    idx++;


                    ws.Cells["A" + idx].Value = "Facturación de Renta Fija:";
                    ws.Cells["B" + idx].Value = frequenciaRenda;
                    idx++;

                    ws.Cells["A" + idx].Value = "Facturación excesiva de páginas:";
                    ws.Cells["B" + idx].Value = frequenciaRendaExcecisva;
                    idx++;

                    ws.Cells["A" + idx].Value = "Volumen Total Negro:";
                    ws.Cells["B" + idx].Value = vt.VolumeBW;
                    idx++;

                    ws.Cells["A" + idx].Value = "Volumen Total Color:";
                    ws.Cells["B" + idx].Value = vt.VolumeC;
                    idx++;

                    if(tipologiaPrintingService == "Haga click en Global: con volumen incluido (VVA)")
                    {
                        ws.Cells["A" + idx].Value = "VVA:";
                        ws.Cells["B" + idx].Value = vt.VVA;
                        idx++;

                        ws.Cells["A" + idx].Value = "Exceso negro:";
                        ws.Cells["B" + idx].Value = vt.clickPriceBW;
                        idx++;

                        ws.Cells["A" + idx].Value = "Exceso Color::";
                        ws.Cells["B" + idx].Value = vt.clickPriceC;
                        idx++;
                    }

                    if (tipologiaPrintingService == "Haga click en Global: no incluye volumen")
                    {
                        ws.Cells["A" + idx].Value = "VVA:";
                        ws.Cells["B" + idx].Value = vt.VVA;
                        idx++;

                        ws.Cells["A" + idx].Value = "Negro:";
                        ws.Cells["B" + idx].Value = vt.clickPriceBW;
                        idx++;

                        ws.Cells["A" + idx].Value = "Color:";
                        ws.Cells["B" + idx].Value = vt.clickPriceC;
                        idx++;
                    }


                    pck.Save();


                pck.Stream.Close();
            }
            }
            catch (Exception ex)
            {
                File.Delete(@path + "\\ConfiguracaoNegocio.xlsx");

            }
}

private void WriteExportExcel(string path, ProposalRootObject p)
{
    try
    {
        using (var db = new BB_DB_DEVEntities2())
        {
            BB_Clientes cliente = new BB_Clientes();

            //using (var db = new BB_DB_DEVEntities2())
            //{
            //    cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
            //    lstBB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();
            //}

            FileInfo newFile = new FileInfo(path);

            ExcelPackage pck = new ExcelPackage(newFile);
            //Add the Content sheet
            var ws = pck.Workbook.Worksheets["Sheet1"];
            //ws.View.ShowGridLines = false;
            //ws.Cells["A1"].Value = "Funcionalidade indisponível temporariamente.";

            ws.Cells["A1"].Value = "Quote";
            ws.Cells["A2"].Value = "Acocunt Number";
            ws.Cells["A3"].Value = "Cliente";
            ws.Cells["A4"].Value = "NIF";

            ws.Cells["B1"].Value = p.Draft.details.CRM_QUOTE_ID;
            ws.Cells["B2"].Value = p.Draft.client.accountnumber;
            ws.Cells["B3"].Value = p.Draft.client.Name;
            ws.Cells["B4"].Value = p.Draft.client.NIF;

            int idx = 6;

            ws.Cells["A" + idx].Value = "OneShot";
            idx++;
            ws.Cells["A" + idx].Value = "Family";
            ws.Cells["B" + idx].Value = "CodeRef";
            ws.Cells["C" + idx].Value = "Description";
            ws.Cells["D" + idx].Value = "UnitPriceCost";
            ws.Cells["E" + idx].Value = "Qty";
            ws.Cells["F" + idx].Value = "TotalCost";
            ws.Cells["G" + idx].Value = "Margin";
            ws.Cells["H" + idx].Value = "PVP";
            ws.Cells["I" + idx].Value = "TotalPVP";
            ws.Cells["J" + idx].Value = "DiscountPercentage";
            ws.Cells["K" + idx].Value = "UnitDiscountPrice";
            ws.Cells["L" + idx].Value = "GPTotal";
            ws.Cells["M" + idx].Value = "GPPercentage";
            ws.Cells["N" + idx].Value = "TotalNetsale";
            ws.Cells["N" + idx].Value = "Lei Copia Privada";
            idx++;

            bool isBusinessdeveloper = IsBusinessdeveloper(p.Draft.details.CreatedBy);

            foreach (var i in p.Draft.baskets.os_basket)
            {
                double? copia = db.BB_Equipamentos.Where(x => x.CodeRef == i.CodeRef).Select(x => x.TCP).FirstOrDefault();

                ws.Cells["A" + idx].Value = i.Family;
                ws.Cells["B" + idx].Value = i.CodeRef;
                ws.Cells["C" + idx].Value = i.Description;
                ws.Cells["D" + idx].Value = isBusinessdeveloper ? i.UnitPriceCost.ToString() : "-";
                ws.Cells["E" + idx].Value = i.Qty;
                ws.Cells["F" + idx].Value = isBusinessdeveloper ? i.TotalCost.ToString() : "-";
                ws.Cells["G" + idx].Value = i.Margin;
                ws.Cells["H" + idx].Value = i.PVP;
                ws.Cells["I" + idx].Value = i.TotalPVP;
                ws.Cells["J" + idx].Value = i.DiscountPercentage;
                ws.Cells["K" + idx].Value = i.UnitDiscountPrice;
                ws.Cells["L" + idx].Value = isBusinessdeveloper ? i.GPTotal.ToString() : "-";
                ws.Cells["M" + idx].Value = isBusinessdeveloper ? i.GPPercentage.ToString() : "-";
                ws.Cells["N" + idx].Value = i.TotalNetsale;
                ws.Cells["N" + idx].Value = i.IsUsed.GetValueOrDefault() ? 0 : copia.GetValueOrDefault();
                idx++;
            }
            idx++;
            idx++;
            ws.Cells["A" + idx].Value = "Serv. recorrentes";
            idx++;
            ws.Cells["A" + idx].Value = "Family";
            ws.Cells["B" + idx].Value = "CodeRef";
            ws.Cells["C" + idx].Value = "Description";
            ws.Cells["D" + idx].Value = "UnitPriceCost";
            ws.Cells["E" + idx].Value = "Qty";

            ws.Cells["F" + idx].Value = "TotalCost";
            ws.Cells["G" + idx].Value = "Margin";
            ws.Cells["H" + idx].Value = "PVP";
            ws.Cells["I" + idx].Value = "TotalPVP";
            ws.Cells["J" + idx].Value = "DiscountPercentage";
            ws.Cells["K" + idx].Value = "UnitDiscountPrice";
            ws.Cells["L" + idx].Value = "GPTotal";
            ws.Cells["M" + idx].Value = "GPPercentage";
            ws.Cells["N" + idx].Value = "TotalNetsale";
            ws.Cells["O" + idx].Value = "TotalMonths";
            idx++;

            foreach (var i in p.Draft.baskets.rs_basket)
            {
                ws.Cells["A" + idx].Value = i.Family;
                ws.Cells["B" + idx].Value = i.CodeRef;
                ws.Cells["C" + idx].Value = i.Description;
                ws.Cells["D" + idx].Value = i.UnitPriceCost;
                ws.Cells["E" + idx].Value = i.Qty;

                ws.Cells["F" + idx].Value = i.TotalCost;
                ws.Cells["G" + idx].Value = i.Margin;
                ws.Cells["H" + idx].Value = i.PVP;
                ws.Cells["I" + idx].Value = i.TotalPVP;
                ws.Cells["J" + idx].Value = i.DiscountPercentage;
                ws.Cells["K" + idx].Value = i.UnitDiscountPrice;
                ws.Cells["L" + idx].Value = i.GPTotal;
                ws.Cells["M" + idx].Value = i.GPPercentage;
                ws.Cells["N" + idx].Value = i.TotalNetsale;
                ws.Cells["O" + idx].Value = i.TotalMonths;
                idx++;
            }

            idx++;
            ws.Cells["A" + idx].Value = "OPS Implement";
            idx++;
            ws.Cells["A" + idx].Value = "Family";
            ws.Cells["B" + idx].Value = "CodeRef";
            ws.Cells["C" + idx].Value = "Description";
            ws.Cells["D" + idx].Value = "PVP";
            ws.Cells["E" + idx].Value = "Quantity";
            ws.Cells["F" + idx].Value = "Total Cost";
            //ws.Cells["G" + idx].Value = "1 - (i.UnitDiscountPrice / i.PVP)) * 100)";
            ws.Cells["G" + idx].Value = "UnitDiscountPrice";
            ws.Cells["H" + idx].Value = "Total Net Sale";

            idx++;
            if (p.Draft.opsPacks.opsImplement.Count() > 0)
            {
                foreach (var i in p.Draft.opsPacks.opsImplement)
                {
                    ws.Cells["A" + idx].Value = i.Family;
                    ws.Cells["B" + idx].Value = i.CodeRef;
                    ws.Cells["C" + idx].Value = i.Description;
                    ws.Cells["D" + idx].Value = i.PVP;
                    ws.Cells["E" + idx].Value = i.Quantity;
                    ws.Cells["F" + idx].Value = i.PVP * i.Quantity;
                    //ws.Cells["G" + idx].Value = ((1 - (i.UnitDiscountPrice / i.PVP)) * 100);
                    ws.Cells["G" + idx].Value = i.UnitDiscountPrice;
                    ws.Cells["H" + idx].Value = i.UnitDiscountPrice * i.Quantity;
                    idx++;
                }
            }
            else
            {
                idx++;
            }

            idx++;
            ws.Cells["A" + idx].Value = "OPS Manage";
            idx++;
            ws.Cells["A" + idx].Value = "Family";
            ws.Cells["B" + idx].Value = "CodeRef";
            ws.Cells["C" + idx].Value = "Description";
            ws.Cells["D" + idx].Value = "PVP";
            ws.Cells["E" + idx].Value = "Quantity";
            ws.Cells["F" + idx].Value = "TotalMonths";
            ws.Cells["G" + idx].Value = "PVP Total";
            //ws.Cells["H" + idx].Value = "1 - (i.UnitDiscountPrice / i.PVP)) * 100)";
            ws.Cells["H" + idx].Value = "UnitDiscountPrice";
            ws.Cells["I" + idx].Value = "Total Net Sale";

            idx++;
            if (p.Draft.opsPacks.opsManage.Count() > 0)
            {
                foreach (var i in p.Draft.opsPacks.opsManage)
                {
                    ws.Cells["A" + idx].Value = i.Family;
                    ws.Cells["B" + idx].Value = i.CodeRef;
                    ws.Cells["C" + idx].Value = i.Description;
                    ws.Cells["D" + idx].Value = i.PVP;
                    ws.Cells["E" + idx].Value = i.Quantity;
                    ws.Cells["F" + idx].Value = i.TotalMonths;
                    ws.Cells["G" + idx].Value = i.PVP * i.Quantity * i.TotalMonths;
                    //ws.Cells["H" + idx].Value = ((1 - (i.UnitDiscountPrice / i.PVP)) * 100);
                    ws.Cells["H" + idx].Value = i.UnitDiscountPrice;
                    ws.Cells["I" + idx].Value = i.UnitDiscountPrice * i.Quantity * i.TotalMonths;
                    idx++;
                }
            }
            else
            {
                idx++;
            }


            idx++;
            ws.Cells["A" + idx].Value = "Financiamento";
            idx++;
            ws.Cells["A" + idx].Value = "Mensal";
            ws.Cells["B" + idx].Value = "24";
            foreach (var i in p.Draft.financing.FinancingFactors.Monthly)
            {
                if (i.Contracto == 24)
                    ws.Cells["C" + idx].Value = i.Value;
            }
            idx++;
            ws.Cells["A" + idx].Value = "Mensal";
            ws.Cells["B" + idx].Value = "36";
            foreach (var i in p.Draft.financing.FinancingFactors.Monthly)
            {
                if (i.Contracto == 36)
                    ws.Cells["C" + idx].Value = i.Value;
            }
            idx++;
            ws.Cells["A" + idx].Value = "Mensal";
            ws.Cells["B" + idx].Value = "48";
            foreach (var i in p.Draft.financing.FinancingFactors.Monthly)
            {
                if (i.Contracto == 48)
                    ws.Cells["C" + idx].Value = i.Value;
            }
            idx++;
            ws.Cells["A" + idx].Value = "Mensal";
            ws.Cells["B" + idx].Value = "60";
            foreach (var i in p.Draft.financing.FinancingFactors.Monthly)
            {
                if (i.Contracto == 60)
                    ws.Cells["C" + idx].Value = i.Value;
            }

            idx++;
            ws.Cells["A" + idx].Value = "Trimestral";
            ws.Cells["B" + idx].Value = "8";
            foreach (var i in p.Draft.financing.FinancingFactors.Trimestral)
            {
                if (i.Contracto == 8)
                    ws.Cells["C" + idx].Value = i.Value;
            }
            idx++;
            ws.Cells["A" + idx].Value = "Trimestral";
            ws.Cells["B" + idx].Value = "12";
            foreach (var i in p.Draft.financing.FinancingFactors.Trimestral)
            {
                if (i.Contracto == 12)
                    ws.Cells["C" + idx].Value = i.Value;
            }
            idx++;
            ws.Cells["A" + idx].Value = "Trimestral";
            ws.Cells["B" + idx].Value = "16";
            foreach (var i in p.Draft.financing.FinancingFactors.Trimestral)
            {
                if (i.Contracto == 16)
                    ws.Cells["C" + idx].Value = i.Value;
            }
            idx++;
            ws.Cells["A" + idx].Value = "Trimestral";
            ws.Cells["B" + idx].Value = "20";
            foreach (var i in p.Draft.financing.FinancingFactors.Trimestral)
            {
                if (i.Contracto == 20)
                    ws.Cells["C" + idx].Value = i.Value;
            }

            //KOnica Representante
            //ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
            //ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

            pck.Save();


            pck.Stream.Close();
        }
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\ConfiguracaoNegocio.xlsx");

    }
}

private bool IsBusinessdeveloper(string v)
{
    if (v == "Joao.Reis@konicaminolta.pt"
        || v == "Fernando.Rodrigues@konicaminolta.pt"
        || v == "Vitor.Medeiros@konicaminolta.pt"
         || v == "Catarina.polaco@konicaminolta.pt"
           || v == "maria.moraisribeiro@konicaminolta.pt"
          || v == "Marta.Sousa@konicaminolta.pt")
    {
        return true;
    }
    else
    {
        return false;
    }
}

[AcceptVerbs("GET", "POST")]
[ActionName("PrintingContracto")]
public HttpResponseMessage PrintingContracto(GerarContrato gc)
{
    //int ID = 375;

    //GerarContrato gc = new GerarContrato();
    //gc.proposalID = 472;
    //gc.Selectmonthly = 18;

    //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
    string erro = "";
    //int proposalID = ID;
    //int? proposalID = o.ProposalID;
    //Create HTTP Response.
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

    string path = "";
    try
    {
        BB_Proposal p = new BB_Proposal();
        AspNetUsers c = new AspNetUsers();
        using (var db = new BB_DB_DEVEntities2())
        {
            p = db.BB_Proposal.Where(x => x.ID == gc.proposalID).First();

        }
        using (var db = new masterEntities())
        {
            c = db.AspNetUsers.Where(x => x.Email == p.CreatedBy).FirstOrDefault();

        }


        string strUser = c.DisplayName;
        string strFolder = "Contracto";
        string strCliente = c.DisplayName;
        path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

        if (!Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);


        using (var stream = File.Open(@AppSettingsGet.DocumentPrintingContractoTemplate, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            try
            {
                using (var outputFile = new FileStream(path + "\\Contracto.xlsx", FileMode.Create))
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

        WriteContractoCliente(@path + "\\Contracto.xlsx", p.ClientAccountNumber, gc.proposalID);

        //Cliente WRITE TO EXCEL
        //bool resultClient = WriteToExcelCliente(@path + "\\Proposal.xlsx", proposalID);
        WriteContractoQuote(@path + "\\Contracto.xlsx", gc.proposalID, p);
        //Thread.Sleep(3000);
        //WritePropostaFinanceira(@path + "\\Contracto.xlsx", proposalID, p);

        WriteContractoServicos(@path + "\\Contracto.xlsx", gc.proposalID, p);

        WriteContractoService2(@path + "\\Contracto.xlsx", p.ClientAccountNumber, gc.proposalID);
        //WritePropostaOutrasCondicoes(@path + "\\Contracto.xlsx", proposalID, p);

        WriteContractoAutoR(@path + "\\Contracto.xlsx", p.ClientAccountNumber);

        WriteContractoDeclaracao(@path + "\\Contracto.xlsx", p.ClientAccountNumber, p.ID);


        ExportWorkbookToPdfContrato(@path + "\\Contracto.xlsx", @path + "\\Contracto.pdf", gc.proposalID);


        string filePath = @path + "\\Contracto.pdf";
        string excelPath = @path + "\\Contracto.xlsx";

        if (gc.TipoFicheiro == "PDF")
        {
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: .");
                throw new HttpResponseException(response);
            }

            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.

            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Contracto.pdf";

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contracto.pdf"));
        }
        if (gc.TipoFicheiro == "EXCEL")
        {
            if (!File.Exists(excelPath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: .");
                throw new HttpResponseException(response);
            }

            byte[] bytes = File.ReadAllBytes(excelPath);

            //Set the Response Content.

            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Contrato.xlsx";

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contrato.xlsx"));
        }


        return response;

    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");
        erro = ex.Message.ToString();
    }
    Console.WriteLine(erro);
    return response;
}

[AcceptVerbs("GET", "POST")]
[ActionName("PrintingContractoFormal")]
public HttpResponseMessage PrintingContractoFormal(GerarContratoFormal gc)
{
    //int ID = 375;

    //GerarContrato gc = new GerarContrato();
    //gc.proposalID = 472;
    gc.Selectmonthly = 18;

    //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
    string erro = "";
    //int proposalID = ID;
    //int? proposalID = o.ProposalID;
    //Create HTTP Response.
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
    List<BB_Proposal_Quote> quotes = new List<BB_Proposal_Quote>();
    string path = "";
    List<DeliveryLocation> deliveryLocations;
    try
    {
        BB_Proposal p = new BB_Proposal();
        AspNetUsers c = new AspNetUsers();
        using (var db = new BB_DB_DEVEntities2())
        {
            p = db.BB_Proposal.Where(x => x.ID == gc.proposalID).First();
            quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == gc.proposalID).ToList();

            deliveryLocations = new List<DeliveryLocation>();
            List<BB_Proposal_DeliveryLocation> bb_Proposal_DeliveryLocation = db.BB_Proposal_DeliveryLocation.Where(x => x.ProposalID == gc.proposalID).ToList();
            foreach (var item in bb_Proposal_DeliveryLocation)
            {
                var configDelivery = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<BB_Proposal_DeliveryLocation, DeliveryLocation>();
                });

                IMapper iMapperDelivery = configDelivery.CreateMapper();

                DeliveryLocation dl = iMapperDelivery.Map<BB_Proposal_DeliveryLocation, DeliveryLocation>(item);



                List<BB_Proposal_ItemDoBasket> itemDoBasket = new List<BB_Proposal_ItemDoBasket>();
                itemDoBasket = db.BB_Proposal_ItemDoBasket.Where(x => x.DeliveryLocationID == item.IDX).ToList();

                dl.items = new List<ItemDoBasket>();
                foreach (var item1 in itemDoBasket)
                {
                    var configItem = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_ItemDoBasket, ItemDoBasket>();
                    });

                    IMapper iMapperItem = configItem.CreateMapper();

                    ItemDoBasket i = iMapperItem.Map<BB_Proposal_ItemDoBasket, ItemDoBasket>(item1);
                    i.psConfig = new PsConfig();
                    i.counters = new List<Counter>();
                    dl.items.Add(i);
                }

                deliveryLocations.Add(dl);
            }


        }
        using (var db = new masterEntities())
        {
            c = db.AspNetUsers.Where(x => x.Email == p.CreatedBy).FirstOrDefault();

        }







        string strUser = c.DisplayName;
        string strFolder = "Contracto";
        string strCliente = c.DisplayName;
        path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente + "\\" + p.ID;

        if (!Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);


        using (var stream = File.Open(@AppSettingsGet.DocumentPrintingContractoWordTemplate, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            try
            {
                using (var outputFile = new FileStream(path + "\\ContractoFormal.doc", FileMode.Create))
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


        WriteContractoFormal(@path + "\\ContractoFormal.doc", p.ClientAccountNumber, gc, quotes, deliveryLocations);
        //WriteContractoFormalEmail(@path + "\\ContractoFormal.doc", p.ClientAccountNumber, gc.ENDEREÇODEMAIL);

        //ExportWorkbookToPdf(@path + "\\Contracto.xlsx", @path + "\\Contracto.pdf");


        string filePath = @path + "\\ContractoFormal.doc";


        //Check whether File exists.
        if (!File.Exists(filePath))
        {
            //Throw 404 (Not Found) exception if File not found.
            response.StatusCode = HttpStatusCode.NotFound;
            response.ReasonPhrase = string.Format("File not found: .");
            throw new HttpResponseException(response);
        }

        byte[] bytes = File.ReadAllBytes(filePath);

        //Set the Response Content.

        response.Content = new ByteArrayContent(bytes);

        //Set the Response Content Length.
        response.Content.Headers.ContentLength = bytes.LongLength;

        //Set the Content Disposition Header Value and FileName.
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        response.Content.Headers.ContentDisposition.FileName = "Contracto.doc";

        //Set the File Content Type.
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contracto.doc"));
        return response;

    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.doc");
        erro = ex.Message.ToString();
    }
    Console.WriteLine(erro);
    return response;
}
private void WriteContractoFormal(string path, string clientAccountNumber, GerarContratoFormal gc, List<BB_Proposal_Quote> qotes, List<DeliveryLocation> deliveryLocations)
{
    try
    {
        Microsoft.Office.Interop.Word.Application wordApp = null;
        wordApp = new Microsoft.Office.Interop.Word.Application();
        wordApp.Visible = false;
        //wordApp.AutomationSecurity 
        wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;

        Document wordDoc = wordApp.Documents.Open(@path);

        wordDoc.Activate();


        FindAndReplace(wordApp, "[Cliente]", gc.Cliente);
        FindAndReplace(wordApp, "[quotas/anónima]", gc.quotasAnonimas != null && gc.quotasAnonimas.Length > 0 ? gc.quotasAnonimas : "[quotas/anónima]");
        FindAndReplace(wordApp, "[morada]", gc.Morada);
        FindAndReplace(wordApp, "[número]", gc.NIF);
        FindAndReplace(wordApp, "[nome do legal representante ou procurador]", gc.LegalRepresentante);
        FindAndReplace(wordApp, "[qualidade em que actua e que pode ser a de gerente, administrador ou procurador]", gc.QualidadeRepresentante);
        FindAndReplace(wordApp, "[ABREVIATURA CLIENTE]", gc.AbreviaturaCliente);
        FindAndReplace(wordApp, "[número de meses]", gc.NrMesesContrato);// -  presente contrato entra em vigor na data de entrega/instalação do(s) 
        FindAndReplace(wordApp, "[cap] - capital social", gc.CapitalSocial);
        FindAndReplace(wordApp, "[N.º de páginas]", gc.NrPaginasPreto);// - equipamentos, e serviços prestados, que incluem a realização mensal das primeira páginas a preto
        FindAndReplace(wordApp, "[valorN]", gc.ValorPaginaPreto);// - preço das paginas a preto
        FindAndReplace(wordApp, "[valor por extensoN]", gc.ValorExtensoPaginaPreto);// - valor por extenso
        FindAndReplace(wordApp, "[valorE]", gc.ValorExtensoPaginaPreto); //- valor excendente
        FindAndReplace(wordApp, "[valor por extensoE]", gc.ValorExtensoPaginaPretoExcedente);// - valor por extenso excednete
        FindAndReplace(wordApp, "[DESIGNAÇÃO COMERCIAL DO CLIENTE]", gc.DesignacaoComercialCliente);// -  designacao comercial do cliente
        FindAndReplace(wordApp, "[ENDEREÇO DE MAIL]", gc.ENDEREÇODEMAIL);// -  designacao comercial do cliente





        object oMissing = System.Reflection.Missing.Value;
        object oEndOfDoc = "\\Tabela1";
        //Table newTable;
        //Range wrdRng = wordDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

        Table newTable = wordDoc.Tables[2];
        Range wrdRng = newTable.Range;

        //newTable = wordDoc.Tables.Add(wrdRng, qotes.Count, 3);
        newTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
        newTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
        newTable.AllowAutoFit = true;


        newTable.Cell(1, 1).Range.Text = "coderef";
        newTable.Cell(1, 2).Range.Text = "Description";
        newTable.Cell(1, 3).Range.Text = "Quantidade";
        newTable.Rows.Add();

        int idx = 2;
        foreach (var i in qotes)
        {

            newTable.Cell(idx, 1).Range.Text = i.CodeRef;
            newTable.Cell(idx, 2).Range.Text = i.Description;
            newTable.Cell(idx, 3).Range.Text = i.Qty.ToString();
            newTable.Rows.Add();
            idx++;
        }

        Table newTable1 = wordDoc.Tables[3];
        Range wrdRng1 = newTable1.Range;

        //newTable = wordDoc.Tables.Add(wrdRng, qotes.Count, 3);
        newTable1.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
        newTable1.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
        newTable1.AllowAutoFit = true;


        newTable1.Cell(1, 1).Range.Text = "coderef";
        newTable1.Cell(1, 2).Range.Text = "Description";
        newTable1.Cell(1, 3).Range.Text = "Morada";
        newTable1.Cell(1, 4).Range.Text = "Codigo Postal";
        newTable1.Cell(1, 5).Range.Text = "Cidade";
        newTable1.Cell(1, 6).Range.Text = "Contacto";

        newTable1.Rows.Add();

        idx = 2;
        foreach (var item in deliveryLocations)
        {

            foreach (var i in item.items)
            {
                newTable1.Cell(idx, 1).Range.Text = i.CodeRef;
                newTable1.Cell(idx, 2).Range.Text = i.Description;
                newTable1.Cell(idx, 3).Range.Text = item.Adress1 + "," + item.Adress2;
                newTable1.Cell(idx, 4).Range.Text = item.PostalCode;
                newTable1.Cell(idx, 5).Range.Text = item.City;
                newTable1.Cell(idx, 6).Range.Text = item.Contacto;
            }

            newTable1.Rows.Add();
            idx++;
        }


        wordDoc.Save();

        wordDoc.Close();
        Marshal.ReleaseComObject(wordApp);
        wordApp = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();


    }
    catch (Exception ex)
    {
        ex.Message.ToString();
    }
}
public HttpResponseMessage DownloadContrato(int? contratoID)
{
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
    LD_Contrato p = new LD_Contrato();
    LD_DocSign_Control_Files dsf = new LD_DocSign_Control_Files();
    using (var db = new BB_DB_DEV_LeaseDesk())
    {
        p = db.LD_Contrato.Where(x => x.ID == contratoID).First();

        dsf = db.LD_DocSign_Control_Files.Where(x => x.ProposalID == p.ProposalID).OrderBy(x => x.OrderFile).FirstOrDefault();
    }

    if (!File.Exists(dsf.FilePath))
    {
        //Throw 404 (Not Found) exception if File not found.
        response.StatusCode = HttpStatusCode.NotFound;
        response.ReasonPhrase = string.Format("File not found: .");
        throw new HttpResponseException(response);
    }

    string filename = Path.GetFileName(dsf.FilePath);
    byte[] bytes = File.ReadAllBytes(dsf.FilePath);

    //Set the Response Content.

    response.Content = new ByteArrayContent(bytes);

    //Set the Response Content Length.
    response.Content.Headers.ContentLength = bytes.LongLength;

    //Set the Content Disposition Header Value and FileName.
    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
    response.Content.Headers.ContentDisposition.FileName = filename;

    //Set the File Content Type.
    response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
    return response;
}

public void WriteContractoServicos(string path, int? ProposalID, BB_Proposal p)
{
    try
    {

        ProposalBLL p1 = new ProposalBLL();
        LoadProposalInfo ii = new LoadProposalInfo();
        ii.ProposalId = ProposalID.GetValueOrDefault();
        ActionResponse loadProposal = p1.LoadProposal(ii);

        List<BB_Proposal_Quote> quotes = new List<BB_Proposal_Quote>();
        BB_Proposal_Overvaluation sobrevalorizacao = new BB_Proposal_Overvaluation();
        List<BB_Equipamentos> lstEquipamentos = new List<BB_Equipamentos>();

        List<BB_Proposal_Counters> lstContadores = new List<BB_Proposal_Counters>();

        BB_PrintingServices active = null;
        BB_VVA vva = null;
        GlobalClickVVA vva1 = null;
        GlobalClickNoVolume nv1 = null;
        BB_PrintingServices_NoVolume nv = null;

        Nullable<int> BWVolume = 0;
        Nullable<int> CVolume = 0;

        BB_Proposal_Financing f = new BB_Proposal_Financing();
        FileInfo newFile = new FileInfo(path);

        ExcelPackage pck = new ExcelPackage(newFile);
        //Add the Content sheet
        var ws = pck.Workbook.Worksheets["Serviço"];
        int frquency = 1;


        using (var db = new BB_DB_DEVEntities2())
        {
            BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2
                .Include(x => x.BB_PrintingServices.Select(y => y.BB_VVA))
                .Include(x => x.BB_PrintingServices.Select(y => y.BB_PrintingServices_NoVolume))
                .Where(x => x.ProposalID == ProposalID)
                .FirstOrDefault();

            ApprovedPrintingService activePS = null;
            if (ps2 != null && ps2.BB_PrintingServices.Count > 0)
            {

                //active = ps2.BB_PrintingServices.ToList()[(int)ps2.ActivePrintingService - 1];

                if (loadProposal.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    activePS = loadProposal.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[loadProposal.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                    if (activePS != null)
                    {
                        BWVolume = activePS.BWVolume;
                        CVolume = activePS.CVolume;
                        if (activePS.GlobalClickVVA != null)
                        {
                            //vva = active.BB_VVA;
                            vva1 = activePS.GlobalClickVVA;
                            frquency = vva1.RentBillingFrequency;
                        }
                        if (activePS.GlobalClickNoVolume != null)
                        {
                            //nv = active.BB_PrintingServices_NoVolume;
                            nv1 = activePS.GlobalClickNoVolume;
                            frquency = nv1.PageBillingFrequency;
                        }
                    }
                }
                quotes = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == ProposalID).ToList();
                lstContadores = db.BB_Proposal_Counters.Where(x => x.ProposalID == ProposalID).ToList();
                lstEquipamentos = db.BB_Equipamentos.ToList();
                f = db.BB_Proposal_Financing.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
            }

            if (activePS == null)
            {
                pck.Workbook.Worksheets.Delete(ws);
            }
            else
            {
                int billingFrequency = 0;
                ws.Cells["F22"].Value = BWVolume != 0 ? BWVolume.ToString() : "";
                ws.Cells["G22"].Value = CVolume != 0 ? CVolume.ToString() : "";

                ws.Cells["H4"].Value = p.CRM_QUOTE_ID;
                string strExcende = " ";
                if (vva1 != null)
                {
                    billingFrequency = vva1.ExcessBillingFrequency;
                    ws.Cells["F9"].Value = "Pagínas";
                    ws.Cells["F10"].Value = "Incluidas";
                    ws.Cells["F11"].Value = "Preto";

                    ws.Cells["G9"].Value = "Pagínas";
                    ws.Cells["G10"].Value = "Incluidas";
                    ws.Cells["G11"].Value = "Cores";

                    ws.Cells["F22"].Value = BWVolume.Value * ((frquency == 0 || frquency == 1) ? 1 : 3);
                    ws.Cells["G22"].Value = CVolume.Value * ((frquency == 0 || frquency == 1) ? 1 : 3); ;

                    ws.Cells["H8"].Value = "Páginas Excedentes";
                    strExcende = " excendentes ";
                    List<BB_Proposal_Quote> q = quotes.Where(x => x.TCP != null).ToList();
                    int c = 12;
                    foreach (var item in q)
                    {
                        BB_Equipamentos equi = lstEquipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                        if (equi != null)
                        {
                            List<BB_Proposal_Counters> counter = lstContadores.Where(x => x.OSID == item.ID).ToList();
                            if (counter.Count > 0)
                            {
                                ws.Cells["C10"].Value = "Contadores Iniciais";
                                ws.Cells["C11"].Value = "Preto";
                                ws.Cells["D11"].Value = "Cor";
                                foreach (var i in counter)
                                {
                                    ws.Cells["A" + c].Value = equi.Name + "-" + i.serialNumber;
                                    ws.Cells["C" + c].Value = i.bwCounter;
                                    ws.Cells["D" + c].Value = i.cCounter;
                                    ws.Cells["H" + c].Value = vva1.BWExcessPVP != 0 ? vva1.BWExcessPVP.ToString() : "";
                                    ws.Cells["I" + c].Value = vva1.CExcessPVP != 0 ? vva1.CExcessPVP.ToString() : "";
                                    c++;
                                }
                            }
                            else
                            {
                                ws.Cells["A" + c].Value = equi.Name;
                                ws.Cells["H" + c].Value = vva1.BWExcessPVP != 0 ? vva1.BWExcessPVP.ToString() : "";
                                ws.Cells["I" + c].Value = vva1.CExcessPVP != 0 ? vva1.CExcessPVP.ToString() : "";
                                c++;
                            }
                        }
                    }
                    if (f != null && f.IncludeServices.Value == false)
                    {
                        if (f.FinancingTypeCode != 3 || f.FinancingTypeCode != 6)
                        {
                            ws.Cells["E9"].Value = "Valor";
                            ws.Cells["E10"].Value = "Taxa Fixa";
                            switch (vva1.RentBillingFrequency)
                            {
                                case 1: ws.Cells["E11"].Value = "Mensal"; break;
                                case 3: ws.Cells["E11"].Value = "Trimestral"; break;
                                case 6: ws.Cells["E11"].Value = "Semestral"; break;
                                default: ws.Cells["E11"].Value = "Mensal"; break;
                            }

                            double vvaRenda = vva1.PVP * vva1.RentBillingFrequency;
                            ws.Cells["E22"].Value = vva1.PVP != 0 ? vvaRenda.ToString() : "";
                        }
                    }
                    else
                    {
                        ws.Cells["E9"].Value = "";
                        ws.Cells["E10"].Value = "";
                        ws.Cells["E11"].Value = "";
                        ws.Cells["E22"].Value = "";
                    }
                }

                if (nv1 != null)
                {
                    billingFrequency = nv1.PageBillingFrequency;
                    ws.Cells["E22"].Value = "";
                    ws.Cells["F22"].Value = "";
                    ws.Cells["G22"].Value = "";
                    ws.Cells["H8"].Value = "";

                    List<BB_Proposal_Quote> q = quotes.Where(x => x.TCP != null).ToList();
                    int idx = 12;
                    foreach (var item in q)
                    {
                        BB_Equipamentos equi = lstEquipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                        if (equi != null)
                        {
                            List<BB_Proposal_Counters> counter = lstContadores.Where(x => x.OSID == item.ID).ToList();
                            if (counter.Count > 0)
                            {
                                ws.Cells["C10"].Value = "Contadores Iniciais";
                                ws.Cells["C11"].Value = "Preto";
                                ws.Cells["D11"].Value = "Cor";
                                foreach (var i in counter)
                                {
                                    ws.Cells["A" + idx].Value = equi.Name + "-" + i.serialNumber;
                                    ws.Cells["C" + idx].Value = i.bwCounter;
                                    ws.Cells["D" + idx].Value = i.cCounter;
                                    ws.Cells["H" + idx].Value = nv1.GlobalClickBW != 0 ? Math.Round(nv1.GlobalClickBW, 5) + "€" : "";
                                    ws.Cells["I" + idx].Value = nv1.GlobalClickC != 0 ? Math.Round(nv1.GlobalClickC, 5) + "€" : "";
                                    idx++;
                                }
                            }
                            else
                            {
                                ws.Cells["A" + idx].Value = equi.Name;
                                ws.Cells["H" + idx].Value = nv1.GlobalClickBW != 0 ? Math.Round(nv1.GlobalClickBW, 5) + "€" : "";
                                ws.Cells["I" + idx].Value = nv1.GlobalClickC != 0 ? Math.Round(nv1.GlobalClickC, 5) + "€" : "";
                                idx++;
                            }
                        }
                    }
                    if (f != null && f.IncludeServices.Value == false)
                    {
                        if (f.FinancingTypeCode != 3 || f.FinancingTypeCode != 6)
                        {
                            ws.Cells["E9"].Value = "Valor";
                            ws.Cells["E10"].Value = "Taxa Fixa";
                            switch (nv1.PageBillingFrequency)
                            {
                                case 1: ws.Cells["E11"].Value = "Mensal"; break;
                                case 3: ws.Cells["E11"].Value = "Trimestral"; break;
                                case 6: ws.Cells["E11"].Value = "Semestral"; break;
                                default: ws.Cells["E11"].Value = "Mensal"; break;
                            }
                        }
                    }
                    else
                    {
                        ws.Cells["E9"].Value = "";
                        ws.Cells["E10"].Value = "";
                        ws.Cells["E11"].Value = "";
                        ws.Cells["E22"].Value = "";
                    }
                }

                if (vva1 == null && nv1 == null && activePS.ClickPerModel != null)
                {
                    billingFrequency = activePS.ClickPerModel.PageBillingFrequency;
                    ws.Cells["E22"].Value = "";
                    ws.Cells["F22"].Value = "";
                    ws.Cells["G22"].Value = "";
                    ws.Cells["H8"].Value = "";

                    List<Machine> q = activePS.Machines.ToList();
                    int idx = 12;
                    foreach (var item in q)
                    {
                        BB_Equipamentos equi = lstEquipamentos.Where(x => x.CodeRef == item.CodeRef).FirstOrDefault();
                        if (equi != null)
                        {
                            List<BB_Proposal_Counters> counter = lstContadores.Where(x => x.OSID == item.ID).ToList();
                            if (counter.Count > 0)
                            {
                                ws.Cells["C10"].Value = "Contadores Iniciais";
                                ws.Cells["C11"].Value = "Preto";
                                ws.Cells["D11"].Value = "Cor";
                                foreach (var i in counter)
                                {
                                    ws.Cells["A" + idx].Value = equi.Name + "-" + i.serialNumber;
                                    ws.Cells["C" + idx].Value = i.bwCounter;
                                    ws.Cells["D" + idx].Value = i.cCounter;
                                    ws.Cells["H" + idx].Value = item.ClickPriceBW.GetValueOrDefault() != 0 ? Math.Round(item.ClickPriceBW.GetValueOrDefault(), 5) + "€" : "";
                                    ws.Cells["I" + idx].Value = item.ClickPriceC.GetValueOrDefault() != 0 ? Math.Round(item.ClickPriceC.GetValueOrDefault(), 5) + "€" : "";
                                    idx++;
                                }
                            }
                            else
                            {
                                ws.Cells["A" + idx].Value = equi.Name;
                                ws.Cells["H" + idx].Value = item.ClickPriceBW.GetValueOrDefault() != 0 ? Math.Round(item.ClickPriceBW.GetValueOrDefault(), 5) + "€" : "";
                                ws.Cells["I" + idx].Value = item.ClickPriceC.GetValueOrDefault() != 0 ? Math.Round(item.ClickPriceC.GetValueOrDefault(), 5) + "€" : "";
                                idx++;
                            }
                        }
                    }
                    //if (f != null && f.IncludeServices.Value == false)
                    //{
                    //    ws.Cells["E9"].Value = "Valor";
                    //    ws.Cells["E10"].Value = "Taxa Fixa";
                    //    switch (billingFrequency)
                    //    {
                    //        case 1: ws.Cells["E11"].Value = "Mensal"; break;
                    //        case 3: ws.Cells["E11"].Value = "Trimestral"; break;
                    //        case 6: ws.Cells["E11"].Value = "Semestral"; break;
                    //        default: ws.Cells["E11"].Value = "Mensal"; break;
                    //    }
                    //}
                    //else
                    //{
                    ws.Cells["E9"].Value = "";
                    ws.Cells["E10"].Value = "";
                    ws.Cells["E11"].Value = "";
                    ws.Cells["E22"].Value = "";
                    //}
                }

                string m = "";
                switch (billingFrequency)
                {
                    case 1:
                        m = "Mensal";
                        break;
                    case 3:
                        m = "Trimestral";
                        break;
                    case 6:
                        m = "Semestral";
                        break;
                    default:
                        break;
                }

                ws.Cells["F27"].Value = activePS.ContractDuration + " meses";
                ws.Cells["A29"].Value = "A facturação de páginas" + strExcende + "produzidas terá uma períodicidade:";
                ws.Cells["F29"].Value = m;


                if (f.PaymentAfter == 60)
                {
                    ws.Cells["A31"].Value = "O  pagamento das faturas referentes ao débito de páginas produzidas, deverá ser efectuado no prazo de 60 dias a contar da data de emissão das mesmas.";

                }
            }

            pck.Save();
            pck.Stream.Close();
        }
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");
    }
}

private void WriteContractoCliente(string path, string acocuntNumber, int? proposalID)
{
    try
    {
        BB_Clientes cliente = new BB_Clientes();
        List<BB_Proposal_Contacts_Signing> lstBB_Proposal_Contacts_Signing = new List<BB_Proposal_Contacts_Signing>();
        using (var db = new BB_DB_DEVEntities2())
        {
            cliente = db.BB_Clientes.Where(x => x.accountnumber == acocuntNumber).FirstOrDefault();
            lstBB_Proposal_Contacts_Signing = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == proposalID).ToList();
        }
        FileInfo newFile = new FileInfo(path);

        ExcelPackage pck = new ExcelPackage(newFile);
        //Add the Content sheet
        var ws = pck.Workbook.Worksheets["Ordem_Aquisicao"];
        //ws.View.ShowGridLines = false;

        ////ws.Column(4).OutlineLevel = 1;
        ////ws.Column(4).Collapsed = true;
        ////ws.Column(5).OutlineLevel = 1;
        ////ws.Column(5).Collapsed = true;
        ////ws.OutLineSummaryRight = true;

        ////Headers
        //ws.Cells["D4"].Value = cliente.Name != null ? cliente.Name : "";
        //ws.Cells["D6"].Value = cliente.NIF != null ? cliente.NIF : "";
        //ws.Cells["D5"].Value = cliente.address1_line1 != null ? cliente.address1_line1 : "";
        //ws.Cells["J5"].Value = cliente.PostalCode != null ? cliente.PostalCode + " " + cliente.City : "";
        //ws.Cells["K3"].Value = cliente.accountnumber;
        //ws.Cells["J6"].Value = cliente.emailaddress1;
        //ws.Cells["F6"].Value = cliente.telephone1;

        BB_Proposal_Contacts_Signing cs = new BB_Proposal_Contacts_Signing();
        cs = lstBB_Proposal_Contacts_Signing.Count > 0 ? lstBB_Proposal_Contacts_Signing[0] : new BB_Proposal_Contacts_Signing();

        ws.Cells["D4"].Value = cliente.Name != null ? cliente.Name : "";
        ws.Cells["D6"].Value = cliente.NIF != null ? cliente.NIF : "";
        ws.Cells["D5"].Value = cliente.address1_line1 != null ? cliente.address1_line1 : "";
        ws.Cells["J5"].Value = cliente.PostalCode != null ? cliente.PostalCode + " " + cliente.City : "";
        ws.Cells["K3"].Value = cliente.accountnumber;
        ws.Cells["J6"].Value = cs.Email;
        ws.Cells["F6"].Value = cs.Telefone;

        //KOnica Representante
        //ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
        //ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

        pck.Save();


        pck.Stream.Close();
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");

    }
}

private void WriteContractoService2(string path, string acocuntNumber, int? proposalID)
{
    try
    {
        FileInfo newFile = new FileInfo(path);

        ExcelPackage pck = new ExcelPackage(newFile);

        var ws = pck.Workbook.Worksheets["Service2"];
        int? nrMeses = 0;
        List<BB_Proposal_Quote_RS> lst = new List<BB_Proposal_Quote_RS>();
        List<BB_Proposal_OPSManage> opsManage = new List<BB_Proposal_OPSManage>();

        BB_Proposal_PrazoDiferenciado p = new BB_Proposal_PrazoDiferenciado();

        using (var db = new BB_DB_DEVEntities2())
        {
            lst = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == proposalID).ToList();
            opsManage = db.BB_Proposal_OPSManage.Where(x => x.ProposalID == proposalID).ToList();
            p = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == proposalID).FirstOrDefault();
        }

        if (lst != null && lst.Count() == 0 && opsManage != null && opsManage.Count() == 0)
        {
            pck.Workbook.Worksheets.Delete(ws);
        }
        else
        {
            int idx = 10;
            double? taxafixa = 0;

            foreach (var item in lst)
            {
                ws.Cells["A" + idx].Value = item.Description;
                ws.Cells["F" + idx].Value = item.CodeRef;
                ws.Cells["G" + idx].Value = item.Qty;
                ws.Cells["H" + idx].Value = item.TotalMonths;
                ws.Cells["I" + idx].Value = item.MonthlyFee;
                idx++;
                taxafixa += item.MonthlyFee;
                nrMeses = nrMeses > item.TotalMonths ? nrMeses : item.TotalMonths;
            }
            foreach (var item in opsManage)
            {
                ws.Cells["A" + idx].Value = item.Name + " - " + item.Description;
                ws.Cells["F" + idx].Value = item.CodeRef;
                ws.Cells["G" + idx].Value = item.Quantity;
                ws.Cells["H" + idx].Value = item.TotalMonths;
                ws.Cells["I" + idx].Value = item.UnitDiscountPrice * item.Quantity;
                idx++;
                //taxafixa +=   item.UnitDiscountPrice * item.Quantity * item.TotalMonths;
                taxafixa += item.UnitDiscountPrice * item.Quantity;
                nrMeses = nrMeses > item.TotalMonths ? nrMeses : item.TotalMonths;
            }

            int frquency = 1;
            BB_PrintingServices active = null;
            BB_VVA vva = null;
            BB_PrintingServices_NoVolume nv = null;
            Nullable<int> BWVolume = 0;
            Nullable<int> CVolume = 0;

            using (var db = new BB_DB_DEVEntities2())
            {
                BB_Proposal_PrintingServices2 ps2 = db.BB_Proposal_PrintingServices2
                    .Include(x => x.BB_PrintingServices.Select(y => y.BB_VVA))
                    .Include(x => x.BB_PrintingServices.Select(y => y.BB_PrintingServices_NoVolume))
                    .Where(x => x.ProposalID == proposalID)
                    .FirstOrDefault();

                if (ps2 != null && ps2.BB_PrintingServices.Count > 0)
                {

                    active = ps2.BB_PrintingServices.ToList()[(int)ps2.ActivePrintingService - 1];
                    if (active != null)
                    {
                        BWVolume = active.BWVolume;
                        CVolume = active.CVolume;
                        if (active.BB_VVA != null)
                        {
                            vva = active.BB_VVA;
                            frquency = vva.RentBillingFrequency.GetValueOrDefault();
                        }
                        if (active.BB_PrintingServices_NoVolume != null)
                        {
                            nv = active.BB_PrintingServices_NoVolume;
                            frquency = nv.PageBillingFrequency.GetValueOrDefault();
                        }
                    }
                }
            }




            ws.Cells["I28"].Value = taxafixa;
            ws.Cells["F33"].Value = nrMeses;
            ws.Cells["F35"].Value = ((frquency == 0 || frquency == 1) ? "Mensal" : "Trimestral");

            //FLEXPAGE
            if (p != null && (p.FinancingID == 6 || p.FinancingID == 5))
            {
                taxafixa = 0;
                idx = 10;

                foreach (var item in lst)
                {
                    ws.Cells["I" + idx].Value = "";
                    idx++;
                }
                foreach (var item in opsManage)
                {
                    ws.Cells["I" + idx].Value = "";
                    idx++;
                }

                ws.Cells["I28"].Value = "";
                ws.Cells["H28"].Value = "";
                ws.Cells["I9"].Value = "";
                ws.Cells["I8"].Value = "";
            }


            idx = 10;
            if (p != null && p.FinancingID == 3)
            {
                foreach (var item in lst)
                {
                    ws.Cells["I" + idx].Value = "";
                    idx++;
                }
                foreach (var item in opsManage)
                {
                    ws.Cells["I" + idx].Value = "";
                    idx++;
                }
                ws.Cells["H28"].Value = "";
                ws.Cells["I28"].Value = "";
                ws.Cells["I8"].Value = "";
                ws.Cells["I9"].Value = "";

            }
        }


        //KOnica Representante
        //ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
        //ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

        pck.Save();


        pck.Stream.Close();
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");

    }
}

private void WriteContractoAutoR(string path, string acocuntNumber)
{
    try
    {
        FileInfo newFile = new FileInfo(path);

        ExcelPackage pck = new ExcelPackage(newFile);

        var ws = pck.Workbook.Worksheets["AutoR"];

        pck.Workbook.Worksheets.Delete(ws);

        //KOnica Representante
        //ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
        //ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

        pck.Save();


        pck.Stream.Close();
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");

    }
}

private void WriteContractoDeclaracao(string path, string acocuntNumber, int? ProposalID)
{
    try
    {
        BB_Proposal_PrazoDiferenciado prazodiferencido = new BB_Proposal_PrazoDiferenciado();
        BB_Proposal_Overvaluation sobrevalorizacao = new BB_Proposal_Overvaluation();

        BB_Proposal_Financing financiamento = new BB_Proposal_Financing();
        List<BB_Proposal_FinancingMonthly> financiamentoMensal = new List<BB_Proposal_FinancingMonthly>();
        List<BB_Proposal_FinancingTrimestral> financiamentoTrimestreal = new List<BB_Proposal_FinancingTrimestral>();
        BB_Proposal_PrintingServices psconfigs = new BB_Proposal_PrintingServices();

        BB_FinancingType FinancingType = new BB_FinancingType();
        BB_FinancingPaymentMethod aymentMethod = new BB_FinancingPaymentMethod();
        BB_FinancingContractType contractType = new BB_FinancingContractType();
        string servicosIncluidos = "";
        using (var db = new BB_DB_DEVEntities2())
        {
            sobrevalorizacao = db.BB_Proposal_Overvaluation.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
            financiamento = db.BB_Proposal_Financing.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
            psconfigs = db.BB_Proposal_PrintingServices.Where(x => x.ProposalID == ProposalID).FirstOrDefault();
            if (financiamento != null)
            {
                financiamentoMensal = db.BB_Proposal_FinancingMonthly.Where(x => x.ProposalID == ProposalID && x.FinancingID == financiamento.ID).ToList();
                financiamentoTrimestreal = db.BB_Proposal_FinancingTrimestral.Where(x => x.ProposalID == ProposalID && x.FinancingID == financiamento.ID).ToList();
                FinancingType = db.BB_FinancingType.Where(x => x.Code == financiamento.FinancingTypeCode).FirstOrDefault();
                aymentMethod = db.BB_FinancingPaymentMethod.Where(x => x.ID == financiamento.PaymentMethodId).FirstOrDefault();
                contractType = db.BB_FinancingContractType.Where(x => x.ID == financiamento.ContractTypeId).FirstOrDefault();
                prazodiferencido = db.BB_Proposal_PrazoDiferenciado.Where(x => x.ProposalID == ProposalID && x.Type == "TAB" && x.IsAproved.Value == true).FirstOrDefault();
                servicosIncluidos = financiamento.IncludeServices.Value == true ? " com serviços incluídos" : "";
            }
        }

        FileInfo newFile = new FileInfo(path);

        ExcelPackage pck = new ExcelPackage(newFile);

        var ws = pck.Workbook.Worksheets["Declaração"];

        if (financiamento != null && (financiamento.FinancingTypeCode == 0 || financiamento.FinancingTypeCode == 5))
        {
            ws.Cells["A33"].Value = "Não Aplicável";
        }

        if (financiamento != null && financiamento.FinancingTypeCode != 0 && financiamento.FinancingTypeCode != 5 && prazodiferencido != null && prazodiferencido.Alocadora == "BNP")
        {
            ws.Cells["A33"].Value = "A Locadora está autorizada pelo Cliente, a transmitir a sua posição contratual no âmbito do presente contrato ao BNP PARIBAS LEASE GROUP, S.A.";
        }

        if (financiamento != null && financiamento.FinancingTypeCode != 0 && financiamento.FinancingTypeCode != 5 && prazodiferencido != null && prazodiferencido.Alocadora == "GRENK")
        {
            ws.Cells["A33"].Value = "A Locadora está autorizada pelo Cliente, a transmitir total ou parcialmente a sua posição contratual no âmbito do presente contrato à GRENKE RENTING, S.A., atuando esta última em seu próprio nome, mas por conta da sociedade denominada Grenke Finance Plc, com sede em Q-House 306, Furze Road, Sandyford Industrial Estate, Dublin 18 (Irlanda)";
        }

        //KOnica Representante
        //ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
        //ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";



        using (var db = new BB_DB_DEVEntities2())
        {
            List<BB_Proposal_Contacts_Signing> cSigning = db.BB_Proposal_Contacts_Signing.Where(x => x.ProposalID == ProposalID).ToList();

            int idx = 0;

            ws.Cells["I40"].Value = "Hugo Silva";


            foreach (var i in cSigning)
            {
                if (idx == 0)
                {
                    ws.Cells["B40"].Value = i.Name;
                    //ws.Cells["D40"].Value = DateTime.Now.ToString("dd/MM/yyyy");

                }
                if (idx == 1)
                {
                    ws.Cells["B42"].Value = i.Name;
                    //ws.Cells["D42"].Value = DateTime.Now.ToString("dd/MM/yyyy");
                    idx++;
                }
                idx++;

            }
        }



        pck.Save();


        pck.Stream.Close();
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");

    }
}

[AcceptVerbs("GET", "POST")]
[ActionName("ProFormaPrinting")]
public HttpResponseMessage ProFormaPrinting(int? ProposalID)
{
    int ID = 354;
    //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
    string erro = "";
    //int proposalID = ID;
    int? proposalID = ProposalID;
    //Create HTTP Response.
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
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
        string strFolder = "Proforma";
        string strCliente = c.DisplayName;
        path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

        if (!Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);


        using (var stream = File.Open(@AppSettingsGet.ProFormaPrintingTemplate, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            try
            {
                using (var outputFile = new FileStream(path + "\\ProForma.xlsx", FileMode.Create))
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


        //QUOTE WRITE TO EXCEL
        string resultQuote = ProFormaWriteToExcelQuote(@path + "\\Proforma.xlsx", proposalID, p.ValueTotal, p.ClientAccountNumber);
        //BtnTest_Click(@path + "\\ProForma.xlsx");
        //Cliente WRITE TO EXCEL
        //bool resultClient = WriteToExcelCliente(@path + "\\Proposal.xlsx", proposalID);

        //Thread.Sleep(3000);




        ExportWorkbookToPdfProforma(@path + "\\ProForma.xlsx", @path + "\\ProForma.pdf", proposalID);


        string filePath = @path + "\\ProForma.pdf";


        //Check whether File exists.
        if (!File.Exists(filePath))
        {
            //Throw 404 (Not Found) exception if File not found.
            response.StatusCode = HttpStatusCode.NotFound;
            response.ReasonPhrase = string.Format("File not found: .");
            throw new HttpResponseException(response);
        }

        byte[] bytes = File.ReadAllBytes(filePath);

        //Set the Response Content.

        response.Content = new ByteArrayContent(bytes);

        //Set the Response Content Length.
        response.Content.Headers.ContentLength = bytes.LongLength;

        //Set the Content Disposition Header Value and FileName.
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        response.Content.Headers.ContentDisposition.FileName = "ProForma.pdf";

        //Set the File Content Type.
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("ProForma.pdf"));
        return response;

    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\ProForma.xlsx");
        erro = ex.Message.ToString();
    }
    Console.WriteLine(erro);
    return response;
}

[AcceptVerbs("GET", "POST")]
[ActionName("DetalheDownload")]
public HttpResponseMessage DetalheDownload(int? ProposalID)
{
    int ID = 354;
    //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
    string erro = "";
    //int proposalID = ID;
    int? proposalID = ProposalID;
    //Create HTTP Response.
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
    string path = "";
    try
    {
        BB_Proposal_LeaseDesk_Detalhe d = new BB_Proposal_LeaseDesk_Detalhe();
        BB_Proposal p = new BB_Proposal();
        AspNetUsers c = new AspNetUsers();
        using (var db = new BB_DB_DEVEntities2())
        {
            d = db.BB_Proposal_LeaseDesk_Detalhe.Where(x => x.ID == ProposalID).FirstOrDefault();
            p = db.BB_Proposal.Where(x => x.ID == d.ProposalID).First();

        }
        using (var db = new masterEntities())
        {
            c = db.AspNetUsers.Where(x => x.Email == p.CreatedBy).FirstOrDefault();

        }


        string strUser = c.DisplayName;
        string strFolder = "NUS";
        string strCliente = c.DisplayName;
        path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

        if (!Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);


        using (var stream = File.Open(@AppSettingsGet.NUS_Template, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            try
            {
                using (var outputFile = new FileStream(path + "\\NUS_Template.xlsx", FileMode.Create))
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


        //QUOTE WRITE TO EXCEL
        string resultQuote = NUSDetalheExcel(@path + "\\NUS_Template.xlsx", ProposalID);

        //BtnTest_Click(@path + "\\ProForma.xlsx");
        //Cliente WRITE TO EXCEL
        //bool resultClient = WriteToExcelCliente(@path + "\\Proposal.xlsx", proposalID);

        //Thread.Sleep(3000);




        //ExportWorkbookToPdfProforma(@path + "\\NUS_Template.xlsx", @path + "\\NUS_Template.pdf", proposalID);


        //string filePath = @path + "\\NUS_Template.pdf";


        //Check whether File exists.
        string excelPath = @path + "\\NUS_Template.xlsx";



        if (!File.Exists(excelPath))
        {
            //Throw 404 (Not Found) exception if File not found.
            response.StatusCode = HttpStatusCode.NotFound;
            response.ReasonPhrase = string.Format("File not found: .");
            throw new HttpResponseException(response);
        }

        byte[] bytes = File.ReadAllBytes(excelPath);

        //Set the Response Content.

        response.Content = new ByteArrayContent(bytes);

        //Set the Response Content Length.
        response.Content.Headers.ContentLength = bytes.LongLength;

        //Set the Content Disposition Header Value and FileName.
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        response.Content.Headers.ContentDisposition.FileName = "NUS_Template.xlsx";

        //Set the File Content Type.
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("NUS_Template.xlsx"));

        return response;

    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\NUS_Template.xlsx");
        erro = ex.Message.ToString();
    }
    Console.WriteLine(erro);
    return response;
}

private string NUSDetalheExcel(string path, int? DetalheID)
{
    try
    {
        BB_Clientes cliente = new BB_Clientes();
        BB_Proposal_LeaseDesk_Detalhe l = new BB_Proposal_LeaseDesk_Detalhe();
        BB_Proposal p = new BB_Proposal();
        using (var db = new BB_DB_DEVEntities2())
        {

            l = db.BB_Proposal_LeaseDesk_Detalhe.Where(x => x.ID == DetalheID).FirstOrDefault();
            p = db.BB_Proposal.Where(x => x.ID == l.ProposalID).FirstOrDefault();
            cliente = db.BB_Clientes.Where(x => x.accountnumber == p.AccountManager).FirstOrDefault();
        }
        FileInfo newFile = new FileInfo(path);

        ExcelPackage pck = new ExcelPackage(newFile);
        //Add the Content sheet
        var ws = pck.Workbook.Worksheets["Formulário Encomenda"];
        //ws.View.ShowGridLines = false;

        ////ws.Column(4).OutlineLevel = 1;
        ////ws.Column(4).Collapsed = true;
        ////ws.Column(5).OutlineLevel = 1;
        ////ws.Column(5).Collapsed = true;
        ////ws.OutLineSummaryRight = true;

        ////Headers
        //ws.Cells["D4"].Value = cliente.Name != null ? cliente.Name : "";
        //ws.Cells["D6"].Value = cliente.NIF != null ? cliente.NIF : "";
        //ws.Cells["D5"].Value = cliente.address1_line1 != null ? cliente.address1_line1 : "";
        //ws.Cells["J5"].Value = cliente.PostalCode != null ? cliente.PostalCode + " " + cliente.City : "";
        //ws.Cells["K3"].Value = cliente.accountnumber;
        //ws.Cells["J6"].Value = cliente.emailaddress1;
        //ws.Cells["F6"].Value = cliente.telephone1;


        //Numero de cliente
        ws.Cells["F6"].Value = cliente.accountnumber;

        //Moara
        ws.Cells["F8"].Value = l.Adress1;

        //CodigoPostal
        ws.Cells["F10"].Value = l.PostalCode;

        //Localidade
        ws.Cells["F11"].Value = l.City;

        //Localidade
        ws.Cells["F12"].Value = l.Contacto;

        //Contribuinte
        ws.Cells["F14"].Value = cliente.NIF;


        //KOnica Representante
        //ws.Cells["A54"].Value = "Sede: Edifício Sagres - Rua Prof. Henrique de Barros, 4-10ºB   2685-338 PRIOR VELHO    Tel. 219 492 108  Fax 219 492 198";
        //ws.Cells["A55"].Value = "NIB: 003300000000521753405 - Cont. nº 502 120 070 - Cap.Soc.Euros 2.750.100 - Matrícula na CRC de Loures sob o nº 20563";

        pck.Save();


        pck.Stream.Close();
    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");

    }

    return "";
}

private void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
{
    //options
    object matchCase = false;
    object matchWholeWord = true;
    object matchWildCards = false;
    object matchSoundsLike = false;
    object matchAllWordForms = false;
    object forward = true;
    object format = false;
    object matchKashida = false;
    object matchDiacritics = false;
    object matchAlefHamza = false;
    object matchControl = false;
    object read_only = false;
    object visible = false;
    object replace = 2;
    object wrap = 1;
    //execute find and replace


    doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
        ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
        ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);

    doc.Selection.Find.ClearFormatting();
}

[AcceptVerbs("GET", "POST")]
[ActionName("PrintingContractoByProposalID")]
public HttpResponseMessage PrintingContractoByProposalID(int? proposalID)
{
    //int ID = 375;

    //GerarContrato gc = new GerarContrato();
    //gc.proposalID = 472;
    //gc.Selectmonthly = 18;

    //BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
    string erro = "";
    //int proposalID = ID;
    //int? proposalID = o.ProposalID;
    //Create HTTP Response.
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

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
        string strFolder = "Contracto";
        string strCliente = c.DisplayName;
        path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;

        if (!Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);


        using (var stream = File.Open(@AppSettingsGet.DocumentPrintingContractoTemplate, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

            try
            {
                using (var outputFile = new FileStream(path + "\\Contracto.xlsx", FileMode.Create))
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

        WriteContractoCliente(@path + "\\Contracto.xlsx", p.ClientAccountNumber, proposalID);

        //Cliente WRITE TO EXCEL
        //bool resultClient = WriteToExcelCliente(@path + "\\Proposal.xlsx", proposalID);
        WriteContractoQuote(@path + "\\Contracto.xlsx", proposalID, p);
        //Thread.Sleep(3000);
        //WritePropostaFinanceira(@path + "\\Contracto.xlsx", proposalID, p);

        WriteContractoServicos(@path + "\\Contracto.xlsx", proposalID, p);

        WriteContractoService2(@path + "\\Contracto.xlsx", p.ClientAccountNumber, proposalID);
        //WritePropostaOutrasCondicoes(@path + "\\Contracto.xlsx", proposalID, p);

        WriteContractoAutoR(@path + "\\Contracto.xlsx", p.ClientAccountNumber);

        WriteContractoDeclaracao(@path + "\\Contracto.xlsx", p.ClientAccountNumber, p.ID);


        ExportWorkbookToPdfContrato(@path + "\\Contracto.xlsx", @path + "\\Contracto.pdf", proposalID);


        string filePath = @path + "\\Contracto.pdf";
        string excelPath = @path + "\\Contracto.xlsx";

        if (false)
        {
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: .");
                throw new HttpResponseException(response);
            }

            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.

            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Contracto.pdf";

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contracto.pdf"));
        }
        if (true)
        {
            if (!File.Exists(excelPath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: .");
                throw new HttpResponseException(response);
            }

            byte[] bytes = File.ReadAllBytes(excelPath);

            //Set the Response Content.

            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Contrato.xlsx";

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contrato.xlsx"));
        }


        return response;

    }
    catch (Exception ex)
    {
        File.Delete(@path + "\\Contracto.xlsx");
        erro = ex.Message.ToString();
    }
    Console.WriteLine(erro);
    return response;
}

[AcceptVerbs("GET", "POST")]
[ActionName("GenerateContractExcel")]
public HttpResponseMessage GenerateContractExcel(int proposalID)
{
    string error = "";
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
    try
    {
        string filePath = GenerateContract(proposalID);
        filePath += ".xlsx";
        if (!File.Exists(filePath))
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.ReasonPhrase = string.Format("File not found: .");
            throw new HttpResponseException(response);
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        response.Content = new ByteArrayContent(bytes);
        response.Content.Headers.ContentLength = bytes.LongLength;
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        response.Content.Headers.ContentDisposition.FileName = "Contrato.xlsx";
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contrato.xlsx"));
    }
    catch (Exception ex)
    {
        error = ex.Message.ToString();
    }
    return response;
}

[AcceptVerbs("GET", "POST")]
[ActionName("GenerateContractPDF")]
public HttpResponseMessage GenerateContractPDF(int proposalID)
{
    string error = "";
    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
    try
    {
        string filePath = GenerateContract(proposalID);
        filePath += ".pdf";
        if (!File.Exists(filePath))
        {
            response.StatusCode = HttpStatusCode.NotFound;
            response.ReasonPhrase = string.Format("File not found: .");
            throw new HttpResponseException(response);
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        response.Content = new ByteArrayContent(bytes);
        response.Content.Headers.ContentLength = bytes.LongLength;
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        response.Content.Headers.ContentDisposition.FileName = "Contracto.pdf";
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Contracto.pdf"));
    }
    catch (Exception ex)
    {
        error = ex.Message.ToString();
    }
    return response;
}

private string GenerateContract(int proposalID)
{
    string path = "";
    string error = "";
    string filePath = "";
    try
    {
        BB_Proposal proposal = new BB_Proposal();
        AspNetUsers accountManager = new AspNetUsers();
        using (var db = new BB_DB_DEVEntities2())
        {
            proposal = db.BB_Proposal.Where(x => x.ID == proposalID).First();

        }
        using (var db = new masterEntities())
        {
            accountManager = db.AspNetUsers.Where(x => x.Email == proposal.CreatedBy).FirstOrDefault();
        }
        string strUser = accountManager.DisplayName;
        string strFolder = "Contracto";
        string strCliente = accountManager.DisplayName;
        path = @AppSettingsGet.DocumentPrintingFolder + strUser + "\\" + strFolder + "\\" + strCliente;
        if (!Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
        using (var stream = File.Open(@AppSettingsGet.DocumentPrintingContractoTemplate, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            try
            {
                using (var outputFile = new FileStream(path + "\\Contracto.xlsx", FileMode.Create))
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
        WriteContractoCliente(@path + "\\Contracto.xlsx", proposal.ClientAccountNumber, proposalID);
        WriteContractoQuote(@path + "\\Contracto.xlsx", proposalID, proposal);
        WriteContractoServicos(@path + "\\Contracto.xlsx", proposalID, proposal);
        WriteContractoService2(@path + "\\Contracto.xlsx", proposal.ClientAccountNumber, proposalID);
        WriteContractoAutoR(@path + "\\Contracto.xlsx", proposal.ClientAccountNumber);
        WriteContractoDeclaracao(@path + "\\Contracto.xlsx", proposal.ClientAccountNumber, proposal.ID);
        ExportWorkbookToPdfContrato(@path + "\\Contracto.xlsx", @path + "\\Contracto.pdf", proposalID);
        filePath = @path + "\\Contracto";

    }
    catch (Exception ex)
    {
        error = ex.Message.ToString();
    }
    return filePath;
}

    }
}
