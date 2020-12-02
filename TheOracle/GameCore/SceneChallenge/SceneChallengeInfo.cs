using Discord;
using System;
using System.Linq;
using TheOracle.GameCore.ProgressTracker;

namespace TheOracle.GameCore.SceneChallenge
{
    public class SceneChallengeInfo : ProgressTrackerInfo
    {
        private int countdownValue = 0;
        public int CountDownValue { get => countdownValue; set => countdownValue = (value > 4 || value < 0) ? countdownValue : value; }

        public SceneChallengeInfo() : base()
        {
        }

        public SceneChallengeInfo(ChallengeRank ChallengeRank, string description, int startingTicks = 0) : base(ChallengeRank, description, startingTicks)
        {
        }

        public override IEmbed BuildEmbed()
        {
            var builder = base.BuildEmbed().ToEmbedBuilder();
            builder.Title = SceneChallengeResources.SceneChallenge;

            builder.AddField(SceneChallengeResources.CountdownAmount, $"{CountDownValue}/4", true); //globalization: figure out how to use different number systems/symbols
            builder.AddField(SceneChallengeResources.CountdownTrack, BuildCountDownGraphic(), true);

            return builder.Build();
        }

        public SceneChallengeInfo FromMessage(IUserMessage message, ChallengeRank challengeRank = ChallengeRank.None)
        {
            base.PopulateFromMessage(message, challengeRank);

            var countdownFieldValue = message.Embeds.First().Fields.FirstOrDefault(fld => fld.Name.Contains(SceneChallengeResources.CountdownAmount));
            int countDownParsed;
            if (countdownFieldValue.Value == null || !int.TryParse(countdownFieldValue.Value.Replace("/4", string.Empty), out countDownParsed)) countDownParsed = 0;
            CountDownValue = countDownParsed;

            return this;
        }

        public virtual string BuildCountDownGraphic()
        {
            //Use standard characters as stand-ins so that we can do easy string math
            string fill = new string('#', CountDownValue);
            fill = fill.PadRight(4, '·');

            fill = String.Join(' ', fill.ToCharArray()); //Add spaces between each character

            //Replace all the stand-in characters with emojis
            fill = fill.Replace("·", "🟩");
            fill = fill.Replace("#", "❎");

            return fill;
        }
    }
}