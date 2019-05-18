using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DTO
{
    public class QuestionDTO
    {
        public int Identifier { get; set; }
        public int Id { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public List<AnswerDTO> Answers { get; set; }
        public IEnumerable<AnswerDTO> ChosenAnswers { get; set; }
        public bool Required { get; set; }

    }
}