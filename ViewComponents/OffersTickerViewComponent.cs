using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TahibTraders.Data;

namespace TahibTraders.ViewComponents
{
	// Loads the active offers for the red moving ticker shown on every page
	public class OffersTickerViewComponent : ViewComponent
	{
		private readonly AppDbContext _db;

		public OffersTickerViewComponent(AppDbContext db) => _db = db;

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var offers = await _db.Offers
				.Where(o => o.IsActive)
				.OrderByDescending(o => o.Id)
				.ToListAsync();
			return View(offers);
		}
	}
}