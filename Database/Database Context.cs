using Microsoft.EntityFrameworkCore;
using RestoreCord.Database.Models;
using RestoreCord.Database.Models.LogModels;

namespace RestoreCord.Database;

public class DatabaseContext : DbContext
{
    private readonly string _connectionString = $"server=localhost;database=restorecord_main;user=restorecord_db;password={Properties.Resources.MySQLPass}";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)).UseLazyLoadingProxies().UseBatchEF_MySQLPomelo();

    public DbSet<Errors> errors { get; set; }
    public DbSet<Member> members { get; set; }
    public DbSet<Server> servers { get; set; }
    public DbSet<Blacklist> blacklist { get; set; }
    public DbSet<User> users { get; set; }
}
