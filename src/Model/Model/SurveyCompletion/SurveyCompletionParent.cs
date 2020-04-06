using App.Model.User;
using Model.Model.Customer;
using Model.Suvery;
using System;
using System.Collections.Generic;

namespace Model.SurveyCompletion
{
    public class SurveyCompletionParent
    {
        public SurveyCompletionParent()
        {
            this.SurveyCompletions = new List<SurveyCompletion>();
            this.CreatedAt = DateTime.Now;
        }

        public SurveyCompletionParent( Company company ) : this()
        {
            this.Company = company;
            this.PartialSaveKey = this.Identifier.GetHashCode().ToString( "x" );
        }

        public int Id { get; set; }
        public virtual Category Category { get; set; }
        public virtual Customer.Customer Customer { get; set; }
        public virtual Company Company { get; set; }
        public virtual Product Product { get; set; }
        public virtual ApplicationRole Role { get; set; }
        public virtual string Email { get; set; }
        public virtual ICollection<SurveyCompletion> SurveyCompletions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Status { get; set; }
        public bool PartialSave { get; set; }
        public string PartialSaveKey { get; set; }
        public DateTime? CompleteReminderSentAt { get; set; }
        public DateTime? UpdateReminderSentAt { get; set; }
        public int PartialSaveReminderCount { get; set; }
        public string Source { get; set; }

        public string Identifier
        {
            get
            {
                var customerId =
                    this.Customer != null
                    ? this.Customer.Id.ToString()
                    : this.Company != null
                    ? this.Company.Id.ToString()
                    : "0";

                var surveyId = this.Id.ToString();

                while( customerId.Length < 4 )
                {
                    customerId = "0" + customerId;
                }

                while( surveyId.Length < 4 )
                {
                    surveyId = "0" + surveyId;
                }

                return this.CreatedAt.ToString( "yyyyMMdd" ) + "-" + customerId + "-" + surveyId;
            }
        }
    }
}