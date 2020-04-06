using ExcelEvaluando.Interface;
using ExcelEvaluando.Models;
using ExcelLibrary;
using ExcelLibrary.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExcelEvaluando
{
    public class EvaluandoExcelBLL : IEvaluandoExcelServiceBLL
    {
        private IExcelService service;

        public EvaluandoExcelBLL()
        {
            service = new ExcelService();
        }

        public void WriteDataInExcel( MemoryStream stream, IEnumerable<SurveyCompletionParentOutput> data )
        {
            if( data.ToList().Count > 0 )
            {
                service.WriteDataInMemory<SurveyCompletionParentOutput>( stream, data, true );
            }
        }
    }
}