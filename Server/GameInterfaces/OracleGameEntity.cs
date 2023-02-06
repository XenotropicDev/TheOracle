using Server.Data;
using Server.OracleRoller;
using TheOracle2;

namespace Server.GameInterfaces;

public class OracleGameEntity
{
    public OracleGameEntity()
    {
        InitialOracles = new();
        FollowUpOracles = new();
    }

    public string Id { get; set; }
    public string DisplayedDescription { get; set; }
    public List<object> ModalInputs { get; set; }
    public string SearchName { get; set; }
    public IronGame Game { get; set; }
    public string Title { get; set; }
    public List<OracleEntityAction> InitialOracles { get; set; }
    public List<OracleEntityAction> FollowUpOracles { get; set; }
    public string? ShortName { get; set; }
    public string? Author { get; set; }
}

public class OracleEntityData
{
    private readonly IOracleRepository oracles;
    private readonly IOracleRoller roller;

    public OracleEntityData(OracleGameEntity entity, IOracleRepository oracleRepository, IOracleRoller oracleRoller, IEmoteRepository emotes)
    {
        Entity = entity;
        oracles = oracleRepository;
        roller = oracleRoller;
        Title = entity.Title;
        Description = entity.DisplayedDescription;

        InitialOracleData = new();
        FollowupOracleData = new();

        foreach (var o in entity.InitialOracles)
        {
            var oracle = oracles.GetOracleById(o.FieldValue);
            if (oracle == null)
            {
                InitialOracleData.Add(o.ShallowCopy());
                continue;
            }

            var result = roller.GetRollResult(oracle);
            this.AddOracleResult(result, o);

            foreach (var followupItem in result.FollowUpTables)
            {
                FollowupOracleData.Add(followupItem);
            }
        }

        foreach (var f in entity.FollowUpOracles)
        {
            FollowupOracleData.Add(new FollowUpItemAdapter(f, emotes));
        }
    }

    private void AddOracleResult(OracleRollResult result, OracleEntityAction o)
    {
        InitialOracleData.Add(new() {FieldName = o.FieldName, FieldValue = result.Description ?? "Unknown oracle result value"});
        foreach(var childResult in result.ChildResults)
        {
            InitialOracleData.Add(new() {FieldName = childResult.Oracle?.Name ?? o.FieldName, FieldValue = childResult.Description! });

            FollowupOracleData.AddRange(childResult.FollowUpTables);
        }
    }

    public OracleGameEntity Entity { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<OracleEntityAction> InitialOracleData { get; set; }
    public List<FollowUpItem> FollowupOracleData { get; set; }
}

internal class FollowUpItemAdapter : FollowUpItem
{
    private OracleEntityAction entityAction;

    public FollowUpItemAdapter(OracleEntityAction FollowupEntityItem, IEmoteRepository emotes) : base(FollowupEntityItem.FieldValue, FollowupEntityItem.FieldName, emotes)
    {
        this.entityAction = FollowupEntityItem;
    }
}

public class OracleEntityAction
{
    public OracleEntityAction()
    {
        ModalSettings = new ModalOverrideSettings();
        FieldName = String.Empty;
        FieldValue = String.Empty;
    }

    public OracleEntityAction ShallowCopy()
    {
        var clone = (OracleEntityAction)this.MemberwiseClone();
        clone.ModalSettings = null;

        return clone;
    }

    public string FieldName { get; set; }
    public string FieldValue { get; set; }
    public bool HasModalOverride { get; set; }
    public ModalOverrideSettings? ModalSettings { get; set; }
    public bool AllowReroll { get; set; }
    public bool AllowFudge { get; set; }
    public string? Emoji { get; set; }
}

public class ModalOverrideSettings
{
    public ModalOverrideSettings()
    {
        TextInputStyle = TextInputStyle.Short;
        PlaceholderText = String.Empty;
    }

    public TextInputStyle TextInputStyle { get; internal set; }
    public string PlaceholderText { get; internal set; }
    public int? MaxLength { get; internal set; }
    public int? MinLength { get; internal set; }
}
