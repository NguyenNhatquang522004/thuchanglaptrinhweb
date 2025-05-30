using System.ComponentModel.DataAnnotations;

namespace nguyennhatquang_2280618967.Models
{
    public class ProductVideo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Kích thước file video (bytes)
        public long FileSize { get; set; }

        // Độ dài video (giây)
        public int Duration { get; set; }

        // MIME type (video/mp4, video/webm, etc.)
        [StringLength(50)]
        public string ContentType { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
