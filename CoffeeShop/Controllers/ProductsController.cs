using CoffeeShop.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepo;
        public ProductsController(IProductRepository productRepo)
        {
            this.productRepo = productRepo;
        }

        public IActionResult Shop()
        {
            var products = productRepo.GetAllProducts();
            return View(products);
        }
    }

}
