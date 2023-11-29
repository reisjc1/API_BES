using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication1.App_Start;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class NUSController : ApiController
    {
        public class BB_LocaisEnvio_Model
        {
            public int? ID { get; set; }
            //public string AccountNumber { get; set; }
            //public string NomeCliente { get; set; }
            public string Adress1 { get; set; }
            public string Adress2 { get; set; }
            public string City { get; set; }
            public string Contacto { get; set; }
            public string PostalCode { get; set; }
            public string County { get; set; }

        }


        [AcceptVerbs("GET", "POST")]
        [ActionName("SaveLocaisEnvio")]
        public HttpResponseMessage SaveLocaisEnvio(ProposalRootObject p)
        {
            ActionResponse ac = new ActionResponse();
            ac.Message = "Gravado com sucesso";
            return  Request.CreateResponse<ActionResponse>(HttpStatusCode.OK, ac);
        }




        [AcceptVerbs("GET", "POST")]
        [ActionName("GetLocaisEnvio")]
        public List<BB_LocaisEnvio_Model> GetLocaisEnvio(string accountNumber)
        {

            List<BB_LocaisEnvio_Model> lst = new List<BB_LocaisEnvio_Model>();

            //using (var db = new BB_DB_DEVEntities2())
            //{
            //    try { 
            //    lst = db.BB_LocaisEnvio.Where(x => x.AccountNumber == accountNumber).Select(x=> new BB_LocaisEnvio_Model { AccountNumber = x.AccountNumber, NomeCliente = x.NomeCliente,
            //        Adress1 = x.Adress1,
            //        Adress2 = x.Adress2, City = x.City, Contacto = x.Contacto, PostalCode = x.PostalCode }).Distinct().ToList();
            //    }
            //    catch(Exception ex)
            //    {
            //        ex.Message.ToString();
            //    }
            //}
            string bdConnect = @AppSettingsGet.BasedadosConnect;
            using (SqlConnection conn = new SqlConnection(bdConnect))
            {
                try
                {
                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    SqlCommand cmd = new SqlCommand("sp_Get_LocaisDeEnvio", conn);
                    cmd.CommandTimeout = 180;
                    // 2. set the command object so it knows to execute a stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", accountNumber);
                    // 3. add parameter to command, which will be passed to the stored procedure
                    //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                    // execute the command

                    //SqlDataReader rdr = cmd.ExecuteNonQuery();

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // iterate through results, printing each to console
                        while (rdr.Read())
                        {
                            BB_LocaisEnvio_Model m = new BB_LocaisEnvio_Model();
                            m.ID = int.Parse(rdr["ID"].ToString());
                            m.Adress1 = rdr["Adress1"] != null ? rdr["Adress1"].ToString() : "";
                            m.Adress2 = rdr["Adress2"] != null ? rdr["Adress2"].ToString() : "";
                            m.City = rdr["City"] != null ? rdr["City"].ToString() : "";
                            m.Contacto = rdr["Contacto"] != null ? rdr["Contacto"].ToString() : "";
                            m.PostalCode = rdr["PostalCode"] != null ? rdr["PostalCode"].ToString() : "";
                            m.County = rdr["County"] != null ? rdr["County"].ToString() : "";
                            lst.Add(m);


                        }
                        rdr.Close();
                        // iterate through results, printing each to console
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }


            return lst;



        }
    }



    public class NUSLocaisEnvio
    {
        public string NUSContracto { get; set; }
        public string quotenumber { get; set; }
        public string ShipAddress { get; set; }
        public string ShipAddress2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipContact { get; set; }
        public string ContactName { get; set; }
        public string ShipPostCode { get; set; }
    }
}
