using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using WebApplication1.DAL;
using WebApplication1.Models;

namespace WebApplication1.BLL
{
    public class HomepageDasboards
    {
       
        public List<Model_sp_Get_CRM_Oportunity_Status> DashBoard_Homepage(string user)
        {
            CRM_ImpersonatedUser sp = new CRM_ImpersonatedUser();
            List<Model_sp_Get_CRM_Oportunity_Status> lst = new List<Model_sp_Get_CRM_Oportunity_Status>();
            lst = sp.sp_Get_CRM_Oportunity_Status(user);
            return lst;
        }
    }
}