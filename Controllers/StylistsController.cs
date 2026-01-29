using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetCare_system.Models;

namespace PetCare_system.Controllers
{
    public class StylistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Stylists
        public ActionResult Index()
        {
            return View(db.Stylists.ToList());
        }

        // GET: Stylists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stylist stylist = db.Stylists.Find(id);
            if (stylist == null)
            {
                return HttpNotFound();
            }
            return View(stylist);
        }

        // GET: Stylists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stylists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StyleId,Name,PhoneNumber,Email")] Stylist stylist)
        {
            if (ModelState.IsValid)
            {
                db.Stylists.Add(stylist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stylist);
        }

        // GET: Stylists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stylist stylist = db.Stylists.Find(id);
            if (stylist == null)
            {
                return HttpNotFound();
            }
            return View(stylist);
        }

        // POST: Stylists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StyleId,Name,PhoneNumber,Email")] Stylist stylist)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stylist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stylist);
        }

        // GET: Stylists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stylist stylist = db.Stylists.Find(id);
            if (stylist == null)
            {
                return HttpNotFound();
            }
            return View(stylist);
        }

        // POST: Stylists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stylist stylist = db.Stylists.Find(id);
            db.Stylists.Remove(stylist);
            db.SaveChanges();
            return RedirectToAction("Index");
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
