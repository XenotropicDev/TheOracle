namespace Server.GameInterfaces.DTOs
{
    public class MoveDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        public MoveTriggerDTO Trigger { get; set; }
        public MoveOutcomesDTO Outcomes { get; set; }
        public SourceDTO Source { get; set; }
        public bool Optional { get; set; }
        public bool? ProgressMove { get; set; } // Nullable bool
        public string VariantOf { get; set; } // This can be null if it's not a variant
    }
}
