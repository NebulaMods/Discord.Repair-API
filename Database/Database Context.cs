using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Database.Models.LogModels;

using Microsoft.EntityFrameworkCore;

namespace DiscordRepair.Api.Database;
/// <summary>
/// Represents the database context for the application.
/// </summary>
public class DatabaseContext : DbContext
{
    // Connection string used to connect to the database.
    private readonly string _connectionString;

    /// <summary>
    /// Creates a new instance of the <see cref="DatabaseContext"/> class.
    /// </summary>
    public DatabaseContext()
    {
        // Determine the appropriate connection string based on whether the application is in DEBUG mode or not.
#if DEBUG
        _connectionString = $"host=chicago-database-node-1.nebulamods.ca;user id=bot;database=discord_repair_api;password={Properties.Resources.MySQLPass}";
#else
        _connectionString = $"host=chicago-database-node-1.nebulamods.ca;user id=bot;database=discord_repair_api;password={Properties.Resources.MySQLPass}";
#endif
    }

    /// <summary>
    /// Configures the options for the database context.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Use PostgreSQL with the specified connection string and enable lazy loading proxies.
        options.UseNpgsql(_connectionString, x => { }).UseLazyLoadingProxies();
    }

    // Define the entity sets for the database context.

    /// <summary>
    /// Gets or sets the entity set for the Errors table.
    /// </summary>
    public DbSet<Errors> errors { get; set; }

    /// <summary>
    /// Gets or sets the entity set for the Members table.
    /// </summary>
    public DbSet<Member> members { get; set; }

    /// <summary>
    /// Gets or sets the entity set for the Users table.
    /// </summary>
    public DbSet<User> users { get; set; }

    /// <summary>
    /// Gets or sets the entity set for the Migrations table.
    /// </summary>
    public DbSet<Migration> migrations { get; set; }

    /// <summary>
    /// Gets or sets the entity set for the Servers table.
    /// </summary>
    public DbSet<Server> servers { get; set; }
}

