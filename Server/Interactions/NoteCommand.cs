using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;

namespace Server.Interactions;

public class NoteCommand : InteractionModuleBase
{
    [SlashCommand("note", "Make a note with editable fields")]
    public async Task PostNote(
    [Summary(description: "The title for the note.")] string title,
    [Summary(description: "The description is the text directly below the title")] string description
    )
    {
        var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description);

        var buttons = new ComponentBuilder().WithButton("Edit", "edit-note");

        await RespondAsync(embed: embed.Build(), components: buttons.Build()).ConfigureAwait(false);
    }

    // Responds to the modal.
    [ModalInteraction("edit_note:*")]
    public async Task ModalResponse(ulong messageId, EditNoteModal modal)
    {
        await DeferAsync();

        if (await Context.Channel.GetMessageAsync(messageId) is not IUserMessage message) return;

        var embed = message.Embeds.FirstOrDefault().ToEmbedBuilder();

        if (modal.EmbedTitle.Length > 0) embed.WithTitle(modal.EmbedTitle);
        if (modal.Description.Length > 0) embed.WithDescription(modal.Description);
        if (modal.Field1.Length > 0)
        {
            if (embed.Fields.Any()) embed.Fields[0].Value = modal.Field1;
            else embed.AddField("Field 1", modal.Field1);
        }
        if (modal.Field2.Length > 0)
        {
            if (embed.Fields.Count >= 2) embed.Fields[1].Value = modal.Field1;
            else embed.AddField("Field 2", modal.Field1);
        }

        await message.ModifyAsync(msg => msg.Embed = embed.Build());
    }
}

public class NoteComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("edit-note")]
    public async Task EditNote()
    {
        var message = Context.Interaction.Message;
        
        await Context.Interaction.RespondWithModalAsync<EditNoteModal>($"edit_note:{message.Id}");
    }
}

public class EditNoteModal : IModal
{
    public string Title => "Edit Note";
    
    [InputLabel("Title")]
    [ModalTextInput("note_title", placeholder: "New Title - Leave blank to keep old", maxLength: EmbedBuilder.MaxTitleLength)]
    [RequiredInput(false)]
    public string EmbedTitle { get; set; }

    [InputLabel("Description")]
    [ModalTextInput("note_desc", TextInputStyle.Paragraph, "New Description - Leave blank to keep old", maxLength: 4000)]
    [RequiredInput(false)]
    public string Description { get; set; }

    //Todo: Maybe make it possible for the field title something other than "Field #"
    [InputLabel("Field 1")]
    [ModalTextInput("note_field1", TextInputStyle.Paragraph, "Field 1 Value - Leave blank to keep old", maxLength: EmbedFieldBuilder.MaxFieldValueLength)]
    [RequiredInput(false)]
    public string Field1 { get; set; }

    [InputLabel("Field 2")]
    [ModalTextInput("note_field2", TextInputStyle.Paragraph, "Field 2 Value - Leave blank to keep old", maxLength: EmbedFieldBuilder.MaxFieldValueLength)]
    [RequiredInput(false)]
    public string Field2 { get; set; }
}
