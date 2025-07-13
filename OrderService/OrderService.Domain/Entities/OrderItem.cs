using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain.Entities;

public class OrderItem
{
    [Key] public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    [Required] public string ProductId { get; set; } = string.Empty;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")] public decimal UnitPrice { get; set; }
}