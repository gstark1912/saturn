using Model.SurveyCompletion;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace Services.Service.EmailService
{
    public class SupplyCompleteRegistrationReminderEmailService
    {
        private EmailSender emailSender;

        public SupplyCompleteRegistrationReminderEmailService()
        {
            this.emailSender = new EmailSender();
        }

        public void Send(string attachmentFileName, SurveyCompletion surveyCompletion)
        {
            var companyMailMessage = this.GetMailMessage(surveyCompletion, attachmentFileName);

            this.emailSender.Send(companyMailMessage);
        }

        private MailMessage GetMailMessage(SurveyCompletion surveyCompletion, string attachmentFileName)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(surveyCompletion.Company.ComercialContact.Email));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Proceso Incompleto";

            var continuelink = string.Format("{0}/Product/Evaluation/RegisterContinue?surveyCompletionId={1}&partialSaveKey={2}",
                ConfigurationManager.AppSettings["URL"],
                surveyCompletion.Id,
                surveyCompletion.PartialSaveKey);

            var unsuscribeLink = string.Format("{0}/JobUnsuscribe/RegisterContinueReminder?surveyCompletionId={1}&partialSaveKey={2}",
                ConfigurationManager.AppSettings["URL"],
                surveyCompletion.Id,
                surveyCompletion.PartialSaveKey);

            mailMessage.Body = "<div>";
            mailMessage.Body += string.Format("<p>Le recordamos que el proceso de registro de su producto/servicio no ha sido finalizado. Para poder continuar con el proceso debe acceder al siguiente link: {0}</p>", continuelink);
            mailMessage.Body += "<p>Adjunto encontrar&aacute; un PDF con informaci&oacute;n registrada hasta el momento.</p>";
            mailMessage.Body += string.Format("<p style='font-size: 10px; color:gray'>Si no desea recibir mas este recordatorio, por favor ingrese al siguiente link {0}. Recuerde que solo a travez del link enviado en este correo podr&aacute; continuar con la carga de su producto/servicio sin perder los datos ya cargados.</p>", unsuscribeLink);
            mailMessage.Body += "</div>";

            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }
    }
}