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
        await policy.ExecuteAsync(async () => await ProcessMessageAsync(message)); // 方法执行
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task ProcessMessageAsync(string message)
    {
        using (HttpClient client = new HttpClient())
        {
            var _apiConfig = _configuration.GetSection("BusinessApi").Get<BusinessApiOptions>();
            ArgumentNullException.ThrowIfNull(_apiConfig, nameof(_apiConfig));
            client.DefaultRequestHeaders.Add("X-USER-LOGINNAME", "0");
            client.Timeout = TimeSpan.FromMilliseconds(_apiConfig.Timeout);
            client.BaseAddress = new Uri(_apiConfig.BaseAddress);
            var request = new { Data = message };
            string jsonContent = JsonSerializer.Serialize(request);
            _logger.LogInformation($"请求内容: {jsonContent}");
            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(_apiConfig.RequestUri, content);// 发送 POST 请求
            response.EnsureSuccessStatusCode(); // 检查 HTTP 状态码
            string responseContent = await response.Content.ReadAsStringAsync(); // 读取响应内容（假设返回 JSON）
            _logger.LogInformation($"响应内容: {responseContent}");
        }
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
            var emailConfig = _configuration.GetSection("Email").Get<EmailOptions>();
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
