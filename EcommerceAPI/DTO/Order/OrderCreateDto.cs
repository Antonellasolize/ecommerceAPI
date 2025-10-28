using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Dto.Order;

public class OrderCreateDto
{
    [Required]
    public List<OrderItemCreateDto> Items { get; set; } = [];
}