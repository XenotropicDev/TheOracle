using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;
using Server.DiscordServer;
using Server.GameInterfaces;
using Server.Interactions.Helpers;
using Server.OracleRoller;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("party", "Create and manage the party/crew.")]
public class PartyCommandsGroup : InteractionModuleBase
{
    private readonly IEmoteRepository emotes;
    private readonly PlayerDataFactory dataFactory;
    private readonly IOracleRoller oracleRoller;

    public ILogger<PartyCommandsGroup> logger { get; set; }

    public PartyCommandsGroup(ApplicationContext dbContext, IEmoteRepository emotes, PlayerDataFactory dataFactory, IOracleRoller oracleRoller)
    {
        DbContext = dbContext;
        this.emotes = emotes;
        this.dataFactory = dataFactory;
        this.oracleRoller = oracleRoller;
    }

    public ApplicationContext DbContext { get; }

    [SlashCommand("create", "Creates a party/crew for characters to join.")]
    public async Task BuildParty([Summary(description: "A name to identifty the party. A random name will be used if you don't provide one")] string? partyName = null,
        [Summary(description: "A direct link to an image used in the created embed. (Or from the edit embed right click menu)")] string? crewImage = null)
    {
        await DeferAsync(); //We have to use defer so that GetOriginalResponse works.
        IUserMessage? message = await Context.Interaction.GetOriginalResponseAsync();
        if (message == null) await FollowupAsync("Something went wrong, please try again, and report this error if it keeps happening", ephemeral: true);

        //get a descriptor/theme combo if a name isn't provided
        if (partyName == null)
        {
            var oracles = await dataFactory.GetPlayerOracles(Context.User.Id);

            var descriptor = oracleRoller.GetRollResult(oracles.FirstOrDefault(o => o.Id.Contains("/Descriptor")));
            var theme = oracleRoller.GetRollResult(oracles.FirstOrDefault(o => o.Id.Contains("/Theme")));

            partyName = $"{descriptor.Description} {theme.Description}";
        }

        var party = new Party
        {
            MessageId = message.Id,
            ChannelId = message.Channel.Id,
            DiscordGuildId = Context.Interaction.GuildId,
            ImageUrl = crewImage,
            Name = partyName,
        };

        DbContext.Parties.Add(party);
        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        var pcEntity = new PartyEntity(party, emotes, dataFactory);
        var characterSheet = await pcEntity.EntityAsResponse(FollowupAsync).ConfigureAwait(false);
        party.MessageId = characterSheet.Id;

        await DbContext.SaveChangesAsync().ConfigureAwait(false);
        return;
    }

    [SlashCommand("add-character", "Adds a character to the party. Note: A character can only be in one party.")]
    public async Task AddPlayer([Autocomplete(typeof(PartyAutocomplete))] int partyId, [Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
        var party = await DbContext.Parties.FindAsync(partyId);

        if (pc == null || party == null) throw new ArgumentException($"Unknown character: {id}, or party:{partyId}");

        await DbContext.Parties.Where(p => p.Characters.Contains(pc)).ForEachAsync(p => p.Characters.Remove(pc)).ConfigureAwait(false);

        party.Characters.Add(pc);

        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync($"Character Added", ephemeral: true).ConfigureAwait(false);

        pc.Supply = party.Supply;
        await pc.UpdateCardDisplay(Context.Client, emotes, dataFactory).ConfigureAwait(false);
        await party.UpdateCardDisplay(Context.Client, emotes, dataFactory);
        return;
    }

    [SlashCommand("remove-character", "Removes a character, from the party/crew.")]
    public async Task RemoveCharacterFromParty([Autocomplete(typeof(PartyAutocomplete))] int party, [Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
        var partyObj = await DbContext.Parties.FindAsync(party);

        if (pc == null || partyObj == null) throw new ArgumentException($"Unknown character: {id}, or party:{party}");

        partyObj.Characters.Remove(pc);

        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync($"Character removed", ephemeral: true).ConfigureAwait(false);

        await partyObj.UpdateCardDisplay((Context.Client as DiscordSocketClient)!, emotes, dataFactory);
        return;
    }

    [SlashCommand("rename-party", "changes the name of the party")]
    public async Task RenameParty([Autocomplete(typeof(PartyAutocomplete))] string party, string name)
    {
        var partyObj = await DbContext.Parties.FindAsync(party);

        if (partyObj == null) throw new ArgumentException($"Unknown party: {party}");

        partyObj.Name = name;

        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync($"Party name changed to {name}", ephemeral: true).ConfigureAwait(false);

        await partyObj.UpdateCardDisplay((Context.Client as DiscordSocketClient)!, emotes, dataFactory);
        return;
    }

    [SlashCommand("remove-party", "Removes the party from autocomplete results. This cannot be undone.")]
    public async Task RemoveParty([Autocomplete(typeof(PartyAutocomplete))] string party)
    {
        if (!int.TryParse(party, out var parsedId)) throw new ArgumentException("Unknown party");
        var partyObj = await DbContext.Parties.FindAsync(parsedId);

        if (partyObj == null) throw new ArgumentException($"Unknown party: {party}");

        DbContext.Parties.Remove(partyObj);

        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync($"Party '{partyObj.Name}' has been removed", ephemeral: true).ConfigureAwait(false);

        try
        {
            var channel = await Context.Client.GetChannelAsync(partyObj.ChannelId ?? 0).ConfigureAwait(false) as IMessageChannel;
            var msg = await channel.GetMessageAsync(partyObj.MessageId ?? 0).ConfigureAwait(false);
            await msg.DeleteAsync().ConfigureAwait(false);    
        }
        catch (Exception ex)
        {
            logger.LogWarning($"The party message could not be deleted. It was probably deleted already. Exception: {ex.Message}");
        }
        
        return;
    }
}

public class PartyComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private ApplicationContext Db;
    private IEmoteRepository emotes;
    private PlayerDataFactory playerDataFactory;

    public PartyComponents(ApplicationContext dbContext, IEmoteRepository emotes, PlayerDataFactory dataFactory)
    {
        Db = dbContext;
        this.emotes = emotes;
        this.playerDataFactory = dataFactory;
    }

    [ComponentInteraction("add-party-supply-*")]
    public async Task AddSupply(string partyInput)
    {
        if (!int.TryParse(partyInput, out int partyId)) throw new ArgumentException($"Unknown Party ID: {partyInput}");
        var party = await Db.Parties.FindAsync(partyId);
        party.Supply++;
        await Db.SaveChangesAsync();

        var partyEntity = new PartyEntity(party, emotes, playerDataFactory);

        await Context.Interaction.UpdateAsync(async msg =>
        {
            msg.Embeds = partyEntity.AsEmbedArray();
            msg.Components = (await partyEntity.GetComponentsAsync()).Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("lose-party-supply-*")]
    public async Task RemoveSupply(string partyInput)
    {
        if (!int.TryParse(partyInput, out int partyId)) throw new ArgumentException($"Unknown Party ID: {partyInput}");
        var party = await Db.Parties.FindAsync(partyId);
        party.Supply--;
        await Db.SaveChangesAsync();

        var partyEntity = new PartyEntity(party, emotes, playerDataFactory);

        await Context.Interaction.UpdateAsync(async msg =>
        {
            msg.Embeds = partyEntity.AsEmbedArray();
            msg.Components = (await partyEntity.GetComponentsAsync()).Build();
        }).ConfigureAwait(false);
    }
}
