using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels;

public class Message
{
    [Key]
    public int key { get; set; }
    public int position { get; set; }
    public string? content { get; set; }
    public string? username { get; set; }
    public ulong userId { get; set; }
    public string? userPicture { get; set; }
}
