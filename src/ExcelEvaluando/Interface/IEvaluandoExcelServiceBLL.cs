using ExcelEvaluando.Models;
using System.Collections.Generic;
using System.IO;

namespace ExcelEvaluando.Interface
{
    public interface IEvaluandoExcelServiceBLL
    {
        void WriteDataInExcel( MemoryStream stream, IEnumerable<SurveyCompletionParentOutput> data );
    }
}