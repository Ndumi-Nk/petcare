using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class Notification
    {
     
            [Key]
            public int Id { get; set; }

            [Required]
            [ForeignKey("User")]
            public string UserId { get; set; }

            [Required]
            [Display(Name = "Message")]
            public string Message { get; set; }

            [Display(Name = "Created At")]
            public DateTime CreatedAt { get; set; } = DateTime.Now;

            [Display(Name = "Is Read")]
            public bool IsRead { get; set; } = false;

            // Navigation Property
            public virtual ApplicationUser User { get; set; }
        }
    }
