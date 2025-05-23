using System.Collections.Generic; // Required for List

namespace Server.GameInterfaces.DTOs
{
    public class OracleRootDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SourceDTO Source { get; set; }
        public List<OracleCategoryDTO> Categories { get; set; }
        public List<OracleDTO> Oracles { get; set; } // Top-level oracles
        public List<string> Aliases { get; set; }

        public OracleRootDTO()
        {
            Categories = new List<OracleCategoryDTO>();
            Oracles = new List<OracleDTO>();
            Aliases = new List<string>();
        }
    }
}
