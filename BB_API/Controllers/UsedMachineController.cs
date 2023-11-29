using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.BLL;
using WebApplication1.Models;
using static WebApplication1.Models.UsedMachineModel;

namespace WebApplication1.Controllers
{
    public class UsedMachineController : ApiController
    {

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetUsedMachinePedidos")]
        public IHttpActionResult GetUsedMachinePedidos(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Pedidos_> listModel = new List<BB_Maquinas_Usadas_Pedidos_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_UsedMachinePedidos", conn);
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        BB_Maquinas_Usadas_Pedidos_ m = new BB_Maquinas_Usadas_Pedidos_();
                        m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                        m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                        m.ReservedBy = rdr["ReservedBy"].ToString();
                        m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                        m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                        m.PS_NUS = rdr["PS_NUS"].ToString();
                        m.Codref = rdr["Codref"].ToString();
                        m.Type_MFP = rdr["Type_MFP"].ToString();
                        m.NrSerie = rdr["NrSerie"].ToString();
                        m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                        m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                        m.IsReserved = rdr["IsReserved"] != null ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                        m.Modelo = rdr["Modelo"].ToString();
                        m.Comentarios = rdr["Comentarios"].ToString();
                        m.Status = rdr["Status"] != null && rdr["Status"].ToString() != "" ? (int)rdr["Status"] : 0;
                        listModel.Add(m);
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetUsedMachineReserved")]
        public IHttpActionResult GetUsedMachineReserved(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Gestor_> listModel = new List<BB_Maquinas_Usadas_Gestor_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_GetUsedMachineReservedByEmail", conn);
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        BB_Maquinas_Usadas_Gestor_ m = new BB_Maquinas_Usadas_Gestor_();
                        m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                        m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                        m.ReservedBy = rdr["ReservedBy"].ToString();
                        m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                        m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                        m.PS_NUS = rdr["PS_NUS"].ToString();
                        m.Codref = rdr["Codref"].ToString();
                        m.Type_MFP = rdr["Type_MFP"].ToString();
                        m.NrSerie = rdr["NrSerie"].ToString();
                        m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                        m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                        m.IsReserved = rdr["IsReserved"] != null ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                        m.Modelo = rdr["Modelo"].ToString();
                        m.Comentarios = rdr["Comentarios"].ToString();
                        listModel.Add(m);
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetUsedMachineReservedLogistica")]
        public IHttpActionResult GetUsedMachineReservedLogistica(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Logistica_> listModel = new List<BB_Maquinas_Usadas_Logistica_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_GetUsedMachineReservedLogisticaByEmail", conn);
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        BB_Maquinas_Usadas_Logistica_ m = new BB_Maquinas_Usadas_Logistica_();
                        m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                        m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                        m.ReservedBy = rdr["ReservedBy"].ToString();
                        m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                        m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                        m.PS_NUS = rdr["PS_NUS"].ToString();
                        m.Codref = rdr["Codref"].ToString();
                        m.Type_MFP = rdr["Type_MFP"].ToString();
                        m.NrSerie = rdr["NrSerie"].ToString();
                        m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                        m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                        m.IsReserved = rdr["IsReserved"] != null ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                        m.Modelo = rdr["Modelo"].ToString();
                        m.Comentarios = rdr["Comentarios"].ToString();
                        listModel.Add(m);
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetUsedMachineReservedTecnico")]
        public IHttpActionResult GetUsedMachineReservedTecnico(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Tecnico_> listModel = new List<BB_Maquinas_Usadas_Tecnico_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_GetUsedMachineReservedTecnicoByEmail", conn);
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        BB_Maquinas_Usadas_Tecnico_ m = new BB_Maquinas_Usadas_Tecnico_();
                        m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                        m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                        m.ReservedBy = rdr["ReservedBy"].ToString();
                        m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                        m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                        m.PS_NUS = rdr["PS_NUS"].ToString();
                        m.Codref = rdr["Codref"].ToString();
                        m.Type_MFP = rdr["Type_MFP"].ToString();
                        m.NrSerie = rdr["NrSerie"].ToString();
                        m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                        m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                        m.IsReserved = rdr["IsReserved"] != null ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                        m.Modelo = rdr["Modelo"].ToString();
                        m.Comentarios = rdr["Comentarios"].ToString();
                        m.Status = rdr["Status"] != null && rdr["Status"].ToString() != "" ? (int)rdr["Status"] : 0;
                        m.Leasedesk = rdr["Leasedesk"] != null && rdr["Leasedesk"].ToString() != "" ? Boolean.Parse(rdr["Leasedesk"].ToString()) : false;
                        m.ModifiedDate = rdr["ModifiedDate"] != null && rdr["ModifiedDate"].ToString() != "" ? DateTime.Parse(rdr["ModifiedDate"].ToString()) : new DateTime();
                        m.RequestTo = rdr["RequestTo"].ToString();
                        listModel.Add(m);
                    }

                    rdr.Close();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UsedMachineToReserved")]
        public IHttpActionResult UsedMachineToReserved(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Gestor_> listModel = new List<BB_Maquinas_Usadas_Gestor_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_UsedMachineToReserved", conn);
                    //cmd.Parameters.Add("@email", SqlDbType.NVarChar, 20).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    try
                    {
                        while (rdr.Read())
                        {
                            BB_Maquinas_Usadas_Gestor_ m = new BB_Maquinas_Usadas_Gestor_();
                            m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                            m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                            m.ReservedBy = rdr["ReservedBy"].ToString();
                            m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                            m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                            m.PS_NUS = rdr["PS_NUS"].ToString();
                            m.Codref = rdr["Codref"].ToString();
                            m.Type_MFP = rdr["Type_MFP"].ToString();
                            m.NrSerie = rdr["NrSerie"].ToString();
                            m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                            m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                            m.IsReserved = rdr["IsReserved"] != null && rdr["IsReserved"].ToString() != "" ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                            m.Modelo = rdr["Modelo"].ToString();
                            m.Comentarios = rdr["Comentarios"].ToString();
                            m.ClienteNr = rdr["ClienteNr"].ToString();
                            listModel.Add(m);
                        }

                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UsedMachineToReserved_Logisitca")]
        public IHttpActionResult UsedMachineToReserved_Logisitca(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Gestor_> listModel = new List<BB_Maquinas_Usadas_Gestor_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_UsedMachineToReserved_Logistica", conn);
                    //cmd.Parameters.Add("@email", SqlDbType.NVarChar, 20).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    try
                    {
                        while (rdr.Read())
                        {
                            BB_Maquinas_Usadas_Gestor_ m = new BB_Maquinas_Usadas_Gestor_();
                            m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                            m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                            m.ReservedBy = rdr["ReservedBy"].ToString();
                            m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                            m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                            m.PS_NUS = rdr["PS_NUS"].ToString();
                            m.Codref = rdr["Codref"].ToString();
                            m.Type_MFP = rdr["Type_MFP"].ToString();
                            m.NrSerie = rdr["NrSerie"].ToString();
                            m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                            m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                            m.IsReserved = rdr["IsReserved"] != null && rdr["IsReserved"].ToString() != "" ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                            m.Modelo = rdr["Modelo"].ToString();
                            m.Comentarios = rdr["Comentarios"].ToString();
                            m.ClienteNr = rdr["ClienteNr"].ToString();
                            listModel.Add(m);
                        }

                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetLogisticaIndex")]
        public IHttpActionResult GetLogisticaIndex(ProfileUser userName)
        {
            List<BB_Maquinas_Usadas_Model_> listModel = new List<BB_Maquinas_Usadas_Model_>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_LogisticaIndex", conn);
                    cmd.Parameters.Add("@email", SqlDbType.NVarChar, 20).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        BB_Maquinas_Usadas_Model_ m = new BB_Maquinas_Usadas_Model_();
                        try
                        {
                            m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                            m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                            m.ReservedBy = rdr["ReservedBy_Gestor"].ToString();
                            //m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                            //m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                            m.PS_NUS = rdr["PS_NUS"].ToString();
                            m.Codref = rdr["Codref"].ToString();
                            m.Type_MFP = rdr["Type_MFP"].ToString();
                            m.NrSerie = rdr["NrSerie"].ToString();
                            m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                            m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                            //m.IsReserved = rdr["IsReserved"] != null ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                            m.Modelo = rdr["Modelo"].ToString();
                            //m.Comentarios = rdr["Comentarios"].ToString();
                            m.PriceUsed = rdr["PriceUsed"] != null && rdr["PriceUsed"].ToString() != "" ? (double)rdr["PriceUsed"] : 0;
                            //m.IsReserved_Gestor = rdr["IsReserved_Gestor"] != null && rdr["IsReserved_Gestor"].ToString() != "" ? Boolean.Parse(rdr["IsReserved_Gestor"].ToString()) : false;
                            m.IsReserved_Gestor = rdr["IsReserved_Gestor"] != null && rdr["IsReserved_Gestor"].ToString() != "" ? true : false;
                            m.ExpireDate_Gestor = rdr["ExpireDate_Gestor"] != null && rdr["ExpireDate_Gestor"].ToString() != "" ? DateTime.Parse(rdr["ExpireDate_Gestor"].ToString()) : new DateTime();
                            m.ReservedBy_Tecnico = rdr["ReservedBy_Tecnico"].ToString();
                            m.IsReserved_Tecnico = rdr["IsReserved_Tecnico"] != null && rdr["IsReserved_Tecnico"].ToString() != "" ? Boolean.Parse(rdr["IsReserved_Tecnico"].ToString()) : false;
                            m.ExpireDate_Tecnico = rdr["ExpireDate_Tecnico"] != null && rdr["ExpireDate_Tecnico"].ToString() != "" ? DateTime.Parse(rdr["ExpireDate_Tecnico"].ToString()) : new DateTime();
                            m.ClientNr_Tecnico = rdr["ClientNr_Tecnico"].ToString();
                            m.ClienteNome = rdr["ClientNr_Gestor"].ToString();
                            m.ReservedDateTime_Gestor = rdr["ReservedDateTime_Gestor"] != null && rdr["ReservedDateTime_Gestor"].ToString() != "" ? DateTime.Parse(rdr["ReservedDateTime_Gestor"].ToString()) : new DateTime();
                            m.ReservedDateTime_Tecnico = rdr["ReservedDateTime_Tecnico"] != null && rdr["ReservedDateTime_Tecnico"].ToString() != "" ? DateTime.Parse(rdr["ReservedDateTime_Tecnico"].ToString()) : new DateTime();
                            listModel.Add(m);
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

            return Ok(listModel);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("React_UsedMachineToReserved")]
        public List<BB_Maquinas_Usadas_Gestor_Modelo> React_UsedMachineToReserved()
        {
            List<BB_Maquinas_Usadas_Gestor_Modelo> listModel = new List<BB_Maquinas_Usadas_Gestor_Modelo>();

            try
            {
                string bdConnect = @AppSettingsGet.BasedadosConnect;
                using (SqlConnection conn = new SqlConnection(bdConnect))
                {

                    conn.Open();
                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("get_UsedMachineToReserved", conn);
                    //cmd.Parameters.Add("@email", SqlDbType.NVarChar, 20).Value = userName.Username;
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@userEmail", Owner.Owner);
                    SqlDataReader rdr = cmd.ExecuteReader();


                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        try
                        {


                            BB_Maquinas_Usadas_Gestor_Modelo m = new BB_Maquinas_Usadas_Gestor_Modelo();
                            m.ID = rdr["ID"] != null ? (int)rdr["ID"] : 0;
                            m.ProposalID = rdr["ProposalID"] != null && rdr["ProposalID"].ToString() != "" ? (int)rdr["ProposalID"] : 0;
                            m.ReservedBy = rdr["ReservedBy"].ToString();
                            m.ReservedDateTime = rdr["ReservedDateTime"] != null ? DateTime.Parse(rdr["ReservedDateTime"].ToString()) : new DateTime();
                            m.ExpireDate = rdr["ExpireDate"] != null ? DateTime.Parse(rdr["ExpireDate"].ToString()) : new DateTime();
                            m.PS_NUS = rdr["PS_NUS"].ToString();
                            m.Codref = rdr["Codref"].ToString();
                            m.Type_MFP = rdr["Type_MFP"].ToString();
                            m.NrSerie = rdr["NrSerie"].ToString();
                            m.NrCopyBW = rdr["NrCopyBW"] != null && rdr["NrCopyBW"].ToString() != "" ? (int)rdr["NrCopyBW"] : 0;
                            m.NrCopyCOLOR = rdr["NrCopyCOLOR"] != null && rdr["NrCopyCOLOR"].ToString() != "" ? (int)rdr["NrCopyCOLOR"] : 0;
                            m.IsReserved = rdr["IsReserved"] != null && rdr["IsReserved"].ToString() != "" ? Boolean.Parse(rdr["IsReserved"].ToString()) : false;
                            m.Modelo = rdr["Modelo"].ToString();
                            m.PriceUsed = rdr["PriceUsed"] != null && rdr["PriceUsed"].ToString() != "" ? (double)rdr["PriceUsed"] : 0;
                            listModel.Add(m);
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

            return listModel;
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UsedMachineReservedDetail")]
        public IHttpActionResult UsedMachineReservedDetail(string serie)
        {
            BB_Maquinas_Usadas_Gestor_Detail a = new BB_Maquinas_Usadas_Gestor_Detail();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas_Gestor _BB_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Gestor != null)
                    {
                        BB_Clientes b = db.BB_Clientes.Where(x => x.accountnumber == _BB_Maquinas_Usadas_Gestor.ClienteNr).FirstOrDefault();
                        a.Codref = _BB_Maquinas_Usadas_Gestor.Codref;
                        a.Type_MFP = _BB_Maquinas_Usadas_Gestor.Type_MFP;
                        a.PS_NUS = _BB_Maquinas_Usadas_Gestor.PS_NUS;

                        a.ExpireDate = _BB_Maquinas_Usadas_Gestor.ExpireDate;
                        a.IsReserved = _BB_Maquinas_Usadas_Gestor.IsReserved;
                        a.ReservedDateTime = _BB_Maquinas_Usadas_Gestor.ReservedDateTime;
                        a.ReservedBy = _BB_Maquinas_Usadas_Gestor.ReservedBy;
                        a.ClienteNome = b != null ? b.Name : "";
                        a.ClienteNr = b != null ? b.accountnumber : "";
                        a.Comentarios = _BB_Maquinas_Usadas_Gestor.Comentarios;

                    }

                    if (_Maquinas_Usadas != null)
                    {
                        a.DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.DVCTRegistada = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.CatCancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes = _Maquinas_Usadas.Observacoes_Logistica;
                        a.NrSerie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.NrCopyBW = _Maquinas_Usadas.Contador_Preto;
                        a.NrCopyCOLOR = _Maquinas_Usadas.Contador_Cor;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.PS_NUS = _Maquinas_Usadas.NUS_PS;
                        a.Modelo = _Maquinas_Usadas.NUS_Modelo;
                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("React_MaquinasUsadas_Save")]
        public HttpResponseMessage React_MaquinasUsadas_Save(PostData postData)
        {
            try
            {
                BB_Maquinas_Usadas u = new BB_Maquinas_Usadas();

                using (var db = new BB_DB_DEVEntities2())
                {
                    u = db.BB_Maquinas_Usadas.Where(x => x.NUS_PS == postData.PS_NUS).FirstOrDefault();

                    BB_Maquinas_Usadas_Gestor updateUsedMachine = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == postData.NrSerie).FirstOrDefault();

                    if (updateUsedMachine == null)
                    {
                        BB_Maquinas_Usadas_Gestor ug = new BB_Maquinas_Usadas_Gestor();
                        ug.Codref = u.NUS_Referencia;
                        ug.Type_MFP = u.NUS_Tipo;
                        ug.PS_NUS = u.NUS_PS;
                        ug.NrSerie = u.NUS_Nr_Serie;
                        ug.NrCopyBW = u.Contador_Preto;
                        ug.NrCopyCOLOR = u.Contador_Cor;
                        ug.ExpireDate = postData.ExpireDate;
                        //ug.IsReserved = postData.IsReserved;
                        ug.IsReserved = false;
                        ug.ReservedDateTime = postData.ReservedDateTime;
                        ug.Comentarios = postData.Comentarios;
                        ug.ClienteNr = postData.ClientNr;
                        ug.ReservedBy = postData.ReservedBy;
                        //ug.PriceUsed = 100;
                        //ug.ProposalID = int.Parse(postData.ProposalID);
                        db.BB_Maquinas_Usadas_Gestor.Add(ug);
                        db.SaveChanges();
                    }
                    else
                    {
                        updateUsedMachine.ExpireDate = postData.ExpireDate;
                        db.Entry(updateUsedMachine).State = EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                ActionResponse ac = new ActionResponse();
                try
                {
                    ProposalBLL pBLL = new ProposalBLL();
                    LoadProposalInfo lpi = new LoadProposalInfo();
                    lpi.ProposalId = int.Parse(postData.ProposalID.ToString());
                    ac = pBLL.LoadProposal(lpi);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }



                ac.Message = "Reserva efetuada com sucesso!";
                return Request.CreateResponse(HttpStatusCode.OK, ac);
            }
            catch (Exception ex)

            {
                throw new ActionFailResponse("Nao gravou")
                {
                    StatusCode = 501
                };
            }
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("React_MaquinasUsadas_Cancelar")]
        public HttpResponseMessage React_MaquinasUsadas_Cancelar(PostData postData)
        {
            ActionResponse ac = new ActionResponse();
            try
            {

                BB_Maquinas_Usadas_Gestor _BB_Maquinas_Usadas_Gestor = new BB_Maquinas_Usadas_Gestor();

                using (var db = new BB_DB_DEVEntities2())
                {

                    _BB_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == postData.NrSerie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Gestor != null)
                    {
                        db.BB_Maquinas_Usadas_Gestor.Remove(_BB_Maquinas_Usadas_Gestor);
                        db.SaveChanges();
                    }

                    ProposalBLL pBLL = new ProposalBLL();
                    LoadProposalInfo lpi = new LoadProposalInfo();
                    lpi.ProposalId = int.Parse(postData.ProposalID.ToString());
                    ac = pBLL.LoadProposal(lpi);

                    ac.Message = "Reserva Cancelada";
                    return Request.CreateResponse(HttpStatusCode.OK, ac);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ac);
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UsedMachineReservedPedidoDetail")]
        public IHttpActionResult UsedMachineReservedPedidoDetail(string psnus, int? id, string nrSerie)
        {
            BB_Maquinas_Usadas_Pedidos_ a = new BB_Maquinas_Usadas_Pedidos_();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.PS_NUS == psnus).FirstOrDefault();
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_PS == psnus).FirstOrDefault();
                    BB_Maquinas_Usadas_Pedidos _BB_Maquinas_Usadas_Pedidos = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == nrSerie).FirstOrDefault();
                    if (_BB_Maquinas_Usadas_Tecnico != null)
                    {
                        BB_Clientes b = db.BB_Clientes.Where(x => x.accountnumber == _BB_Maquinas_Usadas_Tecnico.ClienteNr).FirstOrDefault();
                        a.Codref = _BB_Maquinas_Usadas_Tecnico.Codref;
                        a.Type_MFP = _BB_Maquinas_Usadas_Tecnico.Type_MFP;
                        a.PS_NUS = _BB_Maquinas_Usadas_Tecnico.PS_NUS;
                        a.NrSerie = _BB_Maquinas_Usadas_Tecnico.NrSerie;

                        a.ExpireDate = _BB_Maquinas_Usadas_Tecnico.ExpireDate;
                        a.IsReserved = _BB_Maquinas_Usadas_Tecnico.IsReserved;
                        a.ReservedDateTime = _BB_Maquinas_Usadas_Tecnico.ReservedDateTime;
                        a.ReservedBy = _BB_Maquinas_Usadas_Tecnico.ReservedBy;
                        a.ClienteNome = b != null ? b.Name : "";
                        a.ClienteNr = b != null ? b.accountnumber : "";
                        a.Comentarios = _BB_Maquinas_Usadas_Tecnico.Comentarios;
                    }

                    if (_BB_Maquinas_Usadas_Pedidos != null)
                    {
                        a.Status = _BB_Maquinas_Usadas_Pedidos.Status;
                    }

                    if (_Maquinas_Usadas != null)
                    {
                        a.DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.DVCTRegistada = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.CatCancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes = _Maquinas_Usadas.Observacoes_Logistica;
                        a.NrSerie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.NrCopyBW = _Maquinas_Usadas.Contador_Preto;
                        a.NrCopyCOLOR = _Maquinas_Usadas.Contador_Cor;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.PS_NUS = _Maquinas_Usadas.NUS_PS;
                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("UsedMachineReservedTecnicoDetail")]
        public IHttpActionResult UsedMachineReservedTecnicoDetail(string serie)
        {
            BB_Maquinas_Usadas_Tecnico_Detail a = new BB_Maquinas_Usadas_Tecnico_Detail();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    //BB_Maquinas_Usadas_Gestor _BB_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas_Pedidos _BB_Maquinas_Usadas_Pedidos = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == serie).FirstOrDefault();
                    if (_BB_Maquinas_Usadas_Tecnico != null)
                    {
                        BB_Clientes b = db.BB_Clientes.Where(x => x.accountnumber == _BB_Maquinas_Usadas_Tecnico.ClienteNr).FirstOrDefault();
                        a.Codref = _BB_Maquinas_Usadas_Tecnico.Codref;
                        a.Type_MFP = _BB_Maquinas_Usadas_Tecnico.Type_MFP;
                        a.PS_NUS = _BB_Maquinas_Usadas_Tecnico.PS_NUS;

                        a.ExpireDate = _BB_Maquinas_Usadas_Tecnico.ExpireDate;
                        a.IsReserved = _BB_Maquinas_Usadas_Tecnico.IsReserved;
                        a.ReservedDateTime = _BB_Maquinas_Usadas_Tecnico.ReservedDateTime;
                        a.ReservedBy = _BB_Maquinas_Usadas_Tecnico.ReservedBy;
                        a.ClienteNome = b != null ? b.Name : "";
                        a.ClienteNr = b != null ? b.accountnumber : "";
                        a.Comentarios = _BB_Maquinas_Usadas_Tecnico.Comentarios;

                    }

                    if (_BB_Maquinas_Usadas_Pedidos != null)
                    {
                        a.Status = _BB_Maquinas_Usadas_Pedidos.Status.GetValueOrDefault();
                    }

                    if (_Maquinas_Usadas != null)
                    {
                        a.DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.DVCTRegistada = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.CatCancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes = _Maquinas_Usadas.Observacoes_Logistica;
                        a.NrSerie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.NrCopyBW = _Maquinas_Usadas.Contador_Preto;
                        a.NrCopyCOLOR = _Maquinas_Usadas.Contador_Cor;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.PS_NUS = _Maquinas_Usadas.NUS_PS;
                        a.Codref = _Maquinas_Usadas.NUS_Referencia;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.Modelo = _Maquinas_Usadas.NUS_Modelo;
                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("AccountPopupEdit")]
        public IHttpActionResult AccountPopupEdit(string serie)
        {
            BB_Maquinas_Usadas_Tecnico_Detail a = new BB_Maquinas_Usadas_Tecnico_Detail();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas_Gestor _BB_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Gestor != null)
                    {
                        BB_Clientes b = db.BB_Clientes.Where(x => x.accountnumber == _BB_Maquinas_Usadas_Gestor.ClienteNr).FirstOrDefault();
                        a.Codref = _BB_Maquinas_Usadas_Gestor.Codref;
                        a.Type_MFP = _BB_Maquinas_Usadas_Gestor.Type_MFP;
                        a.PS_NUS = _BB_Maquinas_Usadas_Gestor.PS_NUS;
                        a.NrSerie = _BB_Maquinas_Usadas_Gestor.NrSerie;
                        a.NrCopyBW = _BB_Maquinas_Usadas_Gestor.NrCopyBW;
                        a.NrCopyCOLOR = _BB_Maquinas_Usadas_Gestor.NrCopyCOLOR;
                        a.ExpireDate = _BB_Maquinas_Usadas_Gestor.ExpireDate;
                        a.IsReserved = _BB_Maquinas_Usadas_Gestor.IsReserved;
                        a.ReservedDateTime = _BB_Maquinas_Usadas_Gestor.ReservedDateTime;
                        a.ReservedBy = _BB_Maquinas_Usadas_Gestor.ReservedBy;
                        a.ClienteNome = b != null ? b.Name : "";
                        a.ClienteNr = b != null ? b.accountnumber : "";
                        a.Comentarios = _BB_Maquinas_Usadas_Gestor.Comentarios;
                        a.Modelo = _Maquinas_Usadas.NUS_Modelo;

                    }

                    if (_Maquinas_Usadas != null)
                    {
                        a.DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.DVCTRegistada = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.CatCancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes = _Maquinas_Usadas.Observacoes_Logistica;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.PS_NUS = _Maquinas_Usadas.NUS_PS;
                        a.NrSerie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.NrCopyBW = _Maquinas_Usadas.Contador_Preto;
                        a.NrCopyCOLOR = _Maquinas_Usadas.Contador_Cor;
                        a.Data_Entrada = _Maquinas_Usadas.NUS_Data_Entrada_Equip;
                        a.Codref = _Maquinas_Usadas.NUS_Referencia;
                        a.Modelo = _Maquinas_Usadas.NUS_Modelo;
                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("LogisticaPopupEdit")]
        public IHttpActionResult LogisticaPopupEdit(string serie)
        {
            BB_Maquinas_Usadas_Logistica_Detail a = new BB_Maquinas_Usadas_Logistica_Detail();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();

                    if (_Maquinas_Usadas != null)
                    {
                        a.NUS_DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.NUS_DVCT_REGISTADA = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Nr_Entrada_Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.Cat_Cancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes_Logistica = _Maquinas_Usadas.Observacoes_Logistica;
                        a.NUS_Tipo = _Maquinas_Usadas.NUS_Tipo;
                        a.NUS_PS = _Maquinas_Usadas.NUS_PS;
                        a.NUS_Nr_Serie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.Contador_Preto = _Maquinas_Usadas.Contador_Preto;
                        a.Contador_Cor = _Maquinas_Usadas.Contador_Cor;
                        a.NUS_Data_Entrada_Equip = _Maquinas_Usadas.NUS_Data_Entrada_Equip;
                        a.NUS_Referencia = _Maquinas_Usadas.NUS_Referencia;
                        a.IsReserved = _Maquinas_Usadas.IsReserved;
                        a.Pedido_Por = _Maquinas_Usadas.Pedido_Por;
                        a.PriceUsed = _Maquinas_Usadas.PriceUsed;
                        a.ReservedUser = _Maquinas_Usadas.ReservedUser;
                        a.Validar = _Maquinas_Usadas.Validar.GetValueOrDefault();
                        a.NUS_Modelo = _Maquinas_Usadas.NUS_Modelo;
                        a.ARM = "5008";
                    }

                    BB_Maquinas_Usadas_Gestor gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == serie).FirstOrDefault();
                    if(gestor != null)
                    {
                        a.ReservedUser = gestor.ReservedBy;
                        a.dateTimeCreated = gestor.ReservedDateTime;
                        a.dateTimeExpire = gestor.ExpireDate;
                        a.ClienteNome = gestor.ClienteNr;
                    }

                    BB_Maquinas_Usadas_Tecnico tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == serie).FirstOrDefault();
                    if (tecnico != null)
                    {
                        a.ReservedUser = tecnico.ReservedBy;
                        a.dateTimeCreated = tecnico.ReservedDateTime;
                        a.dateTimeExpire = tecnico.ExpireDate;
                        a.ClienteNome = tecnico.ClienteNr;
                        
                    }

                    BB_Maquinas_Usadas_Logistica l = db.BB_Maquinas_Usadas_Logistica.Where(x => x.NrSerie == serie).FirstOrDefault();

                    if (l != null)
                    {
                        a.dateTimeExpire = l.ExpireDate;
                        a.dateTimeCreated = l.ReservedDateTime;
                        a.Observacoes_Logistica = l.Comentarios;
                        a.ClienteNome = l.ClienteNr;
                        a.ReservedUser = l.ReservedBy;

                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("LogisticaPopupDetail")]
        public IHttpActionResult LogisticaPopupDetail(string serie)
        {
            BB_Maquinas_Usadas_Logistica_Detail a = new BB_Maquinas_Usadas_Logistica_Detail();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();

                    if (_Maquinas_Usadas != null)
                    {
                        a.NUS_DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.NUS_DVCT_REGISTADA = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Nr_Entrada_Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.Cat_Cancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes_Logistica = _Maquinas_Usadas.Observacoes_Logistica;
                        a.NUS_Tipo = _Maquinas_Usadas.NUS_Tipo;
                        a.NUS_PS = _Maquinas_Usadas.NUS_PS;
                        a.NUS_Nr_Serie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.Contador_Preto = _Maquinas_Usadas.Contador_Preto;
                        a.Contador_Cor = _Maquinas_Usadas.Contador_Cor;
                        a.NUS_Data_Entrada_Equip = _Maquinas_Usadas.NUS_Data_Entrada_Equip;
                        a.NUS_Referencia = _Maquinas_Usadas.NUS_Referencia;
                        a.IsReserved = _Maquinas_Usadas.IsReserved;
                        a.Pedido_Por = _Maquinas_Usadas.Pedido_Por;
                        a.PriceUsed = _Maquinas_Usadas.PriceUsed;
                        a.ReservedUser = _Maquinas_Usadas.ReservedUser;
                        a.Validar = _Maquinas_Usadas.Validar.GetValueOrDefault();
                        a.NUS_Modelo = _Maquinas_Usadas.NUS_Modelo;
                        a.ARM = "5008";
                    }

                    BB_Maquinas_Usadas_Logistica l = db.BB_Maquinas_Usadas_Logistica.Where(x => x.NrSerie == serie).FirstOrDefault();

                    if (l != null)
                    {
                        a.dateTimeExpire = l.ExpireDate;
                        a.dateTimeCreated = l.ReservedDateTime;
                        a.ReservedUser = l.ReservedBy;
                        //a.Observacoes_Logistica = l.Comentarios;
                        a.ClienteNome = l.ClienteNr;

                    }

                    BB_Maquinas_Usadas_Gestor g = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == serie).FirstOrDefault();

                    if (g != null)
                    {
                        a.dateTimeExpire = g.ExpireDate;
                        a.dateTimeCreated = g.ReservedDateTime;
                        a.ReservedUser = g.ReservedBy;
                        //a.Observacoes_Logistica = g.Comentarios;
                        a.ClienteNome = g.ClienteNr;

                    }

                    BB_Maquinas_Usadas_Tecnico t = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == serie).FirstOrDefault();

                    if (t != null)
                    {
                        a.dateTimeExpire = t.ExpireDate;
                        a.dateTimeCreated = t.ReservedDateTime;
                        a.ReservedUser = t.ReservedBy;
                        //a.Observacoes_Logistica = g.Comentarios;
                        a.ClienteNome = g.ClienteNr;

                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("TecnicoEdit")]
        public IHttpActionResult TecnicoEdit(string serie)
        {
            BB_Maquinas_Usadas_Tecnico_Detail a = new BB_Maquinas_Usadas_Tecnico_Detail();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Tecnico != null)
                    {
                        BB_Clientes b = db.BB_Clientes.Where(x => x.accountnumber == _BB_Maquinas_Usadas_Tecnico.ClienteNr).FirstOrDefault();
                        a.Codref = _BB_Maquinas_Usadas_Tecnico.Codref;
                        a.Type_MFP = _BB_Maquinas_Usadas_Tecnico.Type_MFP;
                        a.PS_NUS = _BB_Maquinas_Usadas_Tecnico.PS_NUS;
                        a.NrSerie = _BB_Maquinas_Usadas_Tecnico.NrSerie;
                        a.NrCopyBW = _BB_Maquinas_Usadas_Tecnico.NrCopyBW;
                        a.NrCopyCOLOR = _BB_Maquinas_Usadas_Tecnico.NrCopyCOLOR;
                        a.ExpireDate = _BB_Maquinas_Usadas_Tecnico.ExpireDate;
                        a.IsReserved = _BB_Maquinas_Usadas_Tecnico.IsReserved;
                        a.ReservedDateTime = _BB_Maquinas_Usadas_Tecnico.ReservedDateTime;
                        a.ReservedBy = _BB_Maquinas_Usadas_Tecnico.ReservedBy;
                        a.ClienteNome = b != null ? b.Name : "";
                        a.ClienteNr = b != null ? b.accountnumber : "";
                        a.Comentarios = _BB_Maquinas_Usadas_Tecnico.Comentarios;

                        if (_BB_Maquinas_Usadas_Tecnico.IsApproved == null)
                        {
                            a.Status = 0;
                        }
                        if (_BB_Maquinas_Usadas_Tecnico.IsApproved.GetValueOrDefault())
                        {
                            a.Status = 1;
                        }
                        if (!_BB_Maquinas_Usadas_Tecnico.IsApproved.GetValueOrDefault())
                        {
                            a.Status = 2;
                        }






                    }

                    if (_Maquinas_Usadas != null)
                    {
                        a.DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.DVCTRegistada = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.CatCancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes = _Maquinas_Usadas.Observacoes_Logistica;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.PS_NUS = _Maquinas_Usadas.NUS_PS;
                        a.NrSerie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.NrCopyBW = _Maquinas_Usadas.Contador_Preto;
                        a.NrCopyCOLOR = _Maquinas_Usadas.Contador_Cor;
                        a.Data_Entrada = _Maquinas_Usadas.NUS_Data_Entrada_Equip;
                        a.Codref = _Maquinas_Usadas.NUS_Referencia;
                        a.Modelo = _Maquinas_Usadas.NUS_Modelo;

                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);
        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("AccountPopupEdit_Save")]
        public IHttpActionResult AccountPopupEdit_Save(BB_Maquinas_Usadas_Gestor_Detail a)
        {
            try
            {
                if (a.ClienteNr == null)
                {
                    return Content(HttpStatusCode.NotFound, "Não gravado! Selecionar Cliente");
                }

                int totalDays = (a.ExpireDate.GetValueOrDefault() - a.ReservedDateTime.GetValueOrDefault()).Days;
                if (totalDays < 0 || totalDays > 60)
                {
                    return Content(HttpStatusCode.NotFound, "Não gravado! A data de expiração é superior a 60 dias");
                }
                BB_Maquinas_Usadas_Gestor _BB_Maquinas_Usadas_Gestor = new BB_Maquinas_Usadas_Gestor();

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas_Gestor maq = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == a.NrSerie).FirstOrDefault();

                    _BB_Maquinas_Usadas_Gestor.Codref = a.Codref;
                    _BB_Maquinas_Usadas_Gestor.Type_MFP = a.Type_MFP;
                    _BB_Maquinas_Usadas_Gestor.PS_NUS = a.PS_NUS;
                    _BB_Maquinas_Usadas_Gestor.NrSerie = a.NrSerie;
                    _BB_Maquinas_Usadas_Gestor.NrCopyBW = a.NrCopyBW;
                    _BB_Maquinas_Usadas_Gestor.NrCopyCOLOR = a.NrCopyCOLOR;
                    _BB_Maquinas_Usadas_Gestor.ExpireDate = a.ExpireDate;
                    _BB_Maquinas_Usadas_Gestor.IsReserved = false;
                    _BB_Maquinas_Usadas_Gestor.ReservedDateTime = a.ReservedDateTime;
                    _BB_Maquinas_Usadas_Gestor.ReservedBy = a.ReservedBy;
                    _BB_Maquinas_Usadas_Gestor.ClienteNr = a.ClienteNr;
                    _BB_Maquinas_Usadas_Gestor.Comentarios = a.Comentarios;


                    if (maq == null)
                    {
                        db.BB_Maquinas_Usadas_Gestor.Add(_BB_Maquinas_Usadas_Gestor);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.BB_Maquinas_Usadas_Gestor.Remove(maq);
                        db.BB_Maquinas_Usadas_Gestor.Add(_BB_Maquinas_Usadas_Gestor);
                        db.SaveChanges();
                    }

                    return Ok(_BB_Maquinas_Usadas_Gestor);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("LogisticaPopupEdit_Save")]
        public IHttpActionResult LogisticaPopupEdit_Save(BB_Maquinas_Usadas_Logistica_Detail a)
        {
            try
            {

                if (a.accao == "Reserva")
                {
                    int totalDays = (a.dateTimeExpire.GetValueOrDefault() - a.dateTimeCreated.GetValueOrDefault()).Days;
                    if (totalDays < 0 || totalDays > 60)
                    {
                        return Content(HttpStatusCode.NotFound, "Não gravado! A data de expiração é superior a 60 dias");
                    }
                    BB_Maquinas_Usadas_Logistica _BB_Maquinas_Usadas_logistica = new BB_Maquinas_Usadas_Logistica();

                    using (var db = new BB_DB_DEVEntities2())
                    {
                        BB_Maquinas_Usadas_Logistica maq = db.BB_Maquinas_Usadas_Logistica.Where(x => x.NrSerie == a.NUS_Nr_Serie).FirstOrDefault();

                        _BB_Maquinas_Usadas_logistica.Codref = a.NUS_Referencia;
                        _BB_Maquinas_Usadas_logistica.Type_MFP = a.NUS_Tipo;
                        _BB_Maquinas_Usadas_logistica.PS_NUS = a.NUS_PS;
                        _BB_Maquinas_Usadas_logistica.NrSerie = a.NUS_Nr_Serie;
                        _BB_Maquinas_Usadas_logistica.NrCopyBW = a.Contador_Preto;
                        _BB_Maquinas_Usadas_logistica.NrCopyCOLOR = a.Contador_Cor;
                        _BB_Maquinas_Usadas_logistica.ExpireDate = a.dateTimeExpire;
                        _BB_Maquinas_Usadas_logistica.IsReserved = true;
                        _BB_Maquinas_Usadas_logistica.ReservedDateTime = a.dateTimeCreated;
                        _BB_Maquinas_Usadas_logistica.ReservedBy = a.ReservedUser;
                        _BB_Maquinas_Usadas_logistica.ClienteNr = a.ClienteNome;
                        _BB_Maquinas_Usadas_logistica.Comentarios = a.Observacoes_Logistica;
                        _BB_Maquinas_Usadas_logistica.ClienteNr = a.ClienteNome;

                        if (maq == null)
                        {
                            db.BB_Maquinas_Usadas_Logistica.Add(_BB_Maquinas_Usadas_logistica);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.BB_Maquinas_Usadas_Logistica.Remove(maq);
                            db.BB_Maquinas_Usadas_Logistica.Add(_BB_Maquinas_Usadas_logistica);
                            db.SaveChanges();
                        }

                        return Ok(_BB_Maquinas_Usadas_logistica);
                    }
                }

                if (a.accao == "Gravar")
                {
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        BB_Maquinas_Usadas m = new BB_Maquinas_Usadas();
                        m = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == a.NUS_Nr_Serie).FirstOrDefault();

                        if (m != null)
                        {
                            m.Observacoes_Logistica = a.Observacoes_Logistica;
                            m.Validar = m.Validar;
                            db.Entry(m).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                        return Ok();
                    }
                }

                if (a.accao == "Libertar")
                {
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        BB_Maquinas_Usadas_Logistica m = new BB_Maquinas_Usadas_Logistica();
                        m = db.BB_Maquinas_Usadas_Logistica.Where(x => x.NrSerie == a.NUS_Nr_Serie).FirstOrDefault();

                        if (m != null)
                        {
                            db.BB_Maquinas_Usadas_Logistica.Remove(m);
                            db.SaveChanges();
                        }

                        BB_Maquinas_Usadas_Gestor gestor = new BB_Maquinas_Usadas_Gestor();
                        gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == a.NUS_Nr_Serie).FirstOrDefault();

                        if (gestor != null)
                        {
                            db.BB_Maquinas_Usadas_Gestor.Remove(gestor);
                            db.SaveChanges();
                        }

                        BB_Maquinas_Usadas_Tecnico tecnico = new BB_Maquinas_Usadas_Tecnico();
                        tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == a.NUS_Nr_Serie).FirstOrDefault();

                        if (tecnico != null)
                        {
                            db.BB_Maquinas_Usadas_Tecnico.Remove(tecnico);
                            db.SaveChanges();
                        }


                        return Ok();
                    }
                }


                return Ok();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("TecnicoEdit_Save")]
        public async System.Threading.Tasks.Task<IHttpActionResult> TecnicoEdit_SaveAsync(BB_Maquinas_Usadas_Tecnico_Detail a)
        {
            try
            {

                if (a.accao == "Aprovar")
                {
                    if (a.ClienteNr == null)
                    {
                        return Content(HttpStatusCode.NotFound, "Não gravado! Selecionar Cliente");
                    }


                    int totalDays = (a.ExpireDate.GetValueOrDefault() - a.ReservedDateTime.GetValueOrDefault()).Days;
                    if (totalDays < 0 || totalDays > 60)
                    {
                        return Content(HttpStatusCode.NotFound, "Não gravado! A data de expiração é superior a 60 dias");
                    }



                    BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = new BB_Maquinas_Usadas_Tecnico();
                    //BB_Maquinas_Usadas_Pedidos pedido = new BB_Maquinas_Usadas_Pedidos();
                    BB_Clientes cliente = new BB_Clientes();

                    AspNetUsers user1 = new AspNetUsers();

                    using (var db = new BB_DB_DEVEntities2())
                    {

                        BB_Maquinas_Usadas_Pedidos pedido = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == a.NrSerie).FirstOrDefault();
                        //if (pedido != null)
                        //{
                        //    return Content(HttpStatusCode.NotFound, "Não Submetido! Existe um pedido pendente para esta máquina!");
                        //}

                        cliente = db.BB_Clientes.Where(x => x.accountnumber == a.ClienteNr).FirstOrDefault();
                        if (cliente != null)
                        {
                            using (var db1 = new masterEntities())
                            {
                                user1 = db1.AspNetUsers.Where(x => x.DisplayName == cliente.Owner).FirstOrDefault();
                            }

                            if (user1 != null)
                            {
                                pedido = new BB_Maquinas_Usadas_Pedidos();
                                pedido.AccountManager = user1.Email;
                                pedido.RequestTecnico = a.ReservedBy;
                                pedido.RequestTime = a.ReservedDateTime;
                                pedido.Status = 0;
                                pedido.Comentarios = a.Comentarios;
                                pedido.PS_NUS = a.NrSerie;

                                //if (pedido != null)
                                //{
                                //    db.BB_Maquinas_Usadas_Pedidos.Remove(pedido);
                                //}   

                                db.BB_Maquinas_Usadas_Pedidos.Add(pedido);
                            }


                            _BB_Maquinas_Usadas_Tecnico.Codref = a.Codref;
                            _BB_Maquinas_Usadas_Tecnico.Type_MFP = a.Type_MFP;
                            _BB_Maquinas_Usadas_Tecnico.PS_NUS = a.PS_NUS;
                            _BB_Maquinas_Usadas_Tecnico.NrSerie = a.NrSerie;
                            _BB_Maquinas_Usadas_Tecnico.NrCopyBW = a.NrCopyBW;
                            _BB_Maquinas_Usadas_Tecnico.NrCopyCOLOR = a.NrCopyCOLOR;
                            _BB_Maquinas_Usadas_Tecnico.ExpireDate = a.ExpireDate;
                            _BB_Maquinas_Usadas_Tecnico.IsReserved = true;
                            _BB_Maquinas_Usadas_Tecnico.ReservedDateTime = a.ReservedDateTime;
                            _BB_Maquinas_Usadas_Tecnico.ReservedBy = a.ReservedBy;
                            _BB_Maquinas_Usadas_Tecnico.ClienteNr = a.ClienteNr;
                            _BB_Maquinas_Usadas_Tecnico.Comentarios = a.Comentarios;
                            _BB_Maquinas_Usadas_Tecnico.PontoEnvio_CodPostal_Recolha = a.PontoEnvio_CodPostal_Recolha;
                            _BB_Maquinas_Usadas_Tecnico.PontoEnvio_Contacto_Recolha = a.PontoEnvio_Contacto_Recolha;
                            _BB_Maquinas_Usadas_Tecnico.PontoEnvio_Telefone_Recolha = a.PontoEnvio_Telefone_Recolha;
                            _BB_Maquinas_Usadas_Tecnico.PontoEnvio_Localidade_Recolha = a.PontoEnvio_Localidade_Recolha;
                            _BB_Maquinas_Usadas_Tecnico.ContadorBW_Antigo = a.ContadorBW_Antigo;
                            _BB_Maquinas_Usadas_Tecnico.ContadorCOR_Antigo = a.ContadorCOR_Antigo;
                            _BB_Maquinas_Usadas_Tecnico.Recolha_Opcionais = a.Recolha_Opcionais;
                            _BB_Maquinas_Usadas_Tecnico.Leasedesk = a.Leasedesk;
                            _BB_Maquinas_Usadas_Tecnico.ModifiedDate = DateTime.Now;
                            _BB_Maquinas_Usadas_Tecnico.PS_Recolha = a.PS_Recolha;


                            var maq = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == a.NrSerie).FirstOrDefault();
                            if (maq != null)
                            {
                                db.BB_Maquinas_Usadas_Tecnico.Remove(maq);
                            }

                            db.BB_Maquinas_Usadas_Tecnico.Add(_BB_Maquinas_Usadas_Tecnico);
                            db.SaveChanges();

                        }
                        return Ok(_BB_Maquinas_Usadas_Tecnico);
                    }
                }


                if (a.accao == "Leasesdesk")
                {
                    BB_Data_Integration i = new BB_Data_Integration();
                    AspNetUsers user1 = new AspNetUsers();
                    BB_Clientes cliente = new BB_Clientes();
                    BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = new BB_Maquinas_Usadas_Tecnico();
                    using (var db = new BB_DB_DEVEntities2())
                    {
                        i = db.BB_Data_Integration.Where(x => x.CodeRef == a.Codref).FirstOrDefault();
                        cliente = db.BB_Clientes.Where(x => x.accountnumber == a.ClienteNr).FirstOrDefault();
                        _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == a.NrSerie).FirstOrDefault();

                        _BB_Maquinas_Usadas_Tecnico.Leasedesk = true;
                        _BB_Maquinas_Usadas_Tecnico.ModifiedDate = DateTime.Now;
                        db.Entry(_BB_Maquinas_Usadas_Tecnico).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    using (var db1 = new masterEntities())
                    {
                        user1 = db1.AspNetUsers.Where(x => x.Email == _BB_Maquinas_Usadas_Tecnico.ReservedBy).FirstOrDefault();
                    }


                    //envia email
                    //EMAIL SEND
                    EmailService emailSend = new EmailService();
                    EmailMesage message = new EmailMesage();

                    message.Destination = "clientes@konicaminolta.pt";
                    //message.Destination = "joao.reis@konicaminolta.pt";
                    message.Subject = "Business Builder - Substituição Técnica - Pedido - " + user1.DisplayName + " NrCliente - " + a.ClienteNr;
                    StringBuilder strBuilder = new StringBuilder();

                    strBuilder.Append("Caro(a) Leasedesk(a),<br/>");
                    strBuilder.Append("O seguinte pedido do técnico <b>" + user1.DisplayName + " foi submetido</b><br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Cliente</b><br/>");
                    strBuilder.Append("Nome: " + a.ClienteNome + "<br/>");
                    strBuilder.Append("Numero conta: " + a.ClienteNr + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Recolha Contacto</b><br/>");
                    strBuilder.Append("Nome: " + a.PontoEnvio_Contacto_Recolha + "<br/>");
                    strBuilder.Append("Morada: " + a.PontoEnvio_Morada_Recolha + "<br/>");
                    strBuilder.Append("Codigo Postal" + a.PontoEnvio_CodPostal_Recolha + "<br/>");
                    strBuilder.Append("Localidade" + a.PontoEnvio_Localidade_Recolha + "<br/>");
                    strBuilder.Append("Contacto" + a.PontoEnvio_Telefone_Recolha + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<b>Equipamento a recolher:</b><br/>");
                    strBuilder.Append("Máquina/equipamento" +   (i != null? i.Description_Portuguese: _BB_Maquinas_Usadas_Tecnico.Codref) + "<br/>");
                    strBuilder.Append("PS: " + a.PS_Recolha + "<br/>");

                    strBuilder.Append("<b>Equipamento a colocar::</b><br/>");
                    strBuilder.Append("Nr Serie: " + (_BB_Maquinas_Usadas_Tecnico.NrSerie != null ? _BB_Maquinas_Usadas_Tecnico.NrSerie : "") + "<br/>");
                    strBuilder.Append("Ps-Nus: " + (_BB_Maquinas_Usadas_Tecnico.PS_NUS != null ? _BB_Maquinas_Usadas_Tecnico.PS_NUS : "") + "<br/>");
                    strBuilder.Append("Contador BW: " + (_BB_Maquinas_Usadas_Tecnico.NrCopyBW != null ? _BB_Maquinas_Usadas_Tecnico.NrCopyBW.ToString() : "-") + "<br/>");
                    strBuilder.Append("Contador COR: " + (_BB_Maquinas_Usadas_Tecnico.NrCopyCOLOR != null ? _BB_Maquinas_Usadas_Tecnico.NrCopyCOLOR.ToString() : "-") + "<br/>");

                    //strBuilder.Append("<br/>");
                    //if (prazoDif.CreatedTime != null) strBuilder.Append("<b>Data de Pedido</b>: " + prazoDif.CreatedTime.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    //if (prazoDif.ModifiedTime != null) strBuilder.Append("<b>Data de Resposta</b>: " + prazoDif.ModifiedTime.Value.ToString("dd/MM/yyyy HH:mm") + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("Para aceder à aplicação, utilize o seguinte link: " + "https://bb.konicaminolta.pt" + "<br/>");
                    strBuilder.Append("Por favor, não responder a este email." + "<br/>");
                    strBuilder.Append("<br/>");
                    strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                    message.Body = strBuilder.ToString();

                    await emailSend.SendEmailaync(message);
                }

                return Ok();



            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("AccountPedidosEdit_Save")]
        public IHttpActionResult AccountPedidosEdit_Save(BB_Maquinas_Usadas_Pedidos_ a)
        {
            try
            {
                BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = new BB_Maquinas_Usadas_Tecnico();
                BB_Maquinas_Usadas_Pedidos pedido = new BB_Maquinas_Usadas_Pedidos();

                AspNetUsers user1 = new AspNetUsers();

                using (var db = new BB_DB_DEVEntities2())
                {
                    pedido = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == a.NrSerie && x.ID == a.ID).FirstOrDefault();
                    if (pedido != null)
                    {
                        pedido.Status = a.Status;
                        pedido.Comentarios = a.Comentarios;
                        db.Entry(pedido).State = pedido.ID == 0 ? EntityState.Added : EntityState.Modified;
                        db.SaveChanges();
                    }

                    _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.PS_NUS == a.PS_NUS).FirstOrDefault();
                    if (_BB_Maquinas_Usadas_Tecnico != null)
                    {
                        if (a.Status == 1)
                        {
                            _BB_Maquinas_Usadas_Tecnico.IsApproved = true;

                        }
                        else
                        {
                            _BB_Maquinas_Usadas_Tecnico.IsApproved = false;
                        }

                        _BB_Maquinas_Usadas_Tecnico.Comentarios = a.Comentarios;
                        _BB_Maquinas_Usadas_Tecnico.IsReserved = true;

                        db.Entry(_BB_Maquinas_Usadas_Tecnico).State = _BB_Maquinas_Usadas_Tecnico.ID == 0 ? EntityState.Added : EntityState.Modified;
                        db.SaveChanges();
                    }


                    return Ok(_BB_Maquinas_Usadas_Tecnico);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }



        [AcceptVerbs("GET", "POST")]
        [ActionName("AccountPedidosEdit")]
        public IHttpActionResult AccountPedidosEdit(string serie, int? id)
        {
            BB_Maquinas_Usadas_Pedidos_ a = new BB_Maquinas_Usadas_Pedidos_();

            try
            {

                using (var db = new BB_DB_DEVEntities2())
                {
                    BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == serie).FirstOrDefault();
                    BB_Maquinas_Usadas _Maquinas_Usadas = db.BB_Maquinas_Usadas.Where(x => x.NUS_Nr_Serie == serie).FirstOrDefault();
                    //BB_Maquinas_Usadas_Pedidos _BB_Maquinas_Usadas_Pedidos = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == serie && x.ID == id).FirstOrDefault();
                    BB_Maquinas_Usadas_Pedidos _BB_Maquinas_Usadas_Pedidos = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == serie).FirstOrDefault();
                    if (_BB_Maquinas_Usadas_Tecnico != null)
                    {
                        BB_Clientes b = db.BB_Clientes.Where(x => x.accountnumber == _BB_Maquinas_Usadas_Tecnico.ClienteNr).FirstOrDefault();
                        a.Codref = _BB_Maquinas_Usadas_Tecnico.Codref;
                        a.Type_MFP = _BB_Maquinas_Usadas_Tecnico.Type_MFP;
                        a.PS_NUS = _BB_Maquinas_Usadas_Tecnico.PS_NUS;

                        a.ExpireDate = _BB_Maquinas_Usadas_Tecnico.ExpireDate;
                        a.IsReserved = _BB_Maquinas_Usadas_Tecnico.IsReserved;
                        a.ReservedDateTime = _BB_Maquinas_Usadas_Tecnico.ReservedDateTime;
                        a.ReservedBy = _BB_Maquinas_Usadas_Tecnico.ReservedBy;
                        a.ClienteNome = b != null ? b.Name : "";
                        a.ClienteNr = b != null ? b.accountnumber : "";
                        a.Comentarios = _BB_Maquinas_Usadas_Tecnico.Comentarios;
                    }

                    if (_BB_Maquinas_Usadas_Pedidos != null)
                    {
                        a.Status = _BB_Maquinas_Usadas_Pedidos.Status;
                    }

                    if (_Maquinas_Usadas != null)
                    {
                        a.DVCT = _Maquinas_Usadas.NUS_DVCT;
                        a.DVCTRegistada = _Maquinas_Usadas.NUS_DVCT_REGISTADA;
                        a.Armazem = _Maquinas_Usadas.Nr_Entrada_Armazem;
                        a.CatCancelado = _Maquinas_Usadas.Cat_Cancelado;
                        a.Observacoes = _Maquinas_Usadas.Observacoes_Logistica;
                        a.NrSerie = _Maquinas_Usadas.NUS_Nr_Serie;
                        a.NrCopyBW = _Maquinas_Usadas.Contador_Preto;
                        a.NrCopyCOLOR = _Maquinas_Usadas.Contador_Cor;
                        a.Type_MFP = _Maquinas_Usadas.NUS_Tipo;
                        a.PS_NUS = _Maquinas_Usadas.NUS_PS;
                        a.Modelo = _Maquinas_Usadas.NUS_Modelo;
                    }
                }



            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return Ok(a);

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetClientes")]
        public IHttpActionResult GetClientes(string accountManager)
        {
            try
            {
                List<BB_Clientes> lstClientes = new List<BB_Clientes>();
                List<BB_Clientes_> lstClientes_ = new List<BB_Clientes_>();
                string ownerName = "";
                using (var db1 = new masterEntities())
                {
                    ownerName = db1.AspNetUsers.Where(x => x.Email == accountManager).Select(x => x.DisplayName).FirstOrDefault();
                }

                using (var db = new BB_DB_DEVEntities2())
                {
                    lstClientes = db.BB_Clientes.Where(x => x.Owner == ownerName).ToList();

                    foreach (var i in lstClientes)
                    {
                        BB_Clientes_ a = new BB_Clientes_();
                        a.Name = i.Name;
                        a.accountnumber = i.accountnumber;
                        lstClientes_.Add(a);
                    }

                    return Ok(lstClientes_);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("GetClientesTecnico")]
        public IHttpActionResult GetClientesTecnico()
        {
            try
            {
                List<BB_Clientes> lstClientes = new List<BB_Clientes>();
                List<BB_Clientes_> lstClientes_ = new List<BB_Clientes_>();
                string ownerName = "";


                using (var db = new BB_DB_DEVEntities2())
                {
                    lstClientes = db.BB_Clientes.ToList();

                    foreach (var i in lstClientes)
                    {
                        BB_Clientes_ a = new BB_Clientes_();
                        a.Name = i.Name;
                        a.accountnumber = i.accountnumber;
                        lstClientes_.Add(a);
                    }

                    return Ok(lstClientes_);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }
        [AcceptVerbs("GET", "POST")]
        [ActionName("AccountPopupEdit_Libertar")]
        public async System.Threading.Tasks.Task<IHttpActionResult> AccountPopupEdit_LibertarAsync(BB_Maquinas_Usadas_Gestor_Detail a)
        {
            try
            {
                BB_Maquinas_Usadas_Gestor _BB_Maquinas_Usadas_Gestor = new BB_Maquinas_Usadas_Gestor();
                BB_Maquinas_Usadas_Pedidos _BB_Maquinas_Usadas_Pedidos = new BB_Maquinas_Usadas_Pedidos();
                BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_Tecnico = new BB_Maquinas_Usadas_Tecnico();




                bool toGestor = false;
                using (var db = new BB_DB_DEVEntities2())
                {
                    _BB_Maquinas_Usadas_Gestor = db.BB_Maquinas_Usadas_Gestor.Where(x => x.NrSerie == a.NrSerie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Gestor != null)
                    {
                        db.BB_Maquinas_Usadas_Gestor.Remove(_BB_Maquinas_Usadas_Gestor);
                        db.SaveChanges();
                        toGestor = true;
                    }

                    _BB_Maquinas_Usadas_Pedidos = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == a.NrSerie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Pedidos != null)
                    {
                        //envia email
                        //EMAIL SEND
                        EmailService emailSend = new EmailService();
                        EmailMesage message = new EmailMesage();

                        message.Destination = a.ReservedBy;
                        message.Subject = "Business Builder - Maquina Usada - Pedido Reprovado- " + a.NrSerie;
                        StringBuilder strBuilder = new StringBuilder();

                        strBuilder.Append("Caro(a) Supervisor(a),<br/>");
                        strBuilder.Append("O seguinte pedido com o nr de série " + a.NrSerie + " foi reprovado</b><br/>");
                        strBuilder.Append("<br/>");
                        strBuilder.Append("<br/>");
                        strBuilder.Append("Para aceder à aplicação, utilize o seguinte link: " + "https://bb.konicaminolta.pt" + "<br/>");
                        strBuilder.Append("Por favor, não responder a este email." + "<br/>");
                        strBuilder.Append("<br/>");
                        strBuilder.Append("<p><strong><span style='font-size: 48px;'>//Business Builder</span></strong></p>");
                        message.Body = strBuilder.ToString();

                        await emailSend.SendEmailaync(message);

                        db.BB_Maquinas_Usadas_Pedidos.Remove(_BB_Maquinas_Usadas_Pedidos);
                        db.SaveChanges();
                    }

                    _BB_Maquinas_Usadas_Tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == a.NrSerie).FirstOrDefault();

                    if (_BB_Maquinas_Usadas_Tecnico != null)
                    {
                        db.BB_Maquinas_Usadas_Tecnico.Remove(_BB_Maquinas_Usadas_Tecnico);
                        db.SaveChanges();
                    }

                    if (toGestor)
                    {
                        return Ok(_BB_Maquinas_Usadas_Gestor);
                    }
                    else
                    {
                        return Ok(_BB_Maquinas_Usadas_Pedidos);
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }

        [AcceptVerbs("GET", "POST")]
        [ActionName("TecnicoEdit_Libertar")]
        public async System.Threading.Tasks.Task<IHttpActionResult> TecnicoEdit_LibertarAsync(BB_Maquinas_Usadas_Tecnico_Detail a)
        {
            try
            {
                BB_Maquinas_Usadas_Tecnico _BB_Maquinas_Usadas_tecnico = new BB_Maquinas_Usadas_Tecnico();

                using (var db = new BB_DB_DEVEntities2())
                {

                    _BB_Maquinas_Usadas_tecnico = db.BB_Maquinas_Usadas_Tecnico.Where(x => x.NrSerie == a.NrSerie).FirstOrDefault();
                    BB_Maquinas_Usadas_Pedidos pedido = db.BB_Maquinas_Usadas_Pedidos.Where(x => x.PS_NUS == a.NrSerie).FirstOrDefault();




                    if (pedido != null)
                    {
                        db.BB_Maquinas_Usadas_Pedidos.Remove(pedido);
                        db.SaveChanges();
                    }
                    if (_BB_Maquinas_Usadas_tecnico != null)
                    {
                        db.BB_Maquinas_Usadas_Tecnico.Remove(_BB_Maquinas_Usadas_tecnico);
                        db.SaveChanges();
                    }

                    return Ok(_BB_Maquinas_Usadas_tecnico);
                }


            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, "Any object");
            }

        }
    }



    public class BB_Maquinas_Usadas_Gestor_Modelo
    {
        public int ID { get; set; }
        public Nullable<int> ProposalID { get; set; }
        public string ReservedBy { get; set; }
        public Nullable<System.DateTime> ReservedDateTime { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public string PS_NUS { get; set; }
        public string Codref { get; set; }
        public string Type_MFP { get; set; }
        public string NrSerie { get; set; }
        public Nullable<int> NrCopyBW { get; set; }
        public Nullable<int> NrCopyCOLOR { get; set; }
        public Nullable<bool> IsReserved { get; set; }
        public string ClienteNr { get; set; }
        public string Comentarios { get; set; }

        public string Modelo { get; set; }

        public double? PriceUsed { get; set; }
    }

    public class PostData
    {

        public string PS_NUS { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string ProposalID { get; set; }
        public string ClientNr { get; set; }
        public string Codref { get; set; }
        public string Type_MFP { get; set; }
        public string NrSerie { get; set; }
        public int? NrCopyBW { get; set; }
        public int? NrCopyCOLOR { get; set; }
        public bool? IsReserved { get; set; }
        public DateTime? ReservedDateTime { get; set; }
        public string ReservedBy { get; set; }
        public string Comentarios { get; set; }
    }

    public class ProfileUser
    {

        public string Username { get; set; }
    }
}
