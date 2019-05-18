using Model.Model.Customer;
using Model.Model.EvaluationReport;
using Model.Ranking;
using Model.SurveyCompletion;
using Model.Suvery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DTO
{
    public class EvaluationReportTemplateDTO
    {
        public IEnumerable<EvaluationReportRankingDTO> Ranking { get; set; }
        public IEnumerable<EvaluationReportProductDTO> Products { get; set; }
        public SurveyCompletionParent SurveyCompletionParent { get; set; }
        public string Identifier { get; set; }
        public Category Category { get; set; }
    }
}