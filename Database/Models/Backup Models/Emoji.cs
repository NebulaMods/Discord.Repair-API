using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.BackupModels;

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
    [StringLength(100)]
    public string? name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? url { get; set; }
}
