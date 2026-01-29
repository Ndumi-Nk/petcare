namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Body = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UserName = c.String(),
                        Post_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Posts", t => t.Post_Id, cascadeDelete: true)
                .Index(t => t.Post_Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Post_Id = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        UserId = c.String(),
                        Title = c.String(nullable: false),
                        Body = c.String(),
                        AttachmentUrl = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Post_Id);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        DriverId = c.Int(nullable: false, identity: true),
                        DriverName = c.String(nullable: false),
                        DriverType = c.String(nullable: false),
                        DriverSurname = c.String(),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        License = c.String(),
                        CarInfo = c.String(nullable: false),
                        Destination = c.String(nullable: false),
                        Driverstatus = c.String(nullable: false),
                        DeliveryStatus = c.String(nullable: false),
                        ProductId = c.String(),
                        Userr_Id = c.String(),
                        Pet_Id = c.String(),
                        RequestId = c.String(),
                    })
                .PrimaryKey(t => t.DriverId);
            
            CreateTable(
                "dbo.EmergencyRequestTransports",
                c => new
                    {
                        RequestId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        PetId = c.Int(nullable: false),
                        PickupLatitude = c.String(),
                        PickupLongitude = c.String(),
                        EmergencyTypes = c.Int(nullable: false),
                        DriverId = c.Int(),
                        EmergencyDescription = c.String(nullable: false),
                        RequestTime = c.DateTime(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.RequestId)
                .ForeignKey("dbo.Drivers", t => t.DriverId)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.DriverId);
            
            CreateTable(
                "dbo.PaymentProds",
                c => new
                    {
                        PaymentProdId = c.Int(nullable: false, identity: true),
                        BillingAddress = c.String(nullable: false, maxLength: 250),
                        CardNumber = c.String(nullable: false),
                        ExpiryDate = c.String(nullable: false),
                        CVV = c.String(nullable: false, maxLength: 4),
                        PaymentMethod = c.String(nullable: false, maxLength: 50),
                        IsDelivery = c.Boolean(nullable: false),
                        DeliveryAddress = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTime(nullable: false),
                        CartItems = c.String(),
                    })
                .PrimaryKey(t => t.PaymentProdId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ImageUrl = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingId = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.RatingId);
            
            CreateTable(
                "dbo.Stylists",
                c => new
                    {
                        StyleId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.StyleId);
            
            CreateTable(
                "dbo.VaccinationRecords",
                c => new
                    {
                        VaccinationId = c.Int(nullable: false, identity: true),
                        VaccineName = c.String(nullable: false),
                        DateGiven = c.DateTime(nullable: false),
                        NextDueDate = c.DateTime(),
                        Notes = c.String(),
                        PetId = c.Int(nullable: false),
                        VaccineType = c.String(),
                    })
                .PrimaryKey(t => t.VaccinationId)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VaccinationRecords", "PetId", "dbo.Pets");
            DropForeignKey("dbo.EmergencyRequestTransports", "PetId", "dbo.Pets");
            DropForeignKey("dbo.EmergencyRequestTransports", "DriverId", "dbo.Drivers");
            DropForeignKey("dbo.Comments", "Post_Id", "dbo.Posts");
            DropIndex("dbo.VaccinationRecords", new[] { "PetId" });
            DropIndex("dbo.EmergencyRequestTransports", new[] { "DriverId" });
            DropIndex("dbo.EmergencyRequestTransports", new[] { "PetId" });
            DropIndex("dbo.Comments", new[] { "Post_Id" });
            DropTable("dbo.VaccinationRecords");
            DropTable("dbo.Stylists");
            DropTable("dbo.Ratings");
            DropTable("dbo.Products");
            DropTable("dbo.PaymentProds");
            DropTable("dbo.EmergencyRequestTransports");
            DropTable("dbo.Drivers");
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
        }
    }
}
