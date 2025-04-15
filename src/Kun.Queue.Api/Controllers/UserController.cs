using Kun.Queue.Commons;
using Kun.Queue.Models;
using Kun.Queue.Options;
using Microsoft.AspNetCore.Mvc;

namespace Kun.Queue.Controllers;

/// <summary>
/// 用户
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [EndpointSummary("用户登录API")]
    [EndpointDescription("用户登录获取TOKEN")]
    public IActionResult Login([FromBody] LoginModel request)
    {
        ArgumentNullException.ThrowIfNull(request);
        (bool isValidate, string userId) = ValidateUser(request.Username, request.Password);
        if (!isValidate) { return Unauthorized("Invalid credentials"); }
        var _jwtConfig = _configuration.GetSection("JWT").Get<JwtOption>();
        ArgumentNullException.ThrowIfNull(_jwtConfig, nameof(_jwtConfig));
        (string accessToken, string refreshToken) = JwtHelper.GenerateToken(userId, _jwtConfig);
        return Ok(new { accessToken, refreshToken });
    }

    /// <summary>
    /// 刷新TOKEN
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [EndpointSummary("TOKEN刷新API")]
    [EndpointDescription("用户使用长时效TOKEN获取新TOKEN")]
    public IActionResult RefreshToken([FromBody] TokenModel request)
    {
        ArgumentNullException.ThrowIfNull(request);
        (bool isValidate, string userId) = ValidateRefreshToken(request.RefreshToken);
        if (!isValidate) { return Unauthorized("Invalid credentials"); }
        var _jwtConfig = _configuration.GetSection("JWT").Get<JwtOption>();
        ArgumentNullException.ThrowIfNull(_jwtConfig, nameof(_jwtConfig));
        (string accessToken, string refreshToken) = JwtHelper.GenerateToken(userId, _jwtConfig);
        return Ok(new { accessToken, refreshToken });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private (bool, string) ValidateUser(string username, string password)
    {
        return (true, "123456");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    private (bool, string) ValidateRefreshToken(string refreshToken)
    {
        return (true, "123456");
    }
}
