using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;

namespace TheOracle
{
    public interface IOracleEntry
    {
        int Chance { get; set; }
        string Description { get; set; }
    }
    public enum OracleType 
    {
        standard,
        nested,
        multipleColumns,
        paired
    }

    public static class OracleExtensions
    {
        /// <summary>
        /// Gets the result of a oracle roll, and any rolls that would result from it.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static string GetOracleResult(this IOracleEntry oracle, ServiceProvider services, GameName game, Random rnd = null)
        {
            var oracleService = services.GetRequiredService<OracleService>();

            var roller = new OracleRoller(oracleService, game, rnd);
            var tables = roller.ParseOracleTables(oracle.Description);

            if (tables.Count == 0) return oracle.Description;

            roller.BuildRollResults(oracle.Description);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Description);

            return $"{oracle.Description}\n" + String.Join(" / ", finalResults);
        }
    }
}