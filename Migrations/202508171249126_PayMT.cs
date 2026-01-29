namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayMT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentForTrainings",
                c => new
                    {
                        Payment_Id = c.Int(nullable: false, identity: true),
                        TrainingSessionId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PetName = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        PaymentMethod = c.String(nullable: false, maxLength: 50),
                        BankType = c.String(nullable: false, maxLength: 50),
                        CardHolderName = c.String(maxLength: 100),
                        AccountNumber = c.String(nullable: false, maxLength: 16),
                        CVV = c.String(maxLength: 3),
                        ExpiryDate = c.DateTime(),
                        Status = c.String(nullable: false, maxLength: 20),
                        TransactionId = c.String(maxLength: 100),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Payment_Id)
                .ForeignKey("dbo.TrainingSessions", t => t.TrainingSessionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TrainingSessionId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentForTrainings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PaymentForTrainings", "TrainingSessionId", "dbo.TrainingSessions");
            DropIndex("dbo.PaymentForTrainings", new[] { "UserId" });
            DropIndex("dbo.PaymentForTrainings", new[] { "TrainingSessionId" });
            DropTable("dbo.PaymentForTrainings");
        }
    }
}
