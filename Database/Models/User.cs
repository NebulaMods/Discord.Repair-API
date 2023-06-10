using System.ComponentModel.DataAnnotations;

using DiscordRepair.Api.Database.Models.BackupModels;

namespace DiscordRepair.Api.Database.Models;

/// <summary>
/// Represents a user in the application.
/// </summary>
public record User
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    [Required]
    [StringLength(32, MinimumLength = 4)]
    public string username { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string email { get; set; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }

    /// <summary>
    /// Gets or sets the account type of the user.
    /// </summary>
    [Required]
    [EnumDataType(typeof(AccountType))]
    public virtual AccountType accountType { get; set; } = AccountType.Free;

    /// <summary>
    /// Gets or sets the profile picture URL of the user.
    /// </summary>
    [StringLength(250)]
    [DataType(DataType.Url)]
    public string pfp { get; set; } = "https://nebulamods.ca/content/images/logo-nebulamods.png";

    /// <summary>
    /// Gets or sets a value indicating whether the user is banned.
    /// </summary>
    public bool banned { get; set; }

    /// <summary>
    /// Gets or sets the expiry date of the user's account.
    /// </summary>
    [DataType(DataType.Date)]
    public DateOnly? expiry { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time when the user's account was created.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime createdAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the IP address of the user's last login.
    /// </summary>
    [StringLength(64)]
    public string? lastIP { get; set; }

    /// <summary>
    /// Gets or sets the Discord ID of the user.
    /// </summary>
    public ulong? discordId { get; set; }

    /// <summary>
    /// Gets or sets the API token of the user.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string apiToken { get; set; } = Utilities.Miscallenous.GenerateApiToken();

    /// <summary>
    /// Gets or sets the collection of custom bots created by the user.
    /// </summary>
    [DataType(DataType.Custom)]
    public virtual ICollection<CustomBot> bots { get; set; } = new HashSet<CustomBot>();

    /// <summary>
    /// Gets or sets the collection of backups created by the user.
    /// </summary>
    [DataType(DataType.Custom)]
    public virtual ICollection<Models.BackupModels.Backup> backups { get; set; } = new HashSet<Models.BackupModels.Backup>();

    /// <summary>
    /// Gets or sets the collection of blacklisted servers for the user.
    /// </summary>
    public virtual ICollection<Blacklist> globalBlacklist { get; set; } = new HashSet<Blacklist>();
}

/// <summary>
/// Represents the different account types available.
/// </summary>
public enum AccountType
{
    /// <summary>
    /// A free account.
    /// </summary>
    Free,
    /// <summary>
    /// A premium account.
    /// </summary>
    Premium,

    /// <summary>
    /// A staff account.
    /// </summary>
    Staff
}