using EcommerceAPI.Dto.Order;
using EcommerceAPI.Models;

namespace EcommerceAPI.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(int customerUserId, OrderCreateDto dto);
    Task<IReadOnlyList<Order>> GetOrdersForUserAsync(int customerUserId);
}