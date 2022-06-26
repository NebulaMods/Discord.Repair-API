using System.ComponentModel.DataAnnotations;
using RestoreCord.Database.Models.BackupModels;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public class User
{
    [Key]
    public int key { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string role { get; set; } = "free";
    public string pfp { get; set; } = "https://i.imgur.com/w65Dpnw.png";
    public bool banned { get; set; }
    public string? googleAuthCode { get; set; }
    public bool darkmode { get; set; }
    public DateTime? expiry { get; set; }
    public DateTime createdAt { get; set; } = DateTime.Now;
    public string? lastIP { get; set; }
    public ulong? discordId { get; set; }
    public string apiToken { get; set; } = "";//token gen method
    public virtual ICollection<CustomBot> bots { get; set; } = new HashSet<CustomBot>();
    public virtual ICollection<Backup> backups { get; set; } = new HashSet<Backup>();
}
