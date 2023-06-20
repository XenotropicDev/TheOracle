using Dataforged;

namespace TheOracle2;

public class OracleRollResult
{
    public int? Roll { get; internal set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public OracleTable? Oracle { get; set; }
    public string ResultId { get; set; }

    public List<OracleRollResult> ChildResults { get; set; } = new();
    public List<FollowUpItem> FollowUpTables { get; internal set; } = new();

    public OracleRollResult WithTableResult(OracleTableRow table, int roll)
    {
        Roll = roll;
        Description = table.Result;
        ResultId = table.JsonId;
        return this;
    }
}

public class FollowUpItem : SelectMenuOptionBuilder
{
    public FollowUpItem(uint id, string name, IEmoteRepository emotes)
    {
        Id = id;
        Name = name;

        Description = name;
        Value = id.ToString();
        Emote = emotes.Roll;
    }

    public FollowUpItem(string id, string name, IEmoteRepository emotes)
    {
        if (!uint.TryParse(id, out var uintId)) throw new ArgumentException($"Unknown follow up item {id}");

        Id = uintId;
        Name = name;

        Description = name;
        Value = id;
        Emote = emotes.Roll;
    }

    public uint Id { get; set; }
    public string Name { get; set; }
}
