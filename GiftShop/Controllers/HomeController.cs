using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace GiftShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
               ILogger<HomeController> logger,
               ApplicationDbContext context,
               UserManager<IdentityUser> userManager,
               RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            // �������� ���� �������� ���������� � � ������ "Admin"
            var userId = _userManager.GetUserId(User); // ����� ������� ����������
            var user = _userManager.FindByIdAsync(userId).Result; // ������ �����������
            if (user == null)
            {
                // ������������ �� � �������
                return Redirect("/Identity/Account/Register");
            }
            bool isAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;

            if (isAdmin)
            {
                ViewBag.Message = "����� �����, ��������������!";
            }
            else
            {
                ViewBag.Message = "����� �����, �����������!";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ����������� ����� �� ��������� �� ��������� �� ID
        public IActionResult Category(int id)
        {
            var category = _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View("Category", category);
        }
        public IActionResult BuyProduct(int id)
        {
            // ��� ������ �� �������� ������ �� ��������� �� ���������.
            // ��������, ��������� �� �������� �� ������ �����:
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // ����������� ��� �������� �� ������������ ��� �������
            return View(product);
        }


    }
}
