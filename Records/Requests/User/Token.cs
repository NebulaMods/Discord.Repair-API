using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace DiscordRepair.Records.Requests.User;

/// <summary>
/// 
/// </summary>
public record TokenRequest
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [StringLength(100)]
    public string user { get; set; }

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
    [Required]
    //[StringLength(800, MinimumLength = 250)]
    //[JsonProperty("h-captcha-response")]
    public string captchaCode { get; set; }
}

