using App.Models.Survey;
using ExcelEvaluando;
using ExcelEvaluando.Models;
using Model.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers.Admin
{
    public class AdminSurveyCompletionDemandaController : Controller
    {
        public ModelContext modelContext;

        public AdminSurveyCompletionDemandaController()
        {
            this.modelContext = new ModelContext();
        }

        public ActionResult Index()
        {
            ViewBag.SurveysCompletion = this.modelContext
                .SurveyCompletionParent
                .Include( "Customer" )
                .Include( "Category" )
                .Where( x =>
                     x.Customer.Role == "DEMANDA" &&
                     !x.PartialSave )
                    .OrderByDescending( x => x.CreatedAt )
                .ToList();

            return View( "~/Views/Admin/SurveyCompletionDemanda/List.cshtml" );
        }

        public FileResult ExportToExcel()
        {
            byte[] result;

            IEnumerable<SurveyCompletionParentOutput> data = this.modelContext
                .SurveyCompletionParent
                .Include( "Customer" )
                .Include( "Category" )
                .Where( x =>
                     x.Customer.Role == "DEMANDA" &&
                     !x.PartialSave )
                    .OrderByDescending( x => x.CreatedAt )
                .ToList()
                .Select( x => new SurveyCompletionParentOutput
                {
                    Category = x.Category.Name,
                    Id = x.Identifier,
                    CustomerFullName = String.Format( "{0} {1}", x.Customer.FirstName, x.Customer.LastName ),
                    CustomerEmail = x.Customer.Email,
                    LectorType = x.Customer.LectorType,
                    UTMSource = x.Source,
                    CreatedAt = String.Format( "{0} {1}", x.CreatedAt.ToShortDateString(), x.CreatedAt.ToShortTimeString() )
                } );

            using( var templateStream = new MemoryStream() )
            {
                new EvaluandoExcelBLL().WriteDataInExcel( templateStream, data );
                templateStream.Position = 0;
                result = templateStream.ToArray();
                templateStream.Flush();
            }

            return File( result, "application/octet-stream", DateTime.Now.ToShortDateString() + ".xlsx" );
        }

        public ActionResult View( int id )
        {
            var surveyCompletion = this.modelContext
                .SurveyCompletionParent
                .Include( "Customer" )
                .Include( "SurveyCompletions.CategoryObj" )
                .Include( "SurveyCompletions.Questions.Answers" )
                .FirstOrDefault( x => x.Id == id );

            ViewBag.SurveyCompletion = surveyCompletion;

            ViewBag.Ranking = this.modelContext
                .Rankings
                .Include( "SurveyCompletionParentByOferta.Company" )
                .Include( "SurveyCompletionParentByOferta.Product" )
                .Where( x => x.SurveyCompletionParentByDemanda.Id == id )
                .OrderByDescending( x => x.Rank )
                .ToList();

            ViewBag.Customer = surveyCompletion.Customer;

            return View( "~/Views/Admin/SurveyCompletionDemanda/View.cshtml" );
        }

        [HttpGet]
        public ActionResult Update( int id )
        {
            var surveyCompletion = this.modelContext
                .SurveyCompletionParent
                .Include( "Customer" )
                .Include( "Questions" )
                .Include( "Questions.Answers" )
                .FirstOrDefault( x => x.Id == id );

            ViewBag.SurveyCompletion = surveyCompletion;

            ViewBag.Ranking = this.modelContext
                .Rankings
                .Include( "SurveyCompletionSupply.Company" )
                .Where( x => x.SurveyCompletionParentByDemanda.Id == id )
                .OrderByDescending( x => x.Rank );

            var model = new RegisterViewModel
            {
                Email = surveyCompletion.Customer.Email,
                FirstName = surveyCompletion.Customer.FirstName,
                LastName = surveyCompletion.Customer.LastName,
                LectorType = surveyCompletion.Customer.LectorType,
                CompanyType = surveyCompletion.Customer.CompanyType,
                Country = surveyCompletion.Customer.Conutry,
                City = surveyCompletion.Customer.City,
                Sector = surveyCompletion.Customer.Sector,
                Company = surveyCompletion.Customer.Company,
                RoleInCompany = surveyCompletion.Customer.RoleInCompany,
                DeploymentArea = surveyCompletion.Customer.DeploymentArea,
                SoftwareInUse = surveyCompletion.Customer.SoftwareInUse,
                Phone = surveyCompletion.Customer.PhoneNumber
            };

            return View( "~/Views/Admin/SurveyCompletionDemanda/Update.cshtml", model );
        }

        [HttpPost]
        public ActionResult Update( int id, RegisterViewModel registerViewModel )
        {
            var customer = this.modelContext
                .SurveyCompletionParent
                .Include( "Customer" )
                .FirstOrDefault( x => x.Id == id )
                .Customer;

            customer.Email = registerViewModel.Email;
            customer.FirstName = registerViewModel.FirstName;
            customer.LastName = registerViewModel.LastName;
            customer.LectorType = registerViewModel.LectorType;
            customer.CompanyType = registerViewModel.CompanyType;
            customer.Conutry = registerViewModel.Country;
            customer.City = registerViewModel.City;
            customer.Sector = registerViewModel.Sector;
            customer.Company = registerViewModel.Company;
            customer.RoleInCompany = registerViewModel.RoleInCompany;
            customer.DeploymentArea = registerViewModel.DeploymentArea;
            customer.SoftwareInUse = registerViewModel.SoftwareInUse;
            customer.PhoneNumber = registerViewModel.Phone;

            this.modelContext.SaveChanges();

            return RedirectToAction( "Index", "../Admin/EvaluationCompletion/Demanda" );
        }
    }
}