using App.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Survey
{
    public class SurveyViewModel
    {
        public SurveyViewModel()
        {
            this.SurveyDTOs = new List<SurveyDTO>();
            this.surveyCompletionDTOs = new List<SurveyQuestionDTO>();
        }

        public int CompanyId { get; set; }
        public int SurveyId { get; set; }
        public int CategoryId { get; set; }
        public string Email { get; set; }
        public List<SurveyDTO> SurveyDTOs { get; set; }
        public List<SurveyQuestionDTO> surveyCompletionDTOs { get; set; }
        public int ParentSurveyCompletionId { get; set; }
        public string PartialSaveKey { get; set; }
        public int SurveyCompletionId { get; set; }
        public string Source { get; set; }

        //cosass del productoo
        public int ProductId { get; set; }

        [Required]
        [Display( Name = "Nombre" )]
        public string ProductName { get; set; }

        [Display( Name = "Version" )]
        public string ProductVersion { get; set; }

        [Required]
        [Display( Name = "Descripción" )]
        public string ProductDescription { get; set; }

        [Display( Name = "Web Site" )]
        public string ProductWebSite { get; set; }

        [Display( Name = "Nombre y Apellido" )]
        public string ProductContactFullName { get; set; }

        [Display( Name = "Cargo / Función" )]
        public string ProductContactPosition { get; set; }

        [Display( Name = "Teléfono" )]
        public string ProductContactPhone { get; set; }

        [EmailAddress]
        [Display( Name = "Email" )]
        public string ProductContactEmail { get; set; }
    }
}