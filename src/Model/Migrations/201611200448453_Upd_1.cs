namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upd_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyCompletionParents", "Category_Id", c => c.Int());
            CreateIndex("dbo.SurveyCompletionParents", "Category_Id");
            AddForeignKey("dbo.SurveyCompletionParents", "Category_Id", "dbo.Categories", "Id");
            DropColumn("dbo.SurveyCompletions", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyCompletions", "Status", c => c.String());
            DropForeignKey("dbo.SurveyCompletionParents", "Category_Id", "dbo.Categories");
            DropIndex("dbo.SurveyCompletionParents", new[] { "Category_Id" });
            DropColumn("dbo.SurveyCompletionParents", "Category_Id");
        }
    }
}
