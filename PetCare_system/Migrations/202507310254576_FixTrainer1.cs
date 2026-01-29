namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class FixTrainer1 : DbMigration
    {
        public override void Up()
        {
            // First, update existing NULL values to temporary hashes
            Sql("UPDATE dbo.Trainers SET PasswordHash = '$2a$11$placeholderhash' WHERE PasswordHash IS NULL");

            // Then alter the column to be non-nullable
            AlterColumn("dbo.Trainers", "PasswordHash", c => c.String(nullable: false, maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.Trainers", "PasswordHash", c => c.String());
        }
    }
}
