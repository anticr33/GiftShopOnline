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

        public IActionResult CreateOrder()
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

            UpdateCartCount(); // Обновява броя артикули в количката
            return View(cartItems);
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            UpdateCartCount(); // Обновява количката в навигацията
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

            UpdateCartCount(); // Обновява количката в навигацията
            return View(order);
        }

        [HttpPost]
        public IActionResult ConfirmOrder(string shippingAddress, string phoneNumber, string shippingMethod, string paymentMethod)
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

            var order = new Order
            {
                UserId = userId,
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
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            UpdateCartCount(); // Обновява количката след поръчка
            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            UpdateCartCount(); // Обновява количката в навигацията
            return View();
        }
    }
}
