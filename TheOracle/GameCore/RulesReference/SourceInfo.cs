namespace TheOracle.GameCore.RulesReference
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
            string page = (Page?.Length > 0) ? string.Format(RulesResources.SourcePageField, Page) : string.Empty;
            return string.Format(RulesResources.SourceField, Name, page);
        }
    }
}