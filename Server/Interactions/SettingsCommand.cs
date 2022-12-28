using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Server.DiscordServer;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("settings", "Manage how you or your server interact with the bot")]
public class SettingsCommand : InteractionModuleBase
{
    private readonly RollCommand rollCommand;

    public SettingsCommand(ApplicationContext context, InteractionService interactionService, RollCommand rollCommand)
    {
        DbContext = context;
        InteractionService = interactionService;
        this.rollCommand = rollCommand;
    }

    public ApplicationContext DbContext { get; }
    public InteractionService InteractionService { get; }

    [SlashCommand("enable-roll-command", "Adds a basic roll command to the slash command options")]
    public async Task AddRollCommand([Summary(description: "Details about what this is tracking or anything else you want to appear on the generated embed.")]string trackerDescription = "")
    {
        await InteractionService.AddCommandsToGuildAsync(Context.Guild, false, InteractionService.SlashCommands.FirstOrDefault(c => c.Name == "roll-dice"));

        await RespondAsync("Registered the slash command `/roll-dice` to this server", ephemeral:true).ConfigureAwait(false);
    }
}
