using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels.Channel;

public class VoiceChannel
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string? name { get; set; }
    public int? userLimit { get; set; }
    public int bitrate { get; set; }
    public string? region { get; set; }
    public string? videoQuality { get; set; }
    public int position { get; set; }
    public virtual CategoryChannel? category { get; set; }
    public bool synced { get; set; }
    public virtual ICollection<Permissions.ChannelPermissions> permissions { get; set; } = new HashSet<Permissions.ChannelPermissions>();
}
