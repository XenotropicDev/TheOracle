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
    public async Task RemoveOracle([Autocomplete(typeof(OwnerSubAutoComplete))]int setName, [Autocomplete(typeof(OracleAutocomplete))] string oracleName)
    {
        try
        {
            var set = Db.GameContentSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && gs.Id == setName);
            var oracle = set.Oracles.FirstOrDefault(o => o.Id == oracleName);
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
    public async Task RemoveAsset([Autocomplete(typeof(OwnerSubAutoComplete))] int setName, [Autocomplete(typeof(AssetAutoComplete))] string assetName)
    {
        try
        {
            var set = Db.GameContentSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && gs.Id == setName);
            var asset = set.Assets.FirstOrDefault(o => o.Id == assetName);
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
