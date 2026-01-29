using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;
using PetCare_system.Models;

namespace PetCare_system.Helpers
{
   
    
        public static class ProgressHelper
        {
            // Records a video watch and updates progress
            public static void RecordVideoWatched(this ApplicationDbContext context,
                                                int petId,
                                                string trainingType,
                                                int progressIncrement = 5)
            {
                var progress = context.PetProgresses.FirstOrDefault(p => p.PetId == petId)
                              ?? CreateNewPetProgress(context, petId);

                // Update specific progress based on training type
                switch (trainingType)
                {
                    case "Obedience":
                        progress.ObedienceProgress = Math.Min(100, progress.ObedienceProgress + progressIncrement);
                        break;
                    case "Agility":
                        progress.AgilityProgress = Math.Min(100, progress.AgilityProgress + progressIncrement);
                        break;
                    case "Behavior":
                        progress.BehaviorProgress = Math.Min(100, progress.BehaviorProgress + progressIncrement);
                        break;
                }

                // Update tracking fields
                progress.VideosWatched++;
                progress.TotalTrainingSessions++;
                progress.LastTrainingDate = DateTime.Now;
                progress.ProgressPercentage = progress.CalculatedProgress;

                context.SaveChanges();
            }

            // Records a quiz completion and updates progress
            public static void RecordQuizCompleted(this ApplicationDbContext context,
                                                 int petId,
                                                 string trainingType,
                                                 int score,
                                                 int progressIncrement = 10)
            {
                var progress = context.PetProgresses.FirstOrDefault(p => p.PetId == petId)
                              ?? CreateNewPetProgress(context, petId);

                // Update quiz statistics
                progress.QuizzesCompleted++;
                progress.AverageQuizScore = CalculateNewAverage(progress.AverageQuizScore,
                                                              progress.QuizzesCompleted,
                                                              score);

                // Update progress based on training type
                switch (trainingType)
                {
                    case "Obedience":
                        progress.ObedienceProgress = Math.Min(100, progress.ObedienceProgress + progressIncrement);
                        break;
                    case "Agility":
                        progress.AgilityProgress = Math.Min(100, progress.AgilityProgress + progressIncrement);
                        break;
                    case "Behavior":
                        progress.BehaviorProgress = Math.Min(100, progress.BehaviorProgress + progressIncrement);
                        break;
                }

                // Update tracking fields
                progress.TotalTrainingSessions++;
                progress.LastTrainingDate = DateTime.Now;
                progress.ProgressPercentage = progress.CalculatedProgress;

                context.SaveChanges();
            }

            // Helper method to create new PetProgress
            private static PetProgress CreateNewPetProgress(ApplicationDbContext context, int petId)
            {
                var newProgress = new PetProgress
                {
                    PetId = petId,
                    ObedienceProgress = 0,
                    AgilityProgress = 0,
                    BehaviorProgress = 0,
                    VideosWatched = 0,
                    QuizzesCompleted = 0,
                    AverageQuizScore = 0,
                    TotalTrainingSessions = 0,
                    LastTrainingDate = DateTime.Now
                };
                context.PetProgresses.Add(newProgress);
                return newProgress;
            }

            // Calculates new running average for quiz scores
            private static decimal CalculateNewAverage(decimal currentAverage, int newCount, int newScore)
            {
                return (currentAverage * (newCount - 1) + newScore) / newCount;
            }
        }
    }