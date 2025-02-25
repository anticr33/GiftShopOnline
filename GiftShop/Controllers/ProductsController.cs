using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftShop.Data;
using GiftShop.Models;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Показване на всички продукти с търсачка
    public async Task<IActionResult> Index(string searchQuery)
    {
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            products = products.Where(p =>
                p.Name.Contains(searchQuery) ||
                p.SKU.Contains(searchQuery) ||
                p.Category.Name.Contains(searchQuery));
        }

        return View(await products.ToListAsync());
    }


    // Форма за създаване на нов продукт
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int id, int quantity)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.Quantity += quantity;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Products","Admin");
    }




    // Добавяне на нов продукт
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(Product model, IFormFile image)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Записване на изображението, ако има качен файл
        if (image != null && image.Length > 0)
        {
            var fileName = Path.GetFileName(image.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            model.ImageUrl = "/images/" + fileName;
        }

        // Генериране на SKU, ако не е зададено
        if (string.IsNullOrEmpty(model.SKU))
        {
            model.SKU = $"SKU-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        _context.Products.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
