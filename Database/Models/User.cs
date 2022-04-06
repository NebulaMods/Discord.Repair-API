using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

public class User
{
    [Key]
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string role { get; set; }
    public string pfp { get; set; } = "https://i.imgur.com/w65Dpnw.png";
    public string? banned { get; set; }
    public bool twofactor { get; set; }
    public string? googleAuthCode { get; set; }
    public bool darkmode { get; set; }
    public int? expiry { get; set; }
    public bool admin { get; set; }
    public string? last_ip { get; set; }
    public ulong? userId { get; set; }
}
