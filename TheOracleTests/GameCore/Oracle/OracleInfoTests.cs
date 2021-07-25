using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle.GameCore.Oracle.DataSworn;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;

namespace TheOracle.GameCore.Oracle.DataSworn.Tests
{
    [TestClass()]
    public class OracleInfoTests
    {
        [TestMethod()]
        public void TestDesializationTest()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;

            foreach (var file in new System.IO.DirectoryInfo("StarForged\\Data\\oracles").GetFiles("*.json", System.IO.SearchOption.AllDirectories))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(file.FullName);

                    var test = JsonConvert.DeserializeObject<OracleInfo>(json, settings);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"{file.FullName} {ex.Message}");
                    Console.WriteLine($"{file.Name} {ex.Message}");
                }
            }

            //foreach (var file in new System.IO.DirectoryInfo("IronSworn\\Data").GetFiles("ironsworn_oracles*.json"))
            //{
            //    try
            //    {
            //        string json = System.IO.File.ReadAllText(file.FullName);

            //        var test = JsonConvert.DeserializeObject<OracleInfo>(json, settings);
            //    }
            //    catch (Exception ex)
            //    {
            //        Assert.Fail($"{file.Name} {ex.Message}");
            //        Console.WriteLine($"{file.Name} {ex.Message}");
            //    }
            //}

            foreach (var file in new System.IO.DirectoryInfo("StarForged\\Data\\").GetFiles("move*.json", System.IO.SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(file.FullName);

                    var test = JsonConvert.DeserializeObject<MoveInfo>(json, settings);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"{file.FullName} {ex.Message}");
                    Console.WriteLine($"{file.Name} {ex.Message}");
                }
            }

            foreach (var file in new System.IO.DirectoryInfo("StarForged\\Data\\glossary").GetFiles("*.json", System.IO.SearchOption.AllDirectories))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(file.FullName);

                    var test = JsonConvert.DeserializeObject<GlossaryInfo>(json, settings);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"{file.Name} {ex.Message}");
                    Console.WriteLine($"{file.Name} {ex.Message}");
                }
            }

            foreach (var file in new System.IO.DirectoryInfo("StarForged\\Data").GetFiles("asset*.json", System.IO.SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(file.FullName);

                    var test = JsonConvert.DeserializeObject<AssetInfo>(json, settings);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"{file.Name} {ex.Message}");
                    Console.WriteLine($"{file.Name} {ex.Message}");
                }
            }

            var generator = new JSchemaGenerator();

            System.IO.File.WriteAllText("OraclesSchema.json", generator.Generate(typeof(OracleInfo)).ToString());
            System.IO.File.WriteAllText("MovesSchema.json", generator.Generate(typeof(MoveInfo)).ToString());
            System.IO.File.WriteAllText("GlossarySchema.json", generator.Generate(typeof(GlossaryInfo)).ToString());
            System.IO.File.WriteAllText("AssetsSchema.json", generator.Generate(typeof(AssetInfo)).ToString());
        }
    }
}