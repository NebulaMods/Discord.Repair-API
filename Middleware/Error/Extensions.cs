namespace DiscordRepair.Api.Middleware.Error;

public static class Extensions
{
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Handler>();
    }
}
