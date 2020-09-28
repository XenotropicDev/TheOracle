namespace TheOracle
{
    public interface IOracleEntry
    {
        int Chance { get; set; }
        string Description { get; set; }
    }
    public enum OracleType 
    {
        standard,
        nested,
        multipleColumns,
        paired
    }
}