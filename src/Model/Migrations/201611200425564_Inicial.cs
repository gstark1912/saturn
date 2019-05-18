namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SupplyAnswer = c.String(),
                        DemandAnswer = c.String(),
                        Value = c.Int(nullable: false),
                        Question_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.AnswerTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompaniesDirectory = c.String(),
                        ConsultantsDirectory = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        parentCategory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.parentCategory_Id)
                .Index(t => t.parentCategory_Id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        CompanyDescription = c.String(),
                        CompanyWebSite = c.String(),
                        CompanyCountry = c.String(),
                        CompanyCity = c.String(),
                        CompanyAddress = c.String(),
                        CompanyPostalCode = c.String(),
                        CompanyBranchOfficesIn = c.String(),
                        CompanyFiscalStartDate = c.String(),
                        CompanyFiscalEndDate = c.String(),
                        CompanyPeopleInCompany = c.Int(nullable: false),
                        CompanyLogo = c.String(),
                        ComercialContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ComercialContactId, cascadeDelete: true)
                .Index(t => t.ComercialContactId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Position = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Name = c.String(),
                        Version = c.String(),
                        Description = c.String(),
                        WebSite = c.String(),
                        ProductContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Contacts", t => t.ProductContactId)
                .Index(t => t.CompanyId)
                .Index(t => t.ProductContactId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        LectorType = c.String(),
                        CompanyType = c.String(),
                        Conutry = c.String(),
                        City = c.String(),
                        Sector = c.String(),
                        Company = c.String(),
                        RoleInCompany = c.String(),
                        DeploymentArea = c.String(),
                        SoftwareInUse = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        SupplyQuestion = c.String(),
                        DemandQuestion = c.String(),
                        SupplyRequired = c.Boolean(nullable: false),
                        DemandRequired = c.Boolean(nullable: false),
                        AnswerType_Id = c.Int(),
                        Survey_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnswerTypes", t => t.AnswerType_Id)
                .ForeignKey("dbo.Surveys", t => t.Survey_Id, cascadeDelete: true)
                .Index(t => t.AnswerType_Id)
                .Index(t => t.Survey_Id);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Rankings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rank = c.Int(nullable: false),
                        SurveyCompletionDemand_Id = c.Int(),
                        SurveyCompletionSupply_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SurveyCompletions", t => t.SurveyCompletionDemand_Id)
                .ForeignKey("dbo.SurveyCompletions", t => t.SurveyCompletionSupply_Id)
                .Index(t => t.SurveyCompletionDemand_Id)
                .Index(t => t.SurveyCompletionSupply_Id);
            
            CreateTable(
                "dbo.SurveyCompletions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SurveyId = c.Int(nullable: false),
                        Category = c.String(),
                        CategoryId = c.Int(nullable: false),
                        Email = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                        Status = c.String(),
                        PartialSave = c.Boolean(nullable: false),
                        PartialSaveKey = c.String(),
                        CompleteReminderSentAt = c.DateTime(),
                        UpdateReminderSentAt = c.DateTime(),
                        Company_Id = c.Int(),
                        Customer_Id = c.Int(),
                        Parent_Id = c.Int(),
                        Product_Id = c.Int(),
                        Role_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.SurveyCompletionParents", t => t.Parent_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.Roles", t => t.Role_Id)
                .Index(t => t.CategoryId)
                .Index(t => t.Company_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.Parent_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.SurveyCompletionParents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                        Status = c.String(),
                        PartialSave = c.Boolean(nullable: false),
                        PartialSaveKey = c.String(),
                        CompleteReminderSentAt = c.DateTime(),
                        UpdateReminderSentAt = c.DateTime(),
                        Company_Id = c.Int(),
                        Customer_Id = c.Int(),
                        Product_Id = c.Int(),
                        Role_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .ForeignKey("dbo.Products", t => t.Product_Id)
                .ForeignKey("dbo.Roles", t => t.Role_Id)
                .Index(t => t.Company_Id)
                .Index(t => t.Customer_Id)
                .Index(t => t.Product_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        UserSelection = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UsersRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.RoleId)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.SurveyCompletionQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        Question = c.String(),
                        SurveyCompletion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SurveyCompletions", t => t.SurveyCompletion_Id)
                .Index(t => t.SurveyCompletion_Id);
            
            CreateTable(
                "dbo.SurveyCompletionAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Answer = c.String(),
                        AnswerValue = c.Int(nullable: false),
                        SurveyCompletionQuestion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SurveyCompletionQuestions", t => t.SurveyCompletionQuestion_Id)
                .Index(t => t.SurveyCompletionQuestion_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Enabled = c.Boolean(),
                        CompanyId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.UsersClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.UsersLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersRoles", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.UsersLogins", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.UsersClaims", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.UsersRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Rankings", "SurveyCompletionSupply_Id", "dbo.SurveyCompletions");
            DropForeignKey("dbo.Rankings", "SurveyCompletionDemand_Id", "dbo.SurveyCompletions");
            DropForeignKey("dbo.SurveyCompletions", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.SurveyCompletionQuestions", "SurveyCompletion_Id", "dbo.SurveyCompletions");
            DropForeignKey("dbo.SurveyCompletionAnswers", "SurveyCompletionQuestion_Id", "dbo.SurveyCompletionQuestions");
            DropForeignKey("dbo.SurveyCompletions", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.SurveyCompletions", "Parent_Id", "dbo.SurveyCompletionParents");
            DropForeignKey("dbo.SurveyCompletionParents", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.SurveyCompletionParents", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.SurveyCompletionParents", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.SurveyCompletionParents", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.SurveyCompletions", "Customer_Id", "dbo.Customers");
            DropForeignKey("dbo.SurveyCompletions", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.SurveyCompletions", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Questions", "Survey_Id", "dbo.Surveys");
            DropForeignKey("dbo.Surveys", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Questions", "AnswerType_Id", "dbo.AnswerTypes");
            DropForeignKey("dbo.Answers", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.Companies", "ComercialContactId", "dbo.Contacts");
            DropForeignKey("dbo.Products", "ProductContactId", "dbo.Contacts");
            DropForeignKey("dbo.Products", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Categories", "parentCategory_Id", "dbo.Categories");
            DropIndex("dbo.UsersLogins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.UsersClaims", new[] { "IdentityUser_Id" });
            DropIndex("dbo.Users", new[] { "CompanyId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.SurveyCompletionAnswers", new[] { "SurveyCompletionQuestion_Id" });
            DropIndex("dbo.SurveyCompletionQuestions", new[] { "SurveyCompletion_Id" });
            DropIndex("dbo.UsersRoles", new[] { "IdentityUser_Id" });
            DropIndex("dbo.UsersRoles", new[] { "RoleId" });
            DropIndex("dbo.Roles", "RoleNameIndex");
            DropIndex("dbo.SurveyCompletionParents", new[] { "Role_Id" });
            DropIndex("dbo.SurveyCompletionParents", new[] { "Product_Id" });
            DropIndex("dbo.SurveyCompletionParents", new[] { "Customer_Id" });
            DropIndex("dbo.SurveyCompletionParents", new[] { "Company_Id" });
            DropIndex("dbo.SurveyCompletions", new[] { "Role_Id" });
            DropIndex("dbo.SurveyCompletions", new[] { "Product_Id" });
            DropIndex("dbo.SurveyCompletions", new[] { "Parent_Id" });
            DropIndex("dbo.SurveyCompletions", new[] { "Customer_Id" });
            DropIndex("dbo.SurveyCompletions", new[] { "Company_Id" });
            DropIndex("dbo.SurveyCompletions", new[] { "CategoryId" });
            DropIndex("dbo.Rankings", new[] { "SurveyCompletionSupply_Id" });
            DropIndex("dbo.Rankings", new[] { "SurveyCompletionDemand_Id" });
            DropIndex("dbo.Surveys", new[] { "Category_Id" });
            DropIndex("dbo.Questions", new[] { "Survey_Id" });
            DropIndex("dbo.Questions", new[] { "AnswerType_Id" });
            DropIndex("dbo.Products", new[] { "ProductContactId" });
            DropIndex("dbo.Products", new[] { "CompanyId" });
            DropIndex("dbo.Companies", new[] { "ComercialContactId" });
            DropIndex("dbo.Categories", new[] { "parentCategory_Id" });
            DropIndex("dbo.Answers", new[] { "Question_Id" });
            DropTable("dbo.UsersLogins");
            DropTable("dbo.UsersClaims");
            DropTable("dbo.Users");
            DropTable("dbo.SurveyCompletionAnswers");
            DropTable("dbo.SurveyCompletionQuestions");
            DropTable("dbo.UsersRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.SurveyCompletionParents");
            DropTable("dbo.SurveyCompletions");
            DropTable("dbo.Rankings");
            DropTable("dbo.Surveys");
            DropTable("dbo.Questions");
            DropTable("dbo.Customers");
            DropTable("dbo.Products");
            DropTable("dbo.Contacts");
            DropTable("dbo.Companies");
            DropTable("dbo.Categories");
            DropTable("dbo.AnswerTypes");
            DropTable("dbo.Answers");
        }
    }
}
