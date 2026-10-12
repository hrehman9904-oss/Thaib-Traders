namespace TahibTraders.Models
{
    // A single row that holds the editable website settings
    public class SiteSetting
    {
        public int Id { get; set; }

        // Only the file name is stored (files live in wwwroot/uploads/site)
        public string? OwnerPhotoFileName { get; set; }
    }
}