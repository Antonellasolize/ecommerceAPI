using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Dto.Order;

public class OrderItemCreateDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}