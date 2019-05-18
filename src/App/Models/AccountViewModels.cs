using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña no coincide con su confirmación.")]
        public string ConfirmPassword { get; set; }
    }

     public class ResetPassViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña no coincide con su confirmación.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
    public class ForgotViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterViewModel() 
        {
            this.Roles = new List<System.Web.Mvc.SelectListItem>();
            this.Role1 = true;
        }

        [Required]
        [Display(Name = "Usuario")]
        [StringLength(20, ErrorMessage = "El {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electronico")]
        public string Email { get; set; }

        [Display(Name = "Rol")]
        public List<System.Web.Mvc.SelectListItem> Roles { get; set; }

        [Display(Name = "Rol")]
        public string Role { get; set; }

        [Display(Name = "Rol")]
        public bool Role1 { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La nueva contraseña no coincide con su confirmación.")]
        public string ConfirmPassword { get; set; }
    }
}
