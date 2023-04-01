using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Database.Models.LogModels;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Database;

/// <summary>
/// 
/// </summary>
public class DatabaseContext : DbContext
{

#if DEBUG
    private readonly string _connectionString = $"host=chicago-database-node-1.nebulamods.ca;user id=bot;database=discord_repair_api;password={Properties.Resources.MySQLPass}";
#else
private readonly string _connectionString = $"host=chicago-database-node-1.nebulamods.ca;user id=bot;database=discord_repair_api;password={Properties.Resources.MySQLPass}";
#endif
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseNpgsql(_connectionString, x =>
    {

    }).UseLazyLoadingProxies();

    public DbSet<Errors> errors { get; set; }
    public DbSet<Member> members { get; set; }
    public DbSet<User> users { get; set; }
    public DbSet<Statistics> statistics { get; set; }
    public DbSet<Server> servers { get; set; }
}
