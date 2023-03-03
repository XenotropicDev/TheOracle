using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Server.DiscordServer;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.Data;

namespace Server.Data;

public class PlayerDataFactory
{
    private readonly IDbContextFactory<ApplicationContext> dbFactory;
    private IAssetRepository Assets { get; }
    private IMoveRepository Moves { get; }
    private IOracleRepository Oracles { get; }
    public IEntityRepository Entities { get; }

    public PlayerDataFactory(IAssetRepository assets, IMoveRepository moves, IOracleRepository oracles, IEntityRepository entities, IDbContextFactory<ApplicationContext> dbFactory)
    {
        Assets = assets;
        Moves = moves;
        Oracles = oracles;
        Entities = entities;
        this.dbFactory = dbFactory;
    }

    public async Task<IEnumerable<Asset>> GetPlayerAssets(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();

        //Todo: maybe there's a way to set the default subscription set for a new bot user?
        var player = await db.Players.Include(p => p.Assets).FirstOrDefaultAsync(p => p.DiscordId == PlayerId);
        return player.Assets;
    }

    public async Task<IEnumerable<OracleGameEntity>> GetPlayerEntites(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        
        return Entities.GetEntities().Where(a => a.Game == playerGame);
    }

    public async Task<IEnumerable<Move>> GetPlayerMoves(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        
        return Moves.GetMoveRoots().SelectMany(mr => mr.Moves).Where(a => a.Id.Contains(playerGame.ToString()));
    }

    public async Task<IEnumerable<Oracle>> GetPlayerOracles(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        var playerOracles = Oracles.GetOracleRoots()
            .SelectMany(or => or.Oracles)
            .Where(a => a.Id.Contains(playerGame.ToString())).ToList();

        foreach(var cat in Oracles.GetOracleRoots().Where(or => or.Categories?.Count > 0).SelectMany(or => or.Categories))
        {
            if (cat.Id.Contains(playerGame.ToString()))
            {
                playerOracles.AddRange(cat.Oracles);
            }
        }

        return playerOracles;
    }

    public async Task<IEnumerable<OracleRoot>> GetPlayerOraclesRoots(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        return Oracles.GetOracleRoots().Where(or => or.Id.Contains(playerGame.ToString()));
    }
}
