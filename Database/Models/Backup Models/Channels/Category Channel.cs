using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels.Channel;

public class CategoryChannel
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string name { get; set; }
    public int position { get; set; }
    public virtual ICollection<Permissions.ChannelPermissions> permissions { get; set; } = new HashSet<Permissions.ChannelPermissions>();
}
