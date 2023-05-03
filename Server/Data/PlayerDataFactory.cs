using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Server.DiscordServer;
using Server.GameInterfaces;
using TheOracle2;
using TheOracle2.Data;

namespace Server.Data;

public class PlayerDataFactory : IDisposable
{
    private readonly ApplicationContext db;
    private bool disposedValue;

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
        db = dbFactory.CreateDbContext();
    }

    public async Task<IEnumerable<Asset>> GetPlayerAssets(ulong PlayerId, IronGame? gameOverride = null)
    {
        var player = await db.Players.FirstOrDefaultAsync(p => p.DiscordId == PlayerId) ?? await Player.CreateDefault(db, PlayerId);
        
        return player.GameDataSets.SelectMany(gds => gds.Assets);
    }

    public async Task<IEnumerable<OracleGameEntity>> GetPlayerEntites(ulong PlayerId, IronGame? gameOverride = null)
    {
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        
        return Entities.GetEntities().Where(a => a.Game == playerGame);
    }

    public async Task<IEnumerable<Move>> GetPlayerMoves(ulong PlayerId, IronGame? gameOverride = null)
    {
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        
        return Moves.GetMoveRoots().SelectMany(mr => mr.Moves).Where(a => a.JsonId.Contains(playerGame.ToString()));
    }

    public async Task<IEnumerable<Oracle>> GetPlayerOracles(ulong PlayerId, IronGame? gameOverride = null)
    {
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        var playerOracles = Oracles.GetOracleRoots()
            .SelectMany(or => or.Oracles)
            .Where(a => a.JsonId.Contains(playerGame.ToString())).ToList();

        foreach(var cat in Oracles.GetOracleRoots().Where(or => or.Categories?.Count > 0).SelectMany(or => or.Categories))
        {
            if (cat.JsonId.Contains(playerGame.ToString()))
            {
                playerOracles.AddRange(cat.Oracles);
            }
        }

        return playerOracles;
    }

    public async Task<IEnumerable<OracleRoot>> GetPlayerOraclesRoots(ulong PlayerId, IronGame? gameOverride = null)
    {
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        return Oracles.GetOracleRoots().Where(or => or.JsonId.Contains(playerGame.ToString()));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                db.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
