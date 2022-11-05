namespace DiscordRepair.Middleware.ExceptionHandler;

/// <summary>
/// 
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<Handler>();
}
