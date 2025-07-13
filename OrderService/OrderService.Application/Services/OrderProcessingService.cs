using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Services;

public record CreateOrderDto(string ExternalOrderId, List<CreateOrderItemDto> Items);

public record CreateOrderItemDto(string ProductId, int Quantity, decimal UnitPrice);

public class OrderProcessingService
{
    private readonly string _calculatedTopic;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly IOrderRepository _orderRepository;


    public OrderProcessingService(
        IOrderRepository orderRepository,
        IKafkaProducer kafkaProducer,
        ILogger<OrderProcessingService> logger,
        IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
        _calculatedTopic = configuration["Kafka:CalculatedTopic"] ?? "pedidos-calculados";
    }

    public async Task ProcessNewOrderAsync(CreateOrderDto orderDto)
    {
        if (await _orderRepository.ExternalOrderExistsAsync(orderDto.ExternalOrderId))
        {
            _logger.LogWarning("Pedido duplicado recebido e ignorado: {ExternalOrderId}", orderDto.ExternalOrderId);
            return;
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            ExternalOrderId = orderDto.ExternalOrderId,
            Status = OrderStatus.Processing,
            CreatedAt = DateTime.UtcNow,
            Items = orderDto.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(item => item.Quantity * item.UnitPrice);
        order.Status = OrderStatus.Calculated;
        order.ProcessedAt = DateTime.UtcNow;

        try
        {
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            _logger.LogInformation("Pedido {ExternalOrderId} processado e salvo com sucesso.", order.ExternalOrderId);
            await _kafkaProducer.ProduceAsync(_calculatedTopic, order);
            _logger.LogInformation("Notificação do pedido {ExternalOrderId} enviada para o tópico Kafka.",
                order.ExternalOrderId);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            _logger.LogWarning(
                "Violação de chave única ao salvar o pedido {ExternalOrderId}. Provavelmente um processamento concorrente do mesmo pedido. Ignorando.",
                orderDto.ExternalOrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar o pedido {ExternalOrderId}", orderDto.ExternalOrderId);
        }
    }
}