using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model.Customer
{
    public class Contact
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
