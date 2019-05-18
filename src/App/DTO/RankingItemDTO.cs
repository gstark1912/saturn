using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DTO
{
    public class RankingItemDTO
    {
        public string VendorLogo { get; set; }
        public string VendorName { get; set; }
        public string ProductName { get; set; }
        public int Rank { get; set; }
        public bool CheckedByES { get; set; }
    }
}