using Discord;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.BotCore;

namespace TheOracle.GameCore.PlayerCard
{
    public class Player
    {
        private int health = 5;
        private int spirit = 5;
        private int supply = 5;
        private int momentum = 2;

        public string Name { get; set; }
        public int Edge { get; set; }
        public int Heart { get; set; }
        public int Iron { get; set; }
        public int Shadow { get; set; }
        public int Wits { get; set; }
        public int Health { get => health; set => health = (value >= 0 && value <= 5) ? value : health; }
        public int Spirit { get => spirit; set => spirit = (value >= 0 && value <= 5) ? value : spirit; }
        public int Supply { get => supply; set => supply = (value >= 0 && value <= 5) ? value : supply; }
        public int Momentum { get => momentum; set => momentum = (value >= -6 && value <= 10) ? value : momentum; }
        public string AvatarUrl { get; set; }
        public int Debilities { get; set; } = 0;
        public ChannelSettings ChannelSettings { get; internal set; }
        public string DescriptionField { get; private set; }
        public string StatsField { get; private set; }

        public string XPDisplay
        {
            get
            {
                if (SpentXp > 0) return string.Format(PlayerResources.XPDisplayFormatWithSpent, UnspentXp, SpentXp);
                return string.Format(PlayerResources.XPDisplayFormatWithoutSpent, UnspentXp);
            }
        }

        public int UnspentXp { get; set; }
        public int SpentXp { get; set; }

        public Player()
        {
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            string debilitiesTitle = (ChannelSettings?.DefaultGame == GameName.Starforged) ? PlayerResources.StarforgedDebilities : PlayerResources.Debilities;
            var builder = new EmbedBuilder();

            string statsString = String.Format(PlayerResources.StatsFormat,
                PlayerResources.Edge, Edge,
                PlayerResources.Heart, Heart,
                PlayerResources.Iron, Iron,
                PlayerResources.Shadow, Shadow,
                PlayerResources.Wits, Wits);

            builder.WithTitle(Name);
            builder.WithThumbnailUrl(AvatarUrl);
            if (DescriptionField?.Length > 0) builder.WithDescription(DescriptionField);

            builder.WithAuthor(PlayerResources.PlayerCardTitle);

            if (StatsField?.Length > 0) builder.AddField(PlayerResources.Stats, StatsField, false);
            else builder.AddField(PlayerResources.Stats, statsString, false);

            builder.AddField(PlayerResources.Health, Health, true);
            builder.AddField(PlayerResources.Spirit, Spirit, true);
            builder.AddField(PlayerResources.Supply, Supply, true);

            builder.AddField(PlayerResources.Momentum, Momentum, true);

            if (Debilities > 0) builder.AddField(debilitiesTitle, Debilities, true);

            builder.AddField(PlayerResources.XP, XPDisplay, true);

            return builder;
        }

        public Player PopulateFromEmbed(IEmbed embed)
        {
            if (embed.Author.HasValue && embed.Author.Value.Name == PlayerResources.PlayerCardTitle)
            {
                this.Name = embed.Title;
            }
            else
            {
                this.Name = embed.Author.Value.Name;
            }

            this.AvatarUrl = embed.Thumbnail?.Url;

            var stats = Regex.Matches(embed.Fields.FirstOrDefault(fld => fld.Name == PlayerResources.Stats).Value, @"\d+");
            if (stats.Count >= 5)
            {
                int.TryParse(stats[0].Value, out int edge);
                int.TryParse(stats[1].Value, out int heart);
                int.TryParse(stats[2].Value, out int iron);
                int.TryParse(stats[3].Value, out int shadow);
                int.TryParse(stats[4].Value, out int wits);

                Edge = edge;
                Heart = heart;
                Iron = iron;
                Shadow = shadow;
                Wits = wits;
            }

            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Health).Value, out int health)) throw new ArgumentException($"Unknown value for {PlayerResources.Health}");
            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Spirit).Value, out int spirit)) throw new ArgumentException($"Unknown value for {PlayerResources.Spirit}");
            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Supply).Value, out int supply)) throw new ArgumentException($"Unknown value for {PlayerResources.Supply}");
            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Momentum).Value, out int momentum)) throw new ArgumentException($"Unknown value for {PlayerResources.Momentum}");

            Health = health;
            Spirit = spirit;
            Supply = supply;
            Momentum = momentum;

            DescriptionField = embed.Description;
            StatsField = embed.Fields.FirstOrDefault(fld => fld.Name == PlayerResources.Stats).Value;

            EmbedField? debilityField = embed.Fields.FirstOrDefault(fld => fld.Name.Equals(PlayerResources.Debilities) || fld.Name.Equals(PlayerResources.StarforgedDebilities));
            if (debilityField.HasValue && int.TryParse(debilityField.Value.Value, out int debilities))
                Debilities = debilities;

            EmbedField XPField = embed.Fields.FirstOrDefault(fld => fld.Name.Equals(PlayerResources.XP));
            if (XPField.Value != null) this.SetXPFromField(XPField);

            return this;
        }

        private void SetXPFromField(EmbedField xPField)
        {
            if (Utilities.UndoFormatString(xPField.Value, PlayerResources.XPDisplayFormatWithSpent, out string[] spentValues))
            {
                if (int.TryParse(spentValues[0], out int unspent) && int.TryParse(spentValues[1], out int spent))
                {
                    UnspentXp = unspent;
                    SpentXp = spent;
                }
            }
            else if (Utilities.UndoFormatString(xPField.Value, PlayerResources.XPDisplayFormatWithoutSpent, out string[] unspentValue))
            {
                if (int.TryParse(unspentValue[0], out int unspent)) UnspentXp = unspent;
            }
        }

        internal Player WithChannelSettings(ChannelSettings channelSettings)
        {
            ChannelSettings = channelSettings;
            return this;
        }
    }
}