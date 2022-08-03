using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Server.DiscordServer;
using TheOracle2.Data;

namespace Server.Data;

public class PlayerDataFactory
{
    private readonly IDbContextFactory<ApplicationContext> dbFactory;
    private IAssetRepository Assets { get; }
    private IMoveRepository Moves { get; }
    private IOracleRepository Oracles { get; }

    public PlayerDataFactory(IAssetRepository assets, IMoveRepository moves, IOracleRepository oracles, IDbContextFactory<ApplicationContext> dbFactory)
    {
        Assets = assets;
        Moves = moves;
        Oracles = oracles;
        this.dbFactory = dbFactory;
    }

    public IEnumerable<Asset> GetPlayerAssets(ulong PlayerId)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = db.Players.Find(PlayerId)?.Game ?? default;
        var assets = Assets.GetAssetRoots().SelectMany(ar => ar.Assets);
        var playerAssets = assets.Where(a => a.Id.Contains(playerGame.ToString()));
        return playerAssets;
    }

    public IEnumerable<Move> GetPlayerMoves(ulong PlayerId)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = db.Players.Find(PlayerId)?.Game ?? default;
        return Moves.GetMoveRoots().SelectMany(mr => mr.Moves).Where(a => a.Id.Contains(playerGame.ToString()));
    }

    public IEnumerable<Oracle> GetPlayerOracles(ulong PlayerId)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = db.Players.Find(PlayerId)?.Game ?? default;
        return Oracles.GetOracleRoots().SelectMany(or => or.Oracles).Where(a => a.Id.Contains(playerGame.ToString()));
    }
}
