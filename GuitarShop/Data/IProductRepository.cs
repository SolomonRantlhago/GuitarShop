using GuitarShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuitarShop.Data
{
    public interface IProductRepository: IRepositoryBase<Product>
    {
        IEnumerable<Product> GetAllProductsInCategory(string category);
        IEnumerable<Product> GetAllProductsWithCategoryDetails();
    }
}
