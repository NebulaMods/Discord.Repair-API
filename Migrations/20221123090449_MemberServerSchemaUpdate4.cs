using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations;

public partial class MemberServerSchemaUpdate4 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_members_servers_serverkey",
            table: "members");

        migrationBuilder.AddColumn<Guid>(
            name: "verifyEmbedSettingskey",
            table: "ServerSettings",
            type: "uuid",
            nullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "serverkey",
            table: "members",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.CreateTable(
            name: "SuccessVerifyEmbedSettings",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                authorName = table.Column<string>(type: "text", nullable: false),
                iconUrl = table.Column<string>(type: "text", nullable: false),
                footerIconUrl = table.Column<string>(type: "text", nullable: false),
                footerText = table.Column<string>(type: "text", nullable: false),
                title = table.Column<string>(type: "text", nullable: false),
                geoData = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SuccessVerifyEmbedSettings", x => x.key);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ServerSettings_verifyEmbedSettingskey",
            table: "ServerSettings",
            column: "verifyEmbedSettingskey");

        migrationBuilder.AddForeignKey(
            name: "FK_members_servers_serverkey",
            table: "members",
            column: "serverkey",
            principalTable: "servers",
            principalColumn: "key");

        migrationBuilder.AddForeignKey(
            name: "FK_ServerSettings_SuccessVerifyEmbedSettings_verifyEmbedSettin~",
            table: "ServerSettings",
            column: "verifyEmbedSettingskey",
            principalTable: "SuccessVerifyEmbedSettings",
            principalColumn: "key");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_members_servers_serverkey",
            table: "members");

        migrationBuilder.DropForeignKey(
            name: "FK_ServerSettings_SuccessVerifyEmbedSettings_verifyEmbedSettin~",
            table: "ServerSettings");

        migrationBuilder.DropTable(
            name: "SuccessVerifyEmbedSettings");

        migrationBuilder.DropIndex(
            name: "IX_ServerSettings_verifyEmbedSettingskey",
            table: "ServerSettings");

        migrationBuilder.DropColumn(
            name: "verifyEmbedSettingskey",
            table: "ServerSettings");

        migrationBuilder.AlterColumn<Guid>(
            name: "serverkey",
            table: "members",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_members_servers_serverkey",
            table: "members",
            column: "serverkey",
            principalTable: "servers",
            principalColumn: "key",
            onDelete: ReferentialAction.Cascade);
    }
}
