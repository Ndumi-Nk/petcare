using Microsoft.Extensions.Logging;
using PetCare_system.Helpers;
using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class TrainingContentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TrainingContentController> _logger;
        public TrainingContentController()
        {
            _context = new ApplicationDbContext();
          
        }

        #region Module Management
        // GET: TrainingContent/Modules
        public ActionResult Modules()
        {
            var modules = _context.TrainingModules
                .Include(m => m.Videos)
                .Include(m => m.QuizQuestions)
                .OrderBy(m => m.DifficultyLevel)
                .ToList();

            return View(modules);
        }

        // GET: TrainingContent/CreateModule
        public ActionResult CreateModule()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateModule(TrainingModule model, HttpPostedFileBase imageFile)
        {
            try
            {
                // Set basic properties
             
                // Handle image upload to database
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Validate file size (adjust as needed)
                    if (imageFile.ContentLength > 2 * 1024 * 1024) // 2MB limit
                    {
                        ModelState.AddModelError("imageFile", "Image must be less than 2MB");
                        return View(model);
                    }

                    // Convert image to base64 string
                    using (var binaryReader = new BinaryReader(imageFile.InputStream))
                    {
                        byte[] imageData = binaryReader.ReadBytes(imageFile.ContentLength);
                        model.ThumbnailUrl = "data:" + imageFile.ContentType + ";base64," +
                                            Convert.ToBase64String(imageData);
                    }
                }

                // Validate model
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                _context.TrainingModules.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Module created successfully!";
                return RedirectToAction("Modules");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.ToString());
                TempData["ErrorMessage"] = "Error saving module: " + ex.Message;
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditModule(TrainingModule model, HttpPostedFileBase thumbnailFile)
        {
            if (ModelState.IsValid)
            {
                var module = _context.TrainingModules.Find(model.Id);
                if (module == null)
                {
                    return HttpNotFound();
                }

                // Handle thumbnail update
                if (thumbnailFile != null && thumbnailFile.ContentLength > 0)
                {
                    var uploadsDir = Path.Combine(Server.MapPath("~/uploads/module-thumbnails"));
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Delete old thumbnail if exists
                    if (!string.IsNullOrEmpty(module.ThumbnailUrl))
                    {
                        var oldFilePath = Path.Combine(Server.MapPath("~"), module.ThumbnailUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(thumbnailFile.FileName);
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);
                    thumbnailFile.SaveAs(filePath);

                    module.ThumbnailUrl = "/uploads/module-thumbnails/" + uniqueFileName;
                }

                module.Title = model.Title;
                module.Description = model.Description;
                
                module.TrainingType = model.TrainingType;
                module.Difficulty = model.Difficulty;
                module.DurationMinutes = model.DurationMinutes;
                module.SuitableBreeds = model.SuitableBreeds;
                module.SuitableAges = model.SuitableAges;
                module.DifficultyLevel = model.DifficultyLevel;
                module.MinimumAge = model.MinimumAge;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Module updated successfully!";
                return RedirectToAction("Modules");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteModule(int id)
        {
            var module = _context.TrainingModules
                .Include(m => m.Videos)
                .Include(m => m.QuizQuestions)
                .FirstOrDefault(m => m.Id == id);

            if (module == null)
            {
                return HttpNotFound();
            }

            if (module.Videos.Any() || module.QuizQuestions.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete module because it contains videos or quiz questions.";
                return RedirectToAction("Modules");
            }

            // Delete thumbnail if exists
            if (!string.IsNullOrEmpty(module.ThumbnailUrl))
            {
                var filePath = Path.Combine(Server.MapPath("~"), module.ThumbnailUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.TrainingModules.Remove(module);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Module deleted successfully!";
            return RedirectToAction("Modules");
        }
        #endregion

        #region Video Management
        // GET: TrainingContent/ModuleVideos/5
        public ActionResult ModuleVideos(int id)
        {
            var module = _context.TrainingModules
                .Include(m => m.Videos)
                .FirstOrDefault(m => m.Id == id);

            if (module == null)
            {
                return HttpNotFound();
            }

            ViewBag.ModuleTitle = module.Title;
            return View(module.Videos.OrderBy(v => v.DifficultyLevel).ToList());
        }
        // GET: TrainingContent/CreateVideo
        [HttpGet]
        public ActionResult CreateVideo(int moduleId)
        {
            var module = _context.TrainingModules.Find(moduleId);
            if (module == null)
            {
                TempData["ErrorMessage"] = "Module not found";
                return RedirectToAction("Modules");
            }

            ViewBag.ModuleTitle = module.Title;
            ViewBag.ModuleId = moduleId;

            return View(new TrainingVideo
            {
                TrainingModuleId = moduleId,
                UploadDate = DateTime.Now
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVideo(TrainingVideo model, HttpPostedFileBase videoFile)
        {
            // First validate all fields EXCEPT VideoUrl
            ModelState.Remove("VideoUrl"); // Remove VideoUrl from validation

            if (videoFile == null || videoFile.ContentLength == 0)
            {
                ModelState.AddModelError("videoFile", "Please upload a video file.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModuleTitle = _context.TrainingModules
                    .Where(m => m.Id == model.TrainingModuleId)
                    .Select(m => m.Title)
                    .FirstOrDefault();
                return View(model);
            }

            try
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(videoFile.FileName);
                string folderPath = Server.MapPath("~/uploads/training-videos/");
                string filePath = Path.Combine(folderPath, fileName);

                Directory.CreateDirectory(folderPath);
                videoFile.SaveAs(filePath);

                // Now set the VideoUrl after file is saved
                model.VideoUrl = "/uploads/training-videos/" + fileName;
                model.UploadDate = DateTime.Now;

                // Explicitly validate VideoUrl now
                if (string.IsNullOrEmpty(model.VideoUrl))
                {
                    ModelState.AddModelError("VideoUrl", "Video URL could not be generated");
                    return View(model);
                }

                _context.TrainingVideos.Add(model);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Video uploaded successfully!";
                return RedirectToAction("ModuleVideos", new { id = model.TrainingModuleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error uploading file: " + ex.Message);
                return View(model);
            }
        }
        public ActionResult EditVideo(int id)
        {
            var video = _context.TrainingVideos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }

            return View(video);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVideo(TrainingVideo model, HttpPostedFileBase videoFile)
        {
            if (ModelState.IsValid)
            {
                var video = _context.TrainingVideos.Find(model.Id);
                if (video == null)
                {
                    return HttpNotFound();
                }

                // Handle video file update
                if (videoFile != null && videoFile.ContentLength > 0)
                {
                    var uploadsDir = Path.Combine(Server.MapPath("~/uploads/training-videos"));
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Delete old video if exists
                    if (!string.IsNullOrEmpty(video.VideoUrl))
                    {
                        var oldFilePath = Path.Combine(Server.MapPath("~"), video.VideoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(videoFile.FileName);
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);
                    videoFile.SaveAs(filePath);

                    video.VideoUrl = "/uploads/training-videos/" + uniqueFileName;
                }

                video.Title = model.Title;
                video.Description = model.Description;
                video.DurationMinutes = model.DurationMinutes;
                video.DifficultyLevel = model.DifficultyLevel;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Video updated successfully!";
                return RedirectToAction("ModuleVideos", new { id = video.TrainingModuleId });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVideo(int id)
        {
            var video = _context.TrainingVideos.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }

            var moduleId = video.TrainingModuleId;

            // Delete video file if exists
            if (!string.IsNullOrEmpty(video.VideoUrl))
            {
                var filePath = Path.Combine(Server.MapPath("~"), video.VideoUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.TrainingVideos.Remove(video);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Video deleted successfully!";
            return RedirectToAction("ModuleVideos", new { id = moduleId });
        }
        #endregion

        #region Quiz Management
        // GET: TrainingContent/ModuleQuizzes/5
        public ActionResult ModuleQuizzes(int? id)
        {
            if (!id.HasValue)
            {
                // Handle the missing ID case - redirect or show error
                return RedirectToAction("Modules");
            }

         
            var module = _context.TrainingModules
                       .Include(m => m.QuizQuestions)
                       .FirstOrDefault(m => m.Id == id);

            if (module == null)
            {
                return HttpNotFound();
            }

            ViewBag.ModuleTitle = module.Title;
            ViewBag.ModuleId = module.Id; // This is crucial
            return View(module.QuizQuestions.ToList());
        }

        public ActionResult CreateQuiz(int moduleId)
        {
            try
            {
                var module = _context.TrainingModules.Find(moduleId);
                if (module == null)
                {
                    TempData["ErrorMessage"] = "Module not found";
                    return RedirectToAction("Modules");
                }

                ViewBag.ModuleTitle = module.Title;
                return View(new QuizQuestion { TrainingModuleId = moduleId });
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                TempData["ErrorMessage"] = "Error loading quiz creation form";
                return RedirectToAction("Modules");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuiz(QuizQuestion model, string CorrectAnswer)
        {
            if (ModelState.IsValid)
            {
                var module = _context.TrainingModules.Find(model.TrainingModuleId);
                if (module == null)
                {
                    return HttpNotFound();
                }

                // Add question
                _context.QuizQuestions.Add(model);
                _context.SaveChanges();

                // Add True/False options
                var trueOption = new QuizOption
                {
                    QuestionId = model.Id,
                    OptionText = "True",
                    IsCorrect = CorrectAnswer == "true"
                };
                _context.QuizOptions.Add(trueOption);

                var falseOption = new QuizOption
                {
                    QuestionId = model.Id,
                    OptionText = "False",
                    IsCorrect = CorrectAnswer == "false"
                };
                _context.QuizOptions.Add(falseOption);

                _context.SaveChanges();

                TempData["SuccessMessage"] = "True/False quiz question added successfully!";
                return RedirectToAction("ModuleQuizzes", new { id = model.TrainingModuleId });
            }

            if (model.TrainingModuleId > 0)
            {
                var module = _context.TrainingModules.Find(model.TrainingModuleId);
                ViewBag.ModuleTitle = module?.Title;
            }

            return View(model);
        }
        // GET: TrainingContent/EditQuiz/5
        public ActionResult EditQuiz(int id)
        {
            var question = _context.QuizQuestions
                .Include(q => q.Options)
                .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return HttpNotFound();
            }

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditQuiz(QuizQuestion model, List<string> OptionText, List<bool> IsCorrect, List<int> OptionIds)
        {
            if (ModelState.IsValid)
            {
                var question = _context.QuizQuestions
                    .Include(q => q.Options)
                    .FirstOrDefault(q => q.Id == model.Id);

                if (question == null)
                {
                    return HttpNotFound();
                }

                // Update question
                question.QuestionText = model.QuestionText;
                question.Explanation = model.Explanation;

                // Update options
                var existingOptions = question.Options.ToList();

                // Update or add options
                for (int i = 0; i < OptionText.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(OptionText[i]))
                    {
                        if (OptionIds != null && i < OptionIds.Count && OptionIds[i] > 0)
                        {
                            // Update existing option
                            var existingOption = existingOptions.FirstOrDefault(o => o.Id == OptionIds[i]);
                            if (existingOption != null)
                            {
                                existingOption.OptionText = OptionText[i];
                                existingOption.IsCorrect = IsCorrect != null && i < IsCorrect.Count && IsCorrect[i];
                            }
                        }
                        else
                        {
                            // Add new option
                            var option = new QuizOption
                            {
                                QuestionId = question.Id,
                                OptionText = OptionText[i],
                                IsCorrect = IsCorrect != null && i < IsCorrect.Count && IsCorrect[i]
                            };
                            _context.QuizOptions.Add(option);
                        }
                    }
                }

                // Remove options not in the submitted list
                var submittedOptionIds = OptionIds?.Where(id => id > 0).ToList() ?? new List<int>();
                foreach (var option in existingOptions)
                {
                    if (!submittedOptionIds.Contains(option.Id))
                    {
                        _context.QuizOptions.Remove(option);
                    }
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Quiz question updated successfully!";
                return RedirectToAction("ModuleQuizzes", new { id = question.TrainingModuleId });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQuiz(int id)
        {
            var question = _context.QuizQuestions
                .Include(q => q.Options)
                .FirstOrDefault(q => q.Id == id);

            if (question == null)
            {
                return HttpNotFound();
            }

            var moduleId = question.TrainingModuleId;

            _context.QuizOptions.RemoveRange(question.Options);
            _context.QuizQuestions.Remove(question);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Quiz question deleted successfully!";
            return RedirectToAction("ModuleQuizzes", new { id = moduleId });
        }

        // GET: TrainingContent/QuizResults/5
        public ActionResult QuizResults(int moduleId)
        {
            var results = _context.QuizResults
                .Include(r => r.Pet)
                .Where(r => r.ModuleId == moduleId)
                .OrderByDescending(r => r.CompletedAt)
                .ToList();

            var module = _context.TrainingModules.Find(moduleId);
            ViewBag.ModuleTitle = module?.Title;

            return View(results);
        }
        #endregion

        #region Progress Tracking
        // GET: TrainingContent/PetProgress/5
        public ActionResult PetProgress(int petId)
        {
            var progress = _context.ModuleProgresses
                .Include(p => p.Module)
                .Where(p => p.PetId == petId)
                .OrderBy(p => p.Module.DifficultyLevel)
                .ToList();

            var pet = _context.pets.Find(petId);
            ViewBag.PetName = pet?.Name;

            return View(progress);
        }

        // GET: TrainingContent/WatchedVideos/5
        public ActionResult WatchedVideos(int petId)
        {
            var watchedVideos = _context.WatchedVideos
                .Include(w => w.Video)
                .Where(w => w.PetId == petId)
                .OrderByDescending(w => w.WatchedDate)
                .ToList();

            var pet = _context.pets.Find(petId);
            ViewBag.PetName = pet?.Name;

            return View(watchedVideos);
        }

        // GET: TrainingContent/CompletedModules/5
        public ActionResult CompletedModules(int petId)
        {
            var completedModules = _context.CompletedModules
                .Include(c => c.Module)
                .Where(c => c.PetId == petId)
                .OrderByDescending(c => c.CompletionDate)
                .ToList();

            var pet = _context.pets.Find(petId);
            ViewBag.PetName = pet?.Name;

            return View(completedModules);
        }
        #endregion
        [HttpPost]
        public JsonResult MarkVideoWatched(int petId, int moduleId, int videoId)
        {
            try
            {
                // Get or create progress record
                var progress = _context.PetProgresses.FirstOrDefault(p => p.PetId == petId) ??
                              new PetProgress { PetId = petId };

                // Check if already watched
                var alreadyWatched = _context.WatchedVideos
                    .Any(w => w.PetId == petId && w.VideoId == videoId);

                if (!alreadyWatched)
                {
                    // Add watched video record
                    _context.WatchedVideos.Add(new WatchedVideo
                    {
                        PetId = petId,
                        VideoId = videoId,
                        WatchedDate = DateTime.Now
                    });

                    // Update progress
                    progress.VideosWatched++;
                    progress.LastTrainingDate = DateTime.Now;

                    if (progress.PetId == 0)
                    {
                        _context.PetProgresses.Add(progress);
                    }

                    _context.SaveChanges();
                }

                return Json(new
                {
                    success = true,
                    videosWatched = progress.VideosWatched
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // GET: Training/StartTraining?moduleId=1&petId=5
        public ActionResult StartTraining(int moduleId, int petId)
        {
            try
            {
                // Get module with videos and questions
                var module = _context.TrainingModules
                    .Include(m => m.Videos)
                    .Include(m => m.QuizQuestions.Select(q => q.Options))
                    .FirstOrDefault(m => m.Id == moduleId);

                if (module == null)
                {
                    TempData["ErrorMessage"] = "Training module not found";
                    return RedirectToAction("Modules");
                }

                var pet = _context.pets.Find(petId);
                if (pet == null)
                {
                    TempData["ErrorMessage"] = "Pet not found";
                    return RedirectToAction("Index", "Pets");
                }

                // Calculate pet's age if DateOfBirth is available
                int? petAge = null;

                // First check if DateOfBirth is set (not null and not default DateTime)
                if (pet.DateOfBirth != null && pet.DateOfBirth != default(DateTime))
                {
                    // No need for .Value here since we've confirmed it's not null
                    petAge = CalculateAge(pet.DateOfBirth);

                    // Check if pet meets module age requirements
                    if (petAge.HasValue && petAge.Value < module.MinimumAge)
                    {
                        TempData["ErrorMessage"] = $"This pet is too young for this module (Minimum age: {module.MinimumAge})";
                        return RedirectToAction("PetDetails", "Pets", new { id = petId });
                    }
                }

                // Get or create progress tracking
                var progress = _context.ModuleProgresses
                    .FirstOrDefault(p => p.PetId == petId && p.ModuleId == moduleId);

                if (progress == null)
                {
                    progress = new ModuleProgress
                    {
                        PetId = petId,
                        ModuleId = moduleId,
                        Progress = 0,
                        VideosWatched = 0,
                        QuizAttempts = 0,
                        QuizScore = 0,
                        IsCompleted = false,
                        LastAccessed = DateTime.Now
                    };
                    _context.ModuleProgresses.Add(progress);
                    _context.SaveChanges();
                }
                else
                {
                    progress.LastAccessed = DateTime.Now;
                    _context.SaveChanges();
                }

                // Filter videos by difficulty level suitable for the pet
                var suitableVideos = module.Videos
                    .OrderBy(v => v.DifficultyLevel)
                    .ToList();

                return View(new TrainingSessionViewModel
                {
                    Module = module,
                    Videos = suitableVideos,
                    QuizQuestions = module.QuizQuestions.ToList(),
                    Progress = progress,
                    PetId = petId,
                    PetAge = petAge
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting training session for module {ModuleId} and pet {PetId}", moduleId, petId);
                TempData["ErrorMessage"] = "An error occurred while starting the training session";
                return RedirectToAction("Index", "Home");
            }
        }

        // Helper method to calculate age from date of birth
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            // Adjust if birthday hasn't occurred yet this year
            if (dateOfBirth > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
        [HttpPost]
        public JsonResult RecordVideoWatched(int videoId, int petId)
        {
            var video = _context.TrainingVideos
                .Include(v => v.TrainingModule)
                .FirstOrDefault(v => v.Id == videoId);

            if (video == null)
            {
                return Json(new { success = false, message = "Video not found" });
            }

            var module = video.TrainingModule;

            // Ensure progress entry exists
            var progress = _context.ModuleProgresses
                .FirstOrDefault(p => p.PetId == petId && p.ModuleId == module.Id);

            if (progress == null)
            {
                progress = new ModuleProgress
                {
                    PetId = petId,
                    ModuleId = module.Id,
                    VideosWatched = 0,
                    Progress = 0,
                    LastAccessed = DateTime.Now
                };
                _context.ModuleProgresses.Add(progress);
            }

            // Check if already watched
            bool alreadyWatched = _context.WatchedVideos
                .Any(w => w.PetId == petId && w.VideoId == videoId);

            if (!alreadyWatched)
            {
                _context.WatchedVideos.Add(new WatchedVideo
                {
                    PetId = petId,
                    VideoId = videoId,
                    WatchedDate = DateTime.Now
                });

                progress.VideosWatched++;
            }

            progress.Progress = (int)((double)progress.VideosWatched / module.Videos.Count * 100);
            progress.LastAccessed = DateTime.Now;

            _context.SaveChanges();

            bool allVideosWatched = progress.VideosWatched >= module.Videos.Count;

            return Json(new
            {
                success = true,
                progress = progress.Progress,
                videosWatched = progress.VideosWatched,
                totalVideos = module.Videos.Count,
                allVideosWatched = allVideosWatched
            });
        }
        [HttpPost]
        public JsonResult RecordQuizResult(int petId, string trainingType, int score)
        {
            try
            {
                _context.RecordQuizCompleted(petId, trainingType, score);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitQuiz(QuizSubmission submission)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Invalid submission", errors });
                }

                // Load module with questions and options
                var module = _context.TrainingModules
                    .Include(m => m.QuizQuestions.Select(q => q.Options))
                    .FirstOrDefault(m => m.Id == submission.ModuleId);

                if (module == null)
                {
                    return Json(new { success = false, message = "Module not found" });
                }

                // Calculate score
                int correctAnswers = 0;
                var questionResults = new List<object>();

                foreach (var answer in submission.Answers)
                {
                    var question = module.QuizQuestions.FirstOrDefault(q => q.Id == answer.QuestionId);
                    if (question == null) continue;

                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    bool isCorrect = correctOption != null && correctOption.Id == answer.SelectedOptionId;

                    if (isCorrect) correctAnswers++;

                    questionResults.Add(new
                    {
                        questionId = question.Id,
                        correctOptionId = correctOption?.Id ?? 0,
                        isCorrect = isCorrect,
                        explanation = question.Explanation
                    });
                }

                double score = module.QuizQuestions.Count > 0
                    ? Math.Round((correctAnswers * 100.0) / module.QuizQuestions.Count, 2)
                    : 0;

                // Update progress
                var progress = _context.ModuleProgresses
                    .FirstOrDefault(p => p.PetId == submission.PetId && p.ModuleId == submission.ModuleId);

                if (progress != null)
                {
                    progress.QuizAttempts++;
                    progress.QuizScore = score;
                    progress.IsCompleted = progress.VideosWatched >= module.Videos.Count && score >= 70;
                    progress.LastAccessed = DateTime.Now;
                }

                // Save results
                _context.QuizResults.Add(new QuizResult
                {
                    PetId = submission.PetId,
                    ModuleId = submission.ModuleId,
                    Score = (int)score,
                    CompletedAt = DateTime.Now
                });

                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    score = score,
                    isCompleted = progress?.IsCompleted ?? false,
                    results = questionResults,
                    passed = score >= 70
                });
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Quiz submission error: {ex}");
                return Json(new { success = false, message = "An error occurred while processing your submission" });
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
}