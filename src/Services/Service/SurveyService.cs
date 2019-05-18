using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Model.Context;
using Model.Suvery;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Services
{
    public class SurveyService
    {
        public int FirstQuestionRow = 6;

        public int CategoryParentColumn = 1;
        public int CategoryIDColumn = 2;
        public int CategoryNameColumn = 3;
        public int QuestionTypeColumn = 4; // A
        public int AnswerTypeColumn = 5; // B
        public int QuestionTitleColumn = 6; // C
        public int QuestionSupplyColumn = 7; // D
        public int QuestionDemandColumn = 8; // E
        public int AnswerValueColumn = 9; // F
        public int AnswerSupplyColumn = 10; // G
        public int AnswerDemandColumn = 11; // H
        public int QuestionSupplyRequiredColumn = 12; // F
        public int QuestionDemandRequiredColumn = 13; // G

        public ModelContext modelContext;

        public SurveyService() 
        {
            this.modelContext = new ModelContext();
        }

        public List<Survey> Create(Stream stream)
        {
            var surveys = new List<Survey>();
            Dictionary<String, Category> categories = new Dictionary<String,Category>();
            
            using (var package = new ExcelPackage(stream))
            {
                bool updateCategory = false;
                
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();

                var columnsCount = workSheet.Dimension.End.Column - 1; // -1 because always returns one more
                var rowsCount = workSheet.Dimension.End.Row;

                
                
                for (int row = this.FirstQuestionRow; row <= rowsCount;)
                {
                    if ( this.GetStringValue(workSheet, row, this.CategoryParentColumn) == "FIN") break;
                    var catParentID = this.GetStringValue(workSheet, row, this.CategoryParentColumn);
                    var catID = this.GetStringValue(workSheet, row, this.CategoryIDColumn);
                    var catName = this.GetStringValue(workSheet, row, this.CategoryNameColumn);
                    var catParent = catParentID == "" ? null : categories[catParentID];

                    //veo si ya existe la categoria
                    if (catParent == null)
                    {
                        var category = this.modelContext
                            .Categories
                            .FirstOrDefault(x => x.Name == catName);

                        if (category != null)
                        {
                            updateCategory = true;
                        }
                    }


                    Survey survey = new Survey();
                    
                    //creo la categoria
                    survey.Category = new Category
                    { 
                        Name = catName,
                        parentCategory = catParent,
                        CompaniesDirectory = workSheet.Cells[2, 2].Value.ToString(),
                        ConsultantsDirectory = workSheet.Cells[3, 2].Value.ToString()
                    };

                    //si estoy actualizando, me fijo si existe una survey con esa categoria
                    /*if (updateCategory) { 
                        var survey_replace = this.modelContext
                                    .Surveys
                                    .Include("Category")
                                    .FirstOrDefault(x => x.Category.getFullName() == survey.Category.getFullName());
                        if (survey_replace != null) //si existe, la reemplazo
                            survey = survey_replace;
                    }*/

                    //agrego la categoria a la lista de subcategorias del parent
                    if (catParent != null)
                        catParent.subCategories.Add(survey.Category);

                    categories.Add(catID.ToString(), survey.Category);
                    
                    
                    row++; //ya me guardé los datos de la categoria, avanzo a la siguiente fila para procesar las preguntas
                    
                    //proceso las filas hasta la siguiente categoria o hasta fin de archivo
                    while (this.GetStringValue(workSheet, row, this.CategoryIDColumn).ToString() == "" && row <=rowsCount){
                    
                        var questionType = this.GetStringValue(workSheet, row, this.QuestionTypeColumn);

                        if (questionType != "PREGUNTA" && questionType != "RESPUESTA") // file end
                        {
                            break;
                        }

                        var supplyQuestion = this.GetStringValue(workSheet, row, this.QuestionSupplyColumn);
                        var demandQuestion = this.GetStringValue(workSheet, row, this.QuestionDemandColumn);
                        var supplyRequired = this.GetStringValue(workSheet, row, this.QuestionSupplyRequiredColumn);
                        var demandRequired = this.GetStringValue(workSheet, row, this.QuestionDemandRequiredColumn);
                        var answerTypeName = this.GetStringValue(workSheet, row, this.AnswerTypeColumn);

                        var answerType = this.modelContext
                            .AnswerTypes
                            .Where(x => x.Name == answerTypeName)
                            .FirstOrDefault();

                        var question = new Question
                        {
                            Title = this.GetStringValue(workSheet, row, this.QuestionTitleColumn),
                            SupplyQuestion = supplyQuestion,
                            DemandQuestion = demandQuestion,
                            AnswerTypeId = answerType.Id,
                            SupplyRequired = supplyRequired == "SI",
                            DemandRequired = demandRequired == "SI",
                            Old = false
                        };

                        for (int rowAnswer = row + 1; rowAnswer > 1; rowAnswer++)
                        {
                            questionType = this.GetStringValue(workSheet, rowAnswer, this.QuestionTypeColumn);

                            if (questionType != "RESPUESTA")
                            {
                                survey.Questions.Add(question);

                                row = rowAnswer - 1;
                                break;
                            }

                            var supplyAnswer = this.GetStringValue(workSheet, rowAnswer, this.AnswerSupplyColumn);

                            var demandAnswer = this.GetStringValue(workSheet, rowAnswer, this.AnswerDemandColumn);

                            var value = workSheet.Cells[rowAnswer, this.AnswerValueColumn].Value != null ?
                                Convert.ToInt32(workSheet.Cells[rowAnswer, this.AnswerValueColumn].Value) :
                                0;

                            var answer = new Answer
                            {
                                SupplyAnswer = supplyAnswer,
                                DemandAnswer = demandAnswer,
                                Value = value
                            };

                            question.Answers.Add(answer);
                        }
                        row++;
                    }
                    //terminé de agregar las preguntas a la survey
                    //agrego la surve a la lista de surveys para devolver
                    surveys.Add(survey);
                }
            }

            return surveys;
        }

        public IEnumerable<string> ValidateCreation(Stream stream)
        {
            var errors = new List<string>();

            using (var package = new ExcelPackage(stream))
            {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();

                var columnsCount = workSheet.Dimension.End.Column - 1; // -1 because always returns one more
                var rowsCount = workSheet.Dimension.End.Row;

                // survey should have category
                if (workSheet.Cells[1, 2].Value == null)
                {
                    errors.Add("Fila 1, Columna 2: Rubro incompleta");
                }
                else {
                    // category should be unique

                    var categoryName = workSheet.Cells[1, 2].Value.ToString();

                    var category = this.modelContext
                        .Categories
                        .FirstOrDefault(x => x.Name == categoryName);

                    if (category != null)
                    {
                        errors.Add("Fila 1, Columna 2: El Rubro ya existe");
                    }
                }

                // survey should have Directorio de Empresas
                if (workSheet.Cells[2, 2].Value == null)
                {
                    errors.Add("Fila 2, Columna 2: Directorio de Empresas incompleto");
                }

                // survey should have  Directorio de Consultores
                if (workSheet.Cells[3, 2].Value == null)
                {
                    errors.Add("Fila 3, Columna 2: Directorio de Consultores incompleto");
                }

                // first row should be a question
                if (this.GetStringValue(workSheet, this.FirstQuestionRow, this.QuestionTypeColumn) != "PREGUNTA") 
                {
                    errors.Add("Fila " + this.FirstQuestionRow + ": La evaluación debe comenzar con una pregunta");
                }

                // question should have answers
                for (int row = this.FirstQuestionRow; row <= rowsCount; row++)
                {
                    var questionType = this.GetStringValue(workSheet, row, this.QuestionTypeColumn);

                    if (questionType != "PREGUNTA" && questionType != "RESPUESTA" && questionType != string.Empty) 
                    {
                        errors.Add("Fila " + row + ": El tipo de pregunta " + questionType + " es incorrecto");
                    }

                    if (questionType == "PREGUNTA")
                    {
                        var nextQuestionType = this.GetStringValue(workSheet, row + 1, this.QuestionTypeColumn);

                        if (nextQuestionType != "RESPUESTA")
                        {
                            errors.Add("Fila " + row + ": La pregunta no cuenta con respuestas");
                        }

                        var supplyRequired = this.GetStringValue(workSheet, row, this.QuestionSupplyRequiredColumn);
                        var demandRequired = this.GetStringValue(workSheet, row, this.QuestionDemandRequiredColumn);

                        if (supplyRequired != "SI" && supplyRequired != "NO") 
                        {
                            errors.Add("Fila " + row + ": El campo 'Requerida Oferta' es incorrecto");
                        }

                        if (demandRequired != "SI" && demandRequired != "NO")
                        {
                            errors.Add("Fila " + row + ": El campo 'Requerida Demanda' es incorrecto");
                        }
                    }

                    if (questionType == "RESPUESTA")
                    {
                        if (workSheet.Cells[row, this.AnswerValueColumn].Value == null)
                        {
                            errors.Add("Fila " + row + ": La respuesta no cuenta con un valor asignado");
                        }
                    }
                }
            }

            return errors;
        }

        public void Remove(int categoryId) 
        {

            //Elimino recursivamente todas las categorías hijas
            var child_categories = this.modelContext
                .Categories
                .Where(x => x.parentCategory.Id == categoryId).ToList();

            foreach (var child_category in child_categories)
                Remove(child_category.Id);

            var category = this.modelContext
                .Categories
                .FirstOrDefault(x => x.Id == categoryId);

            var survey = this.modelContext
                .Surveys
                .Include("Questions")
                .Include("Questions.Answers")
                .FirstOrDefault(x => x.Category.Id == categoryId);

            var questions = survey.Questions;

            foreach (var question in questions)
            {
                this.modelContext.Answers.RemoveRange(question.Answers);

                this.modelContext.SaveChanges();
            }

            this.modelContext.Questions.RemoveRange(questions);

            this.modelContext.Surveys.Remove(survey);
            this.modelContext.Categories.Remove(category);

            this.modelContext.SaveChanges();
        }

        public string GetStringValue(ExcelWorksheet workSheet, int row, int column)
        {
            if (workSheet.Cells[row, column].Value != null)
            {
                return workSheet.Cells[row, column].Value.ToString();
            }

            return string.Empty;
        }
    }
}