using System.ComponentModel.DataAnnotations;

namespace DiscordRepair.Database.Models.BackupModels.Permissions;

/// <summary>
/// 
/// </summary>
public record RolePermissions
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();
    
    /// <summary>
    /// 
    /// </summary>
    public bool Speak { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool MuteMembers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool DeafenMembers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool MoveMembers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseVAD { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool PrioritySpeaker { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool Stream { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ChangeNickname { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageNicknames { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageEmojisAndStickers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageWebhooks { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool Connect { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseApplicationCommands { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool RequestToSpeak { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageEvents { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageThreads { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool CreatePublicThreads { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool CreatePrivateThreads { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseExternalStickers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageRoles { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool UseExternalEmojis { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool AttachFiles { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ReadMessageHistory { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool CreateInstantInvite { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool BanMembers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool KickMembers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    //     channel permissions.
    public bool Administrator { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool MentionEveryone { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageGuild { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool AddReactions { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageChannels { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ViewGuildInsights { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ViewChannel { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool SendMessages { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool SendTTSMessages { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ManageMessages { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool EmbedLinks { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool SendMessagesInThreads { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool ViewAuditLog { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool StartEmbeddedActivities { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool useVoiceActivation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool moderateMembers { get; set; }
}
