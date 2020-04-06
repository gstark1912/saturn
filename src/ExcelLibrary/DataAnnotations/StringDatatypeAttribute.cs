using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelLibrary.DataAnnotations
{
    public class StringDatatypeAttribute : ValidationAttribute
    {
        public readonly int Width;
        public StringDatatypeAttribute()
            : base( "" )
        {
        }
    }
}