using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Kun.Queue.Models;

/// <summary>
/// 
/// </summary>
public class TokenModel
{
    /// <summary>
    /// 权限TOKEN
    /// </summary>
    [property: Description("权限TOKEN")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 刷新TOKEN
    /// </summary>
    [property: Required]
    [property: Description("刷新TOKEN")]
    public string RefreshToken { get; set; } = string.Empty;
}
