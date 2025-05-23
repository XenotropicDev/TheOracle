using System.Collections.Generic;

namespace Server.GameInterfaces.DTOs
{
    public class AssetDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AssetType { get; set; }
        public List<AbilityDTO> Abilities { get; set; }
        public ConditionMeterDTO ConditionMeter { get; set; }
        public List<InputDTO> Inputs { get; set; }
        public SourceDTO Source { get; set; }
    }
}
