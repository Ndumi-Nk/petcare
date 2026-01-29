using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{

    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bookings/BookingSummary?id=5&type=vet OR adoption OR boarding
        [HttpGet]
        public ActionResult BookingSummary(int id, string type)
        {
            if (id == null || string.IsNullOrEmpty(type))
            {
                return RedirectToAction("Index", "Home"); // Redirect if missing params
            }
            dynamic model = new System.Dynamic.ExpandoObject();

            if (type == "vet")
            {
                var consult = db.Vet_Consultations.Find(id);
                if (consult == null) return HttpNotFound();
                model.VetConsultation = consult;
            }
            else if (type == "adoption")
            {
                var adoption = db.pet_Adoptions.Find(id);
                if (adoption == null) return HttpNotFound();
                model.PetAdoption = adoption;
            }
            else if (type == "boarding")
            {
                var board = db.pet_Boardings.Find(id);
                if (board == null) return HttpNotFound();
                model.PetBoarding = board;
            }

            return View(model);
        }
        [HttpGet]
        public ActionResult AllBookings()
        {
            var model = new AllBookingsViewModel
            {
                VetConsultations = db.Vet_Consultations
                    .OrderByDescending(v => v.Consult_Date)
                    .ToList(),

                PetAdoptions = db.pet_Adoptions
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToList(),

                PetBoardings = db.pet_Boardings
                    .OrderByDescending(b => b.BookingDate)
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(int ConsultId = 0, int AdoptionId = 0, int BoardId = 0)
        {
            string message = "Booking cancelled. No refund will be issued.";

            if (ConsultId > 0)
            {
                var consult = db.Vet_Consultations.Find(ConsultId);
                if (consult != null)
                {
                    db.Vet_Consultations.Remove(consult); // Removed PaymentStatus update
                    db.SaveChanges();
                }
            }
            else if (AdoptionId > 0)
            {
                var adoption = db.pet_Adoptions.Find(AdoptionId);
                if (adoption != null)
                {
                    db.pet_Adoptions.Remove(adoption);
                    db.SaveChanges();
                }
            }
            else if (BoardId > 0)
            {
                var board = db.pet_Boardings.Find(BoardId);
                if (board != null)
                {
                    db.pet_Boardings.Remove(board);
                    db.SaveChanges();
                }
            }

            TempData["CancelMessage"] = message;
            return RedirectToAction("Index", "Home");
        }
    }
}