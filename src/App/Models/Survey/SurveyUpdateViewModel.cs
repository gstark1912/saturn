using App.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.Model.Customer;

namespace App.Models.Survey
{
    public class SurveyUpdateViewModel
    {
        public SurveyUpdateViewModel() 
        {
            this.SurveyQuestionDTOs = new List<SurveyQuestionDTO>();
        }

        [Required]
        public int CompanyId { get; set; }
        public List<SurveyQuestionDTO> SurveyQuestionDTOs { get; set; }
    }
}