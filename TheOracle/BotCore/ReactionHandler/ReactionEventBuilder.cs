using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class ReactionEventBuilder
    {
        private IEmote emote;
        private Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task> reactionAdded;

        public ReactionEventBuilder WithEmote(IEmote emote)
        {
            this.emote = emote;
            return this;
        }

        public ReactionEventBuilder WithEmoji(string value)
        {
            this.emote = new Emoji(value);
            return this;
        }

        public ReactionEventBuilder WithEvent(Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task> value)
        {
            this.reactionAdded = value;
            return this;
        }

        public ReactionEvent Build()
        {
            ReactionEvent reactionEvent = new ReactionEvent();
            reactionEvent.Emote = this.emote;
            reactionEvent.ReactionAdded += this.reactionAdded;

            return reactionEvent;
        }
    }
}