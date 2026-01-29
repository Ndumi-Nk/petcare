using PetCare_system.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class TrainerAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult FirstTimePasswordChange()
        {
            var trainerId = Session["TrainerId"] as int?;
            if (trainerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var trainer = db.Trainers.Find(trainerId);
            if (trainer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if they've already completed password change
            if (!trainer.IsTempPassword)
            {
                return RedirectToAction("Dashboard", "Trainers");
            }

            return View(new FirstTimePasswordChangeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FirstTimePasswordChange(FirstTimePasswordChangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var trainerId = Session["TrainerId"] as int?;
            if (trainerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new ApplicationDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Disable validation for unrelated fields like ConfirmTempPassword
                        db.Configuration.ValidateOnSaveEnabled = false;

                        var trainer = db.Trainers.Find(trainerId);
                        if (trainer == null)
                        {
                            ModelState.AddModelError("", "Trainer not found.");
                            return View(model);
                        }

                        trainer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                        trainer.IsTempPassword = false;

                        db.Entry(trainer).Property(t => t.PasswordHash).IsModified = true;
                        db.Entry(trainer).Property(t => t.IsTempPassword).IsModified = true;

                        db.SaveChanges();
                        transaction.Commit();

                        TempData["SuccessMessage"] = "Password changed successfully!";
                        return RedirectToAction("Dashboard", "Trainers");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Error saving password: " + ex.Message);
                        return View(model);
                    }
                    finally
                    {
                        // Turn validation back on to avoid affecting other parts of your app
                        db.Configuration.ValidateOnSaveEnabled = true;
                    }
                }
            }
        }

        // GET: Regular password change
        public ActionResult ChangePassword()
        {
            var trainerId = Session["TrainerId"] as int?;
            if (trainerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ChangePasswordsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var trainerId = Session["TrainerId"] as int?;
                if (trainerId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                using (var db = new ApplicationDbContext())
                {
                    // ⚠️ Disable full entity validation to avoid issues with [Compare] on ConfirmTempPassword
                    db.Configuration.ValidateOnSaveEnabled = false;

                    var trainer = db.Trainers.Find(trainerId);
                    if (trainer == null)
                    {
                        ModelState.AddModelError("", "Trainer not found.");
                        return View(model);
                    }

                    // ✅ Verify current password
                    if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, trainer.PasswordHash))
                    {
                        ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                        return View(model);
                    }

                    // ✅ Update password hash only
                    trainer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                    // Only mark PasswordHash as modified
                    db.Entry(trainer).Property(t => t.PasswordHash).IsModified = true;

                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Your password has been changed successfully!";
                    return RedirectToAction("Dashboard", "Trainers");
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while changing your password: " + ex.Message);
                return View(model);
            }
        }
    }
}