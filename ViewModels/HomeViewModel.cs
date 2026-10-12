using TahibTraders.Models;

namespace TahibTraders.ViewModels
{
    // The home page needs offers, categories and products in a single object
    public class HomeViewModel
    {
        public List<Offer> Offers { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}