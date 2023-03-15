using Discord.Interactions;
using Server.Data.homebrew;
using Server.DiscordServer;
using TheOracle2.Data;
using TheOracle2.Data.AssetWorkbench;

namespace TheOracle2;

public enum AssetImportFormat
{
    Dataforged,
    AssetWorkbench
}

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

    [SlashCommand("oracle", "Adds a custom oracle to the bot. Must be in Dataforged format")]
    public async Task UploadOracle(IAttachment OracleJson, [Autocomplete(typeof(PublicSubAutoComplete))]int? setName = null)
    {
        try
        {
            var fileTask = GetAttachmentAsync(OracleJson).ConfigureAwait(false);

            await DeferAsync().ConfigureAwait(false);

            var file = await fileTask;
            var oracle = JsonConvert.DeserializeObject<Oracle>(file);

            var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);

            var sets = existing.GameDataSets.Where(gs => gs.CreatorId == Context.Interaction.User.Id && (setName == null || gs.Id == setName));
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
            else if (sets.Count() > 1)
            {
                await FollowupAsync($"Error: There was more than one set found, please specify the set you'd like to use.", ephemeral: true).ConfigureAwait(false);
                return;
            }

            var set = existing.GameDataSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && (setName == null || gs.Id == setName));
            set.Oracles.Add(oracle);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await FollowupAsync($"Oracle added", ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await FollowupAsync($"Something happened and your oracle wasn't uploaded.", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }

    [SlashCommand("asset", "Adds a custom asset to the bot. Must be in Dataforged or Asset Workbench format")]
    public async Task UploadAsset(IAttachment AssetJson, 
        [Autocomplete(typeof(PublicSubAutoComplete))]int? setName = null, 
        AssetImportFormat format = AssetImportFormat.Dataforged)
    {
        try
        {
            var fileTask = GetAttachmentAsync(AssetJson).ConfigureAwait(false);

            await DeferAsync().ConfigureAwait(false);

            var file = await fileTask;
            var asset = (format == AssetImportFormat.Dataforged) ? JsonConvert.DeserializeObject<Asset>(file) : new AssetWorkbenchAdapter(file);

            var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);

            var sets = existing.GameDataSets.Where(gs => gs.CreatorId == Context.Interaction.User.Id && (setName == null || gs.Id == setName));
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
            else if (sets.Count() > 1)
            {
                await FollowupAsync($"Error: There was more than one set found, please specify the set you'd like to use.", ephemeral: true).ConfigureAwait(false);
                return;
            }

            var set = existing.GameDataSets.SingleOrDefault(gs => gs.CreatorId == Context.Interaction.User.Id && (setName == null || gs.Id == setName));
            set.Assets.Add(asset);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await FollowupAsync($"Asset added", ephemeral: true).ConfigureAwait(false);
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
