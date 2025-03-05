using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GiftShop.Controllers
{
    public class BuyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BuyController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Product(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound("Продуктът не е намерен.");
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            string userId = _userManager.GetUserId(User);
            string sessionId = Request.Cookies["GuestSessionId"] ?? Guid.NewGuid().ToString();

            if (Request.Cookies["GuestSessionId"] == null)
            {
                Response.Cookies.Append("GuestSessionId", sessionId, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            }

            var cartItems = await _context.CartItems
                .Where(c => (c.UserId == userId && userId != null) || c.SessionId == sessionId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest("Количката е празна.");
            }

            var order = new Order
            {
                UserId = userId, // Ще бъде NULL за гости
                SessionId = sessionId, // Използва се за гости
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }
    }
}