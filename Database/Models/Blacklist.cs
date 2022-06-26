using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public class Blacklist
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int key { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ulong? discordId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? ip { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? reason { get; set; }
}
