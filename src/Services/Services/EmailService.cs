using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

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

    public class EmailService
    {
        #region SurveyCompletionByDemand

        public void SendEvaluationReport(string attachmentFileName, string emailTo)
        {
            var customerMailMessage = this.GetEvaluationReportCustomerMailMessage(attachmentFileName, emailTo);
            var adminMailMessage = this.GetEvaluationReportAdminMailMessage(attachmentFileName);

            this.Send(customerMailMessage);
            this.Send(adminMailMessage);
        }

        private MailMessage GetEvaluationReportCustomerMailMessage(string attachmentFileName, string emailTo)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(emailTo));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Informe de Evaluación";
            mailMessage.Body = "<p>Gracias por completar la evaluación de Evaluando Software!</p><p>Adjunto encontrará un PDF con el Informe de la Evaluación.</p>";
            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);

            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }

        private MailMessage GetEvaluationReportAdminMailMessage(string attachmentFileName)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Informe de Evaluación";
            mailMessage.Body = "<p>Se ha registrado una nueva evaluación!</p>";
            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);

            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }

        #endregion SurveyCompletionByDemand

        #region SurveyCompletionBySupply

        public void SendSupplySurveyCompletionEmail(string attachmentFileName, string emailTo)
        {
            var companyMailMessage = this.GetSupplySurveyCompletionCompanyMailMessage(emailTo);
            var adminMailMessage = this.GetSupplySurveyCompletionAdminMailMessage(attachmentFileName);

            this.Send(companyMailMessage);
            this.Send(adminMailMessage);
        }

        private MailMessage GetSupplySurveyCompletionCompanyMailMessage(string emailTo)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(emailTo));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Registro de Producto";

            mailMessage.Body = "<p>Gracias por registrar su producto, el mismo se encuentra pendiente de aprobación.</p>";

            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }

        private MailMessage GetSupplySurveyCompletionAdminMailMessage(string attachmentFileName)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Registro de Producto";

            mailMessage.Body = "<p>Se ha registrado un nuevo producto.</p>";
            
            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);

            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }

        #endregion SurveyCompletionBySupply

        #region SurveySupplyApprove

        public void SendSurveySupplyApproveEmail(string emailTo)
        {
            var mailMessage = this.GetSurveySupplyApproveMailMessage(emailTo);

            this.Send(mailMessage);
        }

        private MailMessage GetSurveySupplyApproveMailMessage(string emailTo)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(emailTo));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Registro de Producto Aprobado";

            mailMessage.Body = "<p>El producto ha sido de aprobado. <br /> A partir de ahora aparecerá en los informes de evaluación.</p>";

            mailMessage.IsBodyHtml = true;

            return mailMessage;
        }

        #endregion SurveySupplyApprove

        #region Contact

        public void SendContactEmail(ContactDTO contactDTO)
        {
            var mailMessage = this.GetContactMailMessage(contactDTO);
            this.Send(mailMessage);
        }

        private MailMessage GetContactMailMessage(ContactDTO contactDTO)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Contacto";
            
            mailMessage.Body = "";
            mailMessage.Body += "<table>";
            mailMessage.Body += "<tr><th>Nombre</th><td>" + contactDTO.FirstName + "</td></tr>";
            mailMessage.Body += "<tr><th>Apellido</th><td>" + contactDTO.LastName + "</td></tr>";
            mailMessage.Body += "<tr><th>Empresa</th><td>" + contactDTO.Company + "</td></tr>";
            mailMessage.Body += "<tr><th>Email</th><td>" + contactDTO.Email + "</td></tr>";
            mailMessage.Body += "<tr><th>Motivo</th><td>" + contactDTO.Reason + "</td></tr>";
            mailMessage.Body += "<tr><th>Mensaje</th><td>" + contactDTO.Message + "</td></tr>";
            mailMessage.Body += "</table>";
            
            mailMessage.IsBodyHtml = true;
            
            return mailMessage;
        }

        #endregion Contact

        private void Send(MailMessage mailMessage) 
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Host = ConfigurationManager.AppSettings["SMTPHost"];
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                smtp.EnableSsl = ConfigurationManager.AppSettings["SMTPEnableSsl"] == "true";

                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["SMTPUsername"],
                    Password = ConfigurationManager.AppSettings["SMTPPassword"]
                };

                smtp.Credentials = credential;

                ServicePointManager.ServerCertificateValidationCallback =
               delegate (object s
                   , X509Certificate certificate
                   , X509Chain chai
                   , SslPolicyErrors sslPolicyErrors)
               { return true; };

                smtp.Send(mailMessage);
            }
        }
    }
}