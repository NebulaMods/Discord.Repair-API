using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.BackupModels;

/// <summary>
/// 
/// </summary>
public record Sticker
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();
}
