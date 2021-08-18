using TheOracle.GameCore.DataSourceInfo;
using TheOracle.GameCore.RulesReference;

namespace TheOracle.GameCore
{
    public class SourceInfo : ISourceInfo
    {
        public string Name { get; set; }
        public string Page { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }

        public override string ToString()
        {
            if (Name.Length == 0) Name = SourceInfoResources.Unknown;

            string pageDisplay = string.Empty;
            if (Url?.Length > 0 && Page?.Length > 0) pageDisplay = $"[{Page}]({Url})";
            if (pageDisplay.Length == 0 && Page?.Length > 0) pageDisplay = string.Format(SourceInfoResources.SourcePageField, Page);

            string versionDisplay = (Version?.Length > 0) ? string.Format(SourceInfoResources.VersionField, Version) : string.Empty;

            return string.Format(SourceInfoResources.SourceField, Name, pageDisplay, versionDisplay);
        }
    }
}