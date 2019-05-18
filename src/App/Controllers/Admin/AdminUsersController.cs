using App.Model.User;
using App.Models;
using Model.Context;
using Services;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace App.Controllers.Admin
{
    [MyAuthorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        public ModelContext modelContext;
        private EmailSender emailSender;

        public AdminUsersController()
        {
            this.modelContext = new ModelContext();
            this.emailSender = new EmailSender();
        }

        //public UserManager<ApplicationUser> UserManager { get; private set; }

        public ActionResult Index()
        {
            var users = modelContext.Users.Include("Company").Where(x => !x.Enabled).ToList();

            ViewBag.UsuariosNoHabilitados = users;

            return View("~/Views/Admin/Users/List.cshtml");
        }

        public ActionResult Habilitar(string id)
        {
            var user = modelContext
                .Users
                .FirstOrDefault(x => x.Id == id);

            user.Enabled = true;
            this.modelContext.SaveChanges();

            this.SendEmailUsuarioHaibilitado(user);

            return View("~/Views/Admin/Users/List.cshtml");
        }

        private void SendEmailUsuarioHaibilitado(ApplicationUser user)
        {
            var email = ConfigurationManager.AppSettings["SMTPUsername"];
            var subject = "Usuario Habilidato!";

            var url = Url.Action("Home", "Index");

            var body = $"Hola {user.FirstName}, le informamos que su usuario ya fue habilitado para su uso, haga click <a href='{url}'>aquí</a> para iniciar sesion.";

            MailMessage mailMessage = new MailMessage(email, email, subject, body)
            {
                IsBodyHtml = true
            };

            this.emailSender.Send(mailMessage);
        }

        /*
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Reject")]
        public ActionResult Reject(int id, RegisterViewModel model)
        {
            var surveyCompletion = this.modelContext
                .SurveysCompletion
                .FirstOrDefault(x => x.Id == id);

            surveyCompletion.Status = "Rechazado";

            this.modelContext.SaveChanges();

            return RedirectToAction("Index", "../Admin/EvaluationCompletion/Oferta");
        }
        */
    }
}