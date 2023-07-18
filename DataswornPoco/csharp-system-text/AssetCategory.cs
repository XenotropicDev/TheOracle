namespace Dataforged;

public class AssetCategory
{
    public string Name { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public Dictionary<string, Asset> Contents { get; set; }
    public string Id { get; set; }
    public Source Source { get; set; }
}
