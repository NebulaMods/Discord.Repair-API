namespace DiscordRepair.Api.Middleware.CORs;

/// <summary>
/// Provides extension methods for the <see cref="IApplicationBuilder"/> interface.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds middleware for handling CORS preflight requests.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to use.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseCorsOptions(this IApplicationBuilder builder)
    {
        // Use the Handler middleware to handle CORS preflight requests
        return builder.UseMiddleware<Handler>();
    }
}
