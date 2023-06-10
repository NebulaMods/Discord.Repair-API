using System.Text.Json.Serialization;

namespace DiscordRepair.Api.Records.Responses;

/// <summary>
/// Represents the API statistics response.
/// </summary>
public record APIStatsResponse
{
    /// <summary>
    /// Gets or sets the number of servers that have been backed up.
    /// </summary>
    [JsonPropertyName("server_count")]
    public int serverCount { get; set; }

    /// <summary>
    /// Gets or sets the number of users who have been backed up.
    /// </summary>
    [JsonPropertyName("user_count")]
    public int userCount { get; set; }

    /// <summary>
    /// Gets or sets the number of linked members who have been backed up.
    /// </summary>
    [JsonPropertyName("linked_member_count")]
    public int linkedMemberCount { get; set; }

    /// <summary>
    /// Gets or sets the number of backups that have been created.
    /// </summary>
    [JsonPropertyName("backup_count")]
    public int backupCount { get; set; }

    /// <summary>
    /// Gets or sets the number of backed up roles.
    /// </summary>
    [JsonPropertyName("backed_up_role_count")]

    public int backedupRoleCount { get; set; }

    /// <summary>
    /// Gets or sets the number of backed up channels.
    /// </summary>
    [JsonPropertyName("backed_up_channel_count")]
    public int backedupChannelCount { get; set; }

    /// <summary>
    /// Gets or sets the number of backed up members.
    /// </summary>
    [JsonPropertyName("backed_up_member_count")]
    public int backedupMemberCount { get; set; }
}
