using App.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Models.Survey
{
    public class QuestionViewModel
    {
        public QuestionViewModel() 
        {
            this.ChosenAnswers = new List<AnswerDTO>();
        }

        public int Identifier { get; set; }
        public int Id { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public IEnumerable<AnswerDTO> Answers { get; set; }
        public IEnumerable<AnswerDTO> ChosenAnswers { get; set; }
        public bool Required { get; set; }
    }
}