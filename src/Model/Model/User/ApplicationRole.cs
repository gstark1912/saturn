using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity; 

namespace App.Model.User
{
    public class ApplicationRole : IdentityRole
    {
        public bool UserSelection { get; set; }
    }
}