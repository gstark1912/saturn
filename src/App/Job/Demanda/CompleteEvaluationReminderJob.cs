using App.Services.Demanda.Continue;
using Model.Context;
using Model.SurveyCompletion;
using Services;
using Services.Service.EmailService;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;

namespace App.Job.Demanda
{
    public class CompleteEvaluationReminderJob
    {
        private ModelContext modelContext;
        private SupplyCompleteRegistrationReminderEmailService supplyCompleteRegistrationReminderEmailService;
        private PdfService pdfService;
        private EmailSender emailSender;
        private SendEmailToContinueService sendEmailToContinueService;

        public CompleteEvaluationReminderJob()
        {
            this.modelContext = new ModelContext();
            this.supplyCompleteRegistrationReminderEmailService = new SupplyCompleteRegistrationReminderEmailService();
            this.pdfService = new PdfService();
            this.emailSender = new EmailSender();
            this.sendEmailToContinueService = new SendEmailToContinueService();
        }

        public void Execute()
        {
            var surveysCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include( "Role" )
                .Include( "Category" )
                .Where( x =>
                     x.Role.Name == "DEMANDA" &&
                     x.PartialSave == true &&
                     x.DeletedAt == null &&
                     x.PartialSaveReminderCount <= 3 )
                .Take( 20 )
                .ToList();

            foreach( var surveyCompletionParent in surveysCompletionParent )
            {
                var daysDiff = 0;
                var sendMail = false;

                daysDiff = ( DateTime.Now - surveyCompletionParent.CreatedAt ).Days;

                if( surveyCompletionParent.PartialSaveReminderCount == 0 && daysDiff >= 1 )
                {
                    sendMail = true;
                }

                if( surveyCompletionParent.PartialSaveReminderCount == 1 && daysDiff >= 2 )
                {
                    sendMail = true;
                }

                if( surveyCompletionParent.PartialSaveReminderCount == 2 && daysDiff >= 4 )
                {
                    sendMail = true;
                }

                if( surveyCompletionParent.PartialSaveReminderCount == 3 && daysDiff >= 5 )
                {
                    sendMail = true;
                }

                if( sendMail )
                {
                    this.SendEmail( surveyCompletionParent );

                    surveyCompletionParent.PartialSaveReminderCount++;
                    this.modelContext.SaveChanges();
                }
            }
        }

        private void SendEmail( SurveyCompletionParent surveyCompletionParent )
        {
            var mailMessage = this.GetMailMessage( surveyCompletionParent );

            this.emailSender.Send( mailMessage );
        }

        private MailMessage GetMailMessage( SurveyCompletionParent surveyCompletionParent )
        {
            var link = this.sendEmailToContinueService.GetLink( surveyCompletionParent );
            var email = ConfigurationManager.AppSettings["SMTPUsername"];

            var reminderCount = surveyCompletionParent.PartialSaveReminderCount + 1;
            var subject = $"Evaluando Software - Proyecto no finalizado (Reminder {reminderCount}/4)";

            string body = string.Format( "Su proyecto para obtener una recomencación está pendiente de finalización. Ingresando al siguiente link {0}, podrás terminarlo y recibir la recomendación de producto/ servicio que mejor se adapte a tus necesidades. <br /> <br /> Este servicio forma parte del Programa de Mejoramiento de la Industria, una herramienta de Evaluando Software para mejorar las condiciones de oferta y demanda. Para ti es gratuito", (object)link );

            MailMessage mailMessage = new MailMessage( email, email, subject, body )
            {
                IsBodyHtml = true
            };

            return mailMessage;
        }
    }
}