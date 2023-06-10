using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Discord;

/// <summary>
/// Represents a user on Discord.
/// </summary>
public record User
{
    /// <summary>
    /// Gets or sets the ID of the user.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong id { get; set; }


    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    [JsonPropertyName("username")]
    public string? username { get; set; }

    /// <summary>
    /// Gets or sets the avatar hash of the user.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? avatar { get; set; }

    /// <summary>
    /// Gets or sets additional information about the user's avatar.
    /// </summary>
    [JsonPropertyName("avatar_decoration")]
    public object? avatar_decoration { get; set; }

    /// <summary>
    /// Gets or sets the discriminator of the user.
    /// </summary>
    [JsonPropertyName("discriminator")]
    public string? discriminator { get; set; }

    /// <summary>
    /// Gets or sets the public flags of the user.
    /// </summary>
    [JsonPropertyName("public_flags")]
    public int public_flags { get; set; }
}
