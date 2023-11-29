using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

using System.Web.Http;
using System.Web.Http.Results;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models.SLAs;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using Microsoft.Win32;
using System.ComponentModel;
using Microsoft.Exchange.WebServices.Data;

namespace WebApplication1.Controllers
{
    public class SLAsController : ApiController
    {
        [AcceptVerbs("GET", "POST")]
        [ActionName("GlobalSLAs")]
        public IHttpActionResult GlobalSLAs()
        {
            List<SLAsDetails> lst_Registos = new List<SLAsDetails>();
            List<SLAsDetails> DB_List = new List<SLAsDetails>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("get_BB_SLA_INFO_ALL", conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();


                    while (rdr.Read())
                    {
                        SLAsDetails p = new SLAsDetails();

                        p.ID = (int)rdr["Id"];
                        p.ProposalId = (int)rdr["ProposalID"];
                        p.InitialDate = rdr["CreatedDate"] != DBNull.Value ? (DateTime?)rdr["CreatedDate"] : null;
                        p.FinalDate = rdr["ModifiedDate"] != DBNull.Value ? (DateTime?)rdr["ModifiedDate"] : null;
                        p.Quote = rdr["QuoteNumber"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("QuoteNumber")) : "";
                        p.Client = rdr["Client"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Client")) : "";
                        p.AccountManager = rdr["CreatedBy"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("CreatedBy")) : "";
                        p.Status = rdr["State"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("State")) : "";
                        p.Category = rdr["Categoria"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("Categoria")) : "";
                        p.CategoriaID = (int)rdr["CategoriaID"];
                        p.Cor = rdr["OpColour"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("OpColour")) : "";
                        p.Name = rdr["ProposalName"] != DBNull.Value ? rdr.GetString(rdr.GetOrdinal("ProposalName")) : "";
                        p.TempoRealConsumido = rdr["OpResult"] != DBNull.Value ? (int)rdr["OpResult"] : 0;

                        //buscar todos os registos da BD
                        DB_List.Add(p);
                    }
                    rdr.Close();

                }

                do
                {
                    // "abater" registos anteriores no caso de categorias repetidas
                    List<SLAsDetails> regs_With_Same_QuoteNumber = DB_List.Where(x => x.Quote == DB_List[0].Quote).OrderByDescending(x => x.ID).ToList();

                    SLAsDetails sla = new SLAsDetails
                    {
                        ProposalId = regs_With_Same_QuoteNumber[0].ProposalId,
                        Quote = regs_With_Same_QuoteNumber[0].Quote,
                        Category = regs_With_Same_QuoteNumber[0].Category,
                        Name = regs_With_Same_QuoteNumber[0].Name,
                        Client = regs_With_Same_QuoteNumber[0].Client,
                        InitialDate = regs_With_Same_QuoteNumber.OrderBy(x => x.ID).First().InitialDate,
                        FinalDate = regs_With_Same_QuoteNumber[0].FinalDate,
                        AccountManager = regs_With_Same_QuoteNumber[0].AccountManager,
                    };

                    List<Categoria> lst_Categorias = new List<Categoria>();

                    //DateTime? contratoLanc = null;
                    DateTime? terminProcVendaCreated = null;
                    DateTime? terminProcVendaModified = null;

                    foreach (var item in regs_With_Same_QuoteNumber)
                    {
                        Categoria categoria = new Categoria()
                        {
                            Cor = item.Cor,
                            Name = item.Category,
                            CategoriaID = item.CategoriaID,
                            InitialDate = item.InitialDate,
                            FinalDate = item.FinalDate,
                            TempoRealConsumido = item.TempoRealConsumido,
                        };

                        //com a DB_List em orderDescending, abato as categorias "anteriores"
                        if (lst_Categorias.Where(c => c.CategoriaID == item.CategoriaID).FirstOrDefault() == null)
                        {
                            //if (categoria.CategoriaID == 11) contratoLanc = item.FinalDate;
                            if (categoria.CategoriaID == 12)
                            {
                                terminProcVendaCreated = item.InitialDate;
                                terminProcVendaModified = item.FinalDate;
                            }

                            lst_Categorias.Add(categoria);
                        }
                    }
                    sla.Categorias = lst_Categorias;

                    // #################################### PODERÁ VIR DA SP - A MELHORAR

                    using (var db = new masterEntities())
                    {
                    var email = regs_With_Same_QuoteNumber[0].AccountManager;

                    var aux = db.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
                        if(aux != null)
                        {
                            sla.DSO = aux.Location;
                            sla.AccountManager = aux.DisplayName;
                        }
                    }

                    // ############################################

                    // Duracao Total:
                    //  -> Diferenca entre o createdDate do terminar processo de venda e o modified da ultima categoria adicionada
                    if(lst_Categorias.LastOrDefault().FinalDate != null && terminProcVendaCreated.HasValue)
                    {
                        sla.DuracaoTotal = ((lst_Categorias.LastOrDefault().FinalDate).Value - terminProcVendaCreated.Value).Days;

                    }

                    // Duração Total após adjudicação:
                    if (lst_Categorias.FirstOrDefault().FinalDate != null && terminProcVendaModified.HasValue)
                    {
                        sla.DuracaoAdjuc = ((lst_Categorias.FirstOrDefault().FinalDate).Value - terminProcVendaModified.Value).Days;
                    }
                    else
                    {
                        sla.DuracaoAdjuc = -1;
                    }

                    lst_Registos.Add(sla);

                    //remover os registos que tenham a mesma quote
                    DB_List.RemoveAll(x => regs_With_Same_QuoteNumber.Contains(x));

                } while (DB_List.Count() != 0);

                return Ok(lst_Registos);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }


        // ##################################################################################
        // ##################################################################################
        // ##################################################################################


        [AcceptVerbs("GET", "POST")]
        [ActionName("GetSLADetails")]
        public IHttpActionResult GetSLADetails(string quoteNumber)
        {
            
            List<SLAsDetails> DB_List = new List<SLAsDetails>();
            SLAsDetails result = new SLAsDetails();
            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    SqlCommand cmd = new SqlCommand("get_BB_SLA_INFO_ALL", conn);
                    cmd.CommandTimeout = 180;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();

                    // iterate through results
                    while (rdr.Read())
                    {
                        SLAsDetails sla = new SLAsDetails();

                        sla.ID = (int)rdr["Id"];
                        sla.ProposalId = (int)rdr["ProposalID"];
                        sla.InitialDate = rdr["CreatedDate"] != DBNull.Value ? (DateTime?)rdr["CreatedDate"] : null;
                        sla.FinalDate = rdr["ModifiedDate"] != DBNull.Value ? (DateTime?)rdr["ModifiedDate"] : null;
                        sla.Quote = rdr["QuoteNumber"].ToString();
                        sla.Client = rdr["Client"].ToString();
                        sla.AccountManager = rdr["CreatedBy"].ToString();
                        sla.Status = rdr["State"].ToString();
                        sla.Category = rdr["Categoria"].ToString();
                        sla.CategoriaID = (int)rdr["CategoriaID"];
                        sla.Cor = rdr["OpColour"].ToString();
                        sla.Name = rdr["ProposalName"].ToString();
                        sla.SLA_To_Meet = 123456789;
                        sla.Responsable = rdr["ModifiedBy"].ToString();

                        //buscar todos os registos da BD
                        DB_List.Add(sla);
                    }
                    rdr.Close();


                    DB_List = DB_List.OrderBy(x => x.ID).ToList();

                    // query select de todos os registos da BD com o quoteNumber pretendido
                    List<SLAsDetails> regs_With_Same_QuoteNumber = DB_List.Where(x => x.Quote == quoteNumber).ToList();

                     // atribuicao de uma lista de estados à quoteNumber
                     List<Categoria> lst_Categorias = new List<Categoria>();

                    // dados a visualizar acima da tabela de estados
                    result.AccountManager = regs_With_Same_QuoteNumber.Select(x => x.AccountManager).FirstOrDefault();
                    result.Client = regs_With_Same_QuoteNumber.Select(x => x.Client).FirstOrDefault();
                    result.Name = regs_With_Same_QuoteNumber.Select(x => x.Name).FirstOrDefault();
                    result.Quote = regs_With_Same_QuoteNumber.Select(x => x.Quote).FirstOrDefault();
                    result.InitialDate = regs_With_Same_QuoteNumber.Select(d => d.InitialDate).FirstOrDefault();
                    result.FinalDate = regs_With_Same_QuoteNumber.Select(d => d.FinalDate).LastOrDefault();

                    //DateTime? contratoLanc = null;
                    DateTime? terminProcVendaCreated = null;
                    DateTime? terminProcVendaModified = null;

                    // criação de uma categoria (ESTADO) por cada registo da quoteNumber
                    foreach (var item in regs_With_Same_QuoteNumber)
                    {
                            Categoria categoria = new Categoria()
                            {
                                Name = item.Category,
                                SLA_To_Meet = item.SLA_To_Meet,
                                Cor = item.Cor,
                                InitialDate = item.InitialDate,
                                FinalDate = item.FinalDate,
                                CategoriaID = item.CategoriaID,
                                Responsable = item.Responsable,
                            };


                        //if (categoria.CategoriaID == 11) contratoLanc = item.InitialDate;
                        if (categoria.CategoriaID == 12)
                        {
                            terminProcVendaCreated = item.InitialDate;
                            terminProcVendaModified = item.FinalDate;
                        }

                        lst_Categorias.Add(categoria);
                        }

                    result.Categorias = lst_Categorias;

                    // Duracao Total apos adjudicacao:
                    //  -> Diferenca entre o modifiedDate do terminar processo BB e o modifiedDate da ultima categoria
                    if (lst_Categorias.LastOrDefault().FinalDate != null && terminProcVendaModified.HasValue)
                    {
                        result.DuracaoAdjuc = ((lst_Categorias.LastOrDefault().FinalDate).Value - terminProcVendaModified.Value).Days;
                    }
                    else
                    {
                        result.DuracaoAdjuc = -1;
                    }

                    // Duracao Total:
                    // -> Diferenca entre o createdDate do terminar processo BB e o modifiedDate da ultima categoria
                    if (terminProcVendaCreated.HasValue && lst_Categorias.FirstOrDefault().FinalDate != null)
                    {
                        result.DuracaoTotal = ((lst_Categorias.FirstOrDefault().FinalDate).Value - terminProcVendaCreated.Value).Days;

                    }

                    // COMENTARIOS ..............

                    List<BB_SLA_History> lstComments = new List<BB_SLA_History>();
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        lstComments = db.BB_SLA_History.Where(x => x.QuoteNumber == quoteNumber).ToList();
                    }
                    result.ListComentarios = lstComments;


                    //carregar as categorias pra o resutl
                    //resultPopulatCategorias = listagame da tabela categorias.

                    // result adiconar mais um propriedade do tipo obejcto Historico
                    // hisotrico listar todos as linhas em que tneham a quote.



                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("CreateSLAHistory")]
        public List<BB_SLA_History> CreateSLAHistory(string quoteNumber, int CategoriaID, string userLogged, string comentario)
        {
            try
            {
                List< BB_SLA_History > lstComentarios = new List<BB_SLA_History >();
                using (var db = new BB_DB_DEVEntities2())
                {
                    db.BB_SLA_History.Add(new BB_SLA_History()
                    {
                        QuoteNumber = quoteNumber,
                        CategoriaID = CategoriaID,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userLogged,
                        Comentario = comentario
                    });

                db.SaveChanges();

                    lstComentarios = db.BB_SLA_History.Where(q => q.QuoteNumber == quoteNumber).ToList();
                }

                return lstComentarios;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }

        }




        // HELPERS ************************************************************************

        public string GetSLAColor(int? estimated, int? used)
        {
            if(used == null)
            {
                return "";
            }

            if(estimated != null && estimated != null)
            {
                if (used / estimated > 1)
                {
                    return "red";
                }
                else if (used / estimated > 0.75 && used / estimated <= 1)
                {
                    return "yellow";
                }
                else
                {
                    return "green";
                }

            }
            else
            {
                return "";
            }
        }



    }

}
