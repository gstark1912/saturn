using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/AccessDenied
        public ActionResult AccessDenied()
        {
            return View();
        }
	}
}