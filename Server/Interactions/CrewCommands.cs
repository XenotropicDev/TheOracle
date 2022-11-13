using Discord.Interactions;
using Discord.WebSocket;
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

            partyName = $"{descriptor} {theme}";
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

    [SlashCommand("add-character", "Adds a character to the party")]
    public async Task AddPlayer([Autocomplete(typeof(PartyAutocomplete))] int partyId, [Autocomplete(typeof(CharacterAutocomplete))] string character)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);
        var party = await DbContext.Parties.FindAsync(partyId);

        if (pc == null || party == null) throw new ArgumentException($"Unknown character: {id}, or party:{partyId}");

        party.Characters.Add(pc);

        await DbContext.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync($"Character Added", ephemeral: true).ConfigureAwait(false);

        await party.UpdateCardDisplay((Context.Client as DiscordSocketClient)!, emotes, dataFactory);
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

        await RespondAsync($"Character Added", ephemeral: true).ConfigureAwait(false);

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
}
