using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Models
{
    public class ContactViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Motivo de Contacto")]
        public string Reason { get; set; }

        [Required]
        [Display(Name = "Mensaje")]
        public string Message { get; set; }
    }
}