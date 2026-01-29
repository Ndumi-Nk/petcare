namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompletedModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        Score = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.ModuleProgresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Progress = c.Int(nullable: false),
                        VideosWatched = c.Int(nullable: false),
                        QuizAttempts = c.Int(nullable: false),
                        QuizScore = c.Double(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        LastAccessed = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.WatchedVideos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        VideoId = c.Int(nullable: false),
                        WatchedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingVideos", t => t.VideoId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.VideoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WatchedVideos", "VideoId", "dbo.TrainingVideos");
            DropForeignKey("dbo.WatchedVideos", "PetId", "dbo.Pets");
            DropForeignKey("dbo.ModuleProgresses", "PetId", "dbo.Pets");
            DropForeignKey("dbo.ModuleProgresses", "ModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.CompletedModules", "PetId", "dbo.Pets");
            DropForeignKey("dbo.CompletedModules", "ModuleId", "dbo.TrainingModules");
            DropIndex("dbo.WatchedVideos", new[] { "VideoId" });
            DropIndex("dbo.WatchedVideos", new[] { "PetId" });
            DropIndex("dbo.ModuleProgresses", new[] { "ModuleId" });
            DropIndex("dbo.ModuleProgresses", new[] { "PetId" });
            DropIndex("dbo.CompletedModules", new[] { "ModuleId" });
            DropIndex("dbo.CompletedModules", new[] { "PetId" });
            DropTable("dbo.WatchedVideos");
            DropTable("dbo.ModuleProgresses");
            DropTable("dbo.CompletedModules");
        }
    }
}
