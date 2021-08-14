﻿using Discord;
using System;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.GameCore.ProgressTracker
{
    public class ProgressTrackerInfo
    {
        public const int dangerousTicks = 8;
        public const int epicTicks = 1;
        public const int extremeTicks = 2;
        public const int formidableTicks = 4;
        public const int totalTicks = 40;
        public const int troublesomeTicks = 12;
        private int ticks;

        public ProgressTrackerInfo(ChallengeRank ChallengeRank, string description, int startingTicks = 0)
        {
            Rank = ChallengeRank;
            Description = description;
            Ticks = startingTicks;
        }
        public ProgressTrackerInfo()
        {

        }

        public ProgressTrackerInfo PopulateFromMessage(IUserMessage message, ChallengeRank challengeRank = ChallengeRank.None)
        {
            var embed = message.Embeds.FirstOrDefault();
            if (embed == null) throw new NullReferenceException();

            if (challengeRank == ChallengeRank.None && !Enum.TryParse(embed.Fields.FirstOrDefault(f => f.Name == DifficultyFieldTitle).Value, out challengeRank))
                throw new ArgumentException("Unknown progress tracker post format, unable to parse difficulty");

            Rank = challengeRank;

            if (embed.Footer.HasValue)
            {
                Ticks = (Int32.TryParse(embed.Footer.Value.Text.Replace(ProgressResources.Ticks, "").Replace(":", ""), out int temp)) ? temp : 0;
            }
            Description = embed.Description;
            this.IconUrl = embed.Thumbnail?.Url;

            return this;
        }

        public ChallengeRank Rank { get; set; }

        public int Ticks
        {
            get => ticks;
            set
            {
                if (value < 0) value = 0; 
                if (value > totalTicks) value = totalTicks; 
                ticks = value;
            }
        }

        public string Description { get; set; }
        public string IconUrl { get; private set; }
        public virtual int TicksPerProgress { get => ChallengeRankToTicks(Rank); }
        public int ActionDie { get => (int)Math.Floor(Ticks / 4d); }
        public virtual string DifficultyFieldTitle { get => ProgressResources.Difficulty; }

        public static bool HasMatchingChallengeRank(string rank, string[] trackerArgs)
        {
            return trackerArgs.Any(s => rank.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        private int ChallengeRankToTicks(ChallengeRank cr)
        {
            if (cr == ChallengeRank.Troublesome) return troublesomeTicks;
            if (cr == ChallengeRank.Dangerous) return dangerousTicks;
            if (cr == ChallengeRank.Formidable) return formidableTicks;
            if (cr == ChallengeRank.Extreme) return extremeTicks;
            if (cr == ChallengeRank.Epic) return epicTicks;
            return epicTicks;
        }

        public virtual IEmbed BuildEmbed()
        {
            return new EmbedBuilder()
                .WithTitle(ProgressResources.Progress_Tracker)
                .WithAuthor(ProgressResources.Progress_Tracker)
                .WithThumbnailUrl(IconUrl)
                .WithDescription(Description)
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = DifficultyFieldTitle,
                    Value = ProgressResources.ResourceManager.GetString(Rank.ToString()),
                    IsInline = true
                })
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Progress_Bar,
                    Value = GetProgressGraphic(),
                    IsInline = true
                })
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Progress_Amount,
                    Value = BuildProgressAmount(Ticks),
                    IsInline = true
                })
                .WithFooter($"{ProgressResources.Ticks}: {Ticks}")
                .Build();
        }

        public void RemoveProgress(int amount = 1)
        {
            Ticks -= TicksPerProgress;
        }
        public void AddProgress(int amount = 1)
        {
            Ticks += TicksPerProgress;
        }

        public virtual string BuildProgressAmount(int ticks)
        {
            return $"{ActionDie}/10";
        }

        public virtual string GetProgressGraphic()
        {
            //Use standard characters as stand-ins so that we can do easy string math
            string fill = new string('#', (int)Math.Floor(Ticks / 4d));
            string finalTickMark = ((Ticks % 4) == 1) ? "-" : ((Ticks % 4) == 2) ? "+" : ((Ticks % 4) == 3) ? "*" : string.Empty;
            fill = (fill + finalTickMark).PadRight(10, '·');

            fill = String.Join(' ', fill.ToCharArray()); //Add spaces between each character

            //Replace all the stand-in characters with emojis
            fill = fill.Replace("·", "🟦");
            fill = fill.Replace("-", "\uD83C\uDDEE");
            fill = fill.Replace("+", "\uD83C\uDDFD");
            fill = fill.Replace("*", "*️⃣");
            fill = fill.Replace("#", "\u0023\u20E3");

            return fill;
        }

        public static bool IsProgressTrackerMessage(IMessage message)
        {
            return message?.Embeds?.Any(embed => embed.Title == ProgressResources.Progress_Tracker) ?? false;
        }
    }
}