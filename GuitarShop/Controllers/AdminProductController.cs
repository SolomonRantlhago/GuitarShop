using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GuitarShop.Models;
using GuitarShop.Data;
using GuitarShop.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GuitarShop.Controllers
{
    public class AdminProductController : Controller
    {
        private IEnumerable<Category> categories;
        private readonly IRepositoryWrapper _repo;
        private readonly IWebHostEnvironment _env;

        public AdminProductController(IRepositoryWrapper repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
            categories = _repo.Category.FindAll().OrderBy(c => c.CategoryName);
        }

        public IActionResult List(string id = "All")
        {
            IEnumerable<Product> products;
            if (id == "All")
            {
                products = _repo.Product.FindAll()
                    .OrderBy(p => p.ProductID);
            }
            else
            {
                products = _repo.Product.GetAllProductsWithCategoryDetails()
                    .Where(p => p.Category.CategoryName == id)
                    .OrderBy(p => p.ProductID);
            }

            var model = new ProductListViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategory = id
            };

            // bind products to view
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // create new Product object
            Product product = new();                // create Product object
            product.Category = _repo.Category.GetById(1);  // add Category object - prevents validation problem

            // use ViewBag to pass action and category data to view
            ViewBag.Action = "Add";
            ViewBag.Categories = categories;

            // bind product to AddUpdate view
            return View("AddUpdate", product);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            // get Product object for specified primary key
            Product product = _repo.Product.GetById(id);

            // use ViewBag to pass action and category data to view
            ViewBag.Action = "Update";
            ViewBag.Categories = categories;

            // bind product to AddUpdate view
            return View("AddUpdate", product);
        }

        [HttpPost]
        public IActionResult Update(Product product, IFormFile image)
        {
            // validate the uploaded picture (if any)
            if (image != null && image.Length > 0)
            {
                string[] allowed = { ".png", ".jpg", ".jpeg", ".webp" };
                string ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                    ModelState.AddModelError("", "Picture must be a .png, .jpg, .jpeg or .webp file.");
                else if (image.Length > 2 * 1024 * 1024)
                    ModelState.AddModelError("", "Picture must be smaller than 2 MB.");
            }

            if (ModelState.IsValid)
            {
                if (product.ProductID == 0)           // new product
                {
                    _repo.Product.Create(product);
                    TempData["Message"] = $"{product.Name} has been added";
                }
                else                                  // existing product
                {
                    _repo.Product.Update(product);
                    TempData["Message"] = $"{product.Name} has been updated";
                }
                _repo.Save();

                if (image != null && image.Length > 0)
                {
                    // images are looked up by product code: {code}_s / _m / _l .png
                    string code = Path.GetFileName(product.Code);   // strips any path characters
                    string folder = Path.Combine(_env.WebRootPath, "images");
                    foreach (string size in new[] { "_s", "_m", "_l" })
                    {
                        using var input = image.OpenReadStream();
                        using var output = new FileStream(
                            Path.Combine(folder, code + size + ".png"), FileMode.Create);
                        input.CopyTo(output);
                    }
                }
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = product.ProductID == 0 ? "Add" : "Update";
                ViewBag.Categories = categories;
                return View("AddUpdate", product);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Product product = _repo.Product.GetById(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(Product product)
        {
            _repo.Product.Delete(product);
            TempData["Message"] = $"{product.Name} has been deleted";
            _repo.Save();
            return RedirectToAction("List");
        }
    }
}
