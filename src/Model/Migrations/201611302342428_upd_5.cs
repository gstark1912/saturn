namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upd_5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Products", "User_Id");
            AddForeignKey("dbo.Products", "User_Id", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "User_Id", "dbo.Users");
            DropIndex("dbo.Products", new[] { "User_Id" });
            DropColumn("dbo.Products", "User_Id");
        }
    }
}
