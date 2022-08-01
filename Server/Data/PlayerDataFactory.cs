using Server.DiscordServer;
using TheOracle2.Data;

namespace Server.Data;

public class PlayerDataFactory
{
    private readonly ApplicationContext db;
    private IAssetRepository Assets { get; }
    private IMoveRepository Moves { get; }
    private IOracleRepository Oracles { get; }

    public PlayerDataFactory(IAssetRepository assets, IMoveRepository moves, IOracleRepository oracles, ApplicationContext db)
    {
        Assets = assets;
        Moves = moves;
        Oracles = oracles;
        this.db = db;
    }

    public IEnumerable<Asset> GetPlayerAssets(ulong PlayerId)
    {
        var playerGame = db.Players.Find(PlayerId)?.Game ?? default;
        return Assets.GetAssetRoots().SelectMany(ar => ar.Assets).Where(a => a.Id.Contains(playerGame.ToString()));
    }

    public IEnumerable<Move> GetPlayerMoves(ulong PlayerId)
    {
        var playerGame = db.Players.Find(PlayerId)?.Game ?? default;
        return Moves.GetMoveRoots().SelectMany(mr => mr.Moves).Where(a => a.Id.Contains(playerGame.ToString()));
    }

    public IEnumerable<Oracle> GetPlayerOracles(ulong PlayerId)
    {
        var playerGame = db.Players.Find(PlayerId)?.Game ?? default;
        return Oracles.GetOracleRoots().SelectMany(or => or.Oracles).Where(a => a.Id.Contains(playerGame.ToString()));
    }
}
