using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelLibrary.Exceptions
{
    public class CAMSExcelLibraryException : Exception
    {
        public CAMSExcelLibraryException( string message )
            : base( message )
        {

        }
    }
}
