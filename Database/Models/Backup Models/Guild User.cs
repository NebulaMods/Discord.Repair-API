using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels;

public class GuildUser
{
    [Key]
    public int key { get; set; }
    public ulong id { get; set; }
    public string? username { get; set; }
    public string? avatarUrl { get; set; }
    public virtual ICollection<GuildUserRole> assignedRoles { get; set; } = new HashSet<GuildUserRole>();
}
