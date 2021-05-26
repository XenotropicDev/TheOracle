using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;
using TheOracle.GameCore.Oracle;

namespace TheOracle.GameCore.Oracle
{
    public partial class StandardOracle : IOracleEntry
    {
        public StandardOracle()
        {

        }

        public int Chance { get; set; }
        public string Description { get; set; }
        public string Prompt { get; set; }
        public string QuestStarter { get; private set; }
        public List<StandardOracle> Oracles { get; set; }
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets the result of a oracle roll, and any rolls that would result from it.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public string GetOracleResult(IServiceProvider services, GameName game, Random rnd = null, string[] additionalSearchTerms = null)
        {
            var roller = new OracleRoller(services, game, rnd);
            var tables = roller.ParseOracleTables(Description);

            if (tables.Count == 0) return Description;

            roller.BuildRollResults(Description, additionalSearchTerms);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Description);

            return $"{Description}\n" + String.Join(" / ", finalResults);
        }

        internal StandardOracle DeepClone()
        {
            var clone = (StandardOracle)this.MemberwiseClone();
            if (Oracles != null) clone.Oracles = new List<StandardOracle>(this.Oracles.Select(o => o.DeepClone()));

            return clone;
        }

        //TODO move this to an extension method of IOracleEntry?
        /// <summary>
        /// Gets the result of a oracle roll, and any rolls that would result from it.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public string GetOracleResultPrompt(IServiceProvider services, GameName game, Random rnd = null)
        {
            var roller = new OracleRoller(services, game, rnd);
            var tables = roller.ParseOracleTables(Prompt);

            if (tables.Count == 0) return Prompt;

            roller.BuildRollResults(Prompt);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Prompt);

            return $"{Prompt}\n" + String.Join(" / ", finalResults);
        }
    }
}