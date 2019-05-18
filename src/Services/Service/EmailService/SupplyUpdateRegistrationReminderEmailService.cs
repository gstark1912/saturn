using Model.SurveyCompletion;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Services.Service.EmailService
{
    public class SupplyUpdateRegistrationReminderEmailService
    {
        private EmailSender emailSender;

        public SupplyUpdateRegistrationReminderEmailService() 
        {
            this.emailSender = new EmailSender();    
        }

        public void Send(string attachmentFileName, SurveyCompletionParent surveyCompletionParent)
        {
            var companyMailMessage = this.GetMailMessage(surveyCompletionParent, attachmentFileName);

            this.emailSender.Send(companyMailMessage);
        }

        private MailMessage GetMailMessage(SurveyCompletionParent surveyCompletionParent, string attachmentFileName)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(surveyCompletionParent.Company.ComercialContact.Email));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Actualización de producto/servicio";

            var link = string.Format("{0}/Product/Evaluation/RegisterContinue?surveyCompletionId={1}&partialSaveKey={2}",
                ConfigurationManager.AppSettings["URL"],
                surveyCompletionParent.Id,
                surveyCompletionParent.PartialSaveKey);

            mailMessage.Body = string.Format("<p>La ultima actualizaci&oacute;n de su producto/servicio fue realizada hace 6 meces, le pedimos por favor ingrese al siguiente link para actualizar la información del mismo: {0}</p>", link);
            mailMessage.Body += "<p>Adjunto encontrar&aacute; un PDF con informaci&oacute;n registrada hasta el momento.</p>";

            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }
    }
}