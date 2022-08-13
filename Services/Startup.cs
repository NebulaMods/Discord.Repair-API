using System.Reflection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Converters;

using DiscordRepair.Database;
using DiscordRepair.Middleware;
using DiscordRepair.Middleware.CORs;

namespace DiscordRepair.Services;

/// <summary>
/// 
/// </summary>
public class Startup
{
    /// <summary>
    /// 
    /// </summary>
    public Startup()
    {
        //var proxy = new Utilities.ProxyGenerator();
        //_ = proxy.LoadProxyListAsync("");
        //_client = new DiscordShardedClient(new DiscordSocketConfig
        //{
        //    LogLevel = LogSeverity.Debug,
        //    AlwaysDownloadUsers = false,
        //    GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildIntegrations,
        //    UseSystemClock = true,
        //    MessageCacheSize = 0,
        //    UseInteractionSnowflakeDate = true,
        //    LogGatewayIntentWarnings = false,
        //    DefaultRetryMode = RetryMode.RetryTimeouts,
        //    //RestClientProvider = DefaultRestClientProvider.Create(useProxy: true),
        //    //WebSocketProvider = DefaultWebSocketProvider.Create(proxy)
        //});
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ConfigureServices(IServiceCollection services)
    {
        using (var context = new DatabaseContext())
        {
            context.Database.Migrate();
        }
        services
            .AddSingleton<TokenLoader>()
            .AddSingleton<MigrationMaster.Restore>()
            .AddSingleton<MigrationMaster.Backup>()
            .AddSingleton<MigrationMaster.Pull>()
            .AddSingleton<MigrationMaster.Configuration>();
        //add other endpoints
        //configure cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            // requires using Microsoft.AspNetCore.Http;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        services.AddCors(x =>
        {
            x.AddDefaultPolicy(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
            });
        });
        //configure auth
        services.AddAuthentication(Authentication.Schemes.MainScheme).AddScheme<AuthenticationSchemeOptions, Authentication.Handler>(Authentication.Schemes.MainScheme, null);

        //configure authorization
        services.AddAuthorization(authorizationOptions =>
        {
            authorizationOptions.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(Authentication.Schemes.MainScheme)
            .RequireAuthenticatedUser().Build();
        });

        #region Swagger
        //configure json options for swagger
        services.AddControllers().AddNewtonsoftJson(jsonOptions =>
        {
            jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        //configure swagger
        services.AddSwaggerGen(swaggerGenOptions =>
        {
            //swagger document options
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Discord.Repair | API",
                Version = $"{Assembly.GetExecutingAssembly().GetName().Version}",
                Contact = new OpenApiContact()
                {
                    Email = "support@discord.repair",
                    Name = "Contact",
                    Url = new Uri("https://discord.repair")
                },
                Description = "Advanced discord features",
                TermsOfService = new Uri("https://discord.repair"),
            });

            //swagger auth
            swaggerGenOptions.AddSecurityDefinition(Authentication.Schemes.MainScheme, new OpenApiSecurityScheme
            {
                Description = "Please enter your API Token to gain access to the endpoints.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });
            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {{
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = Authentication.Schemes.MainScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                }, new List<string>()
            }});
            string? xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string? xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swaggerGenOptions.IncludeXmlComments(xmlPath);
            swaggerGenOptions.TagActionsBy(api =>
            {
                return api.GroupName != null
                    ? (new[] { api.GroupName })
                    : api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor
                    ? (new[] { controllerActionDescriptor.ControllerName })
                    : throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });
            swaggerGenOptions.DocInclusionPredicate((name, api) => true);
        });

        //swagger newtonsoft support
        services.AddSwaggerGenNewtonsoftSupport();
        #endregion
    }


    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    public void Configure(IApplicationBuilder app)
    {
        #if DEBUG
        app.UseDeveloperExceptionPage();
        #endif
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        //inject swagger
        app.UseSwagger();
        app.UseCORsOptions();
        //config for swagger ui
        app.UseSwaggerUI(swaggerOptions =>
        {
            swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Discord.Repair");
            swaggerOptions.RoutePrefix = string.Empty;
            swaggerOptions.DocumentTitle = "Discord.Repair | API";
            swaggerOptions.EnableDeepLinking();
            swaggerOptions.EnableValidator(null);
            swaggerOptions.EnablePersistAuthorization();
            swaggerOptions.EnableTryItOutByDefault();
            swaggerOptions.DisplayRequestDuration();
            swaggerOptions.ShowExtensions();
        });
        //inject cookies
        app.UseCookiePolicy();
        //inject routing
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        //inject endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
