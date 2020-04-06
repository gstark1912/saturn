using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExcelLibrary.DataAnnotations
{
    public class WidthAttribute : ValidationAttribute
    {
        public readonly int Width;
        public WidthAttribute( int _width )
            : base( "" )
        {
            Width = _width;
        }

    }
}