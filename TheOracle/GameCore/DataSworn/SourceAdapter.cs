using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.DataSworn
{
    public class SourceAdapter : SourceInfo
    {
        public SourceAdapter(Source source)
        {
            Name = source.Name;
            Page = source.Page;
            Date = source.Date;
            Url = source.Url;
        }
    }
}