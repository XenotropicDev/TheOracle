using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OracleCommands;

public class CommandHandler
{
    private readonly InteractionService _commands;
    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _discord;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandHandler> logger;

    public CommandHandler(InteractionService commands, DiscordSocketClient discord, IConfiguration configuration, IServiceProvider services, ILogger<CommandHandler> logger)
    {
        _commands = commands;
        _discord = discord;
        _configuration = configuration;
        _services = services;
        this.logger = logger;
    }

    public async Task Initialize()
    {
        try
        {
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            _discord.ButtonExecuted += ButtonExecuted;
            _discord.SelectMenuExecuted += SelectExecuted;
            _discord.UserCommandExecuted += UserCommandExecuted;
            _discord.AutocompleteExecuted += AutoCompleteExecuted;
            _discord.SlashCommandExecuted += SlashCommandExecuted;
            _discord.MessageCommandExecuted += MessageCommandExecuted;
            _discord.ModalSubmitted += ModalSubmitted;
            _discord.Ready += Ready;
            _commands.SlashCommandExecuted += _commands_SlashCommandExecuted;
            _commands.AutocompleteHandlerExecuted += _commands_AutocompleteHandlerExecuted;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task RegisterCommands()
    {
        //await _commands.RegisterCommandsToGuildAsync(756890506830807071, true);
        await _commands.RegisterCommandsGloballyAsync(true);
    }

    private Task _commands_AutocompleteHandlerExecuted(IAutocompleteHandler arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private Task _commands_SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private async Task AutoCompleteExecuted(SocketAutocompleteInteraction arg)
    {
        var ctx = new SocketInteractionContext(_discord, arg);
        try
        {
            logger.LogDebug($"{arg.User.Username} is executing Autocomplete Interaction {arg.Data.CommandName} with id:{arg.Data.CommandId}");
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());

            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }

    private async Task ButtonExecuted(SocketMessageComponent arg)
    {
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_discord, arg);
        try
        {
            logger.LogInformation($"{arg.User.Username} is executing Button Interaction {arg.Data.CustomId} with value: '{arg.Data.Value}'.");
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());

            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }

    private async Task MessageCommandExecuted(SocketMessageCommand arg)
    {
        var ctx = new SocketInteractionContext<SocketMessageCommand>(_discord, arg);
        try
        {
            logger.LogInformation($"{arg.User.Username} is executing Message Command {arg.CommandName}");

            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());


            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }

    private async Task ModalSubmitted(SocketModal arg)
    {
        var ctx = new SocketInteractionContext<SocketModal>(_discord, arg);
        try
        {
            logger.LogInformation($"{arg.User.Username} is submitting a Modal for {arg.Data.CustomId} with {arg.Data.Components.Count} Components.");
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }

    private async Task Ready()
    {
        try
        {
            await RegisterCommands();
            _discord.Ready -= Ready;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }

    private async Task SelectExecuted(SocketMessageComponent arg)
    {
        var ctx = new SocketInteractionContext<SocketMessageComponent>(_discord, arg);
        try
        {
            logger.LogInformation($"{arg.User.Username} is executing Select Interaction {arg.Data.CustomId} with value(s): '{string.Join(", ", arg.Data.Values ?? Array.Empty<string>())}'.");
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());

            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }

    private async Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        var ctx = new SocketInteractionContext<SocketSlashCommand>(_discord, arg);
        try
        {
            logger.LogInformation($"{arg.User.Username} is executing Slash Command {arg.Data.Name} with value(s): '{string.Join(", ", arg.Data.Options.Select(o => o.Value))}'.");
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());

            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }

    private async Task UserCommandExecuted(SocketUserCommand arg)
    {
        var ctx = new SocketInteractionContext<SocketUserCommand>(_discord, arg);
        try
        {
            logger.LogInformation($"{arg.User.Username} is executing User Command {arg.Data.Name}");
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());

            if (!ctx.Interaction.HasResponded)
            {
                await ctx.Interaction.RespondAsync($"An error occoured: {ex.Message}", ephemeral: true);
            }
        }
    }
}
