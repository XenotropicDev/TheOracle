using Discord.Interactions;
using Server.Data.homebrew;
using Server.DiscordServer;
using TheOracle2.Commands;
using TheOracle2.Data;
using TheOracle2.Data.AssetWorkbench;

namespace TheOracle2;

[Group("remove-content", "Removes custom game content from a content set.")]
public class RemoveContentCommands : InteractionModuleBase
{
    public RemoveContentCommands(ApplicationContext db)
    {
        Db = db;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("oracle", "Removes an oracle from the content set")]
    public async Task RemoveOracle([Autocomplete(typeof(OwnerSubAutoComplete))]int setName, 
        [Autocomplete(typeof(OracleAutocomplete)), Summary("Oracle Name")] string oracleId)
    {
        try
        {
            var set = Db.GameContentSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && gs.Id == setName);
            Func<Oracle, bool> oracleResolver = int.TryParse(oracleId, out var parsedId) ? o => o.Id == parsedId : o => o.JsonId == oracleId;
            var oracle = set.Oracles.FirstOrDefault(oracleResolver);
            set.Oracles.Remove(oracle);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync($"Oracle removed", ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await RespondAsync($"Something happened and your oracle wasn't removed.", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }

    [SlashCommand("asset", "Removes an asset from the content set")]
    public async Task RemoveAsset([Autocomplete(typeof(OwnerSubAutoComplete))] int setName, 
        [Autocomplete(typeof(AssetAutoComplete))] string assetName)
    {
        try
        {
            var set = Db.GameContentSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && gs.Id == setName);

            Func<Asset, bool> assetResolver = int.TryParse(assetName, out var assetId) ? a => a.Id == assetId : a => a.JsonId == assetName;

            var asset =  set.Assets.FirstOrDefault(assetResolver);
            set.Assets.Remove(asset);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await RespondAsync($"Asset removed", ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await RespondAsync($"Something happened and your oracle wasn't removed.", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }
}
