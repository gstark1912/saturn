using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace App
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo culture = new CultureInfo("es-AR");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}