namespace OrderService.Application.Interfaces;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, T message);
}