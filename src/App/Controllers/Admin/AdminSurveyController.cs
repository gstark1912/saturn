using App.Models;
using App.Models.Admin;
using Model.Context;
using Model.Suvery;
using Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers.Admin
{
    [MyAuthorize(Roles = "Admin")]
    public class AdminSurveyController : Controller
    {
        public ModelContext modelContext;
        public SurveyService surveyService;

        public AdminSurveyController()
        {
            this.modelContext = new ModelContext();
            this.surveyService = new SurveyService();
        }

        public ActionResult Index()
        {
            var categories = this.modelContext
                .Surveys
                .Include("Category")
                .Where(x => x.Category.parentCategory == null)
                .Select(x => x.Category)
                .ToList();

            ViewBag.Categories = categories;

            return View("~/Views/Admin/Survey/List.cshtml");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("~/Views/Admin/Survey/Create.cshtml");
        }

        [HttpPost]
        public ActionResult Create(SurveyViewModel surveyViewModel)
        {
            if (!surveyViewModel.SurveyFile.FileName.EndsWith(".xlsx"))
            {
                ModelState.AddModelError("SurveyFile", "Seleccione un archivo XLSX. (Archivo Excel versión 2007 o superior)");

                return View("~/Views/Admin/Survey/Create.cshtml", surveyViewModel);
            }

            byte[] fileBytes = new byte[surveyViewModel.SurveyFile.ContentLength];
            var data = surveyViewModel.SurveyFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(surveyViewModel.SurveyFile.ContentLength));

            var stream = surveyViewModel.SurveyFile.InputStream;

            /*var errors = this.surveyService.ValidateCreation(stream);

            if (errors.Count() > 0)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("SurveyFile", error);
                }

                return View("~/Views/Admin/Survey/Create.cshtml", surveyViewModel);
            }
            */
            var survey = this.surveyService.Create(stream);

            this.modelContext.Surveys.AddRange(survey);
            this.modelContext.SaveChanges();

            return RedirectToAction("Index", "../Admin/Evaluation");
        }

        [HttpGet]
        public ActionResult EditOld(int id)
        {
            var model = new SurveyEditViewModel
            {
                CategoryId = id
            };

            return View("~/Views/Admin/Survey/Edit.cshtml", model);
        }

        [HttpPost]
        public ActionResult Edit(SurveyEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Survey/Edit.cshtml", model);
            }

            this.surveyService.Remove(model.CategoryId);

            byte[] fileBytes = new byte[model.SurveyFile.ContentLength];
            var data = model.SurveyFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(model.SurveyFile.ContentLength));

            var stream = model.SurveyFile.InputStream;
            var survey = this.surveyService.Create(stream);

            this.modelContext.Surveys.AddRange(survey);
            this.modelContext.SaveChanges();

            return RedirectToAction("Index", "../Admin/Evaluation");
        }

        public ActionResult Remove(int id)
        {
            this.surveyService.Remove(id);

            return RedirectToAction("Index", "../Admin/Evaluation");
        }

        public ActionResult View(int id)
        {
            ViewBag.Survey = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Category.parentCategory.parentCategory.parentCategory.parentCategory")
                .Include("Questions")
                .Include("Questions.Answers")
                .FirstOrDefault(x => x.Category.Id == id);

            ViewBag.SubSurveys = this.modelContext
               .Surveys
               .Include("Category")
               .Include("Category.parentCategory")
               .Include("Questions")
               .Where(x => x.Category.parentCategory.Id == id).ToList();

            return View("~/Views/Admin/Survey/View.cshtml");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Survey = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Category.parentCategory.parentCategory.parentCategory.parentCategory")
                .Include("Questions")
                .Include("Questions.Answers")
                .FirstOrDefault(x => x.Category.Id == id);

            ViewBag.SubSurveys = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Category.parentCategory")
                .Include("Questions")
                .Where(x => x.Category.parentCategory.Id == id).ToList();

            return View("~/Views/Admin/Survey/Edit.cshtml");
        }

        [HttpGet]
        public ActionResult DeleteQuestion(int id, int questionId)
        {
            var question = this.modelContext
                .Questions
                .FirstOrDefault(x => x.Id == questionId);

            question.Old = true;

            this.modelContext.SaveChanges();

            //Check success delete

            return RedirectToAction("Edit/" + id);
        }

        [HttpGet]
        public ActionResult EditQuestion(int id)
        {
            var question = this.modelContext
                .Questions
                .Include("Survey")
                .Include("Answers")
                .Include("AnswerType")
                .FirstOrDefault(x => x.Id == id);

            var AnswerTypes = this.modelContext
                .AnswerTypes
                .ToList();

            ViewBag.AnswerTypesList = new SelectList(AnswerTypes, "Id", "Name");

            return View("~/Views/Admin/Survey/EditQuestion.cshtml", question);
        }

        [HttpGet]
        public ActionResult createQuestion(int id)
        {
            var survey = this.modelContext
                    .Surveys
                    .FirstOrDefault(x => x.Id == id);

            var AnswerTypes = this.modelContext
                .AnswerTypes
                .ToList();

            ViewBag.AnswerTypesList = new SelectList(AnswerTypes, "Id", "Name");

            return View("~/Views/Admin/Survey/CreateQuestion.cshtml", new Question() { Survey = survey });
        }

        [HttpPost]
        public ActionResult EditQuestion(Question question)
        {
            var origQuestion = this.modelContext
                .Questions
                .Include("Answers")
                .Include("Survey")
                .Include("AnswerType")
                .FirstOrDefault(x => x.Id == question.Id);

            var survey = this.modelContext
                .Surveys
                .Include("Category")
                .FirstOrDefault(x => x.Id == question.Survey.Id);

            if (origQuestion != null)
            {
                origQuestion.Old = true;
            }

            question.Survey = survey;
            question.AnswerType = this.modelContext.AnswerTypes.FirstOrDefault(x => x.Id == question.AnswerTypeId);
            question.Answers = question.Answers.Where(x => x.Id > -1).ToList();

            this.modelContext.Questions.Add(question);

            this.modelContext.SaveChanges();

            return RedirectToAction("Edit/" + survey.Category.Id);
        }

        [HttpPost]
        public ActionResult createQuestion(Question question)
        {
            var survey = this.modelContext
                .Surveys
                .Include("Category")
                .FirstOrDefault(x => x.Id == question.Survey.Id);

            question.Survey = survey;
            question.AnswerType = this.modelContext.AnswerTypes.FirstOrDefault(x => x.Id == question.AnswerTypeId);
            question.Answers = question.Answers.Where(x => x.Id > -1).ToList();

            this.modelContext.Questions.Add(question);

            this.modelContext.SaveChanges();

            return RedirectToAction("Edit/" + survey.Category.Id);
        }
    }
}