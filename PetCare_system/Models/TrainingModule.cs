using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class TrainingModule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

       

        [Required]
        [StringLength(50)]
        [Display(Name = "Training Type")]
        public string TrainingType { get; set; }

        [Required]
        [StringLength(20)]
        public string Difficulty { get; set; }

        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Suitable Breeds")]
        public string SuitableBreeds { get; set; }

        [Display(Name = "Suitable Ages")]
        public string SuitableAges { get; set; }

        [Display(Name = "Thumbnail Image")]
        public string ThumbnailUrl { get; set; }


        [Display(Name = "Difficulty Level")]
        public int DifficultyLevel { get; set; } = 1;

        [Display(Name = "Minimum Age (months)")]
        public int MinimumAge { get; set; } = 0;

        [Display(Name = "Duration (minutes)")]

        public virtual ICollection<TrainingVideo> Videos { get; set; }
        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }

    public class ModuleProgress
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public int ModuleId { get; set; }
        public int Progress { get; set; } // Percentage
        public int VideosWatched { get; set; }
        public int QuizAttempts { get; set; }
        public double QuizScore { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastAccessed { get; set; }

        [ForeignKey("PetId")]
        public virtual Pet Pet { get; set; }

        [ForeignKey("ModuleId")]
        public virtual TrainingModule Module { get; set; }
    }

    public class WatchedVideo
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public int VideoId { get; set; }
        public DateTime WatchedDate { get; set; }

        [ForeignKey("PetId")]
        public virtual Pet Pet { get; set; }

        [ForeignKey("VideoId")]
        public virtual TrainingVideo Video { get; set; }
    }

    public class CompletedModule
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public int ModuleId { get; set; }
        public DateTime CompletionDate { get; set; }
        public double Score { get; set; }

        [ForeignKey("PetId")]
        public virtual Pet Pet { get; set; }

        [ForeignKey("ModuleId")]
        public virtual TrainingModule Module { get; set; }
    }
}