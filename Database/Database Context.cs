using Microsoft.EntityFrameworkCore;

using DiscordRepair.Database.Models;
using DiscordRepair.Database.Models.LogModels;

namespace DiscordRepair.Database;

/// <summary>
/// 
/// </summary>
public class DatabaseContext : DbContext
{
    private readonly string _connectionString = $"Host=localhost;Database=restorecord_main;User ID=restorecord_db;Password={Properties.Resources.MySQLPass}";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseNpgsql(_connectionString,x =>
    {
        
    }).UseBatchEF_Npgsql().UseLazyLoadingProxies();//_connectionString, ServerVersion.AutoDetect(_connectionString)).UseLazyLoadingProxies().UseBatchEF_MySQLPomelo();

    public DbSet<Errors> errors { get; set; }
    public DbSet<Member> members { get; set; }
    public DbSet<User> users { get; set; }
    public DbSet<Statistics> statistics { get; set; }
    public DbSet<Server> servers { get; set; }
}
