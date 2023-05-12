using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Server.Data.homebrew;
using Server.DiscordServer;
using TheOracle2.Commands;
using TheOracle2.Data;
using TheOracle2.Data.AssetWorkbench;

namespace TheOracle2;

[Group("upload", "Add custom game content to the bot.")]
public class UploadCommands : InteractionModuleBase
{
    private readonly HttpClient http;

    public UploadCommands(ApplicationContext db, HttpClient http)
    {
        Db = db;
        this.http = http;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("oracle", "Adds or updates a custom oracle to the bot. Must be in Dataforged format")]
    public async Task UploadOracle(IAttachment OracleJson,
                                   [Autocomplete(typeof(OwnerSubAutoComplete)), Summary("Set Name")] int? setId = null,
                                   [Autocomplete(typeof(OracleAutocomplete))] string? OracleToUpdate = null)
    {
        try
        {
            var fileTask = GetAttachmentAsync(OracleJson).ConfigureAwait(false);

            await DeferAsync().ConfigureAwait(false);

            var file = await fileTask;
            var oracle = JsonConvert.DeserializeObject<Oracle>(file);

            var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);

            var sets = existing.GameDataSets.Where(gs => gs.CreatorId == Context.Interaction.User.Id && (setId == null || gs.Id == setId));
            
            if (sets.Count() > 1)
            {
                await FollowupAsync($"Error: There was more than one set found, please specify the set you'd like to use.", ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (!sets.Any())
            {
                var contentSet = new GameContentSet()
                {
                    CreatorId = Context.Interaction.User.Id,
                    IsPublic = false,
                    SetName = "Default",
                    Assets = new List<Asset>(),
                    Moves = new List<Move>(),
                    Oracles = new List<Oracle>(),
                };

                existing.GameDataSets.Add(contentSet);
            }


            var set = existing.GameDataSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && (setId == null || gs.Id == setId));

            if (int.TryParse(OracleToUpdate, out int updateId)) 
            {
                var existingOracle = set.Oracles.FirstOrDefault(o => o.Id == updateId);
                if (existingOracle == null)
                {
                    await FollowupAsync($"Error: Couldn't find the specified oracle in the set", ephemeral: true).ConfigureAwait(false);
                    return;
                }
                //Db.Entry(oracle).CurrentValues.SetValues(existingOracle); //not sure if this is needed or not.
                oracle.Id = existingOracle.Id;
                existingOracle = oracle;
            }
            else
            {
                set.Oracles.Add(oracle);
            }
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await FollowupAsync($"Oracle {(OracleToUpdate != null ? "updated" : "added")}", ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await FollowupAsync($"Something happened and your oracle wasn't uploaded.", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }

    [SlashCommand("asset", "Adds or updates custom asset to the bot. Must be in Dataforged or Asset Workbench format")]
    public async Task UploadAsset(IAttachment AssetJson, 
        [Autocomplete(typeof(PublicSubAutoComplete)), Summary("Set Name")]int? setId = null,
        [Autocomplete(typeof(AssetAutoComplete))]string? assetToUpdate = null)
    {
        try
        {
            var fileTask = GetAttachmentAsync(AssetJson).ConfigureAwait(false);

            await DeferAsync().ConfigureAwait(false);

            var file = await fileTask;
            var isDataforged = JObject.Parse(file)?["documentFormatVersion"] == null; //check for the Asset-Workbench documentFormatVersion
            var asset = (isDataforged) ? JsonConvert.DeserializeObject<Asset>(file) : new AssetWorkbenchAdapter(file);

            var player = await Db.Players.Include(p => p.GameDataSets).FirstOrDefaultAsync(p => p.DiscordId == Context.Interaction.User.Id).ConfigureAwait(false);

            var sets = player.GameDataSets.Where(gs => gs.CreatorId == Context.Interaction.User.Id && (setId == null || gs.Id == setId));
            
            if (sets.Count() > 1)
            {
                await FollowupAsync($"Error: There was more than one set found, please specify the set you'd like to use.", ephemeral: true).ConfigureAwait(false);
                return;
            }

            if (!sets.Any())
            {
                var contentSet = new GameContentSet()
                {
                    CreatorId = Context.Interaction.User.Id,
                    IsPublic = false,
                    SetName = "Default",
                    Assets = new List<Asset>(),
                    Moves = new List<Move>(),
                    Oracles = new List<Oracle>(),
                };

                Db.GameContentSets.Attach(contentSet);
                player.GameDataSets.Add(contentSet);
            }

            var set = player.GameDataSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && (setId == null || gs.Id == setId));
            
            if (!string.IsNullOrWhiteSpace(assetToUpdate) && int.TryParse(assetToUpdate, out var assetId))
            {
                var existingAsset = set.Assets.FirstOrDefault(a => a.Id == assetId);
                if (existingAsset == null)
                {
                    await FollowupAsync($"Error: Couldn't find the specified asset in the set", ephemeral: true).ConfigureAwait(false);
                    return;
                }
                asset.Id = existingAsset.Id;
                existingAsset = asset;
            }
            else
            {
                set.Assets.Add(asset);
            }

            await Db.SaveChangesAsync().ConfigureAwait(false);

            //reload the player data so that the new game content sets get loaded in
            Db.Dispose();

            await FollowupAsync($"Asset {(string.IsNullOrWhiteSpace(assetToUpdate) ? "added" : "updated")}", ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await FollowupAsync($"Something happened and your asset wasn't uploaded.", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }

    private async Task<string> GetAttachmentAsync(IAttachment AssetJson)
    {
        var message = await http.GetAsync(AssetJson.Url).ConfigureAwait(false);
        return await message.Content.ReadAsStringAsync();
    }
}
