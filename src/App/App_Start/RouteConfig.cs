using System.Web.Mvc;
using System.Web.Routing;

namespace App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Admin

            routes.MapRoute(
                name: "AdminSurveyCompletionDemanda",
                url: "Admin/EvaluationCompletion/Demanda/{action}/{id}",
                defaults: new { controller = "AdminSurveyCompletionDemanda", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Admin" });

            routes.MapRoute(
                name: "AdminSurveyCompletionOferta",
                url: "Admin/EvaluationCompletion/Oferta/{action}/{id}",
                defaults: new { controller = "AdminSurveyCompletionOferta", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Admin" });

            routes.MapRoute(
                name: "AdminSurvey",
                url: "Admin/Evaluation/{action}/{id}",
                defaults: new { controller = "AdminSurvey", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Admin" });

            routes.MapRoute(
                name: "AdminUsers",
                url: "Admin/Evaluation/Usuarios/{action}/{id}",
                defaults: new { controller = "AdminUsers", action = "Index", id = 1 },
                namespaces: new[] { "App.Controllers.Admin" });

            routes.MapRoute(
                name: "AdminEvaluationDeleteQuestion",
                url: "Admin/Evaluation/{action}/{id}/{questionId}",
                defaults: new { controller = "AdminSurvey" },
                namespaces: new[] { "App.Controllers.Admin" });

            routes.MapRoute(
                name: "Admin",
                url: "Admin/{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Admin" });

            #endregion Admin

            #region Oferta

            routes.MapRoute(
                name: "OfertaCategory",
                url: "Product/Category/{action}/{id}",
                defaults: new { controller = "OfertaSurvey", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Oferta" });

            routes.MapRoute(
                name: "OfertaSurvey",
                url: "Product/Evaluation",
                defaults: new { controller = "OfertaSurvey", action = "Survey", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Oferta" });

            routes.MapRoute(
                name: "OfertaSurveyContinue",
                url: "Product/EvaluationContinue",
                defaults: new { controller = "OfertaSurvey", action = "SurveyContinue", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Oferta" });

            routes.MapRoute(
                name: "OfertaSurveyAction",
                url: "Product/Evaluation/{action}",
                defaults: new { controller = "OfertaSurvey", action = "Survey", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Oferta" });

            routes.MapRoute(
                name: "Oferta",
                url: "Product/{action}/{id}",
                defaults: new { controller = "OfertaSurvey", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers.Oferta" });

            #endregion Oferta

            #region Demanda

            routes.MapRoute(
                name: "Evaluation",
                url: "Evaluation/{action}/{id}",
                defaults: new { controller = "Survey", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers" });

            routes.MapRoute(
                name: "Continue",
                url: "Continue/{action}/{id}",
                defaults: new { controller = "Continue", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers" });

            #endregion Demanda

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "App.Controllers" });
        }
    }
}