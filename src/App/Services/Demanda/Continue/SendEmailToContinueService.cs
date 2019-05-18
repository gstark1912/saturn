using Model.SurveyCompletion;
using Services;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace App.Services.Demanda.Continue
{
    public class SendEmailToContinueService
    {
        private EmailSender emailSender;

        public SendEmailToContinueService()
        {
            this.emailSender = new EmailSender();
        }

        public void Send(string to, string body, string attachmentFileName)
        {
            var from = ConfigurationManager.AppSettings["SMTPUsername"];
            var subject = "Evaluando Software - Proceso Incompleto";

            using (var message = new MailMessage(from, to))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);
                message.Attachments.Add(attachment);

                this.emailSender.Send(message);
            }
        }

        public string GetLink(SurveyCompletionParent surveyCompletionParent)
        {
            var link = string.Format("{0}/Continue?Category={1}&ParentSurveyCompletionId={2}&PartialSaveKey={3}",
                ConfigurationManager.AppSettings["URL"],
                surveyCompletionParent.Category.Id,
                surveyCompletionParent.Id,
                surveyCompletionParent.PartialSaveKey);

            return link;
        }
    }
}