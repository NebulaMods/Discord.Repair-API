using DiscordRepair.Api.Records.Responses;

namespace DiscordRepair.Api.Middleware.Error;

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
        try
        {
            return _next.Invoke(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 400;
            return context.Response.WriteAsJsonAsync(new Generic()
            {
                success = false,
                details = ex.Message
            });
        }
    }
}
