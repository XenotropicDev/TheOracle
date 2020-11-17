namespace TheOracle.GameCore
{
    public class SourceInfo : ISourceInfo
    {
        public string Name { get; set; }
        public string Page { get; set; }
        public string URL { get; set; }

        public override string ToString()
        {
            if (Name.Length == 0) Name = RulesResources.Unknown;
            if (URL?.Length > 0)
            {
                return string.Format(RulesResources.SourceField, Name, $"[{Page}]({URL})");
            }
            return string.Format(RulesResources.SourceField, Name, Page);
        }
    }
}