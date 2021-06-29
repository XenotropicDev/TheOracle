using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.OracleObjectTemplate
{
    public class OracleEmbed
    {
        private OracleInfo oracleInfo;
        public string Author { get; set; }
        public string BottomImageUrl { get; set; }
        public bool ShowDescription { get; set; } = true;
        public List<EmbedAction> EmbedActions { get; set; }
        public string Footer { get; set; }
        public OracleEmbedHelper Helper { get; set; }
        public string ThumbnailUrl { get; set; }
        public string OracleInfoBaseName { get; set; }
        public string Title { get; set; }
        public bool AddRename { get; set; }

        public Embed GetEmbed(OracleInfo oracleInfo)
        {
            var builder = new EmbedBuilder();
            builder.WithAuthor(Author)
                .WithTitle(Title)
                .WithFooter(Footer)
                .WithThumbnailUrl(ThumbnailUrl)
                .WithImageUrl(BottomImageUrl);

            foreach (var initialField in oracleInfo.Oracles.Where(o => o.Initial))
            {
                builder.AddField(initialField.Name, initialField.Roll(), true);
            }

            return builder.Build();
        }

        //todo: delete this sample code.
        public void FillinTemplate()
        {
            OracleInfoBaseName = "Planet";
            Title = "{0}|[Planet Name]"; //the {0} is the first user input, if empty/null use the Planet Name Oracle

            EmbedActions = new List<EmbedAction>()
            {
                new EmbedAction
                {
                    ComponentTitle = "Activate to reveal life",
                    FieldTitle = "Life",
                    FieldValue = "[Life]",
                    RemoveComponentOnUse = true
                }
            };

            var json = JsonConvert.SerializeObject(this);
        }
    }

    public class OracleEmbedHelper
    {
        public string Description { get; set; }
        public string Footer { get; set; }
        public List<EmbedAction> HelperActions { get; set; }
        public string Title { get; set; }
    }
}