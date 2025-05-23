namespace Server.GameInterfaces.DTOs
{
    public class InputDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AssetInputDTOType InputType { get; set; }
        public object Value { get; set; } // Using object for Value to accommodate different types
    }
}
