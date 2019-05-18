using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Suvery
{
    public class Question
    {
        public Question() 
        {
            this.Answers = new List<Answer>();
        }

        public int Id { get; set; }
        public Survey Survey { get; set; }
        public string Title { get; set; }
        [Display(Name = "Pregunta para oferta")]
        public string SupplyQuestion { get; set; }
        [Display(Name = "Pregunta para demanda")]
        public string DemandQuestion { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public int AnswerTypeId  { get; set; }
        public AnswerType AnswerType { get; set; }
        public bool SupplyRequired { get; set; }
        public bool DemandRequired { get; set; }
        public bool Old { get; set; }
    }
}
