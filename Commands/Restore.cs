using Discord.Interactions;
using RestoreCord.Utilities.DiscordAttributes;

namespace RestoreCord.Commands;

[RequireMaxPermissions]
public class Restore : InteractionModuleBase<ShardedInteractionContext>
{
    private readonly Migrations.Restore _restore;
    public Restore(Migrations.Restore restore)
    {
        _restore = restore;
    }

    //[SlashCommand("restore", "Restore guild settings, channels. & roles")]
    //public async Task RestoreGuildAsync() => await Migrations.Restore.SendRestoreConfirmationMessage(Context);

    //[ComponentInteraction("confirm-restore-button")]
    //public async Task ConfirmRestoreButtonAsync()
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
    //        //check if user is a premium member
    //        if (await DiscordExtensions.CheckPremiumMembership(database, Context) is false)
    //            return;
    //        //do checks on server entry
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
    //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild has been banned from using the bot.", invisible: true, deleteTimer: 60);
    //            return;
    //        }
    //        if (server.backup is null)
    //        {
    //            await Context.ReplyWithEmbedAsync("Error Occurred", "This guild is not backed up, please backup the guild before performing this command.", invisible: true, deleteTimer: 60);
    //            return;
    //        }

    //        await Context.ReplyWithEmbedAsync("Restore Status", "Attempting to restore this guild from the backup. This may take awhile, please be patient.");
    //        //restore guild
    //        await _restore.RestoreGuildAsync(server, Context, database);
    //        await database.ApplyChangesAsync(server);
    //        //success
    //        try
    //        {
    //            SocketTextChannel? newChannel = Context.Client.GetGuild(Context.Guild.Id).TextChannels.First();
    //            if (newChannel is not null)
    //            {
    //                Task? successTask1 = Context.Guild.GetTextChannel(Context.Channel.Id).DeleteAsync();
    //                Task? successTask2 = newChannel.SendEmbedAsync("Restore Status", "Successfully restored the server.", "RestoreCord made with <3");
    //                await Task.WhenAll(successTask1, successTask2);
    //                return;
    //            }
    //            //delete msg on success instead cuz channel is null
    //            Discord.Rest.RestInteractionMessage? msg = await Context.Interaction.GetOriginalResponseAsync();
    //            if (msg is not null)
    //                await msg.DeleteAsync();
    //            await Context.Interaction.Channel.SendEmbedAsync("Restore Status", "Successfully restored the server.", "RestoreCord made with <3");
    //        }
    //        catch { }
    //    }
    //    catch (Exception e)
    //    {
    //        await Context.ReplyWithEmbedAsync("Error Occurred", "An Error Occurred while trying to restore this guild, please try again. If this error occurs again please report this to an administrator, thank you.", deleteTimer: 60);
    //        await e.LogErrorAsync();
    //    }
    //    finally
    //    {
    //        statistics.active = false;
    //        await database.ApplyChangesAsync(statistics);
    //        GC.Collect();
    //    }
    //}

    //[ComponentInteraction("cancel-restore-button")]
    //public async Task CancelRestoreButtonAsync()
    //{
    //    var socketChannel = Context.Interaction.Channel as SocketTextChannel;
    //    if (socketChannel is not null)
    //        await Migrations.Restore.MerkRestoreEmbed(socketChannel);
    //    await Context.Interaction.Channel.SendEmbedAsync("Restore Status", "Restore has been cancelled. No changes have been made.", $"Issued by: {Context.User.Username} | {Context.User.Id}", Context.User.GetAvatarUrl(), deleteTimer: 60);
    //}

}