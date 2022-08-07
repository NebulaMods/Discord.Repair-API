using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Records.Requests.User;

public record TokenRequest
{
    [StringLength(32)]
    public string? username { get; set; }

    [StringLength(100)]
    [DataType(DataType.EmailAddress)]
    public string? email { get; set; }

    [Required]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public string password { get; set; }
}

