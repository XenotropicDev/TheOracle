using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle
{
    internal class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InstallCommandsAsync(services);

                client.ReactionAdded += ReactionAdded;

                await Task.Delay(Timeout.Infinite);
            }
        }

        //TODO find some way to allow reactions to be added per assembly, or at least use the event pattern
        private Task ReactionAdded(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;
            //if (message.Author.Id != BotUserId) return Task.CompletedTask; //TODO make sure this will work if we ever move to a sharded bot

            if (reaction.Emote.Name == "🔍") PlanetCommands.CloserLook(message, channel, reaction);
            if (reaction.Emote.Name == "\U0001F996") PlanetCommands.Life(message, channel, reaction);

            return Task.CompletedTask;
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices(DiscordSocketClient client = null, CommandService command = null)
        {
            var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
            client ??= new DiscordSocketClient(clientConfig);
            command ??= new CommandService();
            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(command)
                .AddSingleton(new CommandHandler(client, command))
                .AddSingleton<OracleService>()
                .BuildServiceProvider();
        }
    }
}