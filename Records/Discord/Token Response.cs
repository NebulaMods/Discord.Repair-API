using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Discord;

/// <summary>
/// Represents a token response returned by an OAuth server.
/// </summary>
public record TokenResponse
{
    /// <summary>
    /// The access token to use for API requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? access_token { get; set; }

    /// <summary>
    /// The time in seconds until the access token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long expires_in { get; set; }

    /// <summary>
    /// The refresh token to use to obtain a new access token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? refresh_token { get; set; }

    /// <summary>
    /// The scope of the access token.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? scope { get; set; }

    /// <summary>
    /// The type of the access token.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string? token_type { get; set; }
}
