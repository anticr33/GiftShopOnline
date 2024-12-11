using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult CreateProduct()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product model)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        ViewBag.Categories = _context.Categories.ToList();
        return View(model);
    }
}
