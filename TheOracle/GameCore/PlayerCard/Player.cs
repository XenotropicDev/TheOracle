using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
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

            builder.WithAuthor(Name);
            builder.WithThumbnailUrl(AvatarUrl);
            if (DescriptionField.Length > 0) builder.WithDescription(DescriptionField);
            builder.WithTitle(PlayerResources.PlayerCardTitle);
            builder.AddField(PlayerResources.Stats, statsString, false);
            builder.AddField(PlayerResources.Health, Health, true);
            builder.AddField(PlayerResources.Spirit, Spirit, true);
            builder.AddField(PlayerResources.Supply, Supply, true);

            builder.AddField(PlayerResources.Momentum, Momentum, true);

            if (Debilities > 0) builder.AddField(debilitiesTitle, Debilities, true);

            return builder;
        }

        public Player PopulateFromEmbed(IEmbed embed)
        {
            this.Name = embed.Author.Value.Name;
            this.AvatarUrl = embed.Thumbnail?.Url;

            embed.Fields.First(fld => fld.Name == PlayerResources.Stats).Value.UndoFormatString(PlayerResources.StatsFormat, out string[] statsValues, true);

            if (!int.TryParse(statsValues[1], out int edge)) throw new ArgumentException($"{statsValues[0]} is in an unknown format {statsValues[1]}");
            if (!int.TryParse(statsValues[3], out int heart)) throw new ArgumentException($"{statsValues[2]} is in an unknown format {statsValues[3]}");
            if (!int.TryParse(statsValues[5], out int iron)) throw new ArgumentException($"{statsValues[4]} is in an unknown format {statsValues[5]}");
            if (!int.TryParse(statsValues[7], out int shadow)) throw new ArgumentException($"{statsValues[6]} is in an unknown format {statsValues[7]}");
            if (!int.TryParse(statsValues[9], out int wits)) throw new ArgumentException($"{statsValues[8]} is in an unknown format {statsValues[9]}");

            Edge = edge;
            Heart = heart;
            Iron = iron;
            Shadow = shadow;
            Wits = wits;

            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Health).Value, out int health)) throw new ArgumentException($"Unknown value for {PlayerResources.Health}");
            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Spirit).Value, out int spirit)) throw new ArgumentException($"Unknown value for {PlayerResources.Spirit}");
            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Supply).Value, out int supply)) throw new ArgumentException($"Unknown value for {PlayerResources.Supply}");
            if (!int.TryParse(embed.Fields.First(fld => fld.Name == PlayerResources.Momentum).Value, out int momentum)) throw new ArgumentException($"Unknown value for {PlayerResources.Momentum}");

            Health = health;
            Spirit = spirit;
            Supply = supply;
            Momentum = momentum;

            DescriptionField = embed.Description;

            EmbedField? debilityField = embed.Fields.FirstOrDefault(fld => fld.Name.Equals(PlayerResources.Debilities) || fld.Name.Equals(PlayerResources.StarforgedDebilities));
            if (debilityField.HasValue && int.TryParse(debilityField.Value.Value, out int debilities))
                Debilities = debilities;

            return this;
        }

        internal Player WithChannelSettings(ChannelSettings channelSettings)
        {
            ChannelSettings = channelSettings;
            return this;
        }
    }
}