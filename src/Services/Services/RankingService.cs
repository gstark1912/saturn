using Model.Context;
using Model.SurveyCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services
{
    public class RankingService
    {
        private const int MaxValuePerAnswer = 10;
        private ModelContext modelContext;

        public RankingService() 
        {
            this.modelContext = new ModelContext();
        }

        public void GenerarRanking(int surveyCompletionId)
        {
            var surveyCompletionByDemand = this.modelContext
                .SurveysCompletion
                .Include("Questions")
                .Include("Questions.Answers")
                .Where(x => x.Id == surveyCompletionId)
                .FirstOrDefault();

            var surveysCompletionsBySupply = this.modelContext
                .SurveysCompletion
                .Include("Questions")
                .Include("Questions.Answers")
                .Where(x => 
                    x.Role.Name == "Oferta" && 
                    x.SurveyId == surveyCompletionByDemand.SurveyId &&
                    x.Status == "Aprobado")
                .ToList();

            foreach (var surveyCompletionBySupply in surveysCompletionsBySupply)
            {
                var ranking = new Model.Ranking.Ranking()
                {
                    SurveyCompletionDemand = surveyCompletionByDemand,
                    SurveyCompletionSupply = surveyCompletionBySupply
                };

                foreach (var surveyCompletionSupplyQuestion in surveyCompletionBySupply.Questions)
                {
                    var demandQuestion = surveyCompletionByDemand
                        .Questions
                        .Where(x => x.QuestionId == surveyCompletionSupplyQuestion.QuestionId)
                        .FirstOrDefault();

                    var answersCountTotal = this.modelContext
                        .Surveys
                        .Include("Questions")
                        .Include("Questions.Answers")
                        .FirstOrDefault(x => x.Id == surveyCompletionByDemand.SurveyId)
                        .Questions
                        .FirstOrDefault(x => x.Id == surveyCompletionSupplyQuestion.QuestionId)
                        .Answers
                        .Count();

                    foreach (var demandAnswer in demandQuestion.Answers)
                    {
                        var rakingValue = 0;

                        foreach (var supplyAnswer in surveyCompletionSupplyQuestion.Answers)
                        {
                            var result = this.getRankingValue(
                                this.getAnswerValueInScale(answersCountTotal, demandAnswer.AnswerValue),
                                this.getAnswerValueInScale(answersCountTotal, supplyAnswer.AnswerValue));
                            
                            if (result > rakingValue)
                            {
                                rakingValue = result;
                            }
                        }

                        ranking.Rank = ranking.Rank + rakingValue;
                    }
                }

                this.modelContext.Rankings.Add(ranking);
            }

            this.modelContext.SaveChanges();
        }

        public int getRankingValue(int demandAnswer, int supplyAnswer)
        {

            if (supplyAnswer == demandAnswer)
            {
                return MaxValuePerAnswer;
            }
            else if (supplyAnswer < demandAnswer)
            {
                return supplyAnswer;
            }
            else
            {
                return MaxValuePerAnswer - (supplyAnswer - demandAnswer);
            }
        }

        public int getAnswerValueInScale(int possibleAnswersTotal, int answerValue)
        {
            return MaxValuePerAnswer - (possibleAnswersTotal - answerValue);
        }
    }

    
}
