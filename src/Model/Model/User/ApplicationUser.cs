using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Model.Model.Customer;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Model.User
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }
        [Display(Name = "Apellido")]
        public string LastName { get; set; }
        public Boolean Enabled { get; set; }

        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        public string confirmToken { get; set; }
        public string resetPassToken { get; set; }
    }
}