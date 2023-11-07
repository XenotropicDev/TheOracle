using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using TheOracle2;

namespace Server.Interactions.Helpers;

public static class DiscordEntityExtensions
{
    public static Embed[]? AsEmbedArray(this IDiscordEntity entity)
    {
        var embed = entity.GetEmbed();
        if (embed == null) return null;

        return new Embed[] { embed.Build() };
    }

    public static async Task<MessageComponent?> AsMessageComponent(this IDiscordEntity entity)
    {
        var comp = await entity.GetComponentsAsync();
        if (comp == null) return null;

        return comp.Build();
    }

    public static async Task EntityAsResponse(this IDiscordEntity entity, Func<string?, Embed[]?, bool, bool, AllowedMentions?, RequestOptions?, MessageComponent?, Embed?, Task> respondFunc)
    {
        await respondFunc(entity.DiscordMessage, entity.AsEmbedArray(), false, entity.IsEphemeral, null, null, await entity.AsMessageComponent(), null);
    }

    public static async Task<IUserMessage> EntityAsResponse(this IDiscordEntity entity, Func<string?, Embed[]?, bool, bool, AllowedMentions?, RequestOptions?, MessageComponent?, Embed?, Task<IUserMessage>> respondFunc)
    {
        return await respondFunc(entity.DiscordMessage, entity.AsEmbedArray(), false, entity.IsEphemeral, null, null, await entity.AsMessageComponent(), null);
    }

    public static async Task<IUserMessage> EntityAsReply(this IDiscordEntity entity, Func<string?, bool, Embed?, RequestOptions?, AllowedMentions?, MessageReference?, MessageComponent?, ISticker[], Embed[], MessageFlags, Task<IUserMessage>> replyFunc)
    {
        return await replyFunc(entity.DiscordMessage, false, entity.GetEmbed()?.Build(), null, null, null, await entity.AsMessageComponent(), null, null, MessageFlags.None);
    }
}
