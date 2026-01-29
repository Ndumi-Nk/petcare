namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Groomw : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Spar_Grooming", name: "GroomingStaff_GroomStaffId", newName: "GroomStaffId");
            RenameIndex(table: "dbo.Spar_Grooming", name: "IX_GroomingStaff_GroomStaffId", newName: "IX_GroomStaffId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Spar_Grooming", name: "IX_GroomStaffId", newName: "IX_GroomingStaff_GroomStaffId");
            RenameColumn(table: "dbo.Spar_Grooming", name: "GroomStaffId", newName: "GroomingStaff_GroomStaffId");
        }
    }
}
