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
    public int id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ulong? userid { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string? ip { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public ulong? server { get; set; }
}
