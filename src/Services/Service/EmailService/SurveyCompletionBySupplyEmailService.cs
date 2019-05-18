using Model.SurveyCompletion;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace Services.Service.EmailService
{
    public class SurveyCompletionBySupplyEmailService
    {
        private EmailSender emailSender;

        public SurveyCompletionBySupplyEmailService()
        {
            this.emailSender = new EmailSender();
        }

        public void Send(string attachmentFileName, SurveyCompletionParent surveyCompletionParent)
        {
            var companyMailMessage = this.GetCompanyMailMessage(surveyCompletionParent, attachmentFileName);
            var adminMailMessage = this.GetAdminMailMessage(attachmentFileName, surveyCompletionParent);

            this.emailSender.Send(companyMailMessage);
            this.emailSender.Send(adminMailMessage);
        }

        private MailMessage GetCompanyMailMessage(SurveyCompletionParent surveyCompletionParent, string attachmentFileName)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(surveyCompletionParent.Company.ComercialContact.Email));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Registro de Producto/Servicio";

            mailMessage.Body = "Estimado " + surveyCompletionParent.Company.ComercialContact.FullName + ":<br><br>";
            mailMessage.Body = "<p>Gracias por registrar su producto/servicio en Evaluando Software. El mismo se encuentra pendiente de aprobaci&oacute;n para aparecer en los resultados de nuestras evaluaciones.</p>";
            mailMessage.Body += "<p>Adjunto encontrar&aacute; un PDF con informaci&oacute;n registrada.</p>";
            mailMessage.Body += "<br><br> Lo saluda atentamente, <br>el equipo de <a href='evaluandosoftware.com' target='_blank'>EvaluandoSoftware.com</a>";

            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }

        private MailMessage GetAdminMailMessage(string attachmentFileName, SurveyCompletionParent surveyCompletionParent)
        {
            var email = ConfigurationManager.AppSettings["SMTPUsername"];
            var subject = "Oferta pendiente de autorización";
            var body = $"La empresa {surveyCompletionParent.Company.CompanyName} ingresó un nuevo producto/ servicio {surveyCompletionParent.Product.Name}";

            MailMessage mailMessage = new MailMessage(email, email, subject, body)
            {
                IsBodyHtml = true
            };

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }
    }
}