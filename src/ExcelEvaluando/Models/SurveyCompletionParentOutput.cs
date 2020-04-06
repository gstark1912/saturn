using ExcelLibrary.DataAnnotations;

namespace ExcelEvaluando.Models
{
    public class SurveyCompletionParentOutput
    {
        public string Id { get; set; }

        [StringDatatypeAttribute]
        [HeaderAttribute( "Rubro" )]
        public string Category { get; set; }

        [StringDatatypeAttribute]
        [HeaderAttribute( "Cliente" )]
        public string CustomerFullName { get; set; }

        [StringDatatypeAttribute]
        [HeaderAttribute( "Email" )]
        public string CustomerEmail { get; set; }

        [StringDatatypeAttribute]
        [HeaderAttribute( "Tipo Lector" )]
        public string LectorType { get; set; }

        [StringDatatypeAttribute]
        [HeaderAttribute( "UTM" )]
        public string UTMSource { get; set; }

        [StringDatatypeAttribute]
        [HeaderAttribute( "Fecha" )]
        public string CreatedAt { get; set; }
    }
}