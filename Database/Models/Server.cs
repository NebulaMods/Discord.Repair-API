using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public record Server
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
    [Required]
    [DataType(DataType.Custom)]
    public virtual User owner { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(50)]
    [Required]
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? guildId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? roleId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool banned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    [DataType(DataType.Custom)]
    public virtual ServerSettings settings { get; set; } = new();
}
