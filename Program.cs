using RestoreCord.Events;
using RestoreCord.Services;

IHostBuilder? builder = Host.CreateDefaultBuilder(args);
var app = builder.ConfigureWebHostDefaults(x =>
{
    x.ConfigureKestrel(options =>
    {
    });
    x.UseUrls("http://127.0.0.1:666");
    x.UseStartup<Startup>();
    x.SuppressStatusMessages(true);

}).Build();
await app.Services.GetRequiredService<InteractionEventHandler>().InitializeAsync();
await app.RunAsync();
