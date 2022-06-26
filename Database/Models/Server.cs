using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public class Server
{
    [Key]
    public int key { get; set; }
    public virtual User owner { get; set; }
    public string name { get; set; }
    public ulong? guildId { get; set; }
    public ulong? roleId { get; set; }
    public bool banned { get; set; }
    public virtual ServerSettings settings { get; set; } = new();
}
