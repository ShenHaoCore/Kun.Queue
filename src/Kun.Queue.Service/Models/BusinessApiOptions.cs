namespace Kun.Queue.Models;

/// <summary>
/// 
/// </summary>
public class BusinessApiOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string BaseAddress { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string RequestUri { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public int Timeout { get; set; }
}
