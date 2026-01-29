namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrainingSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingSessions", "CheckInTime", c => c.DateTime());
            AddColumn("dbo.TrainingSessions", "CheckOutTime", c => c.DateTime());
            AddColumn("dbo.TrainingSessions", "CheckInNotes", c => c.String());
            AddColumn("dbo.TrainingSessions", "CheckOutNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingSessions", "CheckOutNotes");
            DropColumn("dbo.TrainingSessions", "CheckInNotes");
            DropColumn("dbo.TrainingSessions", "CheckOutTime");
            DropColumn("dbo.TrainingSessions", "CheckInTime");
        }
    }
}
