using System.ComponentModel.DataAnnotations;

namespace nguyennhatquang_2280618967.Models
{
    public class ProductImage
    {
        [Key] // Thêm dòng này
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
