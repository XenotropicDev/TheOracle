using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DiceRoller;
using Server.DiscordServer;
using Server.GameInterfaces;
using Server.Interactions.Helpers;
using Server.OracleRoller;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

public class AssetCommand : InteractionModuleBase
{
    public AssetCommand(IAssetRepository AssetRepo, ApplicationContext db)
    {
        assetRepo = AssetRepo;
        this.db = db;
    }

    private readonly IAssetRepository assetRepo;
    private readonly ApplicationContext db;

    [SlashCommand("asset", "Generates an asset")]
    public async Task BuildAsset([Autocomplete(typeof(AssetAutoComplete))] string asset)
    {
        var assetInfo = assetRepo.GetAsset(asset);
        if (assetInfo == null) return;

        var AssetData = new AssetData(assetInfo, Context.Interaction.User.Id);
        db.CharacterAssets.Add(AssetData);
        await db.SaveChangesAsync().ConfigureAwait(false);

        if (assetInfo.Inputs?.Count > 0)
        {
            var modal = new ModalBuilder().WithTitle($"{assetInfo.Name} Fields").WithCustomId($"Asset-Input-Modal{assetInfo.Inputs.Count}:{AssetData.Id}");

            for (var i = 0; i < assetInfo.Inputs.Count; i++)
            {
                modal.AddTextInput($"{assetInfo.Name} - {assetInfo.Inputs[i].Name}", GenericInputModal.GetGenericIdWord(i), required: true);
            }

            await RespondWithModalAsync(modal.Build());
            return;
        }

        var discordEntity = new DiscordAssetEntity(assetInfo, AssetData);

        await discordEntity.EntityAsResponse(RespondAsync).ConfigureAwait(false);
    }

    [ModalInteraction("Asset-Input-Modal1:*")]
    public async Task AssetFromModal(int AssetDataId, GenericInputModal<string> modal)
    {
        await ProcessAssetModalResponse(AssetDataId, modal.First).ConfigureAwait(false);
    }

    [ModalInteraction("Asset-Input-Modal2:*")]
    public async Task AssetFromModal(int AssetDataId, GenericInputModal<string, string> modal)
    {
        await ProcessAssetModalResponse(AssetDataId, modal.First, modal.Second).ConfigureAwait(false);
    }

    [ModalInteraction("Asset-Input-Modal3:*")]
    public async Task AssetFromModal(int AssetDataId, GenericInputModal<string, string, string> modal)
    {
        await ProcessAssetModalResponse(AssetDataId, modal.First, modal.Second, modal.Third).ConfigureAwait(false);
    }

    [ModalInteraction("Asset-Input-Modal4:*")]
    public async Task AssetFromModal(int AssetDataId, GenericInputModal<string, string, string, string> modal)
    {
        await ProcessAssetModalResponse(AssetDataId, modal.First, modal.Second, modal.Third, modal.Fourth).ConfigureAwait(false);
    }

    [ModalInteraction("Asset-Input-Modal5:*")]
    public async Task AssetFromModal(int AssetDataId, GenericInputModal<string, string, string, string, string> modal)
    {
        await ProcessAssetModalResponse(AssetDataId, modal.First, modal.Second, modal.Third, modal.Fourth, modal.Fifth).ConfigureAwait(false);
    }

    private async Task ProcessAssetModalResponse(int assetDataId, params string[] data)
    {
        var AssetData = await db.CharacterAssets.FindAsync(assetDataId).ConfigureAwait(false);
        if (AssetData == null) throw new ArgumentException($"Unknown character asset id: {assetDataId}");

        AssetData.Inputs = data;

        var discordEntity = new DiscordAssetEntity(AssetData.Asset, AssetData);

        await discordEntity.EntityAsResponse(RespondAsync).ConfigureAwait(false);
        await db.SaveChangesAsync().ConfigureAwait(false);
    }
}

public class AssetInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public AssetInteractions(IAssetRepository assets, ApplicationContext db, Random random, IEmoteRepository emotes, PlayerDataFactory dataFactory)
    {
        assetRepo = assets;
        this.db = db;
        this.random = random;
        this.emotes = emotes;
        this.dataFactory = dataFactory;
    }

    private readonly IAssetRepository assetRepo;
    private readonly ApplicationContext db;
    private readonly Random random;
    private readonly IEmoteRepository emotes;
    private readonly PlayerDataFactory dataFactory;

    [ComponentInteraction("asset-ability-select:*")]
    public async Task AbilitySelection(int characterAssetId, string[] selections)
    {
        var data = db.CharacterAssets.Find(characterAssetId);
        if (data == null) return;

        data.SelectedAbilities = selections.ToList();

        AddAnyThumbnails(Context.Interaction.Message, data);
        var discordEntity = new DiscordAssetEntity(data.Asset, data);

        await Context.Interaction.UpdateAsync(async msg =>
        {
            msg.Embeds = discordEntity.AsEmbedArray();
            msg.Components = (await discordEntity.GetComponentsAsync())?.Build();
        }).ConfigureAwait(false);

        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    [ComponentInteraction("asset-condition-select:*")]
    public async Task ConditionSelection(int characterAssetId, string[] values)
    {
        var data = db.CharacterAssets.Find(characterAssetId);
        if (data == null) return;

        switch (values.FirstOrDefault())
        {
            case "asset-condition-up":
                data.ChangeConditionValue(+1, data.Asset);
                break;

            case "asset-condition-down":
                data.ChangeConditionValue(-1, data.Asset);
                break;

            case "asset-condition-roll":
                var roll = new ActionRollRandom(random, emotes, dataFactory, db, Context.User.Id, data.ConditionValue, 0);
                await roll.EntityAsResponse(RespondAsync).ConfigureAwait(false);
                return;

            default:
                break;
        }

        AddAnyThumbnails(Context.Interaction.Message, data);

        var discordEntity = new DiscordAssetEntity(data.Asset, data);
        await Context.Interaction.UpdateAsync(async msg =>
        {
            msg.Embeds = discordEntity.AsEmbedArray();
            msg.Components = (await discordEntity.GetComponentsAsync())?.Build();
        }).ConfigureAwait(false);

        await db.SaveChangesAsync().ConfigureAwait(false);

    }

    private void AddAnyThumbnails(IUserMessage msg, AssetData data)
    {
        if (msg?.Embeds?.FirstOrDefault(e => e.Thumbnail.HasValue)?.Thumbnail is EmbedThumbnail thumb)
        {
            data.ThumbnailURL = thumb.Url;
        }
    }
}
