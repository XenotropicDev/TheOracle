namespace TheOracle2.Data.AssetWorkbench;

public class AssetWorkBenchData
{
    public int? documentFormatVersion { get; set; }
    public string type { get; set; }
    public string name { get; set; }
    public string? writeIn { get; set; }
    public int? track { get; set; }
    public string? description { get; set; }
    public Ability[] abilities { get; set; }
    public Fonts fonts { get; set; }
    public Icon? icon { get; set; }
    public string? writeIn2 { get; set; }
}

public class Fonts
{
    public string assetTypeFontSize { get; set; }
    public string assetNameFontSize { get; set; }
    public string detailsFontSize { get; set; }
    public string trackFontSize { get; set; }
}

public class Icon
{
    public string? name { get; set; }
    public string? author { get; set; }
    public string? dataUri { get; set; }
}

public class Ability
{
    public bool filled { get; set; }
    public string text { get; set; }
    public string name { get; set; }
}
