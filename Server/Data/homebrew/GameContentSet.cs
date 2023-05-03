using System.ComponentModel.DataAnnotations.Schema;
using TheOracle2.Data;

namespace Server.Data.homebrew;

public class GameContentSet
{
    public int Id { get; set; }
    public ulong CreatorId { get; set; }
    public bool IsPublic { get; set; }
    public string SetName { get; set; }
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public virtual ICollection<Oracle> Oracles { get; set; } = new List<Oracle>();
    public virtual ICollection<Move> Moves { get; set; } = new List<Move>();
}
