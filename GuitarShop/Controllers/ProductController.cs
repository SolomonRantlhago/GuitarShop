using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GuitarShop.Models;
using GuitarShop.Models.ViewModels;
using GuitarShop.Data;

namespace GuitarShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepositoryWrapper _repo;
        public ProductController(IRepositoryWrapper repo)
        {
            _repo = repo;
        }

        public IActionResult List(string id = "All")
        {
            List<Product> products;

            if (id == "All")
            {
                products = _repo.Product.FindAll()
                    .OrderBy(p => p.ProductID).ToList();
            }
            else if (id == "Strings")
            {
                products = _repo.Product.GetAllProductsWithCategoryDetails()
                           .Where(p => p.Category.CategoryName == "Guitars" || p.Category.CategoryName == "Basses")
                           .OrderBy(p => p.ProductID).ToList();
            }
            else
            {
                products = _repo.Product.GetAllProductsInCategory(id)
                    .OrderBy(p => p.ProductID).ToList();
            }

            var categories = _repo.Category.FindAll()
                              .OrderBy(g => g.CategoryName);

            var model = new ProductListViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategory = id
            };

            // bind products to view
            return View(model);
        }

        public IActionResult Details(int id)
        {
            Product product = _repo.Product.GetById(id);
            if (product != null)
            {
                // use ViewBag to pass data to view
                ViewBag.ImageFilename = product.Code + "_m.png";

                // bind product to view
                return View(product);
            }
            else
                return RedirectToAction("List");
        }

    }
}