using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public record CustomBot
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
    public string? token { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? clientSecret { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? clientId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(150)]
    public string? urlRedirect { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(BotType))]
    public virtual BotType botType { get; set; } = BotType.EVERYTHING;
}

/// <summary>
/// 
/// </summary>
public enum BotType
{
    /// <summary>
    /// 
    /// </summary>
    AUTH,
    /// <summary>
    /// 
    /// </summary>
    MESSAGES,
    /// <summary>
    /// 
    /// </summary>
    MAIN,
    /// <summary>
    /// 
    /// </summary>
    EVERYTHING
}