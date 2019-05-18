using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Customer
{
    public class Customer
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LectorType { get; set; }
        public string CompanyType { get; set; }
        public string Conutry { get; set; }
        public string City { get; set; }
        public string Sector { get; set; }
        public string Company { get; set; }
        public string RoleInCompany { get; set; }
        public string DeploymentArea { get; set; }
        public string SoftwareInUse { get; set; }
        public string PhoneNumber { get; set; }

        public int Budget { get; set; }
        public int AnnualBilling { get; set; }
        public int PeopleInCompany { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]      
        public decimal BudgetOverBilling 
        {
            get { return AnnualBilling == 0 ? 0 : (decimal)Budget / (decimal)AnnualBilling; }
            private set {}
        }

         [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
         public decimal BudgetOverPeople
         {
             get { return PeopleInCompany == 0 ? 0 : (decimal)Budget / (decimal)PeopleInCompany; }
             private set { }
         }

         [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
         public decimal BillingOverPeople
         {
             get { return PeopleInCompany == 0 ? 0 : (decimal)AnnualBilling / (decimal)PeopleInCompany; }
             private set { }
         }
    }
}
