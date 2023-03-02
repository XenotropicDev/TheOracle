namespace TheOracle2.Data.AssetWorkbench;

internal class AssetWorkbenchAdapter : Asset
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
        AssetType = WorkbenchData.type;
        Name = WorkbenchData.name;
        Display = new Display { Title = WorkbenchData.name, Icon = WorkbenchData.icon?.dataUri };
        if (WorkbenchData.track != null) ConditionMeter = new ConditionMeter { Min = 0, Value = WorkbenchData.track ?? 0, Max = WorkbenchData.track ?? 0};
        Abilities = WorkbenchData.abilities
            .Select(ability => new TheOracle2.Data.Ability
            {
                Text = ability.text,
                Name = ability.name,
                Enabled = ability.filled
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(WorkbenchData.writeIn)) Inputs.Add(new() { InputType = AssetInput.Text, Name = WorkbenchData.writeIn, Adjustable = true });
        if (!string.IsNullOrWhiteSpace(WorkbenchData.writeIn2)) Inputs.Add(new() { InputType = AssetInput.Text, Name = WorkbenchData.writeIn2, Adjustable = true });

        return this;
    }
}
