using Model.SurveyCompletion;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Services.Service.EmailService
{
    public class SurveyPartialCompletionBySupplyEmailService
    {
        private EmailSender emailSender;

        public SurveyPartialCompletionBySupplyEmailService() 
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
            mailMessage.Subject = "Evaluando Software - Proceso Incompleto";

            var link = string.Format("{0}://{1}/Product/Evaluation/RegisterContinue?surveyCompletionId={2}&partialSaveKey={3}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                surveyCompletionParent.Id,
                surveyCompletionParent.PartialSaveKey);

            mailMessage.Body = string.Format("<p>Gracias por registrar su producto/servicio en Evaluando Software. Para poder continuar con el registro de su producto/servicio debe acceder al siguiente link: {0}</p>", link);
            mailMessage.Body += "<p>Adjunto encontrar&aacute; un PDF con informaci&oacute;n registrada hasta el momento.</p>";

            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }
    }
}