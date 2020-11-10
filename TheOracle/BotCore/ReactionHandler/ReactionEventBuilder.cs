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
        private Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task> reactionRemoved;

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

        public ReactionEventBuilder WithRemoveEvent(Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task> value)
        {
            this.reactionRemoved = value;
            return this;
        }

        public ReactionEvent Build()
        {
            ReactionEvent reactionEvent = new ReactionEvent();
            reactionEvent.Emote = this.emote;
            
            if (reactionAdded != null) reactionEvent.ReactionAdded += this.reactionAdded;
            if (reactionRemoved != null) reactionEvent.ReactionRemoved += this.reactionRemoved;

            return reactionEvent;
        }
    }
}