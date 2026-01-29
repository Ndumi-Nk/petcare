namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrainingModule : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TrainingModules", "VideoUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TrainingModules", "VideoUrl", c => c.String(nullable: false));
        }
    }
}
