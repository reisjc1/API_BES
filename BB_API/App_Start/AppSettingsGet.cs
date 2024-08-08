using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApplication1.App_Start
{
    public static class AppSettingsGet
    {
        public static string CRM_Integration_Template_ClickPrice
        {
            get { return ConfigurationManager.AppSettings["CRM_Integration_Template_ClickPrice"].ToString(); }
        }

        public static string LeaseDesk_UploadFile_Contrato_DocuSign
        {
            get { return ConfigurationManager.AppSettings["LeaseDesk_UploadFile_Contrato_DocuSign"].ToString(); }
        }

        public static string CRM_Integration_Template_Quote
        {
            get { return ConfigurationManager.AppSettings["CRM_Integration_Template_Quote"].ToString(); }
        }

        public static string LeaseDesk_UploadFile_DIR
        {
            get { return ConfigurationManager.AppSettings["LeaseDesk_UploadFile_DIR"].ToString(); }
        }

        public static string LeaseDesk_UploadFile_Contrato
        {
            get { return ConfigurationManager.AppSettings["LeaseDesk_UploadFile_Contrato"].ToString(); }
        }

        public static string DocumentPrintingFolder
        {
            get { return ConfigurationManager.AppSettings["DocumentPrintingFolder"].ToString(); }
        }

        public static string DocumentPrintingTemplate
        {
            get { return ConfigurationManager.AppSettings["DocumentPrintingTemplate"].ToString(); }
        }

        public static string DocumentPrintingContractoTemplate
        {
            get { return ConfigurationManager.AppSettings["DocumentPrintingContractoTemplate"].ToString(); }
        }
        public static string DocumentPrintingContractoWordTemplate
        {
            get { return ConfigurationManager.AppSettings["DocumentPrintingContractoWordTemplate"].ToString(); }
        }

        public static string ProFormaPrintingTemplate
        {
            get { return ConfigurationManager.AppSettings["ProFormaPrintingTemplate"].ToString(); }
        }
        public static string ConfiguracaoNegocio
        {
            get { return ConfigurationManager.AppSettings["ConfiguracaoNegocio"].ToString(); }
        }

        public static string ConfiguracaoContrato
        {
            get { return ConfigurationManager.AppSettings["ConfiguracaoContrato"].ToString(); }
        }

        public static string NUS_Template
        {
            get { return ConfigurationManager.AppSettings["NUS_Template"].ToString(); }
        }

        public static string ExcelTemplate
        {
            get { return ConfigurationManager.AppSettings["ExcelTemplate"].ToString(); }
        }

        // I also get my connection string from here
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; }
        }

        public static string PedidoPrazoDiferenciadoEmail
        {
            get { return ConfigurationManager.AppSettings["PedidoPrazoDiferenciadoEmail"].ToString(); }
        }

        public static string HumanResourcesFileLocation
        {
            get { return ConfigurationManager.AppSettings["HumanResourcesFileLocation"].ToString(); }
        }
        public static string SapConfigMESCOD
        {
            get { return ConfigurationManager.AppSettings["SAPENVIROMENT_MESCOD"].ToString(); }
        }
        public static string RCVPRN
        {
            get { return ConfigurationManager.AppSettings["SAPENVIROMENT_RCVPRN"].ToString(); }
        }
        public static string SFTP
        {
            get { return ConfigurationManager.AppSettings["SAPENVIROMENT_SFTP"].ToString(); }
        }

        public static string BasedadosConnect
        {
            get { return ConfigurationManager.AppSettings["BasedadosConnect"].ToString(); }
        }

        //public static string CRM_Integration_Template_Quote
        //{
        //    get { return ConfigurationManager.ConnectionStrings["CRM_Integration_Template_Quote"].ToString(); }
        //}
    }
}