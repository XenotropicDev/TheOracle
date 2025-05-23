using Server.DiscordServer;
// using TheOracle2.Data; // Removed as it's no longer needed
using Server.GameInterfaces.DTOs; // Added for DTOs
using System.Linq; // Added for LINQ methods

namespace Server.GameInterfaces;

public record AssetData
{
    public AssetData()
    {
    }

    public AssetData(AssetDTO assetDto, ulong id) // Changed parameter to AssetDTO
    {
        AssetId = assetDto.Id;
        // Updated to use assetDto and added null check for Abilities
        SelectedAbilities = assetDto.Abilities?.Where(a => a.Enabled).Select(a => a.Id).ToList() ?? new List<string>();
        CreatorDiscordId = id;
        if (assetDto.ConditionMeter != null && assetDto.ConditionMeter.Value > 0) // Updated to use assetDto
        {
            ConditionValue = assetDto.ConditionMeter.Value;
        }
    }

    public int Id { get; set; }
    public ulong CreatorDiscordId { get; set; }
    public string AssetId { get; set; } = String.Empty;
    public IList<string> SelectedAbilities { get; set; } = new List<string>();
    public IList<string> Inputs { get; set; } = new List<string>(); // Remains as IList<string>
    public int ConditionValue { get; set; }
    public string? ThumbnailURL { get; set; }

    // Changed parameter from Asset to ConditionMeterDTO
    public void ChangeConditionValue(int change, ConditionMeterDTO conditionMeterDto) 
    {
        if (conditionMeterDto == null) return; // Added null check

        // Updated to use conditionMeterDto
        if (ConditionValue + change > conditionMeterDto.Max) return;
        if (ConditionValue + change < conditionMeterDto.Min) return;

        ConditionValue += change;
    }
}
