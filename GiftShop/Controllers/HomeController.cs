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

        // ������� ��������
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // �������� ���� ������������ � � ������ Admin
                ViewBag.IsAdmin = User.IsInRole("Admin");
            }
            else
            {
                ViewBag.IsAdmin = false;
            }

            ViewBag.Message = "����� ����� � ����� �������!";
            return View();
        }

        // �������� �� �������������
        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();            // ���� �������
        public IActionResult Returns() => View();          // �������� �� �������
        public IActionResult Delivery() => View();         // ��������
        public IActionResult Cookies() => View();          // �������� �� ���������
        public IActionResult Contacts() => View();         // �������� �� ���������
        public IActionResult FAQ() => View();              // ����� �������� �������
        public IActionResult Wholesale() => View();


        // ������
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ��������� �� ��������� �� ID

        // ������� �� �������
        public IActionResult BuyProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // ����������� ��� �������� �� ������������ ��� �������
            return View(product);
        }
        public async Task<IActionResult> Category(int id, int? pageNumber)
        {
            int pageSize = 12; // ���� �������� �� ��������

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // ��������� ����������� ������
            var pagedProducts = category.Products.OrderBy(p => p.Id).ToPagedList(pageNumber ?? 1, pageSize);

            // ��������� CategoryViewModel
            var viewModel = new CategoryViewModel
            {
                Category = category,
                PagedProducts = pagedProducts
            };

            return View(viewModel); // ������ �� ������ `CategoryViewModel`
        }
    }
}