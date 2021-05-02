using System;
using TheOracle.IronSworn.Settlements;
using TheOracle.StarForged.Settlements;

namespace TheOracle.GameCore.SettlementGenerator
{
    internal class SettlementFactory
    {
        private readonly IServiceProvider services;
        private readonly ulong channelId;

        public SettlementFactory(IServiceProvider service, ulong channelId)
        {
            this.services = service;
            this.channelId = channelId;
        }

        internal ISettlement GetGenerator(GameName game, string options)
        {
            if (game == GameName.Starforged) return new StarSettlement(services, channelId);
            return new IronSettlement(services, channelId);
        }
    }
}