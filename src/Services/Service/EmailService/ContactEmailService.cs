using Model.SurveyCompletion;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace Services.Service.EmailService
{
    public class ContactEmailService
    {
        private EmailSender emailSender;

        public ContactEmailService() 
        {
            this.emailSender = new EmailSender();    
        }

        public void Send(ContactDTO contactDTO)
        {
            var mailMessage = this.GetMailMessage(contactDTO);
            this.emailSender.Send(mailMessage);
        }

        private MailMessage GetMailMessage(ContactDTO contactDTO)
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
    }
}