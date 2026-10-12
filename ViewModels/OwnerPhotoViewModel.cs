using System.ComponentModel.DataAnnotations;

namespace TahibTraders.ViewModels
{
    public class OwnerPhotoViewModel
    {
        public IFormFile? Photo { get; set; }

        public string? ExistingPhoto { get; set; }

        [Display(Name = "Remove current picture")]
        public bool RemovePhoto { get; set; }
    }
}