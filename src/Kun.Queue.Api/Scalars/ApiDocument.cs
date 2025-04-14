using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Kun.Queue.Scalars;

/// <summary>
/// 
/// </summary>
public class ApiDocument : IOpenApiDocumentTransformer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new() { Title = "微服务", Version = "v1", Description = "微服务相关接口" };
        return Task.CompletedTask;
    }
}
