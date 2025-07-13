using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    [Key] public Guid Id { get; set; }

    [Required] public string ExternalOrderId { get; set; } = string.Empty;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    [Column(TypeName = "decimal(18, 2)")] public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}