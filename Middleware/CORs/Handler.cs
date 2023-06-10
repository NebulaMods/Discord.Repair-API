namespace DiscordRepair.Api.Middleware.CORs;

/// <summary>
/// Middleware that enables Cross-Origin Resource Sharing (CORS) for the application.
/// </summary>
public class Handler
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="Handler"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public Handler(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware and handles CORS preflight requests.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task InvokeAsync(HttpContext context)
    {
        // If the request method is OPTIONS, this is a CORS preflight request
        if (context.Request.Method == "OPTIONS")
        {
            // Set the Access-Control-Allow-Origin header to allow requests from any origin
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // Set the Access-Control-Allow-Headers header to allow all headers in the request
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "*" });

            // Set the Access-Control-Allow-Methods header to allow all HTTP methods
            context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "*" });

            // Set the response status code to 200 OK
            context.Response.StatusCode = 200;

            // Write "OK" to the response body
            return context.Response.WriteAsync("OK");
        }

        // If this is not a preflight request, pass the request on to the next middleware in the pipeline
        return _next.Invoke(context);
    }
}
