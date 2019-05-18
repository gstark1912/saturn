using App.Models;
using System.Web.Mvc;

namespace App.Controllers.Admin
{
    [MyAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}