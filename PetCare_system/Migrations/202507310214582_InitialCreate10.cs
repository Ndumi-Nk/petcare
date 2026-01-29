namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Trainers", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Trainers", "UserId", c => c.String(nullable: false));
        }
    }
}
