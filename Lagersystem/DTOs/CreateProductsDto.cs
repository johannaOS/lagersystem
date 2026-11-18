using System.ComponentModel.DataAnnotations;

namespace Lagersystem.DTOs
{
    public class CreateProductsDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [Range(0.01, 100000)]
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Shelf { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Count { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }

    }
}
