using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Server.DiscordServer;
using Server.GameInterfaces;
using Server.GameInterfaces.DTOs; // Already present, good.
using TheOracle2; // For IronGame
// using TheOracle2.Data; // To be removed if OracleGameEntity is not from here
using Server.GameInterfaces; // For OracleGameEntity (assuming it's here)
using System.Linq; // For LINQ methods like Where, SelectMany, etc.
using System.Collections.Generic; // For List<T>

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

    public async Task<IEnumerable<AssetDTO>> GetPlayerAssets(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        var assets = Assets.GetAssets(); 

        return assets.Where(a => a.Id.Contains(playerGame.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<OracleGameEntity>> GetPlayerEntites(ulong PlayerId, IronGame? gameOverride = null)
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        
        return Entities.GetEntities().Where(a => a.Game == playerGame);
    }

    public async Task<IEnumerable<MoveDTO>> GetPlayerMoves(ulong PlayerId, IronGame? gameOverride = null) // Changed return type
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        var moves = Moves.GetMoves(); // Uses DTO-returning method
        
        // Assuming MoveDTO.Id contains the game identifier similar to the original Move entity.
        return moves.Where(a => a.Id.Contains(playerGame.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<OracleDTO>> GetPlayerOracles(ulong PlayerId, IronGame? gameOverride = null) // Changed return type
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        string gameName = playerGame.ToString();

        var gameOracles = new List<OracleDTO>();

        // Get all oracles from roots that match the game
        var oracleRoots = Oracles.GetOracleRoots()
                                 .Where(or => or.Id.Contains(gameName, StringComparison.OrdinalIgnoreCase) || 
                                              or.Source?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true);
        gameOracles.AddRange(oracleRoots.SelectMany(or => or.Oracles));

        // Get all oracles from categories that match the game
        var oracleCategories = Oracles.GetOracleCategories()
                                      .Where(oc => oc.Id.Contains(gameName, StringComparison.OrdinalIgnoreCase) || 
                                                   oc.Source?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true);
        gameOracles.AddRange(oracleCategories.SelectMany(oc => oc.Oracles));
        
        // The above might miss oracles directly in roots if the root ID doesn't match game, but its oracles do.
        // A more comprehensive approach matching the original logic:
        // 1. Get all OracleDTOs
        // 2. Filter them by ID or Source (if applicable for DTOs)
        
        // Simpler approach: Get all DTOs and filter.
        // This relies on IOracleRepository.GetOracles() returning all oracles (top-level and nested within categories)
        // and OracleDTO.Id or OracleDTO.Source.Name being filterable by gameName.
        // The current IOracleRepository.GetOracles() returns only top-level oracles from roots.
        // The original logic iterated through roots, then categories.

        // Replicating original logic structure with DTOs:
        var allMatchingOracles = new List<OracleDTO>();
        var allOracleRoots = Oracles.GetOracleRoots(); // Get all roots

        foreach (var root in allOracleRoots)
        {
            // Oracles directly under a root
            if (root.Id.Contains(gameName, StringComparison.OrdinalIgnoreCase) || root.Source?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true)
            {
                 allMatchingOracles.AddRange(root.Oracles);
            } else { // If root itself doesn't match, check its direct oracles
                 allMatchingOracles.AddRange(root.Oracles.Where(o => o.Id.Contains(gameName, StringComparison.OrdinalIgnoreCase) || o.Source?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true));
            }

            // Oracles under categories of that root
            foreach (var category in root.Categories)
            {
                if (category.Id.Contains(gameName, StringComparison.OrdinalIgnoreCase) || category.Source?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    allMatchingOracles.AddRange(category.Oracles);
                } else { // If category itself doesn't match, check its oracles
                    allMatchingOracles.AddRange(category.Oracles.Where(o => o.Id.Contains(gameName, StringComparison.OrdinalIgnoreCase) || o.Source?.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase) == true));
                }
            }
        }
        
        // Remove duplicates by Id before returning
        return allMatchingOracles.DistinctBy(o => o.Id).ToList();
    }

    public async Task<IEnumerable<OracleRootDTO>> GetPlayerOraclesRoots(ulong PlayerId, IronGame? gameOverride = null) // Changed return type
    {
        using var db = dbFactory.CreateDbContext();
        var playerGame = gameOverride ?? (await db.Players.FindAsync(PlayerId))?.Game ?? default;
        var oracleRoots = Oracles.GetOracleRoots(); // Uses DTO-returning method
        
        // Assuming OracleRootDTO.Id or OracleRootDTO.Source.Name contains the game identifier
        return oracleRoots.Where(or => or.Id.Contains(playerGame.ToString(), StringComparison.OrdinalIgnoreCase) || 
                                       or.Source?.Name.Contains(playerGame.ToString(), StringComparison.OrdinalIgnoreCase) == true);
    }
}
