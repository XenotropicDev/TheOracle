using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Server.Data;
using Server.DiscordServer;
using Server.Interactions.Helpers;
using TheOracle2.UserContent;
using Server.GameInterfaces.DTOs; // Added for MoveDTO
using System; // For IServiceProvider, ArgumentException, etc.
using Microsoft.Extensions.DependencyInjection; // For GetRequiredService

namespace TheOracle2;

public class ReferenceCommand : InteractionModuleBase
{
    public ReferenceCommand(IServiceProvider services, ILogger<ReferenceCommand> logger)
    {
        Moves = services.GetRequiredService<IMoveRepository>();
        Emotes = services.GetRequiredService<IEmoteRepository>();
        this.logger = logger;
    }

    private readonly ILogger<ReferenceCommand> logger;

    public IMoveRepository Moves { get; private set; }
    public IEmoteRepository Emotes { get; }

    [SlashCommand("reference", "Posts the rules text for the given topic.")]
    public async Task GetReferenceMessage([Autocomplete(typeof(ReferenceAutoComplete))] string move,
        [Summary(description: "Set this to False to show it to all people in the chat")] bool ephemeral = true,
        [Summary(description: "Keeps this reference message in chat and doesn't automatically delete it after a few minutes")] bool keepMessage = false)
    {
        var moveInfo = Moves.GetMove(move); // Returns MoveDTO?
        if (moveInfo == null) return;

        var moveItems = new DiscordMoveEntity(moveInfo, Emotes, ephemeral); // Expects MoveDTO

        await moveItems.EntityAsResponse(RespondAsync);

        if (!keepMessage && !ephemeral)
        {
            await Task.Delay(TimeSpan.FromMinutes(10)).ConfigureAwait(false);

            try
            {
                var msg = await GetOriginalResponseAsync().ConfigureAwait(false);
                await msg.DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to cleanup move message. It was probably already deleted.");
            }
        }
    }
}

public class MoveReferenceInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly IEmoteRepository emotes;
    private readonly IMoveRepository moves;
    private readonly ILogger<ReferenceCommand> logger;

    public MoveReferenceInteractions(ApplicationContext db, Random random, IEmoteRepository emotes, IMoveRepository moves, ILogger<ReferenceCommand> logger)
    {
        Db = db;
        this.emotes = emotes;
        this.moves = moves;
        this.logger = logger;
    }

    public ApplicationContext Db { get; }

    [ComponentInteraction("move-references")]
    public async Task ReferenceMessageSelection(string[] selectedMoves)
    {
        var taskList = new List<Task>();
        foreach (var move in selectedMoves)
        {
            var moveInfo = moves.GetMove(move); // Returns MoveDTO?
            if (moveInfo == null) return;

            var moveItems = new DiscordMoveEntity(moveInfo, emotes); // Expects MoveDTO

            if (!Context.Interaction.HasResponded) await Context.Interaction.UpdateAsync(msg => { }).ConfigureAwait(false); 

            var msg = await moveItems.EntityAsReply(ReplyAsync).ConfigureAwait(false);

            taskList.Add(DeleteAfterDelay(msg, TimeSpan.FromMinutes(10)));
        }

        await Task.WhenAll(taskList);
    }

    private async Task DeleteAfterDelay(IUserMessage msg, TimeSpan delay)
    {
        await Task.Delay(delay).ConfigureAwait(false);

        try
        {
            await msg.DeleteAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unable to cleanup reference move message. It was probably already deleted.");
        }
    }
}
