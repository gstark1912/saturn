using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Survey
{
    public class CategoryViewModel
    {
        public CategoryViewModel() 
        {
            this.Categories = new List<System.Web.Mvc.SelectListItem>();
        }

        [Display(Name = "Rubro")]
        [Required]
        public string Category { get; set; }

        public List<System.Web.Mvc.SelectListItem> Categories { get; set; }
    }
}
