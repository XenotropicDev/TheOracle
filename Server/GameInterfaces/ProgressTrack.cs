using Server.Data;
using Server.DiceRoller;
using Server.DiscordServer;
using Server.Interactions.Helpers;
using TheOracle2;
using TheOracle2.Data;
using TheOracle2.GameObjects;

namespace Server.GameInterfaces
{
    public class ProgressTrack : ITrack
    {
        private readonly IMoveRepository moves;
        private readonly PlayerDataFactory factory;
        private readonly ApplicationContext db;

        private IEmoteRepository Emotes { get; }
        private readonly Random Random;

        public ProgressTrack(Random random, ChallengeRank rank, IEmoteRepository emotes, IMoveRepository moves, PlayerDataFactory factory, ulong playerId, string? title = null, string? desc = null, int score = 0)
        {
            Random = random;
            Emotes = emotes;
            this.moves = moves;
            this.factory = factory;
            TrackData = new TrackData
            {
                Rank = rank,
                Ticks = score * BoxSize,
                Title = title,
                Description = desc,
                PlayerId = playerId
            };
        }

        public ProgressTrack(TrackData trackData, Random random, IEmoteRepository emotes, IMoveRepository moves, PlayerDataFactory factory)
        {
            TrackData = trackData;
            Random = random;
            Emotes = emotes;
            this.moves = moves;
            this.factory = factory;
        }

        public TrackData TrackData { get; set; }
        public int BoxSize { get; set; } = 4;
        public bool IsEphemeral { get; set; } = false;
        public string? DiscordMessage { get; set; } = null;
        public string TrackDisplayName { get; set; } = "Track";
        public int TrackSize { get; set; } = 10;
        public int Ticks { get => TrackData.Ticks; set => TrackData.Ticks = value; }

        public ComponentBuilder? GetComponents()
        {
            return new ComponentBuilder().WithRows(GetActionRows());
        }

        public EmbedBuilder? GetEmbed()
        {
            var builder = new EmbedBuilder();
            builder.WithAuthor("Progress Track")
                .WithTitle(TrackData.Title)
                .WithDescription(TrackData.Description)
                .AddField("Rank", TrackData.Rank)
                .AddField(this.AsDiscordField(Emotes, Ticks));

            return builder;
        }

        public int GetScore()
        {
            int rawScore = Ticks / BoxSize;
            return Math.Min(rawScore, TrackSize);
        }

        public IActionRoll Roll()
        {
            return new ProgressRollRandom(Random, GetScore(), $"Progress Roll for {TrackData.Title}");
        }

        internal  List<ActionRowBuilder> GetActionRows()
        {
            var myList = new List<ActionRowBuilder>();
            var actionRow1 = new ActionRowBuilder();
            ActionRowBuilder? actionRow2 = null;

            actionRow1.WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId($"progress-main-{TrackData.Id}")
                .WithPlaceholder("Manage tracker...")
                .AddOption("Mark Progress", $"track-increase", emote: Emotes.MarkProgress)
                .AddOption("Resolve Progress", $"track-roll", emote: Emotes.Roll));

            myList.Add(actionRow1);

            var movesToFind = new string[] { "Swear an Iron Vow", "Reach a Milestone", "Fulfill Your Vow", "Forsake Your Vow" };
            var vowMoves = factory.GetPlayerMoves(TrackData.PlayerId).Where(m => movesToFind.Any(s => s.Contains(m.Name, StringComparison.OrdinalIgnoreCase)));
            var referenceSelectBuilder = new SelectMenuBuilder().WithCustomId("move-references").WithPlaceholder("Reference Moves...");
            foreach (var move in vowMoves)
            {
                if (actionRow2 == null) actionRow2 = new ActionRowBuilder();

                referenceSelectBuilder.AddOption(move.MoveAsSelectOption(Emotes));
            }

            if (actionRow2 != null) myList.Add(actionRow2.WithSelectMenu(referenceSelectBuilder));
            return myList;
        }
    }
}
