using Server.Data;
using TheOracle2;
using TheOracle2.Data;

namespace Server.OracleRoller;

public interface IOracleRoller
{
    OracleRollResult GetRollResult(Oracle oracle);
}

public class RandomOracleRoller : IOracleRoller
{
    private readonly Random random;
    private readonly IOracleRepository oracleRepo;
    private readonly IEmoteRepository emotes;

    public RandomOracleRoller(Random random, IOracleRepository oracleRepo, IEmoteRepository emotes)
    {
        this.random = random;
        this.oracleRepo = oracleRepo;
        this.emotes = emotes;
    }

    public OracleRollResult GetRollResult(Oracle oracle)
    {
        var results = new OracleRollResult
        {
            Oracle = oracle
        };

        foreach (var followUpId in oracle.Usage?.Suggestions?.OracleRolls ?? new())
        {
            var followUpOracle = oracleRepo.GetOracleById(followUpId.Oracle);
            if (followUpOracle?.Oracles?.Count > 0)
            {
                foreach (var subTable in followUpOracle.Oracles)
                {
                    results.FollowUpTables.Add(new FollowUpItem(subTable.Id, subTable.Name, emotes));
                }
            }
            else if (followUpOracle != null)
            {
                results.FollowUpTables.Add(new FollowUpItem(followUpId.Oracle, followUpOracle.Name, emotes));
            }
        }

        if (oracle.Table?.Count > 0)
        {
            var roll = random.Next(1, 101);
            var tableItem = oracle.Table.FirstOrDefault(t => t.CompareTo(roll) == 0);

            if (tableItem != null)
            {
                results.WithTableResult(tableItem, roll);
            }
        }

        foreach (var subOracle in oracle.Oracles ?? new())
        {
            results.ChildResults.Add(GetRollResult(subOracle));
        }

        return results;
    }
}
