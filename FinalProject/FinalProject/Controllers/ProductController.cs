using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;
using System.Threading.Tasks;
using System.Linq;
using FinalProject.Data;
using Microsoft.AspNetCore.Authorization;
namespace FinalProject.Controllers

{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment environment;

        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            this.environment = environment;
        }
        // GET: Product
        public IActionResult Index()
        {
            var products = _dbContext.products.ToList();
            return View(products);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public IActionResult Create(ProductDto productdto)
        {
            if (productdto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image fie is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productdto);
            }
            //save image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productdto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath)) { productdto.ImageFile.CopyTo(stream); }
            //save the new product in DB
            Product product = new Product()
            {
                Name = productdto.Name,
                PriceInEGP = productdto.PriceInEGP,
                Description = productdto.Description,
                Image = newFileName,
                Type = productdto.Type
            };
            _dbContext.products.Add(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Edit(int id)
        {
            var product = _dbContext.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            ProductDto productdto = new ProductDto()
            {
                Name = product.Name,
                PriceInEGP = product.PriceInEGP,
                Description = product.Description,
                Type = product.Type
            };
            ViewData["ProductId"] = product.ProductID;
            ViewData["Image"] = product.Image;
            return View(productdto);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productdto)
        {
            var product = _dbContext.products.Find(id);
            if (product == null)
            { return RedirectToAction("Index", "Product"); }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.ProductID;
                ViewData["Image"] = product.Image;
                return View(productdto);
            }
            //update image file if we have a new one
            string newFileName = product.Name;
            if (productdto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productdto.ImageFile!.FileName);
                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath)) { productdto.ImageFile.CopyTo(stream); }
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.Image;
                if (imageFullPath != oldImageFullPath)
                { System.IO.File.Delete(oldImageFullPath); }

            }
            //update product in DB
            product.Name = productdto.Name;
            product.PriceInEGP = productdto.PriceInEGP;
            product.Image = newFileName;
            product.Description = productdto.Description;
            product.Type = productdto.Type;
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Product");
        }
        public IActionResult Delete(int id)
        {
            var product = _dbContext.products.Find(id);
            if (product == null)
            { return RedirectToAction("Index", "Product"); }
            string imageFullPath = environment.WebRootPath + "/products/" + product.Image;
            System.IO.File.Delete(imageFullPath);
            _dbContext.products.Remove(product);
            _dbContext.SaveChanges(true);
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Details(int id)
        {
            var product = _dbContext.products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }
            return View(product);
        }
    }
}