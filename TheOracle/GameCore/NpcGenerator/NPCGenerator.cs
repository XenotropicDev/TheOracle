using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.GameCore.NpcGenerator
{
    public abstract class NPCGenerator
    {
        public virtual Embed GetEmbed()
        {
            return new EmbedBuilder().Build();
        }
    }
}
