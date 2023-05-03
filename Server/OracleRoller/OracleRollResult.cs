using TheOracle2.Data;

namespace TheOracle2;

public class OracleRollResult
{
    public int? Roll { get; internal set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public Oracle? Oracle { get; set; }
    public string ResultId { get; set; }

    public List<OracleRollResult> ChildResults { get; set; } = new();
    public List<FollowUpItem> FollowUpTables { get; internal set; } = new();

    public OracleRollResult WithTableResult(Table table, int roll)
    {
        Roll = roll;
        Description = table.Result;
        ResultId = table.JsonId;
        return this;
    }
}

public class FollowUpItem : SelectMenuOptionBuilder
{
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
