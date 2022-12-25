using System.ComponentModel.DataAnnotations;

using DiscordRepair.Api.Database.Models;

namespace DiscordRepair.Api.Records.Requests.CustomBot;

public record ModifyCustomBotRequest
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(64)]
    public string? name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? token { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    public string? clientSecret { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(25)]
    public string? clientId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(BotType))]
    public virtual BotType? botType { get; set; }
}

