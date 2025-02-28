using GiftShop.Data;
using GiftShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GiftShop.Controllers
{
    public class QuestionnaireController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionnaireController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new QuestionnaireViewModel
            {
                Questions = new List<string>
                {
                    "За кого е подаръкът?",
                    "Какъв е поводът?",
                    "Какъв е вашият бюджет?"
                },
                Options = new Dictionary<int, List<string>>
                {
                    { 0, new List<string> { "Мъж", "Жена", "Дете" } },
                    { 1, new List<string> { "Рожден ден", "Годишнина", "Без повод" } },
                    { 2, new List<string> { "До 20 лв.", "20-50 лв.", "50-100 лв.", "100-200 лв.", "Над 200 лв." } }
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Recommendations(QuestionnaireViewModel model)
        {
            var filteredProducts = _context.Products.AsQueryable();

            // Филтър по категория
            if (model.SelectedAnswers[0] == "Мъж")
                filteredProducts = filteredProducts.Where(p => p.CategoryId == 1);
            else if (model.SelectedAnswers[0] == "Жена")
                filteredProducts = filteredProducts.Where(p => p.CategoryId == 2);
            else if (model.SelectedAnswers[0] == "Дете")
                filteredProducts = filteredProducts.Where(p => p.CategoryId == 3);



            // Филтър по бюджет
            if (model.SelectedAnswers[2] == "До 20 лв.")
                filteredProducts = filteredProducts.Where(p => p.Price <= 20);
            else if (model.SelectedAnswers[2] == "20-50 лв.")
                filteredProducts = filteredProducts.Where(p => p.Price > 20 && p.Price <= 50);
            else if (model.SelectedAnswers[2] == "50-100 лв.")
                filteredProducts = filteredProducts.Where(p => p.Price > 50 && p.Price <= 100);
            else if (model.SelectedAnswers[2] == "100-200 лв.")
                filteredProducts = filteredProducts.Where(p => p.Price > 100 && p.Price <= 200);
            else if (model.SelectedAnswers[2] == "Над 200 лв.")
                filteredProducts = filteredProducts.Where(p => p.Price > 200);

            return View(filteredProducts.ToList());
        }
    }
}
