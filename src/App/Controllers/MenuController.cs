using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    public class MenuController : Controller
    {
        //
        // GET: /Menu/
        public ActionResult Index()
        {
            if (User.IsInRole("Admin")) 
            {
                return PartialView("Admin");
            }

            if (User.IsInRole("Oferta"))
            {
                return PartialView("Oferta");
            }

            if (User.IsInRole("Demanda"))
            {
                return PartialView("Demanda");
            }

            return PartialView("Default");
        }
	}
}