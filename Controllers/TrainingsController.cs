using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class TrainingsController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // Training prices in ZAR
        private readonly Dictionary<string, decimal> _trainingPrices = new Dictionary<string, decimal>
        {
            {"Obedience", 350m},
            {"Agility", 450m},
            {"Behavioral", 500m}
        };

        // GET: Training/Dashboard
        public ActionResult Dashboard(int? petId)
        {
            // Get current user's ID
            var userId = User.Identity.GetUserId();

            // If no petId provided, get the first pet for current user
            if (!petId.HasValue)
            {
                var firstPet = _context.pets
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.Id)
                    .FirstOrDefault();

                if (firstPet != null)
                {
                    petId = firstPet.Id;
                }
                else
                {
                    return RedirectToAction("Register", "Pet"); // Redirect to create a pet if none exist
                }
            }

            // Rest of your existing code with petId.Value
            var pet = _context.pets
                .Include(p => p.Progress)
                .Include(p => p.TrainingSessions)
                .FirstOrDefault(p => p.Id == petId.Value);

            if (pet == null)
            {
                return HttpNotFound();
            }

            var viewModel = new DashboardViewModel
            {
                PetId = petId.Value,
                Pets = _context.pets.Where(p => p.UserId == pet.UserId).ToList(),
                UpcomingSessions = _context.TrainingSessions
                    .Include(s => s.Pet)
                    .Include(s => s.Trainer)
                    .Where(s => s.Pet.UserId == pet.UserId && s.SessionDate > DateTime.Now)
                    .OrderBy(s => s.SessionDate)
                    .Take(5)
                    .ToList(),

                RecentProgress = _context.PetProgresses
                    .Where(p => p.Pet.UserId == pet.UserId)
                    .OrderByDescending(p => p.LastTrainingDate)
                    .Take(3)
                    .ToList(),

                RecommendedModules = _context.TrainingModules
                    .Where(m => m.MinimumAge <= (DateTime.Now.Year - pet.DateOfBirth.Year) * 12 +
                               DateTime.Now.Month - pet.DateOfBirth.Month)
                    .Where(m => string.IsNullOrEmpty(m.SuitableBreeds) ||
                               m.SuitableBreeds.ToLower().Contains(pet.Breed.ToLower()) ||
                               m.SuitableBreeds.ToLower() == "all")
                    .OrderBy(m => m.DifficultyLevel)
                    .Take(3)
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: Training/Book
        public ActionResult Book()
        {
            var userId = User.Identity.GetUserId();
            var pets = _context.pets
                .Where(p => p.UserId == userId)
                .ToList();

            if (!pets.Any())
            {
                TempData["AlertMessage"] = "Please register a pet before booking training";
                return RedirectToAction("Create", "Pets");
            }

            ViewBag.Pets = pets;
            ViewBag.TrainingTypes = new SelectList(_trainingPrices.Keys);
            return View();
        }

        // GET: Training/GetAvailableSlots
        [HttpGet]
        public JsonResult GetAvailableSlots(int petId, string trainingType, string date)
        {
            try
            {
                DateTime sessionDate;
                if (!DateTime.TryParse(date, out sessionDate))
                {
                    return Json(new { success = false, message = "Invalid date format" }, JsonRequestBehavior.AllowGet);
                }

                var pet = _context.pets.Find(petId);
                if (pet == null)
                {
                    return Json(new { success = false, message = "Pet not found" }, JsonRequestBehavior.AllowGet);
                }

                // Calculate age from DateOfBirth
                var age = DateTime.Today.Year - pet.DateOfBirth.Year;
                if (pet.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

                // Check if pet is eligible for this training type
                if (trainingType == "Agility" && age < 1)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Your pet must be at least 1 year old for agility training (Current age: {age} years)"
                    }, JsonRequestBehavior.AllowGet);
                }

                var suitableTrainers = _context.Trainers
                    .Where(t => t.Specializations.Contains(trainingType))
                    .ToList();

                var bookedSlots = _context.TrainingSessions
                    .Where(ts => ts.SessionDate == sessionDate && ts.Status != "Cancelled")
                    .ToList();

                var availableSlots = new List<object>();
                var startTime = new TimeSpan(9, 0, 0); // 9 AM
                var endTime = new TimeSpan(18, 0, 0);  // 6 PM
                var slotDuration = TimeSpan.FromMinutes(60); // 1 hour sessions

                for (var time = startTime; time < endTime; time = time.Add(slotDuration))
                {
                    var slotEnd = time.Add(slotDuration);
                    var isBooked = bookedSlots.Any(bs =>
                        (time >= bs.StartTime && time < bs.EndTime) ||
                        (slotEnd > bs.StartTime && slotEnd <= bs.EndTime) ||
                        (time <= bs.StartTime && slotEnd >= bs.EndTime));

                    if (!isBooked)
                    {
                        var availableTrainer = suitableTrainers
                            .FirstOrDefault(t => !bookedSlots.Any(bs =>
                                bs.TrainerId == t.TrainerId &&
                                ((time >= bs.StartTime && time < bs.EndTime) ||
                                (slotEnd > bs.StartTime && slotEnd <= bs.EndTime) ||
                                (time <= bs.StartTime && slotEnd >= bs.EndTime))));

                        if (availableTrainer != null)
                        {
                            availableSlots.Add(new
                            {
                                time = time.ToString(@"hh\:mm"),
                                trainerId = availableTrainer.TrainerId,
                                trainerName = availableTrainer.FullName,
                                price = _trainingPrices[trainingType],
                                duration = slotDuration.TotalMinutes
                            });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    slots = availableSlots,
                    price = _trainingPrices[trainingType],
                    currency = "ZAR"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookSession(BookingSessionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Json(new
                {
                    success = false,
                    message = "Invalid form data",
                    errors = errors
                });
            }

            var userId = User.Identity.GetUserId();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 1. Validate pet belongs to user
                    var pet = _context.pets
                        .FirstOrDefault(p => p.Id == request.PetId && p.UserId == userId);

                    if (pet == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Pet not found or doesn't belong to you"
                        });
                    }

                    // 2. Validate trainer exists
                    var trainer = _context.Trainers.Find(request.TrainerId);
                    if (trainer == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Trainer not found"
                        });
                    }

                    // 3. Parse and validate date/time
                    if (!DateTime.TryParse(request.SessionDate, out var sessionDate))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Invalid date format"
                        });
                    }

                    if (!TimeSpan.TryParse(request.StartTime, out var startTime))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Invalid time format"
                        });
                    }

                    var endTime = startTime.Add(TimeSpan.FromMinutes(60));

                    // 4. Check slot availability
                    var isSlotAvailable = !_context.TrainingSessions
                        .Any(ts => ts.TrainerId == request.TrainerId &&
                                   ts.SessionDate == sessionDate &&
                                   ts.Status != "Cancelled" &&
                                   ((startTime >= ts.StartTime && startTime < ts.EndTime) ||
                                    (endTime > ts.StartTime && endTime <= ts.EndTime) ||
                                    (startTime <= ts.StartTime && endTime >= ts.EndTime)));

                    if (!isSlotAvailable)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This time slot is no longer available"
                        });
                    }

                    // 5. Create the training session with all required fields
                    var session = new TrainingSession
                    {
                        PetId = request.PetId,
                        TrainerId = request.TrainerId,
                        SessionDate = sessionDate,
                        StartTime = startTime,
                        EndTime = endTime,
                        TrainingType = request.TrainingType,
                        Status = "Scheduled",
                        Price = _trainingPrices[request.TrainingType],
                        PaymentMethod = request.PaymentMethod,
                        PaymentStatus = "Pending", // Initial status
                        CreatedAt = DateTime.Now,
                        Notes = string.Empty // Ensure no null values
                    };

                    // 6. Process payment
                    var paymentResult = ProcessPayment(request.PaymentMethod, request.PaymentData, session.Price);
                    if (!paymentResult.Success)
                    {
                        return Json(new
                        {
                            success = false,
                            message = paymentResult.Message
                        });
                    }

                    // Update payment details
                    session.PaymentStatus = paymentResult.Status;
                    session.PaymentReference = paymentResult.Reference;

                    // 7. Validate all required fields
                    if (session.PetId == 0 || session.TrainerId == 0 ||
                        session.SessionDate == default(DateTime) ||
                        string.IsNullOrEmpty(session.TrainingType))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Required fields are missing"
                        });
                    }

                    // 8. Save to database
                    _context.TrainingSessions.Add(session);
                    _context.SaveChanges();

                    // 9. Update pet progress (in a separate try-catch to not fail booking)
                    try
                    {
                        UpdatePetProgress(request.PetId, $"Scheduled {request.TrainingType} training");
                    }
                    catch (Exception ex)
                    {
                        // Log but don't fail
                        System.Diagnostics.Trace.TraceError($"Error updating progress: {ex.Message}");
                    }

                    // 10. Commit transaction
                    transaction.Commit();

                    return Json(new
                    {
                        success = true,
                        bookingId = session.Id,
                        redirectUrl = Url.Action("Confirmation", new { id = session.Id })
                    });
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");

                    return Json(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = errorMessages
                    });
                }
                catch (DbUpdateException dbEx)
                {
                    transaction.Rollback();
                    var innerException = dbEx.InnerException ?? dbEx;

                    // Check for specific SQL Server errors
                    if (innerException.Message.Contains("FK_"))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Invalid reference (pet or trainer not found)"
                        });
                    }
                    else if (innerException.Message.Contains("Cannot insert the value NULL"))
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Missing required field in database"
                        });
                    }

                    return Json(new
                    {
                        success = false,
                        message = "Database error",
                        error = innerException.Message
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new
                    {
                        success = false,
                        message = "Unexpected error",
                        error = ex.Message
                    });
                }
            }
        }

        public ActionResult Confirmation(int id)
        {
            var userId = User.Identity.GetUserId();
            var session = _context.TrainingSessions
                .Include(ts => ts.Pet)
                .Include(ts => ts.Trainer)
                .FirstOrDefault(ts => ts.Id == id && ts.Pet.UserId == userId);

            if (session == null)
            {
                return HttpNotFound();
            }

            return View(session);
        }

        private PaymentResult ProcessPayment(string method, Dictionary<string, string> paymentData, decimal amount)
        {
            // In a real application, this would integrate with a payment gateway
            // For demo purposes, we'll simulate successful payment

            return new PaymentResult
            {
                Success = true,
                Status = "Completed",
                Reference = "DEMO-" + Guid.NewGuid().ToString().Substring(0, 8),
                Message = "Payment processed successfully"
            };
        }

        private void SendNotification(string userId, string message)
        {
            // In a real application, this would send email/push notification
            // For demo, we'll just log to database

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        private void UpdatePetProgress(int petId, string activity)
        {
            var progress = _context.PetProgresses.FirstOrDefault(p => p.PetId == petId);

            if (progress == null)
            {
                progress = new PetProgress
                {
                    PetId = petId,
                    LastTrainingDate = DateTime.Now,
                    ProgressPercentage = 0,
                    Notes = activity
                };
                _context.PetProgresses.Add(progress);
            }
            else
            {
                progress.LastTrainingDate = DateTime.Now;
                progress.Notes += $"\n{DateTime.Now:dd MMM yyyy}: {activity}";

                // Simple progress calculation - in real app this would be more sophisticated
                if (progress.ProgressPercentage < 100)
                {
                    progress.ProgressPercentage = Math.Min(progress.ProgressPercentage + 5, 100);
                }
            }

            _context.SaveChanges();
        }

        private List<TrainingModule> GetRecommendedModules(string userId)
        {
            var pets = _context.pets
                .Where(p => p.UserId == userId)
                .ToList();

            if (!pets.Any())
            {
                return new List<TrainingModule>();
            }

            // Get the first pet's age
            var pet = pets.First();
            var age = DateTime.Today.Year - pet.DateOfBirth.Year;
            if (pet.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            return _context.TrainingModules
                .Where(m => m.MinimumAge <= age)
                .OrderBy(m => m.DifficultyLevel)
                .Take(3)
                .ToList();
        }

        // GET: Training/Modules
        public ActionResult Modules()
        {
            var userId = User.Identity.GetUserId();
            var modules = _context.TrainingModules
                .Include(m => m.Videos)
                .OrderBy(m => m.DifficultyLevel)
                .ToList();

            var petIds = _context.pets
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToList();

            var completedModules = _context.CompletedModules
                .Where(cm => petIds.Contains(cm.PetId))
                .ToList();

            var viewModel = new TrainingModulesViewModel
            {
                Modules = modules,
                CompletedModules = completedModules,
                Pets = _context.pets.Where(p => p.UserId == userId).ToList()
            };

            return View(viewModel);
        }

        // GET: Training/StartModule/{moduleId}/{petId}
        public ActionResult StartModule(int moduleId, int petId)
        {
            var userId = User.Identity.GetUserId();

            // Verify pet belongs to user
            var pet = _context.pets.FirstOrDefault(p => p.Id == petId && p.UserId == userId);
            if (pet == null)
            {
                return HttpNotFound("Pet not found or doesn't belong to you");
            }

            var module = _context.TrainingModules
                .Include(m => m.Videos)
                .Include(m => m.QuizQuestions.Select(q => q.Options))
                .FirstOrDefault(m => m.Id == moduleId);

            if (module == null)
            {
                return HttpNotFound("Training module not found");
            }

            // Check if already completed
            var isCompleted = _context.CompletedModules
                .Any(cm => cm.PetId == petId && cm.ModuleId == moduleId);

            var viewModel = new ModuleViewModel
            {
                Module = module,
                PetId = petId,
                IsCompleted = isCompleted,
                Progress = _context.ModuleProgresses
                    .FirstOrDefault(mp => mp.PetId == petId && mp.ModuleId == moduleId)
                    ?? new ModuleProgress { Progress = 0 }
            };

            return View(viewModel);
        }

        // GET: Training/WatchVideo/{videoId}/{petId}
        public ActionResult WatchVideo(int videoId, int petId)
        {
            var userId = User.Identity.GetUserId();

            // Verify pet belongs to user
            var pet = _context.pets.FirstOrDefault(p => p.Id == petId && p.UserId == userId);
            if (pet == null)
            {
                return HttpNotFound("Pet not found or doesn't belong to you");
            }

            var video = _context.TrainingVideos
                .Include(v => v.TrainingModule)
                .FirstOrDefault(v => v.Id == videoId);

            if (video == null)
            {
                return HttpNotFound("Video not found");
            }

            // Record video viewing
            var progress = _context.ModuleProgresses
                .FirstOrDefault(mp => mp.PetId == petId && mp.ModuleId == video.TrainingModuleId);

            if (progress == null)
            {
                progress = new ModuleProgress
                {
                    PetId = petId,
                    ModuleId = video.TrainingModuleId,
                    Progress = 0,
                    LastAccessed = DateTime.Now
                };
                _context.ModuleProgresses.Add(progress);
            }
            else
            {
                progress.LastAccessed = DateTime.Now;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log error but continue
                System.Diagnostics.Trace.TraceError($"Error saving progress: {ex.Message}");
            }

            return View(video);
        }

        // POST: Training/VideoWatched
        [HttpPost]
        public JsonResult VideoWatched(int videoId, int petId)
        {
            var userId = User.Identity.GetUserId();

            try
            {
                // Verify pet belongs to user
                var pet = _context.pets.FirstOrDefault(p => p.Id == petId && p.UserId == userId);
                if (pet == null)
                {
                    return Json(new { success = false, message = "Pet not found" });
                }

                var video = _context.TrainingVideos.Find(videoId);
                if (video == null)
                {
                    return Json(new { success = false, message = "Video not found" });
                }

                // Update progress
                var progress = _context.ModuleProgresses
                    .FirstOrDefault(mp => mp.PetId == petId && mp.ModuleId == video.TrainingModuleId);

                if (progress == null)
                {
                    progress = new ModuleProgress
                    {
                        PetId = petId,
                        ModuleId = video.TrainingModuleId,
                        Progress = 10, // Initial progress for first video
                        LastAccessed = DateTime.Now,
                        VideosWatched = 1
                    };
                    _context.ModuleProgresses.Add(progress);
                }
                else
                {
                    // Only increment if this video hasn't been watched before
                    if (!_context.WatchedVideos.Any(wv => wv.VideoId == videoId && wv.PetId == petId))
                    {
                        progress.VideosWatched++;
                        progress.Progress = Math.Min(progress.Progress + 10, 90); // 90% before quiz
                        progress.LastAccessed = DateTime.Now;

                        _context.WatchedVideos.Add(new WatchedVideo
                        {
                            PetId = petId,
                            VideoId = videoId,
                            WatchedDate = DateTime.Now
                        });
                    }
                }

                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    progress = progress.Progress,
                    isReadyForQuiz = progress.Progress >= 90
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Training/GetQuizQuestions/{moduleId}
        [HttpGet]
        public JsonResult GetQuizQuestions(int moduleId)
        {
            try
            {
                var questions = _context.QuizQuestions
                    .Where(q => q.TrainingModuleId == moduleId)
                    .OrderBy(q => q.Id)
                    .Select(q => new
                    {
                        q.Id,
                        q.QuestionText,
                        Options = q.Options.Select(o => new
                        {
                            o.Id,
                            o.OptionText
                        }).ToList(),
                        q.Explanation
                    })
                    .ToList();

                return Json(new { success = true, questions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Training/SubmitQuiz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(SubmitQuizRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var userId = User.Identity.GetUserId();

            try
            {
                // Verify pet belongs to user
                var pet = _context.pets.FirstOrDefault(p => p.Id == request.PetId && p.UserId == userId);
                if (pet == null)
                {
                    return Json(new { success = false, message = "Pet not found" });
                }

                // Get all questions for this module with correct answers
                var questions = _context.QuizQuestions
                    .Include(q => q.Options)
                    .Where(q => q.TrainingModuleId == request.ModuleId)
                    .ToList();

                var results = new List<QuizAnswerResult>();
                int correctAnswers = 0;

                foreach (var answer in request.Answers)
                {
                    var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                    if (question == null) continue;

                    var selectedOption = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                    if (selectedOption != null && correctOption != null)
                    {
                        var isCorrect = selectedOption.Id == correctOption.Id;
                        if (isCorrect) correctAnswers++;

                        results.Add(new QuizAnswerResult
                        {
                            QuestionId = question.Id,
                            QuestionText = question.QuestionText,
                            SelectedOptionId = selectedOption.Id,
                            SelectedOptionText = selectedOption.OptionText,
                            IsCorrect = isCorrect,
                            CorrectOptionId = correctOption.Id,
                            CorrectOptionText = correctOption.OptionText,
                            Explanation = question.Explanation
                        });
                    }
                }

                // Calculate score
                double score = (double)correctAnswers / questions.Count * 100;
                bool passed = score >= 70; // 70% passing score

                // Update module progress
                var progress = _context.ModuleProgresses
                    .FirstOrDefault(mp => mp.PetId == request.PetId && mp.ModuleId == request.ModuleId);

                if (progress == null)
                {
                    progress = new ModuleProgress
                    {
                        PetId = request.PetId,
                        ModuleId = request.ModuleId,
                        Progress = 100,
                        LastAccessed = DateTime.Now,
                        QuizAttempts = 1,
                        QuizScore = score,
                        IsCompleted = passed
                    };
                    _context.ModuleProgresses.Add(progress);
                }
                else
                {
                    progress.Progress = 100;
                    progress.LastAccessed = DateTime.Now;
                    progress.QuizAttempts++;
                    progress.QuizScore = score;
                    progress.IsCompleted = passed;
                }

                // If passed, mark as completed
                if (passed)
                {
                    var existingCompletion = _context.CompletedModules
                        .FirstOrDefault(cm => cm.PetId == request.PetId && cm.ModuleId == request.ModuleId);

                    if (existingCompletion == null)
                    {
                        _context.CompletedModules.Add(new CompletedModule
                        {
                            PetId = request.PetId,
                            ModuleId = request.ModuleId,
                            CompletionDate = DateTime.Now,
                            Score = score
                        });
                    }

                    // Update pet's overall progress
                    UpdatePetProgress(request.PetId, $"Completed module: {_context.TrainingModules.Find(request.ModuleId)?.Title}");
                }

                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    passed,
                    score,
                    results,
                    certificateUrl = passed ? Url.Action("Certificate", new { petId = request.PetId, moduleId = request.ModuleId }) : null
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Training/Certificate/{petId}/{moduleId}
        public ActionResult Certificate(int petId, int moduleId)
        {
            var userId = User.Identity.GetUserId();

            // Verify pet belongs to user
            var pet = _context.pets.FirstOrDefault(p => p.Id == petId && p.UserId == userId);
            if (pet == null)
            {
                return HttpNotFound("Pet not found or doesn't belong to you");
            }

            var completion = _context.CompletedModules
                .Include(cm => cm.Module)
                .FirstOrDefault(cm => cm.PetId == petId && cm.ModuleId == moduleId);

            if (completion == null)
            {
                return HttpNotFound("Module not completed");
            }

            return View(completion);
        }

        public ActionResult TrainerSessions()
        {
            if (Session["TrainerId"] == null)
            {
                return View("NotAssigned"); // or redirect to login
            }

            int trainerId = (int)Session["TrainerId"];

            var allTrainerSessions = _context.TrainingSessions
                .Include(ts => ts.Pet)
                .Include(ts => ts.Trainer)
                .Where(ts => ts.Trainer.TrainerId == trainerId)
                .OrderByDescending(ts => ts.SessionDate)  // Show latest sessions first
                .ThenBy(ts => ts.StartTime)
                .ToList();

            return View(allTrainerSessions);
        }

        public ActionResult CheckIn(int id)
        {
            var session = _context.TrainingSessions
                .Include(ts => ts.Pet)
                .FirstOrDefault(ts => ts.Id == id);

            if (session == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CheckInViewModel
            {
                SessionId = session.Id,
                PetName = session.Pet.Name,
                TrainingType = session.TrainingType
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIn(CheckInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var session = _context.TrainingSessions.Find(model.SessionId);
                if (session == null)
                {
                    return HttpNotFound();
                }

                session.CheckInTime = DateTime.Now;
                session.CheckInNotes = model.Notes;
                session.Status = "In Progress";

                _context.SaveChanges();

                // Update pet progress
                UpdatePetProgress(session.PetId, $"Checked in for {session.TrainingType} training");

                return RedirectToAction("TrainerSessions");
            }

            return View(model);
        }

        public ActionResult CheckOut(int id)
        {
            var session = _context.TrainingSessions
                .Include(ts => ts.Pet)
                .FirstOrDefault(ts => ts.Id == id);

            if (session == null || session.CheckInTime == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CheckOutViewModel
            {
                SessionId = session.Id,
                PetName = session.Pet.Name,
                TrainingType = session.TrainingType
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(CheckOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var session = _context.TrainingSessions.Find(model.SessionId);
                if (session == null || session.CheckInTime == null)
                {
                    return HttpNotFound();
                }

                session.CheckOutTime = DateTime.Now;
                session.CheckOutNotes = model.CheckOutNotes;
                session.Status = "Completed";

                _context.SaveChanges();

                // Update pet progress with training results
                UpdatePetProgress(session.PetId,
                    $"Completed {session.TrainingType} training. Notes: {model.ProgressNotes}");

                return RedirectToAction("TrainerSessions");
            }

            return View(model);
        }

        // GET: TrainerDashboard
        public ActionResult TrainerDashboard()
        {
            int trainerId = (int)Session["TrainerId"];

            var model = new TrainerDashboardViewModel
            {
                TrainerName = _context.Trainers
                                     .Where(t => t.TrainerId == trainerId)
                                     .Select(t => t.FullName)
                                     .FirstOrDefault(),

                TodaySessions = _context.TrainingSessions
                                        .Where(s => s.TrainerId == trainerId &&
                                                    DbFunctions.TruncateTime(s.SessionDate) == DbFunctions.TruncateTime(DateTime.Now))
                                        .Include(s => s.Pet)
                                        .ToList(),

                UpcomingSessions = _context.TrainingSessions
                                           .Where(s => s.TrainerId == trainerId &&
                                                       s.SessionDate > DateTime.Now)
                                           .OrderBy(s => s.SessionDate)
                                           .Include(s => s.Pet)
                                           .ToList(),

                RecentProgressUpdates = _context.PetProgresses
                                                .Include(p => p.Pet)
                                                .OrderByDescending(p => p.LastTrainingDate)
                                                .Take(5) // show last 5 updates
                                                .ToList()
            };

            return View(model);
        }

        // GET: UpdateProgress
        public ActionResult UpdateProgress(int petId)
        {
            var pet = _context.pets.Find(petId); // FIXED
            if (pet == null)
            {
                return HttpNotFound();
            }

            var progress = _context.PetProgresses.FirstOrDefault(p => p.PetId == petId)
                           ?? new PetProgress { PetId = petId };

            var viewModel = new ProgressUpdateViewModel
            {
                PetId = petId,
                PetName = pet.Name,
                ObedienceProgress = progress.ObedienceProgress,
                AgilityProgress = progress.AgilityProgress,
                BehaviorProgress = progress.BehaviorProgress,
                Notes = progress.Notes
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProgress(ProgressUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var progress = _context.PetProgresses.FirstOrDefault(p => p.PetId == model.PetId);

                if (progress == null)
                {
                    // Create new progress record
                    progress = new PetProgress { PetId = model.PetId };
                    _context.PetProgresses.Add(progress);
                }

                // Update properties
                progress.ObedienceProgress = model.ObedienceProgress;
                progress.AgilityProgress = model.AgilityProgress;
                progress.BehaviorProgress = model.BehaviorProgress;
                progress.Notes = model.Notes;
                progress.LastTrainingDate = DateTime.Now;

                // Recalculate percentage + sessions
                progress.UpdateOverallProgress();

                // 🔑 No need to set entity state here, EF already knows!
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Progress updated successfully";
                return RedirectToAction("TrainerDashboard");
            }

            return View(model);
        }

        // GET: PetProgressDetails
        public ActionResult PetProgressDetails(int petId)
        {
            if (Session["TrainerId"] == null)
            {
                return View("NotAssigned");
            }

            var progress = _context.PetProgresses
                .Include(p => p.Pet)
                .FirstOrDefault(p => p.PetId == petId);

            if (progress == null)
            {
                return HttpNotFound();
            }

            // Fetch videos watched by this pet
            var videosWatched = _context.WatchedVideos
                .Include(w => w.Video)
                .Include(w => w.Video.TrainingModule)
                .Where(w => w.PetId == petId)
                .OrderByDescending(w => w.WatchedDate)
                .ToList();

            // Fetch quizzes completed by this pet
            var quizzesCompleted = _context.QuizResults
                .Include(q => q.TrainingModule)
                .Where(q => q.PetId == petId)
                .OrderByDescending(q => q.CompletedAt)
                .ToList();

            // Fetch completed modules
            var completedModules = _context.CompletedModules
                .Include(c => c.Module)
                .Where(c => c.PetId == petId)
                .OrderByDescending(c => c.CompletionDate)
                .ToList();

            // Create a view model to pass all this data to the view
            var viewModel = new PetProgressDetailsViewModel
            {
                Progress = progress,
                VideosWatched = videosWatched,
                QuizzesCompleted = quizzesCompleted,
                CompletedModules = completedModules
            };

            return View(viewModel);
        }

        // POST: Record video watched
        [HttpPost]
        public JsonResult RecordVideoWatched(int videoId, int petId)
        {
            try
            {
                var video = _context.TrainingVideos.Find(videoId);
                if (video == null)
                {
                    return Json(new { success = false, message = "Video not found" });
                }

                // Check if already watched
                var alreadyWatched = _context.WatchedVideos
                    .Any(w => w.PetId == petId && w.VideoId == videoId);

                if (!alreadyWatched)
                {
                    _context.WatchedVideos.Add(new WatchedVideo
                    {
                        PetId = petId,
                        VideoId = videoId,
                        WatchedDate = DateTime.Now
                    });

                    // Update progress
                    var progress = _context.PetProgresses.FirstOrDefault(p => p.PetId == petId);
                    if (progress != null)
                    {
                        progress.VideosWatched++;
                        progress.LastTrainingDate = DateTime.Now;
                    }

                    _context.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Record quiz completed
        [HttpPost]
        public JsonResult RecordQuizCompleted(int petId, int moduleId, int score)
        {
            try
            {
                _context.QuizResults.Add(new QuizResult
                {
                    PetId = petId,
                    ModuleId = moduleId,
                    Score = score,
                    CompletedAt = DateTime.Now
                });

                // Update progress
                var progress = _context.PetProgresses.FirstOrDefault(p => p.PetId == petId);
                if (progress != null)
                {
                    progress.QuizzesCompleted++;
                    progress.AverageQuizScore = ((progress.AverageQuizScore * (progress.QuizzesCompleted - 1)) + score) / progress.QuizzesCompleted;
                    progress.LastTrainingDate = DateTime.Now;
                }

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // View Model for Pet Progress Details
    public class PetProgressDetailsViewModel
    {
        public PetProgress Progress { get; set; }
        public List<WatchedVideo> VideosWatched { get; set; }
        public List<QuizResult> QuizzesCompleted { get; set; }
        public List<CompletedModule> CompletedModules { get; set; }
    }
}