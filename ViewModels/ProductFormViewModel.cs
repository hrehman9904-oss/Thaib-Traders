using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TahibTraders.ViewModels
{
    // Used by the admin Add/Edit product form
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        [Display(Name = "Product name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 10000000, ErrorMessage = "Please enter a valid price.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please choose a category.")]
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Show on website")]
        public bool IsActive { get; set; } = true;

        // New picture chosen by the admin (optional)
        public IFormFile? Image { get; set; }

        // Current picture (edit only)
        public string? ExistingImageFileName { get; set; }

        [Display(Name = "Remove current picture")]
        public bool RemoveImage { get; set; }

        [ValidateNever]
        public List<SelectListItem> Categories { get; set; } = new();
    }
}