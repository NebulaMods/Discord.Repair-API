namespace DiscordRepair.Middleware.CORs;

public static class Extensions
{
    public static IApplicationBuilder UseCORsOptions(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Handler>();
    }
}
