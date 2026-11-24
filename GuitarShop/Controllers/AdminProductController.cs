using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GuitarShop.Models;
using GuitarShop.Data;
using GuitarShop.Models.ViewModels;

namespace GuitarShop.Controllers
{
    public class AdminProductController : Controller
    {
        private IEnumerable<Category> categories;
        private readonly IRepositoryWrapper _repo;

        public AdminProductController(IRepositoryWrapper repo)
        {
            _repo = repo;
            categories = _repo.Category.FindAll()
                .OrderBy(c => c.CategoryName);
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
        public IActionResult Update(Product product)
        {
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
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = "Save";
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
