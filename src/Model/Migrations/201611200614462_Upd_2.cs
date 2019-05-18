namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Upd_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyCompletionParents", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyCompletionParents", "Email");
        }
    }
}
