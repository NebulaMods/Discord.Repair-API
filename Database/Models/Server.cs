using System.ComponentModel.DataAnnotations;
using RestoreCord.Database.Models.BackupModels;

namespace RestoreCord.Database.Models;

public class Server
{
    [Key]
    public int id { get; set; }
    public string owner { get; set; }
    public string name { get; set; }
    public string? bg_image { get; set; }
    public ulong? guildid { get; set; }
    public ulong? roleid { get; set; }
    public string? redirecturl { get; set; }
    public string pic { get; set; } = new string("https://i.imgur.com/w65Dpnw.png");
    public bool? vpncheck { get; set; }
    public string? webhook { get; set; }
    public string? banned { get; set; }
    public bool autoKickUnVerified { get; set; }
    public int autoKickUnVerifiedTime { get; set; } = 10;
    public virtual Backup? backup { get; set; }
    public bool autoJoin { get; set; }
    public string? verifyDescription { get; set; }
    public int redirectTime { get; set; }
    public string? vanityUrl { get; set; }
    public int webhookLogType { get; set; }
}
