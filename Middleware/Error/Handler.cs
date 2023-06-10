using DiscordRepair.Api.Records.Responses;

namespace DiscordRepair.Api.Middleware.Error;

/// <summary>
/// Middleware that catches and handles exceptions thrown by the application
/// </summary>
public class Handler
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="Handler"/> class.
    /// </summary>
    /// <param name="next">The next middleware component in the pipeline.</param>
    public Handler(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to catch and handle exceptions thrown by the application
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the context on to the next middleware component in the pipeline
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            // If an exception is caught, set the response status code to 400 (Bad Request)
            context.Response.StatusCode = 400;

            // Create a new instance of the Generic class to hold the details of the error
            var error = new Generic()
            {
                success = false,
                details = ex.Message
            };

            // Write the error details as a JSON response
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}

