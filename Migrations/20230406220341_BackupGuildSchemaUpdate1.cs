using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackupGuildSchemaUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FK_Backup_VoiceChannel_afkChannelkey",
                table: "Backup");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_TextChannel_TextChannelkey",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Backup_afkChannelkey",
                table: "Backup");

            migrationBuilder.DropIndex(
                name: "IX_Backup_defaultChannelkey",
                table: "Backup");

            migrationBuilder.DropIndex(
                name: "IX_Backup_publicUpdatesChannelkey",
                table: "Backup");

            migrationBuilder.DropIndex(
                name: "IX_Backup_rulesChannelkey",
                table: "Backup");

            migrationBuilder.DropIndex(
                name: "IX_Backup_systemChannelkey",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "afkChannelkey",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "afkTimeout",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "bannerUrl",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "defaultChannelkey",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "defaultMessageNotifications",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "discoverySplashUrl",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "explicitContentFilterLevel",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "guildName",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "iconUrl",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "isBoostProgressBarEnabled",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "isWidgetEnabled",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "preferredLocale",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "publicUpdatesChannelkey",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "rulesChannelkey",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "splashUrl",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "systemChannelMessageDeny",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "systemChannelkey",
                table: "Backup");

            migrationBuilder.DropColumn(
                name: "vanityUrl",
                table: "Backup");

            migrationBuilder.RenameColumn(
                name: "TextChannelkey",
                table: "Message",
                newName: "Backupkey");

            migrationBuilder.RenameIndex(
                name: "IX_Message_TextChannelkey",
                table: "Message",
                newName: "IX_Message_Backupkey");

            migrationBuilder.RenameColumn(
                name: "widgetChannelkey",
                table: "Backup",
                newName: "guildkey");

            migrationBuilder.RenameColumn(
                name: "verificationLevel",
                table: "Backup",
                newName: "type");

            migrationBuilder.RenameIndex(
                name: "IX_Backup_widgetChannelkey",
                table: "Backup",
                newName: "IX_Backup_guildkey");

            migrationBuilder.CreateTable(
                name: "Guild",
                columns: table => new
                {
                    key = table.Column<Guid>(type: "uuid", nullable: false),
                    guildName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    widgetChannelkey = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild", x => x.key);
                    table.ForeignKey(
                        name: "FK_Guild_TextChannel_defaultChannelkey",
                        column: x => x.defaultChannelkey,
                        principalTable: "TextChannel",
                        principalColumn: "key");
                    table.ForeignKey(
                        name: "FK_Guild_TextChannel_publicUpdatesChannelkey",
                        column: x => x.publicUpdatesChannelkey,
                        principalTable: "TextChannel",
                        principalColumn: "key");
                    table.ForeignKey(
                        name: "FK_Guild_TextChannel_rulesChannelkey",
                        column: x => x.rulesChannelkey,
                        principalTable: "TextChannel",
                        principalColumn: "key");
                    table.ForeignKey(
                        name: "FK_Guild_TextChannel_systemChannelkey",
                        column: x => x.systemChannelkey,
                        principalTable: "TextChannel",
                        principalColumn: "key");
                    table.ForeignKey(
                        name: "FK_Guild_TextChannel_widgetChannelkey",
                        column: x => x.widgetChannelkey,
                        principalTable: "TextChannel",
                        principalColumn: "key");
                    table.ForeignKey(
                        name: "FK_Guild_VoiceChannel_afkChannelkey",
                        column: x => x.afkChannelkey,
                        principalTable: "VoiceChannel",
                        principalColumn: "key");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guild_afkChannelkey",
                table: "Guild",
                column: "afkChannelkey");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_defaultChannelkey",
                table: "Guild",
                column: "defaultChannelkey");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_publicUpdatesChannelkey",
                table: "Guild",
                column: "publicUpdatesChannelkey");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_rulesChannelkey",
                table: "Guild",
                column: "rulesChannelkey");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_systemChannelkey",
                table: "Guild",
                column: "systemChannelkey");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_widgetChannelkey",
                table: "Guild",
                column: "widgetChannelkey");

            migrationBuilder.AddForeignKey(
                name: "FK_Backup_Guild_guildkey",
                table: "Backup",
                column: "guildkey",
                principalTable: "Guild",
                principalColumn: "key");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Backup_Backupkey",
                table: "Message",
                column: "Backupkey",
                principalTable: "Backup",
                principalColumn: "key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Backup_Guild_guildkey",
                table: "Backup");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Backup_Backupkey",
                table: "Message");

            migrationBuilder.DropTable(
                name: "Guild");

            migrationBuilder.RenameColumn(
                name: "Backupkey",
                table: "Message",
                newName: "TextChannelkey");

            migrationBuilder.RenameIndex(
                name: "IX_Message_Backupkey",
                table: "Message",
                newName: "IX_Message_TextChannelkey");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Backup",
                newName: "verificationLevel");

            migrationBuilder.RenameColumn(
                name: "guildkey",
                table: "Backup",
                newName: "widgetChannelkey");

            migrationBuilder.RenameIndex(
                name: "IX_Backup_guildkey",
                table: "Backup",
                newName: "IX_Backup_widgetChannelkey");

            migrationBuilder.AddColumn<Guid>(
                name: "afkChannelkey",
                table: "Backup",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "afkTimeout",
                table: "Backup",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bannerUrl",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "defaultChannelkey",
                table: "Backup",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "defaultMessageNotifications",
                table: "Backup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Backup",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "discoverySplashUrl",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "explicitContentFilterLevel",
                table: "Backup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "guildName",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "iconUrl",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isBoostProgressBarEnabled",
                table: "Backup",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isWidgetEnabled",
                table: "Backup",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "preferredLocale",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "publicUpdatesChannelkey",
                table: "Backup",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "rulesChannelkey",
                table: "Backup",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "splashUrl",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "systemChannelMessageDeny",
                table: "Backup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "systemChannelkey",
                table: "Backup",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vanityUrl",
                table: "Backup",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Message_TextChannel_TextChannelkey",
                table: "Message",
                column: "TextChannelkey",
                principalTable: "TextChannel",
                principalColumn: "key");
        }
    }
}
