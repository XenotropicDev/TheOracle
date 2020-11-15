using Discord;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAsset
    {
        Embed GetEmbed(string[] arguments);
    }
}