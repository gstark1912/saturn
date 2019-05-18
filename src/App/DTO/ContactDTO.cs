using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DTO
{
    public class ContactDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }
        public string Reason { get; set; }
        public string Message { get; set; }
    }
}