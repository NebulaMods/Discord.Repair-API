using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels;

public class Emoji
{
    [Key]
    public int key { get; set; }
    public string? name { get; set; }
    public string? url { get; set; }
}
