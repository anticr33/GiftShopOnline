using GiftShop.Controllers;
using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")] // Ограничаваме достъпа само за администраторите
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
    public IActionResult Orders()
    {
        var orders = _context.Orders
            .Where(o => !o.IsCompleted)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

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

        // Вземаме имейла на потребителя, направил поръчката
        var user = await _userManager.FindByIdAsync(order.UserId);
        ViewBag.UserEmail = user?.Email;

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
    [HttpGet]
    public IActionResult CreateManualOrder()
    {
        var products = _context.Products.Where(p => p.Quantity > 0).ToList();
        ViewBag.Products = products; // Изпращаме списъка с продукти към изгледа
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ProcessManualOrder(int productId, int quantity, string customerName, string shippingAddress)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null || product.Quantity < quantity)
        {
            TempData["Error"] = "Недостатъчна наличност!";
            return RedirectToAction("CreateManualOrder");
        }

        // Намаляване на количеството
        product.Quantity -= quantity;

        // Създаване на нова поръчка
        var order = new Order
        {
            UserId = "Manual", // Маркираме поръчката като ръчна
            FullName = customerName,
            ShippingAddress = shippingAddress,
            PhoneNumber = "Не е зададен",
            ShippingMethod = "Ръчно",
            PaymentMethod = "Ръчно",
            OrderItems = new List<OrderItem>
        {
            new OrderItem { ProductId = productId, Quantity = quantity }
        }
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Ръчната поръчка е създадена успешно!";
        return RedirectToAction("Index");
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
    [HttpPost]
    public IActionResult CompleteOrder(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        order.IsCompleted = true;
        _context.SaveChanges();

        return RedirectToAction("Orders"); // Връща към списъка с поръчки
    }

    public IActionResult CompletedOrders()
    {
        var completedOrders = _context.Orders
            .Where(o => o.IsCompleted)
            .OrderByDescending(o => o.OrderDate)
            .ToList();

        return View(completedOrders);
    }
    public IActionResult MarkAsCompleted(int id)
    {
        var order = _context.Orders.Find(id);
        if (order != null)
        {
            order.IsCompleted = true;
            _context.SaveChanges();
        }

        return RedirectToAction("Orders");
    }
    // GET: Admin/EditProduct/{id}
    public IActionResult EditProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }

        ViewBag.Categories = _context.Categories.ToList();
        return View(product);
    }

    // POST: Admin/EditProduct
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(Product model, IFormFile ImageFile)
    {
        var product = await _context.Products.FindAsync(model.Id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.PurchasePrice = model.PurchasePrice; 
        product.SKU = model.SKU;                     
        product.CategoryId = model.CategoryId;

        if (ImageFile != null && ImageFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", uniqueFileName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(fileStream);
            }

            product.ImageUrl = "/images/" + uniqueFileName;
        }

        _context.Update(product);
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

    public IActionResult Statistics(DateTime? startDate, DateTime? endDate, string filter = "day")
    {
        var query = _context.Orders
            .Where(o => o.IsCompleted) // Само завършени поръчки
            .AsQueryable(); // Оставяме като IQueryable за динамично филтриране

        var today = DateTime.Today;

        // 🛠 Поправен филтър "От-до"
        if (filter == "custom" && startDate.HasValue && endDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= startDate.Value && o.OrderDate <= endDate.Value);
        }
        else
        {
            if (filter == "day")
                query = query.Where(o => o.OrderDate >= today);
            else if (filter == "week")
                query = query.Where(o => o.OrderDate >= today.AddDays(-7));
            else if (filter == "month")
                query = query.Where(o => o.OrderDate >= today.AddMonths(-1));
        }

        // 🛠 Добавяме Include() СЛЕД като сме приложили филтрите
        query = query
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product);

        var orders = query.ToList();

        // 🛠 Изчисляване на приходите и разходите
        var revenue = orders.Sum(o => o.OrderItems.Sum(oi => oi.Product.Price * oi.Quantity));
        var cost = orders.Sum(o => o.OrderItems.Sum(oi => oi.Product.PurchasePrice * oi.Quantity));
        var profit = revenue - cost;

        var model = new StatisticsViewModel
        {
            TotalRevenue = revenue,
            TotalProfit = profit,
            Orders = orders
        };

        return View(model);
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
    [HttpPost]
    public IActionResult DeleteOrder(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        _context.SaveChanges();

        return RedirectToAction("Orders");
    }
}


   

