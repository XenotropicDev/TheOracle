using Discord.Interactions;
using Discord.WebSocket;
using Server.Data;
using Server.DiscordServer;
using Server.GameInterfaces;
using Server.Interactions.Helpers;
using Server.OracleRoller;

namespace TheOracle2;

public class CreateCommand : InteractionModuleBase
{
    private readonly IOracleRepository oracles;
    private readonly IOracleRoller roller;
    private readonly IEmoteRepository emotes;

    public CreateCommand(IEntityRepository entities, IOracleRepository oracles, IOracleRoller roller, IEmoteRepository emotes)
    {
        Entities = entities;
        this.oracles = oracles;
        this.roller = roller;
        this.emotes = emotes;
    }

    public ApplicationContext appContext { get; }
    public IEntityRepository Entities { get; }

    [SlashCommand("create", "Creates an interactive game object (NPC, Settlement, Planet)")]
    public async Task CreateEntity([Autocomplete(typeof(EntityAutoComplete))] string objectType)
    {
        var entity = Entities.GetEntity(objectType);
        if (entity == null)
        {
            await RespondAsync($"Unknown entity: {objectType}", ephemeral: true).ConfigureAwait(false);
            return;
        }

        var modal = new ModalBuilder()
            .WithCustomId($"createHelper{entity.InitialOracles.Count(o => o.HasModalOverride)}:{objectType}")
            .WithTitle($"Specify any {entity.Title} known values");

        foreach (var modalEntry in entity.InitialOracles.Where(o => o.HasModalOverride))
        {
            modal.AddTextInput(modalEntry.FieldName, GenericInputModal.GetNextCustomId(modal), modalEntry.ModalSettings?.TextInputStyle ?? TextInputStyle.Short, modalEntry.ModalSettings?.PlaceholderText, modalEntry.ModalSettings?.MinLength, modalEntry.ModalSettings?.MaxLength);
        }

        if (modal.Components.ActionRows?.Count > 0)
        {
            await RespondWithModalAsync(modal.Build()).ConfigureAwait(false);
            return;
        }

        var discordEntity = new OracleEntityAdapter(entity, oracles, roller, emotes);

        await discordEntity.EntityAsResponse(RespondAsync).ConfigureAwait(false);
    }

    private async Task ProcessEntityModalResponse(string entityId, params string[] values)
    {
        var entity = Entities.GetEntity(entityId);
        if (entity == null) throw new ArgumentException($"Unknown entity {entityId}");

        var modalOverrides = entity.InitialOracles.Where(o => o.HasModalOverride);

        foreach (var modalOverride in modalOverrides)
        {
            
        }

        await RespondAsync("Pretend that this is working for real").ConfigureAwait(false);
    }

    [ModalInteraction("Entity-Input-Modal1:*")]
    public async Task EntityFromModal(string EntityId, GenericInputModal<string> modal)
    {
        await ProcessEntityModalResponse(EntityId, modal.First).ConfigureAwait(false);
    }

    [ModalInteraction("Entity-Input-Modal2:*")]
    public async Task EntityFromModal(string EntityId, GenericInputModal<string, string> modal)
    {
        await ProcessEntityModalResponse(EntityId, modal.First, modal.Second).ConfigureAwait(false);
    }

    [ModalInteraction("Entity-Input-Modal3:*")]
    public async Task EntityFromModal(string EntityId, GenericInputModal<string, string, string> modal)
    {
        await ProcessEntityModalResponse(EntityId, modal.First, modal.Second, modal.Third).ConfigureAwait(false);
    }

    [ModalInteraction("Entity-Input-Modal4:*")]
    public async Task EntityFromModal(string EntityId, GenericInputModal<string, string, string, string> modal)
    {
        await ProcessEntityModalResponse(EntityId, modal.First, modal.Second, modal.Third, modal.Fourth).ConfigureAwait(false);
    }

    [ModalInteraction("Entity-Input-Modal5:*")]
    public async Task EntityFromModal(string EntityId, GenericInputModal<string, string, string, string, string> modal)
    {
        await ProcessEntityModalResponse(EntityId, modal.First, modal.Second, modal.Third, modal.Fourth, modal.Fifth).ConfigureAwait(false);
    }
}

public class CreateComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public CreateComponents(ApplicationContext dbContext)
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
