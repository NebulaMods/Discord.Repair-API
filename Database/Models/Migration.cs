using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents a record for migration
/// </summary>
public record Migration
{
    /// <summary>
    /// Gets or sets the unique identifier for the migration.
    /// </summary>
    [Key]
    public Guid key { get; set; }
    /// <summary>
    /// Gets or sets the server being migrated.
    /// </summary>
    public virtual Server? server { get; set; }

    /// <summary>
    /// Gets or sets the bot responsible for the migration.
    /// </summary>
    public virtual CustomBot? bot { get; set; }

    /// <summary>
    /// Gets or sets the start time of the migration in UTC.
    /// </summary>
    public DateTime startTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the completion time of the migration in UTC.
    /// </summary>
    public DateTime completionTime { get; set; }

    /// <summary>
    /// Gets or sets the current status of the migration.
    /// </summary>
    public MigrationStatus status { get; set; } = MigrationStatus.PENDING;

    /// <summary>
    /// Gets or sets the total number of members to be migrated.
    /// </summary>
    public long totalMemberAmount { get; set; }

    /// <summary>
    /// Gets or sets the ID of the new guild if the migration was successful.
    /// </summary>
    public ulong? newGuildId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the new role if the migration was successful.
    /// </summary>
    public ulong? newRoleId { get; set; }

    /// <summary>
    /// Gets or sets the number of members that failed to migrate.
    /// </summary>
    public ulong failedMemberAmount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of members that were successfully migrated.
    /// </summary>
    public ulong successfulMemberAmount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of invalid tokens used during migration.
    /// </summary>
    public ulong invalidTokenAmount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of members that were already migrated.
    /// </summary>
    public ulong alreadyMigratedMemberAmount { get; set; } = 0;

    /// <summary>
    /// Gets or sets any additional details about the migration.
    /// </summary>
    public string? extraDetails { get; set; }

    /// <summary>
    /// Gets or sets the user responsible for starting the migration.
    /// </summary>
    public virtual User? user { get; set; }

}

/// <summary>
/// Represents the status of a migration process.
/// </summary>
public enum MigrationStatus
{
    /// <summary>
    /// The migration process is pending.
    /// </summary>
    PENDING,
    /// <summary>
    /// The migration process has started.
    /// </summary>
    STARTED,

    /// <summary>
    /// The migration process has failed.
    /// </summary>
    FAILED,

    /// <summary>
    /// The migration process has been cancelled.
    /// </summary>
    CANCELLED,

    /// <summary>
    /// The migration process has been completed.
    /// </summary>
    COMPLETED,

    /// <summary>
    /// The migration process is in progress.
    /// </summary>
    INPROGRESS

}