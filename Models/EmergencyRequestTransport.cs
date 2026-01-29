using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare_system.Models
{
    public class EmergencyRequestTransport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        // FK to ApplicationUser
        public string UserId { get; set; }

        // FK to Pet
        [Required]
        [ForeignKey("Pet")]
        public int PetId { get; set; }
        public virtual Pet Pet { get; set; }

        // Coordinates (not required in model, JS will validate)
        public string PickupLatitude { get; set; }
        public string PickupLongitude { get; set; }

        [Required]
        public EmergencyType EmergencyTypes { get; set; }
        public enum EmergencyType
        {
            Accident,
            Poisoning,
            BreathingDifficulty,
            Seizure,
            Bleeding,
            Unconsciousness,
            SuddenLameness,
            Burns,
            Choking,
            Heatstroke,
            LaborComplications,
            AllergicReaction,
            VomitingOrDiarrhea,
            EyeInjury,
            BrokenBone,
            Other
        }

        public int? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public virtual Driver Driver { get; set; }

        [Required]
        public string EmergencyDescription { get; set; }

        public DateTime RequestTime { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";
    }
}