using System.Text.Json;

namespace Kun.Queue.Extensions;

/// <summary>
/// JSON 扩展
/// </summary>
public static class JsonExtension
{
    /// <summary>
    /// 将对象序列化为 JSON 字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象，不能为 <see langword="null" />。</param>
    /// <param name="options">序列化选项，可为 <see langword="null" />（使用默认配置）。</param>
    /// <returns>序列化后的 JSON 字符串。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="obj" /> 为 <see langword="null" /> 时抛出。</exception>
    public static string ToJson(this object obj, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">目标类型，必须为引用类型。</typeparam>
    /// <param name="json">要反序列化的 JSON 字符串，不能为 <see langword="null" />。</param>
    /// <param name="options">反序列化选项，可为 <see langword="null" />（使用默认配置）。</param>
    /// <returns>反序列化后的对象，或 <see langword="null" />（如果 JSON 为空或无法反序列化）。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="json" /> 为 <see langword="null" /> 时抛出。</exception>
    public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json, nameof(json));
        return JsonSerializer.Deserialize<T>(json, options);
    }
}
