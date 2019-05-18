using App.Model.User;
using App.Models.Survey;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model.Context;
using Model.SurveyCompletion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Service.Demanda
{
    public class EvaluationService
    {
        public ModelContext modelContext;
        private RoleManager<ApplicationRole> roleManager;

        public EvaluationService()
        {
            this.modelContext = new ModelContext();

            var roleStore = new RoleStore<ApplicationRole>(this.modelContext);
            this.roleManager = new RoleManager<ApplicationRole>(roleStore);
        }

        //User.Identity.GetUserId();
        public SurveyCompletionParent Save(SurveyViewModel model, bool partial, string userId)
        {
            var category = this.modelContext
                .Categories
                .Where(x => x.Id == model.CategoryId)
                .FirstOrDefault();

            var role = this.roleManager.FindByName("DEMANDA");

            var surveyCompletionParent = new SurveyCompletionParent
            {
                Product = null,
                Role = role,
                Status = "Aprobado",
                PartialSave = partial,
                Category = category,
                Email = model.Email,
                PartialSaveKey = Guid.NewGuid().ToString()
            };

            foreach (var surveyDTO in model.SurveyDTOs)
            {
                var categoryObj = this.modelContext
                    .Categories
                    .Where(x => x.Id == surveyDTO.CategoryId)
                    .FirstOrDefault();

                var surveyCompletion = new SurveyCompletion
                {
                    SurveyId = surveyDTO.SurveyId,
                    CategoryId = surveyDTO.CategoryId,
                    Category = surveyDTO.CategoryName,
                    CategoryObj = categoryObj,
                    Email = model.Email,
                    Role = role,
                    Parent = surveyCompletionParent,
                    PartialSave = partial,
                };

                var surveyQuestions = this.modelContext
                        .Questions
                        .Include("Answers")
                        .Where(x => x.Survey.Id == surveyDTO.SurveyId)
                        .ToList();

                foreach (var question in model.surveyCompletionDTOs)
                {
                    var answers = new List<SurveyCompletionAnswer>();
                    if (question.SurveyId == surveyDTO.SurveyId)
                    {
                        if (question.Answers != null)
                        {
                            answers = surveyQuestions
                                .Where(x => x.Id == question.QuestionId)
                                .FirstOrDefault()
                                .Answers
                                .Where(x => question.Answers.Contains(x.Id))
                                .Select(x => new SurveyCompletionAnswer
                                {
                                    Answer = x.DemandAnswer,
                                    AnswerValue = x.Value
                                })
                                .ToList();
                        }

                        ApplicationUser currentUser = this.modelContext.Users.FirstOrDefault(x => x.Id == userId);

                        var surveyCompletionQuestion = new SurveyCompletionQuestion
                        {
                            Question = question.Question,
                            QuestionId = question.QuestionId,
                            Answers = answers
                        };

                        surveyCompletion.Questions.Add(surveyCompletionQuestion);
                    }
                }

                this.modelContext.SurveysCompletion.Add(surveyCompletion);
            }

            this.modelContext.SurveyCompletionParent.Add(surveyCompletionParent);
            this.modelContext.SaveChanges();

            return surveyCompletionParent;
        }
    }
}