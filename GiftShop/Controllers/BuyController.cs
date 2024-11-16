using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace GiftShop.Controllers
{
    public class BuyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BuyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Product(int id)
        {
            // Намерете продукта по ID
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Продуктът не е намерен.");
            }

            // Върнете изглед с продукта
            return View(product);
        }
    }
}
