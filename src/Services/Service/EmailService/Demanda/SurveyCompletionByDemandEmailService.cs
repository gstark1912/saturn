using Model.Customer;
using System.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace Services.Service.EmailService
{
    public class SurveyCompletionByDemandEmailService
    {
        private EmailSender emailSender;

        public SurveyCompletionByDemandEmailService()
        {
            this.emailSender = new EmailSender();
        }

        public void Send(string attachmentFileName, Customer customer)
        {
            var customerMailMessage = this.GetCustomerMailMessage(attachmentFileName, customer.Email);
            this.emailSender.Send(customerMailMessage);

            var adminMailMessage = this.GetAdminMailMessage(attachmentFileName, customer);
            this.emailSender.Send(adminMailMessage);
        }

        private MailMessage GetCustomerMailMessage(string attachmentFileName, string emailTo)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(emailTo));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Informe de Evaluación";
            mailMessage.Body = "<p>Gracias por completar la evaluación de Evaluando Software!</p><p>Adjunto encontrar&aacute; un PDF con el Informe de la Evaluaci&oacute;n.</p>";
            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);

            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }

        private MailMessage GetAdminMailMessage(string attachmentFileName, Customer customer)
        {
            var datos = string.Format(@"
            <table>
                <tr>
                    <td>Nombre y apellido</td>
                    <td>{0}</td>
                </tr>
                <tr>
                    <td>Email</td>
                    <td>{1}</td>
                </tr>
                <tr>
                    <td>País</td>
                    <td>{2}</td>
                </tr>
                <tr>
                    <td>Ciudad</td>
                    <td>{3}</td>
                </tr>
                <tr>
                    <td>Teléfono</td>
                    <td>{4}</td>
                </tr>
                <tr>
                    <td>Empresa</td>
                    <td>{5}</td>
                </tr>
                <tr>
                    <td>Cantiad de empleados</td>
                    <td>{6}</td>
                </tr>
                <tr>
                    <td>Facturación</td>
                    <td>{7}</td>
                </tr>
                <tr>
                    <td>Presupuesto</td>
                    <td>{8}</td>
                </tr>
            </table>",
            customer.FirstName + " " + customer.LastName,
            customer.Email,
            customer.Conutry,
            customer.City,
            customer.PhoneNumber,
            customer.Company,
            customer.PeopleInCompany,
            customer.AnnualBilling,
            customer.Budget);

            var mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]));
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            mailMessage.Subject = "Evaluando Software - Informe de Evaluación";
            mailMessage.Body = "<p>Se ha registrado una nueva evaluación!</p></br></br>" + datos;
            mailMessage.IsBodyHtml = true;

            var attachment = new Attachment(attachmentFileName, MediaTypeNames.Application.Pdf);

            mailMessage.Attachments.Add(attachment);

            return mailMessage;
        }
    }
}