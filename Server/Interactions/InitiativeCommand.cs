using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Server.DiscordServer;
using TheOracle2.UserContent;

namespace TheOracle2;

public class InitiativeCommand : InteractionModuleBase
{
    public InitiativeCommand(ApplicationContext context)
    {
        DbContext = context;
    }

    public ApplicationContext DbContext { get; }

    [SlashCommand("initiative", "Creates a post to keep track of combat initiative")]
    public async Task PostTracker([Summary(description: "Details about what this is tracking or anything else you want to appear on the generated embed.")]string trackerDescription = "")
    {
        var initiativeTracker = new EmbedBuilder()
            .WithTitle("Initiative")
            .WithDescription(trackerDescription)
            .AddField("In control", "none", true)
            .AddField("In a bad spot", "none", true);

        var pcs = DbContext.PlayerCharacters.Where(pc => pc.DiscordGuildId == Context.Guild.Id);

        ComponentBuilder component = new ComponentBuilder()
            .WithButton("In control", "initiative-control:true")
            .WithButton("In a bad spot", "initiative-control:false");

        await RespondAsync(embed: initiativeTracker.Build(), components: component.Build()).ConfigureAwait(false);
    }
}

public class InitiativeComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public InitiativeComponents(ApplicationContext dbContext)
    {
        DbContext = dbContext;
    }

    public ApplicationContext DbContext { get; }

    [ComponentInteraction("initiative-control:*")]
    public async Task ConditionSelection(string hasControl)
    {
        if (!Boolean.TryParse(hasControl, out var isInControl)) throw new ArgumentException($"unknown {nameof(hasControl)} argument {hasControl}");

        var embed = Context.Interaction.Message.Embeds.FirstOrDefault()?.ToEmbedBuilder();
        if (embed == null) throw new ArgumentException("Message embed was null, was it deleted?");

        var inControlUsers = embed.Fields.FirstOrDefault(f => f.Name == "In control")?.Value.ToString().Replace("none", "").Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        var badSpotUsers = embed.Fields.FirstOrDefault(f => f.Name == "In a bad spot")?.Value.ToString().Replace("none", "").Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        var triggeringName = (Context.User as IGuildUser)?.Nickname ?? Context.User.Username;

        badSpotUsers.Remove(triggeringName);
        inControlUsers.Remove(triggeringName);

        if (isInControl)
        {
            inControlUsers.Add(triggeringName);
        }
        else
        {
            badSpotUsers.Add(triggeringName);
        }
        if (inControlUsers.Count == 0) inControlUsers.Add("none");
        if (badSpotUsers.Count == 0) badSpotUsers.Add("none");

        embed.Fields.FirstOrDefault(f => f.Name == "In control").Value = String.Join("\n", inControlUsers);
        embed.Fields.FirstOrDefault(f => f.Name == "In a bad spot").Value = String.Join("\n", badSpotUsers);

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = new Embed[] { embed.Build() };
        }).ConfigureAwait(false);
    }
}
