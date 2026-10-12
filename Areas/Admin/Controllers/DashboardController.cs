using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;

namespace TahibTraders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.Products = await _db.Products.CountAsync();
            ViewBag.ActiveProducts = await _db.Products.CountAsync(p => p.IsActive);
            ViewBag.Categories = await _db.Categories.CountAsync();
            ViewBag.Offers = await _db.Offers.CountAsync(o => o.IsActive);
            return View();
        }
    }
}