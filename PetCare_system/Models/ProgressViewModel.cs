using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class ProgressViewModel
    {

        public Pet Pet { get; set; }
        public List<Pet> Pets { get; set; }
        public List<TrainingSession> TrainingSessions { get; set; }
    }

    public class VideoUploadViewModel
    {
        [Required]
        [Display(Name = "Training Module")]
        public int TrainingModuleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Video File")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase VideoFile { get; set; }

        [Required]
        [Display(Name = "Duration (minutes)")]
        [Range(1, 120)]
        public int DurationMinutes { get; set; }

        [Display(Name = "Difficulty Level")]
        [Range(1, 5)]
        public int DifficultyLevel { get; set; } = 1;
    }

    public class VideoEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Training Module")]
        public int TrainingModuleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Duration (minutes)")]
        [Range(1, 120)]
        public int DurationMinutes { get; set; }

        [Display(Name = "Difficulty Level")]
        [Range(1, 5)]
        public int DifficultyLevel { get; set; } = 1;
    }
    public class CheckInViewModel
    {
        public int SessionId { get; set; }
        public string PetName { get; set; }
        public string TrainingType { get; set; }
        [Required(ErrorMessage = "Please enter initial observations.")]
        public string Notes { get; set; }
        // Optional for video streaming room
        public string RoomId { get; set; }
    }


    public class CheckOutViewModel
    {
        public int SessionId { get; set; }
        public string PetName { get; set; }
        public string TrainingType { get; set; }
        public string ProgressNotes { get; set; }
        public string CheckOutNotes { get; set; }
    }
    public class TrainerDashboardViewModel
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; }
        public List<TrainingSession> TodaySessions { get; set; }
        public List<TrainingSession> UpcomingSessions { get; set; }
        public List<PetProgress> RecentProgressUpdates { get; set; }
    }
    public class ProgressUpdateViewModel
    {
        public int PetId { get; set; }
        public string PetName { get; set; }

        [Range(0, 100)]
        public int ObedienceProgress { get; set; }

        [Range(0, 100)]
        public int AgilityProgress { get; set; }

        [Range(0, 100)]
        public int BehaviorProgress { get; set; }

        public string Notes { get; set; }
    }
    public class TrainerCallViewModel
    {
        public int TrainerId { get; set; }
        public List<TrainerCallOwnerViewModel> AvailableOwners { get; set; }
    }

    public class TrainerCallOwnerViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string PetName { get; set; }
        public int SessionId { get; set; }
    }

    public class PetOwnerViewModel
    {
        public string SessionId { get; set; }
        public string PetName { get; set; }
        public string TrainerName { get; set; }
    }
}

