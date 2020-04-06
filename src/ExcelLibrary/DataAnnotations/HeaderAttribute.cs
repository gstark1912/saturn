using System.ComponentModel.DataAnnotations;

namespace ExcelLibrary.DataAnnotations
{
    public class HeaderAttribute : ValidationAttribute
    {
        public readonly string Text;
        public HeaderAttribute( string _text )
            : base( "" )
        {
            Text = _text;
        }
    }
}