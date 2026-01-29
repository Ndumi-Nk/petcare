using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare_system.Models
{
    public class TrainingVideo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("TrainingModule")]
        public int TrainingModuleId { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

      
        [Display(Name = "Video URL")]
     
        public string VideoUrl { get; set; }

        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Difficulty Level")]
        public int DifficultyLevel { get; set; } = 1;

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Navigation Property
        public virtual TrainingModule TrainingModule { get; set; }
    }
}