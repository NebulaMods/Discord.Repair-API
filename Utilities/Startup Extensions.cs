using System.Reflection;

using DiscordRepair.Api.Database;
using DiscordRepair.Api.Middleware;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Converters;

namespace DiscordRepair.Api.Utilities;

/// <summary>
/// Provides extension methods for configuring the application's services.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Configures and migrates the database context.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    public static void AddDatabaseContext(this IServiceCollection services)
    {
        // Initialize the database context and migrate the database
        using var context = new DatabaseContext();
        context.Database.Migrate();
    }

    /// <summary>
    /// Adds Swagger to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    public static void AddSwagger(this IServiceCollection services)
    {
        // Configure Swagger options
        services.AddSwaggerGen(options =>
        {
            // Define the API documentation information
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Discord.Repair | API",
                Version = $"{Assembly.GetExecutingAssembly().GetName().Version}",
                Contact = new OpenApiContact
                {
                    Email = "support@discord.repair",
                    Name = "Contact",
                    Url = new Uri("https://discord.repair")
                },
                Description = "Advanced discord features",
                TermsOfService = new Uri("https://discord.repair"),
            });

            // Define the API authentication requirements
            options.AddSecurityDefinition(Authentication.Schemes.MainScheme, new OpenApiSecurityScheme
            {
                Description = "Please enter your API Token to gain access to the endpoints.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = Authentication.Schemes.MainScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
        });

            // Include XML comments for API documentation
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            // Group API actions by their tags or controller names
            options.TagActionsBy(api =>
            {
                return api.GroupName != null
                    ? new[] { api.GroupName }
                    : api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
                        ? new[] { controllerActionDescriptor.ControllerName }
                        : throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });

            // Include all API actions in the documentation
            options.DocInclusionPredicate((name, api) => true);
        });
    }

    /// <summary>
    /// Adds controllers with JSON options to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    public static void AddControllersWithJsonOptions(this IServiceCollection services)
    {
        // Add controllers with Newtonsoft.Json options
        services.AddControllers().AddNewtonsoftJson(jsonOptions =>
        {
            // Add a StringEnumConverter to the serializer settings
            jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
    }
}

