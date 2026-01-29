using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PetCare_system.Models;

namespace PetCare_system.Controllers
{
    public class PaymentForTrainingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PaymentForTraining
        public ActionResult Index()
        {
            var payments = db.paymentForTraining
                             .Include(p => p.TrainingSession)
                             .Include(p => p.User);
            return View(payments.ToList());
        }

        // GET: PaymentForTraining/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var payment = db.paymentForTraining
                            .Include(p => p.TrainingSession)
                            .Include(p => p.User)
                            .FirstOrDefault(p => p.Payment_Id == id);

            if (payment == null) return HttpNotFound();

            return View(payment);
        }

        // GET: PaymentForTraining/Create
        // GET: PaymentForTraining/Create
        public ActionResult Create(int sessionId)
        {
            var userId = User.Identity.GetUserId();

            var session = db.TrainingSessions
                .Include(ts => ts.Pet)
                .FirstOrDefault(ts => ts.Id == sessionId && ts.Pet.UserId == userId);

            if (session == null)
            {
                return HttpNotFound();
            }

            // Pre-fill payment info
            var payment = new PaymentForTraining
            {
                TrainingSessionId = session.Id,
                PetName = session.Pet.Name,
                Amount = session.Price,
                UserId = userId,
                PaymentDate = DateTime.Now,
                Status = "Pending"
            };

            return View(payment);
        }


        // POST: PaymentForTraining/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentForTraining payment)
        {
            if (ModelState.IsValid)
            {
                db.paymentForTraining.Add(payment);
                db.SaveChanges();
                return RedirectToAction("Success", new { id = payment.Payment_Id });
            }

            ViewBag.TrainingSessionId = new SelectList(db.TrainingSessions, "Id", "TrainingType", payment.TrainingSessionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", payment.UserId);
            return View(payment);
        }

        public ActionResult Success(int id)
        {
            var payment = db.paymentForTraining
                           .Include("TrainingSession")
                           .Include("User")
                           .FirstOrDefault(p => p.Payment_Id == id);

            if (payment == null) return HttpNotFound();

            return View(payment);
        }


        // GET: PaymentForTraining/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var payment = db.paymentForTraining.Find(id);
            if (payment == null) return HttpNotFound();

            ViewBag.TrainingSessionId = new SelectList(db.TrainingSessions, "Id", "TrainingType", payment.TrainingSessionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", payment.UserId);
            return View(payment);
        }

        // POST: PaymentForTraining/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Payment_Id,TrainingSessionId,Amount,PetName,PaymentDate,PaymentMethod,BankType,CardHolderName,AccountNumber,CVV,ExpiryDate,Status,TransactionId,UserId")] PaymentForTraining payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TrainingSessionId = new SelectList(db.TrainingSessions, "Id", "TrainingType", payment.TrainingSessionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", payment.UserId);
            return View(payment);
        }

        // GET: PaymentForTraining/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var payment = db.paymentForTraining
                            .Include(p => p.TrainingSession)
                            .Include(p => p.User)
                            .FirstOrDefault(p => p.Payment_Id == id);

            if (payment == null) return HttpNotFound();

            return View(payment);
        }

        // POST: PaymentForTraining/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var payment = db.paymentForTraining.Find(id);
            db.paymentForTraining.Remove(payment);
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
