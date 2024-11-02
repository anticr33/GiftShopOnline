using Microsoft.AspNetCore.Mvc;
using GiftShop.Models;
using System.Collections.Generic;

namespace GiftShop.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Подаръчна чаша", Description = "Красива чаша за кафе.", Price = 12.99m, ImageUrl = "/images/cup.jpg" },
                new Product { Id = 2, Name = "Ключодържател", Description = "Стилен ключодържател.", Price = 5.50m, ImageUrl = "/images/keychain.jpg" }
            };
            return View(products);
        }
    }
}
