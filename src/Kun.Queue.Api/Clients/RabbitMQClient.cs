using RabbitMQ.Client;
using System.Text;

namespace Kun.Queue.Clients;

/// <summary>
/// 
/// </summary>
public class RabbitMQClient
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="channel"></param>
    public RabbitMQClient(IConnection connection, IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessage(string queueName, string message)
    {
        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: Encoding.UTF8.GetBytes(message));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task Close()
    {
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }
}
