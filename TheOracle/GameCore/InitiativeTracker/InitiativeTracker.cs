using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.BotCore;

namespace TheOracle.GameCore.InitiativeTracker
{
    public class InitiativeTracker
    {
        private ChannelSettings ChannelSettings;

        public InitiativeTracker(ChannelSettings channelSettings = null)
        {
            this.ChannelSettings = channelSettings;
        }

        public InitiativeTracker WithChannelSettings(ChannelSettings channelSettings)
        {
            this.ChannelSettings = channelSettings;
            return this;
        }

        public List<string> Advantage { get; set; } = new List<string>();
        public List<string> Disadvantage { get; set; } = new List<string>();
        public string Description { get; set; }
        public string IconUrl { get; private set; }

        public EmbedBuilder GetEmbedBuilder()
        {
            string advantageTitle = (ChannelSettings?.DefaultGame == GameName.Starforged) ? InitiativeResources.StarforgedAdvantage : InitiativeResources.Advantage;
            string disadvantageTitle = (ChannelSettings?.DefaultGame == GameName.Starforged) ? InitiativeResources.StarforgedDisadvantage : InitiativeResources.Disadvantage;
            return new EmbedBuilder()
                .WithTitle(InitiativeResources.TrackerTitle)
                .WithDescription(Description)
                .WithThumbnailUrl(IconUrl)
                .WithFields(new EmbedFieldBuilder()
                    .WithName(advantageTitle)
                    .WithValue((Advantage.Count > 0) ? String.Join('\n', Advantage) : InitiativeResources.None)
                    .WithIsInline(true))
                .WithFields(new EmbedFieldBuilder()
                    .WithName(disadvantageTitle)
                    .WithValue((Disadvantage.Count > 0) ? String.Join('\n', Disadvantage) : InitiativeResources.None)
                    .WithIsInline(true))
                ;
        }

        public static bool IsInitiativeTrackerMessage(IUserMessage message)
        {
            return (message?.Embeds?.Any(embed => embed.Title == InitiativeResources.TrackerTitle)) ?? false;
        }

        internal static InitiativeTracker FromMessage(IUserMessage message)
        {
            var embed = message.Embeds.First(embed => embed.Title == InitiativeResources.TrackerTitle);

            var tracker = new InitiativeTracker
            {
                Description = embed.Description,
                Advantage = embed.Fields.First(field => field.Name == InitiativeResources.Advantage || field.Name == InitiativeResources.StarforgedAdvantage).Value.Split('\n').ToList(),
                Disadvantage = embed.Fields.First(field => field.Name == InitiativeResources.Disadvantage || field.Name == InitiativeResources.StarforgedDisadvantage).Value.Split('\n').ToList(),
                IconUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : null
            };

            tracker.Advantage.RemoveAll(s => s == InitiativeResources.None);
            tracker.Disadvantage.RemoveAll(s => s == InitiativeResources.None);

            return tracker;
        }
    }
}