namespace TahibTraders.Services
{
    // Saves and deletes pictures inside wwwroot/uploads/<folder>
    public class ImageService
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxBytes = 5 * 1024 * 1024;
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env) => _env = env;

        private string GetFolder(string folder) => Path.Combine(_env.WebRootPath, "uploads", folder);

        public async Task<(string? FileName, string? Error)> SaveAsync(IFormFile file, string folder = "products")
        {
            if (file.Length == 0) return (null, "The selected file is empty.");
            if (file.Length > MaxBytes) return (null, "The image must be 5 MB or smaller.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return (null, "Only JPG, PNG or WEBP images are allowed.");

            var target = GetFolder(folder);
            Directory.CreateDirectory(target);
            var fileName = Guid.NewGuid().ToString("N") + extension;

            await using var stream = File.Create(Path.Combine(target, fileName));
            await file.CopyToAsync(stream);
            return (fileName, null);
        }

        public void Delete(string? fileName, string folder = "products")
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var path = Path.Combine(GetFolder(folder), Path.GetFileName(fileName));
            if (File.Exists(path)) File.Delete(path);
        }
    }
}