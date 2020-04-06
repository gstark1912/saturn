using System.Collections.Generic;
using System.IO;

namespace ExcelLibrary.Interfaces
{
    public interface IExcelService
    {
        List<T> GetRowsDataFromFile<T>( string filePath, bool fileHasHeader = false ) where T : new();
        void WriteDataInFile<T>( string path, IEnumerable<T> data, bool writeHeader ) where T : class;
        void WriteDataInMemory<T>( MemoryStream stream, IEnumerable<T> data, bool writeHeader ) where T : class;
    }
}