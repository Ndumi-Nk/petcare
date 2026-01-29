using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class RatingsController : Controller
    {
        private ApplicationDbContext _context;

        // Parameterless constructor
        public RatingsController()
        {
            _context = new ApplicationDbContext(); // Create a new instance of the database context
        }

        // GET: Ratings/Create
        public ActionResult Create()
        {
            var model = new Rating();
            return View(model);
        }

        // POST: Ratings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Rating rating)
        {
            if (ModelState.IsValid)
            {
                _context.Ratings.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(rating); // Return the view with the model to show validation errors

        }

        // GET: Ratings
        public ActionResult Index()
        {
            var ratings = _context.Ratings.ToList();
            return View(ratings);
        }

        // Dispose method to clean up the context
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}