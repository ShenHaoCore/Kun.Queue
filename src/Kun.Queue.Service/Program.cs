using Kun.Queue.Options;
using Kun.Queue.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

var configuration = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).Enrich.FromLogContext().CreateLogger();

builder.ConfigureLogging(configure =>
{
    configure.ClearProviders(); // 清理默认日志
    configure.AddSerilog(dispose: true); // 添加Serilog为日志
});

(IConnection connection, IChannel channel) = await GetRabbit(configuration);

builder.ConfigureServices((context, services) =>
{
    services.AddSingleton<IConfiguration>(configuration);
    services.AddSingleton<IConnection>(connection);
    services.AddSingleton<IChannel>(channel);
    services.AddHostedService<WorkerService>();
});

var host = builder.Build();
await host.RunAsync();

async Task<(IConnection, IChannel)> GetRabbit(IConfiguration configuration)
{
    var mqConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQOption>();
    ArgumentNullException.ThrowIfNull(mqConfig, nameof(mqConfig));
    ConnectionFactory factory = new ConnectionFactory { HostName = mqConfig.HostName, Port = 5672, UserName = mqConfig.UserName, Password = mqConfig.Password };
    IConnection connection = await factory.CreateConnectionAsync(); // 建立连接和通道
    IChannel channel = await connection.CreateChannelAsync();
    return (connection, channel);
}