using RestoreCord.Database.Models;

namespace RestoreCord.Records.Requests;

public record LinkUser
{
    public ulong userid { get; set; }
    public ulong server { get; set; }
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
    public string? ip { get; set; }
    public string? avatar { get; set; }
    public string? username { get; set; }
    public ulong? creationDate { get; set; }
    public MemberTokenType tokenType { get; set; } = MemberTokenType.RESTORECORD;
}

