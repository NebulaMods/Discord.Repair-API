using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Discord;

/// <summary>
/// Represents information about the current user's OAuth2 access token.
/// </summary>
public record AboutMe
{
    /// <summary>
    /// Gets or sets the application associated with the access token.
    /// </summary>
    [JsonPropertyName("application")]
    public Application application { get; set; }

    /// <summary>
    /// Gets or sets the list of OAuth2 scopes granted by the access token.
    /// </summary>
    [JsonPropertyName("scopes")]
    public List<string> scopes { get; set; }

    /// <summary>
    /// Gets or sets the UTC time when the access token expires.
    /// </summary>
    [JsonPropertyName("expires")]
    public DateTime expires { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the access token.
    /// </summary>
    [JsonPropertyName("user")]
    public User user { get; set; }
}
