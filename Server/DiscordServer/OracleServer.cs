global using System.Linq;
global using Discord;
global using Microsoft.Extensions.DependencyInjection;
global using Newtonsoft.Json;

using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using OracleCommands;
using Server.Data;
using Server.OracleRoller;
using Server.DiscordServer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheOracle2;
using Microsoft.Extensions.Caching.Memory;
using Discord.Net.Queue;
using System.Net.Http.Headers;
using System.Diagnostics;
using TheOracle2.Data;

class OracleServer
{
    internal static Serilog.ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger(); //This gets overwritten by the ConfiguredServices

    public static async Task Main()
    {
        using var services = ConfigureServices();
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings() { Formatting = Formatting.Indented, MetadataPropertyHandling = MetadataPropertyHandling.Ignore };

        var client = services.GetRequiredService<DiscordSocketClient>();
        var commands = services.GetRequiredService<InteractionService>();
        var config = services.GetRequiredService<IConfiguration>();
        var handler = services.GetRequiredService<CommandHandler>();
        var db = services.GetRequiredService<ApplicationContext>();

        try
        {
            Log.Logger = logger;

            logger.Information($"Starting TheOracle v{Assembly.GetEntryAssembly()?.GetName().Version}");

            db.Database.EnsureDeleted();
            await db.Database.EnsureCreatedAsync().ConfigureAwait(false);
            await addDataForged(db, services);

            await handler.Initialize().ConfigureAwait(false);

            await client.LoginAsync(TokenType.Bot, GetToken(services)).ConfigureAwait(false);

            client.Log += LogAsync;
            commands.Log += LogAsync;

            await client.StartAsync().ConfigureAwait(false);

            await client.SetGameAsync($"v{Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3)}", "", ActivityType.Playing).ConfigureAwait(false);

        }
        catch (Exception ex)
        {
            logger.Error(ex.ToString());
        }

        await Task.Delay(Timeout.Infinite);
    }

    private static async Task addDataForged(ApplicationContext db, ServiceProvider services)
    {
        var oracles = services.GetRequiredService<IOracleRepository>().GetOracles().ToList();
        var moves = services.GetRequiredService<IMoveRepository>().GetMoves().ToList();
        var assets = services.GetRequiredService<IAssetRepository>().GetAssets().ToList();

        var ironSet = new Server.Data.homebrew.GameContentSet() { CreatorId = 1, SetName = "Ironsworn", IsPublic = true  };
        foreach (var oracle in oracles.Where(o => o.JsonId.StartsWith("Ironsworn")))
        {
            ironSet.Oracles.Add(oracle);
        }

        foreach (var asset in assets.Where(a => a.JsonId.StartsWith("Ironsworn")))
        {
            ironSet.Assets.Add(asset);
        }

        foreach (var move in moves.Where(a => a.JsonId.StartsWith("Ironsworn")))
        {
            ironSet.Moves.Add(move);
        }

        var starSet = new Server.Data.homebrew.GameContentSet() { CreatorId = 1, SetName = "Starforged", IsPublic = true };
        foreach (var oracle in oracles.Where(o => o.JsonId.StartsWith("Starforged")))
        {
            starSet.Oracles.Add(oracle);
        }

        foreach (var asset in assets.Where(a => a.JsonId.StartsWith("Starforged")))
        {
            starSet.Assets.Add(asset);
        }

        foreach (var move in moves.Where(a => a.JsonId.StartsWith("Starforged")))
        {
            starSet.Moves.Add(move);
        }

        await db.GameContentSets.AddAsync(ironSet);
        await db.GameContentSets.AddAsync(starSet);

        await db.SaveChangesAsync();
    }

    private static Task LogAsync(LogMessage msg)
    {
        switch (msg.Exception)
        {
            case GatewayReconnectException gre:
                logger.Warning(gre.Message);
                return Task.CompletedTask;
            case TimeoutException te:
                logger.Warning(te.Message);
                return Task.CompletedTask;
            case Discord.Net.HttpException httpEx:
                string json = (httpEx.Request is JsonRestRequest jsonRequest) ? $"\n" + jsonRequest.Json : "";
                logger.Warning(httpEx, httpEx.Reason + json);
                return Task.CompletedTask;
            case Exception ex when ex.Message == "WebSocket connection was closed":
                logger.Warning(ex.Message);
                return Task.CompletedTask;
            default:
                break;
        }
        
        switch (msg.Severity)
        {
            case LogSeverity.Critical:
                logger.Fatal(msg.ToString());
                break;
            case LogSeverity.Error:
                logger.Error(msg.ToString());
                break;
            case LogSeverity.Warning:
                logger.Warning(msg.ToString());
                break;
            case LogSeverity.Info:
                logger.Information(msg.ToString());
                break;
            case LogSeverity.Verbose:
                logger.Verbose(msg.ToString());
                break;
            case LogSeverity.Debug:
                logger.Debug(msg.ToString());
                break;
            default:
                break;
        }
        return Task.CompletedTask;
    }

    static ServiceProvider ConfigureServices()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("token.json", optional: true, reloadOnChange: true)
            .AddJsonFile("dbSettings.json", optional: false, reloadOnChange: true)
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();

        var dbConn = config.GetSection("dbConnectionString").Value;
        var dbPass = config.GetSection("dbPassword").Value;
        var dbConnBuilder = new NpgsqlConnectionStringBuilder(dbConn) { Password = dbPass };

        var clientConfig = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info, GatewayIntents = GatewayIntents.DirectMessages | GatewayIntents.GuildMessages | GatewayIntents.Guilds };
        var interactionServiceConfig = new InteractionServiceConfig() { UseCompiledLambda = true, LogLevel = LogSeverity.Info, AutoServiceScopes = true };

        logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine("logs", "log.txt"), rollingInterval: RollingInterval.Day)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .CreateLogger();

        return new ServiceCollection()
            .AddSingleton<IConfiguration>(config)
            .AddSingleton(new DiscordSocketClient(clientConfig))
            .AddSingleton(interactionServiceConfig)
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<Random>()
            .AddSingleton<IOracleRoller, RandomOracleRoller>()
            .AddSingleton<IOracleRepository, JsonOracleRepository>()
            .AddSingleton<IMoveRepository, JsonMoveRepository>()
            .AddSingleton<IAssetRepository, JsonAssetRepository>()
            .AddSingleton<IEmoteRepository, HardCodedEmoteRepo>()
            .AddSingleton<IEntityRepository, JsonEntityRepository>()
            .AddSingleton<IMemoryCache, MemoryCache>()
            .AddSingleton<HttpClient>()
            .AddScoped<PlayerDataFactory>()
            .AddDbContextFactory<ApplicationContext>()
            .AddLogging(builder => builder.AddSerilog(logger)
                .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
                .AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning)
                )
            .AddDbContext<ApplicationContext>(options => options.UseNpgsql(dbConnBuilder.ConnectionString))
            .BuildServiceProvider();
    }

    private static string GetToken(IServiceProvider services)
    {
        var token = Environment.GetEnvironmentVariable("DiscordToken");
        token ??= services.GetRequiredService<IConfiguration>().GetSection("DiscordToken").Value;

        if (token == null)
        {
            Console.WriteLine($"Couldn't find a discord token. Please enter it here. (It will be saved to the token.json file in your bin folder)");
            token = Console.ReadLine();

            var json = JsonConvert.SerializeObject(new { DiscordToken = token });
            File.WriteAllText("token.json", json);
        }

        return token ?? String.Empty;
    }

}
