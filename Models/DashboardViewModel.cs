using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class DashboardViewModel
    {
        public List<Pet> Pets { get; set; }
        public List<TrainingSession> UpcomingSessions { get; set; }
        public List<PetProgress> RecentProgress { get; set; }
        public IEnumerable<TrainingModule> RecommendedModules { get; set; }
        public int PetId { get; set; }

        // Helper properties
        public double AverageObedienceProgress =>
            Pets.Any() ? Pets.Average(p => p.Progress?.ObedienceProgress ?? 0) : 0;
        public double AverageAgilityProgress =>
            Pets.Any() ? Pets.Average(p => p.Progress?.AgilityProgress ?? 0) : 0;
        public double AverageBehaviorProgress =>
            Pets.Any() ? Pets.Average(p => p.Progress?.BehaviorProgress ?? 0) : 0;
      
        public int TotalVideosWatched { get; set; }
        public int TotalQuizzesCompleted { get; set; }
        public double AverageModuleProgress { get; set; }
    }
    public class TrainingModulesViewModel
    {
        public List<TrainingModule> Modules { get; set; }
        public List<CompletedModule> CompletedModules { get; set; }
        public List<Pet> Pets { get; set; }
        public TrainingModule Module { get; set; }
        public List<int> WatchedVideoIds { get; set; }
        public int VideosWatched { get; set; }
        public int TotalVideos { get; set; }
        public int Progress { get; set; }
        public bool CanAttemptQuiz { get; set; }
    }

    public class ModuleViewModel
    {
        public TrainingModule Module { get; set; }
        public int PetId { get; set; }
        public bool IsCompleted { get; set; }
        public ModuleProgress Progress { get; set; }
    }
    public class TrainingSessionViewModel
    {
        public TrainingModule Module { get; set; }
        public List<TrainingVideo> Videos { get; set; }
        public List<QuizQuestion> QuizQuestions { get; set; }
        public ModuleProgress Progress { get; set; }
        public int PetId { get; set; }

        // Add this property
       
        public int? PetAge { get; set; }  // Added this property

        public bool AllVideosWatched => Progress?.VideosWatched >= Videos?.Count;
        public bool CanAttemptQuiz => AllVideosWatched && !Progress.IsCompleted;

        // Helper to display age information
        public string AgeDisplay => PetAge.HasValue ? $"{PetAge} years old" : "Age not specified";
    }
}
