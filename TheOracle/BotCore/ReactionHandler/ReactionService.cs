using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class ReactionService
    {
        public List<ReactionEvent> reactionList = new List<ReactionEvent>();
        public List<ReactionEvent> reactionRemovedList = new List<ReactionEvent>();
    }

    public class ReactionEvent
    {
        public IEmote Emote { get; set; }

        public event Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task> ReactionAdded
        {
            add { ReactionAddedEvent.Add(value); }
            remove { ReactionAddedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task>> ReactionAddedEvent = new AsyncEvent<Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task>>();

        public event Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task> ReactionRemoved
        {
            add { ReactionRemovedEvent.Add(value); }
            remove { ReactionRemovedEvent.Remove(value); }
        }

        internal readonly AsyncEvent<Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task>> ReactionRemovedEvent = new AsyncEvent<Func<IUserMessage, ISocketMessageChannel, SocketReaction, IUser, Task>>();
    }

    internal class AsyncEvent<T> where T : class
    {
        private readonly object _subLock = new object();
        internal ImmutableArray<T> _subscriptions;

        public bool HasSubscribers => _subscriptions.Length != 0;
        public IReadOnlyList<T> Subscriptions => _subscriptions;

        public AsyncEvent()
        {
            _subscriptions = ImmutableArray.Create<T>();
        }

        public void Add(T subscriber)
        {
            NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Add(subscriber);
        }

        public void Remove(T subscriber)
        {
            NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Remove(subscriber);
        }

        public static void NotNull<T>(T obj, string name, string msg = null) where T : class
        {
            if (obj == null) throw CreateNotNullException(name, msg);
        }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(paramName: name);
            else return new ArgumentNullException(paramName: name, message: msg);
        }
    }

    internal static class EventExtensions
    {
        public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke().ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> eventHandler, T1 arg1, T2 arg2)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            try
            {
                var subscribers = eventHandler.Subscriptions;
                for (int i = 0; i < subscribers.Count; i++)
                    await subscribers[i].Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
            }
            catch (Discord.Net.HttpException httpEx)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {arg4} triggered a {httpEx.GetType()} - {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {ex.GetType()} - {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
        }
    }
}