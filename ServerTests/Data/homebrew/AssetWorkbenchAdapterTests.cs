using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.Data.AssetWorkbench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Dataforged;

namespace TheOracle2.Data.AssetWorkbench.Tests
{
    [TestClass()]
    public class AssetWorkbenchAdapterTests
    {
        [TestMethod()]
        public void AssetWorkbenchAdapterTest()
        {
            var fileJson = File.ReadAllText(Path.Combine("Data", "WorkbenchAssetSample.json"));

            Asset dsAsset = new AssetWorkbenchAdapter(fileJson);

            Assert.IsNotNull(dsAsset);
            Assert.AreEqual("Lightbearer", dsAsset.Name);
        }
    }
}
