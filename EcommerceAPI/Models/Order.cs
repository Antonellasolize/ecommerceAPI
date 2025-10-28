using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerUserId { get; set; }

    // navegación opcional (no la exponemos por JSON para evitar ciclos)
    public User? Customer { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Total { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Esta lista sí puede serializarse (no ponemos JsonIgnore) 
    // porque normalmente devolvés la orden con sus ítems.
    public List<OrderItem> Items { get; set; } = [];
}