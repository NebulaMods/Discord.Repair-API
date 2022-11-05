using DiscordRepair.Database.Models;

namespace DiscordRepair.Records.Responses.User;

public record ModifyUserRequest
{
    /// <summary>
    /// 
    /// </summary>
    public string? username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? password { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public virtual AccountType? accountType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? pfp { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? banned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public DateOnly? expiry { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? lastIP { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? discordId { get; set; }
}
