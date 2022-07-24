using System.ComponentModel.DataAnnotations;

using RestoreCord.Database.Models.BackupModels;

namespace RestoreCord.Database.Models;

/// <summary>
/// 
/// </summary>
public record User
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [StringLength(32, MinimumLength = 4)]
    public string username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public string password { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    [StringLength(32)]
    public string role { get; set; } = "free";

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string pfp { get; set; } = "https://i.imgur.com/w65Dpnw.png";

    /// <summary>
    /// 
    /// </summary>
    public bool banned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? googleAuthCode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool darkmode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.DateTime)]
    public DateTime? expiry { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.DateTime)]
    public DateTime createdAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(64)]
    public string? lastIP { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? discordId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    [StringLength(100)]
    public string apiToken { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual ICollection<CustomBot> bots { get; set; } = new HashSet<CustomBot>();

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Custom)]
    public virtual ICollection<Backup> backups { get; set; } = new HashSet<Backup>();
}
