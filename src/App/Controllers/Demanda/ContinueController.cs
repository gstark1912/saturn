using App.Attribute;
using App.DTO;
using App.Models.Demanda.Continue.Email;
using App.Models.Survey;
using App.Services.Demanda.Continue;
using Model.Context;
using Model.SurveyCompletion;
using Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers.Demanda
{
    public class ContinueController : Controller
    {
        private ModelContext modelContext;
        private SendEmailToContinueService sendEmailToContinueService;
        private PdfService pdfService;

        public ContinueController()
        {
            this.modelContext = new ModelContext();
            this.sendEmailToContinueService = new SendEmailToContinueService();
            this.pdfService = new PdfService();
        }

        // GET: Continue?Category=141&ParentSurveyCompletionId=337&PartialSaveKey=f681a306-9dfe-49b5-8d83-671bea112f43
        [HttpGet]
        public ActionResult Index(int Category, int ParentSurveyCompletionId, string PartialSaveKey)
        {
            ViewBag.Category = Category;
            ViewBag.ParentSurveyCompletionId = ParentSurveyCompletionId;
            ViewBag.PartialSaveKey = PartialSaveKey;

            var outdatedSurveys = new Dictionary<int, bool>();

            var surveyCompletionParent = modelContext
                .SurveyCompletionParent
                .Include("SurveyCompletions")
                .Include("SurveyCompletions.CategoryObj")
                .Include("SurveyCompletions.Questions")
                .FirstOrDefault(x =>
                    x.Id == ParentSurveyCompletionId
                    && x.PartialSaveKey == PartialSaveKey);

            if (surveyCompletionParent == null)
            {
                throw new System.ArgumentException("Evaluation not found");
            }

            foreach (var surveyCompletion in surveyCompletionParent.SurveyCompletions)
            {
                var answeredQuestionIds = surveyCompletion
                    .Questions
                    .ToList()
                    .Select(z => z.QuestionId);

                var questions = modelContext
                    .Questions
                    .Include("Survey")
                    .Where(x =>
                        x.Survey.Id == surveyCompletion.SurveyId
                        && !answeredQuestionIds.Contains(x.Id)
                        && x.Old == false)
                    .ToList();

                outdatedSurveys.Add(surveyCompletion.Id, questions.Count() > 0);
            }

            ViewBag.surveys = surveyCompletionParent.SurveyCompletions;
            ViewBag.outdatedSurveys = outdatedSurveys;

            return View("~/Views/Demanda/Continue/CategoryList.cshtml");
        }

        // POST: Continue?Category=141&ParentSurveyCompletionId=337&PartialSaveKey=f681a306-9dfe-49b5-8d83-671bea112f43
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(int Category, int ParentSurveyCompletionId, string PartialSaveKey)
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Single(x => x.Id == ParentSurveyCompletionId);

            surveyCompletionParent.PartialSave = false;

            this.modelContext.SaveChanges();

            return RedirectToAction("../../Evaluation/Registration", new { surveyCompletionParentId = ParentSurveyCompletionId });
        }

        // POST: Continue?Category=141&ParentSurveyCompletionId=337&PartialSaveKey=f681a306-9dfe-49b5-8d83-671bea112f43
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "SaveParcial")]
        public ActionResult SavePartial(int Category, int ParentSurveyCompletionId, string PartialSaveKey)
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include("Category")
                .Single(x =>
                    x.Id == ParentSurveyCompletionId
                    && x.PartialSaveKey == PartialSaveKey);

            //Get email body
            var link = this.sendEmailToContinueService.GetLink(surveyCompletionParent);
            var body = this.RenderRazorViewToString("~/Views/Demanda/Continue/Email/Continue.cshtml", new ContinueViewModel { Link = link });

            //Generate PDF
            var template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationTemplate.cshtml", surveyCompletionParent);
            var fileName = this.pdfService.GetEvaluationFileName(surveyCompletionParent.Id);
            var pdfFullName = this.pdfService.Generate(surveyCompletionParent.Id, template, fileName);

            this.sendEmailToContinueService.Send(surveyCompletionParent.Email, body, pdfFullName);

            return RedirectToAction("ThanksPartial", "../Product/Evaluation");
        }

        // GET: Continue/Update?Category=141&ParentSurveyCompletionId=337&PartialSaveKey=f681a306-9dfe-49b5-8d83-671bea112f43
        [HttpGet]
        public ActionResult Update(int Category, int ParentSurveyCompletionId, string PartialSaveKey, int SurveyCompletionId)
        {
            var surveysCompletion = modelContext
                .SurveysCompletion
                .Include("CategoryObj")
                .Include("Questions")
                .FirstOrDefault(x => x.Id == SurveyCompletionId);

            var questions = modelContext
                .Questions
                .Include("Survey")
                .Where(x => x.Survey.Id == surveysCompletion.SurveyId)
                .Select(x => new SurveyQuestionDTO
                {
                    QuestionId = x.Id,
                    Question = x.DemandQuestion,
                    Required = x.DemandRequired,
                    Old = x.Old
                })
                .ToList();

            ViewBag.survey = surveysCompletion;
            ViewBag.Category = Category;
            ViewBag.ParentSurveyCompletionId = ParentSurveyCompletionId;
            ViewBag.PartialSaveKey = PartialSaveKey;
            ViewBag.SurveyCompletionId = SurveyCompletionId;

            var model = new SurveyUpdateViewModel()
            {
                SurveyQuestionDTOs = questions,
            };

            return View("~/Views/Demanda/Continue/CategoryUpdate.cshtml", model);
        }

        public ActionResult Question(int id, int identifier, int surveyCompletionId)
        {
            var question = this.modelContext
                .Questions
                .Include("AnswerType")
                .Include("Answers")
                .Where(x => x.Id == id)
                .FirstOrDefault();

            var surveyCompletion = this.modelContext
                .SurveysCompletion
                .Include("Questions")
                .Include("Questions.Answers")
                .FirstOrDefault(x => x.Id == surveyCompletionId);

            var chosenAnswers = new List<AnswerDTO>();

            if (surveyCompletion != null)
            {
                var surveyCompletionQuestion = surveyCompletion
                    .Questions
                    .FirstOrDefault(x => x.QuestionId == id);

                if (surveyCompletionQuestion != null)
                {
                    chosenAnswers = surveyCompletionQuestion
                        .Answers
                        .Select(answer => new AnswerDTO
                        {
                            Id = answer.Id,
                            Answer = answer.Answer
                        })
                        .ToList();
                }
            }

            var model = new QuestionViewModel
            {
                Identifier = identifier,
                Id = question.Id,
                Question = question.DemandQuestion,
                Required = question.DemandRequired,
                Type = question.AnswerType.Name,
                Answers = question.Answers
                    .Select(answer => new AnswerDTO
                    {
                        Id = answer.Id,
                        Answer = answer.DemandAnswer
                    })
                    .ToList(),
                ChosenAnswers = chosenAnswers
            };

            var view = "~/Views/Survey/Question/" + question.AnswerType.Name + ".cshtml";

            return PartialView(view, model);
        }

        // POST: Continue/Update
        [HttpPost]
        public ActionResult Update(SurveyViewModel model)
        {
            var surveysCompletions = modelContext
                .SurveysCompletion
                .Include("CategoryObj")
                .Include("Questions")
                .FirstOrDefault(x => x.Id == model.SurveyCompletionId);

            var questions = modelContext
                .Questions
                .Include("Survey")
                .Include("Answers")
                .Where(x => x.Survey.Id == surveysCompletions.SurveyId)
                .ToList();

            var surveysCompletion = modelContext
                .SurveysCompletion
                .Include("CategoryObj")
                .Include("Questions")
                .Include("Questions.Answers")
                .FirstOrDefault(x => x.Id == model.SurveyCompletionId);

            foreach (var DTO in model.surveyCompletionDTOs)
            {
                if (DTO.Answers != null)
                {
                    bool nueva = false;
                    var question = surveysCompletion
                        .Questions
                        .FirstOrDefault(x => x.QuestionId == DTO.QuestionId);

                    if (question == null)
                    {
                        question = new SurveyCompletionQuestion();
                        nueva = true;
                    }

                    question.Question = DTO.Question;
                    question.QuestionId = DTO.QuestionId;

                    if (!nueva && question.Answers != null && question.Answers.Count > 0)
                    {
                        question.Answers.Clear();
                    }

                    question.Answers = questions
                        .Where(x => x.Id == DTO.QuestionId)
                        .FirstOrDefault()
                        .Answers
                        .Where(x => DTO.Answers.Contains(x.Id))
                        .Select(x => new SurveyCompletionAnswer
                        {
                            Answer = x.DemandAnswer,
                            AnswerValue = x.Value
                        })
                        .ToList();

                    if (nueva)
                    {
                        surveysCompletion.Questions.Add(question);
                    }
                }
            }

            modelContext.SaveChanges();

            return RedirectToAction("../../Continue", new
            {
                Category = model.CategoryId,
                ParentSurveyCompletionId = model.ParentSurveyCompletionId,
                PartialSaveKey = model.PartialSaveKey
            });
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(
                    ControllerContext,
                    viewName);

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}