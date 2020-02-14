using App.DTO;
using App.Model.User;
using App.Models;
using App.Models.Survey;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Model.Context;
using Model.Model.Customer;
using Model.SurveyCompletion;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers.Demanda
{
    public class OfertaController : Controller
    {
        private static ModelContext modelContext = new ModelContext();

        private string[] domains = { "gmail", "yahoo", "outlook", "live", "hotmail" };

        public OfertaController()
            : this( new UserManager<ApplicationUser>( new UserStore<ApplicationUser>( modelContext ) ) )
        {
            //this.modelContext = new ModelContext();
        }

        public OfertaController( UserManager<ApplicationUser> userManager )
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult DatosPersonales()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = modelContext.Users.Include( "Company" ).Where( x => x.Id == userId ).FirstOrDefault();
            VendorViewModel vmUser = ApplicationUsertoVendorViewModelMap( user );
            return View( vmUser );
        }

        [HttpPost]
        public ActionResult DatosPersonales( VendorViewModel vmUser )
        {
            modelContext.SaveChanges();
            ApplicationUser user = modelContext.Users.Where( x => x.Id == vmUser.Id ).FirstOrDefault();
            user.FirstName = vmUser.FirstName;
            user.LastName = vmUser.LastName;
            //old_user.Email = user.Email;

            Company company = modelContext.Companies.Where( x => x.Id == user.CompanyId ).FirstOrDefault();
            company.CompanyName = vmUser.CompanyName;
            company.CompanyDescription = vmUser.CompanyDescription;
            company.CompanyWebSite = vmUser.CompanyWebSite;
            company.CompanyPeopleInCompany = vmUser.CompanyPeopleInCompany;
            company.CompanyCity = vmUser.CompanyCity;
            company.CompanyAddress = vmUser.CompanyAddress;
            company.CompanyPostalCode = vmUser.CompanyPostalCode;
            company.CompanyBranchOfficesIn = vmUser.CompanyBranchOfficesIn;
            company.CompanyFiscalStartDate = vmUser.CompanyFiscalStartDate;
            company.CompanyFiscalEndDate = vmUser.CompanyFiscalEndDate;
            modelContext.SaveChanges();

            ApplicationUser updated_user = modelContext.Users.Where( x => x.Id == user.Id ).FirstOrDefault();
            VendorViewModel vm_updated_User = ApplicationUsertoVendorViewModelMap( updated_user );
            ViewBag.Msg = "Datos actualizados";
            return View( vm_updated_User );
        }

        public ActionResult Register()
        {
            return View( "~/Views/Oferta/Register.cshtml", new App.Models.CheckVendorViewModel() );
        }

        public ActionResult RegisterUser()
        {
            return View( "~/Views/Oferta/RegisterUser.cshtml", new App.Models.UserViewModel() );
        }

        public ActionResult RegisterVendorAndUser()
        {
            if( Session["email"] == null )
            {
                return RedirectToAction( "Register", "../Oferta" );
            }

            return View( "~/Views/Oferta/RegisterVendorAndUser.cshtml", new App.Models.VendorViewModel() );
        }

        [HttpPost]
        public ActionResult Register( App.Models.CheckVendorViewModel model, string Argument )
        {
            Session["country"] = model.CompanyCountry;
            Session["email"] = model.Email;

            var address = new MailAddress( model.Email );
            var host = address.Host; // host contains yahoo.com
            var domain = host.Split( '.' )[0];

            var vendor = modelContext
                .Companies
                .FirstOrDefault( x => x.CompanyDomain == domain );

            var commonDomain = this.domains.Any( x => host.Contains( x ) );

            if( !commonDomain && vendor != null ) //existe el vendor, solo creo el usuario y deshabilitado
            {
                Session["company"] = vendor;
                return RedirectToAction( "RegisterUser", "../Oferta" );
            }
            else //no existe el vendor, creo ambos.
            {
                return RedirectToAction( "RegisterVendorAndUser", "../Oferta" );
            }
        }

        [HttpPost]
        public async Task<ActionResult> RegisterVendorAndUser( App.Models.VendorViewModel model, string Argument )
        {
            if( Session["email"] == null )
            {
                return RedirectToAction( "Register", "../Oferta" );
            }

            var company = VendorViewModelCompanyMap( model );
            var user = this.VendorViewModelUserMap( model );
            user.Company = company; //los usuarios pertenecen a una company
            user.Enabled = true;
            company.ComercialContact.Email = user.Email;
            company.ComercialContact.FullName = user.LastName + ", " + user.FirstName;
            company.ComercialContact.Phone = user.PhoneNumber;
            MailAddress address = new MailAddress( user.Email );
            string host = address.Host; // host contains yahoo.com
            var arrayMail = host.Split( '.' );
            company.CompanyDomain = arrayMail[0];
            modelContext.Contacts.Add( company.ComercialContact );
            modelContext.Companies.Add( company );
            modelContext.SaveChanges();

            UserManager.UserValidator = new UserValidator<ApplicationUser>( UserManager ) { AllowOnlyAlphanumericUserNames = false };
            var result = await UserManager.CreateAsync( user, model.Password );

            if( result.Succeeded )
            {
                await enviarMailConfirmacion( user );
                this.enviarMailNewVendorToAdmin( user );
                return RedirectToAction( "WaitConfirmation", "../Product/Evaluation" );
            }
            else
            {
                AddErrors( result );
            }

            modelContext.SaveChanges();
            return RedirectToAction( "WaitConfirmation", "../Product/Evaluation" );
        }

        [HttpPost]
        public async Task<ActionResult> RegisterUser( App.Models.UserViewModel model, string Argument )
        {
            var user = this.UserViewModelUserMap( model );

            user.Company = (Company)Session["company"];

            UserManager.UserValidator = new UserValidator<ApplicationUser>( UserManager ) { AllowOnlyAlphanumericUserNames = false };
            var result = await UserManager.CreateAsync( user, model.Password );

            modelContext.SaveChanges();

            if( result.Succeeded )
            {
                await enviarMailConfirmacion( user );
                this.enviarMailHabilitacionToAdmin( user );
            }
            else
            {
                AddErrors( result );
            }

            modelContext.SaveChanges();

            return RedirectToAction( "WaitConfirmation", "../Product/Evaluation" );
        }

        private async Task enviarMailConfirmacion( ApplicationUser user )
        {
            var dataProtectionProvider = Startup.DataProtectionProvider;
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider( "EvaluandoSoftware" );
            //UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("EmailConfirmation"));
            UserManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>( dataProtectionProvider.Create( "EmailConfirmation" ) );
            var code = await UserManager.GenerateEmailConfirmationTokenAsync( user.Id );
            var callbackUrl = Url.Action(
               "ConfirmEmail", "Account",
               new { userId = user.Id, code = code },
               protocol: Request.Url.Scheme );
            var emailSender = new EmailSender();
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add( user.Email );
            msg.IsBodyHtml = true;
            msg.From = new System.Net.Mail.MailAddress( ConfigurationManager.AppSettings["SMTPUsername"], "Equipo de EvaluandoSoftware.com" );
            msg.Subject = "Confirmar registro en EvaluandoSoftware.com";
            msg.Body = "Estimado " + user.FirstName + " " + user.LastName + ":<br><br>Usted ha iniciado el proceso de registro en Evaluando Software. <br>Por favor haga click <a href='" + callbackUrl + "'>aquí</a> para confirmar su registracion. <br><br> Lo saluda atentamente, <br>el equipo de <a href='evaluandosoftware.com' target='_blank'>EvaluandoSoftware.com</a>";
            emailSender.Send( msg );

            var updateUser = modelContext.Users.First( x => x.Id == user.Id );
            updateUser.confirmToken = code;
            modelContext.SaveChanges();
        }

        private void enviarMailHabilitacionToAdmin( ApplicationUser user )
        {
            var emailSender = new EmailSender();

            var email = ConfigurationManager.AppSettings["SMTPUsername"];
            var subject = "Usuario nuevo pendiente de habilitacion";
            string body = string.Format( "Se registro el usuario '{0} {1}' de la empresa '{2}'", (object)user.FirstName, (object)user.LastName, (object)user.Company.CompanyName );

            MailMessage mailMessage = new MailMessage( email, email, subject, body )
            {
                IsBodyHtml = true
            };

            emailSender.Send( mailMessage );
        }

        private void enviarMailNewVendorToAdmin( ApplicationUser user )
        {
            var emailSender = new EmailSender();

            var email = ConfigurationManager.AppSettings["SMTPUsername"];
            var subject = "Nueva oferta registrada! (no requiere accion)";
            string body = string.Format( "Se registro una nueva empresa: '{0}'. Contacto de la empresa:'{1} {2}'", (object)user.Company.CompanyName, (object)user.FirstName, (object)user.LastName );

            MailMessage mailMessage = new MailMessage( email, email, subject, body )
            {
                IsBodyHtml = true
            };

            emailSender.Send( mailMessage );
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync( ApplicationUser user, bool isPersistent )
        {
            AuthenticationManager.SignOut( DefaultAuthenticationTypes.ExternalCookie );
            var identity = await UserManager.CreateIdentityAsync( user, DefaultAuthenticationTypes.ApplicationCookie );
            AuthenticationManager.SignIn( new AuthenticationProperties() { IsPersistent = isPersistent }, identity );
        }

        private void AddErrors( IdentityResult result )
        {
            foreach( var error in result.Errors )
            {
                ModelState.AddModelError( "", error );
            }
        }

        //TODO: los mappers del viewmodel a los dos models son temporales, luego se resolverá de otra forma.
        private Company VendorViewModelCompanyMap( Models.VendorViewModel model )
        {
            var company = new Company();

            company.CompanyName = model.CompanyName;
            company.CompanyDescription = model.CompanyDescription;
            company.CompanyWebSite = model.CompanyWebSite;

            if( Session["country"] != null )
            {
                company.CompanyCountry = Session["country"].ToString();
            }

            company.CompanyCity = model.CompanyCity;
            company.CompanyAddress = model.CompanyAddress;
            company.CompanyPostalCode = model.CompanyPostalCode;
            company.CompanyBranchOfficesIn = model.CompanyBranchOfficesIn;
            company.CompanyFiscalStartDate = model.CompanyFiscalStartDate;
            company.CompanyFiscalEndDate = model.CompanyFiscalEndDate;
            company.CompanyPeopleInCompany = model.CompanyPeopleInCompany;

            company.ComercialContact = new Contact();

            return company;
        }

        private ApplicationUser VendorViewModelUserMap( Models.VendorViewModel model )
        {
            var email = string.Empty;

            if( Session["email"] != null )
            {
                email = Session["email"].ToString();
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = email
            };

            user.Roles.Add( new IdentityUserRole { RoleId = "2", UserId = user.Id } );

            return user;
        }

        public JsonResult crearUsuarios()
        {
            for( int i = 0; i < 241; i++ )
            {
                var str = RandomString( 10 );
                var user = new ApplicationUser()
                {
                    UserName = str,
                    FirstName = str,
                    LastName = str,
                    Email = str,
                    Enabled = true
                };

                user.Roles.Add( new IdentityUserRole { RoleId = "2", UserId = user.Id } );

                UserManager.UserValidator = new UserValidator<ApplicationUser>( UserManager ) { AllowOnlyAlphanumericUserNames = false };
                IdentityResult result = UserManager.Create( user, str );
                if( !result.Succeeded )
                {
                    List<String> errors = result.Errors.ToList();
                    errors.ForEach( e => Console.WriteLine( e ) );
                }
            }
            modelContext.SaveChanges();

            return Json( "asd" );
        }

        private static Random random = new Random();

        public static string RandomString( int length )
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string( Enumerable.Repeat( chars, length )
              .Select( s => s[random.Next( s.Length )] ).ToArray() );
        }

        private ApplicationUser UserViewModelUserMap( App.Models.UserViewModel model )
        {
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = Session["email"].ToString(),
                Enabled = false
            };

            user.Roles.Add( new IdentityUserRole { RoleId = "2", UserId = user.Id } );

            return user;
        }

        private VendorViewModel ApplicationUsertoVendorViewModelMap( ApplicationUser user )
        {
            var vm = new VendorViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CompanyName = user.Company.CompanyName,
                CompanyDescription = user.Company.CompanyDescription,
                CompanyWebSite = user.Company.CompanyWebSite,
                //CompanyCountry = Session["country"].ToString(),
                CompanyCity = user.Company.CompanyCity,
                CompanyAddress = user.Company.CompanyAddress,
                CompanyPostalCode = user.Company.CompanyPostalCode,
                CompanyBranchOfficesIn = user.Company.CompanyBranchOfficesIn,
                CompanyFiscalStartDate = user.Company.CompanyFiscalStartDate,
                CompanyFiscalEndDate = user.Company.CompanyFiscalEndDate,
                CompanyPeopleInCompany = user.Company.CompanyPeopleInCompany,
            };

            return vm;
        }

        [HttpPost]
        public JsonResult isValidNewUser( string userName )
        {
            var db = new ModelContext();
            if( db.Users.Any( x => x.UserName == userName ) )
                return Json( false );
            else
                return Json( true );
        }

        [HttpPost]
        public JsonResult isValidEmail( string email )
        {
            var db = new ModelContext();
            if( db.Users.Any( x => x.Email == email ) )
                return Json( false );
            else
                return Json( true );
        }

        [MyAuthorize( Roles = "Oferta" )]
        public ActionResult listProducts()
        {
            Dictionary<int, bool> outdatedSurveys = new Dictionary<int, bool>();

            var userId = User.Identity.GetUserId();

            var surveyCompletionParents = modelContext
                .SurveyCompletionParent
                .Include( "Category" )
                .Include( "Product" )
                .Include( "Product.User" )
                .Include( "SurveyCompletions" )
                .Include( "SurveyCompletions.Questions" )
                .Where( x => x.Product.User.Id == userId && x.DeletedAt == null )
                .ToList();

            foreach( var surveyCompletionParent in surveyCompletionParents )
            {
                List<int> answeredQuestionIds = new List<int>();
                surveyCompletionParent.SurveyCompletions.ToList().ForEach( y => y.Questions.ToList().ForEach( z => answeredQuestionIds.Add( z.QuestionId ) ) );

                List<int> surveyIds = new List<int>();
                surveyCompletionParent.SurveyCompletions.ToList().ForEach( y => surveyIds.Add( y.SurveyId ) );

                var questions = modelContext
                    .Questions
                    .Include( "Survey" )
                    .Where( x => surveyIds.Contains( x.Survey.Id ) && !answeredQuestionIds.Contains( x.Id ) && x.Old == false )
                    .ToList();

                outdatedSurveys.Add( surveyCompletionParent.Id, questions.Count() > 0 );
            }

            ViewBag.surveyCompletionParents = surveyCompletionParents;
            ViewBag.outdatedSurveys = outdatedSurveys;

            return View( "~/Views/Oferta/ProductList.cshtml" );
        }

        [MyAuthorize( Roles = "Oferta" )]
        public ActionResult UpdateProduct( int id )
        {
            Dictionary<int, bool> outdatedSurveys = new Dictionary<int, bool>();

            var surveyCompletions = modelContext
                .SurveyCompletionParent
                .Include( "SurveyCompletions" )
                .Include( "SurveyCompletions.CategoryObj" )
                .Include( "SurveyCompletions.Questions" )
                .FirstOrDefault( x => x.Id == id ).SurveyCompletions;

            foreach( var surveyCompletion in surveyCompletions )
            {
                List<int> answeredQuestionIds = new List<int>();
                surveyCompletion.Questions.ToList().ForEach( z => answeredQuestionIds.Add( z.QuestionId ) );

                var questions = modelContext
                    .Questions
                    .Include( "Survey" )
                    .Where( x => x.Survey.Id == surveyCompletion.SurveyId && !answeredQuestionIds.Contains( x.Id ) && x.Old == false )
                    .ToList();

                outdatedSurveys.Add( surveyCompletion.Id, questions.Count() > 0 );
            }

            ViewBag.surveys = surveyCompletions;
            ViewBag.outdatedSurveys = outdatedSurveys;

            return View( "~/Views/Oferta/UpdateProduct.cshtml" );
        }

        [MyAuthorize( Roles = "Oferta" )]
        [HttpGet]
        public ActionResult updateSurvey( int id )
        {
            var surveysCompletion = modelContext
                .SurveysCompletion
                .Include( "CategoryObj" )
                .Include( "Questions" )
                .FirstOrDefault( x => x.Id == id );

            var answeredQuestionIds = new List<int>();
            surveysCompletion
                .Questions
                .ToList()
                .ForEach( x => answeredQuestionIds.Add( x.QuestionId ) );

            var questions = modelContext
                .Questions
                .Include( "Survey" )
                .Where( x => x.Survey.Id == surveysCompletion.SurveyId ) // && !answeredQuestionIds.Contains(x.Id) && x.Old == false)
                .Select( x => new SurveyQuestionDTO
                {
                    QuestionId = x.Id,
                    Question = x.SupplyQuestion,
                    Required = x.SupplyRequired,
                    Old = x.Old
                } )
                .ToList();

            ViewBag.survey = surveysCompletion;

            var userId = this.User.Identity.GetUserId();
            var user = modelContext
                .Users
                .Single( x => x.Id == userId );

            if( !user.CompanyId.HasValue )
            {
                throw new ArgumentNullException( "User company id is null" );
            }

            var model = new SurveyUpdateViewModel()
            {
                CompanyId = user.CompanyId.Value,
                SurveyQuestionDTOs = questions,
            };

            return View( "~/Views/Oferta/UpdateSurvey.cshtml", model );
        }

        [MyAuthorize( Roles = "Oferta" )]
        [HttpPost]
        public ActionResult updateSurvey( SurveyViewModel model )
        {
            Dictionary<int, bool> outdatedSurveys = new Dictionary<int, bool>();

            var surveysCompletions = modelContext
                .SurveysCompletion
                .Include( "CategoryObj" )
                .Include( "Questions" )
                .FirstOrDefault( x => x.Id == model.SurveyCompletionId );

            List<int> answeredQuestionIds = new List<int>();
            surveysCompletions.Questions.ToList().ForEach( z => answeredQuestionIds.Add( z.QuestionId ) );

            var questions = modelContext
                .Questions
                .Include( "Survey" )
                .Include( "Answers" )
                .Where( x => x.Survey.Id == surveysCompletions.SurveyId ) // && !answeredQuestionIds.Contains(x.Id) && x.Old == false)
                .ToList();

            var surveysCompletion = modelContext
                .SurveysCompletion
                .Include( "CategoryObj" )
                .Include( "Questions" )
                .Include( "Questions.Answers" )
                .FirstOrDefault( x => x.Id == model.SurveyCompletionId );

            foreach( var DTO in model.surveyCompletionDTOs )
            {
                if( DTO.Answers != null )
                {
                    bool nueva = false;
                    var question = surveysCompletion.Questions.FirstOrDefault( x => x.QuestionId == DTO.QuestionId );
                    if( question == null )
                    {
                        question = new SurveyCompletionQuestion();
                        nueva = true;
                    }
                    question.Question = DTO.Question;
                    question.QuestionId = DTO.QuestionId;
                    if( !nueva && question.Answers != null && question.Answers.Count > 0 ) question.Answers.Clear();
                    question.Answers = questions
                                .Where( x => x.Id == DTO.QuestionId )
                                .FirstOrDefault()
                                .Answers
                                .Where( x => DTO.Answers.Contains( x.Id ) )
                                .Select( x => new SurveyCompletionAnswer
                                {
                                    Answer = x.SupplyAnswer,
                                    AnswerValue = x.Value
                                } )
                                .ToList();
                    if( nueva ) surveysCompletion.Questions.Add( question );
                }
            }
            modelContext.SaveChanges();

            return RedirectToAction( "UpdateProduct/" + model.ProductId );
        }
    }
}