namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upd4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Budget", c => c.Int(nullable: false));
            AddColumn("dbo.Customers", "AnnualBilling", c => c.Int(nullable: false));
            AddColumn("dbo.Customers", "PeopleInCompany", c => c.Int(nullable: false));
            AddColumn("dbo.Customers", "BudgetOverBilling", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Customers", "BudgetOverPeople", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Customers", "BillingOverPeople", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Questions", "Old", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "Old");
            DropColumn("dbo.Customers", "BillingOverPeople");
            DropColumn("dbo.Customers", "BudgetOverPeople");
            DropColumn("dbo.Customers", "BudgetOverBilling");
            DropColumn("dbo.Customers", "PeopleInCompany");
            DropColumn("dbo.Customers", "AnnualBilling");
            DropColumn("dbo.Customers", "Budget");
        }
    }
}
