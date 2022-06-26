using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

public class ServerSettings
{
    [Key]
    public int key { get; set; }
    public bool autoKickUnVerified { get; set; }
    public int autoKickUnVerifiedTime { get; set; } = 10;
    public bool autoJoin { get; set; } = false;
    public string? verifyDescription { get; set; }
    public int redirectTime { get; set; }
    public string? vanityUrl { get; set; }
    public int webhookLogType { get; set; }
    public bool dmOnAutoKick { get; set; } = false;
    public bool autoBlacklist { get; set; } = false;
    public string? redirectUrl { get; set; }
    public string pic { get; set; } = new string("https://i.imgur.com/w65Dpnw.png");
    public string? backgroundImage { get; set; }
    public bool vpnCheck { get; set; } = false;
    public string? webhook { get; set; }
    public virtual CustomBot? mainBot { get; set; }
    public virtual ICollection<Blacklist> blacklist { get; set; }
}
