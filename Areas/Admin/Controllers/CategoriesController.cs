using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.Models;

namespace TahibTraders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View(new Category());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            category.Name = category.Name?.Trim() ?? string.Empty;

            if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
                ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");

            if (!ModelState.IsValid) return View(category);

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Category added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Category input)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            input.Name = input.Name?.Trim() ?? string.Empty;
            if (await _db.Categories.AnyAsync(c => c.Name == input.Name && c.Id != id))
                ModelState.AddModelError(nameof(Category.Name), "A category with this name already exists.");

            if (!ModelState.IsValid)
            {
                input.Id = id;
                return View(input);
            }

            category.Name = input.Name;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Category updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            // Do not delete a category that still has products
            if (category.Products.Any())
            {
                TempData["Error"] = "This category still has products. Move or delete them first.";
                return RedirectToAction(nameof(Index));
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Category deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}