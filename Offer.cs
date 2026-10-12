using System.ComponentModel.DataAnnotations;

namespace TahibTraders.Models
{
    public class Offer
    {
        public int Id { get; set; }

        // e.g. "This week", "Limited"
        [StringLength(20)]
        public string Tag { get; set; } = "Offer";

        [Required, StringLength(80)]
        public string Title { get; set; } = string.Empty;

        // e.g. "Special price: Rs. 1,650"
        [StringLength(160)]
        public string? Text { get; set; }

        public bool IsActive { get; set; } = true;
    }
}