using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations;

public partial class UserServerMemberSchemaUpdate1 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "creationDate",
            table: "members");

        migrationBuilder.AddColumn<bool>(
            name: "captcha",
            table: "ServerSettings",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "linkDate",
            table: "members",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "captcha",
            table: "ServerSettings");

        migrationBuilder.DropColumn(
            name: "linkDate",
            table: "members");

        migrationBuilder.AddColumn<decimal>(
            name: "creationDate",
            table: "members",
            type: "numeric(20,0)",
            nullable: true);
    }
}
