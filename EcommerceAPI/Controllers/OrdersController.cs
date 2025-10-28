using System.Security.Claims;
using EcommerceAPI.Data;
using EcommerceAPI.Dto.Order;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Cliente")]
public class OrdersController(AppDbContext context) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
    {
        var userId = GetUserId();

        var orders = await context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerUserId == userId)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                Status = o.Status.ToString(),   // 👈 enum → string para el DTO
                Total = o.Total,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();

        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest("Empty order.");

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        if (products.Count != productIds.Count)
            return BadRequest("Some products do not exist.");

        foreach (var item in dto.Items)
        {
            var p = products.First(x => x.Id == item.ProductId);
            if (!p.IsPublished) return BadRequest($"Product {p.Name} is not available.");
            if (p.Stock < item.Quantity) return BadRequest($"Insufficient stock for {p.Name}.");
        }

        var order = new Order
        {
            CustomerUserId = userId,
            Status = OrderStatus.Pending,   // 👈 ahora enum, no string
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

            // opcional: descontar stock ya en Pending (o hacerlo al pagar)
            p.Stock -= item.Quantity;
        }
        order.Total = total;

        using var tx = await context.Database.BeginTransactionAsync();
        try
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return Ok(new { order.Id, Status = order.Status.ToString(), order.Total }); // 👈 enum → string
    }
}