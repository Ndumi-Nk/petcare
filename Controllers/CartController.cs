using PetCare_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetCare_system.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;

        public CartController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Validate that the requested quantity does not exceed available stock
            if (quantity < 1 || quantity > product.Quantity)
            {
                ModelState.AddModelError("", "Invalid quantity selected.");
                return RedirectToAction("Index", "Products");
            }

            var cart = GetCart();

            // Check if the product is already in the cart
            var existingProduct = cart.FirstOrDefault(p => p.ProductId == productId);
            if (existingProduct != null)
            {
                // Update the quantity of the existing product
                existingProduct.Quantity += quantity;
            }
            else
            {
                // Add new product to the cart with the specified quantity
                product.Quantity = quantity; // Assuming you have a property to hold quantity
                cart.Add(product);
            }

            SaveCart(cart);

            // Update cart item count in session
            if (Session["CartItemCount"] == null)
            {
                Session["CartItemCount"] = quantity;
            }
            else
            {
                Session["CartItemCount"] = (int)Session["CartItemCount"] + quantity;
            }

            return RedirectToAction("Index", "Products");
        }


        private List<Product> GetCart()
        {
            return Session["Cart"] as List<Product> ?? new List<Product>();
        }

        private void SaveCart(List<Product> cart)
        {
            Session["Cart"] = cart;
        }

        private void ClearCart()
        {
            Session.Remove("Cart");
        }
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();

            // Find the product to remove
            var productToRemove = cart.FirstOrDefault(p => p.ProductId == productId);
            if (productToRemove != null)
            {
                // Remove all instances of the product from the cart
                cart.RemoveAll(p => p.ProductId == productId);

                // Update cart item count in session
                UpdateCartItemCount(cart);

                SaveCart(cart);
            }

            return RedirectToAction("Index", "Cart");
        }

        private void UpdateCartItemCount(List<Product> cart)
        {
            // Update the cart item count in session
            Session["CartItemCount"] = cart.Sum(p => p.Quantity);
        }
    }
}