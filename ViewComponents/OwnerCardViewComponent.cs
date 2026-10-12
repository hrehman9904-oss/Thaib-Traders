using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;
using TahibTraders.Models;

namespace TahibTraders.ViewComponents
{
    // Renders the owner card in the About section
    public class OwnerCardViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public OwnerCardViewComponent(AppDbContext db) => _db = db;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settings = await _db.SiteSettings.AsNoTracking().FirstOrDefaultAsync() ?? new SiteSetting();
            return View(settings);
        }
    }
}