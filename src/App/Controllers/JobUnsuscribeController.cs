using Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    public class JobUnsuscribeController : Controller
    {
        private ModelContext modelContext;

        public JobUnsuscribeController() 
        {
            this.modelContext = new ModelContext();    
        }

        public ActionResult RegisterContinueReminder(int surveyCompletionId, string partialSaveKey)
        {
            var surveyCompletion = this.modelContext.SurveysCompletion
                .Include("Company")
                .FirstOrDefault(x =>
                    x.Id == surveyCompletionId &&
                    x.PartialSaveKey == partialSaveKey &&
                    x.DeletedAt == null);

            if (surveyCompletion == null)
            {
                throw new ArgumentException("No se encuentra la evaluación");
            }

            surveyCompletion.DeletedAt = DateTime.Now;
            this.modelContext.SaveChanges();

            return View("~/Views/Job/Unsuscribe.cshtml");
        }
    }
}