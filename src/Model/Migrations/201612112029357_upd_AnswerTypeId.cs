namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upd_AnswerTypeId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Questions", "AnswerType_Id", "dbo.AnswerTypes");
            DropIndex("dbo.Questions", new[] { "AnswerType_Id" });
            RenameColumn(table: "dbo.Questions", name: "AnswerType_Id", newName: "AnswerTypeId");
            AlterColumn("dbo.Questions", "AnswerTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Questions", "AnswerTypeId");
            AddForeignKey("dbo.Questions", "AnswerTypeId", "dbo.AnswerTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "AnswerTypeId", "dbo.AnswerTypes");
            DropIndex("dbo.Questions", new[] { "AnswerTypeId" });
            AlterColumn("dbo.Questions", "AnswerTypeId", c => c.Int());
            RenameColumn(table: "dbo.Questions", name: "AnswerTypeId", newName: "AnswerType_Id");
            CreateIndex("dbo.Questions", "AnswerType_Id");
            AddForeignKey("dbo.Questions", "AnswerType_Id", "dbo.AnswerTypes", "Id");
        }
    }
}
