using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using TheOracle2.GameObjects;

namespace TheOracle2;

/// <summary>
/// General-purpose components for managing messages.
/// </summary>

public class GenericComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("delete-original-response")]
    public async Task DeleteOriginalAction()
    {
        await DeferAsync();
        await Context.Interaction.Message.DeleteAsync();
    }

    public static ButtonBuilder CancelButton(string label = null)
    {
        return new ButtonBuilder(label ?? "Cancel", "delete-original-response", style: ButtonStyle.Secondary);
    }

    //Todo: Not sure if this is something I want or if this is even possible with the way ephemeral's work
    //[ComponentInteraction("ephemeral-reveal")]
    //public async Task RevealEphemeral()
    //{
    //    SocketMessageComponent interaction = Context.Interaction as SocketMessageComponent;
    //    SocketUserMessage message = interaction.Message;
    //    ComponentBuilder components = ComponentBuilder.FromComponents(message.Components);
    //    components.RemoveComponentById("ephemeral-reveal");
    //    await RespondAsync(ephemeral: false, embeds: message.Embeds as Embed[], components: components.Build());
    //}
}
