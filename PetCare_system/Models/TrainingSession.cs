using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class TrainingSession
    {
      
        
            [Key]
            public int Id { get; set; }

            [Required]
            public int PetId { get; set; }

            [Required]
            public int TrainerId { get; set; }

            [Required]
            [Display(Name = "Session Date")]
            [DataType(DataType.Date)]
            public DateTime SessionDate { get; set; }

            [Required]
            [Display(Name = "Start Time")]
            [DataType(DataType.Time)]
            public TimeSpan StartTime { get; set; }

            [Required]
            [Display(Name = "End Time")]
            [DataType(DataType.Time)]
            public TimeSpan EndTime { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "Training Type")]
            public string TrainingType { get; set; }

            [StringLength(1000)]
            public string Notes { get; set; }

            [Required]
            [StringLength(20)]
            public string Status { get; set; } = "Scheduled";

            [Range(1, 5)]
            [Display(Name = "Session Rating")]
            public int? Rating { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string CheckInNotes { get; set; }
        public string CheckOutNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "Price")]
        [Column(TypeName = "money")] // Changed from decimal(18,2) to money
        public decimal Price { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }

        [Display(Name = "Created At")]
       
        public virtual Pet Pet { get; set; }
            public virtual Trainer Trainer { get; set; }
        }
    }
