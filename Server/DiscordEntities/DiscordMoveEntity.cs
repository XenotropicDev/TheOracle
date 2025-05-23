// using TheOracle2.Data; // Removed as Move is replaced by MoveDTO
using Server.GameInterfaces.DTOs; // Added for MoveDTO
using Discord; // For IEmote, Emoji, SelectMenuOptionBuilder, EmbedBuilder, ComponentBuilder
using Server.DiscordServer; // For IEmoteRepository (assuming it's here or a similar namespace)

namespace TheOracle2.UserContent; // Assuming this namespace from original

public class DiscordMoveEntity : IDiscordEntity
{
    private readonly IEmoteRepository emotes;

    public DiscordMoveEntity(MoveDTO moveDto, IEmoteRepository emotes, bool ephemeral = false) // Changed parameter
    {
        MoveDto = moveDto; // Updated assignment
        this.emotes = emotes;
        IsEphemeral = ephemeral;
    }

    public bool IsEphemeral { get; set; }
    public MoveDTO MoveDto { get; } // Changed field to MoveDto
    public string? DiscordMessage { get; set; } = null;

    public SelectMenuOptionBuilder ReferenceOption()
    {
        string append = "…";
        int maxChars = SelectMenuOptionBuilder.MaxDescriptionLength;
        SelectMenuOptionBuilder option = new();
        string moveTrigger = MoveDto.Trigger?.Text ?? MoveDto.Text; // Use MoveDto
        string triggerString = moveTrigger.Length <= maxChars ? moveTrigger : moveTrigger[0..(maxChars - 1)] + append;
        option.WithLabel(MoveDto.Name); // Use MoveDto
        option.WithEmote(GetEmoji());
        option.WithValue(MoveDto.Name); // Use MoveDto
        option.WithDescription(triggerString);
        return option;
    }

    public IEmote GetEmoji()
    {
        // Use MoveDto
        if (emotes.AsDictionary().ContainsKey(MoveDto.Name)) { return emotes.AsDictionary()[MoveDto.Name]; }
        if (emotes.AsDictionary().ContainsKey(MoveDto.Category)) { return emotes.AsDictionary()[MoveDto.Category]; }
        return new Emoji("📖");
    }

    public EmbedBuilder? GetEmbed()
    {
        return new EmbedBuilder()
            // Use MoveDto.Source.Name or MoveDto.Category
            .WithAuthor(MoveDto.Source?.Name ?? MoveDto.Category) 
            .WithTitle(MoveDto.Name) // Use MoveDto
            .WithDescription(MoveDto.Text); // Use MoveDto
    }

    public Task<ComponentBuilder?> GetComponentsAsync()
    {
        return Task.FromResult<ComponentBuilder?>(null);
    }
}
