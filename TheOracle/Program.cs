using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle
{
    internal class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        public async Task MainAsync()
        {
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
            await _client.StartAsync();

            CommandService cs = new CommandService();
            CommandHandler ch = new CommandHandler(_client, cs);

            CommandHandlerRegister.Handler = ch;

            await ch.InstallCommandsAsync();

            _client.ReactionAdded += ReactionAdded;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        //TODO find some way to allow reactions to be added per assembly
        private Task ReactionAdded(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;
            if (message.Author.Id != _client.CurrentUser.Id) return Task.CompletedTask; //TODO make sure this will work if we ever move to a sharded bot

            if (reaction.Emote.Name == "🔍") PlanetCommands.CloserLook(message, channel, reaction);
            if (reaction.Emote.Name == "\U0001F996") PlanetCommands.Life(message, channel, reaction);

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}