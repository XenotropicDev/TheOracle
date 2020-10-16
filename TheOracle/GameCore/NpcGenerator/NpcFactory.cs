using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.IronSworn;
using TheOracle.StarForged;
using TheOracle.StarForged.NPC;

namespace TheOracle.GameCore.NpcGenerator
{
    public class NpcFactory
    {
        private readonly IServiceProvider serviceProvider;

        public NpcFactory(IServiceProvider Provider)
        {
            this.serviceProvider = Provider;
        }

        public INpcGenerator GetNPCGenerator(GameName game)
        {
            if (game == GameName.Starforged)
                return new StarforgedNPC(serviceProvider);

            return new IronNPC(serviceProvider);
        }
    }
}
