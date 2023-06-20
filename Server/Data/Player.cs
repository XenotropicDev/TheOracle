using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Server.Data.homebrew;
using Server.DiscordServer;
using Dataforged;

namespace TheOracle2;

public record Player
{
    [Key]
    public ulong DiscordId { get; set; }
    public IronGame Game { get; set; }
    public bool SubsPerServer { get; set; }
    public virtual List<GameContentSet> GameDataSets { get; set; }

    /// <summary>
    /// Creates a player if none exists on the database with the default content set
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public static async Task<Player> CreateDefault(ApplicationContext db, ulong playerId)
    {
        var player = new Player
        {
            Game = IronGame.Ironsworn,
            SubsPerServer = false,
            GameDataSets = new List<GameContentSet>() { db.GameContentSets.FirstOrDefault() },
            DiscordId = playerId
        };

        await db.Players.AddAsync(player);
        await db.SaveChangesAsync();

        return player;
    }

}
