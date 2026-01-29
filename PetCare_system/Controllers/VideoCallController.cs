using Microsoft.AspNet.Identity;
using PetCare_system.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace PetCare_system.Controllers
{
    public class VideoCallController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // Get logged in user ID (works for both Identity and Session demo login)
        private string GetCurrentUserId()
        {
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                // Demo login fallback
                if (Session["TrainerId"] != null)
                {
                    userId = Session["TrainerId"].ToString();
                }
                else if (Session["UserId"] != null)
                {
                    userId = Session["UserId"].ToString();
                }
            }

            return userId;
        }

        // Get logged in role (works for both Identity and Session demo login)
        private bool IsCurrentUserTrainer()
        {
            if (User.IsInRole("Trainer"))
                return true;

            if (Session["TrainerId"] != null)
                return true;

            return false;
        }

        
        public ActionResult JoinSession(int sessionId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new HttpUnauthorizedResult();
            }

            var session = _context.TrainingSessions
                .Include(s => s.Pet)
                .Include(s => s.Trainer)
                .FirstOrDefault(s => s.Id == sessionId);

            if (session == null)
            {
                return HttpNotFound();
            }

            // Verify user is either the trainer or the pet owner
            if (IsCurrentUserTrainer())
            {
                if (session.Trainer.UserId != userId && session.Trainer.TrainerId.ToString() != userId)
                {
                    return new HttpUnauthorizedResult();
                }
            }
            else
            {
                if (session.Pet.UserId != userId && session.Pet.UserId.ToString() != userId)
                {
                    return new HttpUnauthorizedResult();
                }
            }

            var viewModel = new VideoCallViewModel
            {
                SessionId = sessionId,
                UserId = userId,
                UserType = IsCurrentUserTrainer() ? "trainer" : "user",
                OtherParticipantName = IsCurrentUserTrainer()
                    ? session.Pet.Name + " (Owner)"
                    : session.Trainer.FullName,
                SessionStartTime = DateTime.Now,
                IsTrainer = IsCurrentUserTrainer(),
                Session = session
            };

            return View(viewModel);
        }

        [HttpPost]
        
        public ActionResult EndSession(int sessionId)
        {
            var session = _context.TrainingSessions.Find(sessionId);
            if (session != null)
            {
                session.Status = "Completed";
                session.CheckOutTime = DateTime.Now;
                _context.SaveChanges();

                UpdatePetProgress(session.PetId, $"Completed video training session with {session.Trainer.FullName}");
            }

            return RedirectToAction("Dashboard", "Trainer");
        }

        // Add the UpdatePetProgress method that was missing
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

                if (progress.ProgressPercentage < 100)
                {
                    progress.ProgressPercentage = Math.Min(progress.ProgressPercentage + 5, 100);
                }
            }

            _context.SaveChanges();
        }
    }
}
