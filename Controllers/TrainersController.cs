using PetCare_system.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class TrainersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Trainers
        public ActionResult Index()
        {
            return View(db.Trainers.ToList());
        }
        public ActionResult Dashboard()
        {
            // Existing authentication check
            string trainerEmail = Session["TrainerEmail"] as string;
            if (string.IsNullOrEmpty(trainerEmail))
                return RedirectToAction("Login", "Account");

            var trainer = db.Trainers.FirstOrDefault(t => t.Email == trainerEmail);
            if (trainer == null)
                return HttpNotFound("Trainer not found");

            // NEW: Get the first available module ID
            var firstModuleId = db.TrainingModules
                                 .OrderBy(m => m.Id)
                                 .Select(m => m.Id)
                                 .FirstOrDefault();

            // Pass both trainer and module ID to the view
            ViewBag.Trainer = trainer;
            ViewBag.FirstModuleId = firstModuleId;

            return View(trainer);
        }
        public ActionResult Profile()
        {
            // Simulate login/session (replace with your actual auth/session logic)
            string trainerEmail = Session["TrainerEmail"] as string;

            if (string.IsNullOrEmpty(trainerEmail))
                return RedirectToAction("Login", "Account"); // Adjust as needed

            var trainer = db.Trainers.FirstOrDefault(t => t.Email == trainerEmail);

            if (trainer == null)
                return HttpNotFound("Trainer not found");

            return View(trainer); // View: Views/Trainers/Profile.cshtml
        }

        // GET: Trainers/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Trainer trainer)
        {
            try
            {
                // Set only the absolute essentials
                trainer.CreatedAt = DateTime.UtcNow;
                trainer.IsActive = true;

                // Handle image upload only if file was provided
                if (trainer.ImageFile != null && trainer.ImageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(trainer.ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Uploads/Trainers"), fileName);
                    trainer.ImageFile.SaveAs(path);
                    trainer.ProfilePicture = "/Uploads/Trainers/" + fileName;
                }

                // Set password if not provided
                if (string.IsNullOrEmpty(trainer.TempPassword))
                {
                    trainer.TempPassword = GenerateSecurePassword();
                }

                trainer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(trainer.TempPassword);

                // Set UserId if not provided
                if (string.IsNullOrEmpty(trainer.UserId))
                {
                    trainer.UserId = Guid.NewGuid().ToString();
                }

                db.Trainers.Add(trainer);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the full error
                System.Diagnostics.Debug.WriteLine("SAVE ERROR: " + ex.ToString());
                TempData["ErrorMessage"] = "Save failed: " + ex.Message;
                return View(trainer);
            }
        }
        // GET: Trainers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Trainer trainer = db.Trainers.Find(id);
            if (trainer == null)
                return HttpNotFound();

            return View(trainer);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                if (trainer.ImageFile != null && trainer.ImageFile.ContentLength > 0)
                {
                    // Delete old image
                    if (!string.IsNullOrEmpty(trainer.ProfilePicture))
                    {
                        string oldPath = Server.MapPath(trainer.ProfilePicture);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    string uploadDir = Server.MapPath("~/Uploads/Trainers/");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(trainer.ImageFile.FileName)}";
                    string filePath = Path.Combine(uploadDir, fileName);
                    trainer.ImageFile.SaveAs(filePath);
                    trainer.ProfilePicture = "/Uploads/Trainers/" + fileName;
                }

                db.Entry(trainer).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Trainer updated successfully!";
                return RedirectToAction("Index");
            }
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Trainer trainer = db.Trainers.Find(id);
            if (trainer == null)
                return HttpNotFound();

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trainer trainer = db.Trainers.Find(id);

            if (!string.IsNullOrEmpty(trainer.ProfilePicture))
            {
                string path = Server.MapPath(trainer.ProfilePicture);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            db.Trainers.Remove(trainer);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Trainer deleted successfully!";
            return RedirectToAction("Index");
        }

        private void SendWelcomeEmail(Trainer trainer)
        {
            string emailFrom = "shezielihle186@gmail.com";
            string emailPassword = "xjop iuut owdu loav";
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;

            using (var mm = new MailMessage(emailFrom, trainer.Email))
            {
                mm.Subject = "Your New Trainer Account";
                mm.Body = FormatEmailBody(trainer);
                mm.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = smtpHost;
                    smtp.Port = smtpPort;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                    smtp.Send(mm);
                }
            }
        }

        private string GenerateSecurePassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return new string(bytes.Select(b => validChars[b % validChars.Length]).ToArray());
            }
        }

        private string FormatEmailBody(Trainer trainer)
        {
            return $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #4a6fa5;'>Welcome to Our Training System!</h2>
    <p>Dear {trainer.FullName},</p>
    <p>Your trainer account has been successfully created.</p>
    
    <div style='background: #f8f9fa; padding: 15px; border-radius: 5px; margin: 15px 0;'>
        <p><strong>Email:</strong> {trainer.Email}</p>
        <p><strong>Temporary Password:</strong> {trainer.TempPassword}</p>
    </div>
    
    <p style='color: #dc3545; font-weight: bold;'>
        Please change your password after first login.
    </p>
    
    <p>Best regards,<br>Training System Team</p>
</body>
</html>";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
