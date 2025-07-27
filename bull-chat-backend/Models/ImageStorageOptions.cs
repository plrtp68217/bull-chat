namespace bull_chat_backend.Models
{
    public class ImageStorageOptions
    {
        public const string IMAGE_STORAGE_KEY = "ImageStorage";
        public string ImageFolder { get; set; } = default!;
        public string ImageRequestPath { get; set; } = default!;
        public long MaxImageSizeBytes { get; set; } = 5 * 1024 * 1024; // 5MB по умолчанию
        public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    }

}
