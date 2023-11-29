using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using WebApplication1.Models;

namespace WebApplication1.DAL
{

    public class CRM_ImpersonatedUser
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
      int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);


        public List<Model_sp_Get_CRM_Oportunity_Status> sp_Get_CRM_Oportunity_Status_Impersonated()
        {
            List<Model_sp_Get_CRM_Oportunity_Status> lst = new List<Model_sp_Get_CRM_Oportunity_Status>();
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("sp_Get_CRM_Oportunity_Status----------", EventLogEntryType.Information, 101, 1);
                }
                // Get the user token for the specified user, domain, and password using the   
                // unmanaged LogonUser method.   
                // The local machine name can be used for the domain name to impersonate a user on this machine.  

                string domainName = "BS";


                string userName = "BPT.CRM-Master";



                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.   
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.   
                SafeAccessTokenHandle safeAccessTokenHandle;
                bool returnValue = LogonUser(userName, domainName, "664GfxcJ",
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeAccessTokenHandle);
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("returnValue  " + returnValue, EventLogEntryType.Information, 101, 1);
                }
                if (false == returnValue)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("LogonUser", EventLogEntryType.Information, 101, 1);
                    }

                    int ret = Marshal.GetLastWin32Error();
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Ret: " + ret, EventLogEntryType.Information, 101, 1);
                    }
                    Console.WriteLine("LogonUser failed with error code : {0}", ret);
                    throw new System.ComponentModel.Win32Exception(ret);
                }



                // Note: if you want to run as unimpersonated, pass  
                //       'SafeAccessTokenHandle.InvalidHandle' instead of variable 'safeAccessTokenHandle'  
                using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeAccessTokenHandle.DangerousGetHandle()))
                {
                    //WindowsIdentity.RunImpersonated(
                    //safeAccessTokenHandle,
                    //// User action  
                    //() =>
                    //{
                    // Check the identity.  
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("dentro WindowsIdentity", EventLogEntryType.Information, 101, 1);
                    }
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry(WindowsIdentity.GetCurrent().Name, EventLogEntryType.Information, 101, 1);
                    }
                    try
                    {

                        using (SqlConnection conn = new SqlConnection("Server=KMDBSDEV02.bs.kme.intern;Database=BB_DB_DEV;Integrated Security=SSPI;"))
                        {

                            conn.Open();
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("SqlConnectiony", EventLogEntryType.Information, 101, 1);
                            }
                            // 1.  create a command object identifying the stored procedure
                            SqlCommand cmd = new SqlCommand("sp_Get_CRM_Oportunity_Status", conn);
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("sp_Get_CRM_Oportunity_Status SqlCommand", EventLogEntryType.Information, 101, 1);
                            }
                            // 2. set the command object so it knows to execute a stored procedure
                            cmd.CommandType = CommandType.StoredProcedure;

                            // 3. add parameter to command, which will be passed to the stored procedure
                            //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                            // execute the command

                            SqlDataReader rdr = cmd.ExecuteReader();

                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("SqlDataReader", EventLogEntryType.Information, 101, 1);
                            }
                           

                                // iterate through results, printing each to console
                                while (rdr.Read())
                                {
                                    Model_sp_Get_CRM_Oportunity_Status m = new Model_sp_Get_CRM_Oportunity_Status();
                                    m.CurrentStatus = rdr["CurrentStatus"].ToString();
                                    m.TotalCount = rdr["TotalCount"].ToString();
                                    lst.Add(m);
                                }
                           

                            rdr.Close();

                            //using (SqlDataReader rdr = cmd.ExecuteReader())
                            //{
                            //    using (EventLog eventLog = new EventLog("Application"))
                            //    {
                            //        eventLog.Source = "Application";
                            //        eventLog.WriteEntry("SqlDataReader", EventLogEntryType.Information, 101, 1);
                            //    }
                            //    try
                            //    {

                            //        // iterate through results, printing each to console
                            //        while (rdr.Read())
                            //        {
                            //            Model_sp_Get_CRM_Oportunity_Status m = new Model_sp_Get_CRM_Oportunity_Status();
                            //            m.CurrentStatus = rdr["CurrentStatus"].ToString();
                            //            m.TotalCount = rdr["TotalCount"].ToString();
                            //            lst.Add(m);
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        using (EventLog eventLog = new EventLog("Application"))
                            //        {
                            //            eventLog.Source = "Application";
                            //            eventLog.WriteEntry("SqlDataReader" + ex.Message.ToString() + "----" + ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                            //        }
                            //    }

                            //}
                        }

                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry("dentro da query", EventLogEntryType.Information, 101, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry(ex.Message.ToString() + "----" + ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.Message.ToString() + " - " + ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                }
            }
            return lst;
        }

        public List<Model_sp_Get_CRM_Oportunity_Status> sp_Get_CRM_Oportunity_Status(string user)
        {
            List<Model_sp_Get_CRM_Oportunity_Status> lst = new List<Model_sp_Get_CRM_Oportunity_Status>();
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("sp_Get_CRM_Oportunity_Status----------", EventLogEntryType.Information, 101, 1);
                }
                // Get the user token for the specified user, domain, and password using the   
                // unmanaged LogonUser method.   
                // The local machine name can be used for the domain name to impersonate a user on this machine.  

                string domainName = "BS";


                string userName = "BPT.CRM-Master";



                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.   
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token.   
                SafeAccessTokenHandle safeAccessTokenHandle;
                bool returnValue = LogonUser(userName, domainName, "664GfxcJ",
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeAccessTokenHandle);
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("returnValue  " + returnValue, EventLogEntryType.Information, 101, 1);
                }
                if (false == returnValue)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("LogonUser", EventLogEntryType.Information, 101, 1);
                    }

                    int ret = Marshal.GetLastWin32Error();
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Ret: " + ret, EventLogEntryType.Information, 101, 1);
                    }
                    Console.WriteLine("LogonUser failed with error code : {0}", ret);
                    throw new System.ComponentModel.Win32Exception(ret);
                }



               
               
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("dentro WindowsIdentity", EventLogEntryType.Information, 101, 1);
                    }
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry(WindowsIdentity.GetCurrent().Name, EventLogEntryType.Information, 101, 1);
                    }
                    try
                    {

                        using (SqlConnection conn = new SqlConnection("Server=KMDBSDEV02.bs.kme.intern;Database=BB_DB_DEV;User Id=portalkm_webuser;Password=123456789;"))
                        {

                            conn.Open();
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("SqlConnectiony", EventLogEntryType.Information, 101, 1);
                            }
                            // 1.  create a command object identifying the stored procedure
                            SqlCommand cmd = new SqlCommand("sp_Get_CRM_Oportunity_Status", conn);
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("sp_Get_CRM_Oportunity_Status SqlCommand", EventLogEntryType.Information, 101, 1);
                            }
                            // 2. set the command object so it knows to execute a stored procedure
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@userid", user);
                            // 3. add parameter to command, which will be passed to the stored procedure
                            //cmd.Parameters.Add(new SqlParameter("@quote", "fsdfsd"));

                            // execute the command

                            SqlDataReader rdr = cmd.ExecuteReader();

                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "Application";
                                eventLog.WriteEntry("SqlDataReader", EventLogEntryType.Information, 101, 1);
                            }


                            // iterate through results, printing each to console
                            while (rdr.Read())
                            {
                                Model_sp_Get_CRM_Oportunity_Status m = new Model_sp_Get_CRM_Oportunity_Status();
                                m.CurrentStatus = rdr["CurrentStatus"].ToString();
                                m.TotalCount = rdr["TotalCount"].ToString();
                                lst.Add(m);
                            }


                            rdr.Close();

                            //using (SqlDataReader rdr = cmd.ExecuteReader())
                            //{
                            //    using (EventLog eventLog = new EventLog("Application"))
                            //    {
                            //        eventLog.Source = "Application";
                            //        eventLog.WriteEntry("SqlDataReader", EventLogEntryType.Information, 101, 1);
                            //    }
                            //    try
                            //    {

                            //        // iterate through results, printing each to console
                            //        while (rdr.Read())
                            //        {
                            //            Model_sp_Get_CRM_Oportunity_Status m = new Model_sp_Get_CRM_Oportunity_Status();
                            //            m.CurrentStatus = rdr["CurrentStatus"].ToString();
                            //            m.TotalCount = rdr["TotalCount"].ToString();
                            //            lst.Add(m);
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        using (EventLog eventLog = new EventLog("Application"))
                            //        {
                            //            eventLog.Source = "Application";
                            //            eventLog.WriteEntry("SqlDataReader" + ex.Message.ToString() + "----" + ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                            //        }
                            //    }

                            //}
                        }

                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry("dentro da query", EventLogEntryType.Information, 101, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";
                            eventLog.WriteEntry(ex.Message.ToString() + "----" + ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                        }
                    }


                
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.Message.ToString() + " - " + ex.InnerException.ToString(), EventLogEntryType.Information, 101, 1);
                }
            }
            return lst;
        }

        public bool ExportWorkbookToPdf(string workbookPath, string outputPath)
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
    }
    public sealed class SafeAccessTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeAccessTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }

}
