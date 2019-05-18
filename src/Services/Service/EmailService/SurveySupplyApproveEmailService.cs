using Model.SurveyCompletion;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Services.Service.EmailService
{
    public class SurveySupplyApproveEmailService
    {
        private EmailSender emailSender;

        public SurveySupplyApproveEmailService() 
        {
            this.emailSender = new EmailSender();    
        }

        public void Send(string attachmentFileName, SurveyCompletionParent surveyCompletionParent)
        {
            var mailMessage = this.GetMailMessage(surveyCompletionParent, attachmentFileName);

            this.emailSender.Send(mailMessage);
        }

        private MailMessage GetMailMessage(SurveyCompletionParent surveyCompletionParent, string attachmentFileName)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(surveyCompletionParent.Company.ComercialContact.Email));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Registro de Producto/Servicio Aprobado";

            var link = string.Format("{0}://{1}/Product/Evaluation/RegisterContinue?surveyCompletionId={2}&partialSaveKey={3}",
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Host,
                surveyCompletionParent.Id,
                surveyCompletionParent.PartialSaveKey);

            mailMessage.Body = "Estimado " + surveyCompletionParent.Company.ComercialContact.FullName + ":<br><br>";
            mailMessage.Body = "<p>El producto/servicio ha sido de aprobado. <br /> A partir de ahora aparecer&aacute; en los informes de nuestras evaluaciones.</p>";
            mailMessage.Body += string.Format("<p>En caso de querer realizar alguna modificaci&oacute;n sobre el registro, podr&aacute; hacerlo desde la web</p>");
            mailMessage.Body += "<br><br> Lo saluda atentamente, <br>el equipo de <a href='evaluandosoftware.com' target='_blank'>EvaluandoSoftware.com</a>";

            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }
    }
}