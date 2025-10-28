using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Dto.Company;

public class CompanyCreateDto
{
    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(120)]
    public string OwnerName { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal NetWorth { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}