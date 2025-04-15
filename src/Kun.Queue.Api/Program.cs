using Kun.Queue.Clients;
using Kun.Queue.Options;
using Kun.Queue.Scalars;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// 添加认证服务
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"] ?? string.Empty,
        ValidAudience = configuration["Jwt:Audience"] ?? string.Empty,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ?? string.Empty))
    };
});

(IConnection connection, IChannel channel) = await GetRabbit(configuration);
builder.Services.AddSingleton(connection);
builder.Services.AddSingleton(channel);
builder.Services.AddScoped<RabbitMQClient>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<ApiDocument>();
    options.AddDocumentTransformer<BearerSecurity>();
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

async Task<(IConnection, IChannel)> GetRabbit(IConfiguration configuration)
{
    var mqConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQOption>();
    ArgumentNullException.ThrowIfNull(mqConfig, nameof(mqConfig));
    ConnectionFactory factory = new ConnectionFactory { HostName = mqConfig.HostName, Port = 5672, UserName = mqConfig.UserName, Password = mqConfig.Password };
    IConnection connection = await factory.CreateConnectionAsync(); // 建立连接和通道
    IChannel channel = await connection.CreateChannelAsync();
    return (connection, channel);
}
