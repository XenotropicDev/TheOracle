using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.IronSworn.Delve;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TheOracle.GameCore.ProgressTracker;
using System.Linq;

namespace TheOracle.IronSworn.Delve.Tests
{
    [TestClass()]
    public class DelveInfoTests
    {
        [TestMethod()]
        public void FromInputTest()
        {
            var delveThemePath = Path.Combine("IronSworn", "themes.json");
            var delveDomainPath = Path.Combine("IronSworn", "domains.json");
            var delveService = DelveService.Load(new string[] { delveThemePath }, new string[] { delveDomainPath });

            var craftedDelveSite = DelveInfo.FromInput(delveService, "Corrupted, Fortified", "Cavern", "Demo Site", "Test Delve", "Epic");

            Assert.AreEqual(ChallengeRank.Epic, craftedDelveSite.Rank);
            Assert.IsTrue(craftedDelveSite.Themes.Any(t => t.DelveSiteTheme == "Corrupted"));
            Assert.IsTrue(craftedDelveSite.Themes.Any(t => t.DelveSiteTheme == "Fortified"));

            var randomDelveSite = DelveInfo.FromInput(delveService, "Random", "Random, Random", "Demo Site", "Test Delve", "Troublesome");
            Assert.AreEqual(ChallengeRank.Troublesome, randomDelveSite.Rank);
            Assert.AreEqual(1, randomDelveSite.Themes.Count());
            Assert.AreEqual(2, randomDelveSite.Domains.Count());
            Assert.AreNotEqual(randomDelveSite.Domains[0], randomDelveSite.Domains[1]);
        }

        [TestMethod()]
        public void RevealFeatureRollerTest()
        {
            var delveThemePath = Path.Combine("IronSworn", "themes.json");
            var delveDomainPath = Path.Combine("IronSworn", "domains.json");
            var delveService = DelveService.Load(new string[] { delveThemePath }, new string[] { delveDomainPath });

            var delveSite = DelveInfo.FromInput(delveService, "Corrupted, Fortified", "Cavern", "Demo Site", "Test Delve", "Epic");

            var featureRoller = delveSite.RevealFeatureRoller();
            Assert.IsTrue(featureRoller.RollResultList.Count > 0);
        }
    }
}