using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Suvery
{
    public class Survey
    {
        public Survey()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
            this.Questions = new List<Question>();
        }

        public int Id { get; set; }
        public Category Category { get; set; }
        public ICollection<Question> Questions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
