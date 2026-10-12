using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.Models;
using TahibTraders.Services;
using TahibTraders.ViewModels;

namespace TahibTraders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public class OwnerPhotoController : Controller
    {
        private const string Folder = "site";
        private readonly AppDbContext _db;
        private readonly ImageService _images;

        public OwnerPhotoController(AppDbContext db, ImageService images)
        {
            _db = db;
            _images = images;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = await GetOrCreateAsync();
            return View(new OwnerPhotoViewModel { ExistingPhoto = settings.OwnerPhotoFileName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OwnerPhotoViewModel vm)
        {
            var settings = await GetOrCreateAsync();

            if (vm.Photo is { Length: > 0 })
            {
                var (file, error) = await _images.SaveAsync(vm.Photo, Folder);
                if (error != null)
                {
                    ModelState.AddModelError(nameof(vm.Photo), error);
                    vm.ExistingPhoto = settings.OwnerPhotoFileName;
                    return View(vm);
                }

                // The new picture replaces the old one
                _images.Delete(settings.OwnerPhotoFileName, Folder);
                settings.OwnerPhotoFileName = file;
            }
            else if (vm.RemovePhoto)
            {
                _images.Delete(settings.OwnerPhotoFileName, Folder);
                settings.OwnerPhotoFileName = null;
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Owner picture saved.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SiteSetting> GetOrCreateAsync()
        {
            var settings = await _db.SiteSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SiteSetting();
                _db.SiteSettings.Add(settings);
                await _db.SaveChangesAsync();
            }
            return settings;
        }
    }
}