namespace EcommerceAPI.Dto.Product;

public class ProductDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsPublished { get; set; }
    public string? ImageUrl { get; set; }
}