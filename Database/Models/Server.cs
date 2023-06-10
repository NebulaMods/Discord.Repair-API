using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents a server in the application.
/// </summary>
public record Server
{
    /// <summary>
    /// Gets or sets the unique identifier of the server.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// Gets or sets the owner of the server.
    /// </summary>
    [Required]
    [DataType(DataType.Custom)]
    public virtual User owner { get; set; }

    /// <summary>
    /// Gets or sets the name of the server.
    /// </summary>
    [StringLength(50)]
    [Required]
    public string name { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the Discord guild associated with this server.
    /// </summary>
    [Required]
    public ulong guildId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the role associated with this server.
    /// </summary>
    public ulong? roleId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this server is banned.
    /// </summary>
    public bool banned { get; set; }

    /// <summary>
    /// Gets or sets the settings of this server.
    /// </summary>
    [Required]
    [DataType(DataType.Custom)]
    public virtual ServerSettings settings { get; set; } = new();
}
