using Discord;
using System;
using System.Linq;
using TheOracle.BotCore;
using TheOracle.Core;

namespace TheOracle.GameCore.ProgressTracker
{
    public class ProgressTracker
    {
        public const int dangerousTicks = 8;
        public const int epicTicks = 1;
        public const int extremeTicks = 2;
        public const int formidableTicks = 4;
        public const int totalTicks = 40;
        public const int troublesomeTicks = 12;

        public ProgressTracker(ChallengeRank ChallengeRank, string title, int ticks = 0)
        {
            Rank = ChallengeRank;
            Title = title;
            Ticks = ticks;
        }

        public ProgressTracker(IUserMessage message)
        {
            var embed = message.Embeds.FirstOrDefault(item => item.Title == ProgressResources.Progress_Tracker);
            if (embed == null) return;

            var startingPercent = Utilities.ConvertPercentToDecimal(embed.Footer.Value.Text);
            if (!Enum.TryParse(embed.Fields.FirstOrDefault(f => f.Name == ProgressResources.Difficulty).Value, out ChallengeRank cr))
                throw new ArgumentException("Unknown progress tracker post format, unable to parse difficulty");

            Rank = cr;
            Ticks = (int)Math.Floor(startingPercent * totalTicks);
            Title = embed.Description;
        }

        public ChallengeRank Rank { get; set; }
        public int Ticks { get; set; }
        public string Title { get; set; }

        private int TicksPerProgress { get => ChallengeRankToTicks(Rank); }

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

        public Embed BuildEmbed()
        {
            return new EmbedBuilder()
                .WithTitle(ProgressResources.Progress_Tracker)
                .WithDescription(Title)
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Difficulty,
                    Value = ProgressResources.ResourceManager.GetString(Rank.ToString()),
                    IsInline = true
                })
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Progress_Bar,
                    Value = BuildProgressGraphic(Ticks),
                    IsInline = true
                })
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Progress_Amount,
                    Value = BuildProgressAmount(Ticks),
                    IsInline = true
                })
                .WithFooter(((double)Ticks / totalTicks).ToString("P2"))
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

        private object BuildProgressAmount(int ticks)
        {
            return $"{(int)Math.Floor(Ticks / 4d)}/10";
        }

        private string BuildProgressGraphic(int ticks)
        {
            string fill = new string('#', (int)Math.Floor(Ticks / 4d));
            string finalTickMark = ((Ticks % 4) == 1) ? "-" : ((Ticks % 4) == 2) ? "+" : ((Ticks % 4) == 3) ? "*" : string.Empty;
            fill = (fill + finalTickMark).PadRight(10, '·');

            fill = String.Join(' ', fill.ToCharArray()); //Add spaces between each character

            //Replace all the filler characters with emojis
            fill = fill.Replace("·", "🟦");
            fill = fill.Replace("-", "\uD83C\uDDEE");
            fill = fill.Replace("+", "\uD83C\uDDFD");
            fill = fill.Replace("*", "*️⃣");
            fill = fill.Replace("#", "\u0023\u20E3");

            return fill;
        }
    }
}