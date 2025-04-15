namespace Kun.Queue.Options;

/// <summary>
/// 
/// </summary>
public class BusinessApiOption
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
