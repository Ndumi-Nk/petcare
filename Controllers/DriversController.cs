using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PetCare_system.Models;

namespace PetCare_system.Controllers
{
    public class DriversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ===============================
        // DRIVER LIST
        // ===============================
        public ActionResult AddDriver()
        {
            return View(new Driver());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDriver(Driver model)
        {
            if (ModelState.IsValid)
            {
                // Hash the password before saving
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Optionally set default values
                model.Driverstatus = "Available";
                model.DeliveryStatus = "NotStarted";
                model.CarInfo = model.CarInfo ?? "N/A";
                model.Destination = model.Destination ?? "Pending";

                db.Drivers.Add(model);
                db.SaveChanges();

                TempData["Success"] = "Driver added successfully!";
          


        // Email credentials
        string emailFrom = "shezielihle186@gmail.com";
                string emailPassword = "xjop iuut owdu loav"; // Replace with a secure storage for credentials

                // Send email to the doctor
                try
                {
                    using (MailMessage mm = new MailMessage(emailFrom, model.Email))
                    {
                        mm.Subject = "Welcome to PetCare Systems";
                        mm.Body = $"Dear {model.DriverName},\n\n" +
                                  "Your account has been created successfully.\n\n" +
                                  "Login Details:\n" +
                                  $"Email: {model.Email}\n" +
                                  $"Password: {model.Password}\n\n" +
                                  "Please change your password after logging in.\n\n" +
                                  "Best regards,\n" +
                                  "PetCare Systems";

                        mm.IsBodyHtml = false;

                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com"; // Gmail SMTP server
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                            smtp.Port = 587; // Standard Gmail SMTP port

                            smtp.Send(mm);
                        }
                    }

                    // Confirmation feedback
                    TempData["Success"] = "Doctor added successfully! Login details sent via email.";
                }
                catch (SmtpException ex)
                {
                    // Handle email sending errors
                    TempData["Error"] = "Doctor was added, but the email could not be sent. Error: " + ex.Message;
                }
            }
            else
            {
                // Handle validation errors
                return View(model);
            }

            return RedirectToAction("Index");
        }

    

        public ActionResult Index()
        {
            return View(db.Drivers.ToList());
        }
        public ActionResult UserTracking()
        {
            return View(db.Drivers.ToList());
        }
        // ===============================
        // DRIVER DETAILS
        // ===============================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Driver driver = db.Drivers.Find(id);
            if (driver == null)
                return HttpNotFound();

            return View(driver);
        }

        // ===============================
        // CREATE DRIVER
        // ===============================
        public ActionResult Create()
        {
            ViewBag.RequestId = new SelectList(
                db.EmergencyRequestTransports.Where(r => r.Status == "Pending"),
                "RequestId",
                "EmergencyDescription"
            );
            return View();
        }
        public ActionResult EcommerceDashboard()
        {
            if (Session["DriverId"] == null)
                return RedirectToAction("Login");

            int driverId = (int)Session["DriverId"];

            // Fetch bookings or deliveries assigned to this driver
            // Change below to real model once integrated (e.g., Orders, Deliveries)
            var driverDeliveries = db.Drivers
                                     .Where(d => d.DriverId == driverId)
                                     .ToList();

            ViewBag.DriverName = Session["DriverName"];

            return View(driverDeliveries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Driver driver)
        {
            // Set defaults for hidden fields
            driver.Latitude = null;
            driver.Longitude = null;
            driver.Destination = null;
            driver.Driverstatus = "Available";
            driver.DeliveryStatus = "Pending";
            driver.RequestId = null;

            // Remove validation errors for hidden fields
            ModelState.Remove("Latitude");
            ModelState.Remove("Longitude");
            ModelState.Remove("Destination");
            ModelState.Remove("Driverstatus");
            ModelState.Remove("DeliveryStatus");
            ModelState.Remove("RequestId");

            if (ModelState.IsValid)
            {
                db.Drivers.Add(driver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(driver);
        }

        // ===============================
        // EDIT DRIVER
        // ===============================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Driver driver = db.Drivers.Find(id);
            if (driver == null)
                return HttpNotFound();

            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DriverId,DriverName,DriverType,DriverSurname,Username,Password,Latitude,Longitude,License,CarInfo,Destination,Driverstatus,DeliveryStatus,Order_Id,Userr_Id,Pet_Id,RequestId")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(driver);
        }

        // ===============================
        // DELETE DRIVER
        // ===============================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Driver driver = db.Drivers.Find(id);
            if (driver == null)
                return HttpNotFound();

            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Driver driver = db.Drivers.Find(id);
            db.Drivers.Remove(driver);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ===============================
        // DRIVER LOGIN / LOGOUT
        // ===============================
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Driver driver)
        {
            var foundDriver = db.Drivers
                .FirstOrDefault(d => d.Username == driver.Username && d.Password == driver.Password);

            if (foundDriver != null)
            {
                Session["DriverId"] = foundDriver.DriverId;
                Session["DriverName"] = foundDriver.DriverName;
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password.";
            return View(driver);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // ===============================
        // DRIVER DASHBOARDS
        // ===============================
        public ActionResult Dashboard()
        {
            if (Session["DriverId"] == null)
                return RedirectToAction("Login");

            int driverId = (int)Session["DriverId"];

            var driverDeliveries = db.Drivers
                .Where(d => d.DriverId == driverId)
                .ToList();

            // Example: map to ViewModel if needed
            var vm = driverDeliveries.Select(d => new DriverDashboardVM
            {
                Driver = d,
                PetName = (d.Pet_Id != null
                    ? db.pets.Where(p => p.Id.ToString() == d.Pet_Id)
                             .Select(p => p.Name)
                             .FirstOrDefault()
                    : null)
            }).ToList();

            ViewBag.DriverName = Session["DriverName"];
            return View(vm);
        }

        public ActionResult ElihleDashBoard()
        {
            if (Session["DriverId"] == null)
                return RedirectToAction("Login");

            int driverId = (int)Session["DriverId"];

            var driverDeliveries = db.Drivers
                                     .Where(d => d.DriverId == driverId)
                                     .ToList();

            ViewBag.DriverName = Session["DriverName"];
            return View(driverDeliveries);
        }

        // ===============================
        // LOCATION + STATUS
        // ===============================
        public JsonResult GetDriverLocation(int id)
        {
            var driver = db.Drivers.Find(id);
            if (driver == null) return Json(null, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                Latitude = driver.Latitude,
                Longitude = driver.Longitude
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateDeliveryStatus(int driverId, string status)
        {
            var driver = db.Drivers.Find(driverId);
            if (driver == null)
            {
                return HttpNotFound();
            }

            if (driver.DriverType == "Ecommerce")
            {
                driver.DeliveryStatus = status;
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();

                // === Send Email Notification ===
                string emailFrom = "shezielihle186@gmail.com";
                string emailPassword = "xjop iuut owdu loav"; // ⚠ secure this in production

                try
                {
                    using (MailMessage mm = new MailMessage(emailFrom, driver.Username))
                    {
                        mm.Subject = "Delivery Status Update - PetCare Systems";
                        mm.Body = $"Dear {driver.DriverName},\n\n" +
                                  $"Your delivery status has been updated to: {driver.DeliveryStatus}.\n\n" +
                                  "Please check your account for more details.\n\n" +
                                  "Best regards,\n" +
                                  "PetCare Systems";

                        mm.IsBodyHtml = false;

                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                            smtp.Port = 587;

                            smtp.Send(mm);
                        }
                    }

                    TempData["Success"] = "Delivery status updated and email sent successfully.";
                }
                catch (SmtpException ex)
                {
                    TempData["Error"] = "Delivery status updated, but email could not be sent. Error: " + ex.Message;
                }
            }

            return RedirectToAction("ElihleDashBoard");
        }

        [HttpPost]
        public ActionResult StartDriving(int driverId)
        {
            var driver = db.Drivers.Find(driverId);
            if (driver != null)
            {
                driver.Driverstatus = "Driving to Destination";
                db.SaveChanges();
            }

            return RedirectToAction("ElihleDashBoard");
        }

        [HttpPost]
        public ActionResult PatientDelivered(int driverId)
        {
            var driver = db.Drivers.Find(driverId);
            if (driver != null)
            {
                driver.Driverstatus = "Available";
                db.SaveChanges();
            }

            return RedirectToAction("ElihleDashBoard");
        }

        // ===============================
        // CLEANUP
        // ===============================
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
