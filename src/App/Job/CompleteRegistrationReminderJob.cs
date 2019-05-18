using Model.Context;
using Services;
using Services.Service.EmailService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace App.Job
{
    public class CompleteRegistrationReminderJob
    {
        private ModelContext modelContext;
        private SupplyCompleteRegistrationReminderEmailService supplyCompleteRegistrationReminderEmailService;
        private PdfService pdfService;

        public CompleteRegistrationReminderJob() 
        {
            this.modelContext = new ModelContext();
            this.supplyCompleteRegistrationReminderEmailService = new SupplyCompleteRegistrationReminderEmailService();
            this.pdfService = new PdfService();
        }

        public void Execute() 
        {
            var surveyCompletions = this.modelContext
                .SurveysCompletion
                .Include("Company")
                .Include("Role")
                .Where(x => 
                    x.Role.Name == "OFERTA" && 
                    x.PartialSave == true &&
                    x.DeletedAt == null &&
                    (
                        (x.CompleteReminderSentAt == null || DbFunctions.DiffDays(x.CreatedAt, DateTime.Now) >= 10) ||
                        (x.CompleteReminderSentAt != null && DbFunctions.DiffDays(x.CompleteReminderSentAt, DateTime.Now) >= 10))
                    )
                .Take(20)
                .ToList();

            foreach (var surveyCompletion in surveyCompletions) 
            {
                try
                {
                    var PdfName = this.pdfService.GetEvaluationFileName(surveyCompletion.Id);
                    this.supplyCompleteRegistrationReminderEmailService.Send(PdfName, surveyCompletion);
                }
                catch (Exception e)
                {
                    
                }
                finally 
                {
                    surveyCompletion.CompleteReminderSentAt = DateTime.Now;
                    this.modelContext.SaveChanges();
                }
            }
        }
    }
}