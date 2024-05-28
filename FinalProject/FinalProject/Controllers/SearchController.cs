using FinalProject.Data;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment environment;

        public SearchController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            this.environment = environment;
        }
        public IActionResult Search()
        {
            return View();
        }

        // POST: Product/Search
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                TempData["ErrorMessage"] = "Please enter a search term.";
                return RedirectToAction("Index", "Home");
            }

            var products = _dbContext.products
                                     .Where(p => p.Name.Contains(searchTerm))
                                     .ToList();

            if (products == null || products.Count == 0)
            {
                TempData["ErrorMessage"] = "No products found.";
                return RedirectToAction("Index", "Home");
            }

            return View("Search", products);
        }
    }
}
