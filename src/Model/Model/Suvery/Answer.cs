using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Suvery
{
    public class Answer
    {
        public int Id { get; set; }
        public string SupplyAnswer { get; set; }
        public string DemandAnswer { get; set; }
        public int Value { get; set; }

        public string showSupplyAnswer(){
            return SupplyAnswer + " (" + Value + ")";
        }

        public string showDemandAnswer(){
            return DemandAnswer + " (" + Value + ")";
        }
    }
}
