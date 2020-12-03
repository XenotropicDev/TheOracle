using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.StarForged.NPC;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TheOracle.GameCore.Oracle;
using System.Linq;
using TheOracle.BotCore;

namespace TheOracle.StarForged.NPC.Tests
{
    [TestClass()]
    public class StarforgedNPCTests
    {
        [TestMethod()]
        public void NPCBuildTest()
        {
            var services = new ServiceCollection()
                .AddSingleton(new OracleService().Load())
                .AddSingleton<HookedEvents>()
                .AddSingleton<ReactionService>()
                .BuildServiceProvider();
            var NPC = new StarforgedNPC(services);

            for (int i = 0; i < 100; i++)
            {
                NPC.Build("");

                if (NPC.Goals.Any(g => g.Contains("[2x]"))) 
                    Assert.Fail();
            }
        }
    }
}