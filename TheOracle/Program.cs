using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.GameCore.NpcGenerator;
using TheOracle.GameCore.RulesReference;

namespace TheOracle
{
    internal class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();


        public async Task MainAsync()
        {
            using (DiscordChannelContext dbContext = new DiscordChannelContext())
            {
                dbContext.Database.EnsureCreated();
            }

            using (ServiceProvider services = ConfigureServices())
            {
                Console.WriteLine($"Starting TheOracle v{Assembly.GetEntryAssembly().GetName().Version}");
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                #nullable enable
                string? token = Environment.GetEnvironmentVariable("DiscordToken");
                if (token == null)
                {
                    token = services.GetRequiredService<IConfigurationRoot>().GetSection("DiscordToken").Value;
                }
                #nullable disable

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InstallCommandsAsync(services);

                var reactionHandler = new GlobalReactionHandler(services);
                client.ReactionAdded += reactionHandler.ReactionEventHandler;

                await client.SetGameAsync($"!Help | v{Assembly.GetEntryAssembly().GetName().Version}", "", ActivityType.Playing).ConfigureAwait(false);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage msg)
        { 
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices(DiscordSocketClient client = null, CommandService command = null)
        {
            var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info };
            client ??= new DiscordSocketClient(clientConfig);
            command ??= new CommandService();

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("channelsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("token.json", optional: true, reloadOnChange: true)
                .Build();

            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(command)
                .AddSingleton(config)
                .AddSingleton(new CommandHandler(client, command))
                .AddSingleton<OracleService>()
                .AddSingleton<RuleService>()
                .AddSingleton<HookedEvents>()
                .AddScoped<NpcFactory>()
                .BuildServiceProvider();
        }
    }
}