using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.IronSworn;
using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.Core;
using Microsoft.Extensions.DependencyInjection;

namespace TheOracle.IronSworn.Tests
{
    [TestClass()]
    public class StandardOracleTests
    {
        [TestMethod()]
        public void GetOracleResultMultiTest()
        {
            var so = new StandardOracle();
            var services = new ServiceCollection().AddSingleton<OracleService>().BuildServiceProvider();

            so.Description = "[Descriptor/Focus]";
            string result = so.GetOracleResult(services, GameName.Starforged);

            Assert.AreNotEqual(so.Description, result);
        }

        [TestMethod()]
        public void GetOracleResultSingleTest()
        {
            var so = new StandardOracle();
            var services = new ServiceCollection().AddSingleton<OracleService>().BuildServiceProvider();

            so.Description = "Some value / Or something else";
            string result = so.GetOracleResult(services, GameName.Starforged);

            Assert.AreEqual(so.Description, result);
        }
    }
}