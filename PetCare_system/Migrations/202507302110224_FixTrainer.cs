namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrainer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trainers", "PasswordHash", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trainers", "PasswordHash");
        }
    }
}
