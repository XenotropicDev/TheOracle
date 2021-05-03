using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Assets;
using TheOracle.GameCore.NpcGenerator;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.RulesReference;
using TheOracle.IronSworn.Delve;

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
                client.ReactionRemoved += reactionHandler.RemovedReactionHandler;

                new GenericReactions(services).Load();

                await client.SetGameAsync($"!Help | v{Assembly.GetEntryAssembly().GetName().Version}", "", ActivityType.Playing).ConfigureAwait(false);
                //await LogAsync(new LogMessage(LogSeverity.Info, "Info", $"Joined to {client.Rest.GetGuildsAsync().Result.Count} guilds")).ConfigureAwait(false);

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
            var commandConfig = new CommandServiceConfig { LogLevel = LogSeverity.Info };
            client ??= new DiscordSocketClient(clientConfig);
            command ??= new CommandService(commandConfig);

            var delveThemePath = Path.Combine("IronSworn", "themes.json");
            var delveDomainPath = Path.Combine("IronSworn", "domains.json");
            var delveService = DelveService.Load(new string[] { delveThemePath }, new string[] { delveDomainPath });
            var oracleService = new OracleService().Load();
            var AssetList = Asset.LoadAssetList();
            var spellingDictionary = Utilities.CreateDictionaryFromOracles(oracleService, AssetList);

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("token.json", optional: true, reloadOnChange: true)
                .Build();

            return new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(command)
                .AddSingleton(config)
                .AddSingleton(new CommandHandler(client, command))
                .AddSingleton(oracleService)
                .AddSingleton<RuleService>()
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .AddSingleton(AssetList)
                .AddSingleton(delveService)
                .AddScoped<NpcFactory>()
                .AddSingleton<InteractiveService>()
                .AddSingleton(spellingDictionary)
                .BuildServiceProvider();
        }
    }
}