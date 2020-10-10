using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.GameCore.RulesReference;
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

                #nullable enable
                string? token = Environment.GetEnvironmentVariable("DiscordToken");
                if (token == null)
                {
                    JObject jsonToken = JObject.Parse(File.ReadAllText("token.json"));
                    token = (string)jsonToken.SelectToken("DiscordToken");
                }
                #nullable disable

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InstallCommandsAsync(services);

                client.JoinedGuild += LogGuildJoin;

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogGuildJoin(SocketGuild arg)
        {
            LogMessage msg = new LogMessage(LogSeverity.Info, "", $"The bot has been added to a new guild: {arg.Name}");
            LogAsync(msg);
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
                .AddSingleton<RuleService>()
                .BuildServiceProvider();
        }
    }
}