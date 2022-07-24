using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels;

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
