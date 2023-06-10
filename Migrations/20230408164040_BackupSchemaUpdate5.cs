using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations
{
    /// <inheritdoc />
    public partial class BackupSchemaUpdate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userPicture",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "username",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Message",
                newName: "authorId");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "Message",
                newName: "type");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Sticker",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "format",
                table: "Sticker",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isAvailable",
                table: "Sticker",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Sticker",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "packId",
                table: "Sticker",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "sortOrder",
                table: "Sticker",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "tags",
                table: "Sticker",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "Sticker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "channelkey",
                table: "Message",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "createdAt",
                table: "Message",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "flags",
                table: "Message",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPinned",
                table: "Message",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSuppressed",
                table: "Message",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTTS",
                table: "Message",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "source",
                table: "Message",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "threadChannelId",
                table: "Message",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_channelkey",
                table: "Message",
                column: "channelkey");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_TextChannel_channelkey",
                table: "Message",
                column: "channelkey",
                principalTable: "TextChannel",
                principalColumn: "key",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_TextChannel_channelkey",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_channelkey",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "format",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "isAvailable",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "packId",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "sortOrder",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "tags",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "url",
                table: "Sticker");

            migrationBuilder.DropColumn(
                name: "channelkey",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "flags",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "isPinned",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "isSuppressed",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "isTTS",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "source",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "threadChannelId",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Message",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "authorId",
                table: "Message",
                newName: "userId");

            migrationBuilder.AddColumn<string>(
                name: "userPicture",
                table: "Message",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "Message",
                type: "text",
                nullable: true);
        }
    }
}
