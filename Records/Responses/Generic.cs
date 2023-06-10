using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Responses;

/// <summary>
/// Represents a generic response with a success flag and optional details.
/// </summary>
public record Generic
{
    /// <summary>
    /// Gets or sets a value indicating whether the request was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool success { get; set; }

    /// <summary>
    /// Gets or sets additional details about the response.
    /// </summary>
    [JsonPropertyName("details")]
    public string? details { get; set; }
}
