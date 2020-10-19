using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheOracle.GameCore.InitiativeTracker
{
    public class InitiativeTracker
    {
        public List<string> Advantage { get; set; } = new List<string>();
        public List<string> Disadvantage { get; set; } = new List<string>();
        public string Description { get; set; }

        public EmbedBuilder GetEmbedBuilder()
        {
            return new EmbedBuilder()
                .WithTitle(InitiativeResources.TrackerTitle)
                .WithDescription(Description)
                .WithFields(new EmbedFieldBuilder()
                    .WithName(InitiativeResources.Advantage)
                    .WithValue((Advantage.Count > 0) ? String.Join('\n', Advantage) : InitiativeResources.None)
                    .WithIsInline(true))
                .WithFields(new EmbedFieldBuilder()
                    .WithName(InitiativeResources.Disadvantage)
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
                Advantage = embed.Fields.First(field => field.Name == InitiativeResources.Advantage).Value.Split('\n').ToList(),
                Disadvantage = embed.Fields.First(field => field.Name == InitiativeResources.Disadvantage).Value.Split('\n').ToList()
            };

            tracker.Advantage.RemoveAll(s => s == InitiativeResources.None);
            tracker.Disadvantage.RemoveAll(s => s == InitiativeResources.None);

            return tracker;
        }
    }
}