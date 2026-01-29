using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PetCare_system.Models;

namespace PetCare_system.Controllers
{
    public class GroomingStaffsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GroomingStaffs
        public ActionResult Index()
        {
            var groomers = db.GroomingStaffs
                             .Include(g => g.SparGroomings)
                             .ToList();
            return View(groomers);
        }

        // GET: GroomingReport/Report
        public ActionResult Report(int bookingId)
        {
            ViewBag.BookingId = bookingId;
            var model = Session["CurrentBooking"] as BookingReportModel;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report(BookingReportModel model)
        {
            if (ModelState.IsValid)
            {
                // Add actual report processing logic here
                TempData["Message"] = "Report has been submitted successfully.";
                Session.Remove("CurrentBooking");
                return RedirectToAction("Report");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult GetBookingDetails(int bookingId)
        {
            var booking = db.spar_Groomings
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return Json(new { error = "Booking not found" }, JsonRequestBehavior.AllowGet);
            }

            var reportModel = new BookingReportModel
            {
                BookingId = booking.BookingId,
                PetName = booking.PetName,
                PetType = booking.PetType.ToString(),
                Breed = booking.Breed,
                OwnerName = booking.OwnerName,
                ServiceType = booking.ServiceType.ToString(),
                Email = booking.Email,
                PhoneNumber = booking.PhoneNumber,
                SpecialInstructions = booking.SpecialInstructions
            };

            Session["CurrentBooking"] = reportModel;

            return Json(reportModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult IndexTwo()
        {
            var groomersWithSessions = db.GroomingStaffs
                .Include(g => g.SparGroomings)
                .OrderBy(g => g.Groom_Name)
                .ToList();

            return View(groomersWithSessions);
        }


        // Existing CRUD actions below remain unchanged
        // GET: GroomingStaffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var groomingStaff = db.GroomingStaffs.Find(id);
            return groomingStaff == null
                ? (ActionResult)HttpNotFound()
                : View(groomingStaff);
        }

        // GET: GroomingStaffs/Create
        public ActionResult Create()
        {
            return View(new GroomingStaff());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroomStaffId,Groom_Name,Groom_Surname,Groom_Email")] GroomingStaff groomingStaff)
        {
            if (ModelState.IsValid)
            {
                db.GroomingStaffs.Add(groomingStaff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groomingStaff);
        }

        // GET: GroomingStaffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var groomingStaff = db.GroomingStaffs.Find(id);
            return groomingStaff == null
                ? (ActionResult)HttpNotFound()
                : View(groomingStaff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroomStaffId,Groom_Name,Groom_Surname,Groom_Email")] GroomingStaff groomingStaff)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groomingStaff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groomingStaff);
        }

        // GET: GroomingStaffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var groomingStaff = db.GroomingStaffs.Find(id);
            return groomingStaff == null
                ? (ActionResult)HttpNotFound()
                : View(groomingStaff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var groomingStaff = db.GroomingStaffs.Find(id);
            if (groomingStaff != null)
            {
                db.GroomingStaffs.Remove(groomingStaff);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

    // Add this class to your Models folder

}