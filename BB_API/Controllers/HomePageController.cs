using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomePageController : ApiController
    {
        public masterEntities usersDB = new masterEntities();
        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_CRM_Oportunity_Status")]
        public HttpResponseMessage sp_Get_CRM_Oportunity_Status([FromBody] Data data)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";
                eventLog.WriteEntry("sp_Get_CRM_Oportunity_Status", EventLogEntryType.Information, 101, 1);
            }
            HomepageDasboards h = new HomepageDasboards();
            List<Model_sp_Get_CRM_Oportunity_Status> lst = new List<Model_sp_Get_CRM_Oportunity_Status>();
            var username = usersDB.AspNetUsers.Where(x => x.UserName == data.username).FirstOrDefault();
            lst = h.DashBoard_Homepage(username.DisplayName);

            return Request.CreateResponse<List<Model_sp_Get_CRM_Oportunity_Status>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_CRM_UserStatus")]
        public HttpResponseMessage sp_Get_CRM_UserStatus([FromBody] Data data)
        {
            List<Model_sp_Get_CRM_UserStatus> lst = new List<Model_sp_Get_CRM_UserStatus>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_CRM_UserStatus", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_CRM_UserStatus m = new Model_sp_Get_CRM_UserStatus();
                        m.AccountNumber = rdr["AccountNumber"].ToString();
                        m.Cliente = rdr["Cliente"].ToString();
                        m.Leads = rdr["Leads"].ToString();
                        m.Won = rdr["Won"].ToString();
                        m.AmountTotal = rdr["AmountTotal"].ToString();
                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_CRM_UserStatus>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetDocSignnGeral")]
        public HttpResponseMessage GetDocSignnGeral([FromBody] Data data)
        {
            List<Model_sp_Get_DocSign> lst = new List<Model_sp_Get_DocSign>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_DocSignOverview", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_DocSign m = new Model_sp_Get_DocSign();
                        m.quote = rdr["quote"].ToString();
                        m.CreatedBy = rdr["CreatedBy"].ToString();
                        m.CreatedTime = rdr["CreatedTime"] != null && rdr["CreatedTime"].ToString() != "" ? DateTime.Parse(rdr["CreatedTime"].ToString()) : new DateTime();
                        m.ModifiedBy = rdr["ModifiedBy"].ToString();
                        m.ModifiedTime = rdr["ModifiedTime"] != null && rdr["ModifiedTime"].ToString() != "" ? DateTime.Parse(rdr["ModifiedTime"].ToString()) : new DateTime();
                        m.DocSignDateTimeSent = rdr["DocSignDateTimeSent"] != null && rdr["DocSignDateTimeSent"].ToString() != "" ? DateTime.Parse(rdr["DocSignDateTimeSent"].ToString()) : new DateTime();
                        m.Message1 = rdr["Message1"].ToString();
                        m.Status = rdr["Status"].ToString();
                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            lst = lst.OrderByDescending(x => x.CreatedTime).ToList();
            return Request.CreateResponse<List<Model_sp_Get_DocSign>>(HttpStatusCode.OK, lst);
        }


        //[AcceptVerbs("GET", "POST")]
        //[ActionName("GetDocSignnGeral")]
        //public HttpResponseMessage GetDocSignnGeral([FromBody] Data data)
        //{
        //    List<Model_sp_Get_DocSign> lst = new List<Model_sp_Get_DocSign>();
        //    AspNetUsers username = new AspNetUsers();

        //    using (var db = new masterEntities())
        //    {
        //        username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

        //    }

        //    try
        //    {
        //        string bdConnect = @AppSettingsGet.BasedadosConnect;
        //        using (SqlConnection conn = new SqlConnection(bdConnect))
        //        {

        //            conn.Open();

        //            // 1.  create a command object identifying the stored procedure
        //            SqlCommand cmd = new SqlCommand("get_DocSignOverview", conn);
        //            cmd.CommandTimeout = 180;
        //            // 2. set the command object so it knows to execute a stored procedure
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            //cmd.Parameters.AddWithValue("@userid", username.DisplayName);
        //            // 3. add parameter to command, which will be passed to the stored procedure
        //            //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

        //            // execute the command

        //            SqlDataReader rdr = cmd.ExecuteReader();


        //            // iterate through results, printing each to console
        //            while (rdr.Read())
        //            {
        //                Model_sp_Get_DocSign m = new Model_sp_Get_DocSign();
        //                m.quote = rdr["quote"].ToString();
        //                m.CreatedBy = rdr["CreatedBy"].ToString();
        //                m.CreatedTime = rdr["CreatedTime"] != null && rdr["CreatedTime"].ToString() != "" ? DateTime.Parse(rdr["CreatedTime"].ToString()) : new DateTime();
        //                m.ModifiedBy = rdr["ModifiedBy"].ToString();
        //                m.ModifiedTime = rdr["ModifiedTime"] != null && rdr["ModifiedTime"].ToString() != "" ? DateTime.Parse(rdr["ModifiedTime"].ToString()) : new DateTime();
        //                m.DocSignDateTimeSent = rdr["DocSignDateTimeSent"] != null && rdr["DocSignDateTimeSent"].ToString() != "" ? DateTime.Parse(rdr["DocSignDateTimeSent"].ToString()) : new DateTime();
        //                m.Message1 = rdr["Message1"].ToString();
        //                m.Status = rdr["Status"].ToString();
        //                lst.Add(m);
        //            }

        //            rdr.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Message.ToString();
        //    }

        //    lst = lst.OrderByDescending(x => x.CreatedTime).ToList();
        //    return Request.CreateResponse<List<Model_sp_Get_DocSign>>(HttpStatusCode.OK, lst);
        //}

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_BusinessBuilder_LD_Totais")]
        public HttpResponseMessage sp_Get_BusinessBuilder_LD_Totais([FromBody] Data data)
        {
            List<Model_sp_Get_BusinessBuilder_LD_Totais> lst = new List<Model_sp_Get_BusinessBuilder_LD_Totais>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_BusinessBuilder_LD_Totais", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_BusinessBuilder_LD_Totais m = new Model_sp_Get_BusinessBuilder_LD_Totais();
                        m.QuoteNumber = rdr["QuoteNumber"].ToString();
                        m.CriadoEmBB = rdr["CriadoEmBB"].ToString();
                        m.Description = rdr["Description"].ToString();
                        m.StatusCRM1 = rdr["StatusCRM1"].ToString();
                        m.ValueTotal = rdr["ValueTotal"].ToString();
                        m.CreatedBy = rdr["CreatedBy"].ToString();
                        m.Location = rdr["Location"].ToString();
                        m.CriadoemLD = rdr["CriadoemLD"].ToString();
                        m.Fechado = rdr["Fechado"].ToString();
                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_BusinessBuilder_LD_Totais>>(HttpStatusCode.OK, lst);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_CRM_UserStatus_Totais")]
        public HttpResponseMessage sp_Get_CRM_UserStatus_Totais([FromBody] Data data)
        {
            List<Model_sp_Get_CRM_UserStatus_Totais> lst = new List<Model_sp_Get_CRM_UserStatus_Totais>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_CRM_UserStatus_Totais", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_CRM_UserStatus_Totais m = new Model_sp_Get_CRM_UserStatus_Totais();
                        m.Lead = rdr["Lead"].ToString();
                        m.Won = rdr["Won"].ToString();
                        m.AmountTotal = rdr["AmountTotal"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_CRM_UserStatus_Totais>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_GetBusinessBuilder_Totais")]
        public HttpResponseMessage sp_GetBusinessBuilder_Totais([FromBody] Data data)
        {
            List<Model_sp_GetBusinessBuilder_Totais> lst = new List<Model_sp_GetBusinessBuilder_Totais>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_GetBusinessBuilder_Totais", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_GetBusinessBuilder_Totais m = new Model_sp_GetBusinessBuilder_Totais();
                        m.Leads = rdr["Leads"].ToString();
                        m.ValueTotal = rdr["ValueTotal"].ToString();
                        m.ValueComission = rdr["ValueComission"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_GetBusinessBuilder_Totais>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_GetLeaseDesk_Totais")]
        public HttpResponseMessage sp_GetLeaseDesk_Totais([FromBody] Data data)
        {
            List<Model_sp_GetLeaseDesk_Totais> lst = new List<Model_sp_GetLeaseDesk_Totais>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_GetLeaseDesk_Totais", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_GetLeaseDesk_Totais m = new Model_sp_GetLeaseDesk_Totais();
                        m.Rascunho = rdr["Rascunho"].ToString();
                        m.RascunhoAcumulado = rdr["RascunhoAcumulado"].ToString();
                        m.contractosFechado = rdr["contractosFechado"].ToString();
                        m.contractosFechadoAcumulado = rdr["contractosFechadoAcumulado"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_GetLeaseDesk_Totais>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_Proposals_State")]
        public HttpResponseMessage sp_Get_Proposals_State([FromBody] Data data)
        {
            List<Model_sp_Get_Proposals_State> lst = new List<Model_sp_Get_Proposals_State>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_Proposals_State", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_Proposals_State m = new Model_sp_Get_Proposals_State();
                        m.Estado = rdr["Estado"].ToString();
                        m.EstadoCount = rdr["EstadoCount"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_Proposals_State>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_Proposals_StateDSO")]
        public HttpResponseMessage sp_Get_Proposals_StateDSO([FromBody] Data data)
        {
            List<Model_sp_Get_Proposals_StateDSO> lst = new List<Model_sp_Get_Proposals_StateDSO>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_Proposals_StateDSO", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_Proposals_StateDSO m = new Model_sp_Get_Proposals_StateDSO();
                        m.Estado = rdr["Location"].ToString();
                        m.EstadoCount = rdr["LocationCount"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_Proposals_StateDSO>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_Proposals_StateDAY")]
        public HttpResponseMessage sp_Get_Proposals_StateDAY([FromBody] Data data)
        {
            List<Model_sp_Get_Proposals_DAY> lst = new List<Model_sp_Get_Proposals_DAY>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_Proposals_StateDAY", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_Proposals_DAY m = new Model_sp_Get_Proposals_DAY();
                        m.Estado = rdr["Dia"].ToString();
                        m.EstadoCount = rdr["DiaCount"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_Proposals_DAY>>(HttpStatusCode.OK, lst);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_Get_Proposals_StateMonthy")]
        public HttpResponseMessage sp_Get_Proposals_StateMonthy([FromBody] Data data)
        {
            List<Model_sp_Get_Proposals_Mes> lst = new List<Model_sp_Get_Proposals_Mes>();
            AspNetUsers username = new AspNetUsers();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();

            }

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_Proposals_StateMonthy", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", username.DisplayName);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Model_sp_Get_Proposals_Mes m = new Model_sp_Get_Proposals_Mes();
                        m.Estado = rdr["Mes"].ToString();
                        m.EstadoCount = rdr["MesContador"].ToString();

                        lst.Add(m);
                    }

                    rdr.Close();
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<List<Model_sp_Get_Proposals_Mes>>(HttpStatusCode.OK, lst);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("sp_GetStatusBB")]
        public HttpResponseMessage sp_GetStatusBB([FromBody] Data data)
        {
            Model_sp_GetStatusBB GetStatusBB = new Model_sp_GetStatusBB();
            AspNetUsers username = new AspNetUsers();
            List<BB_Proposal> lst = new List<BB_Proposal>();

            using (var db = new masterEntities())
            {
                username = db.AspNetUsers.Where(x => x.Email == data.username).FirstOrDefault();
            }

            try
            {
                if (data.username == "Jorge.Pimentel@konicaminolta.pt")
                {
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        GetStatusBB.statusDRAFT = db.BB_Proposal.Where(x => x.StatusID == 1).Count();
                        GetStatusBB.statusCRM_SUB = db.BB_Proposal.Where(x => x.StatusID == 7).Count();
                        GetStatusBB.statusCOT_ATR = db.BB_Proposal.Where(x => x.StatusID == 8).Count();
                        GetStatusBB.statusPRAZOD_ATRR = db.BB_Proposal.Where(x => x.StatusID == 10).Count();
                        GetStatusBB.statusPRAZOD_REQ = db.BB_Proposal.Where(x => x.StatusID == 9).Count();
                    }
                }

                else
                {
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        GetStatusBB.statusDRAFT = db.BB_Proposal.Where(x => x.CreatedBy == data.username && x.ToDelete == false && x.StatusID == 1).Count();
                        GetStatusBB.statusCRM_SUB = db.BB_Proposal.Where(x => x.CreatedBy == data.username && x.ToDelete == false && x.StatusID == 7).Count();
                        GetStatusBB.statusCOT_ATR = db.BB_Proposal.Where(x => x.CreatedBy == data.username && x.ToDelete == false && x.StatusID == 8).Count();
                        GetStatusBB.statusPRAZOD_ATRR = db.BB_Proposal.Where(x => x.CreatedBy == data.username && x.ToDelete == false && x.StatusID == 10).Count();
                        GetStatusBB.statusPRAZOD_REQ = db.BB_Proposal.Where(x => x.CreatedBy == data.username && x.ToDelete == false && x.StatusID == 9).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Request.CreateResponse<Model_sp_GetStatusBB>(HttpStatusCode.OK, GetStatusBB);
        }

    }

    public class Model_sp_GetStatusBB
    {
        public int statusDRAFT { get; set; }
        public int statusCOLLABE { get; set; }
        public int statusCRM_SUB { get; set; }
        public int statusCOT_ATR { get; set; }
        public int statusPRAZOD_REQ { get; set; }
        public int statusPRAZOD_ATRR { get; set; }
    }

    public class Data
    {
        public string username { get; set; }
    }

    public class Model_sp_Get_CRM_UserStatus
    {
        public string AccountNumber { get; set; }
        public string Cliente { get; set; }
        public string Leads { get; set; }
        public string Won { get; set; }
        public string AmountTotal { get; set; }

    }

    public class Model_sp_Get_BusinessBuilder_LD_Totais
    {
        public string QuoteNumber { get; set; }
        public string CriadoEmBB { get; set; }
        public string Description { get; set; }
        public string StatusCRM1 { get; set; }
        public string ValueTotal { get; set; }
        public string CreatedBy { get; set; }
        public string Location { get; set; }
        public string CriadoemLD { get; set; }
        public string Fechado { get; set; }

    }

    public class Model_sp_Get_CRM_UserStatus_Totais
    {
        public string Lead { get; set; }
        public string Won { get; set; }
        public string AmountTotal { get; set; }
    }

    public class Model_sp_GetBusinessBuilder_Totais
    {
        public string Leads { get; set; }
        public string ValueTotal { get; set; }
        public string ValueComission { get; set; }
    }
    public class Model_sp_GetLeaseDesk_Totais
    {
        public string Rascunho { get; set; }
        public string RascunhoAcumulado { get; set; }
        public string contractosFechado { get; set; }
        public string contractosFechadoAcumulado { get; set; }
    }

    public class Model_sp_Get_Proposals_State
    {
        public string Estado { get; set; }
        public string EstadoCount { get; set; }
    }

    public class Model_sp_Get_Proposals_StateDSO
    {
        public string Estado { get; set; }
        public string EstadoCount { get; set; }
    }

    public class Model_sp_Get_Proposals_Mes
    {
        public string Estado { get; set; }
        public string EstadoCount { get; set; }
    }

    public class Model_sp_Get_Proposals_DAY
    {
        public string Estado { get; set; }
        public string EstadoCount { get; set; }
    }

    public class Model_sp_Get_DocSign
    {
        public string quote { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }

        public string Status { get; set; }

        public DateTime? DocSignDateTimeSent { get; set; }

        public string Message1 { get; set; }

    }
}

