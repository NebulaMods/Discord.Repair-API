using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels.Channel;

public class TextChannel
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string? name { get; set; }
    public int slowModeInterval { get; set; }
    public string? topic { get; set; }
    public virtual CategoryChannel? category { get; set; }
    public bool nsfw { get; set; }
    public int? archiveAfter { get; set; }
    public int position { get; set; }
    public bool locked { get; set; }
    public bool archived { get; set; }
    public bool synced { get; set; }
    public virtual ICollection<Permissions.ChannelPermissions> permissions { get; set; } = new HashSet<Permissions.ChannelPermissions>();
    public virtual ICollection<Message> messages { get; set; } = new HashSet<Message>();

}
