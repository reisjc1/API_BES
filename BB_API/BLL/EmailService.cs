using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication1.BLL
{
    public class EmailService
    {
        public async Task SendEmailaync(EmailMesage message)
        {

            //SmtpClient client = new SmtpClient("owa.konicaminolta.eu", 25);
            //client.EnableSsl = true;

            //SmtpClient client = new SmtpClient("smtp.office365.com", 25);
            //client.EnableSsl = true;
            //client.Credentials = new NetworkCredential("bbservice@konicaminolta.pt", "Portugal.123");

            SmtpClient client = new SmtpClient("kmlexc60.kme.intern", 25);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("bbservice@konicaminolta.pt", "Portugal.123");
            client.EnableSsl = true;
            client.TargetName = "STARTTLS/smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);


            //message.Destination = "joao.reis@konicaminolta.pt";

            MailMessage message1 = new MailMessage();
            message1.From = new MailAddress("bbservice@konicaminolta.pt", String.Empty, System.Text.Encoding.UTF8);
            List<string> destinationArray = message.Destination.Split(';').ToList();

            foreach (string s in destinationArray)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    message1.To.Add(new MailAddress(s, String.Empty, System.Text.Encoding.UTF8));
                }

            }

            //message1.Bcc.Add("bb@konicaminolta.pt");
            message1.Body = message.Body;

            if (message.CC != null && message.CC.Length > 0)
                message1.CC.Add(message.CC);

            message1.BodyEncoding = System.Text.Encoding.UTF8;
            message1.Subject = message.Subject;
            message1.SubjectEncoding = System.Text.Encoding.UTF8;
            message1.IsBodyHtml = true;

            try
            {
                client.Send(message1);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }


        }

        private static bool RemoteServerCertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            //Console.WriteLine(certificate);
            return true;
        }
        public async Task SendEmailayncTeste()
        {

            EmailMesage message = new EmailMesage();

            message.Destination = "joao.reis@konicaminolta.pt";
            message.Subject = "Teste1";
            //SmtpClient client = new SmtpClient("owa.konicaminolta.eu", 25);
            //client.EnableSsl = true;

            SmtpClient client = new SmtpClient("kmlexc60.kme.intern", 25);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("bbservice@konicaminolta.pt", "Portugal.123");
            client.EnableSsl = true;
            client.TargetName = "STARTTLS/smtp.office365.com";
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(RemoteServerCertificateValidationCallback);

            MailMessage message1 = new MailMessage();
            message1.From = new MailAddress("bbservice@konicaminolta.pt", String.Empty, System.Text.Encoding.UTF8);
            List<string> destinationArray = message.Destination.Split(';').ToList();
            foreach (string s in destinationArray)
            {
                message1.To.Add(new MailAddress(s, String.Empty, System.Text.Encoding.UTF8));
            }

            //message1.Bcc.Add("bb@konicaminolta.pt");
            message1.Body = message.Body;

            if (message.CC != null && message.CC.Length > 0)
                message1.CC.Add(message.CC);

            message1.BodyEncoding = System.Text.Encoding.UTF8;
            message1.Subject = message.Subject;
            message1.SubjectEncoding = System.Text.Encoding.UTF8;
            message1.IsBodyHtml = true;

            try
            {
                client.Send(message1);

            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {

                    eventLog.Source = "Application";
                    eventLog.WriteEntry("enviar email: " + ex.Message.ToString(), EventLogEntryType.Information, 101, 1);
                    ex.ToString();
                }
            }


        }

        public async Task SendEmailayncWithAttachement(EmailMesage message, string file)
        {
            List<string> terms = new List<string>();
            terms.Add(@"D:\DocumentPrinting\Terms\1. Condições Gerais.pdf");
            terms.Add(@"D:\DocumentPrinting\Terms\2. Contrato de Compra e Venda.pdf");
            try
            {
                SmtpClient client = new SmtpClient("owa.konicaminolta.eu", 25);
                client.EnableSsl = true;
                //client.Credentials = new System.Net.NetworkCredential("joao.reis@konicaminolta.pt", "Password123");
                MailAddress from = new MailAddress("businessbuilder@konicaminolta.pt", String.Empty, System.Text.Encoding.UTF8);
                MailAddress to = new MailAddress(message.Destination);
                MailMessage message1 = new MailMessage(from, to);
                message1.Body = message.Body;

                if (message.CC != "")
                    message1.CC.Add(message.CC);

                message1.BodyEncoding = System.Text.Encoding.UTF8;
                message1.Subject = message.Subject;
                message1.SubjectEncoding = System.Text.Encoding.UTF8;
                message1.IsBodyHtml = true;

                //string file = @"D:\LeaseDesk\Documentation\Popoup.png";

                Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                // Add time stamp information for the file.
                ContentDisposition disposition = data.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(file);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                // Add the file attachment to this email message.
                message1.Attachments.Add(data);

                foreach (var i in terms)
                {
                    Attachment data1 = new Attachment(i, MediaTypeNames.Application.Octet);
                    message1.Attachments.Add(data1);
                }




                client.Send(message1);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }


        }
    }

    public class EmailMesage
    {
        public string Destination { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }

        public string CC { get; set; }
    }
}