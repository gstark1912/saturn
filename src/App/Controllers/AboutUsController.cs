using System.Web;
using System.Web.Mvc;

namespace App.Controllers.Oferta
{
    public class AboutUsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Home/About.cshtml");
        }
	}
}