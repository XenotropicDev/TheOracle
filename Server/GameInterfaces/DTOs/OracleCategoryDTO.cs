using System.Collections.Generic; // Required for List

namespace Server.GameInterfaces.DTOs
{
    public class OracleCategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<OracleDTO> Oracles { get; set; }
        public SourceDTO Source { get; set; }
        public List<string> Aliases { get; set; }

        public OracleCategoryDTO()
        {
            Oracles = new List<OracleDTO>();
            Aliases = new List<string>();
        }
    }
}
