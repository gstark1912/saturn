using Model.Context;
using Model.Ranking;
using Model.SurveyCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Task GenerarRanking(int surveyCompletionId)
        {
            var parentSurveyCompletionByDemanda = this.modelContext
                .SurveyCompletionParent
                .Include("SurveyCompletions.Questions.QuestionObj.AnswerType")
                .Include("SurveyCompletions.Questions.QuestionObj.Answers")
                .Include("SurveyCompletions.Questions.Answers")
                .Include("Category")
                .Single(x => x.Id == surveyCompletionId);

            var parentSurveysCompletionsByOferta = this.modelContext
                .SurveyCompletionParent
                .Include("SurveyCompletions.Questions")
                .Include("SurveyCompletions.Questions.Answers")
                .Where(x =>
                    //x.SurveyId == surveyCompletionByDemand.SurveyId &&
                    x.Role.Name == "Oferta"
                    && x.Category.Id == parentSurveyCompletionByDemanda.Category.Id
                    && x.Status == "Aprobado"
                    && x.DeletedAt == null
                    && x.Company != null
                    && x.Product != null)
                .ToList();

            var rankings = new List<Ranking>();

            foreach (var parentSurveyCompletionByOferta in parentSurveysCompletionsByOferta)
            {
                var ranking = new Ranking()
                {
                    SurveyCompletionParentByDemanda = parentSurveyCompletionByDemanda,
                    SurveyCompletionParentByOferta = parentSurveyCompletionByOferta,
                    Rank = 0
                };

                foreach (var surveyCompletionByDemanda in parentSurveyCompletionByDemanda.SurveyCompletions)
                {
                    foreach (var demandaQuestion in surveyCompletionByDemanda.Questions)
                    {
                        /*SurveyCompletionQuestion supplyQuestion = null;
                        int answersCountTotal = 0;
                        foreach (var surveyCompletionBySupplySingle in surveyCompletionBySupply.SurveyCompletions)
                        {
                            supplyQuestion = surveyCompletionByDemandSingle
                                .Questions
                                .Where(x => x.QuestionId == surveyCompletionDemandQuestion.QuestionId)
                                .FirstOrDefault();

                            answersCountTotal = this.modelContext
                                .Surveys
                                .Include("Questions")
                                .Include("Questions.Answers")
                                .FirstOrDefault(x => x.Id == surveyCompletionByDemandSingle.SurveyId)
                                .Questions
                                .FirstOrDefault(x => x.Id == surveyCompletionDemandQuestion.QuestionId)
                                .Answers
                                .Count();
                        }*/

                        var surveyCompletion = parentSurveyCompletionByOferta
                            .SurveyCompletions
                            .Where(s =>
                                s.Questions.Where(q =>
                                    q.QuestionId == demandaQuestion.QuestionId
                                ).Count() == 1
                            )
                            .FirstOrDefault();

                        if (surveyCompletion != null)
                        {
                            SurveyCompletionQuestion supplyQuestion = surveyCompletion.Questions.FirstOrDefault(q => q.QuestionObj.Old == false);

                            int answersCountTotal = demandaQuestion.Answers.Count();
                            int possibleAnswersCountTotal = demandaQuestion.QuestionObj.Answers.Count();
                            int possibleAnswersSumTotal = demandaQuestion.QuestionObj.Answers.Sum(x => x.Value);

                            if (supplyQuestion != null)
                            {
                                var answerTypeName = demandaQuestion.QuestionObj.AnswerType.Name;

                                if (answerTypeName != "Multiple")
                                {
                                    foreach (var supplyAnswer in supplyQuestion.Answers)
                                    {
                                        var rakingValue = 0;
                                        var demandAnswer = demandaQuestion.Answers.FirstOrDefault();
                                        int result = 0;
                                        if (demandAnswer != null)
                                        {
                                            if (possibleAnswersCountTotal == possibleAnswersSumTotal)
                                            {
                                                //Comparando respuestas excluyentes entre si
                                                result = demandAnswer.Answer == supplyAnswer.Answer ? 10 : 0;

                                            }
                                            else
                                            {
                                                //Comparando respuestas que incluyen la funcionalidad de la anterior

                                                result = this.getRankingValue(
                                                    this.getAnswerValueInScale(answersCountTotal, demandAnswer.AnswerValue),
                                                    this.getAnswerValueInScale(answersCountTotal, supplyAnswer.AnswerValue));
                                            }
                                        }
                                        rakingValue = result;
                                        ranking.Rank = ranking.Rank + rakingValue;
                                    }
                                }
                                else
                                {
                                    if (answersCountTotal > 0)
                                    {
                                        int supportedAnswers = supplyQuestion.Answers.Select(x => x.Answer).Intersect(demandaQuestion.Answers.Select(x => x.Answer)).Count();
                                        ranking.Rank += (MaxValuePerAnswer * supportedAnswers) / answersCountTotal;
                                    }
                                }
                            }
                        }
                    }
                }

                rankings.Add(ranking);
            }

            this.modelContext.Rankings.AddRange(rankings);
            this.modelContext.SaveChanges();

            return Task.FromResult(true);
        }

        public int getRankingValue(int demandAnswer, int supplyAnswer)
        {

            if (supplyAnswer < demandAnswer)
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
