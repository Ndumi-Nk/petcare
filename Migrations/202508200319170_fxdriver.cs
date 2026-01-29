namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fxdriver : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Drivers", "Password");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Drivers", "Password", c => c.String(nullable: false));
        }
    }
}
