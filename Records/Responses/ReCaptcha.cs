using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Responses;

/// <summary>
/// Represents the response from the reCAPTCHA service.
/// </summary>
public record ReCaptchaResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the reCAPTCHA test was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool success { get; set; }

    /// <summary>
    /// Gets or sets the hostname of the site where the reCAPTCHA was solved.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string? hostname { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the challenge load in ISO format (yyyy-MM-dd'T'HH:mm:ssZZ).
    /// </summary>
    [JsonPropertyName("challenge_ts")]
    public DateTime challenge_ts { get; set; }
}
