using GiftShop.Controllers;
using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")] // Ограничаваме достъпа само за администраторите
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageUsers()
    {
        return View();
    }

    public IActionResult ManageProducts()
    {
        return View();
    }

    // === Управление на поръчки ===
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // === Управление на продукти ===

    public IActionResult Products()
    {
        var products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    public IActionResult CreateProduct()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product model, IFormFile ImageFile)
    {
        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            model.ImageUrl = "/images/" + uniqueFileName;
        }

        _context.Products.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Products");
    }

    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProductConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
        return RedirectToAction("Products");
    }
}
