using System.ComponentModel.DataAnnotations;

namespace OrderService.Api.Dtos;

public record CreateOrderRequest([Required] string ExternalOrderId, [Required] List<OrderItemRequest> Items);

public record OrderItemRequest(
    [Required] string ProductId,
    [Range(1, int.MaxValue)] int Quantity,
    [Range(0.01, (double)decimal.MaxValue)]
    decimal UnitPrice);

public record OrderResponse(
    Guid Id,
    string ExternalOrderId,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    List<OrderItemResponse> Items);

public record OrderItemResponse(Guid Id, string ProductId, int Quantity, decimal UnitPrice);