namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upd_3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "CompanyDomain", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "CompanyDomain");
        }
    }
}
