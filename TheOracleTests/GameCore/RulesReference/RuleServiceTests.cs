using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.GameCore.RulesReference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TheOracle.GameCore.RulesReference.Tests
{
    [TestClass()]
    public class RuleServiceTests
    {
        [TestMethod()]
        public void RuleServiceTest()
        {
            var test = new RuleService();

            Assert.IsTrue(test.Rules.Count > 0);
            Assert.IsTrue(test.Rules.Count(r => r.Game == GameName.Starforged) > 0);
            Assert.IsTrue(test.Rules.Count(r => r.Game == GameName.Ironsworn) > 0);
        }
    }
}