using System.ComponentModel.DataAnnotations;

using DiscordRepair.Api.Database.Models;

namespace DiscordRepair.Api.Records.Requests.CustomBot;

/// <summary>
/// 
/// </summary>
public record CreateCustomBotRequest
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(64)]
    [Required]
    public string name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    [Required]
    public string token { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(100)]
    [Required]
    public string clientSecret { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [StringLength(25)]
    [Required]
    public string clientId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [Required]
    public virtual BotType botType { get; set; }
}

