using System.ComponentModel.DataAnnotations;

namespace RestoreCord.Database.Models.BackupModels.Permissions;

/// <summary>
/// 
/// </summary>
public record ChannelPermissions
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid key { get; init; } = new();

    /// <summary>
    /// 
    /// </summary>
    public ulong targetId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int permissionTarget { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue AttachFiles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue Speak { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue MuteMembers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public PermissionValue DeafenMembers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue MoveMembers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue UseVAD { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue PrioritySpeaker { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue Stream { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue UseApplicationCommands { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue ManageWebhooks { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue Connect { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue RequestToSpeak { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue ManageThreads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue CreatePublicThreads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue CreatePrivateThreads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue UseExternalStickers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue ManageRoles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue UseExternalEmojis { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue StartEmbeddedActivities { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// [EnumDataType(typeof(PermissionValue))]
    public PermissionValue ReadMessageHistory { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue MentionEveryone { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue ManageChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue AddReactions { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue CreateInstantInvite { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue SendMessages { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue SendTTSMessages { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue ManageMessages { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue EmbedLinks { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue SendMessagesInThreads { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public PermissionValue ViewChannel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue useVoiceActivation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue useSlashCommands { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// 
    [EnumDataType(typeof(PermissionValue))]
    public PermissionValue usePrivateThreads { get; set; }
}

/// <summary>
/// 
/// </summary>
public enum PermissionValue
{
    
    /// <summary>
    /// 
    /// </summary>
    Allow = 0,
    
    /// <summary>
    /// 
    /// </summary>
    Deny = 1,
   
    /// <summary>
    /// 
    /// </summary>
    Inherit = 2
}
