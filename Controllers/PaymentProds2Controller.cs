using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity; // For User.Identity.GetUserId()

namespace BARBERSHOP.Controllers
{
    public class PaymentProds2Controller : Controller
    {
        private readonly ApplicationDbContext db;

        public PaymentProds2Controller()
        {
            db = new ApplicationDbContext();
        }

        // GET: Payment
        public ActionResult Index()
        {
            var payment = new PaymentProd();
            var cart = GetCart();

            if (cart == null || !cart.Any())
            {
                Debug.WriteLine("Cart is empty on GET.");
                payment.Amount = 0;
                payment.CartItems = "";
            }
            else
            {
                payment.Amount = CalculateCartTotal(cart);
                payment.CartItems = string.Join(",", cart.Select(item => $"{item.Name}(Quantity: {item.Quantity}, Price: {item.Price})"));
                Debug.WriteLine($"Cart total on GET: {payment.Amount}");
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPayment(PaymentProd payment)
        {
            var cart = GetCart();
            if (cart == null || !cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View("Index", payment);
            }

            if (!ModelState.IsValid)
            {
                payment.Amount = CalculateCartTotal(cart);
                return View("Index", payment);
            }

            payment.Amount = CalculateCartTotal(cart);
            payment.CreatedAt = DateTime.Now;
            payment.CartItems = string.Join(",", cart.Select(item => $"{item.Name}(Quantity: {item.Quantity}, Price: {item.Price})"));

            if (payment.IsDelivery && string.IsNullOrEmpty(payment.DeliveryAddress))
            {
                ModelState.AddModelError("DeliveryAddress", "Delivery address is required.");
                return View("Index", payment);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Save payment
                    db.PaymentProds.Add(payment);
                    db.SaveChanges();

                    // Update product quantities
                    foreach (var item in cart)
                    {
                        var product = db.Products.Find(item.ProductId);
                        if (product != null)
                        {
                            product.Quantity -= item.Quantity;
                            db.Entry(product).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();

                    // Assign Ecommerce driver if delivery is requested
                    if (payment.IsDelivery)
                    {
                        var availableDriver = db.Drivers.FirstOrDefault(d =>
                            d.Driverstatus == "Available" &&
                            d.DeliveryStatus == "Pending" &&
                            d.DriverType == "Ecommerce"

                        );

                        if (availableDriver != null)
                        {
                            availableDriver.DeliveryStatus = "Assigned";
                            availableDriver.Driverstatus = "Booked";
                            availableDriver.Destination = payment.DeliveryAddress;
                            availableDriver.ProductId = string.Join(",", cart.Select(c => c.ProductId));



                            // Add all product IDs to the driver (for multiple items)
                            availableDriver.ProductId = string.Join(",", cart.Select(c => c.ProductId));

                            availableDriver.Userr_Id = User.Identity.GetUserId();
                            db.Entry(availableDriver).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "An error occurred while processing your payment.");
                    return View("Index", payment);
                }
            }

            ClearCart();
            return RedirectToAction("Receipt", new { paymentId = payment.PaymentProdId });
        }

        public ActionResult Receipt(int paymentId)
        {
            var payment = db.PaymentProds.Find(paymentId);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // Helper: Fetch cart
        private List<Product> GetCart()
        {
            var cart = Session["Cart"] as List<Product>;
            if (cart == null || !cart.Any())
            {
                Debug.WriteLine("Cart is null or empty in GetCart().");
            }
            return cart ?? new List<Product>();
        }

        // Helper: Clear cart
        private void ClearCart()
        {
            Session.Remove("Cart");
            Session["CartItemCount"] = 0;
            Debug.WriteLine("Cart cleared.");
        }

        // Helper: Calculate total
        private decimal CalculateCartTotal(List<Product> cart)
        {
            if (cart == null || !cart.Any())
            {
                Debug.WriteLine("Cart is null or empty in CalculateCartTotal().");
                return 0;
            }

            decimal total = 0;
            foreach (var item in cart)
            {
                Debug.WriteLine($"Product: {item?.Name ?? "null"}, Price: {item?.Price}, Quantity: {item?.Quantity}");
                total += item.Price * item.Quantity;
            }
            Debug.WriteLine($"Calculated cart total: {total}");
            return total;
        }
    }
}