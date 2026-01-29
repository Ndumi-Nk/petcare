namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrainingVideo1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TrainingVideos", "VideoUrl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TrainingVideos", "VideoUrl", c => c.String(nullable: false));
        }
    }
}
