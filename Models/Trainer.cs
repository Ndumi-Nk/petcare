using PetCare_system.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System;

using System.IO;
using System.Linq;
namespace PetCare_system.Models
{

    public class Trainer
    {
        [Key]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100)]
        //[Index(IsUnique = true)]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
        public int YearsOfExperience { get; set; } = 0;

        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        [ValidateFile(ErrorMessage = "Please select a valid image file (JPEG, PNG, GIF) under 5MB")]
        public HttpPostedFileBase ImageFile { get; set; }

        [Display(Name = "Specializations")]
        public string Specializations { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Date Created")]
        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Temporary Password")]
        public string TempPassword { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("TempPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmTempPassword { get; set; }
        [Required]
        public bool IsTempPassword { get; set; } = true;

        // Remove [Required] as we'll set this programmatically
        public string UserId { get; set; }

        public virtual ICollection<TrainingSession> TrainingSessions { get; set; }
    }

    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var file = value as HttpPostedFileBase;
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName)?.ToLower();

            return !string.IsNullOrEmpty(extension) &&
                   allowedExtensions.Contains(extension) &&
                   file.ContentType.StartsWith("image/") &&
                   file.ContentLength < 5 * 1024 * 1024; // 5MB max
        }
    }

}
