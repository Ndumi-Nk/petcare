namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Message = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TrainingVideos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrainingModuleId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        VideoUrl = c.String(nullable: false),
                        DurationMinutes = c.Int(nullable: false),
                        DifficultyLevel = c.Int(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.TrainingModuleId, cascadeDelete: true)
                .Index(t => t.TrainingModuleId);
            
            AddColumn("dbo.PetProgresses", "ProgressPercentage", c => c.Int(nullable: false));
            AddColumn("dbo.PetProgresses", "TotalTrainingSessions", c => c.Int(nullable: false));
            AddColumn("dbo.PetProgresses", "Notes", c => c.String());
            AddColumn("dbo.TrainingSessions", "Price", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.TrainingSessions", "PaymentMethod", c => c.String());
            AddColumn("dbo.TrainingSessions", "PaymentStatus", c => c.String());
            AddColumn("dbo.TrainingSessions", "PaymentReference", c => c.String());
            AddColumn("dbo.TrainingModules", "DifficultyLevel", c => c.Int(nullable: false));
            AddColumn("dbo.TrainingModules", "MinimumAge", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingVideos", "TrainingModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TrainingVideos", new[] { "TrainingModuleId" });
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropColumn("dbo.TrainingModules", "MinimumAge");
            DropColumn("dbo.TrainingModules", "DifficultyLevel");
            DropColumn("dbo.TrainingSessions", "PaymentReference");
            DropColumn("dbo.TrainingSessions", "PaymentStatus");
            DropColumn("dbo.TrainingSessions", "PaymentMethod");
            DropColumn("dbo.TrainingSessions", "Price");
            DropColumn("dbo.PetProgresses", "Notes");
            DropColumn("dbo.PetProgresses", "TotalTrainingSessions");
            DropColumn("dbo.PetProgresses", "ProgressPercentage");
            DropTable("dbo.TrainingVideos");
            DropTable("dbo.Notifications");
        }
    }
}
