using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public interface IOracleEntry
    {
        int Chance { get; set; }
        string Description { get; set; }
        string Prompt { get; set; }
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
        public static string GetOracleResult(this IOracleEntry oracle, IServiceProvider services, GameName game, Random rnd = null)
        {
            var roller = new OracleRoller(services, game, rnd);
            var tables = roller.ParseOracleTables(oracle.Description, StrictParsing: true);

            if (tables.Count == 0) return oracle.Description;

            roller.BuildRollResults(oracle.Description);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Description);

            return $"{oracle.Description}\n" + String.Join(" / ", finalResults);
        }
    }
}