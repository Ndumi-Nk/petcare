using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{

    public class AdminProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            var products = db.Products.ToList();
            // Log the products to inspect their values
            foreach (var product in products)
            {
                Debug.WriteLine($"Id: {product.ProductId}, Name: {product.Name}, Price: {product.Price}, Quantity: {product.Quantity}");
            }
            return View(products);
        }



        public ActionResult Create()
        {
            var model = new Product(); // Ensure a model is created and passed
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    // Ensure the directory exists
                    var directoryPath = Server.MapPath("~/Content/Images/Products");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(directoryPath, fileName);
                    Image.SaveAs(path);
                    product.ImageUrl = "~/Content/Images/Products/" + fileName;
                }

                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }
        public ActionResult Edit()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {

                if (Image != null && Image.ContentLength > 0)
                {
                    var directoryPath = Server.MapPath("~/Content/Images/Products");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var fileName = Path.GetFileName(Image.FileName);
                    var path = Path.Combine(directoryPath, fileName);
                    Image.SaveAs(path);


                    product.ImageUrl = "~/Content/Images/Products/" + fileName;
                }
                else
                {

                    db.Entry(product).Property(p => p.ImageUrl).IsModified = false;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
