using RestoreCord.Services;

IHostBuilder? builder = Host.CreateDefaultBuilder(args);
await builder.ConfigureWebHostDefaults(x =>
{
    x.ConfigureKestrel(options =>
    {
    });
    x.UseUrls("http://127.0.0.1:666");
    x.UseStartup<Startup>();
    x.SuppressStatusMessages(true);

}).Build().RunAsync();
