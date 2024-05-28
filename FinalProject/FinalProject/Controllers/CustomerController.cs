using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistrationViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegistrationViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_dbContext.Customers.Any(c => c.Email == customerViewModel.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(customerViewModel);
                }

                var validationResult = new ValidateImageAttribute().GetValidationResult(customerViewModel.cust_Pic, new ValidationContext(customerViewModel));
                if (validationResult != ValidationResult.Success)
                {
                    ModelState.AddModelError(nameof(customerViewModel.cust_Pic), validationResult.ErrorMessage);
                    return View(customerViewModel);
                }

                var customer = new Customer
                {
                    Name = customerViewModel.Name,
                    Address = customerViewModel.Address,
                    PhoneNumber = customerViewModel.PhoneNumber,
                    Email = customerViewModel.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(customerViewModel.Password),
                    ConfirmationPassword = BCrypt.Net.BCrypt.HashPassword(customerViewModel.Password),
                    cust_Pic = customerViewModel.cust_Pic
                };

                _dbContext.Customers.Add(customer);
                _dbContext.SaveChanges();
                return RedirectToAction("Login", "Customer");
            }
            return View(customerViewModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Customers.FirstOrDefault(u => u.Email == model.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    HttpContext.Session.SetInt32("userid", user.CustomerID);
                    HttpContext.Session.SetString("CustomerName", user.Name);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or Password");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult RemoveCart(int id)
        {
            var or = _dbContext.cartItems.Find(id);
            if (or == null)
            {
                return RedirectToAction("CustomerCartHistory", "Customer");
            }
            _dbContext.cartItems.Remove(or);
            _dbContext.SaveChanges(true);
            return RedirectToAction("CustomerCartHistory", "Customer");
        }

        public IActionResult CustomerCartHistory()
        {
            var customerId = HttpContext.Session.GetInt32("userid");

            if (customerId == null)
            {
                TempData["ErrorMessage"] = "Please log in to view your cart history.";
                return RedirectToAction("Login", "Customer");
            }

            var cartItems = _dbContext.cartItems
                .Where(ci => ci.CustomerID == customerId)
                .ToList();

            return View(cartItems);
        }
    }
}
