using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations;

/// <inheritdoc />
public partial class MigrationSchemaUpdate2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "userkey",
            table: "migrations",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_migrations_userkey",
            table: "migrations",
            column: "userkey");

        migrationBuilder.AddForeignKey(
            name: "FK_migrations_users_userkey",
            table: "migrations",
            column: "userkey",
            principalTable: "users",
            principalColumn: "key");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_migrations_users_userkey",
            table: "migrations");

        migrationBuilder.DropIndex(
            name: "IX_migrations_userkey",
            table: "migrations");

        migrationBuilder.DropColumn(
            name: "userkey",
            table: "migrations");
    }
}
