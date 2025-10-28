namespace EcommerceAPI.Dto.Company;

public class CompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public decimal NetWorth { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}