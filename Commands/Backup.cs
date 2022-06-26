using Discord.Interactions;
using RestoreCord.Utilities.DiscordAttributes;

namespace RestoreCord.Commands;

[RequireAdministrator]
public class Backup : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly Migrations.Restore _restore;
    private readonly Migrations.Backup _backup;
    public Backup(Migrations.Restore restore, Migrations.Backup backup)
    {
        _restore = restore;
        _backup = backup;
    }

    //[SlashCommand("backup", "Backup guild roles, channels, & settings")]
    //public async Task BackupGuildAsync()
    //{
    //    await using var database = new DatabaseContext();
    //    var statistics = new Database.Models.LogModels.Statistics()
    //    {
    //        memberStats = new Database.Models.Statistics.MemberMigration
    //        {
    //            startTime = DateTime.Now,

    //        },
    //        active = true
    //    };
    //    await database.statistics.AddAsync(statistics);
    //    try
    //    {
    //        await database.ApplyChangesAsync();
    //        Server? server = await database.servers.FirstOrDefaultAsync(x => x.guildId == Context.Guild.Id);
    //        if (server is null)
    //        {
    //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild does not exist in our database, please try again.", invisible: true, deleteTimer: 60);
    //            return;
    //        }
    //        if (server.banned)
    //        {
    //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
    //            return;
    //        }
    //        if (await DiscordExtensions.IsGuildBusy(Context.Guild.Id))
    //        {
    //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild is busy.", invisible: true, deleteTimer: 60);
    //            return;
    //        }
    //        if (await DiscordExtensions.CheckPremiumMembership(database, Context) is false)
    //            return;
    //        if (server.backup is not null)
    //        {
    //            await _restore.DeleteGuildBackupAsync(server, database);
    //            await database.ApplyChangesAsync(server);
    //        }
    //        await Context.ReplyWithEmbedAsync("Backup Status", "Attempting to backup this guild to the database. This may take awhile, please be patient.");
    //        await _backup.BackupGuildAsync(server, database, Context.Guild, Context);
    //        Discord.Rest.RestInteractionMessage? msg = await Context.Interaction.GetOriginalResponseAsync();
    //        if (msg is not null)
    //            await msg.DeleteAsync();
    //        await Context.Channel.SendEmbedAsync("Backup Status", "Successfully backed up the guild's channels, roles & settings.", "RestoreCord made with <3");
    //    }
    //    catch (Exception e)
    //    {
    //        await Context.ReplyWithEmbedAsync("Error Occurred", "An Error Occurred while trying to backup this guild, please try again. If this error occurs again please report this to an administrator, thank you.", deleteTimer: 60);
    //        await e.LogErrorAsync();
    //    }
    //    finally
    //    {
    //        statistics.active = false;
    //        await database.ApplyChangesAsync();
    //    }
    //}
}
