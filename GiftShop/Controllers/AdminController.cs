using GiftShop.Controllers;
using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")] // Ограничаваме достъпа само за администратор

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

    // === Управление на продукти ===

    // Показване на списък с продукти
    // === Управление на продукти ===

    // Показване на списък с продукти
    public IActionResult Products()
    {
        var products = _context.Products.Include(p => p.Category).ToList(); // Зареждане на продукти с категории
        return View(products); // Препращаме към изгледа
    }

    // Създаване на нов продукт - GET
    public IActionResult CreateProduct()
    {
        // Зареждаме категориите за падащото меню
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateProduct(Product model)
    {
        if (ModelState.IsValid)
        {
            // Добавяне на продукта към базата данни
            _context.Products.Add(model);
            _context.SaveChanges();

            // Пренасочване към списъка с продукти
            return RedirectToAction("Products");
        }

        // Ако има грешки, презареждаме падащото меню с категориите
        ViewBag.Categories = _context.Categories.ToList();
        return View(model);
    }





    // Изтриване на продукт - GET (потвърждение)
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id); // Търсим продукта по ID
        if (product == null)
        {
            return NotFound(); // Ако не е намерен, връщаме грешка 404
        }
        return View(product); // Показваме формата за потвърждение
    }

    // Изтриване на продукт - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProductConfirmed(int id)
    {
        var product = _context.Products.Find(id); // Търсим продукта по ID
        if (product != null)
        {
            _context.Products.Remove(product); // Премахваме продукта
            _context.SaveChanges(); // Запазваме промените
        }
        return RedirectToAction("Products"); // Връщаме се към списъка с продукти
    }
}
