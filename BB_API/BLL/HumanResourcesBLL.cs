using AutoMapper;
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
using System.Web;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Models.ViewModels.HumanResourcesViewModels;

namespace WebApplication1.BLL
{
    public class HumanResourcesBLL
    {
        public BB_DB_DEVEntities2 db = new BB_DB_DEVEntities2();
        public BB_DB_DEV_LeaseDesk ldDB = new BB_DB_DEV_LeaseDesk();
        public CommissionEntry GetCommissionEntryByID(int id)
        {
            var proposalInfo = db.BB_Proposal.Where(x => x.ID == id).FirstOrDefault();
            if (proposalInfo != null)
            {
                List<BB_Proposal_Quote> oneShot = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == id).ToList();
                List<OsBasket> osBasket = new List<OsBasket>();
                foreach (var quote in oneShot)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Quote, OsBasket>();
                    });
                    IMapper iMapper1 = config1.CreateMapper();
                    OsBasket basket = iMapper1.Map<BB_Proposal_Quote, OsBasket>(quote);
                    osBasket.Add(basket);
                }

                List<BB_Proposal_Quote_RS> recurringServices = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == id).ToList();
                List<RsBasket> rsBasket = new List<RsBasket>();
                foreach (var quote in recurringServices)
                {
                    var config1 = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BB_Proposal_Quote_RS, RsBasket>();
                    });
                    IMapper iMapper1 = config1.CreateMapper();
                    RsBasket basket = iMapper1.Map<BB_Proposal_Quote_RS, RsBasket>(quote);
                    rsBasket.Add(basket);
                }

                ProposalBLL p1 = new ProposalBLL();
                LoadProposalInfo i = new LoadProposalInfo();
                i.ProposalId = id;
                ActionResponse ar = p1.LoadProposal(i);

                double TFT = 0;
                ApprovedPrintingService activePS = null;
                if (ar.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    activePS = ar.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[ar.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                    if (activePS != null && activePS.GlobalClickVVA != null)
                    {
                        TFT = activePS.GlobalClickVVA.PVP * activePS.ContractDuration;
                    }
                }

                List<OsBasket> oneShotHardware = osBasket.Where(x => x.IsUsed == false && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                List<OsBasket> usedHardware = osBasket.Where(x => x.IsUsed == true).ToList();
                List<RsBasket> recurringServicesHardware = rsBasket.Where(x => x.Family == "OPSHW" || x.Family == "PPHW").ToList();
                List<OsBasket> oneShotITSProducts = osBasket.Where(x => x.Family == "PRSSW" || x.Family == "PRSHW" || x.Family == "MCSHW" || x.Family == "MCSSW" || x.Family == "IMSHW" || x.Family == "IMSSW").ToList();
                List<RsBasket> recurringServicesITSProducts = rsBasket.Where(x => x.Family == "PRSSW" || x.Family == "PRSHW" || x.Family == "MCSHW" || x.Family == "MCSSW" || x.Family == "IMSHW" || x.Family == "IMSSW").ToList();
                List<OsBasket> oneShotITSServices = osBasket.Where(x => x.Family == "OPSPSV" || x.Family == "IMSCSV" || x.Family == "IMSMSV" || x.Family == "IMSPSV" || x.Family == "MCSPSV" || x.Family == "MCSCSV" || x.Family == "MCSMSV" || x.Family == "PRSMSV" || x.Family == "PRSPSV").ToList();
                List<RsBasket> recurringServicesITSServices = rsBasket.Where(x => x.Family == "OPSPSV" || x.Family == "IMSCSV" || x.Family == "IMSMSV" || x.Family == "IMSPSV" || x.Family == "MCSPSV" || x.Family == "MCSCSV" || x.Family == "MCSMSV" || x.Family == "PRSMSV" || x.Family == "PRSPSV").ToList();
                List<OsBasket> oneShotIP = osBasket.Where(x => x.Family == "IP").ToList();
                List<RsBasket> recurringServicesIP = rsBasket.Where(x => x.Family == "IP").ToList();
                CommissionEntry cee = new CommissionEntry()
                {
                    CRMQuoteID = proposalInfo.CRM_QUOTE_ID,
                    oneShotHardware = oneShotHardware,
                    oneShotUsedHardware = usedHardware,
                    recurringServicesHardware = recurringServicesHardware,
                    oneShotIP = oneShotIP,
                    oneShotITSProducts = oneShotITSProducts,
                    oneShotITSServices = oneShotITSServices,
                    ProposalID = id,
                    recurringServicesITSProducts = recurringServicesITSProducts,
                    recurringServicesITSServices = recurringServicesITSServices,
                    HardwareNetsale = oneShotHardware.Sum(x => x.TotalNetsale) + recurringServicesHardware.Sum(x => x.TotalNetsale),
                    UsedHardwarePVP = usedHardware.Sum(x => x.TotalPVP),
                    UsedHardwareNetsale = usedHardware.Sum(x => x.TotalNetsale),
                    HardwarePVP = oneShotHardware.Sum(x => x.TotalPVP) + recurringServicesHardware.Sum(x => x.TotalPVP),
                    ITSProductsNetsale = oneShotITSProducts.Sum(x => x.TotalNetsale) + recurringServicesITSProducts.Sum(x => x.TotalNetsale),
                    ITSProductsPVP = oneShotITSProducts.Sum(x => x.TotalPVP) + recurringServicesITSProducts.Sum(x => x.TotalPVP),
                    ITSServicesNetsale = oneShotITSServices.Sum(x => x.TotalNetsale) + recurringServicesITSServices.Sum(x => x.TotalNetsale),
                    IndustrialPrintingNetsale = oneShotIP.Sum(x => x.TotalNetsale) + recurringServicesIP.Sum(x => x.TotalNetsale),
                    TFT = TFT
                };
                return cee;
            }
            return null;
        }


        public List<CommissionEntry> GetCommissionEntries()
        {
            List<CommissionEntry> commissionEntries = new List<CommissionEntry>();
            try
            {
                DateTime b = new DateTime(2023, 05, 01);
                //DateTime a = new DateTime(2021, 10, 01);
                List<LD_Contrato> proposalInfo = ldDB.LD_Contrato.Where(x => x.CreatedBy != "jorge.colaco@konicaminolta.pt" && x.CreatedBy != "joao.reis@konicaminolta.pt" && x.CreatedBy != "diogo.cruz@konicaminolta.pt" && x.CreatedTime >= b).ToList();
                //List < LD_Contrato > proposalInfo = ldDB.LD_Contrato.Where(x => x.QuoteNumber == "QU2-432683-H9C0W0").ToList();
                foreach (LD_Contrato ldc in proposalInfo)
                {
                    ProposalBLL p1 = new ProposalBLL();
                    LoadProposalInfo i = new LoadProposalInfo();
                    i.ProposalId = ldc.ProposalID.Value;
                    ActionResponse ar = p1.LoadProposal(i);

                    List<BB_Proposal_Quote> oneShot = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == ldc.ProposalID).ToList();
                    List<BB_Proposal_Quote_RS> recurringServices = db.BB_Proposal_Quote_RS.Where(x => x.ProposalID == ldc.ProposalID).ToList();

                    double TFT = 0;
                    ApprovedPrintingService activePS = null;
                    if (ar.ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                    {
                        activePS = ar.ProposalObj.Draft.printingServices2.ApprovedPrintingServices[ar.ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                        if (activePS != null && activePS.GlobalClickVVA != null)
                        {
                            TFT = activePS.GlobalClickVVA.PVP * activePS.ContractDuration;
                        }
                    }



                    List<BB_Proposal_Quote> oneShotHardware = oneShot.Where(x => x.IsUsed == false && (x.Family == "OPSHW" || x.Family == "PPHW")).ToList();
                    List<BB_Proposal_Quote> oneShotUsedHardware = oneShot.Where(x => x.IsUsed == true).ToList();
                    List<BB_Proposal_Quote_RS> recurringServicesHardware = recurringServices.Where(x => x.Family == "OPSHW" || x.Family == "PPHW").ToList();
                    List<BB_Proposal_Quote> oneShotITSProducts = oneShot.Where(x => x.Family == "PRSSW" || x.Family == "PRSHW" || x.Family == "MCSHW" || x.Family == "MCSSW" || x.Family == "IMSHW" || x.Family == "IMSSW").ToList();
                    List<BB_Proposal_Quote_RS> recurringServicesITSProducts = recurringServices.Where(x => x.Family == "PRSSW" || x.Family == "PRSHW" || x.Family == "MCSHW" || x.Family == "MCSSW" || x.Family == "IMSHW" || x.Family == "IMSSW").ToList();
                    List<BB_Proposal_Quote> oneShotITSServices = oneShot.Where(x => x.Family == "OPSPSV" || x.Family == "IMSCSV" || x.Family == "IMSMSV" || x.Family == "IMSPSV" || x.Family == "MCSPSV" || x.Family == "MCSCSV" || x.Family == "MCSMSV" || x.Family == "PRSMSV" || x.Family == "PRSPSV").ToList();
                    List<BB_Proposal_Quote_RS> recurringServicesITSServices = recurringServices.Where(x => x.Family == "OPSPSV" || x.Family == "IMSCSV" || x.Family == "IMSMSV" || x.Family == "IMSPSV" || x.Family == "MCSPSV" || x.Family == "MCSCSV" || x.Family == "MCSMSV" || x.Family == "PRSMSV" || x.Family == "PRSPSV").ToList();
                    List<BB_Proposal_Quote> oneShotIP = oneShot.Where(x => x.Family == "IP").ToList();
                    List<BB_Proposal_Quote_RS> recurringServicesIP = recurringServices.Where(x => x.Family == "IP").ToList();
                    CommissionEntry cee = new CommissionEntry()
                    {
                        ProposalID = ldc.ProposalID.GetValueOrDefault(),
                        CRMQuoteID = ldc.QuoteNumber,
                        HardwareNetsale = oneShotHardware.Sum(x => x.TotalNetsale) + recurringServicesHardware.Sum(x => x.TotalNetsale),
                        UsedHardwarePVP = oneShotUsedHardware.Sum(x => x.TotalPVP),
                        UsedHardwareNetsale = oneShotUsedHardware.Sum(x => x.TotalNetsale),
                        HardwarePVP = oneShotHardware.Sum(x => x.TotalPVP) + recurringServicesHardware.Sum(x => x.TotalPVP),
                        ITSProductsNetsale = oneShotITSProducts.Sum(x => x.TotalNetsale) + recurringServicesITSProducts.Sum(x => x.TotalNetsale),
                        ITSProductsPVP = oneShotITSProducts.Sum(x => x.TotalPVP) + recurringServicesITSProducts.Sum(x => x.TotalPVP),
                        ITSServicesNetsale = oneShotITSServices.Sum(x => x.TotalNetsale) + recurringServicesITSServices.Sum(x => x.TotalNetsale),
                        IndustrialPrintingNetsale = oneShotIP.Sum(x => x.TotalNetsale) + recurringServicesIP.Sum(x => x.TotalNetsale),
                        TFT = TFT,
                        Createtime = ldc.CreatedTime

                    };
                    commissionEntries.Add(cee);
                }
                commissionEntries.Reverse();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return commissionEntries;
        }

        public List<LD_ContratoModel> GetCommissionEntries_new(string year)
        {
            List<LD_ContratoModel> listModel = new List<LD_ContratoModel>();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_LeaseDesk_ALL_New", conn);
                    cmd.Parameters.Add("@year1", SqlDbType.NVarChar, 20).Value = year;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {


                        LD_ContratoModel m = new LD_ContratoModel();
                        m.ID = (int)rdr["ID"];
                        m.ProposalID = (int)rdr["ProposalID"];
                        m.StatusName = rdr["Status"].ToString();
                        m.SistemaAssinatura = rdr["Assinatura"].ToString();
                        m.Comments = rdr["Comentarios"].ToString();
                        m.MotivoDescricao = rdr["EstadoProcesso"].ToString();
                        m.DevolucaoMotivoDescricao = rdr["Devolucao"].ToString();
                        m.ComentariosDevolucao = //i.ComentariosDevolucao;
                        m.QuoteNumber = rdr["Quote"].ToString();
                        //LD_Contrato_Facturacao cf = db.LD_Contrato_Facturacao.Where(x => x.LDID == i.ID).FirstOrDefault();
                        //if (cf != null)
                        //{
                        //    m.NUS = cf.NUS;
                        //}
                        m.isClosed = rdr["isClosed"] != null ? Boolean.Parse(rdr["isClosed"].ToString()) : false;

                        m.NCliente = rdr["NCliente"].ToString();
                        m.NomeCliente = rdr["Cliente"].ToString();

                        m.ModifiedBy = rdr["ModificadoPor"].ToString();
                        m.CreatedBy = rdr["CriadoPor"].ToString();

                        m.ModifiedTime = rdr["ModificadoEM"] != null ? DateTime.Parse(rdr["ModificadoEM"].ToString()) : new DateTime();
                        m.CreatedTime = rdr["CriadoEM"] != null ? DateTime.Parse(rdr["CriadoEM"].ToString()) : new DateTime();

                        m.Financiamento = rdr["Locadora"].ToString();

                        m.TipoNegocio = rdr["TipoNegocio"].ToString();

                        listModel.Add(m);



                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return listModel;
        }
        public HttpResponseMessage ExportCommissionExcel()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            List<CommissionEntry> commissionEntries = GetCommissionEntries();
            string folderPath = @AppSettingsGet.HumanResourcesFileLocation;
            string fileName = "CommissionExport.xlsx";
            if (!Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);
            string filePath = folderPath + fileName;
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets.Add("Totais de Negócio");
                    ws.Cells["A1"].Value = "Quote";
                    ws.Cells["B1"].Value = "PVP Hardware";
                    ws.Cells["C1"].Value = "Preço Venda Hardware";
                    ws.Cells["D1"].Value = "PVP Hardware Usados";
                    ws.Cells["E1"].Value = "Preço Venda Hardware Usados";
                    ws.Cells["F1"].Value = "PVP Produtos ITS";
                    ws.Cells["G1"].Value = "Preço Venda Produtos ITS";
                    ws.Cells["H1"].Value = "Preço Venda Serviços ITS";
                    ws.Cells["I1"].Value = "Preço Venda IP";
                    ws.Cells["J1"].Value = "TFT";
                    ws.Cells["K1"].Value = "Date Criacao";
                    ws.Cells["A1:K1"].Style.Font.Bold = true;
                    int row = 2;
                    foreach (CommissionEntry cee in commissionEntries)
                    {
                        String rowString = row.ToString();
                        ws.Cells["A" + rowString].Value = cee.CRMQuoteID != null? cee.CRMQuoteID.ToString() :"";
                        ws.Cells["B" + rowString].Value = cee.HardwarePVP.ToString();
                        ws.Cells["C" + rowString].Value = cee.HardwareNetsale.ToString();
                        ws.Cells["D" + rowString].Value = cee.UsedHardwarePVP.ToString();
                        ws.Cells["E" + rowString].Value = cee.UsedHardwareNetsale.ToString();
                        ws.Cells["F" + rowString].Value = cee.ITSProductsPVP.ToString();
                        ws.Cells["G" + rowString].Value = cee.ITSProductsNetsale.ToString();
                        ws.Cells["H" + rowString].Value = cee.ITSServicesNetsale.ToString();
                        ws.Cells["I" + rowString].Value = cee.IndustrialPrintingNetsale.ToString();
                        ws.Cells["J" + rowString].Value = cee.TFT.ToString();
                        ws.Cells["K" + rowString].Value = cee.Createtime.ToString();
                        row++;
                    }
                    package.SaveAs(new FileInfo(@filePath));
                    package.Stream.Close();
                }
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
                response.Content.Headers.ContentDisposition.FileName = fileName;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            }
            catch (Exception ex)
            {
                File.Delete(@filePath);
            }
            return response;
        }
    }
}