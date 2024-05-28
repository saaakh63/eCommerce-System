using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class AddToCartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment environment;

        public AddToCartController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            this.environment = environment;
        }
        public IActionResult AddToCart(int id, int q = 1)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();
            CartItem Item1 = cart.FirstOrDefault(i => i.ProductID == id);
            if (Item1 != null)
            {
                Item1.quintity += q;
            }
            else
            {
                Product p = _dbContext.products.FirstOrDefault(i => i.ProductID == id);
                CartItem newitem = new CartItem
                {
                    ProductID = p.ProductID,
                    productname = p.Name,
                    producprice = (int)p.PriceInEGP,
                    quintity = q
                };
                cart.Add(newitem);
            }
            HttpContext.Session.Set("cart", cart);
            return RedirectToAction("Cart", "AddToCart");
        }
        public IActionResult Cart()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerID");
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();


            return View(cart);
        }
        public IActionResult Removecart(int id)
        {

            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            CartItem citem = cart.Find(i => i.Id == id);
            cart.Remove(citem);
            HttpContext.Session.Set("cart", cart);
            return RedirectToAction("Cart", "AddToCart");
        }


        public IActionResult Buyit(int id)
        {
            // Get the customer ID from session
            var customerId = HttpContext.Session.GetInt32("userid");

            if (customerId == null)
            {
              
                TempData["ErrorMessage"] = "Please log in to complete your purchase.";
                return RedirectToAction("Login", "Customer"); 
            }

            // Get the customer's name (you may need to adjust this based on your application)
            var customer = _dbContext.Customers.FirstOrDefault(c => c.CustomerID == customerId);
            string customerName = customer != null ? customer.Name : "Customer";

            // Get the cart from session
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            // Find the cart item and remove it
            CartItem citem = cart.Find(i => i.Id == id);
            if (citem != null)
            {
                _dbContext.cartItems.Add(citem);
                _dbContext.SaveChanges();

                cart.Remove(citem);
                HttpContext.Session.Set("cart", cart);
            }

            // Pass the customer's name to the view using TempData to avoid issues with ViewData in Redirect
            TempData["CustomerName"] = customerName;

            return RedirectToAction("Buy", "AddToCart");
        }


        public IActionResult Buy()
        {
            // Get the customer's name from TempData
            var customerName = TempData["CustomerName"] as string;

            // If customerName is null, redirect to an error page or handle the error appropriately
            if (string.IsNullOrEmpty(customerName))
            {
                TempData["ErrorMessage"] = "An error occurred while processing your order.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["CustomerName"] = customerName;
            return View();
        }
        [HttpGet]
        public IActionResult checkout()
        {
            var cartItems = _dbContext.cartItems.ToList(); 
            return View(cartItems);
        }
        [HttpPost]
        public IActionResult Checkout()
        {
            var customerId = HttpContext.Session.GetInt32("userid");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();
            foreach (CartItem item in cart)
            {
                var enrol = new CartItem
                {
                    ProductID = item.ProductID,
                    producprice=item.producprice,
                    productname=item.productname,
                    CustomerID =(int)customerId,
                     quintity = item.quintity
                };
                _dbContext.cartItems.Add(enrol);
                _dbContext.SaveChanges();

            }
            HttpContext.Session.Remove("cart");
            return RedirectToAction("Index", "Home");
        }


    }
}
