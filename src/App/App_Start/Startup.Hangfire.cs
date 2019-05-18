using App.Job;
using App.Job.Demanda;
using Hangfire;
using Hangfire.Dashboard;
using Owin;
using System.Collections.Generic;

namespace App
{
    public partial class Startup
    {
        private CompleteRegistrationReminderJob completeRegistrationReminderJob;
        private UpdateRegistrationReminderJob updateRegistrationReminderJob;
        private CompleteEvaluationReminderJob completeEvaluationReminderJob;

        public Startup()
        {
            this.completeRegistrationReminderJob = new CompleteRegistrationReminderJob();
            this.updateRegistrationReminderJob = new UpdateRegistrationReminderJob();
            this.completeEvaluationReminderJob = new CompleteEvaluationReminderJob();
        }

        public void ConfigureHangfire(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("ModelContext");

            var options = new DashboardOptions
            {
                AuthorizationFilters = new List<IAuthorizationFilter>
                {
                    new AuthorizationFilter { Roles = "Admin" }
                }
            };

            app.UseHangfireDashboard("/hangfire", options);
            app.UseHangfireServer();

            this.RegisterJobs();
        }

        private void RegisterJobs()
        {
            RecurringJob.AddOrUpdate(() => this.completeRegistrationReminderJob.Execute(), "0 * * * *");
            RecurringJob.AddOrUpdate(() => this.updateRegistrationReminderJob.Execute(), "0 0 * * *");
            RecurringJob.AddOrUpdate(() => this.completeEvaluationReminderJob.Execute(), "0 0 * * *");
        }
    }
}