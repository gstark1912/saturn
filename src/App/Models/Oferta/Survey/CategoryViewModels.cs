using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Oferta.Survey
{
    public class CategoryViewModel
    {
        public CategoryViewModel() 
        {
            this.Categories = new List<System.Web.Mvc.SelectListItem>();
        }

        [Required(ErrorMessage = "Por favor seleccione el rubro")]
        [Display(Name = "Rubro")]
        public string Category { get; set; }

        public List<System.Web.Mvc.SelectListItem> Categories { get; set; }
    }
}
