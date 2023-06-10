using DiscordRepair.Api.Middleware;
using DiscordRepair.Api.Middleware.Error;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;

namespace DiscordRepair.Api.Services;

/// <summary>
/// Startup class that initializes the application and configures services and middleware.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    public Startup()
    {

    }

    /// <summary>
    /// Configure services that the application needs.
    /// </summary>
    /// <param name="services">Service collection to add services to.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseContext();

        var tokenLoader = new TokenLoader();
        services.AddSingleton(tokenLoader);

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            });
        });

        services.AddAuthentication(Authentication.Schemes.MainScheme)
                .AddScheme<AuthenticationSchemeOptions, Authentication.Handler>(Authentication.Schemes.MainScheme, null);

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Authentication.Schemes.MainScheme)
                .RequireAuthenticatedUser().Build();
        });

        services.AddSwagger();

        services.AddControllersWithJsonOptions();

        services.AddSwaggerGenNewtonsoftSupport();
    }

    /// <summary>
    /// Configure middleware pipeline for the application.
    /// </summary>
    /// <param name="app">Application builder to add middleware to.</param>
    public void Configure(IApplicationBuilder app)
    {
        // Use developer exception page if in debug mode
#if DEBUG
        app.UseDeveloperExceptionPage();
#endif

        // Forward headers from reverse proxy servers
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        // Enable CORS
        app.UseCors();

        // Add error handling middleware
        app.UseErrorHandler();

        // Enable Swagger UI
        app.UseSwagger();
        app.UseSwaggerUI(swaggerOptions =>
        {
            // Set the Swagger endpoint
            swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Discord.Repair");

            // Set the route prefix
            swaggerOptions.RoutePrefix = string.Empty;

            // Set the document title
            swaggerOptions.DocumentTitle = "Discord.Repair | API";

            // Enable deep linking
            swaggerOptions.EnableDeepLinking();

            // Enable validator
            swaggerOptions.EnableValidator(null);

            // Enable authorization persistence
            swaggerOptions.EnablePersistAuthorization();

            // Enable "Try it out" by default
            swaggerOptions.EnableTryItOutByDefault();

            // Display request duration
            swaggerOptions.DisplayRequestDuration();

            // Show extensions
            swaggerOptions.ShowExtensions();
        });

        // Enable cookies
        app.UseCookiePolicy();

        app.UseRouting();
        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
//public void ConfigureServices(IServiceCollection services)
//{
//    using (var context = new DatabaseContext())
//    {
//        context.Database.Migrate();
//    }
//    var tokenLoader = new TokenLoader();
//    services
//        .AddSingleton(tokenLoader);

//    //add other endpoints
//    //configure cookie policy
//    services.Configure<CookiePolicyOptions>(options =>
//    {
//        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
//        options.CheckConsentNeeded = context => true;
//        // requires using Microsoft.AspNetCore.Http;
//        //options.MinimumSameSitePolicy = SameSiteMode.None;
//    });
//    services.AddCors(x =>
//    {
//        x.AddDefaultPolicy(x =>
//        {
//            x.AllowAnyHeader();
//            x.AllowAnyMethod();
//            x.AllowAnyOrigin();
//        });
//    });
//    //configure auth
//    services.AddAuthentication(Authentication.Schemes.MainScheme).AddScheme<AuthenticationSchemeOptions, Authentication.Handler>(Authentication.Schemes.MainScheme, null);

//    //configure authorization
//    services.AddAuthorization(authorizationOptions =>
//    {
//        authorizationOptions.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
//        .AddAuthenticationSchemes(Authentication.Schemes.MainScheme)
//        .RequireAuthenticatedUser().Build();
//    });

//    #region Swagger
//    //configure json options for swagger
//    services.AddControllers().AddNewtonsoftJson(jsonOptions =>
//    {
//        jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
//    });

//    //configure swagger
//    services.AddSwaggerGen(swaggerGenOptions =>
//    {
//        //swagger document options
//        swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
//        {
//            Title = "Discord.Repair | API",
//            Version = $"{Assembly.GetExecutingAssembly().GetName().Version}",
//            Contact = new OpenApiContact()
//            {
//                Email = "support@discord.repair",
//                Name = "Contact",
//                Url = new Uri("https://discord.repair")
//            },
//            Description = "Advanced discord features",
//            TermsOfService = new Uri("https://discord.repair"),
//        });

//        //swagger auth
//        swaggerGenOptions.AddSecurityDefinition(Authentication.Schemes.MainScheme, new OpenApiSecurityScheme
//        {
//            Description = "Please enter your API Token to gain access to the endpoints.",
//            Name = "Authorization",
//            In = ParameterLocation.Header,
//            Type = SecuritySchemeType.ApiKey,
//        });
//        swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
//        {{
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Id = Authentication.Schemes.MainScheme,
//                    Type = ReferenceType.SecurityScheme
//                }
//            }, new List<string>()
//        }});
//        string? xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//        string? xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//        swaggerGenOptions.IncludeXmlComments(xmlPath);
//        swaggerGenOptions.TagActionsBy(api =>
//        {
//            return api.GroupName != null
//                ? (new[] { api.GroupName })
//                : api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
//                ? (new[] { controllerActionDescriptor.ControllerName })
//                : throw new InvalidOperationException("Unable to determine tag for endpoint.");
//        });
//        swaggerGenOptions.DocInclusionPredicate((name, api) => true);
//    });

//    //swagger newtonsoft support
//    services.AddSwaggerGenNewtonsoftSupport();
//    #endregion
//}
//    public void Configure(IApplicationBuilder app)
//    {
//#if DEBUG
//        app.UseDeveloperExceptionPage();
//#endif
//        app.UseForwardedHeaders(new ForwardedHeadersOptions
//        {
//            ForwardedHeaders = ForwardedHeaders.All
//        });

//        //inject swagger
//        app.UseSwagger();
//        app.UseCORsOptions();
//        app.UseErrorHandler();
//        //config for swagger ui
//        app.UseSwaggerUI(swaggerOptions =>
//        {
//            swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Discord.Repair");
//            swaggerOptions.RoutePrefix = string.Empty;
//            swaggerOptions.DocumentTitle = "Discord.Repair | API";
//            swaggerOptions.EnableDeepLinking();
//            swaggerOptions.EnableValidator(null);
//            swaggerOptions.EnablePersistAuthorization();
//            swaggerOptions.EnableTryItOutByDefault();
//            swaggerOptions.DisplayRequestDuration();
//            swaggerOptions.ShowExtensions();
//        });
//        //inject cookies
//        app.UseCookiePolicy();
//        //inject routing
//        app.UseRouting();
//        app.UseCors();
//        app.UseAuthentication();
//        app.UseAuthorization();

//        //inject endpoints
//        app.UseEndpoints(endpoints =>
//        {
//            endpoints.MapControllers();
//        });
//    }
