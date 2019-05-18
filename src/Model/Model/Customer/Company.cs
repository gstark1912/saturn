using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Model.Customer
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string CompanyName { get; set; }
        [Display(Name = "Descripción")]
        public string CompanyDescription { get; set; }
        [Display(Name = "Sitio Web")]
        public string CompanyWebSite { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPostalCode { get; set; }
        public string CompanyBranchOfficesIn { get; set; }
        public string CompanyFiscalStartDate { get; set; }
        public string CompanyFiscalEndDate { get; set; }
        public int CompanyPeopleInCompany { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyDomain { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public int ComercialContactId { get; set; }


        [ForeignKey("ComercialContactId")]
        public virtual Contact ComercialContact { get; set; }

    }
}