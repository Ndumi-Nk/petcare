using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PetCare_system.Models;
using System.Data.Entity;

namespace PetCare_system.Controllers
{
    [Authorize]
    public class Pet_BoardingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: View all boardings (admin only)
        [Authorize(Roles = "Admin")]
        public ActionResult Boardings()
        {
            var boardings = db.pet_Boardings
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
            return View(boardings);
        }

        // GET: Pet boarding form
        [HttpGet]
        public ActionResult Pet_Boarding()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new Pet_Boarding
            {
                OwnerName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Phone = user.CellphoneNumber,
                UserId = userId
            };

            var userPets = db.pets
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .ToList();

            if (!userPets.Any())
            {
                TempData["Warning"] = "You need to register at least one pet before making a boarding reservation.";
                return RedirectToAction("Register", "Pet");
            }

            ViewBag.Pets = userPets;
            return View(model);
        }

        // POST: Submit pet boarding
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pet_Boarding(Pet_Boarding booking)
        {
            var userId = User.Identity.GetUserId();
            var userPets = db.pets
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .ToList();
            ViewBag.Pets = userPets;

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            // Validate dates
            if (booking.CheckInDate < DateTime.Today)
            {
                ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
                return View(booking);
            }

            if (booking.CheckOutDate <= booking.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                return View(booking);
            }

            // Validate pet selection
            if (booking.SelectedPetId <= 0 || !userPets.Any(p => p.Id == booking.SelectedPetId))
            {
                ModelState.AddModelError("SelectedPetId", "Please select a valid pet.");
                return View(booking);
            }

            try
            {
                var user = db.Users.Find(userId);
                var pet = db.pets.Find(booking.SelectedPetId);

                if (user == null || pet == null)
                {
                    ModelState.AddModelError("", "User or pet not found.");
                    return View(booking);
                }

                // Check for overlapping bookings for the same pet
                bool hasOverlap = db.pet_Boardings.Any(b =>
                    b.SelectedPetId == booking.SelectedPetId &&
                    b.Status != "Cancelled" &&
                    ((b.CheckInDate <= booking.CheckInDate && b.CheckOutDate > booking.CheckInDate) ||
                     (b.CheckInDate < booking.CheckOutDate && b.CheckOutDate >= booking.CheckOutDate) ||
                     (b.CheckInDate >= booking.CheckInDate && b.CheckOutDate <= booking.CheckOutDate)));

                if (hasOverlap)
                {
                    ModelState.AddModelError("", "This pet already has a boarding reservation during the selected dates.");
                    return View(booking);
                }

                // Complete booking details
                booking.PetName = pet.Name;
                booking.PetType = pet.Type;
                booking.PetBreed = pet.Breed;
                booking.UserId = userId;
                booking.BookingDate = DateTime.Now;
                booking.Status = "Pending";
                booking.Check_Status = "Not Checked In";

                db.pet_Boardings.Add(booking);
                db.SaveChanges();

                try
                {
                    SendConfirmationEmail(booking);
                }
                catch (Exception ex)
                {
                    // Log email error but don't stop the process
                    System.Diagnostics.Debug.WriteLine($"Email Error: {ex.Message}");
                }

                return RedirectToAction("Confirmation", new { id = booking.board_Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                return View(booking);
            }
        }

        // GET: Booking confirmation
        public ActionResult Confirmation(int id)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.pet_Boardings
                .Include(b => b.User)
                .FirstOrDefault(b => b.board_Id == id && b.UserId == userId);

            if (booking == null)
            {
                return HttpNotFound();
            }

            return View(booking);
        }

        // GET: User's boarding reservations
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var bookings = db.pet_Boardings
                .Where(b => b.UserId == userId && b.Status != "Cancelled")
                .OrderByDescending(b => b.BookingDate)
                .ToList();
            return View(bookings);
        }

        // GET: Admin view of all boarding reservations
        [Authorize(Roles = "Admin")]
        public ActionResult IndexAdmin()
        {
            var bookings = db.pet_Boardings
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
            return View(bookings);
        }

        // GET: Cancel booking form
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var booking = db.pet_Boardings.Find(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Index");
            }

            // Prevent cancel if already checked-in or completed
            if (booking.Check_Status == "Checked-In" || booking.Check_Status == "Completed")
            {
                TempData["Error"] = "You cannot cancel a checked-in or completed booking.";
                return RedirectToAction("Index");
            }

            // Prevent cancel if already checked-in or completed
            if (booking.Check_Status == "Checked-Out" || booking.Check_Status == "Completed")
            {
                TempData["Error"] = "You cannot cancel a checked-out or completed booking.";
                return RedirectToAction("Index");
            }

            // Prevent cancel if already checked-in or completed
            if (booking.Check_Status == "Cancelled" || booking.Check_Status == "Completed")
            {
                TempData["Error"] = "You cannot cancel a cancelled or completed booking.";
                return RedirectToAction("Index");
            }
            // Prevent cancel within 24 hours
            if (booking.CheckInDate <= DateTime.Now.AddHours(24))
            {
                TempData["Error"] = "Cannot cancel a booking within 24 hours of check-in.";
                return RedirectToAction("Index");
            }

            booking.Check_Status = "Cancelled";
            db.SaveChanges();

            TempData["Message"] = "Booking cancelled successfully.";
            return RedirectToAction("Index");
        }


        //// POST: Cancel booking
        //[HttpPost, ActionName("Cancel")]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public ActionResult CancelConfirmed(int id)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var booking = db.pet_Boardings
        //        .FirstOrDefault(b => b.board_Id == id && b.UserId == userId);

        //    if (booking == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // Only allow cancellation if not already checked in or completed
        //    if (booking.Check_Status != "Not Checked In" || booking.Status == "Completed")
        //    {
        //        TempData["Error"] = "You can only cancel bookings that haven't started yet.";
        //        return RedirectToAction("Index");
        //    }

        //    booking.Status = "Cancelled";
        //    db.SaveChanges();

        //    try
        //    {
        //        SendCancellationEmail(booking);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Email Error: {ex.Message}");
        //    }

        //    TempData["Success"] = "Your booking has been cancelled successfully.";
        //    return RedirectToAction("Index");
        //}

        // GET: Admin check-in
        [Authorize(Roles = "Admin")]
        public ActionResult AdminCheckIn(int id)
        {
            var booking = db.pet_Boardings.Find(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("IndexAdmin");
            }

            // Use consistent status checks
            if (booking.Check_Status == "Not Checked In" && booking.Status == "Pending")
            {
                booking.Check_Status = "Checked-In"; // Use hyphen consistently
                booking.Status = "Active";
                db.SaveChanges();
                TempData["Success"] = $"{booking.PetName} has been checked in successfully.";
            }
            else if (booking.Check_Status == "Checked-In") // Match the updated value
            {
                TempData["Info"] = $"{booking.PetName} is already checked in.";
            }
            else
            {
                TempData["Error"] = $"Cannot check in. Current status: {booking.Status}";
            }

            return RedirectToAction("IndexAdmin");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult CheckOut(int id)
        {
            var booking = db.pet_Boardings.Find(id);
            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("IndexAdmin");
            }

            if (booking.Check_Status == "Checked-In" && booking.Status == "Active") // Match hyphen
            {
                booking.Check_Status = "Checked-Out"; // Use hyphen consistently
                booking.Status = "Completed";
                db.SaveChanges();
                TempData["Success"] = $"{booking.PetName} has been checked out successfully.";
            }
            else
            {
                TempData["Error"] = $"Cannot check out. Current status: {booking.Check_Status}";
            }

            return RedirectToAction("IndexAdmin");
        }
        

        // GET: Booking details
        public ActionResult Details(int id)
        {
            var booking = db.pet_Boardings
                .Include(b => b.User)
                .FirstOrDefault(b => b.board_Id == id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            return View(booking);
        }

        // GET: Reschedule booking form
        [Authorize]
        public ActionResult Reschedule(int id)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.pet_Boardings
                .FirstOrDefault(b => b.board_Id == id && b.UserId == userId);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Only allow rescheduling if not already checked in or completed
            if (booking.Check_Status != "Not Checked In" || booking.Status == "Completed")
            {
                TempData["Error"] = "You can only reschedule bookings that haven't started yet.";
                return RedirectToAction("Index");
            }

            ViewBag.Pets = db.pets
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .ToList();

            return View(booking);
        }

        // POST: Reschedule booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Reschedule(int id, Pet_Boarding updatedBooking)
        {
            var userId = User.Identity.GetUserId();
            var originalBooking = db.pet_Boardings
                .FirstOrDefault(b => b.board_Id == id && b.UserId == userId);

            if (originalBooking == null)
            {
                return HttpNotFound();
            }

            // Only allow rescheduling if not already checked in or completed
            if (originalBooking.Check_Status != "Not Checked In" || originalBooking.Status == "Completed")
            {
                TempData["Error"] = "You can only reschedule bookings that haven't started yet.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Pets = db.pets
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.Name)
                    .ToList();
                return View(updatedBooking);
            }

            // Validate dates
            if (updatedBooking.CheckInDate < DateTime.Today)
            {
                ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
                ViewBag.Pets = db.pets
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.Name)
                    .ToList();
                return View(updatedBooking);
            }

            if (updatedBooking.CheckOutDate <= updatedBooking.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                ViewBag.Pets = db.pets
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.Name)
                    .ToList();
                return View(updatedBooking);
            }

            try
            {
                // Check for overlapping bookings for the same pet (excluding current booking)
                bool hasOverlap = db.pet_Boardings.Any(b =>
                    b.board_Id != originalBooking.board_Id &&
                    b.SelectedPetId == updatedBooking.SelectedPetId &&
                    b.Status != "Cancelled" &&
                    ((b.CheckInDate <= updatedBooking.CheckInDate && b.CheckOutDate > updatedBooking.CheckInDate) ||
                     (b.CheckInDate < updatedBooking.CheckOutDate && b.CheckOutDate >= updatedBooking.CheckOutDate) ||
                     (b.CheckInDate >= updatedBooking.CheckInDate && b.CheckOutDate <= updatedBooking.CheckOutDate)));

                if (hasOverlap)
                {
                    ModelState.AddModelError("", "This pet already has a boarding reservation during the selected dates.");
                    ViewBag.Pets = db.pets
                        .Where(p => p.UserId == userId)
                        .OrderBy(p => p.Name)
                        .ToList();
                    return View(updatedBooking);
                }

                // Update booking details
                originalBooking.CheckInDate = updatedBooking.CheckInDate;
                originalBooking.CheckOutDate = updatedBooking.CheckOutDate;
                originalBooking.SelectedPetId = updatedBooking.SelectedPetId;
                originalBooking.Package = updatedBooking.Package;
                originalBooking.SpecialNeeds = updatedBooking.SpecialNeeds;

                // Update pet info in case they selected a different pet
                var pet = db.pets.Find(updatedBooking.SelectedPetId);
                if (pet != null)
                {
                    originalBooking.PetName = pet.Name;
                    originalBooking.PetType = pet.Type;
                    originalBooking.PetBreed = pet.Breed;
                }

                db.SaveChanges();

                try
                {
                    SendRescheduleEmail(originalBooking);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Email Error: {ex.Message}");
                }

                TempData["Success"] = "Your booking has been rescheduled successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                ViewBag.Pets = db.pets
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.Name)
                    .ToList();
                return View(updatedBooking);
            }
        }

        private void SendConfirmationEmail(Pet_Boarding booking)
        {
            // Implementation remains the same as your original
        }

        private void SendCancellationEmail(Pet_Boarding booking)
        {
            using (var mm = new MailMessage("petcare@example.com", booking.Email))
            {
                mm.Subject = $"Pet Boarding Cancellation #{booking.board_Id}";
                mm.Body = $@"
Dear {booking.OwnerName},

Your pet boarding has been cancelled successfully.

Cancelled Booking Details:
- Booking ID: {booking.board_Id}
- Pet: {booking.PetName} ({booking.PetType}, {booking.PetBreed})
- Original Check-In: {booking.CheckInDate:yyyy-MM-dd}
- Original Check-Out: {booking.CheckOutDate:yyyy-MM-dd}

If this was a mistake or you need to reschedule, please contact us.

Thank you,
PetCare Systems
";
                mm.IsBodyHtml = false;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.example.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("email@example.com", "password");
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }

        private void SendRescheduleEmail(Pet_Boarding booking)
        {
            using (var mm = new MailMessage("petcare@example.com", booking.Email))
            {
                mm.Subject = $"Pet Boarding Rescheduled #{booking.board_Id}";
                mm.Body = $@"
Dear {booking.OwnerName},

Your pet boarding has been rescheduled successfully.

Updated Booking Details:
- Booking ID: {booking.board_Id}
- Pet: {booking.PetName} ({booking.PetType}, {booking.PetBreed})
- New Check-In: {booking.CheckInDate:yyyy-MM-dd}
- New Check-Out: {booking.CheckOutDate:yyyy-MM-dd}
- Nights: {booking.TotalNights}
- Package: {booking.Package} (R{booking.PackagePrice}/night)
- Total: R{booking.TotalPrice}

Status: {booking.Status}

Thank you for choosing PetCare Systems.
";
                mm.IsBodyHtml = false;

                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.example.com";
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("email@example.com", "password");
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}