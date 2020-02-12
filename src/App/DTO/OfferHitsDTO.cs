using System.Collections.Generic;

namespace App.DTO
{
    public class OfferDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }

    public class OfferHitsDTO
    {
        public OfferDTO Offer { get; set; }
        public int Count { get; set; }
    }

    public class CompanyHitsDTO
    {
        public CompanyHitsDTO()
        {
            this.Offer = new List<OfferHitsDTO>();
        }
        public List<OfferHitsDTO> Offer { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}