using App.Model.User;
using Model.Context;
using Model.Suvery;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers
{
    public class HomeController : Controller
    {
        private const int MaxValuePerAnswer = 10;
        private ModelContext modelContext;
        private IEnumerable<ApplicationUser> users;
        private IEnumerable<Survey> surveys;

        public HomeController()
        {
            this.modelContext = new ModelContext();
        }

        public ActionResult Index()
        {
            this.users = modelContext.Users.ToList();
            this.surveys = modelContext.Surveys.ToList();

            if (User.IsInRole("Oferta"))
                return RedirectToAction("Home", "Oferta");
            else
                return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}