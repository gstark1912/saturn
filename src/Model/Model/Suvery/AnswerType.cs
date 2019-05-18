using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Suvery
{
    public class AnswerType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString ()
        {
            return Name;
        }
    }
}
