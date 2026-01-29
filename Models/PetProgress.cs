using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PetCare_system.Models
{
    public class PetProgress
    {
        [Key, ForeignKey("Pet")]
        public int PetId { get; set; }

        [Range(0, 100)]
        [Display(Name = "Obedience Progress")]
        public int ObedienceProgress { get; set; }

        [Range(0, 100)]
        [Display(Name = "Agility Progress")]
        public int AgilityProgress { get; set; }

        [Range(0, 100)]
        [Display(Name = "Behavior Progress")]
        public int BehaviorProgress { get; set; }

        [Display(Name = "Last Training Date")]
        public DateTime? LastTrainingDate { get; set; }

        [Display(Name = "Overall Progress")]
        private int _progressPercentage;

        [Display(Name = "Overall Progress")]
        public int ProgressPercentage
        {
            get => _progressPercentage;
            set => _progressPercentage = value;
        }

        public int CalculatedProgress
        {
            get
            {
                if (TotalTrainingSessions == 0) return 0;
                return (ObedienceProgress + AgilityProgress + BehaviorProgress) / 3;
            }
        }

        [Display(Name = "Total Training Sessions")]
        public int TotalTrainingSessions { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        // New properties for video and quiz tracking
        [Display(Name = "Videos Watched")]
        public int VideosWatched { get; set; }

        [Display(Name = "Quizzes Completed")]
        public int QuizzesCompleted { get; set; }

        [Display(Name = "Average Quiz Score")]
        public decimal AverageQuizScore { get; set; }

        public virtual Pet Pet { get; set; }

        // Methods to update progress (added without changing existing structure)
        public void RecordVideoWatched(string trainingType)
        {
            VideosWatched++;
            LastTrainingDate = DateTime.Now;
            UpdateCategoryProgress(trainingType, 5); // 5% progress per video
        }

        public void RecordQuizCompleted(string trainingType, int score)
        {
            QuizzesCompleted++;
            AverageQuizScore = (AverageQuizScore * (QuizzesCompleted - 1) + score) / QuizzesCompleted;
            LastTrainingDate = DateTime.Now;
            UpdateCategoryProgress(trainingType, 10); // 10% progress per quiz
            UpdateOverallProgress();
        }

        private void UpdateCategoryProgress(string trainingType, int progressIncrease)
        {
            switch (trainingType)
            {
                case "Obedience":
                    ObedienceProgress = Math.Min(100, ObedienceProgress + progressIncrease);
                    break;
                case "Agility":
                    AgilityProgress = Math.Min(100, AgilityProgress + progressIncrease);
                    break;
                case "Behavior":
                    BehaviorProgress = Math.Min(100, BehaviorProgress + progressIncrease);
                    break;
            }
            TotalTrainingSessions++;
        }
        public void UpdateOverallProgress()
        {
            this.ProgressPercentage = this.CalculatedProgress;
            this.TotalTrainingSessions = this.VideosWatched + this.QuizzesCompleted;
        }
       
    }
}