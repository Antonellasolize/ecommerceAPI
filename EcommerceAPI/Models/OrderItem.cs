using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceAPI.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }

    [Required]
    public int ProductId { get; set; }

    // navegación ligera (no ignoramos ProductId, pero sí podemos ignorar Product si querés)
    [JsonIgnore]
    public Product? Product { get; set; }

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}