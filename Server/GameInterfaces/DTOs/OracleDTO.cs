using System.Collections.Generic; // Required for List

namespace Server.GameInterfaces.DTOs
{
    public class OracleDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public SourceDTO Source { get; set; }
        public List<OracleTableEntryDTO> Table { get; set; }
        public List<OracleDTO> Oracles { get; set; } // For sub-oracles
        public List<string> Aliases { get; set; }

        public OracleDTO()
        {
            Table = new List<OracleTableEntryDTO>();
            Oracles = new List<OracleDTO>();
            Aliases = new List<string>();
        }
    }
}
