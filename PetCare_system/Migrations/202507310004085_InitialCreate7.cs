namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PetProgresses",
                c => new
                    {
                        PetId = c.Int(nullable: false),
                        ObedienceProgress = c.Int(nullable: false),
                        AgilityProgress = c.Int(nullable: false),
                        BehaviorProgress = c.Int(nullable: false),
                        LastTrainingDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PetId)
                .ForeignKey("dbo.Pets", t => t.PetId)
                .Index(t => t.PetId);
            
            CreateTable(
                "dbo.TrainingSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        TrainerId = c.Int(nullable: false),
                        SessionDate = c.DateTime(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        TrainingType = c.String(nullable: false, maxLength: 50),
                        Notes = c.String(maxLength: 1000),
                        Status = c.String(nullable: false, maxLength: 20),
                        Rating = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.Trainers", t => t.TrainerId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.TrainerId);
            
            CreateTable(
                "dbo.QuizOptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionText = c.String(nullable: false, maxLength: 500),
                        IsCorrect = c.Boolean(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizQuestions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(nullable: false, maxLength: 500),
                        Explanation = c.String(maxLength: 1000),
                        TrainingModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.TrainingModuleId, cascadeDelete: true)
                .Index(t => t.TrainingModuleId);
            
            CreateTable(
                "dbo.TrainingModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        VideoUrl = c.String(nullable: false),
                        TrainingType = c.String(nullable: false, maxLength: 50),
                        Difficulty = c.String(nullable: false, maxLength: 20),
                        DurationMinutes = c.Int(nullable: false),
                        SuitableBreeds = c.String(),
                        SuitableAges = c.String(),
                        ThumbnailUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuizResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        CompletedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingModules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.ModuleId);
            
            AddColumn("dbo.Trainers", "UserId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuizResults", "ModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.QuizResults", "PetId", "dbo.Pets");
            DropForeignKey("dbo.QuizQuestions", "TrainingModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.QuizOptions", "QuestionId", "dbo.QuizQuestions");
            DropForeignKey("dbo.TrainingSessions", "TrainerId", "dbo.Trainers");
            DropForeignKey("dbo.TrainingSessions", "PetId", "dbo.Pets");
            DropForeignKey("dbo.PetProgresses", "PetId", "dbo.Pets");
            DropIndex("dbo.QuizResults", new[] { "ModuleId" });
            DropIndex("dbo.QuizResults", new[] { "PetId" });
            DropIndex("dbo.QuizQuestions", new[] { "TrainingModuleId" });
            DropIndex("dbo.QuizOptions", new[] { "QuestionId" });
            DropIndex("dbo.TrainingSessions", new[] { "TrainerId" });
            DropIndex("dbo.TrainingSessions", new[] { "PetId" });
            DropIndex("dbo.PetProgresses", new[] { "PetId" });
            DropColumn("dbo.Trainers", "UserId");
            DropTable("dbo.QuizResults");
            DropTable("dbo.TrainingModules");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.QuizOptions");
            DropTable("dbo.TrainingSessions");
            DropTable("dbo.PetProgresses");
        }
    }
}
