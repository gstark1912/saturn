using App.Models.Survey;
using Model.Context;
using System.Linq;
using System.Web.Mvc;

namespace App.Controllers.Oferta
{
    public class CategoryController : Controller
    {
        public ModelContext modelContext;

        public CategoryController()
        {
            this.modelContext = new ModelContext();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var categories = this.modelContext
                .Categories
                .Where(x => x.parentCategory == null)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToList();

            var model = new CategoryViewModel
            {
                Categories = categories
            };

            return View("~/Views/Survey/Category.cshtml", model);
        }

        [HttpGet]
        public JsonResult SubCategories(int category)
        {
            var result = new JsonResult();

            var categories = this.modelContext
                .Categories
                .ToList()
                .Where(x => x.parentCategory != null && x.parentCategory.Id == category);

            var json = "[]";

            if (categories.Any())
            {
                json = "[";
                foreach (var item in categories)
                {
                    json = json + item.toJson() + ",";
                }
                json = json.Substring(0, json.Length - 1) + "]";
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}