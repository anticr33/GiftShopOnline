using GiftShop.Models;
using Microsoft.AspNetCore.Mvc;

public class ProductsController : Controller
{
    public IActionResult Details(int id)
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Комплект мъжки часовник от PU кожа", Description = "Луксозен комплект с часовник, слънчеви очила и портфейл, подходящ за стилни мъже.", Price = 50m, ImageUrl = "/images/menbox.png" },
            new Product { Id = 2, Name = "Комплект гарафа за уиски", Description = "Елегантна гарафа за уиски с две чаши, идеален подарък за любителите на уиски.", Price = 80m, ImageUrl = "/images/wesky.png" },
            new Product { Id = 3, Name = "Китарен винилов стенен часовник", Description = "Часовник с форма на китара.", Price = 60m, ImageUrl = "/images/gitar.png" }
        };

        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View("ProductDetails", product);
    }
}
