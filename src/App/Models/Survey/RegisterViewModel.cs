using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace App.Models.Survey
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

            this.LectorTypes = new List<SelectListItem> 
            {
                new SelectListItem { Text = "Consultor independiente", Value = "Consultor independiente" },
                new SelectListItem { Text = "Empresa de consultoría", Value = "Empresa de consultoría" },
                new SelectListItem { Text = "Empresa de software", Value = "Empresa de software" },
                new SelectListItem { Text = "Estudiante", Value = "Estudiante" },
                new SelectListItem { Text = "Usuario final", Value = "Usuario final" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            this.CompanyTypes = new List<SelectListItem> 
            {
                new SelectListItem { Text = "Manufactura", Value = "Manufactura" },
                new SelectListItem { Text = "Fabricación y distribución", Value = "Fabricación y distribución" },
                new SelectListItem { Text = "Distribución", Value = "Distribución" },
                new SelectListItem { Text = "Servicios", Value = "Servicios" },
                new SelectListItem { Text = "Producción (extracción de productos naturales)", Value = "Producción (extracción de productos naturales)" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            this.Sectors = new List<SelectListItem>
            {
                new SelectListItem { Text = "Distribución - Mayoristas", Value = "Distribución - Mayoristas" },
                new SelectListItem { Text = "Distribución - Retail", Value = "Distribución - Retail" },
                new SelectListItem { Text = "Distribución - Transporte", Value = "Distribución - Transporte" },
                new SelectListItem { Text = "Manufactura - Alimentos y Bebidas", Value = "Manufactura - Alimentos y Bebidas" },
                new SelectListItem { Text = "Manufactura - Automotriz", Value = "Manufactura - Automotriz" },
                new SelectListItem { Text = "Manufactura - Curtiembre", Value = "Manufactura - Curtiembre" },
                new SelectListItem { Text = "Manufactura - Laboratorio Medicinal", Value = "Manufactura - Laboratorio Medicinal" },
                new SelectListItem { Text = "Manufactura - Maderera", Value = "Manufactura - Maderera" },
                new SelectListItem { Text = "Manufactura - Maquinaria", Value = "Manufactura - Maquinaria" },
                new SelectListItem { Text = "Manufactura - Metalurgica", Value = "Manufactura - Metalurgica" },
                new SelectListItem { Text = "Manufactura - Plásticos", Value = "Manufactura - Plásticos" },
                new SelectListItem { Text = "Manufactura - Químicos", Value = "Manufactura - Químicos" },
                new SelectListItem { Text = "Manufactura - Textil", Value = "Manufactura - Textil" },
                new SelectListItem { Text = "Producción primaria - Forestal/Agro", Value = "Producción primaria - Forestal/Agro" },
                new SelectListItem { Text = "Producción primaria - Minería", Value = "Producción primaria - Minería" },
                new SelectListItem { Text = "Producción primaria - Petroleo y Gas", Value = "Producción primaria - Petroleo y Gas" },
                new SelectListItem { Text = "Servicios - Agua", Value = "Servicios - Agua" },
                new SelectListItem { Text = "Servicios - Almacenes/Logistica/Distribucion", Value = "Servicios - Almacenes/Logistica/Distribucion" },
                new SelectListItem { Text = "Servicios - Banca/Finanzas", Value = "Servicios - Banca/Finanzas" },
                new SelectListItem { Text = "Servicios - Datos/Conectividad", Value = "Servicios - Datos/Conectividad" },
                new SelectListItem { Text = "Servicios - Edición/Medios/Publicidad", Value = "Servicios - Edición/Medios/Publicidad" },
                new SelectListItem { Text = "Servicios - Educación", Value = "Servicios - Educación" },
                new SelectListItem { Text = "Servicios - Electricidad", Value = "Servicios - Electricidad" },
                new SelectListItem { Text = "Servicios - Gas", Value = "Servicios - Gas" },
                new SelectListItem { Text = "Servicios - Gobierno", Value = "Servicios - Gobierno" },
                new SelectListItem { Text = "Servicios - Intermediarios / Comisionistas", Value = "Servicios - Intermediarios / Comisionistas" },
                new SelectListItem { Text = "Servicios - Mantenimiento", Value = "Servicios - Mantenimiento" },
                new SelectListItem { Text = "Servicios - Minoristas", Value = "Servicios - Minoristas" },
                new SelectListItem { Text = "Servicios - Obras/Construcciones", Value = "Servicios - Obras/Construcciones" },
                new SelectListItem { Text = "Servicios - ONG/Asociación Civil", Value = "Servicios - ONG/Asociación Civil" },
                new SelectListItem { Text = "Servicios - Servicios Profesionales/Consultoría", Value = "Servicios - Servicios Profesionales/Consultoría" },
                new SelectListItem { Text = "Servicios - Telefonía", Value = "Servicios - Telefonía" },
                new SelectListItem { Text = "Servicios - TV por Cable", Value = "Servicios - TV por Cable" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            this.RolesInCompany = new List<SelectListItem> 
            {
                new SelectListItem { Text = "Dirección", Value = "Dirección" },
                new SelectListItem { Text = "Gerencia", Value = "Gerencia" },
                new SelectListItem { Text = "Jefatura", Value = "Jefatura" },
                new SelectListItem { Text = "Operaciones", Value = "Operaciones" },
                new SelectListItem { Text = "Asistente", Value = "Asistente" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };

            this.DeploymentAreas = new List<SelectListItem>
            {
                new SelectListItem { Text = "Gerencia General", Value = "Gerencia General" },
                new SelectListItem { Text = "Recursos Humanos", Value = "Recursos Humanos" },
                new SelectListItem { Text = "Ventas", Value = "Ventas" },
                new SelectListItem { Text = "Tecnología y Sistemas", Value = "Tecnología y Sistemas" },
                new SelectListItem { Text = "Producción", Value = "Producción" },
                new SelectListItem { Text = "Planeamiento", Value = "Planeamiento" },
                new SelectListItem { Text = "Marketing", Value = "Marketing" },
                new SelectListItem { Text = "Logística", Value = "Logística" },
                new SelectListItem { Text = "Gerencia", Value = "Gerencia" },
                new SelectListItem { Text = "Finanzas", Value = "Finanzas" },
                new SelectListItem { Text = "Otro", Value = "Otro" }
            };
        }

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Tipo De Lector")]
        public string LectorType { get; set; }

        public List<SelectListItem> LectorTypes { get; set; }

        [Display(Name = "Tipo de Empresa")]
        public string CompanyType { get; set; }

        public List<SelectListItem> CompanyTypes { get; set; }

        [Required]
        [Display(Name = "País")]
        public string Country { get; set; }

        public List<SelectListItem> Countries { get; set; }

        [Required]
        [Display(Name = "Ciudad")]
        public string City { get; set; }

        [Display(Name = "¿En qué sector de la economía se desenvuelve la empresa?")]
        public string Sector { get; set; }

        public List<SelectListItem> Sectors { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "¿Cuál de los siguientes roles representa mejor su responsabilidad?")]
        public string RoleInCompany { get; set; }

        public List<SelectListItem> RolesInCompany { get; set; }

        [Display(Name = "¿En qué área se desempeña?")]
        public string DeploymentArea { get; set; }

        public List<SelectListItem> DeploymentAreas { get; set; }

        [Display(Name = "¿Qué software empresarial usa su empresa?")]
        public string SoftwareInUse { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        //nuevos
        [Required]
        [Display(Name = "Cantidad de empleados")]
        public int PeopleInCompany { get; set; }

        [Required]
        [Display(Name = "Facturación anual")]
        public int AnnualBilling { get; set; }

        [Required]
        [Display(Name = "Presupuesto estimado")]
        public int Budget { get; set; }
    }
}
