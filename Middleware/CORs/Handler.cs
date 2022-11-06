namespace DiscordRepair.Middleware.CORs;

/// <summary>
/// 
/// </summary>
public class Handler
{
    private readonly RequestDelegate _next;
    /// <summary>
    /// 
    /// </summary>
    public Handler(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
            context.Response.StatusCode = 200;
            return context.Response.WriteAsync("OK");
        }

        return _next.Invoke(context);
    }
}
