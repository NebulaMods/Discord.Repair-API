using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackupSchemaUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guild_TextChannel_defaultChannelkey",
                table: "Guild");

            migrationBuilder.DropForeignKey(
                name: "FK_Guild_TextChannel_publicUpdatesChannelkey",
                table: "Guild");

            migrationBuilder.DropForeignKey(
                name: "FK_Guild_TextChannel_rulesChannelkey",
                table: "Guild");

            migrationBuilder.DropForeignKey(
                name: "FK_Guild_TextChannel_systemChannelkey",
                table: "Guild");

            migrationBuilder.DropForeignKey(
                name: "FK_Guild_TextChannel_widgetChannelkey",
                table: "Guild");

            migrationBuilder.DropForeignKey(
                name: "FK_Guild_VoiceChannel_afkChannelkey",
                table: "Guild");

            migrationBuilder.DropIndex(
                name: "IX_Guild_afkChannelkey",
                table: "Guild");

            migrationBuilder.DropIndex(
                name: "IX_Guild_defaultChannelkey",
                table: "Guild");

            migrationBuilder.DropIndex(
                name: "IX_Guild_publicUpdatesChannelkey",
                table: "Guild");

            migrationBuilder.DropIndex(
                name: "IX_Guild_rulesChannelkey",
                table: "Guild");

            migrationBuilder.DropIndex(
                name: "IX_Guild_systemChannelkey",
                table: "Guild");

            migrationBuilder.DropIndex(
                name: "IX_Guild_widgetChannelkey",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "afkChannelkey",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "defaultChannelkey",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "publicUpdatesChannelkey",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "rulesChannelkey",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "systemChannelkey",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "widgetChannelkey",
                table: "Guild");

            migrationBuilder.AddColumn<decimal>(
                name: "afkChannelId",
                table: "Guild",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "defaultChannelId",
                table: "Guild",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "publicUpdatesChannelId",
                table: "Guild",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rulesChannelId",
                table: "Guild",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "systemChannelId",
                table: "Guild",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "widgetChannelId",
                table: "Guild",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "afkChannelId",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "defaultChannelId",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "publicUpdatesChannelId",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "rulesChannelId",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "systemChannelId",
                table: "Guild");

            migrationBuilder.DropColumn(
                name: "widgetChannelId",
                table: "Guild");

            migrationBuilder.AddColumn<Guid>(
                name: "afkChannelkey",
                table: "Guild",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "defaultChannelkey",
                table: "Guild",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "publicUpdatesChannelkey",
                table: "Guild",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "rulesChannelkey",
                table: "Guild",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "systemChannelkey",
                table: "Guild",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "widgetChannelkey",
                table: "Guild",
                type: "uuid",
                nullable: true);

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
                name: "FK_Guild_TextChannel_defaultChannelkey",
                table: "Guild",
                column: "defaultChannelkey",
                principalTable: "TextChannel",
                principalColumn: "key");

            migrationBuilder.AddForeignKey(
                name: "FK_Guild_TextChannel_publicUpdatesChannelkey",
                table: "Guild",
                column: "publicUpdatesChannelkey",
                principalTable: "TextChannel",
                principalColumn: "key");

            migrationBuilder.AddForeignKey(
                name: "FK_Guild_TextChannel_rulesChannelkey",
                table: "Guild",
                column: "rulesChannelkey",
                principalTable: "TextChannel",
                principalColumn: "key");

            migrationBuilder.AddForeignKey(
                name: "FK_Guild_TextChannel_systemChannelkey",
                table: "Guild",
                column: "systemChannelkey",
                principalTable: "TextChannel",
                principalColumn: "key");

            migrationBuilder.AddForeignKey(
                name: "FK_Guild_TextChannel_widgetChannelkey",
                table: "Guild",
                column: "widgetChannelkey",
                principalTable: "TextChannel",
                principalColumn: "key");

            migrationBuilder.AddForeignKey(
                name: "FK_Guild_VoiceChannel_afkChannelkey",
                table: "Guild",
                column: "afkChannelkey",
                principalTable: "VoiceChannel",
                principalColumn: "key");
        }
    }
}
