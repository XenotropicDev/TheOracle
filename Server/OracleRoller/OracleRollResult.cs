using TheOracle2.Data; // This might be removable if IEmoteRepository is not from here
using Server.GameInterfaces.DTOs; // Added for DTOs
using Discord; // For SelectMenuOptionBuilder and IEmote
using System.Collections.Generic; // For List

namespace TheOracle2; // Assuming this namespace is correct

public class OracleRollResult
{
    public OracleRollResult()
    {
        ChildResults = new List<OracleRollResult>();
        FollowUpTables = new List<FollowUpItem>();
    }

    public int? Roll { get; internal set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public OracleDTO? OracleDto { get; set; } // Changed type to OracleDTO and renamed
    public string ResultId { get; set; }

    public List<OracleRollResult> ChildResults { get; set; }
    public List<FollowUpItem> FollowUpTables { get; internal set; }

    // Changed signature to use OracleTableEntryDTO
    public OracleRollResult WithTableResult(OracleTableEntryDTO tableDto, int roll) 
    {
        Roll = roll;
        Description = tableDto.ResultText; // Updated to use DTO property
        ResultId = tableDto.Id;            // Updated to use DTO property
        return this;
    }
}

public class FollowUpItem : SelectMenuOptionBuilder
{
    // Assuming IEmoteRepository is a custom interface/class. If it's from TheOracle2.Data, the using statement is needed.
    public FollowUpItem(string id, string name, IEmoteRepository emotes) 
    {
        Id = id;
        Name = name;

        Description = name;
        Value = id;
        Emote = emotes.Roll;
    }

    public string Id { get; set; }
    public string Name { get; set; }

    
}
