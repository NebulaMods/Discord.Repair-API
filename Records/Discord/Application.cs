using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Discord;

/// <summary>
/// Represents a generic record that can hold multiple fields with different data types.
/// </summary>
public record Application
{
    /// <summary>
    /// Gets or sets the ID of the application.
    /// </summary>
    [JsonPropertyName("id")]
    public string? id { get; set; }

    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    [JsonPropertyName("name")]
    public string? name { get; set; }

    /// <summary>
    /// Gets or sets the icon URL of the application.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? icon { get; set; }

    /// <summary>
    /// Gets or sets the description of the application.
    /// </summary>
    [JsonPropertyName("description")]
    public string? description { get; set; }

    /// <summary>
    /// Gets or sets the summary of the application.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? summary { get; set; }

    /// <summary>
    /// Gets or sets the type of the application.
    /// </summary>
    [JsonPropertyName("type")]
    public object? type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application has a webhook.
    /// </summary>
    [JsonPropertyName("hook")]
    public bool hook { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application's bot is publicly visible.
    /// </summary>
    [JsonPropertyName("bot_public")]
    public bool bot_public { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application's bot requires code grant.
    /// </summary>
    [JsonPropertyName("bot_require_code_grant")]
    public bool bot_require_code_grant { get; set; }

    /// <summary>
    /// Gets or sets the verification key of the application.
    /// </summary>
    [JsonPropertyName("verify_key")]
    public string? verify_key { get; set; }
}
