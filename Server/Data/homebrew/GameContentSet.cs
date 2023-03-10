using Server.GameInterfaces;
using TheOracle2.Data;

namespace Server.Data.homebrew;

public class GameContentSet
{
    public string Id { get; set; }
    public ulong CreatorId { get; set; }
    public bool IsPublic { get; set; }
    public string SetName { get; set; }
    public virtual ICollection<Asset> Assets { get; set; }
    public virtual ICollection<Oracle> Oracles { get; set; }
    public virtual ICollection<Move> Moves { get; set; }
}
