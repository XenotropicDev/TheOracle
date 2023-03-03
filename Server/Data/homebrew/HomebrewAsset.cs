using Server.DiscordServer;
using TheOracle2;
using TheOracle2.Data;

namespace Server.GameInterfaces;

public record HomebrewAsset
{
    public HomebrewAsset()
    {
    }

    public HomebrewAsset(Asset asset, ulong id)
    {
        Asset = asset;
        CreatorDiscordId = id;
    }

    public int Id { get; set; }
    public Asset Asset { get; }
    public ulong CreatorDiscordId { get; set; }
    public bool IsPublic { get; set; }
    public List<Player> Subscribers { get; set; }
}
