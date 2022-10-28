namespace TheOracle2.GameObjects;

using Server.DiceRoller;
using Server.GameInterfaces;

/// <summary>
/// Interface inherited by all ranked and unranked tracks.
/// </summary>
public interface ITrack : IDiscordEntity
{
    /// <summary>
    /// The number of ticks per progress box.
    /// </summary>
    public int BoxSize { get; set; }

    /// <summary>
    /// The number of ticks per progress track.
    /// </summary>
    public int MaxTicks { get; set; }

    /// <summary>
    /// The name of the track, normally "Track" works well.
    /// </summary>
    public string TrackDisplayName { get; set; }

    /// <summary>
    /// The number of progress boxes per progress track.
    /// </summary>
    public int TrackSize { get; set; }

    /// <summary>
    /// Calculates a progress score from a given number of ticks, capped by TrackSize.
    /// </summary>
    /// <returns></returns>
    public int GetScore();

    /// <summary>
    /// Gets or sets the current amount of completed ticks for the track
    /// </summary>
    public TrackData TrackData { get; set; }

    public IActionRoll Roll();
}

public static class TrackExtensions
{
    /// <summary>
    /// Special hidden character for mobile formatting small emojis.
    /// </summary>
    public const string MobileEmojiSizer = "\u200C";

    private static string MarkedBoxesPattern => "/:progress([1-4]):/";
    private static string PartialBoxesPattern => "/:progress(0|1|2|3):/";

    public static EmbedFieldBuilder StrikeField(EmbedFieldBuilder field)
    {
        field = StrikeFieldName(field);
        field = StrikeFieldValue(field);
        return field;
    }

    public static EmbedFieldBuilder StrikeFieldName(EmbedFieldBuilder field)
    {
        field.Name = "~~" + field.Name + "~~";
        return field;
    }

    public static EmbedFieldBuilder StrikeFieldValue(EmbedFieldBuilder field)
    {
        field.Value = "~~" + field.Value + "~~";
        return field;
    }

    /// <summary>
    /// Renders a text description of a progress amount, for instance: "1 box", "2 ticks", "2 boxes and 2 ticks"
    /// </summary>
    public static string TickString(this ITrack track, int ticks)
    {
        if (ticks >= 3)
        {
            int boxes = ticks / track.BoxSize;
            int remainder = ticks % track.BoxSize;
            string result = boxes.ToString() + " " + (boxes > 1 ? "boxes" : "box");
            if (remainder > 0)
            {
                string remainderTickAutoPlural = (remainder == 1) ? "tick" : "ticks";
                result += $" and {remainder} {remainderTickAutoPlural}";
            }
            return result;
        }
        string tickAutoPlural = (ticks == 1) ? "tick" : "ticks";
        return $"{ticks} {tickAutoPlural}";
    }

    public static string TicksToEmojiTrack(this ITrack track, int ticks, IEmoteRepository emotes)
    {
        var score = track.GetScore();
        List<IEmote> emojis = Enumerable.Repeat(emotes.ProgressEmotes[track.BoxSize], score).ToList();

        int remainder = ticks % track.BoxSize;
        if (remainder > 0)
        {
            emojis.Add(emotes.ProgressEmotes[remainder]);
        }

        int padding = track.TrackSize;
        if (emojis?.Count > 0)
        {
            padding -= emojis.Count;
        }

        emojis!.AddRange(
          Enumerable.Repeat(
            emotes.ProgressEmotes[0],
            padding
        ));
        string result = string.Join(" ", emojis);
        result += MobileEmojiSizer;
        return result;
    }

    public static EmbedFieldBuilder AsDiscordField(this ITrack track, IEmoteRepository emotes, int ticks = 0)
    {
        return new EmbedFieldBuilder()
          .WithName($"{track.TrackDisplayName} [{track.GetScore()}/{track.TrackSize}]")
          .WithValue(TicksToEmojiTrack(track, ticks, emotes));
    }
}
