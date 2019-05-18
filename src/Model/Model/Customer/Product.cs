using System.ComponentModel.DataAnnotations.Schema;
using App.Model.User;

namespace Model.Model.Customer
{
    public class Product
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string WebSite { get; set; }
        public int ProductContactId { get; set; }

        [ForeignKey("ProductContactId")]
        public Contact ProductContact { get; set; }

        public ApplicationUser User { get; set; }
    }
}
