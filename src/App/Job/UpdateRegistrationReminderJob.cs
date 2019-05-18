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
    public class UpdateRegistrationReminderJob
    {
        private ModelContext modelContext;
        private SupplyUpdateRegistrationReminderEmailService supplyUpdateRegistrationReminderEmailService;
        private PdfService pdfService;

        public UpdateRegistrationReminderJob() 
        {
            this.modelContext = new ModelContext();
            this.supplyUpdateRegistrationReminderEmailService = new SupplyUpdateRegistrationReminderEmailService();
            this.pdfService = new PdfService();
        }

        public void Execute() 
        {
            var surveyCompletions = this.modelContext
                .SurveyCompletionParent
                .Include("Company")
                .Include("Role")
                .Where(x => 
                    x.Role.Name == "OFERTA" && 
                    x.PartialSave == false &&
                    x.DeletedAt == null &&
                    x.Status == "Aprobado" &&
                    (
                        (x.UpdateReminderSentAt == null && DbFunctions.DiffMonths(x.CreatedAt, DateTime.Now) >= 6) ||
                        (x.UpdateReminderSentAt != null && DbFunctions.DiffMonths(x.UpdateReminderSentAt, DateTime.Now) >= 6))
                    )
                .Take(20)
                .ToList();

            var existingCategories = this.modelContext
                .Categories
                .Select(x => x.Id)
                .ToList(); 

            foreach (var surveyCompletionParent in surveyCompletions) 
            {
                try
                {
                    if (existingCategories.Contains(surveyCompletionParent.Category.Id)) 
                    {
                        var PdfName = this.pdfService.GetEvaluationFileName(surveyCompletionParent.Id);
                        this.supplyUpdateRegistrationReminderEmailService.Send(PdfName, surveyCompletionParent);
                    }
                }
                catch (Exception e)
                {
                    
                }
                finally 
                {
                    surveyCompletionParent.UpdateReminderSentAt = DateTime.Now;
                    this.modelContext.SaveChanges();
                }
            }
        }
    }
}