namespace OrderService.Api.Dtos;

public class OrderDto
{
    public Guid Id { get; set; }
    public string ExternalOrderId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}