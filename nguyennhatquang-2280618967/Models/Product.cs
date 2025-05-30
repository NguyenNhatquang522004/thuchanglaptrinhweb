using System.ComponentModel.DataAnnotations;

namespace nguyennhatquang_2280618967.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 10000.00)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        // Navigation properties
        public List<ProductImage>? Images { get; set; }
        public List<ProductVideo>? Videos { get; set; } // Thêm property cho videos

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
