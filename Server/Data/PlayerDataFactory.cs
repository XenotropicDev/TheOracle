using Dataforged;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Server.DiscordServer;
using Server.GameInterfaces;
using TheOracle2;
using Dataforged;

namespace Server.Data;

public class PlayerDataFactory
{
    private readonly ApplicationContext db;
    private IMoveRepository Moves { get; }
    public IEntityRepository Entities { get; }

    public PlayerDataFactory(IMoveRepository moves, IEntityRepository entities, ApplicationContext dbFactory)
    {
        Moves = moves;
        Entities = entities;
        db = dbFactory;
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

    public async Task<IEnumerable<OracleTable>> GetPlayerOracles(ulong PlayerId)
    {
        var player = await db.Players.FindAsync(PlayerId);
        return player.GameDataSets.SelectMany(gds => gds.Oracles);
    }
}
