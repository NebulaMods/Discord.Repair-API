using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.LogModels;

public class Errors
{
    [Key]
    public int id { get; set; }
    public string? location { get; set; }
    public string? message { get; set; }
    public string? stackTrace { get; set; }
    public string? extraInfo { get; set; }
    public DateTime errorTime { get; set; }
}
