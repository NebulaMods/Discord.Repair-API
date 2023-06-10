using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Api.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public record Emoji
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? url { get; set; }
}
