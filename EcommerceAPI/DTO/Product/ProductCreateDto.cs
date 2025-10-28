using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Dto.Product;

public class ProductCreateDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [MaxLength(300)]
    public string? ImageUrl { get; set; }
}