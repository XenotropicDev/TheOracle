namespace TheOracle2;

public interface IEmoteRepository
{
    IEmote Adventure { get; }
    IEmote AidYourAlly { get; }
    IEmote AskTheOracle { get; }
    IEmote CheckYourGear { get; }
    IEmote Combat { get; }
    IEmote CompanionTakesAHit { get; }
    IEmote Compel { get; }
    IEmote Connection { get; }
    IEmote EndureHarm { get; }
    IEmote EndureStress { get; }
    IEmote Exploration { get; }
    IEmote FaceDeath { get; }
    IEmote FaceDesolation { get; }
    IEmote Fate { get; }
    IEmote GatherInformation { get; }
    IEmote Legacy { get; }
    IEmote LoseMomentum { get; }
    IEmote MarkProgress { get; }
    IEmote DecreaseProgress { get; }
    IEmote OvercomeDestruction { get; }
    IEmote[] ProgressEmotes { get; }
    IEmote Quest { get; }
    IEmote Recommit { get; }
    IEmote Recovery { get; }
    IEmote Reference { get; }
    IEmote Roll { get; }
    IEmote SacrificeResources { get; }
    IEmote Suffer { get; }
    IEmote Threshold { get; }
    IEmote WithstandDamage { get; }
    IEmote BurnMomentum { get; }

    /// <summary>
    /// A dictionary of all the emoji's. The underlying dictionary should be implemented with StringComparer.OrdinalIgnoreCase comparer.
    /// </summary>
    /// <returns></returns>
    Dictionary<string, IEmote> AsDictionary();
}

public class HardCodedEmoteRepo : IEmoteRepository
{
    internal static readonly Dictionary<string, IEmote> EmojiLibrary = new()
    {
        { "roll", new Emoji("🎲") },
        { "recommit", new Emoji("🔄") },
        { "reference", new Emoji("📖") },
        { "progress", Emote.Parse("<:progress4:880599822820864060>") }
    };

    private Dictionary<string, IEmote> dictionary;

    public HardCodedEmoteRepo()
    {
        Adventure = new Emoji("🌐");
        AidYourAlly = new Emoji("👥");
        AskTheOracle = new Emoji("🔮");
        BurnMomentum = new Emoji("🔥");
        CheckYourGear = new Emoji("🎒");
        Combat = new Emoji("⚔️");
        CompanionTakesAHit = new Emoji("🩸");
        Compel = new Emoji("⚖️");
        Connection = new Emoji("🪢");
        EndureHarm = new Emoji("🩸");
        EndureStress = new Emoji("💔");
        Exploration = new Emoji("🧭");
        FaceDeath = new Emoji("💀");
        FaceDesolation = new Emoji("🖤");
        Fate = new Emoji("🔮");
        GatherInformation = new Emoji("🧩");
        Legacy = new Emoji("🔖");
        LoseMomentum = new Emoji("⌛️");
        OvercomeDestruction = new Emoji("💥");
        Quest = new Emoji("✴️");
        Recommit = new Emoji("🔄");
        Recovery = new Emoji("❤️‍🩹 ");
        Reference = new Emoji("📖");
        Roll = new Emoji("🎲");
        SacrificeResources = new Emoji("💸");
        Suffer = new Emoji("🩸");
        Threshold = new Emoji("🚪");
        WithstandDamage = new Emoji("⚙️");
        DecreaseProgress = new Emoji("⏮️");

        MarkProgress = Emote.TryParse("<:progress4:880599822820864060>", out var progress) ? progress : new Emoji("☑️");

        ProgressEmotes = new List<IEmote>
        {
            Emote.TryParse("<:progress0:880599822468534374>", out var progress0) ? progress0 : new Emoji("🟦"),
            Emote.TryParse("<:progress1:880599822736965702>", out var progress1) ? progress1 : new Emoji("🇮"),
            Emote.TryParse("<:progress2:880599822724390922>", out var progress2) ? progress2 : new Emoji("🇽"),
            Emote.TryParse("<:progress3:880599822736957470>", out var progress3) ? progress3 : new Emoji("*️⃣"),
            Emote.TryParse("<:progress4:880599822820864060>", out var progress4) ? progress4 : new Emoji("#️⃣")
        }.ToArray();

        dictionary = new Dictionary<string, IEmote>(StringComparer.OrdinalIgnoreCase)
        {
            {"Adventure", Adventure},
            {"Aid Your Ally", AidYourAlly},
            {"Ask The Oracle", AskTheOracle},
            {"Burn Momentum", BurnMomentum},
            {"Check Your Gear", CheckYourGear},
            {"Combat", Combat},
            {"Companion Takes A Hit", CompanionTakesAHit},
            {"Compel", Compel},
            {"Connection", Connection},
            {"Endure Harm", EndureHarm},
            {"Endure Stress", EndureStress},
            {"Exploration", Exploration},
            {"Face Death", FaceDeath},
            {"Face Desolation", FaceDesolation},
            {"Fate", Fate},
            {"Gather Information", GatherInformation},
            {"Legacy", Legacy},
            {"Lose Momentum ", LoseMomentum },
            {"Overcome Destruction", OvercomeDestruction},
            {"Quest", Quest},
            {"Recommit", Recommit},
            {"Recovery", Recovery},
            {"Reference", Reference},
            {"Roll", Roll},
            {"Sacrifice Resources", SacrificeResources},
            {"Suffer", Suffer},
            {"Threshold", Threshold},
            {"Withstand Damage", WithstandDamage },
            {"Mark Progress", MarkProgress },
        };
    }

    public IEmote Adventure { get; }
    public IEmote AidYourAlly { get; }
    public IEmote AskTheOracle { get; }
    public IEmote BurnMomentum { get; }
    public IEmote CheckYourGear { get; }
    public IEmote Combat { get; }
    public IEmote CompanionTakesAHit { get; }
    public IEmote Compel { get; }
    public IEmote Connection { get; }
    public IEmote EndureHarm { get; }
    public IEmote EndureStress { get; }
    public IEmote Exploration { get; }
    public IEmote FaceDeath { get; }
    public IEmote FaceDesolation { get; }
    public IEmote Fate { get; }
    public IEmote GatherInformation { get; }
    public IEmote Legacy { get; }
    public IEmote LoseMomentum { get; }
    public IEmote MarkProgress { get; }
    public IEmote OvercomeDestruction { get; }
    public IEmote[] ProgressEmotes { get; }
    public IEmote Quest { get; }
    public IEmote Recommit { get; }
    public IEmote Recovery { get; }
    public IEmote Reference { get; }
    public IEmote Roll { get; }
    public IEmote SacrificeResources { get; }
    public IEmote Suffer { get; }
    public IEmote Threshold { get; }
    public IEmote WithstandDamage { get; }

    public IEmote DecreaseProgress { get; }

    public Dictionary<string, IEmote> AsDictionary()
    {
        return dictionary;
    }
}
