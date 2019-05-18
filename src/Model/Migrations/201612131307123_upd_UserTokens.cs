namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upd_UserTokens : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "confirmToken", c => c.String());
            AddColumn("dbo.Users", "resetPassToken", c => c.String());
            CreateIndex("dbo.SurveyCompletionQuestions", "QuestionId");
            AddForeignKey("dbo.SurveyCompletionQuestions", "QuestionId", "dbo.Questions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyCompletionQuestions", "QuestionId", "dbo.Questions");
            DropIndex("dbo.SurveyCompletionQuestions", new[] { "QuestionId" });
            DropColumn("dbo.Users", "resetPassToken");
            DropColumn("dbo.Users", "confirmToken");
        }
    }
}
