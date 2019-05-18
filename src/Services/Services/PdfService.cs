using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Model.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Services
{
    public class PdfService
    {
        public ModelContext modelContext;
        public SurveyService surveyService;

        public PdfService() 
        {
            this.modelContext = new ModelContext();
            this.surveyService = new SurveyService();
        }

        public string Generate(int surveyCompletionId, string template, string fileName) 
        {
            //Create a byte array that will eventually hold our final PDF
            Byte[] bytes;

            //Boilerplate iTextSharp setup here
            //Create a stream that we can write to, in this case a MemoryStream
            using (var ms = new MemoryStream())
            {
                //Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                using (var doc = new Document())
                {
                    //Create a writer that's bound to our PDF abstraction and our stream
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        //Open the document for writing
                        doc.Open();

                        //XMLWorker also reads from a TextReader and not directly from a string
                        using (var srHtml = new StringReader(@template))
                        {
                            //Parse the HTML
                            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                        }

                        doc.Close();
                    }
                }

                bytes = ms.ToArray();
            }
            
            System.IO.File.WriteAllBytes(fileName, bytes);

            return fileName;
        }

        public string GetEvaluationReportFileName(int surveyCompletionId) 
        {
            var identifier = this.modelContext
                .SurveysCompletion
                .Include("Customer")
                .FirstOrDefault(x => x.Id == surveyCompletionId)
                .Identifier;

            var fileName = "Informe_de_Evaluación-" + identifier;

            var fullName = string.Format(
                System.Web.HttpContext.Current.Server.MapPath("~\\") + @"\Content\PDF\{0}.pdf",
                fileName);

            return fullName;
        }

        public string GetEvaluationFileName(int surveyCompletionId)
        {
            var identifier = this.modelContext
                .SurveysCompletion
                .Include("Company")
                .FirstOrDefault(x => x.Id == surveyCompletionId)
                .Identifier;

            var fileName = "Registro_de_Producto-" + identifier;

            var fullName = string.Format(
                System.Web.HttpContext.Current.Server.MapPath("~\\") + @"\Content\PDF\{0}.pdf",
                fileName);

            return fullName;
        }
    }
}