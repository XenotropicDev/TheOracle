using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.StarForged.Creatures;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Creatures.Tests
{
    [TestClass()]
    public class CreatureTests
    {
        [TestMethod()]
        public void GenerateNewCreatureTest()
        {
            var services = new ServiceCollection().AddSingleton(new OracleService().Load()).BuildServiceProvider();

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    var creature = Creature.GenerateNewCreature(services, 0, CreatureEnvironment.Space);

                    creature.GetEmbedBuilder();
                    Assert.IsTrue(!creature.BasicForm.Contains("2x"));
                    Assert.IsTrue(!creature.BasicForm.Contains("Roll twice"));
                }
                catch (Exception)
                {
                    Console.WriteLine(i);
                    throw;
                }
            }
        }
    }
}