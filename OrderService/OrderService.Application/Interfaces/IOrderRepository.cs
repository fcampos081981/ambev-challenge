using OrderService.Api.Dtos;
using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<bool> ExternalOrderExistsAsync(string externalOrderId);
    Task AddAsync(Order order);
    Task<Order?> GetByExternalIdAsync(string externalId);
    Task<List<OrderDto>> GetAll();
    Task<int> SaveChangesAsync();
}