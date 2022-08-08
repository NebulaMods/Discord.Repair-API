using System.ComponentModel.DataAnnotations;

using DiscordRepair.Database.Models.BackupModels;

namespace DiscordRepair.Database.Models;

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
    public string email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    [EnumDataType(typeof(AccountType))]
    public virtual AccountType accountType{ get; set; } = AccountType.Free;

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(250)]
    [DataType(DataType.Url)]
    public string pfp { get; set; } = "https://nebulamods.ca/content/images/logo-nebulamods.png";

    /// <summary>
    /// 
    /// </summary>
    public bool banned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [DataType(DataType.Date)]
    public DateOnly? expiry { get; set; }

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
    public string apiToken { get; set; } = Utilities.Miscallenous.GenerateApiToken();

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

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<Blacklist> globalBlacklist { get; set; } = new HashSet<Blacklist>();
}

/// <summary>
/// 
/// </summary>
public enum AccountType
{
    /// <summary>
    /// 
    /// </summary>
    Free,
    /// <summary>
    /// 
    /// </summary>
    Premium,
    /// <summary>
    /// 
    /// </summary>
    Staff
}