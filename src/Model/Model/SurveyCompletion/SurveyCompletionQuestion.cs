using App.Model;
using Model.Suvery;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Model.SurveyCompletion
{
    public class SurveyCompletionQuestion
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        [ForeignKey("QuestionId")]
        public Question QuestionObj { get; set; }
        public virtual ICollection<SurveyCompletionAnswer> Answers { get; set; }   
    }
}
