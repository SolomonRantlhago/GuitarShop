using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GuitarShop.Data;
using GuitarShop.Models;

namespace GuitarShop.Controllers
{
    public class CartController : Controller
    {
        private const string CartKey = "cart";   // session key
        private readonly IRepositoryWrapper _repo;

        public CartController(IRepositoryWrapper repo)
        {
            _repo = repo;
        }

        // The cart is stored in session as { ProductID : Quantity }
        private Dictionary<int, int> GetCart()
        {
            string json = HttpContext.Session.GetString(CartKey);
            return json == null
                ? new Dictionary<int, int>()
                : JsonSerializer.Deserialize<Dictionary<int, int>>(json);
        }

        private void SaveCart(Dictionary<int, int> cart) =>
            HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));

        // GET /Cart  -> show cart
        public IActionResult Index()
        {
            var cart = GetCart();
            var products = _repo.Product.FindAll()
                .Where(p => cart.ContainsKey(p.ProductID)).ToList();

            ViewBag.Quantities = cart;
            return View(products);
        }

        // GET /Cart/Add/Fender-Stratocaster  (id is the product slug)
        public IActionResult Add(string id)
        {
            var product = _repo.Product.FindAll().FirstOrDefault(p => p.Slug == id);
            if (product == null)
                return RedirectToAction("List", "Product");

            var cart = GetCart();
            cart[product.ProductID] = cart.TryGetValue(product.ProductID, out int qty) ? qty + 1 : 1;
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // GET /Cart/Remove/5
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.Remove(id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // GET /Cart/Clear
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);
            return RedirectToAction("Index");
        }
    }
}