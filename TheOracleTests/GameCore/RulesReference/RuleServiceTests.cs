using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.GameCore.RulesReference;
using System;
using System.Collections.Generic;
using System.Text;

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
        }
    }
}