using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents a record in the blacklist table, which stores information about blacklisted Discord users and IP addresses.
/// </summary>
public record Blacklist
{
    /// <summary>
    /// Gets or sets the unique identifier for the blacklist record.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// Gets or sets the ID of the blacklisted Discord user, if applicable.
    /// </summary>
    public ulong? discordId { get; set; }

    /// <summary>
    /// Gets or sets the blacklisted IP address, if applicable.
    /// </summary>
    [StringLength(100)]
    public string? ip { get; set; }

    /// <summary>
    /// Gets or sets the reason for the blacklist.
    /// </summary>
    [StringLength(200)]
    public string? reason { get; set; }
}
