using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public class Member
{
    [Key]
    public int key { get; set; }
    public ulong discordId { get; set; }
    public ulong guildId { get; set; }
    public string? accessToken { get; set; }
    public string? refreshToken { get; set; }
    public string? ip { get; set; }
    public string? avatar { get; set; }
    public string? username { get; set; }
    public ulong? creationDate { get; set; }
    public virtual CustomBot? botUsed { get; set; }
}
