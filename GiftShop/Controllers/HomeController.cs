using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using X.PagedList.Extensions;

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
        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();            // Общи условия
        public IActionResult Returns() => View();          // Политика за връщане
        public IActionResult Delivery() => View();         // Доставка
        public IActionResult Cookies() => View();          // Политика за бисквитки
        public IActionResult Contacts() => View();         // Политика за сигурност
        public IActionResult FAQ() => View();              // Често задавани въпроси
        public IActionResult Wholesale() => View();


        // Грешка
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Показване на категория по ID

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
        public async Task<IActionResult> Category(int id, int? pageNumber)
        {
            int pageSize = 12; // Брой продукти на страница

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Създаваме странициран списък
            var pagedProducts = category.Products.OrderBy(p => p.Id).ToPagedList(pageNumber ?? 1, pageSize);

            // Създаваме CategoryViewModel
            var viewModel = new CategoryViewModel
            {
                Category = category,
                PagedProducts = pagedProducts
            };

            return View(viewModel); // Трябва да върнем `CategoryViewModel`
        }
    }
}