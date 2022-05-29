using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoreCord.Migrations;

public partial class autoBlacklist : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "autoBlacklist",
            table: "servers",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "autoBlacklist",
            table: "servers");
    }
}
