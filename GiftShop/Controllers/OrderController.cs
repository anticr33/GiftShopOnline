using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Метод за актуализиране на броя артикули в количката
        private void UpdateCartCount()
        {
            var userId = _userManager.GetUserId(User);
            ViewData["CartCount"] = _context.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            UpdateCartCount();
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

            UpdateCartCount();
            return View(order);
        }

        public IActionResult CreateOrder(int? productId, int? quantity)
        {
            var userId = _userManager.GetUserId(User);

            List<CartItem> cartItems;

            if (productId.HasValue && quantity.HasValue)
            {
                // Ако идва от "Купи сега", правим временна количка с 1 продукт
                var product = _context.Products.FirstOrDefault(p => p.Id == productId.Value);
                if (product == null || product.Quantity < quantity.Value)
                {
                    TempData["Error"] = "Избраният продукт не е наличен в достатъчно количество.";
                    return RedirectToAction("Index", "Home");
                }

                cartItems = new List<CartItem>
        {
            new CartItem { Product = product, Quantity = quantity.Value }
        };
            }
            else
            {
                // Ако идва от нормална количка, зареждаме количката на потребителя
                cartItems = _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Product)
                    .ToList();

                if (!cartItems.Any())
                {
                    return RedirectToAction("Index", "Cart");
                }
            }

            return View(cartItems);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(string fullName, string shippingAddress, string phoneNumber, string shippingMethod, string paymentMethod)
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Проверка за наличност
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Недостатъчна наличност за {item.Product.Name}.";
                    return RedirectToAction("CreateOrder");
                }
            }

            // Намаляване на наличността
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
            }

            // Записване на поръчката
            var order = new Order
            {
                UserId = userId,
                FullName = fullName,
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber,
                ShippingMethod = shippingMethod,
                PaymentMethod = paymentMethod,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Изчистване на количката след поръчка
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderSuccess");
        }


        public IActionResult OrderSuccess()
        {
            UpdateCartCount();
            return View();
        }
    }
}
