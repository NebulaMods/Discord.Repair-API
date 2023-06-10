using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Responses;

/// <summary>
/// Represents the response from the hCaptcha API.
/// </summary>
public record HCaptchaResponse
{
    /// <summary>
    /// Indicates whether the hCaptcha was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool success { get; set; }

    /// <summary>
    /// The timestamp of the hCaptcha challenge as a UTC DateTime object.
    /// </summary>
    [JsonPropertyName("challenge_ts")]
    public DateTime? challengeTimestamp { get; set; }

    /// <summary>
    /// The hostname of the website where the hCaptcha was solved.
    /// </summary>
    [JsonPropertyName("hostname")]
    public string? hostname { get; set; }

    /// <summary>
    /// A list of error codes if the hCaptcha was unsuccessful.
    /// </summary>
    [JsonPropertyName("error-codes")]
    public List<string?>? errorCodes { get; set; }

    /// <summary>
    /// A boolean value indicating whether the hCaptcha is a paid version.
    /// </summary>
    [JsonPropertyName("credit")]
    public bool? isCredit { get; set; }
}
