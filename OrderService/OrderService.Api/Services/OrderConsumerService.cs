using System.Text.Json;
using Confluent.Kafka;
using OrderService.Application.Services;

namespace OrderService.Api.Services;

public class OrderConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<OrderConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _topicName;

    public OrderConsumerService(IConfiguration configuration, ILogger<OrderConsumerService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _topicName = configuration["Kafka:ReceivedTopic"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topicName);
        _logger.LogInformation("Consumidor Kafka iniciado para o tópico '{TopicName}'...", _topicName);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var message = consumeResult.Message.Value;

                _logger.LogDebug("Mensagem recebida: {Message}", message);

                var orderDto = JsonSerializer.Deserialize<CreateOrderDto>(message,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (orderDto is null)
                {
                    _logger.LogWarning("Não foi possível desserializar a mensagem: {Message}", message);
                    continue;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var processingService = scope.ServiceProvider.GetRequiredService<OrderProcessingService>();
                    await processingService.ProcessNewOrderAsync(orderDto);
                }

                _consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no consumidor Kafka.");
                await Task.Delay(5000, stoppingToken);
            }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}