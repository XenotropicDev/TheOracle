using Server.DiscordServer;
using TheOracle2.Data;

namespace Server.GameInterfaces;

public record AssetData
{
    public AssetData()
    {
    }

    public AssetData(Asset asset, ulong id)
    {
        AssetId = asset.Id;
        SelectedAbilities = asset.Abilities.Where(a => a.Enabled).Select(a => a.JsonId).ToList();
        CreatorDiscordId = id;
        if (asset.ConditionMeter?.Value > 0)
        {
            ConditionValue = asset.ConditionMeter.Value ?? 0;
        }
    }

    public int Id { get; set; }
    public ulong CreatorDiscordId { get; set; }
    public uint AssetId { get; set; }
    public virtual Asset Asset { get; set; }
    public IList<string> SelectedAbilities { get; set; } = new List<string>();
    public IList<string> Inputs { get; set; } = new List<string>();
    public int ConditionValue { get; set; }
    public string? ThumbnailURL { get; set; }

    public void ChangeConditionValue(int change, Asset asset)
    {
        if (ConditionValue + change > asset.ConditionMeter.Max) return;
        if (ConditionValue + change < asset.ConditionMeter.Min) return;

        ConditionValue += change;
    }
}
