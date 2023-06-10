using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents a member of a Discord server that has been linked to a third-party service through a custom bot.
/// </summary>
public record Member
{
    /// <summary>
    /// Gets or sets the unique identifier for the member.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// Gets or sets the Discord ID of the member.
    /// </summary>
    [Required]
    public ulong discordId { get; set; }

    /// <summary>
    /// Gets or sets the server associated with the member.
    /// </summary>
    [DataType(DataType.Custom)]
    public virtual Server? server { get; set; }

    /// <summary>
    /// Gets or sets the access token for the third-party service.
    /// </summary>
    [StringLength(100)]
    [Required]
    public string accessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token for the third-party service.
    /// </summary>
    [StringLength(100)]
    [Required]
    public string refreshToken { get; set; }

    /// <summary>
    /// Gets or sets the IP address of the member.
    /// </summary>
    [StringLength(100)]
    public string? ip { get; set; }

    /// <summary>
    /// Gets or sets the avatar of the member.
    /// </summary>
    [StringLength(100)]
    public string? avatar { get; set; }

    /// <summary>
    /// Gets or sets the username of the member.
    /// </summary>
    [StringLength(100)]
    public string? username { get; set; }

    /// <summary>
    /// Gets or sets the date that the member was linked to the third-party service.
    /// </summary>
    public DateTime linkDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the custom bot used to link the member.
    /// </summary>
    [DataType(DataType.Custom)]
    [Required]
    public virtual CustomBot botUsed { get; set; }

}
