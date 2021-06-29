using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheOracle.GameCore.Oracle
{
    public class OracleService
    {
        private List<DataSworn.OracleInfo> OracleInfo { get; set; }

        public OracleService()
        {
            OracleInfo = new List<DataSworn.OracleInfo>();
        }

        //Todo: don't forget to add Ironsowrn and Tarot cards

        public OracleService Load()
        {
            DirectoryInfo starOraclesDir = new DirectoryInfo(Path.Combine("StarForged", "Data", "oracles"));
            if (starOraclesDir.Exists)
            {
                foreach (var file in starOraclesDir.GetFiles("*.json", SearchOption.AllDirectories))
                {
                    var oracleInfoFile = JsonConvert.DeserializeObject<DataSworn.OracleInfo>(File.ReadAllText(file.FullName));

                    OracleInfo.Add(oracleInfoFile);
                }
            }

            foreach (var info in this.OracleInfo)
            {
                foreach (var oracleSet in info.Oracles)
                    try
                    {
                        if (oracleSet.Table?.All(o => o.Chance == 0) ?? false)
                        {
                            for (int i = 0; i < oracleSet.Table.Count; i++)
                            {
                                oracleSet.Table[i].Chance = i + 1;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Error Loading oracle: {oracleSet.Name}");
                        throw;
                    }
            }

            return this;
        }

        public List<DataSworn.OracleInfo> GetUserOracles(long userId = 0)
        {
            return OracleInfo;
        }

        public string RandomOracleResult(object var1 = null, object var2 = null, object var3 = null)
        {
            return "Xeno broke this";
        }

        public object RandomRow(object var1 = null, object var2 = null, object var3 = null)
        {
            return null;
        }
    }
}