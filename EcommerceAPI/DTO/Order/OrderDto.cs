namespace EcommerceAPI.Dto.Order;

public class OrderDto
{
    public int Id { get; set; }
    public string Status { get; set; } = null!; // enum→string en el controller
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}