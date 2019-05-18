using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DTO
{
    public class SurveyDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SurveyId { get; set; }
        public List<SurveyQuestionDTO> SurveyQuestionDTOs { get; set; }
        public int QuestionCount { get; set; }
    }
}