using System.ComponentModel.DataAnnotations.Schema;
using TheOracle2;
using Dataforged;
using Dataforged;

namespace Server.Data.homebrew;

public class GameContentSet
{
    public int Id { get; set; }
    public ulong CreatorId { get; set; }
    public bool IsPublic { get; set; }
    public string SetName { get; set; }
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public virtual ICollection<OracleTable> Oracles { get; set; } = new List<OracleTable>();
    public virtual ICollection<Move> Moves { get; set; } = new List<Move>();
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
