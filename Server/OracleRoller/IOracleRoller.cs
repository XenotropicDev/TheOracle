using Server.Data;
// using TheOracle2.Data; // Removed as Oracle is replaced by OracleDTO
using Server.GameInterfaces.DTOs; // Added for DTOs
using System; // For Random
using System.Collections.Generic; // For List
using System.Linq; // For Enumerable.Empty and FirstOrDefault
using TheOracle2; // For OracleRollResult, FollowUpItem, IEmoteRepository (assuming they are here)

namespace Server.OracleRoller;

public interface IOracleRoller
{
    OracleRollResult GetRollResult(OracleDTO oracleDto); // Changed parameter to OracleDTO
}

public class RandomOracleRoller : IOracleRoller
{
    private readonly Random random;
    private readonly IOracleRepository oracleRepo; // Stays as is
    private readonly IEmoteRepository emotes;

    public RandomOracleRoller(Random random, IOracleRepository oracleRepo, IEmoteRepository emotes)
    {
        this.random = random;
        this.oracleRepo = oracleRepo;
        this.emotes = emotes;
    }

    public OracleRollResult GetRollResult(OracleDTO oracleDto) // Changed parameter to OracleDTO
    {
        var results = new OracleRollResult
        {
            OracleDto = oracleDto // Updated to assign to OracleDto
        };

        // Omitted the section dealing with oracle.Usage?.Suggestions?.OracleRolls as per instructions.
        // results.FollowUpTables can still be populated by child results if necessary,
        // or if the logic for suggestions is re-added later using DTOs.

        if (oracleDto.Table != null && oracleDto.Table.Any()) // Use oracleDto.Table
        {
            var roll = random.Next(1, 101);
            
            // Updated logic to find tableItem in List<OracleTableEntryDTO>
            var tableItem = oracleDto.Table.FirstOrDefault(t => 
                t.Floor.HasValue && t.Ceiling.HasValue && 
                roll >= t.Floor.Value && roll <= t.Ceiling.Value);

            if (tableItem != null)
            {
                results.WithTableResult(tableItem, roll); // WithTableResult expects OracleTableEntryDTO
            }
        }

        // Use oracleDto.Oracles (which is List<OracleDTO>)
        foreach (var subOracle in oracleDto.Oracles ?? Enumerable.Empty<OracleDTO>()) 
        {
            results.ChildResults.Add(GetRollResult(subOracle)); // Recursive call with OracleDTO
        }

        return results;
    }
}
