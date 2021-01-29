using App.Attribute;
using App.DTO;
using App.Model.User;
using App.Models.Demanda.Continue.Email;
using App.Models.Survey;
using App.Service.Demanda;
using App.Services.Demanda.Continue;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model.Context;
using Model.Customer;
using Model.Model.EvaluationReport;
using Model.SurveyCompletion;
using Model.Suvery;
using Services;
using Services.Service.EmailService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Controllers.Demanda
{
    public class SurveyController : Controller
    {
        private ModelContext modelContext;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<ApplicationRole> roleManager;
        private SurveyCompletionByDemandEmailService surveyCompletionByDemandEmailService;
        private PdfService pdfService;
        private RankingService rankingService;
        private SurveyService surveyService;
        private EvaluationService evaluationService;
        private SendEmailToContinueService sendEmailToContinueService;

        public SurveyController()
        {
            this.modelContext = new ModelContext();

            var userStore = new UserStore<ApplicationUser>(this.modelContext);
            var roleStore = new RoleStore<ApplicationRole>(this.modelContext);

            this.userManager = new UserManager<ApplicationUser>(userStore);
            this.roleManager = new RoleManager<ApplicationRole>(roleStore);

            this.surveyCompletionByDemandEmailService = new SurveyCompletionByDemandEmailService();
            this.pdfService = new PdfService();
            this.rankingService = new RankingService();
            this.surveyService = new SurveyService();
            this.evaluationService = new EvaluationService();
            this.sendEmailToContinueService = new SendEmailToContinueService();
        }

        public ActionResult Index(int Category, string subCategories)
        {
            List<SurveyDTO> surveys = new List<SurveyDTO>();

            List<int> subcategories_Ids = new List<int>();

            foreach (string s in subCategories.Split(','))
                if (s != "")
                    subcategories_Ids.Add(int.Parse(s));

            renderSubCategory(Category, surveys, subcategories_Ids);

            var model = new SurveyViewModel
            {
                SurveyDTOs = surveys,
                CategoryId = Category,
                SurveyId = surveys.Where(x => x.CategoryId == Category).FirstOrDefault().SurveyId
            };

            return View("~/Views/Demanda/Survey.cshtml", model);
        }

        private void renderSubCategory(int CategoryId, List<SurveyDTO> surveys, List<int> subcategories_Ids)
        {
            //primero me agrego a mi mismo
            var survey = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Questions")
                .Where(x => x.Category.Id == CategoryId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var questions = survey.Questions
                .Select(question => new SurveyQuestionDTO
                {
                    QuestionId = question.Id,
                    Question = question.DemandQuestion,
                    Required = question.DemandRequired,
                    Old = question.Old
                })
                .ToList();

            ViewBag.QuestionCount += questions.Count();

            SurveyDTO surveyDTO = new DTO.SurveyDTO()
            {
                CategoryId = CategoryId,
                CategoryName = survey.Category.getFullName(),
                SurveyQuestionDTOs = questions,
                SurveyId = survey.Id
            };

            surveys.Add(surveyDTO);

            //busco las subSurveys de las subcategorias hijas y que estèn incluidas en el array de seleccioadas
            var subSurveys = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Questions")
                .Where(x => x.Category.parentCategory.Id == CategoryId && subcategories_Ids.Contains(x.Category.Id)).ToList();

            foreach (Survey subSurvey in subSurveys)
            {
                renderSubCategory(subSurvey.Category.Id, surveys, subcategories_Ids);
            }
        }

        public ActionResult Question(int id, int identifier)
        {
            var question = this.modelContext
                .Questions
                .Include("AnswerType")
                .Include("Answers")
                .Where(x => x.Id == id)
                .FirstOrDefault();

            var model = new QuestionViewModel
            {
                Identifier = identifier,
                Id = question.Id,
                Question = question.DemandQuestion,
                Required = question.DemandRequired,
                Type = question.AnswerType.Name,
                Answers = question.Answers.Select(answer => new AnswerDTO
                {
                    Id = answer.Id,
                    Answer = answer.DemandAnswer
                })
                    .ToList()
            };

            var view = "~/Views/Survey/Question/" + question.AnswerType.Name + ".cshtml";

            return PartialView(view, model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(SurveyViewModel model, string Argument)
        {
            string sourceQueryStringKey = ConfigurationManager.AppSettings["UTMSource"];
            if (Session[sourceQueryStringKey] != null)
            {
                model.Source = Session[sourceQueryStringKey].ToString();
                Session[sourceQueryStringKey] = null;
            }
            var surveyCompletionParent = this.evaluationService.Save(model, false, User.Identity.GetUserId());

            var surveyCompletion = this.modelContext
                .SurveyCompletionParent
                .FirstOrDefault(x => x.Id == surveyCompletionParent.Id);

            var customer = this.modelContext
                .Customers
                .Where(x => x.Email == model.Email)
                .FirstOrDefault();

            if (customer != null)
            {
                surveyCompletion.Customer = customer;
                this.modelContext.SaveChanges();

                return RedirectToAction("Thanks", "../Evaluation", new { surveyCompletionId = surveyCompletionParent.Id });
            }

            return RedirectToAction("Registration", "../Evaluation", new { surveyCompletionParentId = surveyCompletionParent.Id });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "SaveParcial")]
        public ActionResult SaveParcial(SurveyViewModel model, string Argument)
        {
            string sourceQueryStringKey = ConfigurationManager.AppSettings["UTMSource"];
            if (Session[sourceQueryStringKey] != null)
            {
                model.Source = Session[sourceQueryStringKey].ToString();
                Session[sourceQueryStringKey] = null;
            }
            var surveyCompletionParent = this.evaluationService.Save(model, true, User.Identity.GetUserId());

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

        private SurveyCompletionParent InsertSurveyCompletion(SurveyViewModel model, bool partial)
        {
            var category = this.modelContext
                .Categories
                .Where(x => x.Id == model.CategoryId)
                .FirstOrDefault();

            var role = this.roleManager.FindByName("DEMANDA");

            var surveyCompletionParent = new SurveyCompletionParent
            {
                Product = null,
                Role = role,
                Status = "Aprobado",
                PartialSave = partial,
                Category = category,
                Email = model.Email,
                PartialSaveKey = Guid.NewGuid().ToString()
            };

            foreach (var survey in model.SurveyDTOs)
            {
                var surveyCompletion = new SurveyCompletion
                {
                    SurveyId = survey.SurveyId,
                    CategoryId = survey.CategoryId,
                    Category = survey.CategoryName,
                    Email = model.Email,
                    Role = role,
                    Parent = surveyCompletionParent,
                    PartialSave = partial
                };

                var surveyQuestions = this.modelContext
                        .Questions
                        .Include("Answers")
                        .Where(x => x.Survey.Id == survey.SurveyId)
                        .ToList();

                foreach (var question in model.surveyCompletionDTOs)
                {
                    var answers = new List<SurveyCompletionAnswer>();
                    if (question.SurveyId == survey.SurveyId)
                    {
                        if (question.Answers != null)
                        {
                            answers = surveyQuestions
                                .Where(x => x.Id == question.QuestionId)
                                .FirstOrDefault()
                                .Answers
                                .Where(x => question.Answers.Contains(x.Id))
                                .Select(x => new SurveyCompletionAnswer
                                {
                                    Answer = x.DemandAnswer,
                                    AnswerValue = x.Value
                                })
                                .ToList();
                        }

                        string currentUserId = User.Identity.GetUserId();
                        ApplicationUser currentUser = this.modelContext.Users.FirstOrDefault(x => x.Id == currentUserId);

                        var surveyCompletionQuestion = new SurveyCompletionQuestion
                        {
                            Question = question.Question,
                            QuestionId = question.QuestionId,
                            Answers = answers
                        };

                        surveyCompletion.Questions.Add(surveyCompletionQuestion);
                    }
                }

                this.modelContext.SurveysCompletion.Add(surveyCompletion);
            }

            this.modelContext.SurveyCompletionParent.Add(surveyCompletionParent);
            this.modelContext.SaveChanges();

            return surveyCompletionParent;
        }

        [HttpGet]
        public ActionResult Registration(int surveyCompletionParentId)
        {
            var survey = this.modelContext
                .SurveyCompletionParent
                .FirstOrDefault(x => x.Id == surveyCompletionParentId);

            ViewBag.SurveyId = surveyCompletionParentId;

            var model = new RegisterViewModel();

            model.Email = survey.Email;

            return View("~/Views/Demanda/Register.cshtml", model);
        }

        [HttpPost]
        public ActionResult Registration(int surveyId, RegisterViewModel registerViewModel)
        {
            var surveyCompletion = this.modelContext
                .SurveyCompletionParent
                .FirstOrDefault(x => x.Id == surveyId);

            var role = this.roleManager.FindByName("DEMANDA");

            var customer = new Customer
            {
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                LectorType = registerViewModel.LectorType,
                CompanyType = registerViewModel.CompanyType,
                Conutry = registerViewModel.Country,
                City = registerViewModel.City,
                Sector = registerViewModel.Sector,
                Company = registerViewModel.Company,
                RoleInCompany = registerViewModel.RoleInCompany,
                SoftwareInUse = registerViewModel.SoftwareInUse,
                PhoneNumber = registerViewModel.Phone,
                Budget = registerViewModel.Budget,
                AnnualBilling = registerViewModel.AnnualBilling,
                PeopleInCompany = registerViewModel.PeopleInCompany,
                Role = "DEMANDA"
            };

            this.modelContext.Customers.Add(customer);

            surveyCompletion.Customer = customer;

            this.modelContext.SaveChanges();

            return RedirectToAction("Thanks", "../Evaluation", new { surveyCompletionId = surveyId });
        }

        public ActionResult Thanks(int surveyCompletionId)
        {
            var rankingGenerated = this.modelContext
                .Rankings
                .Where(x => x.SurveyCompletionParentByDemanda.Id == surveyCompletionId)
                .Count() > 0;

            if (!rankingGenerated)
            {
                var task = Task.Run(() => this.rankingService.GenerarRanking(surveyCompletionId));

                var surveyCompletionParent = this.modelContext
                    .SurveyCompletionParent
                    .Include("SurveyCompletions")
                    .Include("Company")
                    .Include("Customer")
                    .Include("Category")
                    .Include("SurveyCompletions.Questions")
                    .Include("SurveyCompletions.Questions.Answers")
                    .Include("SurveyCompletions.CategoryObj")
                    .Include("Product")
                    .FirstOrDefault(x => x.Id == surveyCompletionId);

                var category = this.modelContext
                    .Categories
                    .FirstOrDefault(x => x.Id == surveyCompletionParent.Category.Id);

                var products = this.modelContext
                    .SurveyCompletionParent
                    .Include("Product")
                    .Include("Product.Company")
                    .Include("Role")
                    .Where(x =>
                        x.Category.Id == category.Id
                        && x.Role.Name == "OFERTA"
                        && x.Status == "Aprobado"
                        && x.DeletedAt == null)
                    .Select(x => new EvaluationReportProductDTO
                    {
                        ProductName = x.Product.Name,
                        CompanyName = x.Product.Company.CompanyName,
                        CompanyWebSite = x.Product.Company.CompanyWebSite
                    })
                    //.OrderBy(x => x.Product.Name)
                    .ToList();

                var ranking = this.modelContext
                    .Rankings
                    .Include("SurveyCompletionSupply.Company")
                    .Where(x => x.SurveyCompletionParentByDemanda.Id == surveyCompletionId)
                    .OrderByDescending(x => x.Rank)
                    .Take(10)
                    .Select(x => new EvaluationReportRankingDTO
                    {
                        CompanyName = x.SurveyCompletionParentByOferta.Company.CompanyName,
                        ProductName = x.SurveyCompletionParentByOferta.Product.Name,
                        Ranking = x.Rank
                    })
                    .ToList();

                var model = new EvaluationReportTemplateDTO
                {
                    Ranking = ranking,
                    SurveyCompletionParent = surveyCompletionParent,
                    Category = category,
                    Products = products
                };

                String template = "";

                //Si no hay productos muestro el reporte vacío
                if (products.Count > 0)
                {
                    template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationReportTemplate.cshtml", model);
                }
                else
                {
                    template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationEmptyReportTemplate.cshtml", model);
                }

                var fileName = this.pdfService.GetEvaluationReportFileName(surveyCompletionParent.Id);
                var pdfFullName = this.pdfService.Generate(surveyCompletionId, template, fileName);

                this.surveyCompletionByDemandEmailService.Send(pdfFullName, surveyCompletionParent.Customer);

                task.Wait();
                return View("~/Views/Survey/Thanks.cshtml");
            }

            return View("~/Views/Survey/Thanks.cshtml");
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

        private SurveyDTO GetSurveyDTO(int categoryId)
        {
            var survey = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Questions")
                .Where(x => x.Category.Id == categoryId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var questions = survey.Questions
                .Select(question => new SurveyQuestionDTO
                {
                    QuestionId = question.Id,
                    Question = question.DemandQuestion
                })
                .ToList();

            var surveyDTO = new SurveyDTO
            {
                CategoryId = survey.Category.Id,
                SurveyId = survey.Id,
                SurveyQuestionDTOs = questions,
                QuestionCount = questions.Count()
            };

            return surveyDTO;
        }
    }
}