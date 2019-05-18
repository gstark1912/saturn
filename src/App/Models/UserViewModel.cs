using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace App.Models
{
    public class UserViewModel
    {
        public UserViewModel()
            {
            
        }

        [Required]
        [Display(Name = "Nombre de Usuario")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

   

    }
}
