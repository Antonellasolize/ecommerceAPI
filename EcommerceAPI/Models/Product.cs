using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [JsonIgnore]
    public Company? Company { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public bool IsPublished { get; set; } = false;

    [MaxLength(300)]
    public string? ImageUrl { get; set; }

    // Evita ciclos Order -> Items -> Product -> Items -> ...
    [JsonIgnore]
    public List<OrderItem> OrderItems { get; set; } = [];
}