using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.GameCore.Action;

namespace TheOracle.Core.Tests
{
    [TestClass()]
    public class ActionRollTests
    {
        [TestMethod()]
        public void ActionRollTest1()
        {
            var roll = new ActionRoll(-10) {ActionDie = 6 };
            Assert.AreEqual(-4, roll.ActionScore);
            Assert.IsTrue(roll.ToString() != string.Empty);
        }

        [TestMethod()]
        public void ActionRollTest2()
        {
            var complicationRoll = new ActionRoll { ActionDie = 6, ChallengeDie1 = 10, ChallengeDie2 = 10 };
            var opportunityRoll = new ActionRoll { ActionDie = 6, ChallengeDie1 = 5, ChallengeDie2 = 5 };
            var strongHit = new ActionRoll { ActionDie = 6, ChallengeDie1 = 5, ChallengeDie2 = 4 };
            var weakHit = new ActionRoll { ActionDie = 5, ChallengeDie1 = 2, ChallengeDie2 = 5 };
            var miss = new ActionRoll { ActionDie = 3, ChallengeDie1 = 3, ChallengeDie2 = 4 };

            Assert.AreEqual(ActionResources.Complication, complicationRoll.ResultText());
            Assert.AreEqual(ActionResources.Opportunity, opportunityRoll.ResultText());
            Assert.AreEqual(ActionResources.Strong_Hit, strongHit.ResultText());
            Assert.AreEqual(ActionResources.Weak_Hit, weakHit.ResultText());
            Assert.AreEqual(ActionResources.Miss, miss.ResultText());
        }
    }
}