using DiscordRepair.Api.Database;
using DiscordRepair.Api.Database.Models;
using DiscordRepair.Api.Records.Responses;
using DiscordRepair.Api.Utilities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DiscordRepair.Api.Endpoints.V1.Restore;

/// <summary>
/// REST API controller for retrieving migration history.
/// </summary>
[ApiController]
[Route("/v1/")]
[ApiExplorerSettings(GroupName = "Restore Endpoints")]
public class Get : ControllerBase
{
    /// <summary>
    /// Retrieves migration history for the user and returns an array of migration objects.
    /// </summary>
    /// <returns>An array of Migration objects representing the user's migration history.</returns>
    [HttpGet("restore")]
    [Consumes("text/plain")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Migration[]), 200)]
    [ProducesResponseType(typeof(Generic), 404)]
    [ProducesResponseType(typeof(Generic), 400)]
    public async Task<ActionResult<Migration[]>> HandleAsync()
    {
        // Create a new instance of the database context to interact with the database.
        using var database = new DatabaseContext();

        // Get the user associated with the current HTTP context.
        var user = await database.users.FirstOrDefaultAsync(x => x.username == HttpContext.WhoAmI());

        // Retrieve all migrations for the user and convert the result to an array.
        var migrationHistory = await database.migrations.Where(x => x.user == user).ToArrayAsync();

        // Check if the user has any migration history. If not, return a 400 (Bad Request) response with an error message.
        if (migrationHistory.Any() is false)
        {
            return BadRequest(new Generic()
            {
                success = false,
                details = "no migration history"
            });
        }

        // Map each MigrationHistory object to a new Migration object with only the required properties.
        var results = migrationHistory.Select(migration => new Migration
        {
            totalMemberAmount = migration.totalMemberAmount,
            alreadyMigratedMemberAmount = migration.alreadyMigratedMemberAmount,
            failedMemberAmount = migration.failedMemberAmount,
            invalidTokenAmount = migration.invalidTokenAmount,
            successfulMemberAmount = migration.successfulMemberAmount,
            botName = migration.bot?.name,
            completionTime = migration.completionTime,
            extraDetails = migration.extraDetails,
            newGuildId = migration.newGuildId,
            newRoleId = migration.newRoleId,
            startTime = migration.startTime,
            status = migration.status,
            serverName = migration.server?.name
        }).ToArray();

        // Return the array of Migration objects in a 200 (OK) response.
        return results;
    }

    /// <summary>
    /// Represents a migration object containing information about a server migration.
    /// </summary>
    public record Migration
    {
        /// <summary>
        /// Gets or sets the name of the server associated with the migration.
        /// </summary>
        public string? serverName { get; set; }

        /// <summary>
        /// Gets or sets the name of the bot associated with the migration.
        /// </summary>
        public string? botName { get; set; }

        /// <summary>
        /// Gets or sets the start time of the migration.
        /// </summary>
        public DateTime startTime { get; set; }

        /// <summary>
        /// Gets or sets the completion time of the migration.
        /// </summary>
        public DateTime completionTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the migration.
        /// </summary>
        public MigrationStatus status { get; set; }

        /// <summary>
        /// Gets or sets the total number of members to be migrated.
        /// </summary>
        public long totalMemberAmount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the ID of the new guild created for the migration.
        /// </summary>
        public ulong? newGuildId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the new role created for the migration.
        /// </summary>
        public ulong? newRoleId { get; set; }

        /// <summary>
        /// Gets or sets the number of members that failed to migrate.
        /// </summary>
        public ulong? failedMemberAmount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of members that were successfully migrated.
        /// </summary>
        public ulong? successfulMemberAmount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of invalid tokens encountered during the migration.
        /// </summary>
        public ulong? invalidTokenAmount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the number of members that were already migrated prior to the migration.
        /// </summary>
        public ulong? alreadyMigratedMemberAmount { get; set; } = 0;

        /// <summary>
        /// Gets or sets any additional details about the migration.
        /// </summary>
        public string? extraDetails { get; set; }
    }

}
//public async Task<ActionResult> HandleAsync()
//{
//    List<Migration> results = new();
//    await using var database = new DatabaseContext();
//    var user = await database.users.FirstOrDefaultAsync(x => x.username == HttpContext.WhoAmI());
//    var migrationHistory = await database.migrations.Where(x => x.user == user).ToArrayAsync();
//    if (migrationHistory.Any() is false)
//    {
//        return BadRequest(new Generic()
//        {
//            success = false,
//            details = "no migration history"
//        });
//    }
//    for (int i = 0; i < migrationHistory.Length; i++)
//    {
//        var migration = migrationHistory[i];
//        results.Add(new Migration()
//        {
//            totalMemberAmount = migration.totalMemberAmount,
//            alreadyMigratedMemberAmount = migration.alreadyMigratedMemberAmount,
//            failedMemberAmount = migration.failedMemberAmount,
//            invalidTokenAmount = migration.invalidTokenAmount,
//            successfulMemberAmount = migration.successfulMemberAmount,
//            botName = migration.bot?.name,
//            completionTime = migration.completionTime,
//            extraDetails = migration.extraDetails,
//            newGuildId = migration.newGuildId,
//            newRoleId = migration.newRoleId,
//            startTime = migration.startTime,
//            status = migration.status,
//            serverName = migration.server?.name
//        });
//    }
//    return Ok(results.ToArray());
//}
