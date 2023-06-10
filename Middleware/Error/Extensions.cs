namespace DiscordRepair.Api.Middleware.Error;

/// <summary>
/// Extension class to add middleware to handle errors in the application pipeline.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds the ErrorHandler middleware to the specified <see cref="IApplicationBuilder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
    {
        // UseMiddleware is an extension method provided by ASP.NET Core to add middleware to the pipeline.
        // It takes a type parameter that specifies the middleware to add to the pipeline.
        return builder.UseMiddleware<Handler>();
    }
}