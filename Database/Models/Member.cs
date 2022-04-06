using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

public class Member
{
    [Key]
    public int id { get; set; }
    public ulong userid { get; set; }
    public ulong server { get; set; }
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? ip { get; set; }
    public string? avatar { get; set; }
    public string? username { get; set; }
    public ulong? creationDate { get; set; }
}
