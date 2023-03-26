using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Discord.Interactions;
using Microsoft.VisualBasic.FileIO;

namespace TheOracle2.Data.Tests;

[TestClass()]
public class OracleCategoryTests
{
    [TestMethod()]
    [DataRow(typeof(List<OracleRoot>), "*oracle*.json")]
    [DataRow(typeof(List<AssetRoot>), "*assets.json")]
    [DataRow(typeof(List<MoveRoot>), "*moves*.json")]
    public void LoadAndGenerateTest(Type T, string searchOption)
    {
        var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
        var files = baseDir.GetFiles(searchOption);

        Assert.IsTrue(files.Length >= 1, $"No files found in {baseDir} for {searchOption}");

        foreach (var file in files)
        {
            string text = file.OpenText().ReadToEnd();

            var jsonSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

            var root = JsonConvert.DeserializeObject(text, T, jsonSettings);

            Assert.IsNotNull(root);

            switch (root)
            {
                case List<AssetRoot> assetList:
                    Console.WriteLine($"there are {assetList.Sum(i => i.Assets.Count)} assets in {file.Name}");
                    //Assert.IsTrue(assetList.Any(ar => ar.Assets.Any(a => a.Abilities.Any(ab => ab.Id != null))));
                    break;
                case List<MoveRoot> m:
                    Console.WriteLine($"there are {m.Sum(i => i.Moves.Count)} moves in {file.Name}");
                    break;
                case List<OracleRoot> o:
                    Console.WriteLine($"there are {o.Sum(i => i.Oracles.Count)} in {file.Name}");
                    break;
                default:
                    break;
            }
        }
    }

    [TestMethod()]
    public void AbilityTest()
    {
        var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
        var file = baseDir.GetFiles("AssAbility.json").FirstOrDefault();

        Assert.IsNotNull(file);

        string text = file!.OpenText().ReadToEnd();

        var jsonSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, MetadataPropertyHandling = MetadataPropertyHandling.Ignore };

        var ability = JsonConvert.DeserializeObject<Ability>(text, jsonSettings);
        Assert.IsNotNull(ability);
        var reSerialized = JsonConvert.SerializeObject(ability);

        Assert.AreEqual("Ironsworn/Assets/Companion/Cave_Lion/Abilities/1", ability.Id);
    }

    [TestMethod()]
    public void SerializeOracleTest()
    {
        var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
        var files = baseDir.GetFiles("*oracle*.json");

        Assert.IsTrue(files.Length >= 1, $"No files found in {baseDir} for *oracle*.json");

        foreach (var file in files)
        {
            string text = file.OpenText().ReadToEnd();

            var jsonSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

            var root = JsonConvert.DeserializeObject<List<OracleRoot>>(text, jsonSettings);

            Assert.IsNotNull(root);

            var oracle = root.FirstOrDefault().Oracles.FirstOrDefault();
            oracle.Usage.Suggestions.OracleRolls.Add(new("Suggestion for Oracle Rolls"));

            var jsonSer = JsonConvert.SerializeObject(oracle.Usage);
            Assert.IsNotNull(jsonSer);
            
        }
    }
}
