namespace Kun.Queue.Options;

/// <summary>
/// RabbitMQ配置
/// </summary>
public class RabbitMQOption
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
