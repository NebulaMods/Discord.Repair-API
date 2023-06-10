﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordRepair.Api.Migrations;

/// <inheritdoc />
public partial class MigrationSchemaUpdate3 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "successfulMemberAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "invalidTokenAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "failedMemberAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "alreadyMigratedMemberAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(
            name: "successfulMemberAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)");

        migrationBuilder.AlterColumn<decimal>(
            name: "invalidTokenAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)");

        migrationBuilder.AlterColumn<decimal>(
            name: "failedMemberAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)");

        migrationBuilder.AlterColumn<decimal>(
            name: "alreadyMigratedMemberAmount",
            table: "migrations",
            type: "numeric(20,0)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric(20,0)");
    }
}
