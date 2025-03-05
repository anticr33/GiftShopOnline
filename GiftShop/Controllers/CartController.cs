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
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Продуктът не е намерен.");
            }

            string userId = _userManager.GetUserId(User);
            string sessionId = Request.Cookies["GuestSessionId"] ?? Guid.NewGuid().ToString();

            if (Request.Cookies["GuestSessionId"] == null)
            {
                Response.Cookies.Append("GuestSessionId", sessionId, new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ProductId == productId && (c.UserId == userId || c.SessionId == sessionId));
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UserId = userId, // Може да е null за гости
                    SessionId = sessionId
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> Index()
        {
            string sessionId = Request.Cookies["GuestSessionId"];
            string userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Where(c => (c.UserId == userId && userId != null) || c.SessionId == sessionId)
                .Include(c => c.Product)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cartItem = _context.CartItems.Find(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
