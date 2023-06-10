using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents a custom bot used in the system.
/// </summary>
public record CustomBot
{
    /// <summary>
    /// The unique identifier for the custom bot.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// The name of the custom bot.
    /// </summary>
    [StringLength(64)]
    [Required]
    public string name { get; set; }

    /// <summary>
    /// The access token for the custom bot.
    /// </summary>
    [StringLength(100)]
    [Required]
    public string token { get; set; }

    /// <summary>
    /// The client secret for the custom bot.
    /// </summary>
    [StringLength(100)]
    [Required]
    public string clientSecret { get; set; }

    /// <summary>
    /// The client ID for the custom bot.
    /// </summary>
    [StringLength(25)]
    [Required]
    public string clientId { get; set; }

    /// <summary>
    /// The type of the custom bot.
    /// </summary>
    [EnumDataType(typeof(BotType))]
    public virtual BotType botType { get; set; } = BotType.EVERYTHING;

}

/// <summary>
/// Represents the type of a custom bot.
/// </summary>
public enum BotType
{
    /// <summary>
    /// A bot used for authentication.
    /// </summary>
    AUTH,

    /// <summary>
    /// A bot used for messaging.
    /// </summary>
    MESSAGES,

    /// <summary>
    /// The main bot used for various purposes.
    /// </summary>
    MAIN,

    /// <summary>
    /// A bot used for all purposes.
    /// </summary>
    EVERYTHING
}
