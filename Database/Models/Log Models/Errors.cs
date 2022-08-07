using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.LogModels;

/// <summary>
/// 
/// </summary>
public record Errors
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
    [StringLength(50)]
    public string? location { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(200)]
    public string? message { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(300)]
    public string? stackTrace { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(300)]
    public string? extraInfo { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.DateTime)]
    public DateTime errorTime { get; init; } = DateTime.UtcNow;
}
