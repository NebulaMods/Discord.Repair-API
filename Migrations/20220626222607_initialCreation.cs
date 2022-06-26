using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoreCord.Migrations;

public partial class initialCreation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "errors",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                location = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                message = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                stackTrace = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                extraInfo = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                errorTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_errors", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "GuildMigration",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                startTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                totalTime = table.Column<TimeSpan>(type: "time(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildMigration", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "MemberMigration",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                startTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                totalTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                successCount = table.Column<int>(type: "int", nullable: false),
                bannedCount = table.Column<int>(type: "int", nullable: false),
                tooManyGuildsCount = table.Column<int>(type: "int", nullable: false),
                invalidTokenCount = table.Column<int>(type: "int", nullable: false),
                alreadyHereCount = table.Column<int>(type: "int", nullable: false),
                failedCount = table.Column<int>(type: "int", nullable: false),
                totalCount = table.Column<int>(type: "int", nullable: false),
                blacklistedCount = table.Column<int>(type: "int", nullable: false),
                estimatedCompletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberMigration", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "RolePermissions",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                Speak = table.Column<bool>(type: "tinyint(1)", nullable: false),
                MuteMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                DeafenMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                MoveMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseVAD = table.Column<bool>(type: "tinyint(1)", nullable: false),
                PrioritySpeaker = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Stream = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ChangeNickname = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageNicknames = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageEmojisAndStickers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageWebhooks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Connect = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseApplicationCommands = table.Column<bool>(type: "tinyint(1)", nullable: false),
                RequestToSpeak = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageEvents = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreatePublicThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreatePrivateThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseExternalStickers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageRoles = table.Column<bool>(type: "tinyint(1)", nullable: false),
                UseExternalEmojis = table.Column<bool>(type: "tinyint(1)", nullable: false),
                AttachFiles = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ReadMessageHistory = table.Column<bool>(type: "tinyint(1)", nullable: false),
                CreateInstantInvite = table.Column<bool>(type: "tinyint(1)", nullable: false),
                BanMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                KickMembers = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Administrator = table.Column<bool>(type: "tinyint(1)", nullable: false),
                MentionEveryone = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageGuild = table.Column<bool>(type: "tinyint(1)", nullable: false),
                AddReactions = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageChannels = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ViewGuildInsights = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ViewChannel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                SendMessages = table.Column<bool>(type: "tinyint(1)", nullable: false),
                SendTTSMessages = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ManageMessages = table.Column<bool>(type: "tinyint(1)", nullable: false),
                EmbedLinks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                SendMessagesInThreads = table.Column<bool>(type: "tinyint(1)", nullable: false),
                ViewAuditLog = table.Column<bool>(type: "tinyint(1)", nullable: false),
                StartEmbeddedActivities = table.Column<bool>(type: "tinyint(1)", nullable: false),
                useVoiceActivation = table.Column<bool>(type: "tinyint(1)", nullable: false),
                moderateMembers = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RolePermissions", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                username = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                email = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                password = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                role = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                pfp = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                banned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                googleAuthCode = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                darkmode = table.Column<bool>(type: "tinyint(1)", nullable: false),
                expiry = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                createdAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                lastIP = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                discordId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                apiToken = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_users", x => x.key);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "CustomBot",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                name = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                token = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                clientSecret = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                clientId = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                urlRedirect = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                botType = table.Column<int>(type: "int", nullable: false),
                Userkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CustomBot", x => x.key);
                table.ForeignKey(
                    name: "FK_CustomBot_users_Userkey",
                    column: x => x.Userkey,
                    principalTable: "users",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "members",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                discordId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                guildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                accessToken = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                refreshToken = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ip = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                avatar = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                username = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                creationDate = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                botUsedkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_members", x => x.key);
                table.ForeignKey(
                    name: "FK_members_CustomBot_botUsedkey",
                    column: x => x.botUsedkey,
                    principalTable: "CustomBot",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ServerSettings",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                autoKickUnVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                autoKickUnVerifiedTime = table.Column<int>(type: "int", nullable: false),
                autoJoin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                verifyDescription = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                redirectTime = table.Column<int>(type: "int", nullable: false),
                vanityUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                webhookLogType = table.Column<int>(type: "int", nullable: false),
                dmOnAutoKick = table.Column<bool>(type: "tinyint(1)", nullable: false),
                autoBlacklist = table.Column<bool>(type: "tinyint(1)", nullable: false),
                redirectUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                pic = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                backgroundImage = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                vpnCheck = table.Column<bool>(type: "tinyint(1)", nullable: false),
                webhook = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                mainBotkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServerSettings", x => x.key);
                table.ForeignKey(
                    name: "FK_ServerSettings_CustomBot_mainBotkey",
                    column: x => x.mainBotkey,
                    principalTable: "CustomBot",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Blacklist",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                discordId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                ip = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                reason = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                ServerSettingskey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Blacklist", x => x.key);
                table.ForeignKey(
                    name: "FK_Blacklist_ServerSettings_ServerSettingskey",
                    column: x => x.ServerSettingskey,
                    principalTable: "ServerSettings",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "servers",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ownerkey = table.Column<int>(type: "int", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                guildId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                roleId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                banned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                settingskey = table.Column<int>(type: "int", nullable: false)
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
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "statistics",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                serverkey = table.Column<int>(type: "int", nullable: false),
                guildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                MigratedBykey = table.Column<int>(type: "int", nullable: false),
                active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                startDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                endDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                memberStatskey = table.Column<int>(type: "int", nullable: true),
                guildStatskey = table.Column<int>(type: "int", nullable: true)
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
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_statistics_users_MigratedBykey",
                    column: x => x.MigratedBykey,
                    principalTable: "users",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Backup",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                guildName = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                creationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                vanityUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                preferredLocale = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                verificationLevel = table.Column<int>(type: "int", nullable: false),
                systemChannelMessageDeny = table.Column<int>(type: "int", nullable: false),
                defaultMessageNotifications = table.Column<int>(type: "int", nullable: false),
                explicitContentFilterLevel = table.Column<int>(type: "int", nullable: false),
                splashUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                iconUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                description = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                discoverySplashUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                isWidgetEnabled = table.Column<bool>(type: "tinyint(1)", nullable: true),
                isBoostProgressBarEnabled = table.Column<bool>(type: "tinyint(1)", nullable: true),
                afkTimeout = table.Column<int>(type: "int", nullable: true),
                bannerUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                afkChannelkey = table.Column<int>(type: "int", nullable: true),
                defaultChannelkey = table.Column<int>(type: "int", nullable: true),
                publicUpdatesChannelkey = table.Column<int>(type: "int", nullable: true),
                rulesChannelkey = table.Column<int>(type: "int", nullable: true),
                systemChannelkey = table.Column<int>(type: "int", nullable: true),
                widgetChannelkey = table.Column<int>(type: "int", nullable: true),
                Userkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Backup", x => x.key);
                table.ForeignKey(
                    name: "FK_Backup_users_Userkey",
                    column: x => x.Userkey,
                    principalTable: "users",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "CategoryChannel",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                position = table.Column<int>(type: "int", nullable: false),
                Backupkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoryChannel", x => x.key);
                table.ForeignKey(
                    name: "FK_CategoryChannel_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Emoji",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                name = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                url = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Backupkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Emoji", x => x.key);
                table.ForeignKey(
                    name: "FK_Emoji_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "GuildUser",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                username = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                avatarUrl = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Backupkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildUser", x => x.key);
                table.ForeignKey(
                    name: "FK_GuildUser_Backup_Backupkey",
                    column: x => x.Backupkey,
                    principalTable: "Backup",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Role",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                icon = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                color = table.Column<uint>(type: "int unsigned", nullable: false),
                isHoisted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                isManaged = table.Column<bool>(type: "tinyint(1)", nullable: false),
                isMentionable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                position = table.Column<int>(type: "int", nullable: false),
                isEveryone = table.Column<bool>(type: "tinyint(1)", nullable: false),
                permissionskey = table.Column<int>(type: "int", nullable: false),
                Backupkey = table.Column<int>(type: "int", nullable: true)
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
                    name: "FK_Role_RolePermissions_permissionskey",
                    column: x => x.permissionskey,
                    principalTable: "RolePermissions",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "TextChannel",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                slowModeInterval = table.Column<int>(type: "int", nullable: false),
                topic = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                categorykey = table.Column<int>(type: "int", nullable: true),
                nsfw = table.Column<bool>(type: "tinyint(1)", nullable: false),
                archiveAfter = table.Column<int>(type: "int", nullable: true),
                position = table.Column<int>(type: "int", nullable: false),
                locked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                archived = table.Column<bool>(type: "tinyint(1)", nullable: false),
                synced = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Backupkey = table.Column<int>(type: "int", nullable: true)
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
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "VoiceChannel",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                name = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                userLimit = table.Column<int>(type: "int", nullable: true),
                bitrate = table.Column<int>(type: "int", nullable: false),
                region = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                videoQuality = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                position = table.Column<int>(type: "int", nullable: false),
                categorykey = table.Column<int>(type: "int", nullable: true),
                synced = table.Column<bool>(type: "tinyint(1)", nullable: false),
                Backupkey = table.Column<int>(type: "int", nullable: true)
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
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "GuildUserRole",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                rolekey = table.Column<int>(type: "int", nullable: false),
                GuildUserkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GuildUserRole", x => x.key);
                table.ForeignKey(
                    name: "FK_GuildUserRole_GuildUser_GuildUserkey",
                    column: x => x.GuildUserkey,
                    principalTable: "GuildUser",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_GuildUserRole_Role_rolekey",
                    column: x => x.rolekey,
                    principalTable: "Role",
                    principalColumn: "key",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Message",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                position = table.Column<int>(type: "int", nullable: false),
                content = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                username = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                userId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                userPicture = table.Column<string>(type: "longtext", nullable: true)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                TextChannelkey = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Message", x => x.key);
                table.ForeignKey(
                    name: "FK_Message_TextChannel_TextChannelkey",
                    column: x => x.TextChannelkey,
                    principalTable: "TextChannel",
                    principalColumn: "key");
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "ChannelPermissions",
            columns: table => new
            {
                key = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                targetId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                permissionTarget = table.Column<int>(type: "int", nullable: false),
                AttachFiles = table.Column<int>(type: "int", nullable: false),
                Speak = table.Column<int>(type: "int", nullable: false),
                MuteMembers = table.Column<int>(type: "int", nullable: false),
                DeafenMembers = table.Column<int>(type: "int", nullable: false),
                MoveMembers = table.Column<int>(type: "int", nullable: false),
                UseVAD = table.Column<int>(type: "int", nullable: false),
                PrioritySpeaker = table.Column<int>(type: "int", nullable: false),
                Stream = table.Column<int>(type: "int", nullable: false),
                UseApplicationCommands = table.Column<int>(type: "int", nullable: false),
                ManageWebhooks = table.Column<int>(type: "int", nullable: false),
                Connect = table.Column<int>(type: "int", nullable: false),
                RequestToSpeak = table.Column<int>(type: "int", nullable: false),
                ManageThreads = table.Column<int>(type: "int", nullable: false),
                CreatePublicThreads = table.Column<int>(type: "int", nullable: false),
                CreatePrivateThreads = table.Column<int>(type: "int", nullable: false),
                UseExternalStickers = table.Column<int>(type: "int", nullable: false),
                ManageRoles = table.Column<int>(type: "int", nullable: false),
                UseExternalEmojis = table.Column<int>(type: "int", nullable: false),
                StartEmbeddedActivities = table.Column<int>(type: "int", nullable: false),
                ReadMessageHistory = table.Column<int>(type: "int", nullable: false),
                MentionEveryone = table.Column<int>(type: "int", nullable: false),
                ManageChannel = table.Column<int>(type: "int", nullable: false),
                AddReactions = table.Column<int>(type: "int", nullable: false),
                CreateInstantInvite = table.Column<int>(type: "int", nullable: false),
                SendMessages = table.Column<int>(type: "int", nullable: false),
                SendTTSMessages = table.Column<int>(type: "int", nullable: false),
                ManageMessages = table.Column<int>(type: "int", nullable: false),
                EmbedLinks = table.Column<int>(type: "int", nullable: false),
                SendMessagesInThreads = table.Column<int>(type: "int", nullable: false),
                ViewChannel = table.Column<int>(type: "int", nullable: false),
                useVoiceActivation = table.Column<int>(type: "int", nullable: false),
                useSlashCommands = table.Column<int>(type: "int", nullable: false),
                usePrivateThreads = table.Column<int>(type: "int", nullable: false),
                CategoryChannelkey = table.Column<int>(type: "int", nullable: true),
                TextChannelkey = table.Column<int>(type: "int", nullable: true),
                VoiceChannelkey = table.Column<int>(type: "int", nullable: true)
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
            })
            .Annotation("MySql:CharSet", "utf8mb4");

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
            name: "IX_GuildUserRole_GuildUserkey",
            table: "GuildUserRole",
            column: "GuildUserkey");

        migrationBuilder.CreateIndex(
            name: "IX_GuildUserRole_rolekey",
            table: "GuildUserRole",
            column: "rolekey");

        migrationBuilder.CreateIndex(
            name: "IX_members_botUsedkey",
            table: "members",
            column: "botUsedkey");

        migrationBuilder.CreateIndex(
            name: "IX_Message_TextChannelkey",
            table: "Message",
            column: "TextChannelkey");

        migrationBuilder.CreateIndex(
            name: "IX_Role_Backupkey",
            table: "Role",
            column: "Backupkey");

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
            name: "GuildUserRole");

        migrationBuilder.DropTable(
            name: "members");

        migrationBuilder.DropTable(
            name: "Message");

        migrationBuilder.DropTable(
            name: "statistics");

        migrationBuilder.DropTable(
            name: "GuildUser");

        migrationBuilder.DropTable(
            name: "Role");

        migrationBuilder.DropTable(
            name: "GuildMigration");

        migrationBuilder.DropTable(
            name: "MemberMigration");

        migrationBuilder.DropTable(
            name: "servers");

        migrationBuilder.DropTable(
            name: "RolePermissions");

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
