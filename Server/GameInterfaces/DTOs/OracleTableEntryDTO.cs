namespace Server.GameInterfaces.DTOs
{
    public class OracleTableEntryDTO
    {
        public string Id { get; set; } // Added Id property
        public int? Floor { get; set; }
        public int? Ceiling { get; set; }
        public string ResultText { get; set; }

        public OracleTableEntryDTO()
        {
            // Default constructor
        }
    }
}
