using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Migrations;

public partial class InitialCreation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "errors",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                message = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                stackTrace = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                extraInfo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                errorTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_errors", x => x.key);
            });

        migrationBuilder.CreateTable(
            name: "GuildMigration",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                totalTime = table.Column<TimeSpan>(type: "interval", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildMigration", x => x.key);
            });

        migrationBuilder.CreateTable(
            name: "MemberMigration",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                totalTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                successCount = table.Column<int>(type: "integer", nullable: false),
                bannedCount = table.Column<int>(type: "integer", nullable: false),
                tooManyGuildsCount = table.Column<int>(type: "integer", nullable: false),
                invalidTokenCount = table.Column<int>(type: "integer", nullable: false),
                alreadyHereCount = table.Column<int>(type: "integer", nullable: false),
                failedCount = table.Column<int>(type: "integer", nullable: false),
                totalCount = table.Column<int>(type: "integer", nullable: false),
                blacklistedCount = table.Column<int>(type: "integer", nullable: false),
                estimatedCompletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberMigration", x => x.key);
            });

        migrationBuilder.CreateTable(
            name: "RolePermissions",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                Speak = table.Column<bool>(type: "boolean", nullable: false),
                MuteMembers = table.Column<bool>(type: "boolean", nullable: false),
                DeafenMembers = table.Column<bool>(type: "boolean", nullable: false),
                MoveMembers = table.Column<bool>(type: "boolean", nullable: false),
                UseVAD = table.Column<bool>(type: "boolean", nullable: false),
                PrioritySpeaker = table.Column<bool>(type: "boolean", nullable: false),
                Stream = table.Column<bool>(type: "boolean", nullable: false),
                ChangeNickname = table.Column<bool>(type: "boolean", nullable: false),
                ManageNicknames = table.Column<bool>(type: "boolean", nullable: false),
                ManageEmojisAndStickers = table.Column<bool>(type: "boolean", nullable: false),
                ManageWebhooks = table.Column<bool>(type: "boolean", nullable: false),
                Connect = table.Column<bool>(type: "boolean", nullable: false),
                UseApplicationCommands = table.Column<bool>(type: "boolean", nullable: false),
                RequestToSpeak = table.Column<bool>(type: "boolean", nullable: false),
                ManageEvents = table.Column<bool>(type: "boolean", nullable: false),
                ManageThreads = table.Column<bool>(type: "boolean", nullable: false),
                CreatePublicThreads = table.Column<bool>(type: "boolean", nullable: false),
                CreatePrivateThreads = table.Column<bool>(type: "boolean", nullable: false),
                UseExternalStickers = table.Column<bool>(type: "boolean", nullable: false),
                ManageRoles = table.Column<bool>(type: "boolean", nullable: false),
                UseExternalEmojis = table.Column<bool>(type: "boolean", nullable: false),
                AttachFiles = table.Column<bool>(type: "boolean", nullable: false),
                ReadMessageHistory = table.Column<bool>(type: "boolean", nullable: false),
                CreateInstantInvite = table.Column<bool>(type: "boolean", nullable: false),
                BanMembers = table.Column<bool>(type: "boolean", nullable: false),
                KickMembers = table.Column<bool>(type: "boolean", nullable: false),
                Administrator = table.Column<bool>(type: "boolean", nullable: false),
                MentionEveryone = table.Column<bool>(type: "boolean", nullable: false),
                ManageGuild = table.Column<bool>(type: "boolean", nullable: false),
                AddReactions = table.Column<bool>(type: "boolean", nullable: false),
                ManageChannels = table.Column<bool>(type: "boolean", nullable: false),
                ViewGuildInsights = table.Column<bool>(type: "boolean", nullable: false),
                ViewChannel = table.Column<bool>(type: "boolean", nullable: false),
                SendMessages = table.Column<bool>(type: "boolean", nullable: false),
                SendTTSMessages = table.Column<bool>(type: "boolean", nullable: false),
                ManageMessages = table.Column<bool>(type: "boolean", nullable: false),
                EmbedLinks = table.Column<bool>(type: "boolean", nullable: false),
                SendMessagesInThreads = table.Column<bool>(type: "boolean", nullable: false),
                ViewAuditLog = table.Column<bool>(type: "boolean", nullable: false),
                StartEmbeddedActivities = table.Column<bool>(type: "boolean", nullable: false),
                useVoiceActivation = table.Column<bool>(type: "boolean", nullable: false),
                moderateMembers = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => x.key);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                accountType = table.Column<int>(type: "integer", nullable: false),
                pfp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                banned = table.Column<bool>(type: "boolean", nullable: false),
                expiry = table.Column<DateOnly>(type: "date", nullable: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                lastIP = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                discordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                apiToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.key);
            });

        migrationBuilder.CreateTable(
            name: "CustomBot",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                token = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                clientSecret = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                clientId = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                botType = table.Column<int>(type: "integer", nullable: false),
                Userkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomBot", x => x.key);
                table.ForeignKey(
                    name: "FK_CustomBot_users_Userkey",
                    column: x => x.Userkey,
                    principalTable: "users",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "ServerSettings",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                vanityUrl = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                webhookLogType = table.Column<int>(type: "integer", nullable: false),
                redirectUrl = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                pic = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                backgroundImage = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                vpnCheck = table.Column<bool>(type: "boolean", nullable: false),
                webhook = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                mainBotkey = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServerSettings", x => x.key);
                table.ForeignKey(
                    name: "FK_ServerSettings_CustomBot_mainBotkey",
                    column: x => x.mainBotkey,
                    principalTable: "CustomBot",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Blacklist",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                discordId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                ip = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                ServerSettingskey = table.Column<Guid>(type: "uuid", nullable: true),
                Userkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Blacklist", x => x.key);
                table.ForeignKey(
                    name: "FK_Blacklist_ServerSettings_ServerSettingskey",
                    column: x => x.ServerSettingskey,
                    principalTable: "ServerSettings",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_Blacklist_users_Userkey",
                    column: x => x.Userkey,
                    principalTable: "users",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "servers",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                ownerkey = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                guildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                roleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                banned = table.Column<bool>(type: "boolean", nullable: false),
                settingskey = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_servers", x => x.key);
                table.ForeignKey(
                    name: "FK_servers_ServerSettings_settingskey",
                    column: x => x.settingskey,
                    principalTable: "ServerSettings",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_servers_users_ownerkey",
                    column: x => x.ownerkey,
                    principalTable: "users",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "members",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                discordId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                serverkey = table.Column<Guid>(type: "uuid", nullable: false),
                accessToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                refreshToken = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                ip = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                avatar = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                creationDate = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                botUsedkey = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_members", x => x.key);
                table.ForeignKey(
                    name: "FK_members_CustomBot_botUsedkey",
                    column: x => x.botUsedkey,
                    principalTable: "CustomBot",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_members_servers_serverkey",
                    column: x => x.serverkey,
                    principalTable: "servers",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "statistics",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                serverkey = table.Column<Guid>(type: "uuid", nullable: true),
                guildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                MigratedBykey = table.Column<Guid>(type: "uuid", nullable: true),
                active = table.Column<bool>(type: "boolean", nullable: false),
                startDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                endDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                memberStatskey = table.Column<Guid>(type: "uuid", nullable: true),
                guildStatskey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_statistics", x => x.key);
                table.ForeignKey(
                    name: "FK_statistics_GuildMigration_guildStatskey",
                    column: x => x.guildStatskey,
                    principalTable: "GuildMigration",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_statistics_MemberMigration_memberStatskey",
                    column: x => x.memberStatskey,
                    principalTable: "MemberMigration",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_statistics_servers_serverkey",
                    column: x => x.serverkey,
                    principalTable: "servers",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_statistics_users_MigratedBykey",
                    column: x => x.MigratedBykey,
                    principalTable: "users",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "Backup",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                guildName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                creationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                vanityUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                preferredLocale = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                verificationLevel = table.Column<int>(type: "integer", nullable: false),
                systemChannelMessageDeny = table.Column<int>(type: "integer", nullable: false),
                defaultMessageNotifications = table.Column<int>(type: "integer", nullable: false),
                explicitContentFilterLevel = table.Column<int>(type: "integer", nullable: false),
                splashUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                iconUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                discoverySplashUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                isWidgetEnabled = table.Column<bool>(type: "boolean", nullable: true),
                isBoostProgressBarEnabled = table.Column<bool>(type: "boolean", nullable: true),
                afkTimeout = table.Column<int>(type: "integer", nullable: true),
                bannerUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                afkChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                defaultChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                publicUpdatesChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                rulesChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                systemChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                widgetChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                Userkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Backup", x => x.key);
                table.ForeignKey(
                    name: "FK_Backup_users_Userkey",
                    column: x => x.Userkey,
                    principalTable: "users",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "CategoryChannel",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                position = table.Column<int>(type: "integer", nullable: false),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoryChannel", x => x.key);
                table.ForeignKey(
                    name: "FK_CategoryChannel_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "Emoji",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                url = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Emoji", x => x.key);
                table.ForeignKey(
                    name: "FK_Emoji_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "GuildUser",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                avatarUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildUser", x => x.key);
                table.ForeignKey(
                    name: "FK_GuildUser_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "Sticker",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sticker", x => x.key);
                table.ForeignKey(
                    name: "FK_Sticker_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "TextChannel",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                slowModeInterval = table.Column<int>(type: "integer", nullable: false),
                topic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                categorykey = table.Column<Guid>(type: "uuid", nullable: true),
                nsfw = table.Column<bool>(type: "boolean", nullable: false),
                archiveAfter = table.Column<int>(type: "integer", nullable: true),
                position = table.Column<int>(type: "integer", nullable: false),
                locked = table.Column<bool>(type: "boolean", nullable: false),
                archived = table.Column<bool>(type: "boolean", nullable: false),
                synced = table.Column<bool>(type: "boolean", nullable: false),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TextChannel", x => x.key);
                table.ForeignKey(
                    name: "FK_TextChannel_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_TextChannel_CategoryChannel_categorykey",
                    column: x => x.categorykey,
                    principalTable: "CategoryChannel",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "VoiceChannel",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                userLimit = table.Column<int>(type: "integer", nullable: true),
                bitrate = table.Column<int>(type: "integer", nullable: false),
                region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                videoQuality = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                position = table.Column<int>(type: "integer", nullable: false),
                categorykey = table.Column<Guid>(type: "uuid", nullable: true),
                synced = table.Column<bool>(type: "boolean", nullable: false),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VoiceChannel", x => x.key);
                table.ForeignKey(
                    name: "FK_VoiceChannel_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_VoiceChannel_CategoryChannel_categorykey",
                    column: x => x.categorykey,
                    principalTable: "CategoryChannel",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "Role",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                color = table.Column<long>(type: "bigint", nullable: false),
                isHoisted = table.Column<bool>(type: "boolean", nullable: false),
                isManaged = table.Column<bool>(type: "boolean", nullable: false),
                isMentionable = table.Column<bool>(type: "boolean", nullable: false),
                position = table.Column<int>(type: "integer", nullable: false),
                isEveryone = table.Column<bool>(type: "boolean", nullable: false),
                permissionskey = table.Column<Guid>(type: "uuid", nullable: true),
                Backupkey = table.Column<Guid>(type: "uuid", nullable: true),
                GuildUserkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Role", x => x.key);
                table.ForeignKey(
                    name: "FK_Role_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_Role_GuildUser_GuildUserkey",
                    column: x => x.GuildUserkey,
                    principalTable: "GuildUser",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_Role_RolePermissions_permissionskey",
                    column: x => x.permissionskey,
                    principalTable: "RolePermissions",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "Message",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                position = table.Column<int>(type: "integer", nullable: false),
                content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                userId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                userPicture = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                TextChannelkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Message", x => x.key);
                table.ForeignKey(
                    name: "FK_Message_TextChannel_TextChannelkey",
                    column: x => x.TextChannelkey,
                    principalTable: "TextChannel",
                    principalColumn: "key");
            });

        migrationBuilder.CreateTable(
            name: "ChannelPermissions",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                targetId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                permissionTarget = table.Column<int>(type: "integer", nullable: false),
                AttachFiles = table.Column<int>(type: "integer", nullable: false),
                Speak = table.Column<int>(type: "integer", nullable: false),
                MuteMembers = table.Column<int>(type: "integer", nullable: false),
                DeafenMembers = table.Column<int>(type: "integer", nullable: false),
                MoveMembers = table.Column<int>(type: "integer", nullable: false),
                UseVAD = table.Column<int>(type: "integer", nullable: false),
                PrioritySpeaker = table.Column<int>(type: "integer", nullable: false),
                Stream = table.Column<int>(type: "integer", nullable: false),
                UseApplicationCommands = table.Column<int>(type: "integer", nullable: false),
                ManageWebhooks = table.Column<int>(type: "integer", nullable: false),
                Connect = table.Column<int>(type: "integer", nullable: false),
                RequestToSpeak = table.Column<int>(type: "integer", nullable: false),
                ManageThreads = table.Column<int>(type: "integer", nullable: false),
                CreatePublicThreads = table.Column<int>(type: "integer", nullable: false),
                CreatePrivateThreads = table.Column<int>(type: "integer", nullable: false),
                UseExternalStickers = table.Column<int>(type: "integer", nullable: false),
                ManageRoles = table.Column<int>(type: "integer", nullable: false),
                UseExternalEmojis = table.Column<int>(type: "integer", nullable: false),
                StartEmbeddedActivities = table.Column<int>(type: "integer", nullable: false),
                ReadMessageHistory = table.Column<int>(type: "integer", nullable: false),
                MentionEveryone = table.Column<int>(type: "integer", nullable: false),
                ManageChannel = table.Column<int>(type: "integer", nullable: false),
                AddReactions = table.Column<int>(type: "integer", nullable: false),
                CreateInstantInvite = table.Column<int>(type: "integer", nullable: false),
                SendMessages = table.Column<int>(type: "integer", nullable: false),
                SendTTSMessages = table.Column<int>(type: "integer", nullable: false),
                ManageMessages = table.Column<int>(type: "integer", nullable: false),
                EmbedLinks = table.Column<int>(type: "integer", nullable: false),
                SendMessagesInThreads = table.Column<int>(type: "integer", nullable: false),
                ViewChannel = table.Column<int>(type: "integer", nullable: false),
                useVoiceActivation = table.Column<int>(type: "integer", nullable: false),
                useSlashCommands = table.Column<int>(type: "integer", nullable: false),
                usePrivateThreads = table.Column<int>(type: "integer", nullable: false),
                CategoryChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                TextChannelkey = table.Column<Guid>(type: "uuid", nullable: true),
                VoiceChannelkey = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChannelPermissions", x => x.key);
                table.ForeignKey(
                    name: "FK_ChannelPermissions_CategoryChannel_CategoryChannelkey",
                    column: x => x.CategoryChannelkey,
                    principalTable: "CategoryChannel",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_ChannelPermissions_TextChannel_TextChannelkey",
                    column: x => x.TextChannelkey,
                    principalTable: "TextChannel",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_ChannelPermissions_VoiceChannel_VoiceChannelkey",
                    column: x => x.VoiceChannelkey,
                    principalTable: "VoiceChannel",
                    principalColumn: "key");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Backup_afkChannelkey",
            table: "Backup",
            column: "afkChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Backup_defaultChannelkey",
            table: "Backup",
            column: "defaultChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Backup_publicUpdatesChannelkey",
            table: "Backup",
            column: "publicUpdatesChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Backup_rulesChannelkey",
            table: "Backup",
            column: "rulesChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Backup_systemChannelkey",
            table: "Backup",
            column: "systemChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Backup_Userkey",
            table: "Backup",
            column: "Userkey");

        migrationBuilder.CreateIndex(
            name: "IX_Backup_widgetChannelkey",
            table: "Backup",
            column: "widgetChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Blacklist_ServerSettingskey",
            table: "Blacklist",
            column: "ServerSettingskey");

        migrationBuilder.CreateIndex(
            name: "IX_Blacklist_Userkey",
            table: "Blacklist",
            column: "Userkey");

        migrationBuilder.CreateIndex(
            name: "IX_CategoryChannel_Backupkey",
            table: "CategoryChannel",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_ChannelPermissions_CategoryChannelkey",
            table: "ChannelPermissions",
            column: "CategoryChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_ChannelPermissions_TextChannelkey",
            table: "ChannelPermissions",
            column: "TextChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_ChannelPermissions_VoiceChannelkey",
            table: "ChannelPermissions",
            column: "VoiceChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_CustomBot_Userkey",
            table: "CustomBot",
            column: "Userkey");

        migrationBuilder.CreateIndex(
            name: "IX_Emoji_Backupkey",
            table: "Emoji",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_GuildUser_Backupkey",
            table: "GuildUser",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_members_botUsedkey",
            table: "members",
            column: "botUsedkey");

        migrationBuilder.CreateIndex(
            name: "IX_members_serverkey",
            table: "members",
            column: "serverkey");

        migrationBuilder.CreateIndex(
            name: "IX_Message_TextChannelkey",
            table: "Message",
            column: "TextChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Role_Backupkey",
            table: "Role",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_Role_GuildUserkey",
            table: "Role",
            column: "GuildUserkey");

        migrationBuilder.CreateIndex(
            name: "IX_Role_permissionskey",
            table: "Role",
            column: "permissionskey");

        migrationBuilder.CreateIndex(
            name: "IX_servers_ownerkey",
            table: "servers",
            column: "ownerkey");

        migrationBuilder.CreateIndex(
            name: "IX_servers_settingskey",
            table: "servers",
            column: "settingskey");

        migrationBuilder.CreateIndex(
            name: "IX_ServerSettings_mainBotkey",
            table: "ServerSettings",
            column: "mainBotkey");

        migrationBuilder.CreateIndex(
            name: "IX_statistics_guildStatskey",
            table: "statistics",
            column: "guildStatskey");

        migrationBuilder.CreateIndex(
            name: "IX_statistics_memberStatskey",
            table: "statistics",
            column: "memberStatskey");

        migrationBuilder.CreateIndex(
            name: "IX_statistics_MigratedBykey",
            table: "statistics",
            column: "MigratedBykey");

        migrationBuilder.CreateIndex(
            name: "IX_statistics_serverkey",
            table: "statistics",
            column: "serverkey");

        migrationBuilder.CreateIndex(
            name: "IX_Sticker_Backupkey",
            table: "Sticker",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_TextChannel_Backupkey",
            table: "TextChannel",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_TextChannel_categorykey",
            table: "TextChannel",
            column: "categorykey");

        migrationBuilder.CreateIndex(
            name: "IX_VoiceChannel_Backupkey",
            table: "VoiceChannel",
            column: "Backupkey");

        migrationBuilder.CreateIndex(
            name: "IX_VoiceChannel_categorykey",
            table: "VoiceChannel",
            column: "categorykey");

        migrationBuilder.AddForeignKey(
            name: "FK_Backup_TextChannel_defaultChannelkey",
            table: "Backup",
            column: "defaultChannelkey",
            principalTable: "TextChannel",
            principalColumn: "key");

        migrationBuilder.AddForeignKey(
            name: "FK_Backup_TextChannel_publicUpdatesChannelkey",
            table: "Backup",
            column: "publicUpdatesChannelkey",
            principalTable: "TextChannel",
            principalColumn: "key");

        migrationBuilder.AddForeignKey(
            name: "FK_Backup_TextChannel_rulesChannelkey",
            table: "Backup",
            column: "rulesChannelkey",
            principalTable: "TextChannel",
            principalColumn: "key");

        migrationBuilder.AddForeignKey(
            name: "FK_Backup_TextChannel_systemChannelkey",
            table: "Backup",
            column: "systemChannelkey",
            principalTable: "TextChannel",
            principalColumn: "key");

        migrationBuilder.AddForeignKey(
            name: "FK_Backup_TextChannel_widgetChannelkey",
            table: "Backup",
            column: "widgetChannelkey",
            principalTable: "TextChannel",
            principalColumn: "key");

        migrationBuilder.AddForeignKey(
            name: "FK_Backup_VoiceChannel_afkChannelkey",
            table: "Backup",
            column: "afkChannelkey",
            principalTable: "VoiceChannel",
            principalColumn: "key");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Backup_TextChannel_defaultChannelkey",
            table: "Backup");

        migrationBuilder.DropForeignKey(
            name: "FK_Backup_TextChannel_publicUpdatesChannelkey",
            table: "Backup");

        migrationBuilder.DropForeignKey(
            name: "FK_Backup_TextChannel_rulesChannelkey",
            table: "Backup");

        migrationBuilder.DropForeignKey(
            name: "FK_Backup_TextChannel_systemChannelkey",
            table: "Backup");

        migrationBuilder.DropForeignKey(
            name: "FK_Backup_TextChannel_widgetChannelkey",
            table: "Backup");

        migrationBuilder.DropForeignKey(
            name: "FK_Backup_users_Userkey",
            table: "Backup");

        migrationBuilder.DropForeignKey(
            name: "FK_Backup_VoiceChannel_afkChannelkey",
            table: "Backup");

        migrationBuilder.DropTable(
            name: "Blacklist");

        migrationBuilder.DropTable(
            name: "ChannelPermissions");

        migrationBuilder.DropTable(
            name: "Emoji");

        migrationBuilder.DropTable(
            name: "errors");

        migrationBuilder.DropTable(
            name: "members");

        migrationBuilder.DropTable(
            name: "Message");

        migrationBuilder.DropTable(
            name: "Role");

        migrationBuilder.DropTable(
            name: "statistics");

        migrationBuilder.DropTable(
            name: "Sticker");

        migrationBuilder.DropTable(
            name: "GuildUser");

        migrationBuilder.DropTable(
            name: "RolePermissions");

        migrationBuilder.DropTable(
            name: "GuildMigration");

        migrationBuilder.DropTable(
            name: "MemberMigration");

        migrationBuilder.DropTable(
            name: "servers");

        migrationBuilder.DropTable(
            name: "ServerSettings");

        migrationBuilder.DropTable(
            name: "CustomBot");

        migrationBuilder.DropTable(
            name: "TextChannel");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "VoiceChannel");

        migrationBuilder.DropTable(
            name: "CategoryChannel");

        migrationBuilder.DropTable(
            name: "Backup");
    }
}
