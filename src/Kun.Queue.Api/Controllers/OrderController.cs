using Kun.Queue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kun.Queue.Controllers;

/// <summary>
/// 订单
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class OrderController : ControllerBase
{
    /// <summary>
    /// 创建
    /// </summary>
    /// <param name="order">订单</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [Tags("幂等接口")]
    [EndpointSummary("创建订单API")]
    [EndpointDescription("创建订单")]
    public IActionResult Create([FromBody] CreateOrderModel order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return Ok("SUCCESS-CREATE");
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize]
    [Tags("幂等接口")]
    [EndpointSummary("删除订单API")]
    [EndpointDescription("删除订单")]
    public IActionResult Delete(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return Ok($"SUCCESS-DELETE-{id}");
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="order">订单</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize]
    [Tags("幂等接口")]
    [EndpointSummary("修改订单API")]
    [EndpointDescription("修改订单")]
    public IActionResult Put(string id, UpdateOrderModel order)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(order);
        return Ok($"SUCCESS-UPDATE-{id}");
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize]
    [Tags("非幂等接口")]
    [EndpointSummary("获取订单API")]
    [EndpointDescription("获取订单")]
    public IActionResult Get(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return Ok($"SUCCESS-{id}");
    }
}
