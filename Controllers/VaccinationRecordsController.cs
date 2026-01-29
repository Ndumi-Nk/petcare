using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class VaccinationRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VaccinationRecords
        public ActionResult Index()
        {
            var vaccinations = db.VaccinationRecords.Include(v => v.Pet).ToList();
            return View(vaccinations);
        }
        public ActionResult Index2()
        {
            string loggedUserId = Session["LoggedInUserId"].ToString();

            if (loggedUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var vaccinations = db.VaccinationRecords
                                 .Include(v => v.Pet)
                                 .Where(v => v.Pet.UserId == loggedUserId)
                                 .ToList();

            return View(vaccinations);
        }




        // GET: VaccinationRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VaccinationRecord vaccinationRecord = db.VaccinationRecords.Find(id);
            if (vaccinationRecord == null)
            {
                return HttpNotFound();
            }
            return View(vaccinationRecord);
        }

        // GET: VaccinationRecords/Create
        public ActionResult Create()
        {
            // Get all pets along with their owner name
            var petsWithOwners = db.pets
                .Select(p => new
                {
                    p.Id,
                    PetName = p.Name,
                    OwnerName = p.User.UserName ?? "No Owner"  // adjust according to your User model field
                })
                .ToList();

            // Group pets by owner
            var groupedPets = petsWithOwners
                .GroupBy(p => p.OwnerName)
                .Select(g => new SelectListGroup { Name = g.Key })
                .ToList();

            // Build SelectListItem list with groups
            var petItems = new List<SelectListItem>();
            foreach (var group in groupedPets)
            {
                var petsInGroup = petsWithOwners.Where(p => p.OwnerName == group.Name);
                foreach (var pet in petsInGroup)
                {
                    petItems.Add(new SelectListItem
                    {
                        Text = pet.PetName,
                        Value = pet.Id.ToString(),
                        Group = group
                    });
                }
            }

            ViewBag.PetId = petItems;

            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VaccinationId,VaccineType,VaccineName,DateGiven,NextDueDate,Notes,PetId")] VaccinationRecord vaccinationRecord)
        {
            if (ModelState.IsValid)
            {
                db.VaccinationRecords.Add(vaccinationRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            

            var userId = Session["LoggedInUserId"]?.ToString();
            ViewBag.PetId = db.pets
                .Where(p => p.UserId == userId)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            return View(vaccinationRecord);
        }

        // GET: VaccinationRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VaccinationRecord vaccinationRecord = db.VaccinationRecords.Find(id);
            if (vaccinationRecord == null)
            {
                return HttpNotFound();
            }
            return View(vaccinationRecord);
        }

        // POST: VaccinationRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VaccinationId,VaccineName,DateGiven,NextDueDate,Notes")] VaccinationRecord vaccinationRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vaccinationRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vaccinationRecord);
        }

        // GET: VaccinationRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VaccinationRecord vaccinationRecord = db.VaccinationRecords.Find(id);
            if (vaccinationRecord == null)
            {
                return HttpNotFound();
            }
            return View(vaccinationRecord);
        }

        // POST: VaccinationRecords/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VaccinationRecord vaccinationRecord = db.VaccinationRecords.Find(id);
            db.VaccinationRecords.Remove(vaccinationRecord);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateField(VaccinationUpdateModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.Field))
                return new HttpStatusCodeResult(400, "Invalid request");

            var record = db.VaccinationRecords.FirstOrDefault(v => v.VaccinationId == request.Id);
            if (record == null)
                return HttpNotFound("Record not found");

            try
            {
                switch (request.Field)
                {
                    case "VaccineName":
                        record.VaccineName = request.Value;
                        break;

                    case "VaccineType":
                        record.VaccineType = request.Value;
                        break;

                    case "DateGiven":
                        if (DateTime.TryParse(request.Value, out var givenDate))
                            record.DateGiven = givenDate;
                        break;

                    case "NextDueDate":
                        if (DateTime.TryParse(request.Value, out var nextDate))
                            record.NextDueDate = nextDate;
                        else
                            record.NextDueDate = null;
                        break;

                    case "Notes":
                        record.Notes = request.Value;
                        break;

                    default:
                        return new HttpStatusCodeResult(400, $"Unknown field: {request.Field}");
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, $"Error: {ex.Message}");
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
