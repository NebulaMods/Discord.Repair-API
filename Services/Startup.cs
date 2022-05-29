using System.Diagnostics;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using RestoreCord.Database;
using RestoreCord.Events;

namespace RestoreCord.Services;

/// <summary>
/// 
/// </summary>
public class Startup
{
    private DiscordShardedClient _client;
    /// <summary>
    /// 
    /// </summary>
    public Startup()
    {
        //var proxy = new Utilities.ProxyGenerator();
        //_ = proxy.LoadProxyListAsync("");
        _client = new DiscordShardedClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Debug,
            AlwaysDownloadUsers = false,
            GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.Guilds | GatewayIntents.GuildIntegrations,
            UseSystemClock = true,
            MessageCacheSize = 0,
            UseInteractionSnowflakeDate = true,
            LogGatewayIntentWarnings = false,
            DefaultRetryMode = RetryMode.RetryTimeouts,
            //RestClientProvider = DefaultRestClientProvider.Create(useProxy: true),
            //WebSocketProvider = DefaultWebSocketProvider.Create(proxy)
        });
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ConfigureServices(IServiceCollection services)
    {
        using (var context = new DatabaseContext()) context.Database.Migrate();
        services.AddSingleton(_client)
            .AddSingleton<InteractionEventHandler>()
            .AddSingleton<Endpoints.Guild.User.Blacklist>()
            .AddSingleton<Commands.Blacklist>()
            .AddSingleton<Commands.Restore>()
            .AddSingleton<Migrations.Restore>()
            .AddSingleton<Migrations.Backup>()
            .AddSingleton<Migrations.Configuration>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordShardedClient>(), new InteractionServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info,
            }));
        //add other endpoints
        DiscordBot(new CancellationToken()).ConfigureAwait(false);
        //configure cookie policy
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            // requires using Microsoft.AspNetCore.Http;
            options.MinimumSameSitePolicy = SameSiteMode.None;
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
                Title = "RestoreCord | API",
                Version = $"{Assembly.GetExecutingAssembly().GetName().Version}",
                Contact = new OpenApiContact()
                {
                    Email = "support@restorecord.com",
                    Name = "Contact",
                    Url = new Uri("https://restorecord.comt")
                },
                Description = "Advanced discord features",
                TermsOfService = new Uri("https://restorecord.com"),
            });

            //swagger auth
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

    private async Task DiscordBot(CancellationToken cancellationToken)
    {
        if (Debugger.IsAttached)
            await _client.LoginAsync(TokenType.Bot, Properties.Resources.TestToken);
        else
            await _client.LoginAsync(TokenType.Bot, Properties.Resources.Token);
        await _client.StartAsync();
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

        //config for swagger ui
        app.UseSwaggerUI(swaggerOptions =>
        {
            swaggerOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "RestoreCord");
            swaggerOptions.RoutePrefix = string.Empty;
            swaggerOptions.DocumentTitle = "RestoreCord | API";
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
        app.UseCors(x =>
        {
            x.AllowAnyMethod();
            x.AllowAnyHeader();
            x.AllowAnyOrigin();
        });

        //inject endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
