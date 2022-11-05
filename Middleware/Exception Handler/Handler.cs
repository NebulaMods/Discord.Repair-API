namespace DiscordRepair.Middleware.ExceptionHandler;

/// <summary>
/// 
/// </summary>
public class Handler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);
    }
}
