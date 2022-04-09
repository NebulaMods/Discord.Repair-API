using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels;

public class GuildUserRole
{
    [Key]
    public int key { get; set; }
    public virtual Role role { get; set; }
}
