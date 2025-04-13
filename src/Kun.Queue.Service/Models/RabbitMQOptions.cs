namespace Kun.Queue.Models;

/// <summary>
/// RabbitMQ配置
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string QueueName { get; set; } = string.Empty;
}
