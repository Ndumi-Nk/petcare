using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class AllBookingsViewModel
    {
        public List<Vet_Consultations> VetConsultations { get; set; }
        public List<Pet_Adoption> PetAdoptions { get; set; }
        public List<Pet_Boarding> PetBoardings { get; set; }
    }

    public class BookingReportModel
    {
        public int BookingId { get; set; }
        public string PetName { get; set; }
        public string PetType { get; set; }
        public string Breed { get; set; }
        public string OwnerName { get; set; }
        public string ServiceType { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SpecialInstructions { get; set; }
    }
    // Models/VideoCallViewModel.cs
    public class VideoCallViewModel
    {
        public int SessionId { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; } // "trainer" or "user"
        public string OtherParticipantName { get; set; }
        public DateTime SessionStartTime { get; set; }
        public bool IsTrainer { get; set; }
        public TrainingSession Session { get; set; }
    }
}