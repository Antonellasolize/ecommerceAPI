using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models;

public class Company
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(120)]
    public string OwnerName { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal NetWorth { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public List<Product> Products { get; set; } = [];
}