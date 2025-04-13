using Kun.Queue.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Kun.Queue.Service;

/// <summary>
/// 
/// </summary>
public class WorkerService : IHostedService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public WorkerService(ILogger<WorkerService> logger, IConfiguration configuration, IConnection connection, IChannel channel)
    {
        _logger = logger;
        _configuration = configuration;
        _connection = connection;
        _channel = channel;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"服务启动中...");
        var mqConfig = _configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>();
        ArgumentNullException.ThrowIfNull(mqConfig, nameof(mqConfig));
        await _channel.QueueDeclareAsync(queue: mqConfig.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += ConsumeAckAsync;
        await _channel.BasicConsumeAsync(mqConfig.QueueName, autoAck: false, consumer: consumer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"服务停止中...");
        await _channel.CloseAsync();
        await _connection.CloseAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="ea"></param>
    /// <returns></returns>
    public async Task ConsumeAckAsync(object model, BasicDeliverEventArgs ea)
    {
        string message = string.Empty;
        try
        {
            byte[] body = ea.Body.ToArray();
            message = Encoding.UTF8.GetString(body);
            _logger.LogInformation($"处理消息内容：【{message}】");
            await RetryProcessMessageAsync(message);// 处理业务逻辑
            await _channel.BasicAckAsync(ea.DeliveryTag, false); // 手动确认消息
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"消费异常 DeliveryTag：{ea.DeliveryTag}");
            await SendEmailAsync(message, ex);
            await _channel.BasicAckAsync(ea.DeliveryTag, false); // 手动确认消息
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task RetryProcessMessageAsync(string message)
    {
        var policy = Policy.Handle<Exception>().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(5)); // 创建重试策略：最多重试3次，间隔为5秒
        await policy.ExecuteAsync(async () => await ProcessMessageAsync(message)); // 包裹方法执行
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private async Task ProcessMessageAsync(string message)
    {
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private async Task SendEmailAsync(string msg, Exception ex)
    {
        try
        {
            // 创建邮件内容
            var emailConfig = _configuration.GetSection("EmailNotifier").Get<EmailOptions>();
            ArgumentNullException.ThrowIfNull(emailConfig, nameof(emailConfig));
            _logger.LogInformation($"邮箱配置：{JsonSerializer.Serialize(emailConfig)}");
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(emailConfig.From));
            emailConfig.To.ForEach(it => message.To.Add(MailboxAddress.Parse(it)));
            message.Subject = "异常通知：执行失败";
            message.Body = new TextPart("plain") { Text = $"机器名：{Environment.MachineName}\n消息内容：{msg}\n发生异常：{ex.Message}\n堆栈跟踪：{ex.StackTrace}" };

            // 配置 SMTP 客户端
            using (SmtpClient client = new SmtpClient())
            {
                await client.ConnectAsync(emailConfig.Host, 25, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            _logger.LogInformation("通知邮件已发送");
        }
        catch (Exception emailEx)
        {
            _logger.LogError(emailEx, $"发送邮件失败 {emailEx.Message}");
        }
    }
}
