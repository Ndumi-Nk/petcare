namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTrainer2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Trainers", new[] { "Email" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Trainers", "Email", unique: true);
        }
    }
}
