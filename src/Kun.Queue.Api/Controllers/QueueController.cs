using Kun.Queue.Clients;
using Kun.Queue.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kun.Queue.Controllers;

/// <summary>
/// 队列
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class QueueController : ControllerBase
{
    private readonly RabbitMQClient _mqClient;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mqClient"></param>
    public QueueController(RabbitMQClient mqClient)
    {
        _mqClient = mqClient;
    }

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="order">订单</param>
    /// <returns></returns>
    [HttpPost]
    [EndpointSummary("发送消息API")]
    [EndpointDescription("发送消息")]
    public async Task<IActionResult> SendMessage()
    {
        await _mqClient.SendMessage("BY_MQ_Deduct_Test", new { Text = "测试" }.ToJson());
        return Ok("SUCCESS");
    }
}
