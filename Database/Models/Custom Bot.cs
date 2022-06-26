using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models;

public class CustomBot
{
    [Key]
    public int key { get; set; }
    public string name { get; set; }
    public string token { get; set; }
    public string clientSecret { get; set; }
    public string clientId { get; set; }
    public string urlRedirect { get; set; }
    public virtual BotType botType { get; set; } = BotType.EVERYTHING;
}

public enum BotType
{
    AUTH,
    MESSAGES,
    MAIN,
    EVERYTHING
}