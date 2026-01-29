using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace PetCare_system.Models
{
    public class Pet_Boarding
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int board_Id { get; set; }

        // Owner information
        [Required]
        public string OwnerName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        // Pet information
        [Required]
        public string PetName { get; set; }

        [Required]
        public string PetType { get; set; }

        public string PetBreed { get; set; }

        [Required]
        public int SelectedPetId { get; set; }

        // User reference
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        // Dates
        [Required, DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;

        // Package information
        [Required]
        public string Package { get; set; }

        [NotMapped]
        public decimal PackagePrice
        {
            get
            {
                if (Package.Contains("Standard")) return 350;
                if (Package.Contains("Deluxe")) return 500;
                if (Package.Contains("Executive")) return 750;
                return 0;
            }
        }

        public string SpecialNeeds { get; set; }

        [Required]
        [Display(Name = "I agree to the terms and conditions")]
        public bool Agreement { get; set; }

        public string Status { get; set; } = "Pending";
        public string Check_Status { get; set; } = "Not Checked In";

        // Navigation property
        public virtual ICollection<PaymentForBoard> Payments { get; set; } = new List<PaymentForBoard>();

        // Helper method to calculate total nights
        [NotMapped]
        public int TotalNights => (CheckOutDate - CheckInDate).Days;

        // Helper method to calculate total price
        [NotMapped]
        public decimal TotalPrice => TotalNights * PackagePrice;
    }
}