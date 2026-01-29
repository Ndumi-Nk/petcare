using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using PetCare_system.Models;
namespace PetCare_system.Controllers
{
    public class EmergencyRequestTransportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmergencyRequestTransports
        public ActionResult Index()
        {
            var requests = db.EmergencyRequestTransports.Include(e => e.Pet).ToList();
            return View(requests);
        }

        // GET: EmergencyRequestTransports/Create
        public ActionResult Create()
        {
            var userId = Session["LoggedInUserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var pets = db.pets
                         .Where(p => p.UserId == userId)
                         .Select(p => new { p.Id, p.Name })
                         .ToList();

            ViewBag.PetId = new SelectList(pets, "Id", "Name");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmergencyRequestTransport model)
        {
            var userId = Session["LoggedInUserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                model.UserId = userId;
                model.RequestTime = DateTime.Now;
                model.Status = "Pending";

                db.EmergencyRequestTransports.Add(model);
                db.SaveChanges();

                // ========================
                // ASSIGN DRIVER AUTOMATICALLY
                // ========================
                var availableDriver = db.Drivers.FirstOrDefault(d => d.Driverstatus == "Available");
                if (availableDriver != null)
                {
                    double latitude = double.Parse(model.PickupLatitude, CultureInfo.InvariantCulture);
                    double longitude = double.Parse(model.PickupLongitude, CultureInfo.InvariantCulture);

                    string destination = await GeoHelper.GetAddressFromCoordinatesAsync(latitude, longitude);

                    // Verify the pet belongs to this user
                    var pet = db.pets.FirstOrDefault(p => p.Id == model.PetId && p.UserId == userId);
                    if (pet != null)
                    {
                        availableDriver.Pet_Id = pet.Id.ToString();  // ✅ Assign PetId here
                    }

                    // Assign driver to this request
                    availableDriver.RequestId = model.RequestId.ToString();
                    availableDriver.Latitude = model.PickupLatitude.ToString(CultureInfo.InvariantCulture);
                    availableDriver.Longitude = model.PickupLongitude.ToString(CultureInfo.InvariantCulture);
                    availableDriver.Destination = destination;  // human-readable location
                    availableDriver.Driverstatus = "Booked";
                    availableDriver.DeliveryStatus = "On The Way";
                    availableDriver.Userr_Id = userId;

                    // Update request status
                    model.Status = "Assigned";
                    db.Entry(model).State = EntityState.Modified;

                    db.SaveChanges();
                }


                return RedirectToAction("Index");
            }

            // Re-populate dropdown if validation fails
            var pets = db.pets.Where(p => p.UserId == userId)
                              .Select(p => new { p.Id, p.Name })
                              .ToList();
            ViewBag.PetId = new SelectList(pets, "Id", "Name", model.PetId);

            return View(model);
        }

        // GET: EmergencyRequestTransports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.EmergencyRequestTransports.Include(e => e.Pet)
                                                       .FirstOrDefault(r => r.RequestId == id);
            if (request == null) return HttpNotFound();

            return View(request);
        }

        // GET: EmergencyRequestTransports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.EmergencyRequestTransports.Find(id);
            if (request == null) return HttpNotFound();

            return View(request);
        }

        // POST: EmergencyRequestTransports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequestId,PickupLatitude,PickupLongitude,EmergencyTypes,EmergencyDescription,RequestTime,Status,PetId")] EmergencyRequestTransport request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // GET: EmergencyRequestTransports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var request = db.EmergencyRequestTransports.Find(id);
            if (request == null) return HttpNotFound();

            return View(request);
        }

        // POST: EmergencyRequestTransports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var request = db.EmergencyRequestTransports.Find(id);
            db.EmergencyRequestTransports.Remove(request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}