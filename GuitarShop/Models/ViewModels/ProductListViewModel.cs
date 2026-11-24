using System.Collections.Generic;

namespace GuitarShop.Models.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public string SelectedCategory { get; set; }
        public string CheckActiveCategory(string category) =>
            category == SelectedCategory ? "active" : "";
    }
}
