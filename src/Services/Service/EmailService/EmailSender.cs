using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Services
{
    public class ContactDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
    }

    public class EmailSender
    {
        public void Send( MailMessage mailMessage )
        {
            using( var smtp = new SmtpClient() )
            {
                smtp.Host = ConfigurationManager.AppSettings["SMTPHost"];
                smtp.Port = Convert.ToInt32( ConfigurationManager.AppSettings["SMTPPort"] );
                smtp.EnableSsl = ConfigurationManager.AppSettings["SMTPEnableSsl"] == "true";
                smtp.UseDefaultCredentials = false;
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["SMTPUsername"],
                    Password = ConfigurationManager.AppSettings["SMTPPassword"]
                };

                smtp.Credentials = credential;

                ServicePointManager.ServerCertificateValidationCallback =
                    delegate ( object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                             X509Chain chain, SslPolicyErrors sslPolicyErrors )
                    { return true; };
                try
                {
                    smtp.Send( mailMessage );
                }
                catch( Exception e )
                {
                    //TODO: log the generated exception
                }
            }
        }
    }
}