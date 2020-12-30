using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TheOracle
{
    public class ReferencedMessageCommandHandler
    {
        internal static async Task<bool> Process(SocketUserMessage message)
        {
            Uri url;
            bool messageHasUrl = Uri.TryCreate(message.Content, UriKind.Absolute, out url)
                && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);

            if ((message.Attachments.Count > 0 || messageHasUrl) && message.ReferencedMessage.Embeds.Count > 0)
            {
                if (!messageHasUrl) url = new Uri(message.Attachments.First().Url);
                var embed = (message.ReferencedMessage as IUserMessage).Embeds.First();
                await message.ReferencedMessage.ModifyAsync(msg => msg.Embed = embed.ToEmbedBuilder().WithThumbnailUrl(url.ToString()).Build());
                await message.DeleteAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }
    }
}