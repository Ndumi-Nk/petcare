namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PetProgresses", "VideosWatched", c => c.Int(nullable: false));
            AddColumn("dbo.PetProgresses", "QuizzesCompleted", c => c.Int(nullable: false));
            AddColumn("dbo.PetProgresses", "AverageQuizScore", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PetProgresses", "AverageQuizScore");
            DropColumn("dbo.PetProgresses", "QuizzesCompleted");
            DropColumn("dbo.PetProgresses", "VideosWatched");
        }
    }
}
