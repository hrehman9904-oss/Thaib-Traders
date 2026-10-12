using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.Models;
using TahibTraders.Services;
using TahibTraders.ViewModels;

namespace TahibTraders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ImageService _images;

        public ProductsController(AppDbContext db, ImageService images)
        {
            _db = db;
            _images = images;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Products.Include(p => p.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(p => p.Name.Contains(q));

            ViewData["Query"] = q;
            return View(await query.OrderBy(p => p.Name).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ProductFormViewModel { Categories = await LoadCategoriesAsync() };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel vm)
        {
            await ValidateCategoryAsync(vm);

            string? fileName = null;
            if (ModelState.IsValid && vm.Image is { Length: > 0 })
            {
                var (saved, error) = await _images.SaveAsync(vm.Image);
                if (error != null) ModelState.AddModelError(nameof(vm.Image), error);
                else fileName = saved;
            }

            if (!ModelState.IsValid)
            {
                vm.Categories = await LoadCategoriesAsync();
                return View(vm);
            }

            _db.Products.Add(new Product
            {
                Name = vm.Name.Trim(),
                Description = vm.Description?.Trim(),
                Price = vm.Price,
                CategoryId = vm.CategoryId!.Value,
                IsActive = vm.IsActive,
                ImageFileName = fileName
            });
            await _db.SaveChangesAsync();

            TempData["Success"] = "Product added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                ExistingImageFileName = product.ImageFileName,
                Categories = await LoadCategoriesAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel vm)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            vm.Id = id;
            await ValidateCategoryAsync(vm);

            string? newFile = null;
            if (ModelState.IsValid && vm.Image is { Length: > 0 })
            {
                var (saved, error) = await _images.SaveAsync(vm.Image);
                if (error != null) ModelState.AddModelError(nameof(vm.Image), error);
                else newFile = saved;
            }

            if (!ModelState.IsValid)
            {
                vm.ExistingImageFileName = product.ImageFileName;
                vm.Categories = await LoadCategoriesAsync();
                return View(vm);
            }

            product.Name = vm.Name.Trim();
            product.Description = vm.Description?.Trim();
            product.Price = vm.Price;
            product.CategoryId = vm.CategoryId!.Value;
            product.IsActive = vm.IsActive;

            if (newFile != null)
            {
                // A new picture replaces the old one
                _images.Delete(product.ImageFileName);
                product.ImageFileName = newFile;
            }
            else if (vm.RemoveImage)
            {
                _images.Delete(product.ImageFileName);
                product.ImageFileName = null;
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Product updated.";
            return RedirectToAction(nameof(Index));
        }

        // Shows or hides a product on the website without deleting it
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = !product.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = product.IsActive ? "Product is now visible." : "Product is now hidden.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _images.Delete(product.ImageFileName);
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> LoadCategoriesAsync() =>
            await _db.Categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToListAsync();

        private async Task ValidateCategoryAsync(ProductFormViewModel vm)
        {
            if (vm.CategoryId.HasValue && !await _db.Categories.AnyAsync(c => c.Id == vm.CategoryId.Value))
                ModelState.AddModelError(nameof(vm.CategoryId), "Please choose a valid category.");
        }
    }
}