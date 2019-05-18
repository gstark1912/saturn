using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace App.Models.Oferta
{
    public class RegisterViewModel
    {
        public RegisterViewModel() 
        {
            this.Countries = new List<SelectListItem>
            {
                new SelectListItem { Text = "Argentina", Value = "Argentina" },
                new SelectListItem { Text = "Bolivia", Value = "Bolivia" },
                new SelectListItem { Text = "Brasil", Value = "Brasil" },
                new SelectListItem { Text = "Canadá", Value = "Canadá" },
                new SelectListItem { Text = "Chile", Value = "Chile" },
                new SelectListItem { Text = "Colombia", Value = "Colombia" },
                new SelectListItem { Text = "Costa Rica", Value = "Costa Rica" },
                new SelectListItem { Text = "Cuba", Value = "Cuba" },
                new SelectListItem { Text = "Ecuador", Value = "Ecuador" },
                new SelectListItem { Text = "EEEUU", Value = "EEEUU" },
                new SelectListItem { Text = "El Salvador", Value = "El Salvador" },
                new SelectListItem { Text = "España", Value = "España" },
                new SelectListItem { Text = "Honduras", Value = "Honduras" },
                new SelectListItem { Text = "México", Value = "México" },
                new SelectListItem { Text = "Nicaragua", Value = "Nicaragua" },
                new SelectListItem { Text = "Panamá", Value = "Panamá" },
                new SelectListItem { Text = "Paraguay", Value = "Paraguay" },
                new SelectListItem { Text = "Perú", Value = "Perú" },
                new SelectListItem { Text = "Uruguay", Value = "Uruguay" },
                new SelectListItem { Text = "Venezuela", Value = "Venezuela" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            this.Months = new List<SelectListItem>
            {
                new SelectListItem { Text = "Enero", Value = "Enero" },
                new SelectListItem { Text = "Febrero", Value = "Febrero" },
                new SelectListItem { Text = "Marzo", Value = "Marzo" },
                new SelectListItem { Text = "Abril", Value = "Abril" },
                new SelectListItem { Text = "Mayo", Value = "Mayo" },
                new SelectListItem { Text = "Junio", Value = "Junio" },
                new SelectListItem { Text = "Julio", Value = "Julio" },
                new SelectListItem { Text = "Agosto", Value = "Agosto" },
                new SelectListItem { Text = "Septiembre", Value = "Septiembre" },
                new SelectListItem { Text = "Octubre", Value = "Octubre" },
                new SelectListItem { Text = "Noviembre", Value = "Noviembre" },
                new SelectListItem { Text = "Diciembre", Value = "Diciembre" },
            };
        }

        public int CategoryId { get; set; }
        public int SurveyCompletionId { get; set; }

        //[Required]
        [Display(Name = "Nombre")]
        public string CompanyName { get; set; }
        
        //[Required]
        [Display(Name = "Descripción")]
        public string CompanyDescription { get; set; }

        //[Required]
        [Display(Name = "Sitio web")]
        public string CompanyWebSite { get; set; }

        public List<SelectListItem> Countries { get; set; }

        //[Required]
        [Display(Name = "País")]
        public string CompanyCountry { get; set; }

        //[Required]
        [Display(Name = "Ciudad")]
        public string CompanyCity { get; set; }

        //[Required]
        [Display(Name = "Dirección")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Código Postal")]
        public string CompanyPostalCode { get; set; }

        [Display(Name = "Sucursales en")]
        public string CompanyBranchOfficesIn { get; set; }

        public List<SelectListItem> Months { get; set; }

        //[Required]
        [Display(Name = "Mes de inicio del año fiscal")]
        public string CompanyFiscalStartDate { get; set; }

        //[Required]
        [Display(Name = "Mes de cierre del año fiscal")]
        public string CompanyFiscalEndDate { get; set; }

        //[Required]
        [Display(Name = "Personal total de la empresa")]
        public int CompanyPeopleInCompany { get; set; }

        [Display(Name = "Logo")]
        public HttpPostedFileBase CompanyLogo { get; set; }

        public string CompanyLogoFileName { get; set; }

        //[Required]
        [Display(Name = "Nombre")]
        public string ProductName { get; set; }

        [Display(Name = "Version")]
        public string ProductVersion { get; set; }

        //[Required]
        [Display(Name = "Descripción")]
        public string ProductDescription { get; set; }

        [Display(Name = "Sitio web")]
        public string ProductWebSite { get; set; }

        //[Required]
        [Display(Name = "Nombre y Apellido")]
        public string ComercialContactFullName { get; set; }

        //[Required]
        [Display(Name = "Cargo / Función")]
        public string ComercialContactPosition { get; set; }

        //[Required]
        [Display(Name = "Teléfono")]
        public string ComercialContactPhone { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string ComercialContactEmail { get; set; }

        [Display(Name = "Nombre y Apellido")]
        public string ProductContactFullName { get; set; }

        [Display(Name = "Cargo / Función")]
        public string ProductContactPosition { get; set; }

        [Display(Name = "Teléfono")]
        public string ProductContactPhone { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string ProductContactEmail { get; set; }
    }
}
