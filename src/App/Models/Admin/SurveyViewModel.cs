using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Models.Admin
{
    public class SurveyViewModel
    {
        [Required]
        [Display(Name = "Evaluación")]
        public HttpPostedFileBase SurveyFile { get; set; }

        public string BusinessError { get; set; }
    }
}