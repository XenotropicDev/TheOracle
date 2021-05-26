using Discord;

namespace TheOracle.GameCore.Assets
{
    public interface IAssetEmbedField
    {
        public string Name { get; set; }
        string ActiveText { get; set; }
        string InactiveText { get; set; }
        bool IsActive { get; set; }

        public EmbedField ToDiscordEmbedField();
    }

    public class AssetEmbedField : IAssetEmbedField
    {
        public string Name { get; set; }
        public string ActiveText { get; set; }
        public string InactiveText { get; set; }
        public bool IsActive { get; set; }

        public EmbedField ToDiscordEmbedField()
        {
            throw new System.NotImplementedException();
        }
    }
}