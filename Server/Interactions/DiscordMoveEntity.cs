using TheOracle2.Data;

namespace TheOracle2.UserContent;

public class DiscordMoveEntity : IDiscordEntity
{
    private readonly IEmoteRepository emotes;

    public DiscordMoveEntity(Move move, IEmoteRepository emotes, bool ephemeral = false)
    {
        Move = move;
        this.emotes = emotes;
        IsEphemeral = ephemeral;
    }

    public bool IsEphemeral { get; set; }
    public Move Move { get; }
    public string? DiscordMessage { get; set; } = null;

    public SelectMenuOptionBuilder ReferenceOption()
    {
        string append = "…";
        int maxChars = SelectMenuOptionBuilder.MaxDescriptionLength;
        SelectMenuOptionBuilder option = new();
        string moveTrigger = Move.Trigger?.Text ?? Move.Text;
        string triggerString = moveTrigger.Length <= maxChars ? moveTrigger : moveTrigger[0..(maxChars - 1)] + append;
        option.WithLabel(Move.Name);
        option.WithEmote(GetEmoji());
        option.WithValue(Move.Name);
        option.WithDescription(triggerString);
        return option;
    }

    public IEmote GetEmoji()
    {
        if (emotes.AsDictionary().ContainsKey(Move.Name)) { return emotes.AsDictionary()[Move.Name]; }
        if (emotes.AsDictionary().ContainsKey(Move.Category)) { return emotes.AsDictionary()[Move.Category]; }
        return new Emoji("📖");
    }

    public EmbedBuilder? GetEmbed()
    {
        return new EmbedBuilder()
            .WithAuthor(Move.Parent?.Display.Title ?? Move.Category)
            .WithTitle(Move.Name)
            .WithDescription(Move.Text);
    }

    public Task<ComponentBuilder?> GetComponentsAsync()
    {
        return Task.FromResult<ComponentBuilder?>(null);
    }
}
