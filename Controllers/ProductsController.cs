using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.ViewModels;

namespace TahibTraders.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db) => _db = db;

        // Lists active products, optionally filtered by category and search text
        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            var query = _db.Products.Include(p => p.Category).Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Name.Contains(q));

            var vm = new ProductListViewModel
            {
                Products = await query.OrderBy(p => p.Name).ToListAsync(),
                Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync(),
                SelectedCategoryId = categoryId,
                Query = q
            };
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return NotFound();
            return View(product);
        }
    }
}