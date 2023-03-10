using System.Diagnostics;
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

public class UploadAssetCommand : InteractionModuleBase
{
    private readonly HttpClient http;

    public UploadAssetCommand(ApplicationContext db, HttpClient http)
    {
        Db = db;
        this.http = http;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("upload-asset", "Adds a game content subscription")]
    public async Task UploadAsset(IAttachment AssetJson, string SearchName, AssetImportFormat format = AssetImportFormat.Dataforged)
    {
        try
        {
            var fileTask = GetAttachmentAsync(AssetJson).ConfigureAwait(false);

            await DeferAsync().ConfigureAwait(false);

            var file = await fileTask;
            var asset = (format == AssetImportFormat.Dataforged) ? JsonConvert.DeserializeObject<Asset>(file) : new AssetWorkbenchAdapter(file);

            var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);

            //Todo: Actually implement this
            var contentSet = new GameContentSet();
            Db.GameContentSets.Add(contentSet);
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
