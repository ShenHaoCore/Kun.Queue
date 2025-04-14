using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Kun.Queue.Models;

/// <summary>
/// 
/// </summary>
public class LoginModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [property: Required]
    [property: DefaultValue("Admin")]
    [property: Description("用户名")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [property: Required]
    [property: DefaultValue("123456")]
    [property: Description("密码")]
    public string Password { get; set; } = string.Empty;
}
