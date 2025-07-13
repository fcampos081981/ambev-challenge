using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Dtos;
using OrderService.Application.Interfaces;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<OrdersController> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly string _receivedTopic;

    public OrdersController(
        IOrderRepository orderRepository,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
        _receivedTopic = configuration["Kafka:ReceivedTopic"] ?? "pedidos-recebidos";
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _orderRepository.GetAll());
    }

    [HttpGet("external/{externalId}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByExternalId(string externalId)
    {
        var order = await _orderRepository.GetByExternalIdAsync(externalId);

        if (order == null) return NotFound();

        var response = new OrderResponse(
            order.Id,
            order.ExternalOrderId,
            order.TotalAmount,
            order.Status.ToString(),
            order.CreatedAt,
            order.ProcessedAt,
            order.Items.Select(i => new OrderItemResponse(i.Id, i.ProductId, i.Quantity, i.UnitPrice)).ToList()
        );

        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var order = await _orderRepository.GetByExternalIdAsync(request.ExternalOrderId);
            
            if (order != null) return StatusCode(StatusCodes.Status409Conflict,
                "Este número de ordem de serviço já existe!.");
            
            _logger.LogInformation("Recebida nova requisição de pedido via API: {ExternalOrderId}",
                request.ExternalOrderId);

            await _kafkaProducer.ProduceAsync(_receivedTopic, request);

            _logger.LogInformation("Pedido {ExternalOrderId} enviado para o tópico Kafka '{Topic}' para processamento.",
                request.ExternalOrderId, _receivedTopic);

            return AcceptedAtAction(nameof(GetByExternalId), new { externalId = request.ExternalOrderId }, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar publicar o pedido {ExternalOrderId} no Kafka.",
                request.ExternalOrderId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Ocorreu um erro ao processar sua solicitação.");
        }
    }
}