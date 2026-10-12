using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.Models;

namespace TahibTraders.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DbSeeder.AdminRole)]
    public class OffersController : Controller
    {
        private readonly AppDbContext _db;

        public OffersController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() =>
            View(await _db.Offers.OrderByDescending(o => o.Id).ToListAsync());

        [HttpGet]
        public IActionResult Create() => View(new Offer());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tag,Title,Text,IsActive")] Offer offer)
        {
            if (!ModelState.IsValid) return View(offer);

            _db.Offers.Add(offer);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Offer added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer == null) return NotFound();
            return View(offer);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Tag,Title,Text,IsActive")] Offer input)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer == null) return NotFound();

            if (!ModelState.IsValid)
            {
                input.Id = id;
                return View(input);
            }

            offer.Tag = input.Tag;
            offer.Title = input.Title;
            offer.Text = input.Text;
            offer.IsActive = input.IsActive;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Offer updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer == null) return NotFound();

            offer.IsActive = !offer.IsActive;
            await _db.SaveChangesAsync();
            TempData["Success"] = offer.IsActive ? "Offer is now visible." : "Offer is now hidden.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer == null) return NotFound();

            _db.Offers.Remove(offer);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Offer deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}