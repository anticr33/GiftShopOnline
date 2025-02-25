using GiftShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class StockController : Controller
{
    private readonly ApplicationDbContext _context;
    public StockController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var stockItems = await _context.Stock
            .Include(s => s.Product)
            .ToListAsync();
        return View(stockItems);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateStock(int id, int quantity)
    {
        var stockItem = await _context.Stock.FindAsync(id);
        if (stockItem != null)
        {
            stockItem.Quantity += quantity;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

}
