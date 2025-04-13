namespace Kun.Queue.Models;

/// <summary>
/// 
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string Host { get; set; } = string.Empty;

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
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public List<string> To { get; set; } = new List<string>();
}
