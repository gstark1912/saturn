using Model.Context;
using OfficeOpenXml;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace App.Models.Admin
{
    public class SurveyEditViewModel : IValidatableObject
    {
        private SurveyService surveyService;
        private ModelContext modelContext;

        public SurveyEditViewModel() 
        {
            this.surveyService = new SurveyService();
            this.modelContext = new ModelContext();
        }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Evaluación")]
        public HttpPostedFileBase SurveyFile { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) 
        {
            if (!this.SurveyFile.FileName.EndsWith(".xlsx"))
            {
                yield return new ValidationResult("Seleccione un archivo XLSX. (Archivo Excel versión 2007 o superior)");
                yield break;
            }

            byte[] fileBytes = new byte[this.SurveyFile.ContentLength];
            var data = this.SurveyFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(this.SurveyFile.ContentLength));

            var stream = this.SurveyFile.InputStream;

            using (var package = new ExcelPackage(stream))
            {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();

                var columnsCount = workSheet.Dimension.End.Column - 1; // -1 because always returns one more
                var rowsCount = workSheet.Dimension.End.Row;

                // survey should have category
                if (workSheet.Cells[1, 2].Value == null)
                {
                    yield return new ValidationResult("Fila 1, Columna 2: Rubro incompleta");
                }
                else
                {
                    // category should be unique

                    var categoryName = workSheet.Cells[1, 2].Value.ToString();

                    var categories = this.modelContext
                        .Surveys
                        .Include("Category")
                        .Where(x =>
                            x.Category.Name == categoryName
                            && x.Category.Id != this.CategoryId)
                        .ToList();

                    if (categories.Count() > 0)
                    {
                        yield return new ValidationResult("Fila 1, Columna 2: El Rubro ya existe");
                    }
                }

                // survey should have Directorio de Empresas
                if (workSheet.Cells[2, 2].Value == null)
                {
                    yield return new ValidationResult("Fila 2, Columna 2: Directorio de Empresas incompleto");
                }

                // survey should have  Directorio de Consultores
                if (workSheet.Cells[3, 2].Value == null)
                {
                    yield return new ValidationResult("Fila 3, Columna 2: Directorio de Consultores incompleto");
                }

                // first row should be a question
                if (this.surveyService.GetStringValue(workSheet, this.surveyService.FirstQuestionRow, this.surveyService.QuestionTypeColumn) != "PREGUNTA")
                {
                    yield return new ValidationResult("Fila " + this.surveyService.FirstQuestionRow + ": La evaluación debe comenzar con una pregunta");
                }

                // question should have answers
                for (int row = this.surveyService.FirstQuestionRow; row <= rowsCount; row++)
                {
                    var questionType = this.surveyService.GetStringValue(workSheet, row, this.surveyService.QuestionTypeColumn);

                    if (questionType != "PREGUNTA" && questionType != "RESPUESTA" && questionType != string.Empty)
                    {
                        yield return new ValidationResult("Fila " + row + ": El tipo de pregunta " + questionType + " es incorrecto");
                    }

                    if (questionType == "PREGUNTA")
                    {
                        var nextQuestionType = this.surveyService.GetStringValue(workSheet, row + 1, this.surveyService.QuestionTypeColumn);

                        if (nextQuestionType != "RESPUESTA")
                        {
                            yield return new ValidationResult("Fila " + row + ": La pregunta no cuenta con respuestas");
                        }

                        var supplyRequired = this.surveyService.GetStringValue(workSheet, row, this.surveyService.QuestionSupplyRequiredColumn);
                        var demandRequired = this.surveyService.GetStringValue(workSheet, row, this.surveyService.QuestionDemandRequiredColumn);

                        if (supplyRequired != "SI" && supplyRequired != "NO")
                        {
                            yield return new ValidationResult("Fila " + row + ": El campo 'Requerida Oferta' es incorrecto");
                        }

                        if (demandRequired != "SI" && demandRequired != "NO")
                        {
                            yield return new ValidationResult("Fila " + row + ": El campo 'Requerida Demanda' es incorrecto");
                        }
                    }

                    if (questionType == "RESPUESTA") 
                    {
                        if (workSheet.Cells[row, this.surveyService.AnswerValueColumn].Value == null)
                        {
                            yield return new ValidationResult("Fila " + row + ": La respuesta no cuenta con un valor asignado");
                        }
                    }
                }
            }
        }
    }
}