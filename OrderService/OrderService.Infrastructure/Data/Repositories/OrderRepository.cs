using Microsoft.EntityFrameworkCore;
using OrderService.Api.Dtos;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExternalOrderExistsAsync(string externalOrderId)
    {
        return await _context.Orders.AnyAsync(o => o.ExternalOrderId == externalOrderId);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByExternalIdAsync(string externalId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.ExternalOrderId == externalId);
    }

    public async Task<List<OrderDto>> GetAll()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Select(order => new OrderDto 
            {
                Id = order.Id,
                ExternalOrderId = order.ExternalOrderId,
                Status = order.Status.ToString(), 
                TotalPrice = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}