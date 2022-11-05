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
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)context.Request.Headers["Origin"] });
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept" });
            context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync("OK");
            return;
        }

        await _next.Invoke(context);
    }
}
