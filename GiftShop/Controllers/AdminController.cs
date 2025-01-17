using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var admins = _context.Admins.ToList();
        return View(admins);
    }

    // === Управление на продукти ===

    // Показване на списък с продукти
    public IActionResult Products()
    {
        var products = _context.Products.Include(p => p.Category).ToList();
        return View(products);
    }

    // Създаване на нов продукт
    public IActionResult CreateProduct()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult CreateProduct(Product model)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        ViewBag.Categories = _context.Categories.ToList();
        return View(model);
    }

    // Изтриване на продукт
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
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
