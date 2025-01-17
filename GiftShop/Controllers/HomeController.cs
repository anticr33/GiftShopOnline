using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GiftShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Начална страница
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Проверка дали потребителят е в ролята Admin
                ViewBag.IsAdmin = User.IsInRole("Admin");
            }
            else
            {
                ViewBag.IsAdmin = false;
            }

            ViewBag.Message = "Добре дошли в нашия магазин!";
            return View();
        }

        // Страница за поверителност
        public IActionResult Privacy()
        {
            return View();
        }

        // Грешка
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Показване на категория по ID
        public IActionResult Category(int id)
        {
            var category = _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // Покупка на продукт
        public IActionResult BuyProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Прехвърляне към страница за потвърждение или кошница
            return View(product);
        }
    }
}
