using System.Reflection;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using RestoreCord.Database;
using RestoreCord.Database.Models.LogModels;
using RestoreCord.Utilities;

namespace RestoreCord.Events;

internal class InteractionEventHandler
{
    private readonly DiscordShardedClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _provider;
    public InteractionEventHandler(DiscordShardedClient client, InteractionService interactionService, IServiceProvider provider)
    {
        _client = client;
        _commands = interactionService;
        _provider = provider;
        _client.ShardReady += GuildReady;
        _client.InteractionCreated += HandleInteraction;
        _commands.SlashCommandExecuted += SlashCommandExecuted;
        _commands.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        catch (Exception ex)
        {
            await ex.LogErrorAsync();
        }
    }

    public async Task GuildReady(DiscordSocketClient arg)
    {
        try
        {
            await _commands.RegisterCommandsGloballyAsync();
            await arg.SetGameAsync("restorecord.com", null, Discord.ActivityType.Watching);
            await arg.SetStatusAsync(Discord.UserStatus.DoNotDisturb);
        }
        catch(Exception ex)
        {
            await ex.LogErrorAsync();
        }
    }

    public async Task HandleInteraction(SocketInteraction arg) 
    {
        try
        {
            await _commands.ExecuteCommandAsync(new ShardedInteractionContext(_client, arg), _provider);
        }
        catch(Exception ex)
        {
            await ex.LogErrorAsync();
        }
    }

    public async Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
    {
        switch (arg3.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                break;
            case InteractionCommandError.UnknownCommand:
                // implement
                break;
            case InteractionCommandError.BadArgs:
                await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                break;
            case InteractionCommandError.Exception:
                await using (var database = new DatabaseContext())
                {
                    var entry = new Errors
                    {
                        errorTime = DateTime.Now,
                        location = arg1.Name,
                        message = arg3.ErrorReason,
                        extraInfo = "interaction handler",
                        stackTrace = null
                    };
                    await database.AddAsync(entry);
                    await database.ApplyChangesAsync();
                };
                await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                break;
            case InteractionCommandError.Unsuccessful:
                await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                break;
            default:
                break;
        }
    }
    public async Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Exception:
                    await using (var database = new DatabaseContext())
                    {
                        var entry = new Errors
                        {
                            errorTime = DateTime.Now,
                            location = arg1.Name,
                            message = arg3.ErrorReason,
                            extraInfo = "component handler",
                            stackTrace = null
                        };
                        await database.AddAsync(entry);
                        await database.ApplyChangesAsync();
                    };
                    await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                    break;
                case InteractionCommandError.Unsuccessful:
                    await arg2.ReplyWithEmbedAsync("Error Occurred", arg3.ErrorReason, deleteTimer: 60);
                    break;
                default:
                    break;
            }
        }
    }
}
