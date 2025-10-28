using System.Linq;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Data;
using EcommerceAPI.Dto.Order;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services;

public class OrderService(AppDbContext context) : IOrderService
{
    public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(int customerUserId)
    {
        return await context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerUserId == customerUserId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order> CreateOrderAsync(int customerUserId, OrderCreateDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            throw new ArgumentException("Empty order.");

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        if (products.Count != productIds.Count)
            throw new InvalidOperationException("Some products do not exist.");

        foreach (var item in dto.Items)
        {
            var p = products.First(x => x.Id == item.ProductId);
            if (!p.IsPublished) throw new InvalidOperationException($"Product {p.Name} is not available.");
            if (p.Stock < item.Quantity) throw new InvalidOperationException($"Insufficient stock for {p.Name}.");
        }

        var order = new Order
        {
            CustomerUserId = customerUserId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>()
        };

        decimal total = 0m;
        foreach (var item in dto.Items)
        {
            var p = products.First(x => x.Id == item.ProductId);
            order.Items.Add(new OrderItem
            {
                ProductId = p.Id,
                Quantity = item.Quantity,
                UnitPrice = p.Price
            });
            total += p.Price * item.Quantity;

            p.Stock -= item.Quantity; // si prefieres descontar al pagar, mueve esto a otro flujo
        }
        order.Total = total;

        using var tx = await context.Database.BeginTransactionAsync();
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        await tx.CommitAsync();

        return order;
    }
}