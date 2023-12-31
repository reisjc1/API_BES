﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class ComissionController : ApiController
    {
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

                            if ((int)rdr["ID"] > 2000)
                            {
                                var test = m;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }


            return lst;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetProposta")]
        public IHttpActionResult GetProposta(int? proposalID)
        {
            try
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

                //if (sobrevalorizacao1 != null && sobrevalorizacao1 != 0)
                //{

                //    a.ProposalObj.Draft.details.ValueTotal = 0;
                //    foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                //    {
                //        quote.TotalNetsale = sobrevalorizacao1 != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao1) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;
                //        quote.UnitDiscountPrice = Math.Round((quote.TotalNetsale / quote.Qty), 2);
                //        //quote.TotalNetsale = sobrevalorizacao != 0 && quote.Family == "OPSHW" ? Math.Round((((quote.TotalNetsale / OPSHWvalorTotal) * sobrevalorizacao) + quote.TotalNetsale).Value, 2) : quote.TotalNetsale;

                //        a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                //    }
                //    a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
                //}
                //else
                //{
                //    //a.ProposalObj.Draft.details.ValueTotal = 0;
                //    //foreach (var quote in a.ProposalObj.Draft.baskets.os_basket)
                //    //{
                //    //    a.ProposalObj.Draft.details.ValueTotal += quote.TotalNetsale;
                //    //}
                //    //a.ProposalObj.Draft.details.ValueTotal += LeiDaCopiaPriada.Value;
                //}

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
                    try
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
                    catch (Exception ex)
                    {
                        
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



                ProposalBLL p11 = new ProposalBLL();
                LoadProposalInfo ii = new LoadProposalInfo();
                ii.ProposalId = proposalID.Value;
                ActionResponse aa = p11.LoadProposal(ii);
                //CALCULO COMISSOES
                Comission _Comission = new Comission();


                //HW PRINTING
                HW_Printing hw = new HW_Printing();
                hw = CalculoHWPrinting(aa.ProposalObj);
                _Comission.Hw_Printing = hw;


                //ITS
                ITS_Comissao its  = CalculoITS(aa.ProposalObj);
                _Comission.ITS = its;


                //VVA
                TFM_Comissao tfm = CalculoTFM(aa.ProposalObj);
                _Comission.TFM = tfm;


                //ACELADOR NOVO CLIETNE
               NewClient_Comissao com  = CalculoNewClient(_Comission, proposalID);
                _Comission.Newclient = com;

                //ACELADOR Debito Directo
                DebitoDirecto_Commisao d = CalculoDebitoDirecto(_Comission, proposalID);
                _Comission.DebitoDirecto = d;

                _Comission.TotalComissao = _Comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() 
                    + _Comission.ITS.ITSComissao.GetValueOrDefault() + 
                    _Comission.TFM.TFMComissao.GetValueOrDefault() +
                    _Comission.Newclient.NewClientComissao.GetValueOrDefault()
                    + _Comission.DebitoDirecto.DebitoDirectoComissao.GetValueOrDefault();


                a.ProposalObj.Comission = _Comission;

                return Ok(a.ProposalObj);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok();
        }

        private DebitoDirecto_Commisao CalculoDebitoDirecto(Comission comission, int? proposalID)
        {
            //double? DebitoDirecto = 0;

            DebitoDirecto_Commisao DebitoDirecto = new DebitoDirecto_Commisao();
            DebitoDirecto.Isdebit = false;
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_Financing _BB_Proposal_Financing = db.BB_Proposal_Financing.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                    //Codigo debito directo é 2
                    if (_BB_Proposal_Financing != null && _BB_Proposal_Financing.PaymentMethodId.GetValueOrDefault() == 2)
                    {
                        DebitoDirecto.Isdebit = true;
                        double? Applydiscount = 2.5;
                        double? Applydiscountpercetagem = Math.Round((Applydiscount / 100).GetValueOrDefault(), 4);
                        DebitoDirecto.ValueValor = comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault();
                        DebitoDirecto.Percentagem = Applydiscount;
                        double? vlaor = (comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault()) * Applydiscountpercetagem;
                        DebitoDirecto.DebitoDirectoComissao = Math.Round(vlaor.GetValueOrDefault(), 2) ;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return DebitoDirecto;
        }

        private NewClient_Comissao CalculoNewClient(Comission comission, int? proposalID)
        {
            //double? NewClientComissao = 0;

            NewClient_Comissao _NewClient_Comissao = new NewClient_Comissao();
            _NewClient_Comissao.IsNewClient = false;
            
            try
            {
                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Proposal_Client _BB_Proposal_Client = db.BB_Proposal_Client.Where(x => x.ProposalID == proposalID).FirstOrDefault();

                    if(_BB_Proposal_Client != null && _BB_Proposal_Client.IsNewClient == true)
                    {
                        double? Applydiscount = 25;
                        double? Applydiscountpercetagem = Applydiscount / 100;
                        _NewClient_Comissao.Percentagem = Applydiscount;
                         double? valor = (comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault()) * Applydiscountpercetagem;
                        _NewClient_Comissao.NewClientComissao = Math.Round(valor.GetValueOrDefault(), 2);
                        _NewClient_Comissao.IsNewClient = true;
                        _NewClient_Comissao.ValueValor = comission.Hw_Printing.HW_PrintingComissao.GetValueOrDefault() + comission.ITS.ITSComissao.GetValueOrDefault() + comission.TFM.TFMComissao.GetValueOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return _NewClient_Comissao;
        }

        private TFM_Comissao CalculoTFM(ProposalRootObject ProposalObj)
        {
            double? vva = 0;
            double? VvaComissao = 0;

            TFM_Comissao tfm = new TFM_Comissao();

            try
            {
                ApprovedPrintingService activePS = null;
                if (ProposalObj.Draft.printingServices2.ActivePrintingService != null)
                {
                    activePS = ProposalObj.Draft.printingServices2.ApprovedPrintingServices[ProposalObj.Draft.printingServices2.ActivePrintingService.Value - 1];
                    if (activePS != null && activePS.GlobalClickVVA != null)
                    {

                        vva = activePS.GlobalClickVVA.PVP * activePS.ContractDuration;
                        
                        
                    }
                }

                double? ApplyDiscount = 1;
                double? ApplyDiscountPercentagem = ApplyDiscount  / 100;

                VvaComissao = Math.Round((vva * ApplyDiscountPercentagem).GetValueOrDefault(),2);

                if (VvaComissao >= 500)
                    VvaComissao = 500;

                tfm.TFMComissao = VvaComissao;
                tfm.Percentagem = ApplyDiscount;
                tfm.ValueValor = vva;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return tfm;
        }

        private ITS_Comissao CalculoITS(ProposalRootObject proposalObj)
        {
            double? _SumGP2_HW = 0;
            double? _SumGP2_BPO = 0;
            double? _SumGP2_SW = 0;

            double? _SumGP2_Total = 0;

            ITS_Comissao its = new ITS_Comissao();

            try
            {
                //Configurador ONE SHOT
                List<OsBasket> _BB_Proposal_Quote_HW = proposalObj.Draft.baskets.os_basket.Where(
                    x => x.Family == "PRSHW"
                    || x.Family == "MCSHW"
                    || x.Family == "IMSHW"
                    || x.Family == "VSSHW"
                    || x.Family == "BPOHW"
                    || x.Family == "OPSPSV"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_HW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_HW += item.GPTotal;
                }

              

                //Configurador Servicos Recorrnts
                List<RsBasket> _BB_Proposal_Quote_RS_HW = proposalObj.Draft.baskets.rs_basket.Where(
                    x => x.Family == "PRSHW"
                    || x.Family == "MCSHW"
                    || x.Family == "IMSHW"
                    || x.Family == "VSSHW"
                    || x.Family == "BPOHW"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_RS_HW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_HW += item.GPTotal;
                }

                its.HW = _SumGP2_HW;

                //Configurador ONE SHOT
                List<OsBasket> _BB_Proposal_Quote_SW = proposalObj.Draft.baskets.os_basket.Where(
                    x => x.Family == "PRSSW"
                    || x.Family == "MCSSW"
                    || x.Family == "IMSSW"
                    || x.Family == "VSSSW"
                    || x.Family == "BPOSW"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_SW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_SW += item.GPTotal;
                }



                //Configurador Servicos Recorrnts
                List<RsBasket> _BB_Proposal_Quote_RS_SW = proposalObj.Draft.baskets.rs_basket.Where(
                    x => x.Family == "PRSSW"
                    || x.Family == "MCSSW"
                    || x.Family == "IMSSW"
                    || x.Family == "VSSSW"
                    || x.Family == "BPOSW"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_RS_SW)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_SW += item.GPTotal;
                }

                its.SW = _SumGP2_SW;

                //Configurador BPO
                List<OsBasket> _BB_Proposal_Quote_BPO = proposalObj.Draft.baskets.os_basket.Where(
                    x => x.CodeRef == "IMS-MSV-BPO" || x.CodeRef == "MCS-MSV-BPO" || x.CodeRef == "BPS-MSV-BPO"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_BPO)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_BPO += item.GPTotal;
                }

                //Configurador BPO
                List<RsBasket> _BB_Proposal_Quote_BPO_RS = proposalObj.Draft.baskets.rs_basket.Where(
                    x => x.CodeRef == "IMS-MSV-BPO" || x.CodeRef == "MCS-MSV-BPO" || x.CodeRef == "BPS-MSV-BPO"
                    ).ToList();

                //Por causa dos GP2
                foreach (var item in _BB_Proposal_Quote_BPO_RS)
                {
                    if (item.GPTotal >= 0)
                        _SumGP2_BPO += item.GPTotal;
                }

                _SumGP2_Total = _SumGP2_BPO + _SumGP2_SW + _SumGP2_HW;

                double? ApplyDiscount = 6;

                double? ApplyDiscountPercetagem = ApplyDiscount / 100;

                its.ITSComissao = Math.Round((_SumGP2_Total * ApplyDiscountPercetagem).GetValueOrDefault(), 2);
                its.GP2 = _SumGP2_Total;
                its.Percetangem = ApplyDiscount;
                //Tecto maximo 6000
                if (its.ITSComissao >= 6000)
                    its.ITSComissao = 6000;

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return its;
        }

        public HW_Printing CalculoHWPrinting(ProposalRootObject proposalObj)
        {
            //double? HW_Printing = 0;
            double? PVPTotal = 0;
            double? ValorFinal_C_Desconto = 0;

            HW_Printing hw = new HW_Printing();

            ProposalBLL p1 = new ProposalBLL();
            LoadProposalInfo i = new LoadProposalInfo();
            i.ProposalId = proposalObj.Draft.details.ID;
            ActionResponse a = p1.LoadProposal(i);
            proposalObj = a.ProposalObj;


            try
            {
                double? _PercentagemDesconto = 0;
                List<OsBasket> _BB_Proposal_Quote = proposalObj.Draft.baskets.os_basket.Where(x => x.Family == "OPSHW" || x.Family == "PPHW").ToList();

                PVPTotal = _BB_Proposal_Quote.Sum(x => x.TotalPVP);
                ValorFinal_C_Desconto = _BB_Proposal_Quote.Sum(x => x.TotalNetsale);

                 using (var db = new BB_DB_DEVEntities2())
                {
                    //ValorFinal_C_Desconto = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalObj.Draft.details.ID && (x.Family == "OPSHW" || x.Family == "PPHW")).Select(x => x.TotalNetsale).Sum().GetValueOrDefault();
                    //PVPTotal = db.BB_Proposal_Quote.Where(x => x.Proposal_ID == proposalObj.Draft.details.ID && (x.Family == "OPSHW" || x.Family == "PPHW")).Select(x => x.TotalPVP).Sum().GetValueOrDefault();
                    //if (proposalObj.Draft.overvaluations.Count() > 0)
                    //{
                    //    ValorFinal_C_Desconto = ValorFinal_C_Desconto + proposalObj.Draft.overvaluations.Select(x => x.Total).FirstOrDefault();
                    //}

                

                if (ValorFinal_C_Desconto >= 0 && PVPTotal >= 0)
                {
                    _PercentagemDesconto = Math.Round(ValorFinal_C_Desconto.GetValueOrDefault() / PVPTotal.GetValueOrDefault(), 4);
                    _PercentagemDesconto = Math.Round((100 - (_PercentagemDesconto * 100)).GetValueOrDefault(), 2);
                }
              
                    List<BB_Commission> _lstBB_Commission = db.BB_Commission.ToList();
                    double? _ApplyDiscount = 0;

                    if (_PercentagemDesconto < 0)
                        _PercentagemDesconto = 0;

                    foreach (var item in _lstBB_Commission)
                    {
                        if (_PercentagemDesconto < item.Discount_End && _PercentagemDesconto >= item.Discount_Start)
                        {
                            _ApplyDiscount = item.HW_Printing;
                            hw.Percentagem = _ApplyDiscount;
                            _ApplyDiscount = _ApplyDiscount / 100;
                            break;
                        }
                    }
                    
                    //Calculate Comission
                    hw.HW_PrintingComissao = Math.Round((_ApplyDiscount * ValorFinal_C_Desconto).GetValueOrDefault(), 2);
                   
                    hw.ValueHardware = ValorFinal_C_Desconto;
                   

                    //Tecto maximo 5000
                    if (hw.HW_PrintingComissao >= 5000)
                        hw.HW_PrintingComissao = 5000;
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return hw;

        }
    }
}
