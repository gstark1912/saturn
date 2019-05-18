using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DTO
{
    public class SurveyCompletionDTO
    {
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string Question { get; set; }
        public string Required { get; set; }
        public ICollection<int> Answers { get; set; }
    }
}