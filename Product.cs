using System.ComponentModel.DataAnnotations;

namespace TahibTraders.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10000000)]
        public decimal Price { get; set; }

        // Only the file name is stored, e.g. "abc123.jpg" (the file lives in wwwroot/uploads/products)
        public string? ImageFileName { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // false hides the product from the website without deleting it
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}