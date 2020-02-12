using App.Attribute;
using App.Models.Oferta;
using Model.Context;
using Services;
using Services.Service.EmailService;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers.Admin
{
    public class AdminSurveyCompletionOfertaController : Controller
    {
        public ModelContext modelContext;
        public SurveySupplyApproveEmailService surveySupplyApproveEmailService;
        public PdfService pdfService;

        public AdminSurveyCompletionOfertaController()
        {
            this.modelContext = new ModelContext();
            this.surveySupplyApproveEmailService = new SurveySupplyApproveEmailService();
            this.pdfService = new PdfService();
        }

        public ActionResult Index()
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include( "Company" )
                .Include( "Product" )
                .Include( "SurveyCompletions" )
                .Include( "SurveyCompletions.CategoryObj" )
                .Where( x =>
                     x.Role.Name.ToUpper() == "OFERTA" &&
                     x.PartialSave == false &&
                     x.DeletedAt == null )
                    .OrderByDescending( x => x.CreatedAt )
                .ToList();

            ViewBag.SurveysCompletionParent = surveyCompletionParent;
            ViewBag.ExistingCategories = this.modelContext
                .Categories
                .Select( x => x.Id )
                .ToList();

            return View( "~/Views/Admin/SurveyCompletionOferta/List.cshtml" );
        }

        public ActionResult View( int id )
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include( "SurveyCompletions" )
                .Include( "Company" )
                .Include( "Company.ComercialContact" )
                .Include( "Product" )
                .Include( "Product.ProductContact" )
                .Include( "SurveyCompletions.Questions" )
                .Include( "SurveyCompletions.Questions.Answers" )
                .Include( "SurveyCompletions.Questions.QuestionObj" )
                .Include( "SurveyCompletions.CategoryObj" )
                .FirstOrDefault( x => x.Id == id );

            ViewBag.SurveyCompletion = surveyCompletionParent;

            ViewBag.Company = surveyCompletionParent.Company;
            ViewBag.Product = surveyCompletionParent.Product;

            return View( "~/Views/Admin/SurveyCompletionOferta/View.cshtml" );
        }

        [HttpGet]
        public ActionResult Update( int id )
        {
            var surveyCompletion = this.modelContext
                .SurveyCompletionParent
                .Include( "SurveyCompletions" )
                .Include( "Company" )
                .Include( "Company.ComercialContact" )
                .Include( "Product" )
                .Include( "Product.ProductContact" )
                .Include( "SurveyCompletions.Questions" )
                .Include( "SurveyCompletions.Questions.Answers" )
                .Include( "SurveyCompletions.Questions.QuestionObj" )
                .Include( "SurveyCompletions.CategoryObj" )
                .FirstOrDefault( x => x.Id == id );

            ViewBag.SurveyCompletion = surveyCompletion;

            var model = new RegisterViewModel
            {
                CompanyName = surveyCompletion.Company.CompanyName,
                CompanyDescription = surveyCompletion.Company.CompanyDescription,
                CompanyWebSite = surveyCompletion.Company.CompanyWebSite,
                CompanyLogoFileName = surveyCompletion.Company.CompanyLogo,
                CompanyCountry = surveyCompletion.Company.CompanyCountry,
                CompanyCity = surveyCompletion.Company.CompanyCity,
                CompanyAddress = surveyCompletion.Company.CompanyAddress,
                CompanyPostalCode = surveyCompletion.Company.CompanyPostalCode,
                CompanyBranchOfficesIn = surveyCompletion.Company.CompanyBranchOfficesIn,
                CompanyFiscalStartDate = surveyCompletion.Company.CompanyFiscalStartDate,
                CompanyFiscalEndDate = surveyCompletion.Company.CompanyFiscalEndDate,
                CompanyPeopleInCompany = surveyCompletion.Company.CompanyPeopleInCompany,
                ProductName = surveyCompletion.Product.Name,
                ProductVersion = surveyCompletion.Product.Version,
                ProductDescription = surveyCompletion.Product.Description,
                ProductWebSite = surveyCompletion.Product.WebSite,
                ComercialContactFullName = surveyCompletion.Company.ComercialContact.FullName,
                ComercialContactPosition = surveyCompletion.Company.ComercialContact.Position,
                ComercialContactPhone = surveyCompletion.Company.ComercialContact.Phone,
                ComercialContactEmail = surveyCompletion.Company.ComercialContact.Email,
                ProductContactFullName = surveyCompletion.Product.ProductContact.FullName,
                ProductContactPosition = surveyCompletion.Product.ProductContact.Position,
                ProductContactPhone = surveyCompletion.Product.ProductContact.Phone,
                ProductContactEmail = surveyCompletion.Product.ProductContact.Email
            };

            return View( "~/Views/Admin/SurveyCompletionOferta/Update.cshtml", model );
        }

        [HttpPost]
        [MultipleButton( Name = "action", Argument = "Update" )]
        public ActionResult Update( int id, RegisterViewModel model )
        {
            var SurveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include( "Company" )
                .Include( "Company.ComercialContact" )
                .Include( "Product" )
                .Include( "Product.ProductContact" )
                .FirstOrDefault( x => x.Id == id );

            var company = SurveyCompletionParent.Company;
            var product = SurveyCompletionParent.Product;
            var productContact = SurveyCompletionParent.Product.ProductContact;
            var comercialContact = SurveyCompletionParent.Company.ComercialContact;

            company.CompanyName = model.CompanyName;
            company.CompanyDescription = model.CompanyDescription;
            company.CompanyWebSite = model.CompanyWebSite;
            company.CompanyCountry = model.CompanyCountry;
            company.CompanyCity = model.CompanyCity;
            company.CompanyAddress = model.CompanyAddress;
            company.CompanyPostalCode = model.CompanyPostalCode;
            company.CompanyBranchOfficesIn = model.CompanyBranchOfficesIn;
            company.CompanyFiscalStartDate = model.CompanyFiscalStartDate;
            company.CompanyFiscalEndDate = model.CompanyFiscalEndDate;
            company.CompanyPeopleInCompany = model.CompanyPeopleInCompany;
            product.Name = model.ProductName;
            product.Version = model.ProductVersion;
            product.Description = model.ProductDescription;
            product.WebSite = model.ProductWebSite;
            comercialContact.FullName = model.ComercialContactFullName;
            comercialContact.Position = model.ComercialContactPosition;
            comercialContact.Phone = model.ComercialContactPhone;
            comercialContact.Email = model.ComercialContactEmail;
            productContact.FullName = model.ProductContactFullName;
            productContact.Position = model.ProductContactPosition;
            productContact.Phone = model.ProductContactPhone;
            productContact.Email = model.ProductContactEmail;

            this.modelContext.SaveChanges();

            if( model.CompanyLogo != null )
            {
                company.CompanyLogo = company.Id + "_" + model.CompanyLogo.FileName;
            }

            this.modelContext.SaveChanges();

            if( model.CompanyLogo != null )
            {
                var fileName = Path.GetFileName( company.CompanyLogo );
                var path = Path.Combine( Server.MapPath( "~/Content/images/logos" ), fileName );
                model.CompanyLogo.SaveAs( path );
            }

            return RedirectToAction( "Index", "../Admin/EvaluationCompletion/Oferta" );
        }

        [HttpPost]
        [MultipleButton( Name = "action", Argument = "Approve" )]
        public ActionResult Approve( int id, RegisterViewModel model )
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .Include( "Company" )
                .Include( "Company.ComercialContact" )
                .Include( "Product" )
                .Include( "Product.ProductContact" )
                .FirstOrDefault( x => x.Id == id );

            surveyCompletionParent.Status = "Aprobado";

            this.modelContext.SaveChanges();

            var template = this.RenderRazorViewToString( "~/Views/Demanda/Email/EvaluationTemplate.cshtml", surveyCompletionParent );
            var fileName = this.pdfService.GetEvaluationFileName( surveyCompletionParent.Id );

            this.surveySupplyApproveEmailService.Send( fileName, surveyCompletionParent );

            return RedirectToAction( "Index", "../Admin/EvaluationCompletion/Oferta" );
        }

        [HttpPost]
        [MultipleButton( Name = "action", Argument = "Reject" )]
        public ActionResult Reject( int id, RegisterViewModel model )
        {
            var surveyCompletion = this.modelContext
                .SurveyCompletionParent
                .FirstOrDefault( x => x.Id == id );

            surveyCompletion.Status = "Rechazado";

            this.modelContext.SaveChanges();

            return RedirectToAction( "Index", "../Admin/EvaluationCompletion/Oferta" );
        }

        [HttpGet]
        public ActionResult Delete( int id )
        {
            var surveyCompletionParent = this.modelContext
                .SurveyCompletionParent
                .FirstOrDefault( x => x.Id == id );

            surveyCompletionParent.DeletedAt = DateTime.Now;

            this.modelContext.SaveChanges();

            return RedirectToAction( "Index", "../Admin/EvaluationCompletion/Oferta" );
        }

        private string RenderRazorViewToString( string viewName, object model )
        {
            ViewData.Model = model;
            using( var sw = new StringWriter() )
            {
                var viewResult = ViewEngines.Engines.FindPartialView(
                    ControllerContext,
                    viewName );

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw );

                viewResult.View.Render( viewContext, sw );
                viewResult.ViewEngine.ReleaseView( ControllerContext, viewResult.View );
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}