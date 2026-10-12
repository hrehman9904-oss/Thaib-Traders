using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.Models;
using TahibTraders.ViewModels;

namespace TahibTraders.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Offers = await _db.Offers
                    .Where(o => o.IsActive)
                    .OrderByDescending(o => o.Id)
                    .ToListAsync(),
                Categories = await _db.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                Products = await _db.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(8)
                    .ToListAsync()
            };
            return View(vm);
        }

        public IActionResult About() => View();

        public IActionResult Contact() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}