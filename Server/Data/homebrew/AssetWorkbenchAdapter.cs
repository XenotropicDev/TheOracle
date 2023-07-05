using System.Collections.ObjectModel;
using Dataforged;

namespace TheOracle2.Data.AssetWorkbench;

public class AssetWorkbenchAdapter : Asset
{
    public AssetWorkbenchAdapter(string jsonData)
    {
        WorkbenchData = JsonConvert.DeserializeObject<AssetWorkBenchData>(jsonData);
        Convert();
    }

    public AssetWorkbenchAdapter(AssetWorkBenchData data)
    {
        WorkbenchData = data;
        Convert();
    }

    public AssetWorkBenchData WorkbenchData { get; }

    private Asset Convert()
    {
        Name = WorkbenchData.name;
        if (WorkbenchData.track != null) ConditionMeter = new AssetConditionMeter { Min = 0, Value = WorkbenchData.track ?? 0, Max = WorkbenchData.track ?? 0};
        Abilities = new ObservableCollection<AssetAbility>(
            WorkbenchData.abilities
            .Select(ability => new AssetAbility
            {
                Text = ability.text,
                Name = ability.name,
                Enabled = ability.filled
            })
            .ToList()
            );
        
        Options = new Dictionary<string, AssetOptionField>();

        if (!string.IsNullOrWhiteSpace(WorkbenchData.writeIn)) Options.Add("Name", new AssetOptionFieldText() { Value = WorkbenchData.writeIn});
        if (!string.IsNullOrWhiteSpace(WorkbenchData.writeIn2)) Options.Add("Name2", new AssetOptionFieldText() { Value = WorkbenchData.writeIn2});

        return this;
    }
}
