namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trainers", "TempPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trainers", "TempPassword");
        }
    }
}
