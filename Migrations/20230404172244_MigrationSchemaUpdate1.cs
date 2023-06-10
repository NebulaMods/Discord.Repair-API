using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations;

/// <inheritdoc />
public partial class MigrationSchemaUpdate1 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "statistics");

        migrationBuilder.DropTable(
            name: "GuildMigration");

        migrationBuilder.DropTable(
            name: "MemberMigration");

        migrationBuilder.CreateTable(
            name: "migrations",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                serverkey = table.Column<Guid>(type: "uuid", nullable: true),
                botkey = table.Column<Guid>(type: "uuid", nullable: true),
                startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                completionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                totalMemberAmount = table.Column<long>(type: "bigint", nullable: false),
                newGuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                newRoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                failedMemberAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                successfulMemberAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                invalidTokenAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                alreadyMigratedMemberAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                extraDetails = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_migrations", x => x.key);
                table.ForeignKey(
                    name: "FK_migrations_CustomBot_botkey",
                    column: x => x.botkey,
                    principalTable: "CustomBot",
                    principalColumn: "key");
                table.ForeignKey(
                    name: "FK_migrations_servers_serverkey",
                    column: x => x.serverkey,
                    principalTable: "servers",
                    principalColumn: "key");
            });

        migrationBuilder.CreateIndex(
            name: "IX_migrations_botkey",
            table: "migrations",
            column: "botkey");

        migrationBuilder.CreateIndex(
            name: "IX_migrations_serverkey",
            table: "migrations",
            column: "serverkey");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "migrations");

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
                alreadyHereCount = table.Column<int>(type: "integer", nullable: false),
                bannedCount = table.Column<int>(type: "integer", nullable: false),
                blacklistedCount = table.Column<int>(type: "integer", nullable: false),
                estimatedCompletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                failedCount = table.Column<int>(type: "integer", nullable: false),
                invalidTokenCount = table.Column<int>(type: "integer", nullable: false),
                startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                successCount = table.Column<int>(type: "integer", nullable: false),
                tooManyGuildsCount = table.Column<int>(type: "integer", nullable: false),
                totalCount = table.Column<int>(type: "integer", nullable: false),
                totalTime = table.Column<TimeSpan>(type: "interval", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberMigration", x => x.key);
            });

        migrationBuilder.CreateTable(
            name: "statistics",
            columns: table => new
            {
                key = table.Column<Guid>(type: "uuid", nullable: false),
                guildStatskey = table.Column<Guid>(type: "uuid", nullable: true),
                memberStatskey = table.Column<Guid>(type: "uuid", nullable: true),
                MigratedBykey = table.Column<Guid>(type: "uuid", nullable: true),
                serverkey = table.Column<Guid>(type: "uuid", nullable: true),
                active = table.Column<bool>(type: "boolean", nullable: false),
                endDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                guildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                startDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
    }
}
