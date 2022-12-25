using DiscordRepair.Api.Database.Models;

namespace DiscordRepair.Api.Records.Responses.User;

public record GetUserResponse
{
    /// <summary>
    /// 
    /// </summary>
    public string username { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public virtual AccountType accountType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string pfp { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool banned { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public DateOnly? expiry { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public DateTime createdAt { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string? lastIP { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? discordId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    public string apiToken { get; set; }
}

public record GetAllUsersResponse
{
    public string username { get; set; }
    public string email { get; set; }
    public ulong? discordId { get; set; }
    public virtual AccountType accountType { get; set; }
    public DateTime creationDate { get; set; }
    public string? lastIp { get; set; }
    public DateOnly? expiry { get; set; }
}

