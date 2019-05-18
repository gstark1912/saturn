using App.Attribute;
using App.DTO;
using App.Model.User;
using App.Models;
using App.Models.Survey;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model.Context;
using Model.Model.Customer;
using Model.SurveyCompletion;
using Model.Suvery;
using Services;
using Services.Service.EmailService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers.Oferta
{
    [MyAuthorize(Roles = "Oferta")]
    public class OfertaSurveyController : Controller
    {
        private ModelContext modelContext;
        private SurveyPartialCompletionBySupplyEmailService surveyPartialCompletionBySupplyEmailService;
        private SurveyCompletionBySupplyEmailService surveyCompletionBySupplyEmailService;
        private PdfService pdfService;
        public UserManager<ApplicationUser> userManager;
        public RoleManager<ApplicationRole> roleManager;

        public OfertaSurveyController()
        {
            this.modelContext = new ModelContext();
            this.surveyPartialCompletionBySupplyEmailService = new SurveyPartialCompletionBySupplyEmailService();
            this.surveyCompletionBySupplyEmailService = new SurveyCompletionBySupplyEmailService();
            this.pdfService = new PdfService();

            var userStore = new UserStore<ApplicationUser>(this.modelContext);
            var roleStore = new RoleStore<ApplicationRole>(this.modelContext);

            this.userManager = new UserManager<ApplicationUser>(userStore);
            this.roleManager = new RoleManager<ApplicationRole>(roleStore);
        }

        public ActionResult Index()
        {
            var categories = this.modelContext
                .Categories
                .Where(x => x.parentCategory == null)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            var categoryViewModel = new App.Models.Oferta.Survey.CategoryViewModel
            {
                Categories = categories
            };

            return View("~/Views/Oferta/Survey/Category.cshtml", categoryViewModel);
        }

        [HttpGet]
        public ActionResult Register(int Category)
        {
            var model = new App.Models.Oferta.RegisterViewModel { CategoryId = Category };
            ViewBag.Argument = "Register";
            return View("~/Views/Oferta/Register.cshtml", model);
        }

        [HttpGet]
        public ActionResult RegisterContinue(int surveyCompletionId, string partialSaveKey)
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include("Category")
                .Include("Company")
                .Include("Company.ComercialContact")
                .Include("Product")
                .Include("Product.ProductContact")
                .FirstOrDefault(x =>
                    x.Id == surveyCompletionId &&
                    x.PartialSaveKey == partialSaveKey &&
                    x.DeletedAt == null);

            if (surveyCompletionParent == null)
            {
                throw new ArgumentException("No se encuentra la evaluación");
            }

            var model = this.CompanyRegisterViewModelMap(surveyCompletionParent, surveyCompletionId);
            model.CategoryId = surveyCompletionParent.Category.Id;
            model.CompanyLogoFileName = surveyCompletionParent.Company.CompanyLogo;

            ViewBag.Argument = "RegisterContinue";

            return View("~/Views/Oferta/Register.cshtml", model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Register")]
        public ActionResult Register(int Category, App.Models.Oferta.RegisterViewModel model, string Argument)
        {
            this.ValidateRegistration(model, false);

            if (!ModelState.IsValid)
            {
                ViewBag.Argument = Argument;
                return View("~/Views/Oferta/Register.cshtml", model);
            }

            var surveycomplation = this.RegisterViewModelCompanyMap(model);

            this.modelContext.Companies.Add(surveycomplation.Company);
            this.modelContext.Products.Add(surveycomplation.Product);
            this.modelContext.Contacts.Add(surveycomplation.Company.ComercialContact);
            this.modelContext.Contacts.Add(surveycomplation.Product.ProductContact);
            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                surveycomplation.Company.CompanyLogo = surveycomplation.Company.Id + "_" + model.CompanyLogo.FileName;
            }

            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                var fileName = Path.GetFileName(surveycomplation.Company.CompanyLogo);
                var path = Path.Combine(Server.MapPath("~/Content/images/logos"), fileName);
                model.CompanyLogo.SaveAs(path);
            }

            return RedirectToAction("Survey", "../Product", new { Category = Category, Company = surveycomplation.Company.Id });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "RegisterPartial")]
        public ActionResult RegisterPartial(App.Models.Oferta.RegisterViewModel model, string Argument)
        {
            this.ValidateRegistration(model, true);

            if (!ModelState.IsValid)
            {
                ViewBag.Argument = Argument;
                return View("~/Views/Oferta/Register.cshtml", model);
            }

            var surveyCompletionTmp = this.RegisterViewModelCompanyMap(model);

            this.modelContext.Companies.Add(surveyCompletionTmp.Company);
            this.modelContext.Products.Add(surveyCompletionTmp.Product);
            this.modelContext.Contacts.Add(surveyCompletionTmp.Company.ComercialContact);
            this.modelContext.Contacts.Add(surveyCompletionTmp.Product.ProductContact);
            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                surveyCompletionTmp.Company.CompanyLogo = surveyCompletionTmp.Company.Id + "_" + model.CompanyLogo.FileName;
            }

            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                var fileName = Path.GetFileName(surveyCompletionTmp.Company.CompanyLogo);
                var path = Path.Combine(Server.MapPath("~/Content/images/logos"), fileName);
                model.CompanyLogo.SaveAs(path);
            }
            else
            {
                if (model.SurveyCompletionId != 0)
                {
                    var surveyCompletionPartialCompany = this.modelContext
                    .SurveysCompletion
                    .Include("Company")
                    .FirstOrDefault(x => x.Id == model.SurveyCompletionId)
                    .Company;

                    surveyCompletionTmp.Company.CompanyLogo = surveyCompletionPartialCompany.CompanyLogo;

                    this.modelContext.SaveChanges();
                }
            }

            var surveyViewModel = new SurveyViewModel
            {
                CompanyId = surveyCompletionTmp.Company.Id,
                CategoryId = model.CategoryId
            };

            var surveyCompletion = this.InsertSurveyCompletion(surveyViewModel, true);

            var template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationTemplate.cshtml", surveyCompletion);
            var pdfFileName = this.pdfService.GetEvaluationFileName(surveyCompletion.Id);
            var pdfFullName = this.pdfService.Generate(surveyCompletion.Id, template, pdfFileName);

            this.surveyPartialCompletionBySupplyEmailService.Send(pdfFullName, surveyCompletion);

            return RedirectToAction("ThanksPartial", "../Product/Evaluation");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "RegisterContinue")]
        public ActionResult RegisterContinue(App.Models.Oferta.RegisterViewModel model, string Argument)
        {
            this.ValidateRegistration(model, false);

            if (!ModelState.IsValid)
            {
                ViewBag.Argument = Argument;
                return View("~/Views/Oferta/Register.cshtml", model);
            }

            var surveyCompletion = this.RegisterViewModelCompanyMap(model);

            this.modelContext.Companies.Add(surveyCompletion.Company);
            this.modelContext.Products.Add(surveyCompletion.Product);
            this.modelContext.Contacts.Add(surveyCompletion.Company.ComercialContact);
            this.modelContext.Contacts.Add(surveyCompletion.Product.ProductContact);
            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                surveyCompletion.Company.CompanyLogo = surveyCompletion.Company.Id + "_" + model.CompanyLogo.FileName;
            }

            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                var fileName = Path.GetFileName(surveyCompletion.Company.CompanyLogo);
                var path = Path.Combine(Server.MapPath("~/Content/images/logos"), fileName);
                model.CompanyLogo.SaveAs(path);
            }
            else
            {
                var surveyCompletionPartialCompany = this.modelContext
                    .SurveysCompletion
                    .Include("Company")
                    .FirstOrDefault(x => x.Id == model.SurveyCompletionId)
                    .Company;

                surveyCompletion.Company.CompanyLogo = surveyCompletionPartialCompany.CompanyLogo;

                this.modelContext.SaveChanges();
            }

            return RedirectToAction("SurveyContinue", "../Product", new
            {
                Category = model.CategoryId,
                Company = surveyCompletion.Company.Id,
                SurveyCompletionId = model.SurveyCompletionId
            });
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "RegisterContinuePartial")]
        public ActionResult RegisterContinuePartial(App.Models.Oferta.RegisterViewModel model, string Argument)
        {
            this.ValidateRegistration(model, true);

            if (!ModelState.IsValid)
            {
                ViewBag.Argument = Argument;
                return View("~/Views/Oferta/Register.cshtml", model);
            }

            var surveyCompletionTmp = this.RegisterViewModelCompanyMap(model);

            this.modelContext.Companies.Add(surveyCompletionTmp.Company);
            this.modelContext.Products.Add(surveyCompletionTmp.Product);
            this.modelContext.Contacts.Add(surveyCompletionTmp.Company.ComercialContact);
            this.modelContext.Contacts.Add(surveyCompletionTmp.Product.ProductContact);
            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                surveyCompletionTmp.Company.CompanyLogo = surveyCompletionTmp.Company.Id + "_" + model.CompanyLogo.FileName;
            }

            this.modelContext.SaveChanges();

            if (model.CompanyLogo != null)
            {
                var fileName = Path.GetFileName(surveyCompletionTmp.Company.CompanyLogo);
                var path = Path.Combine(Server.MapPath("~/Content/images/logos"), fileName);
                model.CompanyLogo.SaveAs(path);
            }
            else
            {
                if (model.SurveyCompletionId != 0)
                {
                    var surveyCompletionPartialCompany = this.modelContext
                    .SurveysCompletion
                    .Include("Company")
                    .FirstOrDefault(x => x.Id == model.SurveyCompletionId)
                    .Company;

                    surveyCompletionTmp.Company.CompanyLogo = surveyCompletionPartialCompany.CompanyLogo;

                    this.modelContext.SaveChanges();
                }
            }

            var surveyViewModel = new SurveyViewModel
            {
                CompanyId = surveyCompletionTmp.Company.Id,
                SurveyId = model.CategoryId,
                SurveyCompletionId = model.SurveyCompletionId,
                CategoryId = model.CategoryId
            };

            var surveyCompletionPartial = this.modelContext
                .SurveysCompletion
                .Include("Questions")
                .Include("Questions.Answers")
                .FirstOrDefault(x => x.Id == model.SurveyCompletionId);

            var surveyCompletionParent = this.InsertSurveyCompletion(surveyViewModel, true);

            //surveyCompletionParent.Questions = surveyCompletionPartial.Questions;
            /*this.modelContext.SaveChanges();

            var template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationTemplate.cshtml", surveyCompletion);
            var pdfFileName = this.pdfService.GetEvaluationFileName(surveyCompletion.Id);
            var pdfFullName = this.pdfService.Generate(surveyCompletion.Id, template, pdfFileName);

            this.surveyPartialCompletionBySupplyEmailService.Send(pdfFullName, surveyCompletion);*/

            return RedirectToAction("ThanksPartial", "../Product/Evaluation");
        }

        public ActionResult Survey(int Category, string subCategories)
        {
            var surveys = new List<SurveyDTO>();
            var subcategories_Ids = new List<int>();

            foreach (string subcategory in subCategories.Split(','))
            {
                if (subcategory != "")
                {
                    subcategories_Ids.Add(int.Parse(subcategory));
                }
            }

            renderSubCategory(Category, surveys, subcategories_Ids);

            var userId = this.User.Identity.GetUserId();
            var user = this.modelContext
                .Users
                .Single(x => x.Id == userId);

            if (!user.CompanyId.HasValue)
            {
                throw new ArgumentNullException("User company id is null");
            }

            var model = new SurveyViewModel
            {
                SurveyDTOs = surveys,
                CategoryId = Category,
                SurveyId = surveys.Where(x => x.CategoryId == Category).FirstOrDefault().SurveyId,
                CompanyId = user.CompanyId.Value,
                SurveyCompletionId = 0
            };

            return View("~/Views/Oferta/Survey/Survey.cshtml", model);
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
                    Question = question.SupplyQuestion,
                    Required = question.SupplyRequired,
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

        public ActionResult SurveyContinue(int Category, int Company, int SurveyCompletionId)
        {
            var survey = this.modelContext
                .Surveys
                .Include("Category")
                .Include("Questions")
                .Where(x => x.Category.Id == Category)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var questions = survey.Questions
                .Select(question => new SurveyQuestionDTO
                {
                    QuestionId = question.Id,
                    Question = question.SupplyQuestion,
                    Required = question.SupplyRequired
                })
                .ToList();

            ViewBag.QuestionCount = questions.Count();

            var model = new SurveyViewModel
            {
                SurveyDTOs = null,
                CategoryId = survey.Category.Id,
                SurveyId = survey.Id,
                CompanyId = Company,
                SurveyCompletionId = SurveyCompletionId,
            };

            return View("~/Views/Oferta/Survey/Survey.cshtml", model);
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
                Question = question.SupplyQuestion,
                Required = question.SupplyRequired,
                Type = question.AnswerType.Name,
                Answers = question.Answers
                    .Select(answer => new AnswerDTO
                    {
                        Id = answer.Id,
                        Answer = answer.SupplyAnswer
                    })
                    .ToList(),
                ChosenAnswers = chosenAnswers
            };

            var view = "~/Views/Survey/Question/" + question.AnswerType.Name + ".cshtml";

            return PartialView(view, model);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save(SurveyViewModel model)
        {
            var surveyCompletionParent = this.InsertSurveyCompletion(model);

            var template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationTemplate.cshtml", surveyCompletionParent);
            var fileName = this.pdfService.GetEvaluationFileName(surveyCompletionParent.Id);
            var pdfFullName = this.pdfService.Generate(surveyCompletionParent.Id, template, fileName);

            this.surveyCompletionBySupplyEmailService.Send(pdfFullName, surveyCompletionParent);

            return RedirectToAction("Thanks", "../Product/Evaluation");
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "PartialSave")]
        public ActionResult SaveParcial(SurveyViewModel model)
        {
            var surveyCompletionParent = this.InsertSurveyCompletion(model, true);

            var template = this.RenderRazorViewToString("~/Views/Demanda/Email/EvaluationTemplate.cshtml", surveyCompletionParent);
            var fileName = this.pdfService.GetEvaluationFileName(surveyCompletionParent.Id);
            var pdfFullName = this.pdfService.Generate(surveyCompletionParent.Id, template, fileName);

            this.surveyPartialCompletionBySupplyEmailService.Send(pdfFullName, surveyCompletionParent);

            return RedirectToAction("ThanksPartial", "../Product/Evaluation");
        }

        public ActionResult Thanks()
        {
            return View("~/Views/Oferta/Survey/Thanks.cshtml");
        }

        [AllowAnonymous]
        public ActionResult ThanksPartial()
        {
            return View("~/Views/Oferta/Survey/PartialThanks.cshtml");
        }

        [AllowAnonymous]
        public ActionResult WaitConfirmation()
        {
            return View("~/Views/Oferta/Survey/WaitConfirmation.cshtml");
        }

        [HttpGet]
        public JsonResult SubCategories(int category)
        {
            var result = new JsonResult();

            var categories = this.modelContext
                .Categories
                .ToList()
                .Where(x => x.parentCategory != null && x.parentCategory.Id == category);

            var json = "[]";

            if (categories.Any())
            {
                json = "[";
                foreach (var item in categories)
                {
                    json = json + item.toJson() + ",";
                }
                json = json.Substring(0, json.Length - 1) + "]";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
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

        private SurveyCompletion RegisterViewModelCompanyMap(App.Models.Oferta.RegisterViewModel model)
        {
            var surveyCompletion = new SurveyCompletion
            {
                Company =
                {
                    CompanyName = model.CompanyName,
                    CompanyDescription = model.CompanyDescription,
                    CompanyWebSite = model.CompanyWebSite,
                    CompanyCountry = model.CompanyCountry,
                    CompanyCity = model.CompanyCity,
                    CompanyAddress = model.CompanyAddress,
                    CompanyPostalCode = model.CompanyPostalCode,
                    CompanyBranchOfficesIn = model.CompanyBranchOfficesIn,
                    CompanyFiscalStartDate = model.CompanyFiscalStartDate,
                    CompanyFiscalEndDate = model.CompanyFiscalEndDate,
                    CompanyPeopleInCompany = model.CompanyPeopleInCompany,
                    ComercialContact =
                    {
                        FullName = model.ComercialContactFullName,
                        Position = model.ComercialContactPosition,
                        Phone = model.ComercialContactPhone,
                        Email = model.ComercialContactEmail
                    },
                }//,
                //Product =
                //{
                //    Name = model.ProductName,
                //    Version = model.ProductVersion,
                //    Description = model.ProductDescription,
                //    WebSite = model.ProductWebSite,
                //    ProductContact =
                //    {
                //        FullName = model.ProductContactFullName,
                //        Position = model.ProductContactPosition,
                //        Phone = model.ProductContactPhone,
                //        Email = model.ProductContactEmail
                //    }
                //}
            };

            return surveyCompletion;
        }

        private App.Models.Oferta.RegisterViewModel CompanyRegisterViewModelMap(SurveyCompletionParent surveyCompletion, int surveyCompletionId)
        {
            var model = new App.Models.Oferta.RegisterViewModel
            {
                SurveyCompletionId = surveyCompletionId,
                CompanyName = surveyCompletion.Company.CompanyName,
                CompanyDescription = surveyCompletion.Company.CompanyDescription,
                CompanyWebSite = surveyCompletion.Company.CompanyWebSite,
                CompanyCountry = surveyCompletion.Company.CompanyCountry,
                CompanyCity = surveyCompletion.Company.CompanyCity,
                CompanyAddress = surveyCompletion.Company.CompanyAddress,
                CompanyPostalCode = surveyCompletion.Company.CompanyPostalCode,
                CompanyBranchOfficesIn = surveyCompletion.Company.CompanyBranchOfficesIn,
                CompanyFiscalStartDate = surveyCompletion.Company.CompanyFiscalStartDate,
                CompanyFiscalEndDate = surveyCompletion.Company.CompanyFiscalEndDate,
                CompanyPeopleInCompany = surveyCompletion.Company.CompanyPeopleInCompany,
                ComercialContactFullName = surveyCompletion.Company.ComercialContact.FullName,
                ComercialContactPosition = surveyCompletion.Company.ComercialContact.Position,
                ComercialContactPhone = surveyCompletion.Company.ComercialContact.Phone,
                ComercialContactEmail = surveyCompletion.Company.ComercialContact.Email
            };

            return model;
        }

        private SurveyCompletionParent InsertSurveyCompletion(SurveyViewModel model, bool partial = false)
        {
            this.RemovePartialSurveyCompletion(model);

            var category = this.modelContext
                .Categories
                .Where(x => x.Id == model.CategoryId)
                .FirstOrDefault();

            var userId = User.Identity.GetUserId();
            var role = this.roleManager.FindByName("OFERTA");

            var user = this.modelContext
                    .Users
                    .FirstOrDefault(x => x.Id == userId);

            var company = this.modelContext
                .Companies
                .Include("ComercialContact")
                .FirstOrDefault(x => x.Id == model.CompanyId);
            //aca  crear el pridcuto con el contactro del producto
            var producto = new Product
            {
                Name = model.ProductName,
                Description = model.ProductDescription,
                WebSite = model.ProductWebSite,
                Version = model.ProductVersion,
                User = user,
                ProductContact = new Contact
                {
                    FullName = model.ProductContactFullName,
                    Position = model.ProductContactPosition,
                    Phone = model.ProductContactPhone,
                    Email = model.ProductContactEmail
                }
            };

            producto.Company = company;
            modelContext.Products.Add(producto);
            modelContext.SaveChanges();

            var surveyCompletionParent = new SurveyCompletionParent(company)
            {
                Product = producto,
                Role = role,
                Status = "Pendiente",
                PartialSave = partial,
                Category = category
            };

            foreach (var surveyDTO in model.SurveyDTOs)
            {
                var categoryObj = this.modelContext
                .Categories
                .Where(x => x.Id == surveyDTO.CategoryId)
                .FirstOrDefault();

                var surveyCompletion = new SurveyCompletion(company)
                {
                    SurveyId = surveyDTO.SurveyId,
                    CategoryId = surveyDTO.CategoryId,
                    Product = producto,
                    Category = surveyDTO.CategoryName,
                    CategoryObj = categoryObj,
                    Role = role,
                    PartialSave = partial,
                    Parent = surveyCompletionParent
                };

                var surveyQuestions = this.modelContext
                    .Questions
                    .Include("Answers")
                    .Where(x => x.Survey.Id == surveyDTO.SurveyId)
                    .ToList();

                foreach (var question in model.surveyCompletionDTOs)
                {
                    var answers = new List<SurveyCompletionAnswer>();

                    if (question.SurveyId == surveyDTO.SurveyId)
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
                                    Answer = x.SupplyAnswer,
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

        private void RemovePartialSurveyCompletion(SurveyViewModel model)
        {
            if (model.SurveyCompletionId == 0)
            {
                return;
            }

            var surveyCompletionPartial = this.modelContext
                .SurveysCompletion
                .FirstOrDefault(x => x.Id == model.SurveyCompletionId);

            surveyCompletionPartial.DeletedAt = DateTime.Now;

            this.modelContext.SaveChanges();
        }

        private void ValidateRegistration(App.Models.Oferta.RegisterViewModel model, bool partial)
        {
            var errorMessage = "El campo {0} es obligatorio";

            if (model.ComercialContactEmail == null)
            {
                ModelState.AddModelError("ComercialContactEmail", string.Format(errorMessage, "Email"));
            }

            if (!partial)
            {
                if (model.CompanyName == null)
                {
                    ModelState.AddModelError("CompanyName", string.Format(errorMessage, "Nombre"));
                }

                if (model.CompanyDescription == null)
                {
                    ModelState.AddModelError("CompanyDescription", string.Format(errorMessage, "Descripción"));
                }

                if (model.CompanyWebSite == null)
                {
                    ModelState.AddModelError("CompanyWebSite", string.Format(errorMessage, "Web Site"));
                }

                if (model.CompanyCountry == null)
                {
                    ModelState.AddModelError("CompanyCountry", string.Format(errorMessage, "País"));
                }

                if (model.CompanyCity == null)
                {
                    ModelState.AddModelError("CompanyCity", string.Format(errorMessage, "Ciudad"));
                }

                if (model.CompanyAddress == null)
                {
                    ModelState.AddModelError("CompanyAddress", string.Format(errorMessage, "Dirección"));
                }

                if (model.CompanyFiscalStartDate == null)
                {
                    ModelState.AddModelError("CompanyFiscalStartDate", string.Format(errorMessage, "Mes de inicio del año fiscal"));
                }

                if (model.CompanyFiscalEndDate == null)
                {
                    ModelState.AddModelError("CompanyFiscalEndDate", string.Format(errorMessage, "Mes de cierre del año fiscal"));
                }

                //if (model.ProductName == null)
                //{
                //    ModelState.AddModelError("ProductName", string.Format(errorMessage, "Nombre"));
                //}

                //if (model.ProductDescription == null)
                //{
                //    ModelState.AddModelError("ProductDescription", string.Format(errorMessage, "Descripción"));
                //}

                if (model.ComercialContactFullName == null)
                {
                    ModelState.AddModelError("ComercialContactFullName", string.Format(errorMessage, "Nombre y Apellido"));
                }

                if (model.ComercialContactPosition == null)
                {
                    ModelState.AddModelError("ComercialContactPosition", string.Format(errorMessage, "Cargo / Función"));
                }

                if (model.ComercialContactPhone == null)
                {
                    ModelState.AddModelError("ComercialContactPhone", string.Format(errorMessage, "Teléfono"));
                }
            }
        }
    }
}