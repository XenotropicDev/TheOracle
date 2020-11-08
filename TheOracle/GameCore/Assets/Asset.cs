using Discord;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public class Asset : IAsset
    {
        public Asset()
        {
            AssetFields = new List<AssetField>();
            InputFields = new List<string>();
            MultiFieldAssetTrack = new MultiFieldAssetTrack();
            NumericAssetTrack = new NumericAssetTrack();
            CountingAssetTrack = new CountingAssetTrack();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string AssetType { get; set; }
        public List<AssetField> AssetFields { get; set; }
        public MultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        public CountingAssetTrack CountingAssetTrack { get; set; }
        public NumericAssetTrack NumericAssetTrack { get; set; }
        public List<string> InputFields { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public Embed GetEmbed(string[] arguments)
        {
            int nextArgument = 0;

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor(AssetType, IconUrl);
            builder.WithTitle(Name);

            string fullDesc = string.Empty;
            foreach (var fld in InputFields)
            {
                string userVal = (arguments.Length - 1 >= nextArgument) ? arguments[nextArgument] : string.Empty.PadLeft(8);
                fullDesc += $"__{fld} : {userVal}__\n";
                nextArgument++;
            }
            fullDesc += (fullDesc.Length > 0) ? "\n" + Description : Description;

            builder.WithDescription(fullDesc);

            foreach (var fld in AssetFields)
            {
                string label = (fld.Enabled) ? ":record_button:" : ":blue_square:";

                string inputField = string.Empty;
                if (fld.InputFields?.Count > 0)
                {
                    foreach (var inputItem in fld.InputFields)
                    {
                        string userVal = (arguments.Length - 1 >= nextArgument) ? arguments[nextArgument] : string.Empty.PadLeft(8);
                        inputField += $"\n__{inputItem} : {userVal}__";
                        nextArgument++;
                    }
                }

                builder.AddField(label, fld.Text + inputField);
            }

            if (MultiFieldAssetTrack?.Fields != null)
            {
                foreach (var trackItem in MultiFieldAssetTrack.Fields)
                {
                    string text = (trackItem.IsActive) ? trackItem.ActiveText : trackItem.InactiveText;
                    builder.AddField(trackItem.Name, text, true);
                }
            }

            if (CountingAssetTrack?.Name != null)
            {
                builder.AddField(CountingAssetTrack.Name, CountingAssetTrack.StartingValue);
            }

            if (NumericAssetTrack != null && !(NumericAssetTrack.Max == 0 && NumericAssetTrack.Min == 0))
            {
                string trackText = string.Empty;
                for (int i = NumericAssetTrack.Min; i <= NumericAssetTrack.Max; i++) trackText += $"{i} ";
                trackText = trackText.Trim().Replace(NumericAssetTrack.ActiveNumber.ToString(), $"__**{NumericAssetTrack.ActiveNumber}**__");
                builder.AddField(AssetResources.Track, trackText);
            }

            return builder.Build();
        }
    }
}