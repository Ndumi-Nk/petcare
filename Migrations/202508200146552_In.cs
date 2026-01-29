namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class In : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Drivers", "Email", c => c.String(nullable: false));
            AddColumn("dbo.Drivers", "PasswordHash", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Drivers", "PasswordHash");
            DropColumn("dbo.Drivers", "Email");
        }
    }
}
