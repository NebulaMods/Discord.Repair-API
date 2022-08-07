using DiscordRepair.Services;

IHostBuilder? builder = Host.CreateDefaultBuilder(args);
IHost? app = builder.ConfigureWebHostDefaults(x =>
{
    x.ConfigureKestrel(options =>
    {
    });
    x.UseUrls("http://127.0.0.1:420");
    x.UseStartup<Startup>();
    x.SuppressStatusMessages(true);

}).Build();
await app.RunAsync();
