namespace PetCare_system.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adoptions",
                c => new
                    {
                        AdoptionId = c.Int(nullable: false, identity: true),
                        ExperienceLevel = c.String(nullable: false),
                        HomeDescription = c.String(nullable: false, maxLength: 500),
                        AdoptionReason = c.String(nullable: false, maxLength: 1000),
                        Status = c.String(),
                        SubmittedDate = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        AdopterFullName = c.String(),
                        AdopterEmail = c.String(),
                        AdopterPhone = c.String(),
                        Id = c.Int(nullable: false),
                        PetName = c.String(),
                        PetType = c.String(),
                        PetBreed = c.String(),
                        PetDOB = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AdoptionId)
                .ForeignKey("dbo.Pets", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Pets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        Breed = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        PicturePath = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PetProgresses",
                c => new
                    {
                        PetId = c.Int(nullable: false),
                        ObedienceProgress = c.Int(nullable: false),
                        AgilityProgress = c.Int(nullable: false),
                        BehaviorProgress = c.Int(nullable: false),
                        LastTrainingDate = c.DateTime(),
                        ProgressPercentage = c.Int(nullable: false),
                        TotalTrainingSessions = c.Int(nullable: false),
                        Notes = c.String(),
                        VideosWatched = c.Int(nullable: false),
                        QuizzesCompleted = c.Int(nullable: false),
                        AverageQuizScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PetId)
                .ForeignKey("dbo.Pets", t => t.PetId)
                .Index(t => t.PetId);
            
            CreateTable(
                "dbo.TrainingSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        TrainerId = c.Int(nullable: false),
                        SessionDate = c.DateTime(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        TrainingType = c.String(nullable: false, maxLength: 50),
                        Notes = c.String(maxLength: 1000),
                        Status = c.String(nullable: false, maxLength: 20),
                        Rating = c.Int(),
                        CheckInTime = c.DateTime(),
                        CheckOutTime = c.DateTime(),
                        CheckInNotes = c.String(),
                        CheckOutNotes = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        PaymentMethod = c.String(),
                        PaymentStatus = c.String(),
                        PaymentReference = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.Trainers", t => t.TrainerId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.TrainerId);
            
            CreateTable(
                "dbo.Trainers",
                c => new
                    {
                        TrainerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                        PhoneNumber = c.String(),
                        Bio = c.String(),
                        YearsOfExperience = c.Int(nullable: false),
                        ProfilePicture = c.String(),
                        Specializations = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                        TempPassword = c.String(),
                        IsTempPassword = c.Boolean(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.TrainerId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        CellphoneNumber = c.String(nullable: false),
                        IdNumber = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.BookingSpars",
                c => new
                    {
                        BookingSparId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        SpaServiceId = c.Int(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        PetName = c.String(nullable: false),
                        PetType = c.String(nullable: false),
                        SpecialInstructions = c.String(),
                        IsPaid = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BookingSparId)
                .ForeignKey("dbo.SpaServices", t => t.SpaServiceId, cascadeDelete: true)
                .Index(t => t.SpaServiceId);
            
            CreateTable(
                "dbo.SpaServices",
                c => new
                    {
                        SpaServiceId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DurationMinutes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SpaServiceId);
            
            CreateTable(
                "dbo.CompletedModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        Score = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.TrainingModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        TrainingType = c.String(nullable: false, maxLength: 50),
                        Difficulty = c.String(nullable: false, maxLength: 20),
                        DurationMinutes = c.Int(nullable: false),
                        SuitableBreeds = c.String(),
                        SuitableAges = c.String(),
                        ThumbnailUrl = c.String(),
                        DifficultyLevel = c.Int(nullable: false),
                        MinimumAge = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(nullable: false, maxLength: 500),
                        Explanation = c.String(maxLength: 1000),
                        TrainingModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.TrainingModuleId, cascadeDelete: true)
                .Index(t => t.TrainingModuleId);
            
            CreateTable(
                "dbo.QuizOptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionText = c.String(nullable: false, maxLength: 500),
                        IsCorrect = c.Boolean(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizQuestions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.TrainingVideos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TrainingModuleId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        VideoUrl = c.String(),
                        DurationMinutes = c.Int(nullable: false),
                        DifficultyLevel = c.Int(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.TrainingModuleId, cascadeDelete: true)
                .Index(t => t.TrainingModuleId);
            
            CreateTable(
                "dbo.DayCares",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MembershipId = c.Int(nullable: false),
                        PetId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CareType = c.String(nullable: false),
                        CheckInDate = c.DateTime(nullable: false),
                        CheckOutDate = c.DateTime(),
                        SpecialInstructions = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Memberships", t => t.MembershipId, cascadeDelete: true)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.MembershipId)
                .Index(t => t.PetId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Memberships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        PetId = c.Int(nullable: false),
                        MembershipType = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        PaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false),
                        PaymentStatus = c.String(),
                        PaymentDate = c.DateTime(nullable: false),
                        CancellationDate = c.DateTime(),
                        LastRenewalDate = c.DateTime(),
                        AccountNumber = c.String(),
                        CVV = c.String(),
                        ExpiryDate = c.DateTime(),
                        AccountHolder = c.String(),
                        BankType = c.String(),
                        PreviousMembershipId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        Specialization = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        PasswordHash = c.String(),
                        AvailabilityStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GroomingStaffs",
                c => new
                    {
                        GroomStaffId = c.Int(nullable: false, identity: true),
                        Groom_Name = c.String(nullable: false, maxLength: 50),
                        Groom_Surname = c.String(nullable: false, maxLength: 50),
                        Groom_Email = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.GroomStaffId);
            
            CreateTable(
                "dbo.Spar_Grooming",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        OwnerName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                        PetName = c.String(nullable: false, maxLength: 50),
                        PetType = c.Int(nullable: false),
                        Breed = c.String(nullable: false, maxLength: 50),
                        ServiceType = c.Int(nullable: false),
                        AddOnServices = c.Int(nullable: false),
                        DurationHours = c.Double(nullable: false),
                        PreferredDate = c.DateTime(nullable: false),
                        PreferredTime = c.String(nullable: false),
                        SpecialInstructions = c.String(maxLength: 500),
                        BookingDate = c.DateTime(nullable: false),
                        PaymentStatus = c.Int(nullable: false),
                        PaymentDate = c.DateTime(),
                        PaymentMethod = c.Int(),
                        TransactionId = c.String(maxLength: 50),
                        GroomStaffId = c.Int(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.GroomingStaffs", t => t.GroomStaffId)
                .Index(t => t.GroomStaffId);
            
            CreateTable(
                "dbo.Inventories",
                c => new
                    {
                        InventoryId = c.Int(nullable: false, identity: true),
                        MedicationName = c.String(),
                        InventoryType = c.Int(nullable: false),
                        InventoryQuantity = c.Int(nullable: false),
                        MedDiscription = c.String(),
                    })
                .PrimaryKey(t => t.InventoryId);
            
            CreateTable(
                "dbo.ModuleProgresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Progress = c.Int(nullable: false),
                        VideosWatched = c.Int(nullable: false),
                        QuizAttempts = c.Int(nullable: false),
                        QuizScore = c.Double(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        LastAccessed = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TrainingModules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Message = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PaymentForAdoptions",
                c => new
                    {
                        Payment_Id = c.Int(nullable: false, identity: true),
                        AdoptionId = c.Int(nullable: false),
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
                .ForeignKey("dbo.Pet_Adoption", t => t.AdoptionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.AdoptionId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Pet_Adoption",
                c => new
                    {
                        Adoption_Id = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        ExperienceLevel = c.String(nullable: false),
                        HomeDescription = c.String(nullable: false, maxLength: 500),
                        AdoptionReason = c.String(nullable: false, maxLength: 1000),
                        Status = c.String(),
                        HasAgreedToTerms = c.Boolean(nullable: false),
                        ApplicationDate = c.DateTime(nullable: false),
                        PetName = c.String(),
                        PetType = c.String(),
                        PetBreed = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        AdopterFullName = c.String(),
                        AdopterEmail = c.String(),
                        AdopterPhone = c.String(),
                        PickupLocation = c.String(),
                        PickupOrDropoff = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Adoption_Id)
                .ForeignKey("dbo.Pets", t => t.Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.Id)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PaymentForBoards",
                c => new
                    {
                        Payment_boardId = c.Int(nullable: false, identity: true),
                        BoardingId = c.Int(nullable: false),
                        AmountPaid = c.Decimal(nullable: false, storeType: "money"),
                        PaymentDate = c.DateTime(nullable: false),
                        PaymentMethod = c.String(nullable: false, maxLength: 50),
                        BankType = c.String(nullable: false, maxLength: 50),
                        CardHolderName = c.String(maxLength: 100),
                        AccountNumber = c.String(),
                        CVV = c.String(nullable: false, maxLength: 4),
                        ExpiryDate = c.DateTime(),
                        Status = c.String(nullable: false, maxLength: 20),
                        TransactionId = c.String(maxLength: 100),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Payment_boardId)
                .ForeignKey("dbo.Pet_Boarding", t => t.BoardingId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.BoardingId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Pet_Boarding",
                c => new
                    {
                        board_Id = c.Int(nullable: false, identity: true),
                        OwnerName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        PetName = c.String(nullable: false),
                        PetType = c.String(nullable: false),
                        PetBreed = c.String(),
                        SelectedPetId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CheckInDate = c.DateTime(nullable: false),
                        CheckOutDate = c.DateTime(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        Package = c.String(nullable: false),
                        SpecialNeeds = c.String(),
                        Agreement = c.Boolean(nullable: false),
                        Status = c.String(),
                        Check_Status = c.String(),
                    })
                .PrimaryKey(t => t.board_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PaymentForSpars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentDate = c.DateTime(nullable: false),
                        PaymentMethod = c.String(nullable: false),
                        TransactionId = c.String(),
                        Booking_BookingSparId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookingSpars", t => t.Booking_BookingSparId)
                .Index(t => t.Booking_BookingSparId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                        AmountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentDate = c.DateTime(nullable: false),
                        PaymentMethod = c.String(nullable: false, maxLength: 50),
                        AccountNumber = c.String(nullable: false, maxLength: 16),
                        CVV = c.String(maxLength: 3),
                        ExpiryDate = c.DateTime(),
                        AccountHolder = c.String(maxLength: 100),
                        BankType = c.String(nullable: false, maxLength: 50),
                        ConsultId = c.Int(nullable: false),
                        PaymentStatus = c.Boolean(nullable: false),
                        TransactionReference = c.String(),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Vet_Consultations", t => t.ConsultId, cascadeDelete: true)
                .Index(t => t.ConsultId);
            
            CreateTable(
                "dbo.Vet_Consultations",
                c => new
                    {
                        Consult_Id = c.Int(nullable: false, identity: true),
                        Consult_Description = c.String(nullable: false, maxLength: 250),
                        Consult_Date = c.DateTime(nullable: false),
                        Consult_Time = c.DateTime(nullable: false),
                        HasSignsOfIllness = c.Boolean(nullable: false),
                        IsOnMedication = c.Boolean(nullable: false),
                        IsVaccinated = c.Boolean(nullable: false),
                        HasChangedEatingHabits = c.Boolean(nullable: false),
                        HasUnusualBehaviors = c.Boolean(nullable: false),
                        IsDewormed = c.Boolean(nullable: false),
                        InjectionAndVaccination = c.Boolean(nullable: false),
                        Neutering = c.Boolean(nullable: false),
                        GeneralCheckUp = c.Boolean(nullable: false),
                        Deworming = c.Boolean(nullable: false),
                        PregnancyOrUltraSound = c.Boolean(nullable: false),
                        AllergyTest = c.Boolean(nullable: false),
                        PicturePath = c.String(),
                        PetId = c.Int(nullable: false),
                        PetName = c.String(),
                        PetType = c.String(),
                        PetBreed = c.String(),
                        PetDateOfBirth = c.DateTime(nullable: false),
                        PetPicturePath = c.String(),
                        ConsultationType = c.String(nullable: false),
                        Feedback = c.String(),
                        DoctorAvailability = c.Boolean(nullable: false),
                        PaymentStatus = c.Boolean(nullable: false),
                        DoctorId = c.Int(),
                    })
                .PrimaryKey(t => t.Consult_Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.QuizResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        CompletedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingModules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Spar_GroomPayment",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CardNumber = c.String(nullable: false),
                        ExpiryDate = c.String(nullable: false),
                        CVV = c.String(nullable: false),
                        CardName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId);
            
            CreateTable(
                "dbo.Trainings",
                c => new
                    {
                        TrainingId = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        TrainingTypeId = c.Int(nullable: false),
                        TrainingDate = c.DateTime(nullable: false),
                        TrainingDuration = c.Int(nullable: false),
                        TrainingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TrainingLocation = c.String(),
                        TrainingStatus = c.String(nullable: false),
                        ProgressNotes = c.String(),
                        NextSessionDate = c.DateTime(),
                        IsGroupSession = c.Boolean(nullable: false),
                        PetBehaviorBefore = c.String(),
                        PetBehaviorAfter = c.String(),
                    })
                .PrimaryKey(t => t.TrainingId)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingTypes", t => t.TrainingTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.PetId)
                .Index(t => t.UserId)
                .Index(t => t.TrainingTypeId);
            
            CreateTable(
                "dbo.TrainingTypes",
                c => new
                    {
                        TrainingTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        DefaultDuration = c.Int(nullable: false),
                        DefaultCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TrainingTypeId);
            
            CreateTable(
                "dbo.WatchedVideos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PetId = c.Int(nullable: false),
                        VideoId = c.Int(nullable: false),
                        WatchedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pets", t => t.PetId, cascadeDelete: true)
                .ForeignKey("dbo.TrainingVideos", t => t.VideoId, cascadeDelete: true)
                .Index(t => t.PetId)
                .Index(t => t.VideoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WatchedVideos", "VideoId", "dbo.TrainingVideos");
            DropForeignKey("dbo.WatchedVideos", "PetId", "dbo.Pets");
            DropForeignKey("dbo.Trainings", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Trainings", "TrainingTypeId", "dbo.TrainingTypes");
            DropForeignKey("dbo.Trainings", "PetId", "dbo.Pets");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.QuizResults", "ModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.QuizResults", "PetId", "dbo.Pets");
            DropForeignKey("dbo.Payments", "ConsultId", "dbo.Vet_Consultations");
            DropForeignKey("dbo.Vet_Consultations", "PetId", "dbo.Pets");
            DropForeignKey("dbo.Vet_Consultations", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.PaymentForSpars", "Booking_BookingSparId", "dbo.BookingSpars");
            DropForeignKey("dbo.PaymentForBoards", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PaymentForBoards", "BoardingId", "dbo.Pet_Boarding");
            DropForeignKey("dbo.Pet_Boarding", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PaymentForAdoptions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PaymentForAdoptions", "AdoptionId", "dbo.Pet_Adoption");
            DropForeignKey("dbo.Pet_Adoption", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Pet_Adoption", "Id", "dbo.Pets");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ModuleProgresses", "PetId", "dbo.Pets");
            DropForeignKey("dbo.ModuleProgresses", "ModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.Spar_Grooming", "GroomStaffId", "dbo.GroomingStaffs");
            DropForeignKey("dbo.DayCares", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DayCares", "PetId", "dbo.Pets");
            DropForeignKey("dbo.DayCares", "MembershipId", "dbo.Memberships");
            DropForeignKey("dbo.Memberships", "PetId", "dbo.Pets");
            DropForeignKey("dbo.CompletedModules", "PetId", "dbo.Pets");
            DropForeignKey("dbo.CompletedModules", "ModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.TrainingVideos", "TrainingModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.QuizQuestions", "TrainingModuleId", "dbo.TrainingModules");
            DropForeignKey("dbo.QuizOptions", "QuestionId", "dbo.QuizQuestions");
            DropForeignKey("dbo.BookingSpars", "SpaServiceId", "dbo.SpaServices");
            DropForeignKey("dbo.Adoptions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Adoptions", "Id", "dbo.Pets");
            DropForeignKey("dbo.Pets", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TrainingSessions", "TrainerId", "dbo.Trainers");
            DropForeignKey("dbo.TrainingSessions", "PetId", "dbo.Pets");
            DropForeignKey("dbo.PetProgresses", "PetId", "dbo.Pets");
            DropIndex("dbo.WatchedVideos", new[] { "VideoId" });
            DropIndex("dbo.WatchedVideos", new[] { "PetId" });
            DropIndex("dbo.Trainings", new[] { "TrainingTypeId" });
            DropIndex("dbo.Trainings", new[] { "UserId" });
            DropIndex("dbo.Trainings", new[] { "PetId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.QuizResults", new[] { "ModuleId" });
            DropIndex("dbo.QuizResults", new[] { "PetId" });
            DropIndex("dbo.Vet_Consultations", new[] { "DoctorId" });
            DropIndex("dbo.Vet_Consultations", new[] { "PetId" });
            DropIndex("dbo.Payments", new[] { "ConsultId" });
            DropIndex("dbo.PaymentForSpars", new[] { "Booking_BookingSparId" });
            DropIndex("dbo.Pet_Boarding", new[] { "UserId" });
            DropIndex("dbo.PaymentForBoards", new[] { "UserId" });
            DropIndex("dbo.PaymentForBoards", new[] { "BoardingId" });
            DropIndex("dbo.Pet_Adoption", new[] { "UserId" });
            DropIndex("dbo.Pet_Adoption", new[] { "Id" });
            DropIndex("dbo.PaymentForAdoptions", new[] { "UserId" });
            DropIndex("dbo.PaymentForAdoptions", new[] { "AdoptionId" });
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.ModuleProgresses", new[] { "ModuleId" });
            DropIndex("dbo.ModuleProgresses", new[] { "PetId" });
            DropIndex("dbo.Spar_Grooming", new[] { "GroomStaffId" });
            DropIndex("dbo.Memberships", new[] { "PetId" });
            DropIndex("dbo.DayCares", new[] { "UserId" });
            DropIndex("dbo.DayCares", new[] { "PetId" });
            DropIndex("dbo.DayCares", new[] { "MembershipId" });
            DropIndex("dbo.TrainingVideos", new[] { "TrainingModuleId" });
            DropIndex("dbo.QuizOptions", new[] { "QuestionId" });
            DropIndex("dbo.QuizQuestions", new[] { "TrainingModuleId" });
            DropIndex("dbo.CompletedModules", new[] { "ModuleId" });
            DropIndex("dbo.CompletedModules", new[] { "PetId" });
            DropIndex("dbo.BookingSpars", new[] { "SpaServiceId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.TrainingSessions", new[] { "TrainerId" });
            DropIndex("dbo.TrainingSessions", new[] { "PetId" });
            DropIndex("dbo.PetProgresses", new[] { "PetId" });
            DropIndex("dbo.Pets", new[] { "UserId" });
            DropIndex("dbo.Adoptions", new[] { "Id" });
            DropIndex("dbo.Adoptions", new[] { "UserId" });
            DropTable("dbo.WatchedVideos");
            DropTable("dbo.TrainingTypes");
            DropTable("dbo.Trainings");
            DropTable("dbo.Spar_GroomPayment");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.QuizResults");
            DropTable("dbo.Vet_Consultations");
            DropTable("dbo.Payments");
            DropTable("dbo.PaymentForSpars");
            DropTable("dbo.Pet_Boarding");
            DropTable("dbo.PaymentForBoards");
            DropTable("dbo.Pet_Adoption");
            DropTable("dbo.PaymentForAdoptions");
            DropTable("dbo.Notifications");
            DropTable("dbo.ModuleProgresses");
            DropTable("dbo.Inventories");
            DropTable("dbo.Spar_Grooming");
            DropTable("dbo.GroomingStaffs");
            DropTable("dbo.Doctors");
            DropTable("dbo.Memberships");
            DropTable("dbo.DayCares");
            DropTable("dbo.TrainingVideos");
            DropTable("dbo.QuizOptions");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.TrainingModules");
            DropTable("dbo.CompletedModules");
            DropTable("dbo.SpaServices");
            DropTable("dbo.BookingSpars");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Trainers");
            DropTable("dbo.TrainingSessions");
            DropTable("dbo.PetProgresses");
            DropTable("dbo.Pets");
            DropTable("dbo.Adoptions");
        }
    }
}
