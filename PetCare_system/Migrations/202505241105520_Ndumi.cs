namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ndumi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pet_Adoption", "PickupLocation", c => c.String());
            AddColumn("dbo.Pet_Adoption", "PickupOrDropoff", c => c.String());
            AddColumn("dbo.Pet_Boarding", "SelectedPetId", c => c.Int(nullable: false));
            AddColumn("dbo.Pet_Boarding", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.PaymentForBoards", "AccountNumber", c => c.String());
            AlterColumn("dbo.PaymentForBoards", "CVV", c => c.String(nullable: false, maxLength: 4));
            CreateIndex("dbo.Pet_Boarding", "UserId");
            AddForeignKey("dbo.Pet_Boarding", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pet_Boarding", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Pet_Boarding", new[] { "UserId" });
            AlterColumn("dbo.PaymentForBoards", "CVV", c => c.String(maxLength: 3));
            AlterColumn("dbo.PaymentForBoards", "AccountNumber", c => c.String(nullable: false, maxLength: 16));
            DropColumn("dbo.Pet_Boarding", "UserId");
            DropColumn("dbo.Pet_Boarding", "SelectedPetId");
            DropColumn("dbo.Pet_Adoption", "PickupOrDropoff");
            DropColumn("dbo.Pet_Adoption", "PickupLocation");
        }
    }
}
