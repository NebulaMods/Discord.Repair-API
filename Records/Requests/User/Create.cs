using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace DiscordRepair.Records.Requests.User;

public record CreateUserRequest
{
    [Required]
    [StringLength(32)]
    public string username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    [StringLength(100)]
    public string email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100)]
    public string password { get; set; }

    [Required]
    //[StringLength(800, MinimumLength = 250)]
    //[JsonProperty("h-captcha-response")]
    public string captchaCode { get; set; }
}

